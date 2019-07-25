using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using OpenPop.Mime;
using OpenPop.Pop3;

namespace TimeTableManager.MailService
{
    public class Mailer
    {
        private const int LastMessageCount = 10;
        private readonly Pop3Client _client = new Pop3Client();

        private string _sender;
        private string _lastMessageDate;


        private List<Message> _messages;

        private int GetMessagesCount()
        {
            // Подключение к серверу
            _client.Connect(Config.Pop3HostName, Config.Pop3Port, Config.IsSSL);

            Console.WriteLine("Pop3 connect");

            // Аутентификация (проверка логина и пароля)
            _client.Authenticate(Config.Email, Config.Password, AuthenticationMethod.UsernameAndPassword);

            Console.WriteLine(Config.Email + " login");

            if (_client.Connected)
            {

                // Получение количества сообщений в почтовом ящике
                var messageCount = _client.GetMessageCount();

                Console.WriteLine("Message count: " + messageCount.ToString());

                // Выделяем память под список сообщений. Мы хотим получить все сообщения
                _messages = new List<Message>(messageCount);

                return messageCount;
            }
            else
            {
                return 0;
            }
        }

        private IEnumerable<Message> GetMessages()
        {
            var messageCount = GetMessagesCount();

            if(messageCount != 0)
            {
                for (var i = messageCount; i > messageCount - LastMessageCount - 1; i--)
                {
                    try
                    {
                        _messages.Add(_client.GetMessage(i));

                        if(messageCount >= 10)
                            Console.WriteLine(((i - messageCount)  * -100 / LastMessageCount) + "%");
                        else
                            Console.WriteLine(((i - messageCount) * -100 / messageCount) + "%");


                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
                _client.Disconnect();
            }
           
            //Console.Clear();

            return _messages;
        }

        public string GetAttachment()
        {
            var subject = "Расписание на ";

            var filterMessages = FilterMessages(subject);

            var path = "";
            if (filterMessages.Count > 0)
            {
                Message msg = filterMessages[0];
                var date = msg.Headers.Date;

                if (date == _lastMessageDate)
                    return "";
                else
                    _lastMessageDate = date;
                //_sender = msg.ToMailMessage().From.ToString().Split("<")[1].Replace(">", "");
                var att = msg.FindAllAttachments();
                foreach (var ado in att)
                {
                    //сохраняем все найденные в письмах вложения
                    path = StorageManager.CreateFilePath(ado.FileName, "Attachments");
                    ado.Save(new System.IO.FileInfo(path));
                    Console.WriteLine("File created: " + ado.FileName + "\n");
                }
            }

            return path;
        }

        public List<Message> FilterMessages(string _subject)
        {
            var messages = GetMessages()
                .Where(a => a.FindAllAttachments() != null)
                .Where(x => x.Headers.Subject != null)
                .Where(y => y.Headers.Subject.Contains(_subject))
                //.Where(z => z.Headers.Date.Split(' ')[1] == DateTime.Now.Day.ToString())
                .ToList();

            Console.WriteLine("Find messages:\n");
            foreach (var message in messages)
            {
                string subject = message.Headers.Subject; //заголовок
                string date = message.Headers.Date; //Дата/Время
                string from = message.Headers.From.ToString(); //от кого

                Console.WriteLine(subject);
                Console.WriteLine(date);
                Console.WriteLine(from + "\n");
            }

            return messages;
        }

        //public void SendMessage(string message)
        //{
        //    SmtpClient c = new SmtpClient(Config.SmtpHostName, Config.SmtpPort);
        //    MailAddress add = new MailAddress(_sender);
        //    MailMessage msg = new MailMessage();
        //    msg.To.Add(add);
        //    msg.From = new MailAddress(Config.Email);
        //    msg.IsBodyHtml = false;
        //    msg.Subject = "Расписание обновленно";
        //    msg.Body = message;
        //    c.Credentials = new System.Net.NetworkCredential(Config.Email, Config.Password);
        //    c.EnableSsl = Config.IsSSL;
        //    c.DeliveryMethod = SmtpDeliveryMethod.Network;
        //    c.UseDefaultCredentials = false;
        //    c.Send(msg);

        //    msg.IsBodyHtml = true;
        //    c.Send(msg);
        //    Console.WriteLine("Message send\n");
        //}
    }
}
