using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace WebRecommend
{
    public class EmailSender : IEmailSender
    {
        public IConfiguration Configuration { get; }
        public EmailSender(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            string fromMail = Configuration["Mail:Addres"];
            string fromPassword = Configuration["Mail:AppPassword"];

            MailMessage message = new()
            {
                From = new MailAddress(fromMail),
                Subject = subject
            };
            message.To.Add(new MailAddress(email));
            message.Body = "<html><body> " + htmlMessage + " </body></html>";
            message.IsBodyHtml = true;

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(fromMail, fromPassword),
                EnableSsl = true,
            };
            smtpClient.Send(message);
            return Task.CompletedTask;
        }
    }
}
