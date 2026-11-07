using Microsoft.AspNetCore.Mvc;
using Template.BuildingBlocks.Application.Models.Wrappers;
using Template.BuildingBlocks.Web.Results;
using Template.SharedKernel.Results;

namespace Template.BuildingBlocks.Web.Extensions
{
    /// <summary>Turns service <see cref="Result"/>s into MVC action results.</summary>
    public static class ResultExtensions
    {
        public static ActionResult Match(this Result result, Func<ActionResult> onSuccess)
            => result.IsSuccess ? onSuccess() : result.ToProblem();

        public static ActionResult Match<T>(this Result<T> result, Func<T, ActionResult> onSuccess)
            => result.IsSuccess ? onSuccess(result.Value) : result.ToProblem();

        /// <summary>204 No Content on success, problem details otherwise.</summary>
        public static ActionResult ToNoContent(this Result result)
            => result.Match(() => new NoContentResult());

        /// <summary>200 OK with the value wrapped in <see cref="ApiResponse{T}"/> on success, problem details otherwise.</summary>
        public static ActionResult ToOk<T>(this Result<T> result)
            => result.Match(value => new OkObjectResult(new ApiResponse<T>(value)));

        public static ActionResult ToProblem(this Result result)
        {
            if (result.IsSuccess)
            {
                throw new InvalidOperationException("A successful result cannot be converted to a problem response.");
            }

            return new ErrorResult(result.Error);
        }
    }
}
