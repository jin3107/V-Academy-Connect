using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using VAcademyConnect.Application.Interfaces;

namespace VAcademyConnect.Infrastructure.Email
{
    public class EmailService : IEmailService
    {
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;
        private readonly string _fromEmail;
        private readonly string _fromName;

        public EmailService(IConfiguration configuration)
        {
            _smtpServer = configuration["EmailSettings:SmtpServer"]!;
            _smtpPort = int.Parse(configuration["EmailSettings:SmtpPort"] ?? "587");
            _smtpUsername = configuration["EmailSettings:SmtpUsername"]!;
            _smtpPassword = configuration["EmailSettings:SmtpPassword"]!;
            _fromEmail = configuration["EmailSettings:FromEmail"]!;
            _fromName = configuration["EmailSettings:FromName"] ?? "VAcademyConnect";
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_fromName, _fromEmail));
            message.To.Add(new MailboxAddress("", to));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder
            {
                TextBody = body,
                HtmlBody = body.Replace("\n", "<br/>")
            };
            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(_smtpServer, _smtpPort, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_smtpUsername, _smtpPassword);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
