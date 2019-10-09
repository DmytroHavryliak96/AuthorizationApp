using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationApp.Services
{
    public interface IMailSender
    {
        Task SendEmailAsync(string email, string subject, string htmlMessage);
    }
}
