using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using LocationInterface.Utils;
using Microsoft.Xna.Framework;
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

        public void OnClose()
        {
            SelectionManagerWindow.Close();
            TimeManagerWindow.Close();
            TimeSetterWindow.Close();
        }

        private void UpdatePoints()
        {
            if (SelectedImageFile != null)
            {
                MacPointCollections = SelectionManagerWindow.Addresses.ToArray();

                foreach (Table table in Common.LoadedDataTables)
                    foreach (Record record in table.GetRecords("Deck", SelectedImageFile.Identifier))
                        foreach (MacPointCollection macPointCollection in MacPointCollections)
                            if (record.GetValue<string>("MAC") == macPointCollection.Address)
                            {
                                LocationRecord locationRecord = record.ToObject<LocationRecord>();
                                macPointCollection.MacPoints.Add(
                                    new LocationPoint
                                    {
                                        Point = locationRecord,
                                        Time = locationRecord.Date.TimeOfDay,
                                        Node = locationRecord.Location,
                                    });
                                break;
                            }
                if (!TimeManagerWindow.TimeEnabled) MapViewer.LoadPoints(MacPointCollections);
            }
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

        private void UpdateTimedPoints(double hours)
        {
            if (TimeManagerWindow.TimeEnabled)
            {
                TimeSpan selectedTime = TimeSpan.FromHours(hours);
                TimeManagerWindow.SelectedTimeLabel.Content = selectedTime.ToString(@"hh\:mm\:ss");

                MacPointCollection[] macPointCollections = new MacPointCollection[MacPointCollections.Length];
                LocationPoint[] laterPoints;
                for (int i = 0; i < macPointCollections.Length; i++)
                    macPointCollections[i] = new MacPointCollection()
                    {
                        Address = MacPointCollections[i].Address,
                        Colour = MacPointCollections[i].Colour,
                        MacPoints = (laterPoints = MacPointCollections[i].MacPoints.Where(point => point.Time < selectedTime).ToArray()).Length > 0
                                ? new List<LocationPoint> { laterPoints.Last() } : new List<LocationPoint>()
                    };

                MapViewer.LoadPoints(macPointCollections);
            }
        }

        private void TimeEnabledEvent(object sender, RoutedEventArgs e)
        {
            TimeSetterWindow.TimeSlider.IsEnabled = MapViewer.TimeBased = true;
        }

        private void TimeDisabledEvent(object sender, RoutedEventArgs e)
        {
            TimeSetterWindow.TimeSlider.IsEnabled = MapViewer.TimeBased = false;
            MapViewer.LoadPoints(MacPointCollections.ToArray());
        }

        private void ShowTimeManager(object sender, RoutedEventArgs e)
        {
            TimeManagerWindow.Show();
            TimeSetterWindow.Show();
        }

        private void HideTimeManager(object sender, RoutedEventArgs e)
        {
            TimeManagerWindow.Hide();
            TimeSetterWindow.Hide();
        }

        private void UpdatePointsClick(object sender, RoutedEventArgs e)
        {
            UpdatePoints();
        }

        private void SelectDataClick(object sender, RoutedEventArgs e)
        {
            SelectionManagerWindow.Show();
        }
    }
}
