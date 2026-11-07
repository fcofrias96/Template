using Microsoft.AspNetCore.Identity;

namespace Template.Modules.Identity.Application.Errors
{
    internal static class IdentityResultExtensions
    {
        /// <summary>Joins the descriptions of every error of a failed <see cref="IdentityResult"/>.</summary>
        public static string Describe(this IdentityResult result)
            => string.Join("; ", result.Errors.Select(e => e.Description));
    }
}
