using Microsoft.AspNetCore.Identity;
using Template.Infrastructure.Identity.Contracts.Repositories;
using Template.Infrastructure.Identity.Entity;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Template.Infrastructure.Identity.Repositories
{
    public class AuthRepositoryAsync : IAuthRepositoryAsync
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _singInManager;

        public AuthRepositoryAsync(UserManager<User> userManager, SignInManager<User> singInManager)
        {
            _userManager = userManager;
            _singInManager = singInManager;
        }

        public async Task<IdentityResult> ConfirmEmailAsync(User user, string code)
        {
            return await _userManager.ConfirmEmailAsync(user, code);
        }

        public async Task<string> GenerateEmailConfirmationTokenAsync(User user)
        {
            return await _userManager.GenerateEmailConfirmationTokenAsync(user);
        }

        public async Task<string> GeneratePasswordResetTokenAsync(User user)
        {
            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }

        public async Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure)
        {
            return await _singInManager.PasswordSignInAsync(userName, password, isPersistent, lockoutOnFailure: lockoutOnFailure);
        }

        public async Task<IdentityResult> ResetPasswordAsync(User user, string token, string newPassword)
        {
            return await _userManager.ResetPasswordAsync(user, token, newPassword);
        }
    }
}