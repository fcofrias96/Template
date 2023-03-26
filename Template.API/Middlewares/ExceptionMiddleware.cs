using Serilog.Context;
using Template.Core.Application.Extensions;
using Template.Core.Application.Wrappers;
using Template.Core.Domain.Exceptions;
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
            try
            {
                await _next(context);
            }
            catch (Exception e)
            {
                #region StatusCode

                switch (e)
                {
                    case BadRequestException:
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                        break;

                    case NotFoundException:
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