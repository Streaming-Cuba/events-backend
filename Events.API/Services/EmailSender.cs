using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Net.Mail;
using System.Net;

namespace Events.API.Services
{
    public class EmailSender
    {
        private readonly SmtpClient _smtpClient;
        private readonly MailAddress _senderAddress;

        public EmailSender(IConfiguration configuration) 
        {
            _smtpClient = new SmtpClient() 
            {
                Host = configuration["Email::Host"],
                Port = configuration.GetValue<short>("Email::Port"),
                EnableSsl = configuration.GetValue<bool>("Email::Ssl"),
            };
            if (configuration.GetValue<bool>("Email::UseCredentials")) 
            {
                _smtpClient.Credentials = new NetworkCredential() 
                {
                    UserName = configuration["Email::Username"],
                    Password = configuration["Email::Password"]
                };
            }

            _senderAddress = new MailAddress(configuration["Email::SenderAddress"], 
                                            configuration["Email::SenderDisplayName"]);
        }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            MailMessage mail = new MailMessage() 
            {
                From = _senderAddress,
                To = { new MailAddress(email) },
                Subject = subject,
                Body = message,
                IsBodyHtml = false
            };

            return _smtpClient.SendMailAsync(mail);
        }
    }
}