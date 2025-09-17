using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace EventManagementSystem.Services
{
    public class EmailService
    {
        private readonly string? _smtpServer;
        private readonly int _smtpPort;
        private readonly string? _userName;
        private readonly string? _password;
        private readonly string? _from;

        public EmailService(IConfiguration config)
        {
            _smtpServer = config["Email:smtpServer"];
            _smtpPort = config.GetValue<int>("Email:smtpPort");
            _userName = config["Email:userName"];
            _password = config["Email:password"];
            _from = config["Email:From"];
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            using var client = new SmtpClient(_smtpServer, _smtpPort)
            {
                Credentials = new NetworkCredential(_userName, _password),
                EnableSsl = true
            };

            var mail = new MailMessage(_from, to, subject, body);
            await client.SendMailAsync(mail);
        }
    }
}
