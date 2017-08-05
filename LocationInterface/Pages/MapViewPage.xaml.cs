using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
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

        public MapViewPage(Action ShowHomePage)
        {
            this.ShowHomePage = ShowHomePage;

            InitializeComponent();
            ImageSource image = new BitmapImage(new Uri("Images\\Deck4.bmp", UriKind.Relative));

            for (int i = 0; i < 11; i++) ((ComboBoxItem)deckSelectionComboBox.Items[i]).IsEnabled = File.Exists(string.Format("Images\\Deck{0}.bmp", i + 1));

            currentImage = new Image
            {
                Width = image.Width,
                Height = image.Height,
                Name = "DeckImage",
                Source = image,
            };

            canvas.Children.Add(currentImage);

            Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        Dispatcher.Invoke(delegate
                        {
                            if (Keyboard.IsKeyDown(Key.Left)) MovePosition(5, 0);
                            if (Keyboard.IsKeyDown(Key.Right)) MovePosition(-5, 0);
                            if (Keyboard.IsKeyDown(Key.Up)) MovePosition(0, 5);
                            if (Keyboard.IsKeyDown(Key.Down)) MovePosition(0, -5);
                        });
                        Thread.Sleep((int)(1000 / 60));
                    }
                    catch (TaskCanceledException) { }
                }
            });
        }

        protected double x = 0;
        protected double y = 0;
        protected void MovePosition(double x, double y)
        {
            SetPosition(this.x + x, this.y + y);
        }

        protected void SetPosition(double x, double y)
        {
            this.x = x;
            this.y = y;
            Canvas.SetLeft(currentImage, x);
            Canvas.SetTop(currentImage, y);
        }

        private void DeckSelectionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
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
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("Deck file could not be found.", "File Not Found");
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            ShowHomePage();
        }
    }
}
