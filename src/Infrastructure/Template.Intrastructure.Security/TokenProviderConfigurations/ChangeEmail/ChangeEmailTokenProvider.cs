using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Template.Security.TokenProviderConfigurations.ChangeEmail
{
    public class ChangeEmailTokenProvider<TUser> : BaseTokenProvider<TUser> where TUser : class
    {
        public ChangeEmailTokenProvider(IDataProtectionProvider dataProtectionProvider,
            IOptions<ChangeEmailTokenProviderOptions> options,
            ILogger<DataProtectorTokenProvider<TUser>> logger) : base(dataProtectionProvider, options, logger)
        {
        }
    }
}