using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Template.BuildingBlocks.Application.Enums;
using Template.BuildingBlocks.Application.Extensions;

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

        // Extra InvokeAsync parameters are resolved from DI, so the request's token comes from context.RequestAborted.
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
                    ["UserName"] = context.GetUserId()
                }))
                {
                    _logger.LogWarning(ex, "Request cancelled by the client.");
                }

                if (!context.Response.HasStarted)
                {
                    // 499 no está “oficial” en ASP.NET, pero es común para client closed request
                    context.Response.StatusCode = StatusCodes.Status499ClientClosedRequest;
                    // Not RequestAborted: it is already cancelled here, so the write would throw.
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

                // Expected business failures travel as Result/Error and are written by the controllers
                // (see ResultExtensions). This middleware only handles what is genuinely exceptional.
                switch (exception)
                {
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

                #endregion StatusCode & Titles

                var request = context.Request;
                var clientIP = context.Connection.RemoteIpAddress;

                using (_logger.BeginScope(new Dictionary<string, object>
                {
                    ["TraceId"] = context.TraceIdentifier,
                    ["UserId"] = context.GetUserId(),
                    ["ClientIP"] = clientIP?.ToString() ?? "Unknown",
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

                    await context.Response.WriteAsJsonAsync(problem, context.RequestAborted);
                }
                else
                {
                    _logger.LogWarning("Response has already started, cannot write error response.");
                }
            }
        }
    }
}
