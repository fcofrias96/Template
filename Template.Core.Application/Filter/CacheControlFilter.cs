using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Template.Core.Application.Filter
{
    public class CacheControlFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.HttpContext.Response.Headers.TryGetValue("X-Validate-ETag", out var validateETag))
            {
                context.HttpContext.Response.Headers.Remove("X-Validate-ETag");

                if (context.HttpContext.Request.Headers.TryGetValue("If-None-Match", out var requestETag) &&
                    requestETag == validateETag)
                {
                    context.Result = new StatusCodeResult(StatusCodes.Status304NotModified);
                }
                else
                {
                    context.HttpContext.Response.Headers.Add("ETag", validateETag);
                    context.HttpContext.Response.Headers.Add("Cache-Control", "no-cache, must-revalidate");
                }
            }
        }

    }
}
