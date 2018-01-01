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
        public bool TimeEnabled { get; set; }
        public bool Polling { get; protected set; }
        protected bool TimeAutomation { get; set; }
        private SelectionManagerWindow SelectionManagerWindow { get; }
        private TimeManagerWindow TimeManagerWindow { get; }

        public MapViewPage(Common common)
        {
            InitializeComponent();
            DataContext = this;

            MacPointCollections = new MacPointCollection[0];

            Common = common;
            TimeEnabled = false;
            TimeAutomation = false;

            ImageFiles = App.ImageIndex.ImageFiles;

            SelectionManagerWindow = new SelectionManagerWindow(Common, UpdatePoints);
            TimeManagerWindow = new TimeManagerWindow(UpdateTimedPoints);
        }

        public void OnClose()
        {
            SelectionManagerWindow.Close();
            TimeManagerWindow.Close();
        }

        private void UpdatePoints()
        {
            if (SelectedImageFile != null)
            {
                MacPointCollections = SelectionManagerWindow.Addresses.ToArray();

                foreach (MacPointCollection macPointCollection in MacPointCollections)
                    foreach (Table table in Common.LoadedDataTables)
                        foreach (Record record in table.GetRecords("MAC", macPointCollection.Address))
                            if (record.GetValue<string>("Deck") == SelectedImageFile.Identifier)
                                macPointCollection.MacPoints.Add(new LocationPoint { Point = new Vector2((float)record.GetValue<double>("X"), (float)record.GetValue<double>("Y")), Time = record.GetValue<DateTime>("Date").TimeOfDay });
                
                MapViewer.LoadPoints(MacPointCollections);
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
            if (TimeEnabled)
            {
                //TimeSpan selectedTime = TimeSpan.FromHours(hours);
                //TimeManagerWindow.SelectedTimeLabel.Content = selectedTime.ToString(@"hh\:mm\:ss");

                //LocationPoint[] activePoints = MacPointCollections.Where(point => point.Time > selectedTime).ToArray();
                //if (activePoints.Length > 0)
                //{
                //    TimeManagerWindow.DisplayedTimeLabel.Content = activePoints[0].Time.ToString(@"hh\:mm\:ss");
                //    MapViewer.LoadPoints(new Vector2[] { activePoints[0].Point });
                //}
                //else TimeManagerWindow.DisplayedTimeLabel.Content = "No Points";
            }
        }

        private void TimeDisabled(object sender, RoutedEventArgs e)
        {
            MapViewer.LoadPoints(MacPointCollections.ToArray());
        }

        private void UpdatePointsClick(object sender, RoutedEventArgs e)
        {
            UpdatePoints();
        }

        private void SelectDataClick(object sender, RoutedEventArgs e)
        {
            SelectionManagerWindow.Show();
        }

        private void SelectTimeClick(object sender, RoutedEventArgs e)
        {
            TimeManagerWindow.Show();
        }
    }
}