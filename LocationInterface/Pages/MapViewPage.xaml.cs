using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;
using LocationInterface.Utils;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using System.Linq;
using DatabaseManagerLibrary;

namespace LocationInterface.Pages
{
    /// <summary>
    /// Interaction logic for MapViewPage.xaml
    /// </summary>
    public partial class MapViewPage : Page
    {
        protected Common Common { get; }
        protected ImageFile SelectedImageFile { get; set; }
        public List<ImageFile> ImageFiles { get; private set; }
        protected List<LocationPoint> Points { get; set; }
        public string SelectedMacAddress { get; set; }
        public bool TimeEnabled { get; set; }
        public bool Polling { get; protected set; }
        public bool PageLoaded { get { return Common.CurrentPage.GetType() == typeof(MapViewPage); } }

        public MapViewPage(Common common)
        {
            InitializeComponent();
            DataContext = this;
            
            SelectedMacAddress = "00:19:94:32:fc:20";

            Points = new List<LocationPoint>();

            Common = common;
            TimeEnabled = false;

            ImageFiles = App.ImageIndex.ImageFiles;
        }

        private void BackButtonClick(object sender, RoutedEventArgs e)
        {
            Common.ShowHomePage();
        }
        
        private void DeckSelectionComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedImageFile = ((ImageFile)((ComboBox)sender).SelectedValue);
            MapViewer.LoadMap(SelectedImageFile);
        }

        private void SliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (TimeEnabled)
                UpdateTimedPoints(((Slider)sender).Value);
        }

        private void UpdateTimedPoints(double dayRatio)
        {
            TimeSpan selectedTime = TimeSpan.FromHours(dayRatio * 24d);
            SelectedTimeLabel.Content = selectedTime.ToString();

            LocationPoint[] activePoints = Points.Where(point => point.Time > selectedTime).ToArray();
            if (activePoints.Length > 0)
            {
                DisplayedTimeLabel.Content = activePoints[0].Time.ToString();
                MapViewer.LoadPoints(new Vector2[] { activePoints[0].Point });
            }
            else DisplayedTimeLabel.Content = "No Points";
        }

        private void UpdateButtonClick(object sender, RoutedEventArgs e)
        {
            if (SelectedImageFile != null)
            {
                Points.Clear();
                foreach (Table table in Common.LoadedDataTables)
                    foreach (Record record in table.GetRecords("MAC", SelectedMacAddress))
                        if (record.GetValue<string>("Deck") == SelectedImageFile.Identifier)
                            Points.Add(new LocationPoint { Point = new Vector2((float)record.GetValue<double>("X"), (float)record.GetValue<double>("Y")), Time = record.GetValue<DateTime>("Date").TimeOfDay });
                if (!TimeEnabled) MapViewer.LoadPoints(Points.Select(point => point.Point).ToArray());
            }
        }

        private void TimeEnabledChecked(object sender, RoutedEventArgs e)
        {
            TimeEnabled = true;
        }

        private void TimeEnabledUnchecked(object sender, RoutedEventArgs e)
        {
            TimeEnabled = false;
            MapViewer.LoadPoints(Points.Select(point => point.Point).ToArray());
        }
    }
}

///// <summary>
///// Check if the application is focused
///// </summary>
///// <returns>true if the application is focused</returns>
//public static bool ApplicationIsActivated()
//{
//    IntPtr activatedHandle = GetForegroundWindow();
//    if (activatedHandle == IntPtr.Zero)
//        // No window is currently activated
//        return false;

//    int procId = Process.GetCurrentProcess().Id;
//    GetWindowThreadProcessId(activatedHandle, out int activeProcId);

//    return activeProcId == procId;
//}

///// <summary>
///// Get the a pointer to the focused window
///// </summary>
///// <returns>an int pointer referencing the process id of the foreground window</returns>
//[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
//private static extern IntPtr GetForegroundWindow();

///// <summary>
///// Get the id of a process
///// </summary>
///// <param name="handle">An integer pointer to the process to id</param>
///// <param name="processId">A reference to the output id</param>
///// <returns>a number referencing the outcome of the operation</returns>
//[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
//private static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);
