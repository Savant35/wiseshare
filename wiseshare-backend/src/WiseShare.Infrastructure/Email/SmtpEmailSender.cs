using Wiseshare.Application.Common.Interfaces.Email;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace Wiseshare.Infrastructure.Email;
    public class SmtpEmailSender : IEmailSender
    {
        private readonly SmtpClient _client;
        private readonly string _fromAddress;

        public SmtpEmailSender(IConfiguration config)
{
    _fromAddress = config["Smtp:From"] 
                   ?? throw new ArgumentNullException(nameof(config), "'Smtp:From' config value is missing");

    _client = new SmtpClient
    {
        Host = config["Smtp:Host"] 
               ?? throw new ArgumentNullException(nameof(config), "'Smtp:Host' config value is missing"),
        Port = int.Parse(config["Smtp:Port"] 
               ?? throw new ArgumentNullException(nameof(config), "'Smtp:Port' config value is missing")),
        Credentials = new NetworkCredential(
            config["Smtp:User"] 
                ?? throw new ArgumentNullException(nameof(config), "'Smtp:User' config value is missing"),
            config["Smtp:Pass"] 
                ?? throw new ArgumentNullException(nameof(config), "'Smtp:Pass' config value is missing")
        ),
        EnableSsl = true
    };
}

        public async Task SendAsync(string to, string subject, string htmlBody){

            var msg = new MailMessage(_fromAddress, to){
                Subject    = subject,
                Body       = htmlBody,
                IsBodyHtml = true
            };
            await _client.SendMailAsync(msg);
        }
    }
