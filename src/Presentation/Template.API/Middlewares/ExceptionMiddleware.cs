using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using Serilog.Context;
using System.Diagnostics;
using Template.Application.Enums;
using Template.Application.Exceptions;
using Template.Application.Extensions;
using Template.Application.Models.Wrappers;
using ILogger = Serilog.ILogger;

namespace Template.Api.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly ILogger _logger;
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next, ILogger logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var request = context.Request;
            var clientIP = context.Connection.RemoteIpAddress;
            var traceId = context.TraceIdentifier;
            var stopwatch = Stopwatch.StartNew();

            using (LogContext.PushProperty("ClientIP", clientIP))
            using (LogContext.PushProperty("Referer", request.Headers["Referer"]))
            using (LogContext.PushProperty("RequestMethod", request.Method))
            using (LogContext.PushProperty("RequestPath", request.Path))
            using (LogContext.PushProperty("RequestProtocol", request.Protocol))
            using (LogContext.PushProperty("RequestQueryString", request.QueryString))
            using (LogContext.PushProperty("UserAgent", request.Headers["User-Agent"]))
            using (LogContext.PushProperty("UserName", context.GetUserName()))
            using (LogContext.PushProperty("TraceId", traceId))
            {
                try
                {
                    await _next(context);
                    stopwatch.Stop();
                    var elapsed = stopwatch.Elapsed.TotalMilliseconds;
                    using (LogContext.PushProperty("Elapsed", elapsed))
                    {
                        _logger.Information("Request processed");
                    }
                }
                catch (OperationCanceledException ex)
                {
                    stopwatch.Stop();
                    var elapsed = stopwatch.Elapsed.TotalMilliseconds;
                    using (LogContext.PushProperty("Elapsed", elapsed))
                    {
                        _logger.Warning(ex, "Request cancelled by the client.");
                    }

                    if (!context.Response.HasStarted)
                    {
                        context.Response.StatusCode = StatusCodes.Status499ClientClosedRequest;
                        await context.Response.WriteAsync("Request was cancelled.");
                    }
                    return;
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    var elapsed = stopwatch.Elapsed.TotalMilliseconds;
                    var exception = ex.InnerException ?? ex;

                    int statusCode;
                    string message;
                    string title;
                    string errorCode;

                    #region StatusCode & Titles

                    switch (exception)
                    {
                        case BadRequestException:
                            statusCode = StatusCodes.Status400BadRequest;
                            title = "Bad Request";
                            message = exception.Message;
                            errorCode = "BAD_REQUEST";
                            break;

                        case SecurityTokenException:
                            statusCode = StatusCodes.Status401Unauthorized;
                            title = "Unauthorized";
                            message = "El token de autenticación no es válido o ha expirado.";
                            errorCode = "UNAUTHORIZED";
                            break;

                        case ForbiddenException:
                            statusCode = StatusCodes.Status403Forbidden;
                            title = "Forbidden";
                            message = "No tiene permiso para realizar esta acción.";
                            errorCode = "FORBIDDEN";
                            break;

                        case NotFoundException:
                            statusCode = StatusCodes.Status404NotFound;
                            title = "Not Found";
                            message = exception.Message;
                            errorCode = "NOT_FOUND";
                            break;

                        case SqlException sqlEx:
                            switch ((SqlErrorCodes)sqlEx.Number)
                            {
                                case SqlErrorCodes.UniqueConstraintViolation:
                                case SqlErrorCodes.UniqueIndexViolation:
                                    statusCode = StatusCodes.Status409Conflict;
                                    title = "Conflict";
                                    message = "Ya existe un registro con los mismos datos.";
                                    errorCode = "DUPLICATE_ENTRY";
                                    break;

                                case SqlErrorCodes.ForeignKeyViolation:
                                    statusCode = StatusCodes.Status400BadRequest;
                                    title = "Foreign Key Violation";
                                    message = "No se puede agregar, actualizar o eliminar el registro debido a restricciones de integridad referencial.";
                                    errorCode = "FOREIGN_KEY_VIOLATION";
                                    break;

                                default:
                                    statusCode = StatusCodes.Status500InternalServerError;
                                    title = "Database Error";
                                    message = "Ocurrió un error en la base de datos.";
                                    errorCode = "DATABASE_ERROR";
                                    break;
                            }
                            break;

                        default:
                            statusCode = StatusCodes.Status500InternalServerError;
                            title = "Internal Server Error";
                            message = "Ocurrió un problema en el servidor, por favor contacte al administrador.";
                            errorCode = "INTERNAL_SERVER_ERROR";
                            break;
                    }

                    #endregion

                    using (LogContext.PushProperty("StatusCode", statusCode))
                    using (LogContext.PushProperty("Elapsed", elapsed))
                    {
                        _logger.Error(ex, message);
                    }

                    if (!context.Response.HasStarted)
                    {
                        var problem = new ProblemDetails
                        {
                            Status = statusCode,
                            Title = title,
                            Detail = message,
                            Instance = context.Request.Path
                        };


                        problem.Extensions["traceId"] = context.TraceIdentifier;
                        problem.Extensions["errorCode"] = errorCode;

                        context.Response.Clear();
                        context.Response.StatusCode = statusCode;
                        context.Response.ContentType = "application/problem+json";

                        await context.Response.WriteAsJsonAsync(problem);
                    }
                    else
                    {
                        _logger.Warning("Response has already started, cannot write error response.");
                    }
                }
            }
        }
    }
}
