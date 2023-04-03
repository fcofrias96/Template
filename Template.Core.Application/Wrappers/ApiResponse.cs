using Template.Core.Application.DTOs.Item;
using Template.Core.Domain.Entities;

namespace Template.Core.Application.Wrappers
{
    public class ApiResponse<TSource>
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public TSource Data { get; set; }
        public Dictionary<string, string[]> Errors { get; set; } = new Dictionary<string, string[]>();
        public bool Success => StatusCode >= 200 && StatusCode < 300;

        public ApiResponse(int statusCode, string message = null, TSource data = default, Dictionary<string, string[]> errors = default)
        {
            StatusCode = statusCode;
            Message = message;
            Data = data;
            Errors = errors;
        }

        public ApiResponse()
        {
        }

        public static ApiResponse<TSource> Ok(TSource data = default, string message = default)
        {
            return new ApiResponse<TSource>(200, message, data);
        }

        public static ApiResponse<TSource> Created(TSource data = default, string message = null)
        {
            return new ApiResponse<TSource>(201, message, data);
        }

        public static ApiResponse<TSource> BadRequest(Dictionary<string, string[]> errors = null, string message = null)
        {
            return new ApiResponse<TSource>(400, message, default, errors);
        }

        public static ApiResponse<TSource> NotFound(string message = null)
        {
            return new ApiResponse<TSource>(404, message);
        }

        public static ApiResponse<TSource> InternalServerError(string message = null)
        {
            return new ApiResponse<TSource>(500, message);
        }
    }
}