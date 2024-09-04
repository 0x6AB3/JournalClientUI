using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Shapes;
using JournalNetCode.ClientSide;
using JournalNetCode.Common.Communication.Containers;

namespace NotesApp
{
    /// <summary>
    /// Interaction logic for wdwHomePage.xaml
    /// </summary>
    public partial class wdwEditNote : Window
    {
        private bool upload = false;
        private bool locked = false;
        private readonly byte[] encryptionKey;
        private Client client;
        private Note? note;
        private string defaultTitle = "Title...";
        private string defaultBody = "Body";

        public wdwEditNote(byte[] encryptionKey, Client client, Note? existingNote = null)
        {
            InitializeComponent();
            this.encryptionKey = encryptionKey;
            this.client = client;
            if (existingNote == null)
            {
                lblLastModified.Content = $"Last modified: {DateTime.Now}";
            }
            else
            {
                note = existingNote;
                lblLastModified.Content = $"Last modified: {existingNote.LastModified}";
                txtNoteName.Text = existingNote.Title;
                var content = existingNote.GetText(encryptionKey);
                var textRange = new TextRange(txtNoteContent.Document.ContentStart, txtNoteContent.Document.ContentEnd)
                {
                    Text = content
                };
                if (note.Upload)
                {
                    btnCloud_Click(btnCloud, new RoutedEventArgs());
                }
            }
        }

        private void txtNoteName_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtNoteName.Text))
            {
                txtNoteName.Text = "Note Title";
            }
        }

        private void txtNoteName_GotFocus(object sender, RoutedEventArgs e)
        {
            txtNoteName.Text = "";
        }

        private void btnLock_Click(object sender, RoutedEventArgs e)
        {
            locked = !locked;
            if (locked)
            {
                btnLock.Content = "🔒";
            }
            else
            {
                btnLock.Content = "🔓";
            }
        }

        private void btnCloud_Click(object sender, RoutedEventArgs e)
        {
            upload = !upload;
            if (upload) 
            {
                btnCloud.Foreground = new SolidColorBrush(Colors.White);
            }
            else
            {
                btnCloud.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        private async void btnCreateNote_Click(object sender, RoutedEventArgs e)
        {
            var newPath = System.IO.Path.Join(Directory.GetCurrentDirectory(), $"/Notes/{txtNoteName.Text}.json");
            var UpdateExistingNoteLocation = note != null && note.Title != txtNoteName.Text;

            // checking if note with this title already exists
            if (File.Exists(newPath) && ((note == null) || UpdateExistingNoteLocation))
            {
                var confirm = new wdwConfirmYesCancel("A note with this title already exists, overwrite?");
                confirm.ShowDialog();
                if (!confirm.confirm)
                {
                    return; // early return to avoid overwrite (cancel creation)
                }
            }

            // deleting old note and creating a new one with the updated title
            if (UpdateExistingNoteLocation)
            {
                var oldNotePath = System.IO.Path.Join(Directory.GetCurrentDirectory(), $"/Notes/{note.Title}.json");
                File.Delete(oldNotePath);
            }


            var noteContent = new TextRange(txtNoteContent.Document.ContentStart, txtNoteContent.Document.ContentEnd);
            note = new Note(txtNoteName.Text);
            note.SetText(noteContent.Text, encryptionKey);
            note.Upload = upload;
            note.ToFile();
            var wdwSuc = new wdwSuccess("Note has been saved to your device");
            wdwSuc.ShowDialog();
            if (upload)
            {
                (var success, var message) = await client.PostNote(note);
                if (success)
                {
                    wdwSuc = new wdwSuccess(message != null ? message : "Note successfully uploaded to the server");
                    wdwSuc.ShowDialog();
                }
                else
                {
                    var wdwError = new wdwErrorOK(message != null ? message : "Unable to upload note");
                    wdwError.ShowDialog();
                }
            }
            this.Close();
        }

        private void btnDeleteNote_Click(object sender, RoutedEventArgs e)
        {
            var confirm = new wdwConfirmYesCancel("Your note will be permanently delete from your device");
            confirm.ShowDialog();
            if (confirm.confirm)
            {
                if (note != null)
                {
                    note.Delete();
                }
                this.Close();
            }
        }
    }
}
