using LocationInterface.Utils;
using Microsoft.Win32;
using Microsoft.Xna.Framework;
using System;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;

namespace LocationInterface.Windows
{
    /// <summary>
    /// Interaction logic for ImageFileWindow.xaml
    /// </summary>
    public partial class ImageFileWindow : Window
    {
        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
        }

        private bool Changed { get; set; }
        
        private ImageFile ActiveImageFile { get; set; }
        private ImageFile EnteredImageFile
        {
            get
            {
                return new ImageFile
                {
                    FileName = FileNameInput.Text,
                    LocationIdentifier = LocationIdentifierInput.Text,
                    DataReference = DataFloorReferenceInput.Text,
                    AreaFileName = AreaFileNameEntry.Text,
                    FlipHorizontal = (bool)FlipHorizontalEntry.IsChecked,
                    FlipVertical = (bool)FlipVerticalEntry.IsChecked,
                    Scale = new Vector2(Convert.ToSingle(ScaleXEntry.Text), Convert.ToSingle(ScaleYEntry.Text)),
                    Offset = new Vector2(Convert.ToSingle(OffsetXEntry.Text), Convert.ToSingle(OffsetYEntry.Text)),
                };
            }
            set
            {
                FileNameInput.Text = value.FileName;
                LocationIdentifierInput.Text = value.LocationIdentifier;
                DataFloorReferenceInput.Text = value.DataReference;
                AreaFileNameEntry.Text = value.AreaFileName;
                FlipHorizontalEntry.IsChecked = value.FlipHorizontal;
                FlipVerticalEntry.IsChecked = value.FlipVertical;
                ScaleXEntry.Text = value.Scale.X.ToString();
                ScaleYEntry.Text = value.Scale.Y.ToString();
                OffsetXEntry.Text = value.Offset.X.ToString();
                OffsetYEntry.Text = value.Offset.Y.ToString();
            }
        }

        public ImageFileWindow(ImageFile imageFile)
        {
            InitializeComponent();
            EnteredImageFile = ActiveImageFile = imageFile;
            Changed = false;
        }

        public ImageFileWindow(string fileName)
        {
            InitializeComponent();
            ActiveImageFile = new ImageFile(fileName, Vector2.Zero, Vector2.Zero, "");
            EnteredImageFile = ActiveImageFile;
            Changed = true;
            Apply();
        }

        private void CloseClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ApplyClick(object sender, RoutedEventArgs e)
        {
            Apply();
        }

        private void Apply()
        {
            Changed = true;
            ActiveImageFile = EnteredImageFile;
            if (!App.ImageIndex.ImageFileExists(EnteredImageFile.FileName)) App.ImageIndex.AddImage(EnteredImageFile);
            else App.ImageIndex.UpdateImageFile(EnteredImageFile);
            App.ImageIndex.SaveIndex();
            FieldChange();
        }

        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            FieldChange();
        }

        private void FieldChange()
        {
            if (IsInitialized)
                try
                {
                    // If the hash code of the entered settings is equal to the hash code of the active settings disable the apply button
                    ApplyButton.IsEnabled = EnteredImageFile.GetHashCode() != ActiveImageFile.GetHashCode();
                }
                catch (FormatException)
                {
                    ApplyButton.IsEnabled = false;
                }
        }

        private void NumberValidation(object sender, TextCompositionEventArgs e)
        {
            // Mark the validation as having been handled if the text is numerical
            e.Handled = !IsNumericalText(e.Text);
            // If the length of the text is zero, set the value of the text to the number zero
            if (e.Text.Length == 0) { e.Handled = true; ((TextBox)sender).Text = "0"; }
        }

        private void PasteNumberValidation(object sender, DataObjectPastingEventArgs e)
        {
            // If the datatype of the current sender is a string
            if (e.DataObject.GetDataPresent(typeof(String)))
                // If the entered text is not numerical cancel the paste
                if (!IsNumericalText((String)e.DataObject.GetData(typeof(String)))) e.CancelCommand();
                else e.CancelCommand();
        }

        private bool IsNumericalText(string text)
        {
            // Return true if the text is not a match to a regular expression that check if text is not numerical
            return !new Regex("[^0-9.-]+").IsMatch(text);
        }

        private void CheckboxChanged(object sender, RoutedEventArgs e)
        {
            FieldChange();
        }

        private void BrowseClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true) AreaFileNameEntry.Text = openFileDialog.FileName;
        }
    }
}
