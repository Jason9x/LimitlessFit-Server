using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Options;
using LimitlessFit.Interfaces;
using LimitlessFit.Models;

namespace LimitlessFit.Services
{
    public class EmailService(IOptions<SmtpSettings> smtpSettings) : IEmailService
    {
        public async Task SendPasswordResetEmailAsync(string email, string resetToken)
        {
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress(smtpSettings.Value.FromName, smtpSettings.Value.FromAddress));
            message.To.Add(new MailboxAddress("", email));
            message.Subject = "Password Reset Request";

            var resetLink = $"http://localhost:3000/reset-password?email={email}&token={resetToken}";

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $"""
                            <h1>Password reset</h1>
                            <p>Click the link below to reset your password (valid for 15 minutes):</p>
                            <p><a href="{resetLink}">{resetLink}</a></p>
                            <p>If you didn't request this, please ignore this email.</p>
                            """
            };

            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();

            await client.ConnectAsync(smtpSettings.Value.Host, smtpSettings.Value.Port,
                SecureSocketOptions.SslOnConnect);

            await client.AuthenticateAsync(smtpSettings.Value.Username, smtpSettings.Value.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}