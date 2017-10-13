using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Windows;

namespace LocationInterface.Utils
{
    public class EmailProcessor
    {
        public string PreProcessedBody { get; set; }
        public Dictionary<string, AnalysisResult> BindableVariables { get; set; }

        public EmailProcessor()
        {
            PreProcessedBody = "";
            BindableVariables = new Dictionary<string, AnalysisResult>();
        }

        public string ProcessedBody
        {
            get
            {
                bool inVariable = false;
                string currentVariable = "";
                string processedString = "";
                string variableType = "";
                bool inVariableType = false;
                for (int i = 0; i < PreProcessedBody.Length; i++)
                    if (PreProcessedBody[i] == '{') inVariable = true;
                    else if (PreProcessedBody[i] == '}')
                    {
                        inVariable = false;
                        inVariableType = false;
                        switch (variableType)
                        {
                            case "short":
                                processedString += BindableVariables[currentVariable].ShortOutputString;
                                break;
                            case "full":
                                processedString += BindableVariables[currentVariable].LongOutputString;
                                break;
                            default:
                                processedString += BindableVariables[currentVariable].StandardOutputString;
                                break;
                        }
                        variableType = "";
                        currentVariable = "";
                    }
                    else if (inVariable && !inVariableType && PreProcessedBody[i] == ':')
                    {
                        inVariable = false;
                        inVariableType = true;
                    }
                    else if (!inVariable && inVariableType && PreProcessedBody[i] != ' ') variableType += PreProcessedBody[i];
                    else if (inVariable && !inVariableType && PreProcessedBody[i] != ' ') currentVariable += PreProcessedBody[i];
                    else if ((inVariable || inVariableType) && PreProcessedBody[i] == ' ') continue;
                    else processedString += PreProcessedBody[i];
                return processedString;
            }
        }
    }

    public class EmailPreset
    {
        public string Name { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

        /// <summary>
        /// Initialise the EmailPreset
        /// </summary>
        public EmailPreset()
        {
            Name = "";
            Subject = "";
            Body = "";
        }
        /// <summary>
        /// Initialise the EmailPreset
        /// </summary>
        /// <param name="name">The name of the preset</param>
        /// <param name="subject">The email subject</param>
        /// <param name="body">The email body</param>
        public EmailPreset(string name, string subject, string body)
        {
            Name = name;
            Subject = subject;
            Body = body;
        }
    }
    public class EmailAccount
    {
        public MailAddress MailAddress { get; }
        public string Password { get; }

        /// <summary>
        /// Initialise the EmailAccount
        /// </summary>
        /// <param name="emailAddress">The account holder email address</param>
        /// <param name="name">The display name of the account holder</param>
        /// <param name="password">The password of the account holder</param>
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

        /// <summary>
        /// Initialise an EmailContact
        /// </summary>
        public EmailContact()
        {
            Name = "";
            EmailAddress = "";
        }
        /// <summary>
        /// Initialise an EmailContact
        /// </summary>
        /// <param name="name">The name of the contact</param>
        /// <param name="emailAddress">The email address contact</param>
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

        /// <summary>
        /// Initialise a new Email
        /// </summary>
        /// <param name="account">The EmailAccount of the sender</param>
        /// <param name="recipients">The contacts to send the email to</param>
        /// <param name="ccs">The contacts to CC</param>
        /// <param name="bccs">The contacts to BCC</param>
        public Email(EmailAccount account, EmailContact[] recipients, EmailContact[] ccs, EmailContact[] bccs)
        {
            SenderAccount = account;
            Recipients = recipients;
            CCs = ccs;
            BCCs = bccs;
        }

        /// <summary>
        /// Send the email
        /// </summary>
        public void Send(EmailProcessor emailProcessor)
        {
            emailProcessor.PreProcessedBody = Body;

            // Create a new MailMessage
            using (MailMessage message = new MailMessage()
            {
                From = SenderAccount.MailAddress,
                Subject = Subject,
                Body = emailProcessor.ProcessedBody,
            })
            {
                // For each recipient in the recipients array add it to the message
                foreach (EmailContact recipient in Recipients) message.To.Add(new MailAddress(recipient.EmailAddress, recipient.Name));
                // For each cc in the CCs array add it to the message
                foreach (EmailContact cc in CCs) message.CC.Add(new MailAddress(cc.EmailAddress, cc.Name));
                // For each bcc in the BCCs array add it to the message
                foreach (EmailContact bcc in BCCs) message.Bcc.Add(new MailAddress(bcc.EmailAddress, bcc.Name));
                try
                {
                    // Try to instanciate an smtpclient and send the message
                    new SmtpClient(SettingsManager.Active.EmailServer, SettingsManager.Active.EmailPort)
                    {
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(SenderAccount.MailAddress.Address, SenderAccount.Password)
                    }.Send(message);
                }
                catch (SmtpException e)
                {
                    // If an smtpexception occurrs output the error message
                    MessageBox.Show($"An SMTP error occurred. Message from server: { e.Message }", "SMTP Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (InvalidOperationException e)
                {
                    // If an invalidoperationexception occurrs output the error message
                    MessageBox.Show(e.Message, "Email Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
