using AnalysisSDK;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Windows;

// TODO: MAKE FINITE STATE MACHINE FOR PRE-PROCESSOR

namespace LocationInterface.Utils
{
    public class EmailProcessor
    {
        public string PreProcessedBody { get; set; }
        public Dictionary<string, IAnalysis> BindableVariables { get; set; }

        /// <summary>
        /// Initialize the emailprocessor type
        /// </summary>
        public EmailProcessor()
        {
            PreProcessedBody = "";
            BindableVariables = new Dictionary<string, IAnalysis>();
        }

        /// <summary>
        /// Pre-process an email body to insert analysis results
        /// </summary>
        /// <param name="processedBody">The output processed body</param>
        /// <returns>true if the processing is successful</returns>
        public bool ProcessedBody(out string processedBody)
        {
            // Define the processed body
            processedBody = "";

            // Define control and storage variables for detected variables
            bool inVariable = false;
            string currentVariable = "";

            // Define control and storage variables for detected metadata
            bool inMetadata = false;
            string currentMetadata = "";

            // Define information variables for the position of the pre-processor
            int lineNumber = 1, linePosition = 0;
            // Loop through each character in the pre-processed data
            for (int i = 0; i < PreProcessedBody.Length; i++)
            {
                // If there is a new line, reset the line position and increment the line number
                if (PreProcessedBody[i] == '\n') { linePosition = 0; lineNumber++; }
                // If the current character is an opener, mark the state as being in-variable
                if (PreProcessedBody[i] == '{') inVariable = true;
                // Else if the current character was a closer
                else if (PreProcessedBody[i] == '}')
                {
                    // If the bindable variables has a definition for the selected variable reference name
                    if (BindableVariables.ContainsKey(currentVariable))
                    {
                        // Fetch the result from the AnalysisSDK plugin
                        AnalysisResult result = BindableVariables[currentVariable].FetchResult(currentMetadata);
                        // If the outcome is valid, add the analysis content to the processed body
                        if (result.Outcome == ResultRequestOutcome.OK) processedBody += result.Content;
                        else
                        {
                            // Else show an error message (along with line position, etc) and return false
                            MessageBox.Show($"{ currentVariable } returned an error { result.Outcome.ToString() } at line { lineNumber }, pos { linePosition }. { (!string.IsNullOrWhiteSpace(result.Content) ? $" Message: { result.Content }" : "") }", "Preprocessor Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return false;
                        }
                    }
                    else
                    {
                        // Else show an error message and return false
                        MessageBox.Show($"Could not find plugin '{ currentVariable }' at line { lineNumber }, pos { linePosition }.", "Preprocessor Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return false;
                    }

                    // Re-set the control and storage variables
                    currentVariable = "";
                    inVariable = false;
                    currentMetadata = "";
                    inMetadata = false;
                }
                // Else if the pre-processor is recording a variable and a colon is found, switch to being int the metadata
                else if (inVariable && !inMetadata && PreProcessedBody[i] == ':')
                {
                    inVariable = false;
                    inMetadata = true;
                }
                // Else if pre-processor is recording a variable, not recording metadata and the current characer is not a space, add the current character to the current variable storage
                else if (inVariable && !inMetadata && PreProcessedBody[i] != ' ') currentVariable += PreProcessedBody[i];
                // Else if pre-processor is not recording a variable, recording metadata and the current characer is not a space, add the current character to the current metadata storage
                else if (!inVariable && inMetadata && PreProcessedBody[i] != ' ') currentMetadata += PreProcessedBody[i];
                // Else if the pre-processor is either recording a variable or metadata and the current character is a space, move over to the next iteration of the loop
                else if ((inVariable || inMetadata) && PreProcessedBody[i] == ' ') continue;
                // Else add the current character to the processed body
                else processedBody += PreProcessedBody[i];
                // Increment the line position
                linePosition++;
            }
            // Return true (as if it has reached this point without return the processed value is assumed to be valid)
            return true;
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
        /// <param name="emailProcessor">The email processor to use</param>
        /// <returns>true if the email is sent, false if not</returns>
        public bool Send(EmailProcessor emailProcessor)
        {
            // Set the pre-processed body in the email processor to the raw entered body
            emailProcessor.PreProcessedBody = Body;
            // If the processed body is not valid return false (and dont send the email), and output the processed body
            if (!emailProcessor.ProcessedBody(out string body)) return false;

            // Create a new MailMessage
            using (MailMessage message = new MailMessage()
            {
                From = SenderAccount.MailAddress,
                Subject = Subject,
                Body = body,
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
                    return false;
                }
                catch (InvalidOperationException e)
                {
                    // If an invalidoperationexception occurrs output the error message
                    MessageBox.Show(e.Message, "Email Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
                return true;
            }
        }
    }
}
