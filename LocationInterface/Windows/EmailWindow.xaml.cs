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

        /// <summary>
        /// Initialze the email window
        /// </summary>
        public EmailWindow()
        {
            InitializeComponent();

            // Create an email processor instance
            EmailProcessor = new EmailProcessor()
            {
                BindableVariables = new Dictionary<string, IAnalysis>(),
            };

            // Loop through each plugin in the plugin manager
            foreach (AnalysisPlugin plugin in PluginManager.Plugins)
                // Add the plugin as a bindable variable to the email processor
                EmailProcessor.BindableVariables[plugin.Name] = plugin.Analysis;
        }

        /// <summary>
        /// Open the contacts manager window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ManageContactsButtonClick(object sender, RoutedEventArgs e)
        {
            new ContactWindow().ShowDialog();
        }

        /// <summary>
        /// Get the email contacts to be set to from an email contact string
        /// </summary>
        /// <param name="contactsString">The email contact string to parse</param>
        /// <returns>an array of email contacts derived from the string</returns>
        private EmailContact[] GetEmailContacts(string contactsString)
        {
            // Define and initialze the list of email conatacts
            List<EmailContact> contacts = new List<EmailContact>();
            // If the contacats tring is not white space or empty
            if (!string.IsNullOrWhiteSpace(contactsString))
                // Loop through each email address in the string
                foreach (string emailAddress in contactsString.Split(';'))
                    // If the curent email address matches the pre-define email regex
                    if (Regex.IsMatch(emailAddress, Constants.EMAILREGEX, RegexOptions.IgnoreCase))
                        // Add it to the list of email conatacts
                        contacts.Add(new EmailContact("", emailAddress));
            // Return the list of email contacts as an array
            return contacts.ToArray();
        }

        /// <summary>
        /// Get all the email contacts in use (from recipient, CC and BCC)
        /// </summary>
        /// <returns>an array of in-use email contacts</returns>
        private EmailContact[] GetEmailContacts()
        {
            // Combine the different contact list strings and resolve them to email contacts
            return GetEmailContacts($"{ RecipientEntryBox.Text };{ CcEntryBox.Text };{ BccEntryBox.Text }");
        }

        /// <summary>
        /// Add an array of email contacts to a contacts tring
        /// </summary>
        /// <param name="currentString"></param>
        /// <param name="contacts"></param>
        /// <returns></returns>
        private string CompleteContactsString(string currentString, EmailContact[] contacts)
        {
            // Check if the last character in the supplied string is a semi-colon, if not, add one
            if (!string.IsNullOrWhiteSpace(currentString) && currentString[currentString.Length - 1] != ';') currentString += ';';
            // Loop through each contact in the contacts array and add the amil to the string
            foreach (EmailContact contact in contacts) currentString += $"{ contact.EmailAddress };";
            // Return the string, removing the final semi-colon
            return currentString.Length > 0 ? currentString.Substring(0, currentString.Length - 1) : "";
        }

        /// <summary>
        /// Begin sending the email
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SendButtonClick(object sender, RoutedEventArgs e)
        {
            // Check if the subject is empty, if it is ask the user if they want so send without a subject
            if (!string.IsNullOrWhiteSpace(SubjectEntryBox.Text)
                || (MessageBox.Show("Are you sure you want to send without a subject?", "No Subject", MessageBoxButton.YesNo) == MessageBoxResult.Yes))
            {
                // Formulate an instance of an email object
                Email email = new Email(new EmailAccount(SettingsManager.Active.EmailAddress, SettingsManager.Active.DisplayName, SettingsManager.Active.Password),
                    GetEmailContacts(RecipientEntryBox.Text),
                    GetEmailContacts(CcEntryBox.Text),
                    GetEmailContacts(BccEntryBox.Text))
                { Body = new TextRange(EmailBodyRichTextBox.Document.ContentStart, EmailBodyRichTextBox.Document.ContentEnd).Text, Subject = SubjectEntryBox.Text };
                // Send the email in a new background thread
                new Thread(SendEmail) { IsBackground = true }.Start(email);
            }
        }

        /// <summary>
        /// Allow the user to add contacts
        /// </summary>
        /// <param name="currentSelection">The current selected contacts</param>
        /// <returns>the new contacat string</returns>
        private string HandleSelection(string currentSelection)
        {
            // Create and show the window as a dialog
            ContactSelectorWindow window = new ContactSelectorWindow(GetEmailContacts());
            window.ShowDialog();
            // Return the contact string with any new selected contacts in it
            return CompleteContactsString(currentSelection, window.SelectedContacts);
        }

        /// <summary>
        /// Send the email
        /// </summary>
        /// <param name="email"></param>
        private void SendEmail(object email)
        {
            // Manage the WPF window for sending an email
            StatusLabel.Dispatcher.Invoke(() => StatusLabel.Content = "Sending Email");
            SendButton.Dispatcher.Invoke(() => SendButton.IsEnabled = false);
            // Send the email
            bool emailSent = ((Email)email).Send(EmailProcessor);
            // Re-set the email window
            StatusLabel.Dispatcher.Invoke(() => StatusLabel.Content = emailSent ? "Email Sent" : "Email Not Sent");
            SendButton.Dispatcher.Invoke(() => SendButton.IsEnabled = true);
        }

        /// <summary>
        /// Add recipients
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddRecipientsButtonClick(object sender, RoutedEventArgs e)
        {
            RecipientEntryBox.Text = HandleSelection(RecipientEntryBox.Text);
        }

        /// <summary>
        /// Add CCs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddCCsButtonClick(object sender, RoutedEventArgs e)
        {
            CcEntryBox.Text = HandleSelection(CcEntryBox.Text);
        }

        /// <summary>
        /// Add BCCs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddBCCsButtonClick(object sender, RoutedEventArgs e)
        {
            BccEntryBox.Text = HandleSelection(BccEntryBox.Text);
        }

        /// <summary>
        /// Load or save a preset
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadPresetButtonClick(object sender, RoutedEventArgs e)
        {
            // Create a new preset manager window
            EmailPresetWindow window = new EmailPresetWindow(SubjectEntryBox.Text, XamlWriter.Save(EmailBodyRichTextBox.Document));
            // Show it as a dialog
            window.ShowDialog();
            // If a preset is loaded by the user
            if (window.PresetLoaded)
            {
                // Load the preset to the subject and body
                SubjectEntryBox.Text = window.SelectedPreset.Subject;
                EmailBodyRichTextBox.Document = (FlowDocument)XamlReader.Parse(window.SelectedPreset.Body);
            }
        }

        /// <summary>
        /// Insert an analysis reference
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InsertAnalysisButtonClick(object sender, RoutedEventArgs e)
        {
            // Create an insert analysis window
            InsertAnalysisWindow insertAnalysisWindow = new InsertAnalysisWindow(EmailProcessor);
            // Show it as a dialog
            insertAnalysisWindow.ShowDialog();
            // If an analysis is selected by the user
            if (insertAnalysisWindow.Selected)
                // Insert the analysis text to the body at the selected position
                EmailBodyRichTextBox.CaretPosition.InsertTextInRun(insertAnalysisWindow.AnalysisBindString);
        }
    }
}
