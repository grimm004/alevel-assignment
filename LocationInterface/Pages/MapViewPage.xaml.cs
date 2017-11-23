using System;
using System.Collections.Generic;
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
using System.Runtime.InteropServices;
using System.Linq;

namespace LocationInterface.Pages
{
    /// <summary>
    /// Interaction logic for MapViewPage.xaml
    /// </summary>
    public partial class MapViewPage : Page
    {
        protected Image CurrentImage { get; set; }
        protected Action ShowHomePage { get; set; }
        protected List<LocationPoint> LocationPoints { get; set; }
        protected List<Utils.Point> ActivePoints { get; set; }
        protected static Camera Camera { get; set; }
        protected Common Common { get; }
        protected bool LoadingPoints { get; private set; }
        protected KeyBind SKeyBind { get; set; }
        protected ImageFile SelectedImageFile { get; set; }
        public List<ImageFile> ImageFiles { get; private set; }
        public string SelectedMacAddress { get; set; }
        public bool TimeEnabled { get; set; }
        public bool Polling { get; protected set; }
        public bool PageLoaded { get { return Common.CurrentPage.GetType() == typeof(MapViewPage); } }

        public MapViewPage(Common common, Action ShowHomePage)
        {
            InitializeComponent();
            DataContext = this;

            ImageFiles = App.ImageIndex.ImageFiles;
            SelectedMacAddress = "00:19:94:32:fc:20";

            Common = common;
            this.ShowHomePage = ShowHomePage;

            ImageFiles = App.ImageIndex.ImageFiles;

            SKeyBind = new KeyBind(Key.S, SaveInfo);

            LocationPoints = new List<Utils.LocationPoint>();
            LoadingPoints = false;
            Camera = new Camera();

            KeyDown += KeyPress;
        }

        public void StartPolling()
        {
            Polling = true;
            new Thread(() =>
            {
                while (PageLoaded && Polling)
                    try
                    {
                        if (ApplicationIsActivated()) Dispatcher.Invoke(Update);
                        Thread.Sleep(1000 / Constants.MAPUPS);
                    }
                    catch (ThreadAbortException) { }

                Polling = false;
            })
            { IsBackground = true, }.Start();
        }

        public void StopPolling()
        {
            Polling = false;
        }

        protected void SaveInfo()
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
                App.ImageIndex.SaveIndex();
        }

        protected void KeyPress(object sender, KeyEventArgs e)
        {
            if (ApplicationIsActivated() && PageLoaded)
                foreach (Key key in new Key[] { Key.A, Key.D, Key.W, Key.S, Key.F, Key.G, Key.H, Key.T, Key.R, Key.Y, Key.Up, Key.Down, Key.Left, Key.Right })
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

            if (!LoadingPoints) foreach (LocationPoint locationPoint in LocationPoints) locationPoint.Point.Update(Camera, SelectedImageFile.Offset, SelectedImageFile.Multiplier);

            if (CurrentImage != null) Canvas.SetLeft(CurrentImage, Camera.Position.X);
            if (CurrentImage != null) Canvas.SetTop(CurrentImage, Camera.Position.Y);
        }

        protected void Scale(double change)
        {
            SelectedImageFile.Multiplier += change;
        }
        protected void Translate(double x, double y)
        {
            SelectedImageFile.Offset += new Vector2(x, y);
        }

        public void LoadTables(ImageFile imageFile)
        {
            try
            {
                Stopwatch timer = Stopwatch.StartNew();
                LoadingPoints = true;
                LocationPoints = new List<LocationPoint>();
                Console.WriteLine("Loading tables...");
                foreach (Table table in Common.LoadedDataTables)
                {
                    Stopwatch tableTimer = Stopwatch.StartNew();
                    Record[] records = table.GetRecords("MAC", SelectedMacAddress);
                    tableTimer.Stop();
                    Console.WriteLine($"Loaded { records.Length } records with MAC address '{ SelectedMacAddress }' of { table.RecordCount } records in { (tableTimer.ElapsedMilliseconds / 1000d).ToString("0.000") } seconds.");
                    for (int i = 0; i < records.Length; i++)
                        if (records[i].GetValue<string>("Deck") == SelectedImageFile.Identifier)
                            LocationPoints.Add(new LocationPoint() { Point = new Utils.Point(records[i].GetValue<double>("X"), records[i].GetValue<double>("Y")), Time = records[i].GetValue<DateTime>("Date").TimeOfDay });
                }
                timer.Stop();
                Console.WriteLine($"Added { LocationPoints.Count } points from { Common.LoadedDataTables.Length } table(s) in { (timer.ElapsedMilliseconds / 1000d).ToString("0.000") } seconds.");

                Dispatcher.Invoke(delegate
                {
                    ImageSource image = new BitmapImage(new Uri($"{ SettingsManager.Active.ImageFolder }\\{ SelectedImageFile.FileName }", UriKind.Relative));
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

                    foreach (LocationPoint locationPoint in LocationPoints)
                    {
                        locationPoint.Point.SetEllipse(new Ellipse() { Width = 5, Height = 5, Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0x00, 0x00)) });
                        canvas.Children.Add(locationPoint.Point.Ellipse);
                    }
                    Console.WriteLine("Added {0} points to the canvas.", LocationPoints.Count);
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
            if (SelectedImageFile != null && SelectedMacAddress.Length == 17)
                new Thread(() => LoadTables(SelectedImageFile)) { IsBackground = true }.Start();
        }
        private void DeckSelectionComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedImageFile = (ImageFile)((ComboBox)sender).SelectedItem;
            if (SelectedImageFile != null && SelectedMacAddress.Length == 17)
                new Thread(() => LoadTables(SelectedImageFile)) { IsBackground = true }.Start();
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
            Console.WriteLine(time);
            TimeSpan selectedTime = TimeSpan.FromHours(time);
            Utils.Point lastKnownPoint = null;
            foreach (LocationPoint locationPoint in LocationPoints)
            {
                //Console.WriteLine(locationPoint.Time < selectedTime);
                if (locationPoint.Time < selectedTime) { lastKnownPoint = locationPoint.Point; break; }
            }
            
            if (lastKnownPoint != null) ActivePoints = new List<Utils.Point>() { lastKnownPoint };
            else ActivePoints = new List<Utils.Point>();
        }

        private void TimeEnabledChecked(object sender, RoutedEventArgs e)
        {
            UpdateTimedPoints(0);
        }

        private void TimeEnabledUnchecked(object sender, RoutedEventArgs e)
        {
            ActivePoints = LocationPoints.Select(locationPoint => locationPoint.Point).ToList();
        }
    }
}
