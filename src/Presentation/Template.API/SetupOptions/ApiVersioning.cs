using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;

namespace Template.Api.SetupOptions
{
    public static class ApiVersioning
    {
        public static readonly Action<ApiVersioningOptions> Options = options =>
        {
            options.ReportApiVersions = true;
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.DefaultApiVersion = new ApiVersion(1, 0);
        };
    }
}
