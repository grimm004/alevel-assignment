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
        protected Database Database { get; set; }
        protected EmailContact[] Contacts { get; set; }

        public ContactWindow()
        {
            Database = new CSVDatabase(SettingsManager.Active.EmailDatabase, tableFileExtention: ".csv");
            InitializeComponent();
            UpdateTable();

            KeyDown += delegate { if (Keyboard.IsKeyDown(Key.Delete)) RemoveContact(); };
        }

        public void RemoveContact()
        {
            Table contactsTable = Database.GetTable("Contacts");
            foreach (EmailContact contact in contactsDataGrid.SelectedItems)
                contactsTable.DeleteRecord((uint)Array.IndexOf(Contacts, contact));
            Database.SaveChanges();
            UpdateTable();
        }

        public void UpdateTable()
        {
            contactsDataGrid.Items.Clear();
            foreach (EmailContact contact in (Contacts = Database.GetRecords<EmailContact>("Contacts")))
                contactsDataGrid.Items.Add(contact);
        }

        private void AddContactClick(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(nameEntry.Text))
                if (Regex.IsMatch(emailEntry.Text, Constants.EMAILREGEX, RegexOptions.IgnoreCase))
                {
                    Table contactsTable = Database.GetTable("Contacts");

                    if (contactsTable.GetRecords("EmailAddress", emailEntry.Text).Length == 0)
                    {
                        contactsTable.AddRecord(new object[] { nameEntry.Text, emailEntry.Text });
                        Database.SaveChanges();
                        nameEntry.Text = "";
                        emailEntry.Text = "";
                    }
                    else MessageBox.Show("Email already in database.", "Error");
                    UpdateTable();
                }
                else MessageBox.Show("You must enter a valid email.", "Error");
            else MessageBox.Show("You must enter a name.", "Error");
        }
    }
}
