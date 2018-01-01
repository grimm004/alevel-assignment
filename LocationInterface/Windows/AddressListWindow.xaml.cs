using DatabaseManagerLibrary;
using LocationInterface.Utils;
using System;
using System.Collections.Generic;
using System.Windows;

namespace LocationInterface.Windows
{
    /// <summary>
    /// Interaction logic for AddressListWindow.xaml
    /// </summary>
    public partial class AddressListWindow : Window
    {
        protected Common Common { get; }
        protected HashSet<string> MacAddresses { get; set; }
        protected string[] ExistingMacAddresses { get; set; }
        public string[] SelectedMacAddresses { get; protected set; }
        public bool Selected { get; protected set; }

        public AddressListWindow(Common common, string[] existingMacAddresses)
        {
            Common = common;
            InitializeComponent();

            ExistingMacAddresses = existingMacAddresses;
            SelectedMacAddresses = new string[0];
            MacAddresses = new HashSet<string>();
            Selected = false;
            UpdateTable();
        }

        public void UpdateTable()
        {
            MacAddresses.Clear();
            foreach (Table table in Common.LoadedDataTables)
                table.SearchRecords(record => MacAddresses.Add((string)record.GetValue("MAC")));
            
            MacSelectionDataGrid.Items.Clear();
            foreach (string mac in MacAddresses)
            {
                bool insertMac = true;
                foreach (string existingMac in ExistingMacAddresses)
                    if (existingMac.ToLower() == mac.ToLower())
                        insertMac = false;
                if (insertMac) MacSelectionDataGrid.Items.Add(new Mac { Address = mac });
            }
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            Selected = false;
            Close();
        }

        private void AddMacButtonPress(object sender, RoutedEventArgs e)
        {
            Selected = true;
            Close();
        }

        private void ContactsDataGridSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            SelectedMacAddresses = new string[MacSelectionDataGrid.SelectedItems.Count];
            for (int i = 0; i < SelectedMacAddresses.Length; i++) SelectedMacAddresses[i] = ((Mac)MacSelectionDataGrid.SelectedItems[i]).Address;
        }
    }

    class Mac
    {
        public string Address { get; set; }
    }
}
