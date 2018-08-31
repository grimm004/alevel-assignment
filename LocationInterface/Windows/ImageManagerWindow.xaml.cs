using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using LocationInterface.Utils;
using Microsoft.Win32;

namespace LocationInterface.Windows
{
    /// <summary>
    /// Interaction logic for IndexManagerWindow.xaml
    /// </summary>
    public partial class IndexManagerWindow : Window
    {
        private bool Changed { get; set; }

        public IndexManagerWindow()
        {
            InitializeComponent();
            UpdateTable();
            Changed = false;
        }

        private void AddImageClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog selectFileDialog = new OpenFileDialog();
            if (selectFileDialog.ShowDialog() == true)
            {
                if (ImportImageFile(selectFileDialog.FileName, out string newFileName))
                {
                    ImageFileWindow imageFileWindow = new ImageFileWindow(newFileName);
                    imageFileWindow.ShowDialog();
                    Changed = true;
                    UpdateTable();
                }
                else MessageBox.Show("Could not import image file (unsupported format).", "Import Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ImportImageFile(string sourceFileName, out string newFileName)
        {
            newFileName = "";
            string imageName = "";

            if (!File.Exists($"{ SettingsManager.Active.ImageFolder }\\{ Path.GetFileNameWithoutExtension(sourceFileName) }.bmp")) imageName = Path.GetFileNameWithoutExtension(sourceFileName);
            else if (MessageBox.Show("An image with this name already exists. Import with a new name?", "Image Already Exists", MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes)
            {
                FileRenameWindow fileRenameWindow = new FileRenameWindow(Path.GetFileNameWithoutExtension(sourceFileName));
                if (fileRenameWindow.ShowDialog() == true) imageName = fileRenameWindow.ImageName;
                else return false;
            }
            else return false;

            newFileName = $"{ imageName }.bmp";
            if (!IsImage(Path.GetExtension(sourceFileName))) return false;
            try
            {
                System.Drawing.Image.FromFile(sourceFileName).Save($"{ SettingsManager.Active.ImageFolder }\\{ newFileName }", ImageFormat.Bmp);
                return true;
            }
            catch (OutOfMemoryException)
            {
                return false;
            }
        }

        private bool IsImage(string extension)
        {
            foreach (string acceptableExtension in new string[] { ".jpg", ".jpeg", ".png", ".bmp" })
                if (extension.ToLower() == acceptableExtension) return true;
            return false;
        }

        private void CloseClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            DialogResult = Changed;
            base.OnClosing(e);
        }

        private void DoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (ImageIndexDataGrid.SelectedItems.Count == 1)
            {
                if (new ImageFileWindow(ImageIndexDataGrid.SelectedItem as ImageFile).ShowDialog() == true) Changed = true;
                UpdateTable();
            }
        }
        
        public void UpdateTable()
        {
            ImageIndexDataGrid.Items.Clear();
            foreach (ImageFile imageFile in App.ImageIndex.ImageFiles)
                if (imageFile != null) ImageIndexDataGrid.Items.Add(imageFile);
        }

        private void DataGridKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Delete && ImageIndexDataGrid.SelectedItems.Count > 0 && MessageBox.Show("Are you sure you want to delete the selected images?", "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                DeleteSelected();
        }

        private void DeleteSelected()
        {
            foreach (ImageFile imageFile in ImageIndexDataGrid.SelectedItems)
                App.ImageIndex.DeleteImage(imageFile);
            Changed = true;
            UpdateTable();
        }
    }
}
