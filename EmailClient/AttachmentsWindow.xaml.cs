using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.IO;

namespace EmailClient
{
    /// <summary>
    /// Interaction logic for AttachmentsWindow.xaml
    /// </summary>
    public partial class AttachmentsWindow : Window
    {
        public List<AttachmentItem> attachments;

        public AttachmentsWindow(List<AttachmentItem> currentAttachments)
        {
            InitializeComponent();
            attachments = new List<AttachmentItem>();
            foreach (AttachmentItem attachment in currentAttachments) AddAttachment(attachment);
            UpdateSize();
            this.KeyDown += new KeyEventHandler(Key_Press);
        }

        private void Key_Press(object sender, KeyEventArgs args)
        {
            if (args.IsDown)
                switch (args.Key)
                {
                    case Key.Delete:
                        DeleteSelected();
                        break;
                    default:
                        break;
                }
        }

        private void DeleteSelected()
        {
            List<AttachmentItem> selectedItems = attachmentList.SelectedItems.Cast<AttachmentItem>().ToList();
            for (int i = 0; i < selectedItems.Count; i++) { attachmentList.Items.Remove(selectedItems[i]); attachments.Remove(selectedItems[i]); }
            UpdateSize();
        }

        private void AddAttachment(AttachmentItem attachment)
        {
            if (attachment.FileSize < 25 * Math.Pow(1024, 2))
            {
                attachmentList.Items.Add(attachment);
                attachments.Add(attachment);
                if (!UpdateSize())
                {
                    attachmentList.Items.Remove(attachment);
                    attachments.Remove(attachment);
                    MessageBox.Show("Maximum email size is 25 MB.", "Attachment Error");
                }
            }
            else MessageBox.Show("Maximum file size is 25 MB.", "Attachment Error");
        }

        private bool UpdateSize()
        {
            long totalLength = 0;
            foreach (AttachmentItem attachment in attachments) totalLength += attachment.FileSize;
            if (totalLength < 25 * Math.Pow(1024, 2))
            {
                if (totalLength > Math.Pow(1024, 2)) fileSizeLabel.Content = string.Format("Total Size: {0:0.00} MB", totalLength / Math.Pow(1024, 2));
                else if (totalLength > Math.Pow(1024, 1)) fileSizeLabel.Content = string.Format("Total Size: {0:0.00} KB", totalLength / Math.Pow(1024, 1));
                else if (totalLength > Math.Pow(1024, 0)) fileSizeLabel.Content = string.Format("Total Size: {0} bytes", totalLength);
                else fileSizeLabel.Content = "Total Size: 0 bytes";
                return true;
            }
            else return false;
        }

        private void AddAttachmentsButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
                AddAttachment(new AttachmentItem { FileName = Path.GetFileName(openFileDialog.FileName), FileSize = new FileInfo(openFileDialog.FileName).Length, FileDirectory = Path.GetDirectoryName(openFileDialog.FileName) });
        }

        private void ClearAllButton_Click(object sender, RoutedEventArgs e)
        {
            attachmentList.Items.Clear();
            attachments.Clear();
            UpdateSize();
        }
    }

    public class AttachmentItem
    {
        public string FileDirectory { get; set; }
        public string FileName { get; set; }
        public long FileSize { get; set; }
    }
}
