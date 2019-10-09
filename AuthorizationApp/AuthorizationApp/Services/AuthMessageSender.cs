using AuthorizationApp.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MimeKit;
using MailKit.Net.Smtp;

namespace AuthorizationApp.Services
{
    public class AuthMessageSender : IMailSender
    {
        public AuthMessageSender(IOptions<AuthMessageSenderOptions> optionsAccessor)
        {
            Options = optionsAccessor.Value;
        }

        public AuthMessageSenderOptions Options { get; } 
        public async Task SendEmailAsync(string _email, string _subject, string _htmlMessage)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("Authorization App", Options.Gmail));
            emailMessage.To.Add(new MailboxAddress("", _email));
            emailMessage.Subject = _subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = _htmlMessage
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.gmail.com", 587, false);
                await client.AuthenticateAsync(Options.Gmail, Options.Password);
                await client.SendAsync(emailMessage);

                await client.DisconnectAsync(true);
            }

            
        }
    }
}
