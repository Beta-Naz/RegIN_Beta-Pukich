using System;
using System.Windows;
using System.Windows.Controls;
using RegIN.Classes;

namespace RegIN.Pages
{
    public partial class CreatePin : Page
    {
        public CreatePin()
        {
            InitializeComponent();
        }

        private void SavePin(object sender, RoutedEventArgs e)
        {
            if (!PinHelper.IsValid(TbPin.Password))
            {
                TNotification.Text = "PIN must contain 4 digits";
                return;
            }

            if (TbPin.Password != TbConfirmPin.Password)
            {
                TNotification.Text = "PIN codes do not match";
                return;
            }

            try
            {
                WorkingDB.SavePin(
                    MainWindow.mainWindow.UserLogin.Login,
                    TbPin.Password);

                MessageBox.Show("PIN-код успешно создан.");

                MainWindow.mainWindow.OpenPage(new Login());
            }
            catch (Exception ex)
            {
                TNotification.Text = ex.Message;
            }
        }

        private void SkipPin(object sender, RoutedEventArgs e)
        {
            MainWindow.mainWindow.OpenPage(new Login());
        }
    }
}