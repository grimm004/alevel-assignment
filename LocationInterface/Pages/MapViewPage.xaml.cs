using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Threading;
using System.IO;
using DatabaseManagerLibrary;
using LocationInterface.Utils;
using System.Collections.ObjectModel;

namespace LocationInterface.Pages
{
    /// <summary>
    /// Interaction logic for MapViewPage.xaml
    /// </summary>
    public partial class MapViewPage : Page
    {
        protected Image CurrentImage { get; set; }
        protected Action ShowHomePage { get; set; }
        protected List<Utils.Point> Points { get; set; }
        protected static Camera Camera { get; set; }
        protected Common Common { get; }
        protected bool LoadingPoints { get; private set; }
        protected KeyBind SKeyBind { get; set; }
        protected ImageFile CurrentImageFile { get; set; }

        public List<ImageFile> Tests { get; protected set; }

        public MapViewPage(Common common, Action ShowHomePage)
        {
            Common = common;
            this.ShowHomePage = ShowHomePage;

            Tests = App.ImageIndex.ImageFiles;

            SKeyBind = new KeyBind(Key.S, SaveInfo);

            Points = new List<Utils.Point>();
            LoadingPoints = false;
            Camera = new Camera();

            InitializeComponent();

            KeyDown += KeyPress;

            Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        Dispatcher.Invoke(Update);
                        Thread.Sleep(1000 / 30);
                    }
                    catch (TaskCanceledException) { }
                }
            });
        }

        protected void SaveInfo()
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
                App.ImageIndex.SaveIndex();
        }

        protected Key[] RefocusCanvasKeys = new Key[] { Key.A, Key.D, Key.W, Key.S, Key.F, Key.G, Key.H, Key.T, Key.R, Key.Y, Key.Up, Key.Down, Key.Left, Key.Right };
        protected void KeyPress(object sender, KeyEventArgs e)
        {
            foreach (Key key in RefocusCanvasKeys)
                if (e.Key == key)
                {
                    Keyboard.Focus(canvas);
                    canvas.Focus();
                    e.Handled = true;
                    break;
                }
        }

        protected void Update()
        {
            SKeyBind.Update();
            bool shiftDown = Keyboard.IsKeyDown(Key.LeftShift);
            if (Keyboard.IsKeyDown(Key.A)) Camera.Move(5, 0);
            if (Keyboard.IsKeyDown(Key.D)) Camera.Move(-5, 0);
            if (Keyboard.IsKeyDown(Key.W)) Camera.Move(0, 5);
            if (Keyboard.IsKeyDown(Key.S) && !Keyboard.IsKeyDown(Key.LeftCtrl)) Camera.Move(0, -5);
            if (Keyboard.IsKeyDown(Key.R)) Scale(shiftDown ? -.05 : -.001);
            if (Keyboard.IsKeyDown(Key.Y)) Scale(shiftDown ? +.05 : +.001);
            if (Keyboard.IsKeyDown(Key.T)) Translate(0, shiftDown ? -4 : -1);
            if (Keyboard.IsKeyDown(Key.F)) Translate(shiftDown ? -4 : -1, 0);
            if (Keyboard.IsKeyDown(Key.G)) Translate(0, shiftDown ? +4 : +1);
            if (Keyboard.IsKeyDown(Key.H)) Translate(shiftDown ? +4 : +1, 0);

            if (!LoadingPoints) foreach (Utils.Point point in Points) point.Update(Camera, CurrentImageFile.Offset, CurrentImageFile.Multiplier);

            if (CurrentImage != null) Canvas.SetLeft(CurrentImage, Camera.Position.X);
            if (CurrentImage != null) Canvas.SetTop(CurrentImage, Camera.Position.Y);
        }

        protected void Scale(double change)
        {
            CurrentImageFile.Multiplier += change;
        }
        protected void Translate(double x, double y)
        {
            CurrentImageFile.Offset += new Vector2(x, y);
        }

        public void LoadTables()
        {
            int currentDeck = deckSelectionComboBox.SelectedIndex + 1;
            string macAddress = macAddressEntry.Text;
            Task.Run(() => LoadTables(currentDeck, macAddress));
        }
        private void LoadTables(int deckNumber, string macAddress)
        {
            if (CurrentImageFile != null)
                try
                {
                    CurrentImageFile = App.ImageIndex.GetDataFile($"Deck{ deckNumber }");

                    Stopwatch timer = Stopwatch.StartNew();
                    LoadingPoints = true;
                    Points = new List<Utils.Point>();
                    Console.WriteLine("Loading tables...");
                    foreach (Table table in Common.LoadedDataTables)
                    {
                        Stopwatch tableTimer = Stopwatch.StartNew();
                        Record[] records = table.GetRecords("MAC", macAddress);
                        tableTimer.Stop();
                        Console.WriteLine("Loaded {0} of {1} records in {2:0.000} seconds.", records.Length, table.RecordCount, tableTimer.ElapsedMilliseconds / 1000d);
                        for (int i = 0; i < records.Length; i++)
                            if (records[i].GetValue<string>("Deck") == CurrentImageFile.Identifier)
                                Points.Add(new Utils.Point(records[i].GetValue<double>("X"), records[i].GetValue<double>("Y")));
                    }
                    timer.Stop();
                    Console.WriteLine("Added {0} points from {1} table(s) in {2:0.000} seconds.", Points.Count, Common.LoadedDataTables.Length, timer.ElapsedMilliseconds / 1000d);

                    Dispatcher.Invoke(delegate
                    {
                        ImageSource image = new BitmapImage(new Uri($"{ SettingsManager.Active.ImageFolder }\\{ CurrentImageFile.FileName }", UriKind.Relative));
                        CurrentImage = new Image
                        {
                            Width = image.Width,
                            Height = image.Height,
                            Name = "DeckImage",
                            Source = image,
                        };
                        canvas.Children.Clear();
                        canvas.Children.Add(CurrentImage);
                        Canvas.SetLeft(CurrentImage, Camera.Position.X);
                        Canvas.SetTop(CurrentImage, Camera.Position.Y);

                        foreach (Utils.Point point in Points)
                        {
                            point.SetEllipse(new Ellipse() { Width = 5, Height = 5, Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0x00, 0x00)) });
                            canvas.Children.Add(point.Ellipse);
                        }
                        Console.WriteLine("Added {0} points to the canvas.", Points.Count);
                    });
                }
                catch (FileNotFoundException)
                {
                    MessageBox.Show("Deck image file could not be found.", "File Not Found");
                }
                finally
                {
                    LoadingPoints = false;
                }
        }

        private void BackButtonClick(object sender, RoutedEventArgs e)
        {
            ShowHomePage();
        }
        private void MacAddressEntryTextChanged(object sender, TextChangedEventArgs e)
        {
            if (deckSelectionComboBox != null && macAddressEntry.Text.Length == 17)
                LoadTables();
        }
        private void DeckSelectionComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (macAddressEntry != null) LoadTables();
        }
    }
}
