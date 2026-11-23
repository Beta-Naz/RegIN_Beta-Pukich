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
                Credentials = new NetworkCredential("sabitovnazir123@yandex.ru", "Asdfg123"),
                EnableSsl = true
            };
            smtpClient.Send("h4tache@yandex.ru", To, "Проект RegIn", Message);
        }
    }
}
