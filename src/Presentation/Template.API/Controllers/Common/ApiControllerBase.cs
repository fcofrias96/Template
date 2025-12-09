using Microsoft.AspNetCore.Mvc;
using Template.Application.Models.DTOs;
using Template.Application.Models.Wrappers;

namespace Template.Api.Controllers.Common
{
    public abstract class ApiControllerBase : ControllerBase
    {
        protected ActionResult< ApiResponse<IReadOnlyList<T>>> FromEtagResult<T>(
            EtagListResult<T> result)
        {
            if (!string.IsNullOrWhiteSpace(result.Etag))
            {
                Response.Headers.ETag = result.Etag;
            }

            if (result.IsNotModified)
            {
                return StatusCode(StatusCodes.Status304NotModified);
            }

            var apiResponse = new ApiResponse<IReadOnlyList<T>>(result.Items!);
            return Ok(apiResponse);
        }
    }

}
