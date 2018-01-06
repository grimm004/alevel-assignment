using DatabaseManagerLibrary;
using LocationInterface.Utils;
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

        /// <summary>
        /// Initialize the address list window
        /// </summary>
        /// <param name="common">The common data object</param>
        /// <param name="existingMacAddresses">The MAC addresses already in use</param>
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

        /// <summary>
        /// Update the MAC addresses in the table
        /// </summary>
        public void UpdateTable()
        {
            // Clear the current HashSet of MAC addresses
            MacAddresses.Clear();
            // Loop through each table in the loaded data tables list
            foreach (Table table in Common.LoadedDataTables)
                // Search each record for new MAC addresses (the hash set will only add it if it is unique)
                table.SearchRecords(record => MacAddresses.Add((string)record.GetValue("MAC")));
            
            // Clear the DataGrid's list of MAC addresses
            MacSelectionDataGrid.Items.Clear();
            // Loop through each MAC address in the HashSet
            foreach (string mac in MacAddresses)
            {
                bool insertMac = true;
                // Loop through each MAC address in the existing MAC addresses list
                foreach (string existingMac in ExistingMacAddresses)
                    // If the mac address to be added is already in used
                    if (existingMac.ToLower() == mac.ToLower())
                        // Mark it not not be added
                        insertMac = false;
                // If the current MAC address can be added, add it to the DataGrid's list
                if (insertMac) MacSelectionDataGrid.Items.Add(new Mac { Address = mac });
            }
        }

        /// <summary>
        /// Cancel MAC selection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            Selected = false;
            Close();
        }

        /// <summary>
        /// Confirm MAC selection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddMacButtonPress(object sender, RoutedEventArgs e)
        {
            Selected = true;
            Close();
        }

        /// <summary>
        /// Update a local list of the selected MAC addresses for when the window is closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ContactsDataGridSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            SelectedMacAddresses = new string[MacSelectionDataGrid.SelectedItems.Count];
            for (int i = 0; i < SelectedMacAddresses.Length; i++) SelectedMacAddresses[i] = ((Mac)MacSelectionDataGrid.SelectedItems[i]).Address;
        }

        /// <summary>
        /// A class for WPF path binding in the DataGrid
        /// </summary>
        private class Mac
        {
            public string Address { get; set; }
        }
    }
}
