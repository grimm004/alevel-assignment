using LocationInterface.Utils;
using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Forms;
using System.ComponentModel;
using System.Windows.Threading;
using System.Windows.Input;
using Microsoft.Xna.Framework;
using System.Windows.Controls;
using System.Linq;

namespace LocationInterface.Windows
{
    public partial class SelectionManagerWindow : Window
    {
        public List<MacPointCollection> Addresses { get; set; }
        public Action MacChangeCallback { get; }
        public Common Common { get; }

        /// <summary>
        /// Initialize the selection manager window
        /// </summary>
        /// <param name="common">The common data</param>
        /// <param name="MacChangeCallback">Callback for when a MAC address is changed</param>
        public SelectionManagerWindow(Common common, Action MacChangeCallback)
        {
            Common = common;
            this.MacChangeCallback = MacChangeCallback;
            InitializeComponent();
            DataContext = this;
            Addresses = new List<MacPointCollection>()
            {
                new MacPointCollection { Address = "10:b1:f8:e7:ca:d1", Colour = Color.Black },
                new MacPointCollection { Address = "08:ee:8b:7f:09:2d", Colour = Color.Red },
                new MacPointCollection { Address = "44:00:10:9f:bb:b0", Colour = Color.Green },
                new MacPointCollection { Address = "58:c9:35:2a:a9:df", Colour = Color.Blue },
                new MacPointCollection { Address = "5c:8d:4e:10:70:ba", Colour = Color.Orange },
                new MacPointCollection { Address = "64:6e:69:9b:55:bb", Colour = Color.Yellow },
                new MacPointCollection { Address = "64:6e:69:9b:61:b3", Colour = Color.Cyan },
                new MacPointCollection { Address = "64:6e:69:b9:af:85", Colour = Color.Violet },
                new MacPointCollection { Address = "fc:2d:5e:a0:e0:ac", Colour = Color.Black },
            };

            // Change the close action so that the window hides
            Closing += delegate (object sender, CancelEventArgs e)
            {
                e.Cancel = true;
                System.Windows.Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                    (DispatcherOperationCallback)(arg =>
                    {
                        Hide();
                        return null;
                    }), null);
            };

            // A key listener for the delete key
            KeyDown += delegate { if (Keyboard.IsKeyDown(Key.Delete)) RemoveMac(); };
            
            UpdateTable();
        }

        /// <summary>
        /// DataGrid RowLoad event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void RowLoad(object sender, DataGridRowEventArgs e)
        {
            // If the row's item is a MacPointColelction
            if (e.Row.DataContext is MacPointCollection RowDataContaxt)
            {
                // Get the colour of the MAC point
                Color color = RowDataContaxt.Colour;
                // Check if the colour is dark or light
                bool dark = (color.R + color.G + color.B) / 3 < 128;
                // Set the background colour to that of the MAC point
                e.Row.Background = new System.Windows.Media.SolidColorBrush(new System.Windows.Media.Color() { R = color.R, G = color.G, B = color.B, A = color.A });
                // If the background is dark, set white text for the foreground, else set black text
                e.Row.Foreground = new System.Windows.Media.SolidColorBrush(dark ? System.Windows.Media.Colors.White : System.Windows.Media.Colors.Black);
            }
        }

        /// <summary>
        /// Add MAC event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddMacButtonPress(object sender, RoutedEventArgs e)
        {
            AddMac();
        }

