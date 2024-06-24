using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Template.Application.Models.Wrappers;

namespace Template.Application.SetupOptions
{
    public static class ApiBehavior
    {
        public static readonly Action<ApiBehaviorOptions> Options = options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState
                    .Where(ms => ms.Value!.Errors.Any())
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                var apiResponse = new ApiResponse(StatusCodes.Status400BadRequest, "One or more validation errors occurred.", errors);

                return new BadRequestObjectResult(apiResponse);
            };
        };
    }
}