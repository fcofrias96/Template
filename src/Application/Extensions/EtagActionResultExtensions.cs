using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Template.Application.Caching.Models;
using Template.Application.Models.Wrappers;

namespace Template.Application.Extensions
{
    public static class EtagActionResultExtensions
    {
        public static ActionResult ToActionResult<T>(this EtagResult<T> result)
        {
            if (result.IsNotModified)
                return new StatusCodeResult(StatusCodes.Status304NotModified);

            return new OkObjectResult( new ApiResponse<T>(result.Data!));
        }
    }
}