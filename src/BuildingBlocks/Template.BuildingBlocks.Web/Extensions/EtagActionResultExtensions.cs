using Template.BuildingBlocks.Application.Caching.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Template.BuildingBlocks.Application.Models.Wrappers;

namespace Template.BuildingBlocks.Web.Extensions
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
