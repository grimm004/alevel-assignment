using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using LocationInterface.Utils;
using System.Linq;
using DatabaseManagerLibrary;
using LocationInterface.Windows;

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
        protected MacPointCollection[] MacPointCollections { get; set; }
        public bool Polling { get; protected set; }
        protected bool TimeAutomation { get; set; }
        private SelectionManagerWindow SelectionManagerWindow { get; }
        private TimeManagerWindow TimeManagerWindow { get; }
        private TimeSetterWindow TimeSetterWindow { get; }

        public MapViewPage(Common common)
        {
            InitializeComponent();
            DataContext = this;

            MacPointCollections = new MacPointCollection[0];

            Common = common;
            TimeAutomation = false;

            ImageFiles = App.ImageIndex.ImageFiles;

            SelectionManagerWindow = new SelectionManagerWindow(Common, UpdatePoints);
            TimeSetterWindow = new TimeSetterWindow(UpdateTimedPoints);
            TimeManagerWindow = new TimeManagerWindow(TimeSetterWindow.AutoTimeChange, TimeEnabledEvent, TimeDisabledEvent);
        }

        /// <summary>
        /// Clean up the map page
        /// </summary>
        public void OnClose()
        {
            // Close the non-dialog windows
            SelectionManagerWindow.Close();
            TimeManagerWindow.Close();
            TimeSetterWindow.Close();
        }

        /// <summary>
        /// Update all the points being used in the map-viewer
        /// </summary>
        private void UpdatePoints()
        {
            // Check that there is a selected image file
            if (SelectedImageFile != null)
            {
                // Get the selected MAC addresses
                MacPointCollections = SelectionManagerWindow.Addresses.ToArray();

                // Loop through each selected table
                foreach (Table table in Common.LoadedDataTables)
                    // Loop through each record in the current selected table where the deck is equal to the current deck map's ID
                    foreach (Record record in table.GetRecords("Deck", SelectedImageFile.Identifier))
                        // Loop through each selected MAC address
                        foreach (MacPointCollection macPointCollection in MacPointCollections)
                            // If the current deck MAC address is equal to the current selected MAC address
                            if (record.GetValue<string>("MAC") == macPointCollection.Address)
                            {
                                // Get the location record representation of the location point
                                LocationRecord locationRecord = record.ToObject<LocationRecord>();
                                // Implicitly cast this to a LocationPoint and add it to the list of active macPointCollection
                                macPointCollection.MacPoints.Add(locationRecord);
                                // Break out of the loop as there is no need to continue searching the MAC addresses
                                break;
                            }
                // If the map viewer is not in timed mode, show all loaded points
                if (!TimeManagerWindow.TimeEnabled) MapViewer.LoadPoints(MacPointCollections);
            }
        }

        /// <summary>
        /// Show the home page
        /// </summary>
        /// <param name="sender">Control that called the event</param>
        /// <param name="e">Information about the event</param>
        private void BackButtonClick(object sender, RoutedEventArgs e)
        {
            Common.ShowHomePage();
        }
        
        /// <summary>
        /// Update the deck map when changed
        /// </summary>
        /// <param name="sender">The control that called the action</param>
        /// <param name="e">Information about the selection change</param>
        private void DeckSelectionComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Fetch the new selectedimagefile instance
            SelectedImageFile = ((ImageFile)((ComboBox)sender).SelectedValue);
            // Load the image in the map
            MapViewer.LoadMap(SelectedImageFile);
        }

        /// <summary>
        /// Update the timed points
        /// </summary>
        /// <param name="hour">The hour of the day to get points for</param>
        private void UpdateTimedPoints(double hour)
        {
            // Check that the time mode is enabled
            if (TimeManagerWindow.TimeEnabled)
            {
                // Get a TimeSpan object for the selected hour
                TimeSpan selectedTime = TimeSpan.FromHours(hour);
                // Set the selected time label to a representation of the current selected time
                TimeManagerWindow.SelectedTimeLabel.Content = selectedTime.ToString(@"hh\:mm\:ss");

                // Produce a local MacPointCollection array to store the latest MacPointLocations
                MacPointCollection[] macPointCollections = new MacPointCollection[MacPointCollections.Length];
                // Pre-define the laterPoints (to temporarily store the points to be used)
                LocationPoint[] laterPoints;
                // Loop through the current macpointcollections
                for (int i = 0; i < macPointCollections.Length; i++)
                    // Set the current temporary macPointCollections item with up to a single point that represents the most available location point
                    macPointCollections[i] = new MacPointCollection()
                    {
                        Address = MacPointCollections[i].Address,
                        Colour = MacPointCollections[i].Colour,
                        MacPoints = (laterPoints = MacPointCollections[i].MacPoints.Where(point => point.Time < selectedTime).ToArray()).Length > 0
                                ? new List<LocationPoint> { laterPoints.Last() } : new List<LocationPoint>()
                    };

                // Load these points into the map viewer
                MapViewer.LoadPoints(macPointCollections);
            }
        }

        /// <summary>
        /// Run when time-mode is enabled
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimeEnabledEvent(object sender, RoutedEventArgs e)
        {
            TimeSetterWindow.TimeSlider.IsEnabled = MapViewer.TimeBased = true;
        }

        /// <summary>
        /// Run when time-mode is disabled
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimeDisabledEvent(object sender, RoutedEventArgs e)
        {
            TimeSetterWindow.TimeSlider.IsEnabled = MapViewer.TimeBased = false;
            // Load all of the available points
            MapViewer.LoadPoints(MacPointCollections.ToArray());
        }

        /// <summary>
        /// Show the time management windows
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowTimeManager(object sender, RoutedEventArgs e)
        {
            TimeManagerWindow.Show();
            TimeSetterWindow.Show();
        }

        /// <summary>
        /// Hide the time management windows
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HideTimeManager(object sender, RoutedEventArgs e)
        {
            TimeManagerWindow.Hide();
            TimeSetterWindow.Hide();
        }

        /// <summary>
        /// Update all of the points
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdatePointsClick(object sender, RoutedEventArgs e)
        {
            UpdatePoints();
        }

        /// <summary>
        /// Show the MAC selection window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectDataClick(object sender, RoutedEventArgs e)
        {
            SelectionManagerWindow.Show();
        }
    }
}
