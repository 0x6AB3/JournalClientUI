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
using JournalNetCode.ClientSide;
using JournalNetCode.Common.Communication.Containers;
using JournalNetCode.Common.Communication.Types;
using JournalNetCode.Common.Security;
using JournalNetCode.Common.Utility;

namespace NotesApp
{
    /// <summary>
    /// Interaction logic for wdwCreateAccount.xaml
    /// </summary>
    public partial class wdwCreateAccount : Window
    {
        private readonly string defaultEmail;

        public wdwCreateAccount()
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

        private void txtPasswordConfirm_GotFocus(object sender, RoutedEventArgs e)
        {
            lblPasswordConfirm.Visibility = Visibility.Collapsed;
        }

        private void txtPasswordConfirm_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtPasswordConfirm.Password.Trim()))
            {
                lblPasswordConfirm.Visibility = Visibility.Visible;
            }
        }

        private async void btnCreateAccount_Click(object sender, RoutedEventArgs e)
        {
            if (txtPassword.Password != txtPasswordConfirm.Password)
            {
                var wdwError = new wdwErrorOK("Passwords do not match.");
                wdwError.Show();
                return;
            }
            if (txtPassword.Password.Length < 8)
            {
                var wdwError = new wdwErrorOK("Password must be at least 8 characters.");
                wdwError.Show();
                return;
            }
            if (!Validate.EmailAddress(txtEmail.Text))
            {
                var wdwError = new wdwErrorOK("Invalid email address.");
                wdwError.Show();
                return;
            }
            var client = new Client("127.0.0.1", 9600);
            try
            {
                (var success, var message) = await client.SignUp(txtEmail.Text, txtPassword.Password);
                if (success)
                {
                    var hashingAlgorithm = new PasswordHashing();
                    var encryptionKey = hashingAlgorithm.GetEncryptionKey(txtPassword.Password, txtEmail.Text);
                    var wdwSuc = new wdwSuccess(message != null ? message : "Successful Sign-up!");
                    wdwSuc.ShowDialog();
                    var wdwHome = new wdwHomePage(encryptionKey, client);
                    wdwHome.Show();
                    this.Close();
                }
                else
                {
                    var wdwError = new wdwErrorOK(message != null ? message : "Unable to signup...");
                    wdwError.Show();
                }
            }
            catch (Exception ex)
            {
                var wdwError = new wdwErrorOK($"Unable to signup...\n{ex.Message}");
                wdwError.Show();
            }
        }
    }
}
