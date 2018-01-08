using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Windows;
using System.Threading;
using LocationInterface.Utils;
using System.Windows.Documents;
using System.Windows.Markup;
using AnalysisSDK;

namespace LocationInterface.Windows
{
    /// <summary>
    /// Interaction logic for EmailWindow.xaml
    /// </summary>
    public partial class EmailWindow : Window
    {
        protected EmailProcessor EmailProcessor { get; set; }

        public EmailWindow()
        {
            InitializeComponent();

            EmailProcessor = new EmailProcessor()
            {
                BindableVariables = new Dictionary<string, IAnalysis>(),
            };

            foreach (AnalysisPlugin plugin in PluginManager.Plugins)
                EmailProcessor.BindableVariables[plugin.Name] = plugin.Analysis;
        }

        public void ManageContactsButtonClick(object sender, RoutedEventArgs e)
        {
            new ContactWindow().ShowDialog();
        }

        private EmailContact[] GetEmailContacts(string contactsString)
        {
            List<EmailContact> contacts = new List<EmailContact>();
            if (!string.IsNullOrWhiteSpace(contactsString))
                foreach (string emailAddress in contactsString.Split(';'))
                    if (Regex.IsMatch(emailAddress, Constants.EMAILREGEX, RegexOptions.IgnoreCase))
                        contacts.Add(new EmailContact("", emailAddress));
            return contacts.ToArray();
        }

        private EmailContact[] GetEmailContacts()
        {
            return GetEmailContacts($"{ recipientEntryBox.Text };{ ccEntryBox.Text };{ bccEntryBox.Text }");
        }

        private string CompleteContactsString(string currentString, EmailContact[] contacts)
        {
            if (!string.IsNullOrWhiteSpace(currentString) && currentString[currentString.Length - 1] != ';') currentString += ';';
            foreach (EmailContact contact in contacts) currentString += $"{ contact.EmailAddress };";
            return currentString.Length > 0 ? currentString.Substring(0, currentString.Length - 1) : "";
        }

        private void SendButtonClick(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(subjectEntryBox.Text)
                || (MessageBox.Show("Are you sure you want to send without a subject?", "No Subject", MessageBoxButton.YesNo) == MessageBoxResult.Yes))
            {
                Email email = new Email(new EmailAccount(SettingsManager.Active.EmailAddress, SettingsManager.Active.DisplayName, SettingsManager.Active.Password),
                    GetEmailContacts(recipientEntryBox.Text),
                    GetEmailContacts(ccEntryBox.Text),
                    GetEmailContacts(bccEntryBox.Text))
                { Body = new TextRange(emailBodyRichTextBox.Document.ContentStart, emailBodyRichTextBox.Document.ContentEnd).Text, Subject = subjectEntryBox.Text };
                new Thread(SendEmail) { IsBackground = true }.Start(email);
            }
        }

        private string HandleSelection(string currentSelection)
        {
            ContactSelectorWindow window = new ContactSelectorWindow(GetEmailContacts());
            window.ShowDialog();
            return CompleteContactsString(currentSelection, window.SelectedContacts);
        }

        private void SendEmail(object email)
        {
            statusLabel.Dispatcher.Invoke(() => statusLabel.Content = "Sending Email");
            sendButton.Dispatcher.Invoke(() => sendButton.IsEnabled = false);
            bool emailSent = ((Email)email).Send(EmailProcessor);
            statusLabel.Dispatcher.Invoke(() => statusLabel.Content = emailSent ? "Email Sent" : "Email Not Sent");
            sendButton.Dispatcher.Invoke(() => sendButton.IsEnabled = true);
        }

        private void AddRecipientsButtonClick(object sender, RoutedEventArgs e)
        {
            recipientEntryBox.Text = HandleSelection(recipientEntryBox.Text);
        }

        private void AddCCsButtonClick(object sender, RoutedEventArgs e)
        {
            ccEntryBox.Text = HandleSelection(ccEntryBox.Text);
        }

        private void AddBCCsButtonClick(object sender, RoutedEventArgs e)
        {
            bccEntryBox.Text = HandleSelection(bccEntryBox.Text);
        }

        private void LoadPresetButtonClick(object sender, RoutedEventArgs e)
        {
            EmailPresetWindow window = new EmailPresetWindow(subjectEntryBox.Text, XamlWriter.Save(emailBodyRichTextBox.Document));
            window.ShowDialog();
            if (window.PresetLoaded)
            {
                subjectEntryBox.Text = window.SelectedPreset.Subject;
                emailBodyRichTextBox.Document = (FlowDocument)XamlReader.Parse(window.SelectedPreset.Body);
            }
        }

        private void InsertAnalysisButtonClick(object sender, RoutedEventArgs e)
        {
            InsertAnalysisWindow insertAnalysisWindow = new InsertAnalysisWindow(EmailProcessor);
            insertAnalysisWindow.ShowDialog();
            if (insertAnalysisWindow.Selected)
                emailBodyRichTextBox.CaretPosition.InsertTextInRun(insertAnalysisWindow.AnalysisBindString);
        }
    }
}
