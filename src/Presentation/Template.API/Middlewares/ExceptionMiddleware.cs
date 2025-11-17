using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using Template.Application.Enums;
using Template.Application.Exceptions;
using Template.Application.Extensions;


namespace Template.Api.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (OperationCanceledException ex)
            {
                // Petición cancelada por el cliente (ej: abortar en el browser)
                using (_logger.BeginScope(new Dictionary<string, object>
                {
                    ["TraceId"] = context.TraceIdentifier,
                    ["UserName"] = context.GetUserName()
                }))
                {
                    _logger.LogWarning(ex, "Request cancelled by the client.");
                }

                if (!context.Response.HasStarted)
                {
                    // 499 no está “oficial” en ASP.NET, pero es común para client closed request
                    context.Response.StatusCode = StatusCodes.Status499ClientClosedRequest;
                    await context.Response.WriteAsync("Request was cancelled.");
                }

                return;
            }
            catch (Exception ex)
            {
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

                var request = context.Request;
                var clientIP = context.Connection.RemoteIpAddress ;

                using (_logger.BeginScope(new Dictionary<string, object>
                {
                    ["TraceId"] = context.TraceIdentifier,
                    ["UserName"] = context.GetUserName(),
                    ["ClientIP"] = clientIP,
                    ["RequestMethod"] = request.Method,
                    ["RequestPath"] = request.Path,
                    ["RequestQueryString"] = request.QueryString.ToString(),
                    ["StatusCode"] = statusCode,
                    ["ErrorCode"] = errorCode
                }))

                {
                    _logger.LogError(ex, "Unhandled exception: {ErrorCode} - {Message}", errorCode, message);
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
                    _logger.LogWarning("Response has already started, cannot write error response.");
                }
            }
        }
    }
}
