﻿using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Windows;
using System.Threading;
using LocationInterface.Utils;
using System.Windows.Documents;

namespace LocationInterface
{
    /// <summary>
    /// Interaction logic for EmailWindow.xaml
    /// </summary>
    public partial class EmailWindow : Window
    {
        public EmailWindow()
        {
            InitializeComponent();
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

        private void SendEmail(object email)
        {
            statusLabel.Dispatcher.Invoke(() => statusLabel.Content = "Sending Email");
            sendButton.Dispatcher.Invoke(() => sendButton.IsEnabled = false);
            ((Email)email).Send();
            statusLabel.Dispatcher.Invoke(() => statusLabel.Content = "Email Sent");
            sendButton.Dispatcher.Invoke(() => sendButton.IsEnabled = true);
        }

        private void AddRecipientsButtonClick(object sender, RoutedEventArgs e)
        {
            ContactSelectorWindow window = new ContactSelectorWindow(GetEmailContacts(recipientEntryBox.Text));
            window.ShowDialog();
            recipientEntryBox.Text = CompleteContactsString(recipientEntryBox.Text, window.SelectedContacts);
        }

        private void AddCCsButtonClick(object sender, RoutedEventArgs e)
        {
            ContactSelectorWindow window = new ContactSelectorWindow(GetEmailContacts(ccEntryBox.Text));
            window.ShowDialog();
            ccEntryBox.Text = CompleteContactsString(ccEntryBox.Text, window.SelectedContacts);
        }

        private void AddBCCsButtonClick(object sender, RoutedEventArgs e)
        {
            ContactSelectorWindow window = new ContactSelectorWindow(GetEmailContacts(bccEntryBox.Text));
            window.ShowDialog();
            bccEntryBox.Text = CompleteContactsString(bccEntryBox.Text, window.SelectedContacts);
        }

        private void LoadPresetButtonClick(object sender, RoutedEventArgs e)
        {
            new EmailPresetWindow().ShowDialog();
        }
    }
}
