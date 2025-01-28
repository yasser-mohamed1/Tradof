using DotNetEnv;
using System.Net;
using System.Net.Mail;
using Tradof.Auth.Services.Interfaces;

namespace Tradof.Auth.Services.Implementation
{
    public class EmailService : IEmailService
    {
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;

        public EmailService()
        {
            Env.Load();

            _smtpServer = Environment.GetEnvironmentVariable("SMTP_SERVER")!;
            _smtpPort = int.Parse(Environment.GetEnvironmentVariable("SMTP_PORT")!);
            _smtpUsername = Environment.GetEnvironmentVariable("SMTP_USERNAME")!;
            _smtpPassword = Environment.GetEnvironmentVariable("SMTP_PASSWORD")!;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                using (var smtpClient = new SmtpClient(_smtpServer, _smtpPort))
                {
                    smtpClient.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
                    smtpClient.EnableSsl = true;

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(_smtpUsername),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true
                    };

                    mailMessage.To.Add(to);

                    await smtpClient.SendMailAsync(mailMessage);
                }
            }
            catch (SmtpException ex)
            {
                throw new ApplicationException("Failed to send email.", ex);
            }
        }
    }
}