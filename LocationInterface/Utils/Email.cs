using System;
using System.Net;
using System.Net.Mail;
using System.Windows;

namespace LocationInterface.Utils
{
    public class EmailAccount
    {
        public MailAddress MailAddress { get; }
        public string Password { get; }

        public EmailAccount(string emailAddress, string name, string password)
        {
            MailAddress = new MailAddress(emailAddress, name);
            Password = password;
        }
    }
    public class EmailContact
    {
        public string Name { get; set; }
        public string EmailAddress { get; set; }

        public EmailContact()
        {

        }

        public EmailContact(string name, string emailAddress)
        {
            Name = name;
            EmailAddress = emailAddress;
        }
    }

    public class Email
    {
        public EmailAccount SenderAccount { get; }
        public EmailContact[] Recipients { get; }
        public EmailContact[] CCs { get; }
        public EmailContact[] BCCs { get; }

        public string Subject { get; set; }
        public string Body { get; set; }

        public Email(EmailAccount account, EmailContact[] recipients, EmailContact[] ccs, EmailContact[] bccs)
        {
            SenderAccount = account;
            Recipients = recipients;
            CCs = ccs;
            BCCs = bccs;
        }

        public void Send()
        {
            // TODO: ADD CONFIGURABLE SUBJECTS AND BODIES WITH BINDABLE VARIABLES

            var smtp = new SmtpClient(SettingsManager.Active.EmailServer, SettingsManager.Active.EmailPort)
            {
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(SenderAccount.MailAddress.Address, SenderAccount.Password)
            };

            using (var message = new MailMessage()
            {
                From = SenderAccount.MailAddress,
                Subject = Subject,
                Body = Body,
            })
            {
                foreach (EmailContact recipient in Recipients) message.To.Add(new MailAddress(recipient.EmailAddress, recipient.Name));
                foreach (EmailContact cc in CCs) message.CC.Add(new MailAddress(cc.EmailAddress, cc.Name));
                foreach (EmailContact bcc in BCCs) message.Bcc.Add(new MailAddress(bcc.EmailAddress, bcc.Name));
                try
                {
                    smtp.Send(message);
                }
                catch (SmtpException e)
                {
                    MessageBox.Show($"An SMTP error occurred. Message from server: { e.Message }", "SMTP Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (InvalidOperationException e)
                {
                    MessageBox.Show(e.Message, "Email Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
