using Microsoft.Extensions.Options;
using ProjMens.Services.Api.Settings;
using System.Net;
using System.Net.Mail;

namespace ProjMens.Services.Api.Helpers
{
    public class EmailHelper
    {
        //atributo
        private readonly MailSettings? _mailSettings;

        //construtor
        public EmailHelper(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        //método para envio de mensagem
        public void Send(string mailTo, string subject, string body)
        {
            var mailMessage = new MailMessage(_mailSettings.Email, mailTo);
            mailMessage.Subject = subject;
            mailMessage.Body = body;
            mailMessage.IsBodyHtml = true;

            var smtpClient = new SmtpClient(_mailSettings.Smtp,(int)_mailSettings.Port);
            smtpClient.EnableSsl = true;
            smtpClient.Credentials = new NetworkCredential(_mailSettings.Email, _mailSettings.Password);
            smtpClient.Send(mailMessage);
        }
    }

}
