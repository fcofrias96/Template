using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace Template.Application.SetupOptions
{
    public static class ApiExplorer
    {
        public static readonly Action<ApiExplorerOptions> Options = options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        };
    }
}