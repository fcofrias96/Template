using System.Security.Cryptography;

namespace Template.Api.Middlewares
{
    public class ETagMiddleware
    {
        private readonly RequestDelegate _next;

        public ETagMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Method != HttpMethods.Get)
            {
                await _next(context);
                return;
            }

            var response = context.Response;
            var originalBodyStream = response.Body;

            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    response.Body = memoryStream;

                    await _next(context);

                    if (response.StatusCode == StatusCodes.Status200OK && memoryStream.Length > 0)
                    {
                        var checksum = GenerateETag(memoryStream);

                        if (context.Request.Headers.TryGetValue("If-None-Match", out var requestETag) && requestETag == checksum)
                        {
                            response.StatusCode = StatusCodes.Status304NotModified;
                            response.Body = originalBodyStream;
                            return;
                        }

                        response.Headers["ETag"] = checksum;
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        await memoryStream.CopyToAsync(originalBodyStream);
                    }
                    else
                    {
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        await memoryStream.CopyToAsync(originalBodyStream);
                    }
                }
            }
            finally
            {
                response.Body = originalBodyStream;
            }
        }

        static private string GenerateETag(Stream memStream)
        {
            memStream.Seek(0, SeekOrigin.Begin);
            using (var sha256 = SHA256.Create())
            {
                var hash = sha256.ComputeHash(memStream);
                return Convert.ToBase64String(hash);
            }
        }
    }
}