using Microsoft.AspNetCore.Http;

namespace Template.Core.Application.Extensions
{
    public static class HttpRequestExtension
    {
        public static string GetBaseUrl(this HttpRequest httpRequest)
        {
            var host = httpRequest.Host.Value;
            var scheme = httpRequest.Scheme;
            return $"{scheme}://{host}/api/";
        }
    }
}