using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Template.Modules.Identity.Infrastructure.TokenProviderConfigurations
{
    public abstract class BaseTokenProvider<TUser> : DataProtectorTokenProvider<TUser> where TUser : class
    {
        protected readonly ILogger<DataProtectorTokenProvider<TUser>> _logger;
        protected readonly DataProtectionTokenProviderOptions _options;

        protected BaseTokenProvider(IDataProtectionProvider dataProtectionProvider,
            IOptions<DataProtectionTokenProviderOptions> options,
            ILogger<DataProtectorTokenProvider<TUser>> logger) : base(dataProtectionProvider, options, logger)
        {
            _logger = logger;
            _options = options.Value;
        }

        public override async Task<bool> ValidateAsync(string purpose, string token, UserManager<TUser> manager, TUser user)
        {
            var unprotectedData = Protector.Unprotect(Convert.FromBase64String(token));
            var ms = new MemoryStream(unprotectedData);
            using (var reader = new BinaryReader(ms))
            {
                var creationTime = new DateTimeOffset(reader.ReadInt64(), TimeSpan.Zero);
                var expirationTime = creationTime + _options.TokenLifespan;
                if (expirationTime < DateTimeOffset.UtcNow)
                {
                    _logger.LogWarning("The token has expired.");
                    throw new SecurityTokenException("The token has expired.");
                }

                var userId = reader.ReadString();
                var actualUserId = await manager.GetUserIdAsync(user);
                if (userId != actualUserId)
                {
                    _logger.LogWarning("User IDs do not match.");
                    throw new SecurityTokenException("Invalid token.");
                }

                var purp = reader.ReadString();
                if (!string.Equals(purp, purpose))
                {
                    _logger.LogWarning("Token purpose mismatch: expected {ExpectedPurpose}, actual {ActualPurpose}.", purpose, purp);
                    throw new SecurityTokenException("Invalid token.");
                }

                var stamp = reader.ReadString();
                if (reader.PeekChar() != -1)
                {
                    _logger.LogWarning("Unexpected end of input.");
                    throw new SecurityTokenException("Invalid token.");
                }

                if (manager.SupportsUserSecurityStamp)
                {
                    var isEqualsSecurityStamp = stamp == await manager.GetSecurityStampAsync(user);
                    if (!isEqualsSecurityStamp)
                    {
                        _logger.LogWarning("Security stamp mismatch.");
                    }

                    return isEqualsSecurityStamp;
                }

                var stampIsEmpty = stamp == "";
                if (!stampIsEmpty)
                {
                    _logger.LogWarning("Security stamp is not empty.");
                }

                return stampIsEmpty;
            }
        }
    }
}
