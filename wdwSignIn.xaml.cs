using System;

using System.Windows;

using JournalNetCode.ClientSide;
using JournalNetCode.Common.Communication.Containers;
using JournalNetCode.Common.Utility;
using JournalNetCode.Common.Security;

namespace NotesApp
{
    /// <summary>
    /// Interaction logic for wdwSignIn.xaml
    /// </summary>
    public partial class wdwSignIn : Window
    {
        private readonly string defaultEmail;

        public wdwSignIn()
        {
            InitializeComponent();
            defaultEmail = txtEmail.Text;
        }

        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            var wdw = new wdwInitial();
            wdw.Show();
            this.Close();
        }

        private void txtEmail_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtEmail.Text == defaultEmail)
            {
                txtEmail.Text = "";
            }
        }

        private void txtEmail_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtEmail.Text.Trim()))
            {
                txtEmail.Text = defaultEmail;
            }
        }

        private void txtPassword_GotFocus(object sender, RoutedEventArgs e)
        {
            lblPassword.Visibility = Visibility.Collapsed;
        }

        private void txtPassword_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtPassword.Password.Trim()))
            {
                lblPassword.Visibility = Visibility.Visible;
            }
        }

        private async void btnSignIn_Click(object sender, RoutedEventArgs e)
        {
            if (!Validate.EmailAddress(txtEmail.Text))
            {
                var wdwError = new wdwErrorOK("Invalid email address.");
                wdwError.Show();
                return;
            }
            var client = new Client("127.0.0.1", 9600);
            try
            {
                (bool success, string? message) = await client.LogIn(txtEmail.Text, txtPassword.Password);
                if (success)
                {
                    var hashingAlgorithm = new PasswordHashing();
                    var encryptionKey = hashingAlgorithm.GetEncryptionKey(txtPassword.Password, txtEmail.Text);
                    var wdwSuc = new wdwSuccess(message != null ? message : "Successful Login!");
                    wdwSuc.ShowDialog();
                    var wdwHome = new wdwHomePage(encryptionKey, client);
                    wdwHome.Show();
                    this.Close();
                }
                else
                {
                    var wdwError = new wdwErrorOK(message != null ? message : "Unable to Login...");
                    wdwError.Show();
                }
            }
            catch (Exception ex)
            {
                var wdwError = new wdwErrorOK($"Unable to Login...\n{ex.Message}");
                wdwError.Show();
            }
        }
    }
}
