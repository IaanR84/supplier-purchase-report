using System.Net;
using System.Net.Mail;

namespace SupplierPurchaseReport.Services
{
    public class EmailService : IEmailService
    {
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _username;
        private readonly string _password;
        private readonly string _from;

        public EmailService(string smtpServer, int smtpPort,
                            string username, string password, string from)
        {
            _smtpServer = smtpServer;
            _smtpPort = smtpPort;
            _username = username;
            _password = password;
            _from = from;
        }

        public async Task SendAsync(string to, string subject,
                                   string fileName, byte[] attachment)
        {
            using var message = new MailMessage();
            message.To.Add(to);
            message.Subject = subject;
            message.Body = "Please find the daily purchases attached.";
            message.From = new MailAddress(_from);

            using var ms = new MemoryStream(attachment);
            message.Attachments.Add(new Attachment(ms, fileName, "text/csv"));

            using var smtp = new SmtpClient(_smtpServer, _smtpPort);
            smtp.Credentials = new NetworkCredential(_username, _password);
            smtp.EnableSsl = true;

            await smtp.SendMailAsync(message);
        }
    }
}