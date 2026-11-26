using RegIN.Classes;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace RegIN
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow mainWindow;
        public User UserLogin = new User();

        public MainWindow()
        {
            InitializeComponent();

            try
            {
                Classes.WorkingDB.InitializeDatabase();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ошибка подключения к базе данных:\n" + ex.Message);

                Application.Current.Shutdown();
                return;
            }

            mainWindow = this;
            OpenPage(new Pages.Login());
        }

        public void OpenPage(Page page)
        {
            DoubleAnimation StartAnimation = new DoubleAnimation();

            StartAnimation.From = 0;
            StartAnimation.To = 0;
            StartAnimation.Duration = TimeSpan.FromSeconds(0.6);

            StartAnimation.Completed += delegate
            {
                frame.Navigate(page);
                DoubleAnimation EndAnimation = new DoubleAnimation();
                EndAnimation.From = 0;
                EndAnimation.To = 1;
                EndAnimation.Duration = TimeSpan.FromSeconds(1.2);
                frame.BeginAnimation(Frame.OpacityProperty, EndAnimation);
            };
            frame.BeginAnimation(Frame.OpacityProperty, StartAnimation);
        }
    }
}