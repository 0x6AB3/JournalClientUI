using JournalNetCode.ClientSide;
using JournalNetCode.Common.Communication.Containers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
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

namespace NotesApp
{
    /// <summary>
    /// Interaction logic for NoteDisplay.xaml
    /// </summary>
    public partial class NoteDisplay : UserControl
    {
        public event EventHandler UpdateButtonClicked;

        private byte[] encryptionKey;
        private Client client;
        private Note note;
        private string path;

        public NoteDisplay(Note note, string path, byte[] encryptionKey, Client client)
        {
            InitializeComponent();
          
            lblTitle.Text = note.Title;

            this.note = note;
            this.path = path;
            this.encryptionKey = encryptionKey;
            this.client = client;
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            var edit = new wdwEditNote(encryptionKey, client, note);
            edit.ShowDialog();
            UpdateButtonClicked?.Invoke(this, EventArgs.Empty);
        }

        private void btnCloud_Click(object sender, RoutedEventArgs e)
        {
            note.Upload = !note.Upload;
            UpdateCloudIcon();
        }

        private void btnDeleteNote_Click(object sender, RoutedEventArgs e)
        {
            var confirm = new wdwConfirmYesCancel("This note will be permanently deleted from your device");
            confirm.ShowDialog();
            if (confirm.confirm)
            {
                File.Delete(path);
            }
            UpdateButtonClicked?.Invoke(this, EventArgs.Empty);
        }

        private void UpdateCloudIcon()
        {
            if (note.Upload)
            {
                btnCloud.Foreground = new SolidColorBrush(Colors.White);
                client.PostNote(note);
            }
            else
            {
                btnCloud.Foreground = new SolidColorBrush(Colors.Black);
            }
        }
    }
}
