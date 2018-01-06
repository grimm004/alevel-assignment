using System.Windows;
using DatabaseManagerLibrary;
using DatabaseManagerLibrary.CSV;
using LocationInterface.Utils;
using System.Windows.Controls;

namespace LocationInterface.Windows
{
    /// <summary>
    /// Interaction logic for ContactWindow.xaml
    /// </summary>
    public partial class ContactSelectorWindow : Window
    {
        protected Database Database { get; set; }
        protected EmailContact[] Contacts { get; set; }
        protected EmailContact[] IgnoreableContacts { get; set; }
        public EmailContact[] SelectedContacts { get; protected set; }

        /// <summary>
        /// Initialize a ContactSelector window
        /// </summary>
        /// <param name="ignoreableContacts">The contacts that are already in use</param>
        public ContactSelectorWindow(EmailContact[] ignoreableContacts)
        {
            SelectedContacts = new EmailContact[0];
            IgnoreableContacts = ignoreableContacts;
            Database = new CSVDatabase(SettingsManager.Active.EmailDatabase, tableFileExtention: ".csv");
            InitializeComponent();
            PopulateTable();
        }

        /// <summary>
        /// Populate the DataGrid with the available contacts
        /// </summary>
        public void PopulateTable()
        {
            // Clear the current items in the data grid's list
            ContactsDataGrid.Items.Clear();
            // Fetch the contacts from the database
            Contacts = Database.GetRecords<EmailContact>("Contacts");
            // Loop through each contact in the contacts list
            foreach (EmailContact contact in Contacts)
            {
                bool insertContact = true;
                // Loop through each ignorable contact
                foreach (EmailContact ignoreableContact in IgnoreableContacts)
                    // If the email address of the current contact is equal to that of the current ignorable contact
                    if (ignoreableContact.EmailAddress.ToLower() == contact.EmailAddress.ToLower())
                        // Mark the contact not to be added to list
                        insertContact = false;
                // If the contact can be added to the list, add it
                if (insertContact) ContactsDataGrid.Items.Add(contact);
            }
        }

        /// <summary>
        /// Store a local set of selected contacts (for when the window is closed)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ContactsDataGridSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedContacts = new EmailContact[ContactsDataGrid.SelectedItems.Count];
            for (int i = 0; i < SelectedContacts.Length; i++) SelectedContacts[i] = (EmailContact)ContactsDataGrid.SelectedItems[i];
        }

        /// <summary>
        /// Submit the selected contacts and close the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SubmitSelectionButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Cancel the selection and close the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            SelectedContacts = new EmailContact[0];
            Close();
        }
    }
}
