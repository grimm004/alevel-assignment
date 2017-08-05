using System;
using System.Net;
using System.Net.Mail;
using System.Windows;
using System.Windows.Documents;
using System.Collections.Generic;
using System.Net.Mime;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Media;

namespace EmailClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        protected AttachmentsWindow attachmentsWindow;
        protected List<AttachmentItem> attachments;

        public MainWindow()
        {
            InitializeComponent();
            attachments = new List<AttachmentItem>();
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBoxResult.Yes;
            if (string.IsNullOrWhiteSpace(subjectEntryBox.Text)) result = System.Windows.MessageBox.Show("Are you sure you want to send without a subject?", "Email Confirmation", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                sendButton.IsEnabled = false;
                statusLabel.Content = "Sending Email";
                Email email =
                    new Email()
                    {
                        Server = serverEntryBox.Text,
                        Port = Convert.ToInt16(portEntryBox.Text),
                        Recipients = messageRecipientEntryBox.Text.Split(';'),
                        CC = !string.IsNullOrWhiteSpace(ccEntryBox.Text) ? ccEntryBox.Text.Split(';') : null,
                        BCC = !string.IsNullOrWhiteSpace(bccEntryBox.Text) ? bccEntryBox.Text.Split(';') : null,
                        SenderEmail = senderEmailEntryBox.Text,
                        SenderPassword = senderPasswordEntryBox.Password,
                        SenderName = senderNameEntryBox.Text,
                        Subject = subjectEntryBox.Text,
                        Body = new TextRange(emailBodyRichTextBox.Document.ContentStart, emailBodyRichTextBox.Document.ContentEnd).Text,
                        AttachmentItems = attachments.ToArray(),
                    };

                new Thread(new ParameterizedThreadStart(delegate { SendEmail(email); })) { IsBackground = true }.Start();
            }
        }

        private void SendEmail(Email email)
        {
            try
            {
                email.Send();
                statusLabel.Dispatcher.Invoke(delegate { statusLabel.Content = "Email Sent"; });
            }
            catch (SmtpException emailException)
            {
                System.Windows.MessageBox.Show(string.Format("An error occurred when sending the email:\n\r{0}", emailException.Message), "Email Error");
                statusLabel.Dispatcher.Invoke(delegate { statusLabel.Content = "Ready"; });
            }
            catch (FormatException)
            {
                System.Windows.MessageBox.Show(string.Format("Invalid recipient format, email could not be sent."), "Format Error", MessageBoxButton.OK, MessageBoxImage.Error);
                statusLabel.Dispatcher.Invoke(delegate { statusLabel.Content = "Ready"; });
            }
            sendButton.Dispatcher.Invoke(delegate { sendButton.IsEnabled = true; });
        }

        private void AttachmentsButton_Click(object sender, RoutedEventArgs e)
        {
            attachmentsWindow = new AttachmentsWindow(attachments);
            attachmentsWindow.ShowDialog();
            attachments = attachmentsWindow.attachments;
        }

        private void ColourSelectionButton_Click(object sender, RoutedEventArgs e)
        {
            ColorDialog colourPicker = new ColorDialog();
            if (colourPicker.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Brush brush = new SolidColorBrush(new System.Windows.Media.Color() { A = colourPicker.Color.A, R = colourPicker.Color.R, G = colourPicker.Color.G, B = colourPicker.Color.B });
                TextRange selectedText = new TextRange(emailBodyRichTextBox.Selection.Start, emailBodyRichTextBox.Selection.End);
                selectedText.ApplyPropertyValue(TextElement.ForegroundProperty, brush);
            }
        }
    }

    public class Email
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public string SenderEmail { get; set; }
        public string SenderPassword { get; set; }
        public string[] Recipients { get; set; }
        public string[] CC { get; set; }
        public string[] BCC { get; set; }
        public string SenderName { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public AttachmentItem[] AttachmentItems { get; set; }

        public void Send()
        {
            MailAddress fromAddress = new MailAddress(SenderEmail, SenderName);

            SmtpClient smtp = new SmtpClient
            {
                Host = Server,
                Port = Port,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, SenderPassword),
            };

            using (MailMessage message = new MailMessage() { From = fromAddress, Subject = Subject, Body = Body })
            {
                if (Recipients != null) foreach (string recipient in Recipients) message.To.Add(new MailAddress(recipient));
                if (CC != null) foreach (string cc in CC) message.CC.Add(new MailAddress(cc));
                if (BCC != null) foreach (string bcc in BCC) message.Bcc.Add(new MailAddress(bcc));

                foreach (AttachmentItem attachmentItem in AttachmentItems)
                    if (attachmentItem.FileDirectory != null && attachmentItem.FileName != null)
                    {
                        string fileName = Path.Combine(attachmentItem.FileDirectory, attachmentItem.FileName);
                        Attachment attachment = new Attachment(fileName, MediaTypeNames.Application.Octet);
                        ContentDisposition disposition = attachment.ContentDisposition;
                        disposition.CreationDate = File.GetCreationTime(fileName);
                        disposition.ModificationDate = File.GetLastWriteTime(fileName);
                        disposition.ReadDate = File.GetLastAccessTime(fileName);
                        disposition.FileName = Path.GetFileName(fileName);
                        disposition.Size = new FileInfo(fileName).Length;
                        disposition.DispositionType = DispositionTypeNames.Attachment;
                        message.Attachments.Add(attachment);
                    }

                smtp.Send(message);
            }
        }
    }
}
