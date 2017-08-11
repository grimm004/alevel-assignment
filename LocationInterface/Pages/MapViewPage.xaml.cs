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
using DatabaseManager;

namespace LocationInterface.Pages
{
    /// <summary>
    /// Interaction logic for MapViewPage.xaml
    /// </summary>
    public partial class MapViewPage : Page
    {
        protected Image currentImage;
        protected Action ShowHomePage;
        private Database database;
        protected List<Point> points;
        protected double multiplier;
        protected Vector2 offset;
        public static Camera Camera;
        public Table[] LoadedTables;

        public MapViewPage(Action ShowHomePage)
        {
            this.ShowHomePage = ShowHomePage;

            database = new BINDatabase("C:\\BINData", false);
            LoadedTables = new Table[] { database.GetTable("TUI_D1_location_data_03-12-2017"), };
            points = new List<Point>();
            offset = new Vector2(1);
            multiplier = 1;
            Camera = new Camera();

            InitializeComponent();

            for (int i = 0; i < 11; i++) ((ComboBoxItem)deckSelectionComboBox.Items[i]).IsEnabled = File.Exists(string.Format("Images\\Deck{0}.bmp", i + 1));

            Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        Dispatcher.Invoke(Update);
                        Thread.Sleep((int)(1000 / 60));
                    }
                    catch (TaskCanceledException) { }
                }
            });
        }

        protected void Update()
        {
            bool shiftDown = Keyboard.IsKeyDown(Key.LeftShift);
            if (Keyboard.IsKeyDown(Key.A)) Camera.Move(5, 0);
            if (Keyboard.IsKeyDown(Key.D)) Camera.Move(-5, 0);
            if (Keyboard.IsKeyDown(Key.W)) Camera.Move(0, 5);
            if (Keyboard.IsKeyDown(Key.S)) Camera.Move(0, -5);
            if (Keyboard.IsKeyDown(Key.R)) Scale(shiftDown ? -.05 : -.001);
            if (Keyboard.IsKeyDown(Key.Y)) Scale(shiftDown ? +.05 : +.001);
            if (Keyboard.IsKeyDown(Key.T)) Translate(0, shiftDown ? -4 : -1);
            if (Keyboard.IsKeyDown(Key.F)) Translate(shiftDown ? -4 : -1, 0);
            if (Keyboard.IsKeyDown(Key.G)) Translate(0, shiftDown ? +4 : +1);
            if (Keyboard.IsKeyDown(Key.H)) Translate(shiftDown ? +4 : +1, 0);

            foreach (Point point in points) point.Update(offset, multiplier);

            if (currentImage != null) Canvas.SetLeft(currentImage, Camera.Position.X);
            if (currentImage != null) Canvas.SetTop(currentImage, Camera.Position.Y);
        }

        protected void Scale(double change)
        {
            multiplier += change;
        }
        protected void Translate(double x, double y)
        {
            offset += new Vector2(x, y);
        }

        private void DeckSelectionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int currentDeck = deckSelectionComboBox.SelectedIndex + 1;
            string macAddress = macAddressEntry.Text;
            Task.Run(() => LoadRecords(currentDeck, macAddress));
        }

        private void LoadRecords(int deckNumber, string macAddress)
        {
            try
            {
                // Load Points
                Stopwatch timer = Stopwatch.StartNew();
                points = new List<Point>();
                foreach (Table table in LoadedTables)
                {
                    Stopwatch tableTimer = Stopwatch.StartNew();
                    Record[] records = table.GetRecords("MAC", macAddress);
                    tableTimer.Stop();
                    Console.WriteLine("Loaded {0} of {1} records in {2:0.000} seconds.", records.Length, table.RecordCount, tableTimer.ElapsedMilliseconds / 1000d);
                    for (int i = 0; i < records.Length; i++)
                        if (records[i].GetValue<string>("Deck") == string.Format("Deck{0}", deckNumber)) if (records[i].GetValue<string>("Locationid") == "POO000447DEGSB") points.Add(new Point(records[i].GetValue<double>("X"), records[i].GetValue<double>("Y"), new Ellipse() { Width = 5, Height = 5, Fill = new SolidColorBrush(Color.FromRgb(0x00, 0x00, 0xFF)) }));
                            else points.Add(new Point(records[i].GetValue<double>("X"), records[i].GetValue<double>("Y")));
                }
                timer.Stop();
                Console.WriteLine("Added {0} points in {1:0.000} seconds.", points.Count, timer.ElapsedMilliseconds / 1000d);

                Dispatcher.Invoke(delegate
                {
                    ImageSource image = new BitmapImage(new Uri(string.Format("Images\\Deck{0}.bmp", deckNumber), UriKind.Relative));
                    currentImage = new Image
                    {
                        Width = image.Width,
                        Height = image.Height,
                        Name = "DeckImage",
                        Source = image,
                    };
                    canvas.Children.Clear();
                    canvas.Children.Add(currentImage);
                    Canvas.SetLeft(currentImage, Camera.Position.X);
                    Canvas.SetTop(currentImage, Camera.Position.Y);

                    foreach (Point point in points)
                    {
                        point.SetEllipse(new Ellipse() { Width = 5, Height = 5, Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0x00, 0x00)) });
                        canvas.Children.Add(point.Ellipse);
                    }
                    Console.WriteLine("Added {0} points to the canvas.", points.Count);
                });
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("Deck image file could not be found.", "File Not Found");
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            ShowHomePage();
        }

        private void MacAddressEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            int currentDeck = deckSelectionComboBox != null ? deckSelectionComboBox.SelectedIndex + 1 : 0;
            string macAddress = macAddressEntry.Text;
            if (deckSelectionComboBox != null && macAddressEntry.Text.Length == 17)
                Task.Run(() => LoadRecords(currentDeck, macAddress));
        }
    }

    public class Camera
    {
        public Vector2 Position { get; set; }

        public Camera()
        {
            Position = new Vector2(0, 0);
        }

        public void SetPos(double x, double y)
        {
            Position.X = x;
            Position.Y = y;
        }
        public void SetPos(Vector2 position)
        {
            Position = position;
        }

        public void Move(double x, double y)
        {
            Position.X += x;
            Position.Y += y;
        }
        public void Move(Vector2 position)
        {
            Position += position;
        }
    }

    public class Point
    {
        protected Vector2 OriginalPosition { get; set; }
        protected Vector2 Position { get; set; }
        protected bool Initialised { get; set; }
        public Ellipse Ellipse { get; protected set; }

        public Point()
        {
            OriginalPosition = Vector2.Zero;
            Position = Vector2.Zero;
        }
        public Point(Vector2 position)
        {
            OriginalPosition = position.Copy;
            Position = position.Copy;
        }
        public Point(double x, double y)
        {
            OriginalPosition = new Vector2(x, y);
            Position = new Vector2(x, y);
        }
        public Point(Vector2 position, Ellipse ellipse)
        {
            OriginalPosition = position.Copy;
            Position = position.Copy;
            Ellipse = ellipse;
            Canvas.SetLeft(Ellipse, Position.X);
            Canvas.SetTop(Ellipse, Position.Y);
        }
        public Point(double x, double y, Ellipse ellipse)
        {
            OriginalPosition = new Vector2(x, y);
            Position = new Vector2(x, y);
            Ellipse = ellipse;
            Canvas.SetLeft(Ellipse, Position.X);
            Canvas.SetTop(Ellipse, Position.Y);
        }

        public void SetEllipse(Ellipse ellipse)
        {
            Ellipse = ellipse;
            Canvas.SetLeft(Ellipse, Position.X);
            Canvas.SetTop(Ellipse, Position.Y);
            Initialised = true;
        }

        public void Update(Vector2 offset, double multiplier)
        {
            Position = (multiplier * OriginalPosition) + MapViewPage.Camera.Position + offset;
            if (Initialised)
            {
                Canvas.SetLeft(Ellipse, Position.X);
                Canvas.SetTop(Ellipse, Position.Y);
            }
        }
    }

    public class Vector2
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Vector2() { X = Y = 0; }
        public Vector2(double value) { X = Y = value; }
        public Vector2(double x, double y) { X = x; Y = y; }

        public Vector2 Copy { get { return new Vector2(X, Y); } }

        public static Vector2 operator *(double left, Vector2 right)
        {
            return new Vector2(left * right.X, left * right.Y);
        }
        public static Vector2 operator +(Vector2 left, Vector2 right)
        {
            return new Vector2(left.X + right.X, left.Y + right.Y);
        }
        public static Vector2 Zero
        {
            get { return new Vector2(); }
        }
    }
}