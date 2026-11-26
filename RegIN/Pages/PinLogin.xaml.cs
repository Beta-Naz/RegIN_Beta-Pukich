using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using RegIN.Classes;

namespace RegIN.Pages
{
    public partial class PinLogin : Page
    {
        public PinLogin(string login = "")
        {
            InitializeComponent();
            TbLogin.Text = login;
        }

        private void LoginByPin(
            object sender, RoutedEventArgs e)
        {
            CheckLogin();
        }

        private void PinKeyDown(
            object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                CheckLogin();
        }

        private void CheckLogin()
        {
            TNotification.Text = "";

            if (string.IsNullOrWhiteSpace(TbLogin.Text))
            {
                TNotification.Text = "Enter login";
                return;
            }

            try
            {
                if (WorkingDB.CheckPin(
                    TbLogin.Text.Trim(),
                    TbPin.Password,
                    out string message))
                {
                    TbPin.Clear();

                    MessageBox.Show(
                        "Авторизация пользователя по PIN-коду успешно подтверждена.",
                        "RegIN",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    MainWindow.mainWindow.OpenPage(new Login());
                }
                else
                {
                    TNotification.Text = message;
                    TbPin.Clear();
                }
            }
            catch (Exception ex)
            {
                TNotification.Text = "Authorization error";
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        private void OpenLogin(
            object sender, RoutedEventArgs e)
        {
            MainWindow.mainWindow.OpenPage(new Login());
        }
    }
}