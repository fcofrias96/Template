using Microsoft.AspNetCore.Http;

namespace Template.Application.Etag
{
    public sealed class EtagHttpResponder : IEtagHttpResponder
    {
        private readonly IHttpContextAccessor _http;
        public EtagHttpResponder(IHttpContextAccessor http) => _http = http;

        public string? GetClientEtag()
            => _http.HttpContext?.Request.Headers["If-None-Match"].FirstOrDefault();

        public bool Apply(string etag, string? clientEtag = null)
        {
            var ctx = _http.HttpContext;
            if (ctx is null) return false;

            // Siempre setear el ETag de respuesta
            ctx.Response.Headers["ETag"] = etag;

            clientEtag ??= GetClientEtag();
            if (!string.IsNullOrWhiteSpace(clientEtag) &&
                string.Equals(clientEtag, etag, StringComparison.Ordinal))
            {
                ctx.Response.StatusCode = StatusCodes.Status304NotModified;
                return true;
            }

            return false;
        }
    }
}