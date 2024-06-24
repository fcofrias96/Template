using Serilog.Context;
using System.Diagnostics;
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
            var stopwatch = Stopwatch.StartNew();

            using (LogContext.PushProperty("ClientIP", clientIP))
            using (LogContext.PushProperty("Referer", request.Headers["Referer"]))
            using (LogContext.PushProperty("RequestMethod", request.Method))
            using (LogContext.PushProperty("RequestPath", request.Path))
            using (LogContext.PushProperty("RequestProtocol", request.Protocol))
            using (LogContext.PushProperty("RequestQueryString", request.QueryString))
            using (LogContext.PushProperty("UserAgent", request.Headers["User-Agent"]))
            using (LogContext.PushProperty("UserName", context.GetUserName()))
            {
                try
                {
                    await _next(context);
                    stopwatch.Stop();
                    var elapsed = stopwatch.ElapsedMilliseconds.ToString("0.0000");
                    LogContext.PushProperty("Elapsed", elapsed);
                    _logger.Information("Request processed");
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    int statusCode;
                    var exception = ex.InnerException ?? ex;
                    string message = exception.Message;

                    #region StatusCode

                    switch (ex)
                    {
                        case BadRequestException:
                            statusCode = StatusCodes.Status400BadRequest;
                            break;

                        case ForbiddenException:
                            statusCode = StatusCodes.Status403Forbidden;
                            break;

                        case NotFoundException:
                            statusCode = StatusCodes.Status404NotFound;
                            break;

                        default:
                            statusCode = StatusCodes.Status500InternalServerError;
                            message = "A problem occurred on the server, please contact the administrator";
                            break;
                    }

                    #endregion StatusCode

                    var elapsed = stopwatch.ElapsedMilliseconds.ToString("0.0000");
                    LogContext.PushProperty("StatusCode", statusCode);
                    LogContext.PushProperty("Elapsed", elapsed);
                    _logger.Error(ex, message);

                    var response = new ApiResponse(statusCode, message);

                    context.Response.StatusCode = statusCode;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsJsonAsync(response);
                }
            }
        }
    }
}