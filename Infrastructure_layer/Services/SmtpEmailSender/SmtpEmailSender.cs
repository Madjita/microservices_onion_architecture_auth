using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Infrastructure_layer.Services.SmtpEmailSender
{
    public interface IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage);

    }

    public class SmtpEmailSender : IEmailSender
    {
        private readonly SmtpClient smtpClient;

        public SmtpEmailSender(SmtpClient smtpClient)
        {
            this.smtpClient = smtpClient;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {

            MailMessage Msg = new MailMessage();
            // Sender e-mail address.
            Msg.From = new MailAddress("madjita@mail.ru");
            // Recipient e-mail address.
            Msg.To.Add(email);
            //Msg.CC.Add(email);

            Msg.Subject = subject;
            Msg.IsBodyHtml = true;
            Msg.Body = htmlMessage;



            await smtpClient.SendMailAsync(Msg);
        }
    }
}

