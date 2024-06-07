
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using pdfyai_api.Services.IServices;

namespace pdfyai_api.Services
{
    public class EmailService : IEmailService
    {

        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var mimeMessage = new MimeMessage();
            mimeMessage.Sender = MailboxAddress.Parse(_configuration["Mail:Email"]);
            mimeMessage.To.Add(MailboxAddress.Parse(email));
            mimeMessage.Subject = subject;
            var builder = new BodyBuilder();
            builder.HtmlBody = message;
            mimeMessage.Body = builder.ToMessageBody();
            using var smtp = new SmtpClient();
            smtp.Connect("smtp.titan.email", 465, true);
            smtp.Authenticate(_configuration["Mail:Email"], _configuration["Email:Pass"]);
            await smtp.SendAsync(mimeMessage);
            smtp.Disconnect(true);
        }
    }
}