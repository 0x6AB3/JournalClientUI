using JournalNetCode.ClientSide;
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
using JournalNetCode.Common.Communication.Containers;
using System.Text.Json;

namespace NotesApp
{
    /// <summary>
    /// Interaction logic for wdwHomePage.xaml
    /// </summary>
    public partial class wdwHomePage : Window
    {
        private readonly byte[] encryptionKey;
        private Client client;
        private List<Note> notes = new List<Note>();
        private string noteDirectory;

        public wdwHomePage(byte[] encryptionKey, Client client)
        {
            InitializeComponent();

            this.encryptionKey = encryptionKey;
            this.client = client;

            noteDirectory = Path.Join(Directory.GetCurrentDirectory(), "Notes/");
            Directory.CreateDirectory(noteDirectory);

            UpdateLocal();
        }

        private async void UpdateCloud()
        {
            var titles = await client.GetNoteTitles();
            if (titles == null)
            {
                return;
            }
            foreach (var title in titles)
            {

            }
        }

        private void UpdateLocal()
        {
            cnvNotes.Children.Clear();
            var notePaths = Directory.GetFiles(noteDirectory);

            for (int i = 0; i < notePaths.Length; i++)
            {
                var note = JsonSerializer.Deserialize<Note>(File.ReadAllText(notePaths[i]));
                var noteDisplay = new NoteDisplay(note, notePaths[i], encryptionKey, client);
                noteDisplay.Width = 300;
                noteDisplay.Height = 150;
                Canvas.SetLeft(noteDisplay, (i%2) * 300);
                Canvas.SetTop(noteDisplay, (int)(i/2) * 150);
                noteDisplay.UpdateButtonClicked += NoteDisplay_UpdateButtonClicked;
                if (note.Upload)
                {
                    noteDisplay.btnCloud.Foreground = new SolidColorBrush(Colors.White);
                }
                cnvNotes.Children.Add(noteDisplay);
            }
        }

        private void NoteDisplay_UpdateButtonClicked(object sender, EventArgs e)
        {
            UpdateLocal();
        }

        private void btnCreateNote_Click(object sender, RoutedEventArgs e)
        {
            var wdwEdit = new wdwEditNote(encryptionKey, client);
            wdwEdit.ShowDialog();
            UpdateLocal();
        }
    }
}
