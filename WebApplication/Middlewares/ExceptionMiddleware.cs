using Serilog.Context;
using Template.Core.Application.Exceptions;
using Template.Core.Application.Extensions;
using Template.Core.Application.Wrappers;
using ILogger = Serilog.ILogger;

namespace WebApi.Middlewares
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
            try
            {
                await _next(context);
            }
            catch (Exception e)
            {
                #region StatusCode

                switch (e)
                {
                    case ApiException:
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                        break;

                    case KeyNotFoundException:
                        context.Response.StatusCode = StatusCodes.Status404NotFound;
                        break;

                    default:
                        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                        break;
                }

                #endregion StatusCode

                var userName = context.GetUserName();
                LogContext.PushProperty("UserName", userName);
                _logger.Error(e, e.Message);

                var response = new Response<string>(e.Message);
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }
}