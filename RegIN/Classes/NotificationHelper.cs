using System.Windows.Controls;
using System.Windows.Media;

namespace RegIN.Classes
{
    public static class NotificationHelper
    {
        public static void Show(Label label, string message, SolidColorBrush color)
        {
            label.Content = message;
            label.Foreground = color;
        }
    }
}