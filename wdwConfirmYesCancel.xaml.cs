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
    /// Interaction logic for wdwConfirmYesCancel.xaml
    /// </summary>
    public partial class wdwConfirmYesCancel : Window
    {
        public bool confirm = false;
        public wdwConfirmYesCancel(string message)
        {
            InitializeComponent();
            txtInformation.Text = message;
        }

        private void btnYes_Click(object sender, RoutedEventArgs e)
        {
            confirm = true;
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            confirm = false;
            this.Close();
        }
    }
}
