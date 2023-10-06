using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Tazmania.Entities;
using Tazmania.Interfaces.Services;
using Microsoft.Extensions.Logging;
using Tazmania.Interfaces.Repositories;

namespace Tazmania.Services
{
    public class MailService : IMailService
    {
        const string USERNAME = "no-reply@dementiaroom.it";
        const string PASSWORD = "Clementina%2525";
        const string MAIL_ADDRESS = USERNAME;
        const string HOST = "smtp.dementiaroom.it";
        const int PORT = 587;
        const bool SSL = false;

        readonly ILogger<MailService> Logger;
        readonly IUserRepository UserRepository;

        public MailService(ILogger<MailService> logger, IUserRepository userRepository)
        {
            Logger = logger;
            UserRepository = userRepository;
        }

        public async void SendAsync(MailObject mailObject)
        {
            try
            {
                string content = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"resources/templates/{mailObject.Template}.htm"));

                foreach (var msg in mailObject.Messages)
                {
                    content = content.Replace(msg.Key, msg.Value);
                }

                SmtpClient smtp = new SmtpClient(HOST, PORT)
                {
                    Credentials = new NetworkCredential(USERNAME, PASSWORD),
                    EnableSsl = SSL
                };

                MailMessage message = new MailMessage()
                {
                    From = new MailAddress(MAIL_ADDRESS),
                    Subject = mailObject.Subject,
                    Body = content,
                    IsBodyHtml = true,
                    Priority = mailObject.Priority ? MailPriority.High : MailPriority.Normal
                };

                foreach (var user in (await UserRepository.Fetchs()).Where(r => r.NotifyMail))
                {
                    message.To.Add(user.Mail);
                }

                await smtp.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Send Mail error");
            }
        }
    }
}
