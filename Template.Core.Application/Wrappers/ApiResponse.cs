
using System.Text.Json.Serialization;

namespace Template.Core.Application.Wrappers
{
    public class ApiResponse<TSource>
    {
        public int StatusCode { get; set; } = 200;
        public string Message { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public TSource Data { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Dictionary<string, string[]> Errors { get; set; }
        public bool Success => StatusCode >= 200 && StatusCode < 300;

        public ApiResponse( TSource data )
        {

            Data = data;
            Message = "Successful Operation";
        }


        public ApiResponse(int statusCode, string message)
        {
            StatusCode = statusCode;
            Message = message;
        }

        public ApiResponse(int statusCode, string message, Dictionary<string, string[]> errors)
        {
            StatusCode = statusCode;
            Message = message;
            Errors = errors;
        }

        public ApiResponse()
        {
        }

        
    }
}