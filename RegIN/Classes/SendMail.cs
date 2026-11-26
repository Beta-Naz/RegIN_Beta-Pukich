using System.Net;
using System.Net.Mail;

namespace RegIN.Classes
{
    public class SendMail
    {
        public static void SendMessage(string Message, string To)
        {
            var smtpClient = new SmtpClient("smtp.yandex.ru")
            {
                Port = 587,
                Credentials = new NetworkCredential("eldargafiullin@yandex.ru", "aegxgczyutngtikd"),
                EnableSsl = true
            };
            smtpClient.Send("eldargafiullin@yandex.ru", To, "Проект RegIn", Message);
        }
    }
}
