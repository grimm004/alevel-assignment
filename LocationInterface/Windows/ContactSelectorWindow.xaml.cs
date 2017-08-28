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

        public ContactSelectorWindow(EmailContact[] ignoreableContacts)
        {
            SelectedContacts = new EmailContact[0];
            IgnoreableContacts = ignoreableContacts;
            Database = new CSVDatabase(SettingsManager.Active.EmailDatabase, tableFileExtention: ".csv");
            InitializeComponent();
            PopulateTable();
        }

        public void PopulateTable()
        {
            contactsDataGrid.Items.Clear();
            Contacts = Database.GetRecords<EmailContact>("Contacts");
            foreach (EmailContact contact in Contacts)
            {
                bool insertContact = true;
                foreach (EmailContact ignoreableContact in IgnoreableContacts)
                    if (ignoreableContact.EmailAddress.ToLower() == contact.EmailAddress.ToLower())
                        insertContact = false;
                if (insertContact) contactsDataGrid.Items.Add(contact);
            }
        }

        private void ContactsDataGridSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedContacts = new EmailContact[contactsDataGrid.SelectedItems.Count];
            for (int i = 0; i < SelectedContacts.Length; i++) SelectedContacts[i] = (EmailContact)contactsDataGrid.SelectedItems[i];
        }

        private void SubmitSelectionButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            SelectedContacts = new EmailContact[0];
            Close();
        }
    }
}
