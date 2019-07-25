using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using TimeTableManager.ExcelService;
using TimeTableManager.HTMLService;
using TimeTableManager.MailService;
using TimeTableManager.NetworkService;

namespace TimeTableManager
{
    class Program
    {
        static MailService.Mailer mailer = new MailService.Mailer();
        private static Timer timer = new Timer(Callback, null, 0, 0);
        private static bool isStart = true;
        private static int seconds = 10;

        //private static List<string> messages;

        static void Main(string[] args)
        {
            Start();
            Console.Read();
        }

        private static void Start()
        {
            Console.WriteLine();


            var attachmentPath = mailer.GetAttachment();
            if (attachmentPath != "")
            {
                var parser = new ExcelParser();
                var tables = HtmlConverter.CreateTable(parser.Parse(attachmentPath));

                //messages = new List<string>();

                for (int i = 0; i < tables.Count; i++)
                {
                    Regex regex = new Regex(@"<title>.*<\/title>");
                    MatchCollection matches = regex.Matches(tables[i]);
                    var fileName = ClearName(matches[0].Value);
                    var filePath = StorageManager.CreateFile(fileName,"Tables",tables[i],"html");
                    var savePath = SftpManager.FileUploadSFTP(fileName + ".html", filePath);

                    //messages.Add("http://wwww." + savePath.Split(".")[1]);
                }

                //var msg = "";

                //foreach (var message in messages)
                //{
                //    msg += message + "\n";
                //}

                //mailer.SendMessage(msg);
            }
            else
            {
                Console.WriteLine("No new file\n");
            }
            timer.Change(0, 1000);
        }

        private static string ClearName(string fileName)
        {
            fileName = fileName.Replace("<title>", "");
            fileName = fileName.Replace("</title>", "");
            fileName = fileName.Replace("(9 кл)", "");
            fileName = fileName.Replace("(11 кл)", "");
            fileName = fileName.Replace(" ", "");

            return fileName;
        } 

        private static void Callback(object state)
        {
            if (seconds == 0)
            {
                timer.Change(0, 0);
                seconds = 180;
                isStart = true;
                Start();
                return;
            }

            if (!isStart)
            {
                seconds--;
                if (seconds / 60 != 0)
                    Console.WriteLine(seconds / 60 + "m " + seconds % 60 + "s");
                else
                    Console.WriteLine(seconds + "s");
            }

            isStart = false;
        }
    }
}
