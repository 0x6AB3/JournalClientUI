using JournalNetCode.ClientSide;
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
using System.Windows.Shapes;

namespace NotesApp
{
    /// <summary>
    /// Interaction logic for wdwServerNoteOptions.xaml
    /// </summary>
    public partial class wdwServerNoteOptions : Window
    {
        private Client client;
        private readonly string title;
        private readonly byte[] encryptionKey;
        public wdwServerNoteOptions(string noteTitle, byte[] encryptionKey, Client client)
        {
            InitializeComponent();
            lblNoteTitle.Content = noteTitle;
            this.client = client;
            this.title = noteTitle;
        }

        private async void btnDownload_Click(object sender, RoutedEventArgs e)
        {
            (var note, var message) = await client.GetNote(title);
            if (note != null)
            {
                var suc = new wdwSuccess("This note has been downloaded to your device, view it in your local notes");
                suc.ShowDialog();
            }
            else
            {
                var error = new wdwErrorOK(message != null ? message : "Unable to download this note from the server");
                error.ShowDialog();
            }
            this.Close();
        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var success = await client.DeleteNote(title);
            if (success)
            {
                var suc = new wdwSuccess("Note has been successfully deleted");
                suc.ShowDialog();
            }
            else
            {
                var error = new wdwErrorOK("Unable to delete this note from the server");
                error.ShowDialog();
            }
            this.Close();
        }
    }
}
