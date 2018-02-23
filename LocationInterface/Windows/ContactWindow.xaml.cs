using System;
using System.Windows;
using DatabaseManagerLibrary;
using DatabaseManagerLibrary.CSV;
using LocationInterface.Utils;
using System.Windows.Input;
using System.Text.RegularExpressions;

namespace LocationInterface.Windows
{
    /// <summary>
    /// Interaction logic for ContactWindow.xaml
    /// </summary>
    public partial class ContactWindow : Window
    {
        protected Database Database { get; private set; }
        protected EmailContact[] Contacts { get; set; }

        /// <summary>
        /// Initialse the contact manager window
        /// </summary>
        public ContactWindow()
        {
            Database = new CSVDatabase(SettingsManager.Active.EmailDatabase, tableFileExtention: ".csv");
            InitializeComponent();
            UpdateTable();

            // Add a key listener for the delete key
            KeyDown += delegate { if (Keyboard.IsKeyDown(Key.Delete)) RemoveSelectedContacts(); };
        }

        /// <summary>
        /// Remove the selected contacts
        /// </summary>
        public void RemoveSelectedContacts()
        {
            // Fetch the contacts table from the database
            Table contactsTable = Database.GetTable("Contacts");
            // Loop through the selected contacts
            foreach (EmailContact contact in ContactsDataGrid.SelectedItems)
                // Delete each selectec contact from the table
                contactsTable.DeleteRecord((uint)Array.IndexOf(Contacts, contact));
            // Save the changes to the database
            Database.SaveChanges();
            // Update the table with the new contact list
            UpdateTable();
        }

        /// <summary>
        /// Update the DataGrid with available contacts
        /// </summary>
        public void UpdateTable()
        {
            // Clear the items in the DataGrid
            ContactsDataGrid.Items.Clear();
            // Loop through each contact in the contacts database table
            foreach (EmailContact contact in (Contacts = Database.GetRecords<EmailContact>("Contacts")))
                // Add the contact to the DataGrid
                ContactsDataGrid.Items.Add(contact);
        }

        /// <summary>
        /// Add a new contact
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddContactClick(object sender, RoutedEventArgs e)
        {
            // If the contact name is not white space or emty
            if (!string.IsNullOrWhiteSpace(NameEntry.Text))
                // And if the email matches the pre-defined email regular expression
                if (Regex.IsMatch(EmailEntry.Text, Constants.EMAILREGEX, RegexOptions.IgnoreCase))
                {
                    // Fetch the contacts table
                    Table contactsTable = Database.GetTable("Contacts");

                    // If the contact email does not already exist in the table
                    if (contactsTable.GetRecords("EmailAddress", EmailEntry.Text).Length == 0)
                    {
                        // Add the contact as a new record to the contacts database table
                        contactsTable.AddRecord(new object[] { NameEntry.Text, EmailEntry.Text });
                        // Save the changes to the database
                        Database.SaveChanges();
                        // Re-set the name and email entry boxes
                        NameEntry.Text = "";
                        EmailEntry.Text = "";
                        // Update the DataGrid
                        UpdateTable();
                    }
                    // Else show an error message
                    else MessageBox.Show("Email already in database.", "Error");
                }
                // Else show an error message
                else MessageBox.Show("You must enter a valid email.", "Error");
            // Else show an error message
            else MessageBox.Show("You must enter a name.", "Error");
        }
    }
}
