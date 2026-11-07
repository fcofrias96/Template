using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Template.SharedKernel.Results;

namespace Template.BuildingBlocks.Web.Results
{
    /// <summary>
    /// Writes an <see cref="Error"/> as <c>application/problem+json</c>, with the same shape the
    /// exception middleware uses (status, title, detail, instance, traceId, errorCode).
    /// </summary>
    public sealed class ErrorResult : ActionResult
    {
        public ErrorResult(Error error)
        {
            Error = error;
        }

        public Error Error { get; }

        public override Task ExecuteResultAsync(ActionContext context)
        {
            var httpContext = context.HttpContext;
            var (statusCode, title) = MapStatus(Error.Type);

            var problem = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = Error.Description,
                Instance = httpContext.Request.Path
            };

            problem.Extensions["traceId"] = httpContext.TraceIdentifier;
            problem.Extensions["errorCode"] = Error.Code;

            var result = new ObjectResult(problem)
            {
                StatusCode = statusCode,
                ContentTypes = { "application/problem+json" }
            };

            return result.ExecuteResultAsync(context);
        }

        private static (int StatusCode, string Title) MapStatus(ErrorType type) => type switch
        {
            ErrorType.Validation => (StatusCodes.Status400BadRequest, "Bad Request"),
            ErrorType.Unauthorized => (StatusCodes.Status401Unauthorized, "Unauthorized"),
            ErrorType.Forbidden => (StatusCodes.Status403Forbidden, "Forbidden"),
            ErrorType.NotFound => (StatusCodes.Status404NotFound, "Not Found"),
            ErrorType.Conflict => (StatusCodes.Status409Conflict, "Conflict"),
            _ => (StatusCodes.Status500InternalServerError, "Internal Server Error")
        };
    }
}