        /// <summary>
        /// Add a MAC address to the selected addresses
        /// </summary>
        public void AddMac()
        {
            // Check the entered text is not blank or white space
            if (string.IsNullOrWhiteSpace(MacEntry.Text))
            {
                // If it is, show an error message
                System.Windows.MessageBox.Show("Please enter a MAC address...", "Invalid MAC", MessageBoxButton.OK, MessageBoxImage.Error);
                MacEntry.Text = "";
                return;
            }

            // Check the entered MAC address is valid with a regular expression
            if (!new Regex("^([0-9A-Fa-f]{2}[:]){5}([0-9A-Fa-f]{2})$").IsMatch(MacEntry.Text))
            {
                // If it is not, show an error message
                System.Windows.MessageBox.Show("Please enter a valid MAC address...", "Invalid MAC", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Start a colour dialog to fetch the desired MAC address colour
            ColorDialog colorDialog;
            if ((colorDialog = new ColorDialog()).ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // Instanciate and add a MacPointCollection to the list of selected addresses
                Addresses.Add(new MacPointCollection { Address = MacEntry.Text, Colour = new Color { R = colorDialog.Color.R, G = colorDialog.Color.G, B = colorDialog.Color.B, A = colorDialog.Color.A } });
                // Re-set the mac entry textbox
                MacEntry.Text = "";
                // Update the DataGrid table
                UpdateTable();
                // Run the callback for the new MAC address
                MacChangeCallback?.Invoke();
            }
        }

        /// <summary>
        /// Remove selected MAC addresses from the selection
        /// </summary>
        public void RemoveMac()
        {
            // Loop through each slected MacPointCollection
            foreach (MacPointCollection pointCollection in MacSelectionDataGrid.SelectedItems)
                // Remove the current MacPointCollection
                Addresses.Remove(pointCollection);
            // Update the DataGrid table
            UpdateTable();
            // Run the callback for the change in MAC selection
            MacChangeCallback?.Invoke();
        }

        /// <summary>
        /// Update the DataGrid table with the selected MAC addresses
        /// </summary>
        public void UpdateTable()
        {
            // Clear the table's items
            MacSelectionDataGrid.Items.Clear();
            // Loop through each MacPointColelction in the desired address list
            foreach (MacPointCollection address in Addresses)
                // Add it to the table's items
                MacSelectionDataGrid.Items.Add(address);
        }

        /// <summary>
        /// Show a window allowing users to view a list of active MAC addresses
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddressListButtonPress(object sender, RoutedEventArgs e)
        {
            // Instanciate a new random object
            Random random = new Random();
            // Create and show the address list window ad a dialog
            AddressListWindow addressListWindow = new AddressListWindow(Common, Addresses.Select(macPointCollection => macPointCollection.Address).ToArray());
            addressListWindow.ShowDialog();
            // If items have been selected
            if (addressListWindow.Selected)
            {
                // Use Linq to fetch all the selected MAC addresses that are not already loaded
                string[] newAddresses = addressListWindow.SelectedMacAddresses.Where(newMacAddress => !Addresses.Any(macExistingCollection => macExistingCollection.Address == newMacAddress)).ToArray();
                // Loop through each new mac address
                foreach (string macAddress in newAddresses)
                    // Produce a MacPointCollection for it along with a random colour and add it to the list of selected addresses
                    Addresses.Add(new MacPointCollection() { Address = macAddress, Colour = new Color((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble()) });
                // Update the DataGrid table
                UpdateTable();
                // Run the MacChangeCallback for the new mac addresses
                MacChangeCallback?.Invoke();
            }
        }

        /// <summary>
        /// Add the mac address upon enter key press within the textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMacEntryKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Return) AddMac();
        }

        private void DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (MacSelectionDataGrid.SelectedItem != null)
            {
                ColorDialog colorDialog;
                if ((colorDialog = new ColorDialog()).ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    foreach (MacPointCollection collection in Addresses.Where(c => (MacSelectionDataGrid.SelectedItem as MacPointCollection).Address == c.Address))
                        collection.Colour = new Color { R = colorDialog.Color.R, G = colorDialog.Color.G, B = colorDialog.Color.B, A = colorDialog.Color.A };
                    // Re-set the mac entry textbox
                    MacEntry.Text = "";
                    // Update the DataGrid table
                    UpdateTable();
                    // Run the callback for the new MAC address
                    MacChangeCallback?.Invoke();
                }
            }
        }
    }
}
