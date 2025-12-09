using Microsoft.Extensions.Options;
using Template.Application.Contracts.Services.V1;
using Template.Application.Models.DTOs.V1.Emails;
using Template.Security.Contracts.Services;
using Template.Security.Entities;
using Template.Security.Settings;

public class AuthEmailService : IAuthEmailService
{
    private readonly IEmailServiceAsync _emailServiceAsync;
    private readonly JwtSettings _jwtSettings;

    public AuthEmailService(
        IEmailServiceAsync emailServiceAsync,
        IOptions<JwtSettings> jwtSettings)
    {
        _emailServiceAsync = emailServiceAsync;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task SendConfirmationEmailAsync(User user, string confirmationLink, CancellationToken cancellationToken)
    {
        var tokenDuration = _jwtSettings.EmailConfirmationTokenDurationInHours;
        string duration = $"{tokenDuration} hour{(tokenDuration >= 2 ? "s" : "")}";

        var emailDto = new EmailDto
        {
            To = user.Email,
            Subject = "Confirm Registration",
            Body = $@"
            <html>
            <body style='font-family: Arial, sans-serif; line-height: 1.6;'>
                <h2>Confirm Your Account</h2>
                <p>Dear {user.FirstName} {user.LastName},</p>
                <p>Thank you for registering. Please confirm your account by clicking the link below:</p>
                <p><a href='{confirmationLink}' style='color: #1a73e8;'>Confirm Registration</a></p>
                <p>This link is valid for {duration}.</p>
                <p>If you did not create an account, please ignore this email.</p>
                <p>Best regards,<br>Your Company Team</p>
            </body>
            </html>"
        };

        await _emailServiceAsync.SendEmailAsync(emailDto, cancellationToken);
    }

    public async Task SendChangeEmailNotificationAsync(
        User user,
        string newEmail,
        string changeEmailLink,
        CancellationToken cancellationToken)
    {
        var tokenDuration = _jwtSettings.ChangeEmailTokenDurationInHours;
        string duration = $"{tokenDuration} hour{(tokenDuration >= 2 ? "s" : "")}";

        var emailDto = new EmailDto
        {
            To = newEmail,
            Subject = "Confirm your new email address",
            Body = $@"
                <html>
                <body style='font-family: Arial, sans-serif; line-height: 1.6;'>
                    <h2>Confirm Your Email Address</h2>
                    <p>Dear {user.FirstName} {user.LastName},</p>
                    <p>You have requested to update your email address.</p>
                    <p>Please confirm your new email address by clicking the link below:</p>
                    <p><a href='{changeEmailLink}'>Confirm Email</a></p>
                    <p>This link is valid for {duration}.</p>
                    <p>If you did not request this change, please ignore this email or contact support.</p>
                    <p>Best regards,<br>Your Company Team</p>
                </body>
                </html>"
        };

        await _emailServiceAsync.SendEmailAsync(emailDto, cancellationToken);
    }

    public async Task SendForgotPasswordEmailAsync(
        User user,
        string resetPasswordUrl,
        CancellationToken cancellationToken)
    {
        var tokenDuration = _jwtSettings.PasswordResetTokenDurationInHours;
        string duration = $"{tokenDuration} hour{(tokenDuration >= 2 ? "s" : "")}";

        var emailRequest = new EmailDto
        {
            Body = $@"
                <html>
                <body style='font-family: Arial, sans-serif; line-height: 1.6;'>
                    <h2>Reset Your Password</h2>
                    <p>Dear {user.FirstName} {user.LastName},</p>
                    <p>You requested a password reset.</p>
                    <p>Please click the following link to reset your password:</p>
                    <p><a href='{resetPasswordUrl}'>Reset Password</a></p>
                    <p>This link is valid for {duration}.</p>
                    <p>If you did not request this, please ignore this email.</p>
                    <p>Best regards,<br>Your Company Team</p>
                </body>
                </html>",
            To = user.Email,
            Subject = "Reset Password",
        };

        await _emailServiceAsync.SendEmailAsync(emailRequest, cancellationToken);
    }
}
