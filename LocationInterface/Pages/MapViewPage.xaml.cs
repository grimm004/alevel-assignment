using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Threading;
using System.IO;

namespace LocationInterface.Pages
{
    /// <summary>
    /// Interaction logic for MapViewPage.xaml
    /// </summary>
    public partial class MapViewPage : Page
    {
        protected Image currentImage;
        protected Action ShowHomePage;
        private DatabaseManager.Database database;
        protected List<Point> points;
        protected double multiplier;
        protected Vector2 offset;
        public static Camera Camera;

        public MapViewPage(Action ShowHomePage)
        {
            this.ShowHomePage = ShowHomePage;

            database = new DatabaseManager.CSVDatabase("C:\\Data", false, ".csv");
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
            if (Keyboard.IsKeyDown(Key.Q)) Scale(shiftDown ? -.05 : -.001);
            if (Keyboard.IsKeyDown(Key.E)) Scale(shiftDown ? +.05 : +.001);
            if (Keyboard.IsKeyDown(Key.T)) Translate(0, shiftDown ? -4 : -1);
            if (Keyboard.IsKeyDown(Key.F)) Translate(shiftDown ? -4 : -1, 0);
            if (Keyboard.IsKeyDown(Key.G)) Translate(0, shiftDown ? +4 : +1);
            if (Keyboard.IsKeyDown(Key.H)) Translate(shiftDown ? +4 : +1, 0);

            foreach (Point point in points) point.Update(offset, multiplier);

            Canvas.SetLeft(currentImage, Camera.Position.X);
            Canvas.SetTop(currentImage, Camera.Position.Y);
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
            try
            {
                // Load Image
                ImageSource image = new BitmapImage(new Uri(string.Format("Images\\Deck{0}.bmp", deckSelectionComboBox.SelectedIndex + 1), UriKind.Relative));

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

                // Load Points
                DatabaseManager.Table table = database.GetTable("TUI_D1_location_data_03-12-2017");
                DatabaseManager.Record[] records = table.GetRecords("mac", macAddressEntry.Text);
                Console.WriteLine("Loaded {0} records.", records.Length);
                points = new List<Point>();
                for (int i = 0; i < records.Length; i++)
                    if ((string)records[i].GetValue("deck") == string.Format("Deck{0}", deckSelectionComboBox.SelectedIndex + 1)) if ((string)records[i].GetValue("locationid") == "POO000447DEGSB") points.Add(new Point((double)records[i].GetValue("x"), (double)records[i].GetValue("y"), new Ellipse() { Width = 5, Height = 5, Fill = new SolidColorBrush(Color.FromRgb(0x00, 0x00, 0xFF)) }));
                        else points.Add(new Point((double)records[i].GetValue("x"), (double)records[i].GetValue("y"), new Ellipse() { Width = 5, Height = 5, Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0x00, 0x00)) }));
                foreach (Point point in points) canvas.Children.Add(point.Ellipse);
                Console.WriteLine("Added {0} points.", points.Count);
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
        public Ellipse Ellipse { get; }

        public Point()
        {
            OriginalPosition = Vector2.Zero;
            Position = Vector2.Zero;
        }
        public Point(Vector2 position, Ellipse ellipse)
        {
            this.OriginalPosition = position.Copy;
            this.Position = position.Copy;
            this.Ellipse = ellipse;
            Canvas.SetLeft(Ellipse, Position.X);
            Canvas.SetTop(Ellipse, Position.Y);
        }
        public Point(double x, double y, Ellipse ellipse)
        {
            this.OriginalPosition = new Vector2(x, y);
            this.Position = new Vector2(x, y);
            this.Ellipse = ellipse;
            Canvas.SetLeft(Ellipse, Position.X);
            Canvas.SetTop(Ellipse, Position.Y);
        }
        
        public void Update(Vector2 offset, double multiplier)
        {
            Position = (multiplier * OriginalPosition) + MapViewPage.Camera.Position + offset;
            Canvas.SetLeft(Ellipse, Position.X);
            Canvas.SetTop(Ellipse, Position.Y);
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