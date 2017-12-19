using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;
using LocationInterface.Utils;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;

namespace LocationInterface.Pages
{
    /// <summary>
    /// Interaction logic for MapViewPage.xaml
    /// </summary>
    public partial class MapViewPage : Page
    {
        protected Image CurrentImage { get; set; }
        protected Common Common { get; }
        protected ImageFile SelectedImageFile { get; set; }
        public List<ImageFile> ImageFiles { get; private set; }
        public string SelectedMacAddress { get; set; }
        public bool TimeEnabled { get; set; }
        public bool Polling { get; protected set; }
        public bool PageLoaded { get { return Common.CurrentPage.GetType() == typeof(MapViewPage); } }

        public MapViewPage(Common common)
        {
            InitializeComponent();
            DataContext = this;

            ImageFiles = App.ImageIndex.ImageFiles;
            SelectedMacAddress = "00:19:94:32:fc:20";

            Common = common;

            ImageFiles = App.ImageIndex.ImageFiles;
        }

        protected void Scale(double change)
        {
            SelectedImageFile.Multiplier += change;
        }
        protected void Translate(double x, double y)
        {
            SelectedImageFile.Offset += new Vector2((float)x, (float)y);
        }

        // tabled: Common.LoadedDataTables
        // deck: SelectedImageFile.Identifier
        // X: (int)records[i].GetValue<double>("X")
        // Y: (int)records[i].GetValue<double>("Y")
        // time: records[i].GetValue<DateTime>("Date").TimeOfDay
        // image: $"{ SettingsManager.Active.ImageFolder }\\{ SelectedImageFile.FileName }"
        // records: table.GetRecords("MAC", SelectedMacAddress)

        private void BackButtonClick(object sender, RoutedEventArgs e)
        {
            Common.ShowHomePage();
        }
        private void MacAddressEntryTextChanged(object sender, TextChangedEventArgs e)
        {
            //if (SelectedImageFile != null && SelectedMacAddress.Length == 17)
            //    new Thread(() => LoadTables(SelectedImageFile)) { IsBackground = true }.Start();
        }
        private void DeckSelectionComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //SelectedImageFile = (ImageFile)((ComboBox)sender).SelectedItem;
            //if (SelectedImageFile != null && SelectedMacAddress.Length == 17)
            //    new Thread(() => LoadTables(SelectedImageFile)) { IsBackground = true }.Start();
        }

        /// <summary>
        /// Check if the application is focused
        /// </summary>
        /// <returns>true if the application is focused</returns>
        public static bool ApplicationIsActivated()
        {
            IntPtr activatedHandle = GetForegroundWindow();
            if (activatedHandle == IntPtr.Zero)
                // No window is currently activated
                return false;

            int procId = Process.GetCurrentProcess().Id;
            GetWindowThreadProcessId(activatedHandle, out int activeProcId);

            return activeProcId == procId;
        }

        /// <summary>
        /// Get the a pointer to the focused window
        /// </summary>
        /// <returns>an int pointer referencing the process id of the foreground window</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern IntPtr GetForegroundWindow();

        /// <summary>
        /// Get the id of a process
        /// </summary>
        /// <param name="handle">An integer pointer to the process to id</param>
        /// <param name="processId">A reference to the output id</param>
        /// <returns>a number referencing the outcome of the operation</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);

        private void SliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (TimeEnabled)
                UpdateTimedPoints(((Slider)sender).Value);
        }

        private void UpdateTimedPoints(double dayRatio)
        {
            double time = dayRatio * 24d;
            TimeSpan selectedTime = TimeSpan.FromHours(time);
            Console.WriteLine(selectedTime);
        }

        private void TimeEnabledChecked(object sender, RoutedEventArgs e)
        {
            UpdateTimedPoints(0);
        }

        private void TimeEnabledUnchecked(object sender, RoutedEventArgs e)
        {
            //ActivePoints = LocationPoints.Select(locationPoint => locationPoint.Point).ToList();
        }
    }
}
