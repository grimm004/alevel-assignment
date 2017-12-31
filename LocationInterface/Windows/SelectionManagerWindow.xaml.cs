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

namespace LocationInterface.Windows
{
    public partial class SelectionManagerWindow : Window
    {
        public List<MacPointCollection> Addresses { get; set; }
        public Action MacChangeCallback { get; }

        public SelectionManagerWindow(Action MacChangeCallback)
        {
            this.MacChangeCallback = MacChangeCallback;
            InitializeComponent();
            DataContext = this;
            Addresses = new List<MacPointCollection>()
            {
                new MacPointCollection() { Address = "00:0c:09:ee:2e:cb", Colour = Color.Black },
                new MacPointCollection() { Address = "10:66:75:b4:0a:27", Colour = Color.Blue },
                new MacPointCollection() { Address = "28:e0:2c:32:04:40", Colour = Color.Red },
                new MacPointCollection() { Address = "2c:0e:3d:44:9a:08", Colour = Color.Green },
                new MacPointCollection() { Address = "38:2d:e8:74:19:26", Colour = Color.Blue },
            };

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

            KeyDown += delegate { if (Keyboard.IsKeyDown(Key.Delete)) RemoveMac(); };
            
            UpdateTable();
        }

        void RowLoad(object sender, DataGridRowEventArgs e)
        {
            if (e.Row.DataContext is MacPointCollection RowDataContaxt)
            {
                Color color = RowDataContaxt.Colour;
                bool dark = (color.R + color.G + color.B) / 3 < 128;
                e.Row.Background = new System.Windows.Media.SolidColorBrush(new System.Windows.Media.Color() { R = color.R, G = color.G, B = color.B, A = color.A });
                e.Row.Foreground = new System.Windows.Media.SolidColorBrush(dark ? System.Windows.Media.Colors.White : System.Windows.Media.Colors.Black);
            }
        }

        private void AddMacButtonPress(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(MacEntry.Text))
            {
                System.Windows.MessageBox.Show("Please enter a MAC address...", "Invalid MAC", MessageBoxButton.OK, MessageBoxImage.Error);
                MacEntry.Text = "";
                return;
            }

            if (!new Regex("^([0-9A-Fa-f]{2}[:]){5}([0-9A-Fa-f]{2})$").IsMatch(MacEntry.Text))
            {
                System.Windows.MessageBox.Show("Please enter a valid MAC address...", "Invalid MAC", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            ColorDialog colorDialog;
            if ((colorDialog = new ColorDialog()).ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Addresses.Add(new MacPointCollection { Address = MacEntry.Text, Colour = new Color { R = colorDialog.Color.R, G = colorDialog.Color.G, B = colorDialog.Color.B, A = colorDialog.Color.A }, MacPoints = new List<LocationPoint>() });
                MacChangeCallback?.Invoke();
                UpdateTable();

                MacEntry.Text = "";
            }
        }

        public void RemoveMac()
        {
            foreach (MacPointCollection pointCollection in MacSelectionDataGrid.SelectedItems)
                Addresses.Remove(pointCollection);
            MacChangeCallback?.Invoke();
            UpdateTable();
        }

        public void UpdateTable()
        {
            MacSelectionDataGrid.Items.Clear();
            foreach (MacPointCollection address in Addresses)
                MacSelectionDataGrid.Items.Add(address);
        }
    }
}
