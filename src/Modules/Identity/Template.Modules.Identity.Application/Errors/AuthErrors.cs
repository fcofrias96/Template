using Microsoft.AspNetCore.Identity;
using Template.SharedKernel.Results;

namespace Template.Modules.Identity.Application.Errors
{
    public static class AuthErrors
    {
        // ---------- Login ----------

        // Same message whether the email or the password is wrong, so accounts cannot be enumerated.
        public static readonly Error InvalidCredentials = Error.Validation(
            "Auth.InvalidCredentials",
            "The email or password is incorrect. Please try again.");

        public static Error EmailNotConfirmed(string email) => Error.Validation(
            "Auth.EmailNotConfirmed",
            $"Email for user '{email}' is not confirmed.");

        public static Error AccountLocked(string email) => Error.Validation(
            "Auth.AccountLocked",
            $"User account '{email}' is locked.");

        public static Error AccountInactive(string email) => Error.Validation(
            "Auth.AccountInactive",
            $"User account '{email}' is inactivated.");

        public static Error LockedOut(string email) => Error.Validation(
            "Auth.LockedOut",
            $"User account '{email}' locked out.");

        public static Error LoginNotAllowed(string email) => Error.Validation(
            "Auth.LoginNotAllowed",
            $"Login not allowed for user '{email}'.");

        public static Error InvalidLoginAttempt(string email) => Error.Validation(
            "Auth.InvalidLoginAttempt",
            $"Invalid login attempt for user '{email}'.");

        public static readonly Error NotAuthenticated = Error.Unauthorized(
            "Auth.NotAuthenticated",
            "The current request is not authenticated.");

        // ---------- Refresh token ----------

        public static readonly Error RefreshTokenMissing = Error.Forbidden(
            "Auth.RefreshTokenMissing",
            "No refresh token provided");

        public static readonly Error RefreshTokenInvalid = Error.Forbidden(
            "Auth.RefreshTokenInvalid",
            "Invalid refresh token");

        // ---------- Registration ----------

        public static Error UserNameTaken(string userName) => Error.Validation(
            "Auth.UserNameTaken",
            $"The username '{userName}' is already taken. Please choose a different username.");

        public static Error EmailTaken(string email) => Error.Validation(
            "Auth.EmailTaken",
            $"The email address '{email}' is already registered. Please use a different email address.");

        public static Error RegistrationFailed(IdentityResult result) => Error.Validation(
            "Auth.RegistrationFailed",
            $"User creation failed: {result.Describe()}");

        // ---------- Email / password tokens ----------

        public static readonly Error InvalidToken = Error.Validation(
            "Auth.InvalidToken",
            "The token is invalid or has expired.");

        public static readonly Error EmailAlreadyConfirmed = Error.Validation(
            "Auth.EmailAlreadyConfirmed",
            "Email is already confirmed.");

        public static Error EmailConfirmationFailed(IdentityResult result) => Error.Validation(
            "Auth.EmailConfirmationFailed",
            $"Email confirmation failed: {result.Describe()}");

        public static Error PasswordResetFailed(IdentityResult result) => Error.Validation(
            "Auth.PasswordResetFailed",
            $"Password reset failed: {result.Describe()}");

        // ---------- Change email ----------

        public static readonly Error IncorrectPassword = Error.Validation(
            "Auth.IncorrectPassword",
            "Incorrect password.");

        public static readonly Error SameEmail = Error.Validation(
            "Auth.SameEmail",
            "New email address is the same as the current email address.");

        public static Error ChangeEmailFailed(IdentityResult result) => Error.Validation(
            "Auth.ChangeEmailFailed",
            $"Error changing email: {result.Describe()}");
    }
}
