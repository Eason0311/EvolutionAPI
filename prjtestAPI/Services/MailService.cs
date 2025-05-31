using prjtestAPI.Services.Interfaces;
using System.Net.Mail;
using System.Net;

namespace prjtestAPI.Services
{
    public class MailService : IMailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<MailService> _logger;

        public MailService(IConfiguration config, ILogger<MailService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task SendAsync(string toEmail, string subject, string htmlContent)
        {
            var settings = _config.GetSection("MailSettings");
            var fromEmail = settings["SenderEmail"];
            var fromName = settings["SenderName"];
            var host = settings["SmtpHost"];
            var port = int.Parse(settings["SmtpPort"]);
            var password = settings["SenderPassword"];

            var mail = new MailMessage
            {
                From = new MailAddress(fromEmail, fromName),
                Subject = subject,
                Body = htmlContent,
                IsBodyHtml = true
            };
            mail.To.Add(toEmail);

            using var smtp = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(fromEmail, password),
                EnableSsl = true
            };

            try
            {
                await smtp.SendMailAsync(mail);
                _logger.LogInformation("✅ 信件已成功寄出至 {Email}", toEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ 寄送信件至 {Email} 失敗", toEmail);
                throw;
            }
        }
    }
}
