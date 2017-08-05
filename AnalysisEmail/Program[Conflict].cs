using System.Net;
using System.Net.Mail;

namespace Mailer
{
    class Program
    {
        public static void Main(string[] args)
        {
            MailAddress fromAddress = new MailAddress("grimm004@gmail.com", "Test Name");
            MailAddress toAddress = new MailAddress("grimm004@gmail.com", "Max Grimmett");
            const string fromPassword = "lmuwfxoaqwuxjrqf";
            const string subject = "Test Subject";
            const string body = "Test Body";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                smtp.Send(message);
            }
        }
    }
}