using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
        private static int seconds = 180;

        private static List<string> groupsNameList = new List<string>()
        {
            "IB-21", "IB-22", "C-12", "C-21", "KC-21", "KC-32", "VKC-32","KCiK-11", "KCiK-22", "ISiP-12","ISiP-13","ISiP-21","PRV-33","PRV-42"
        };

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

                for (int i = 0; i < tables.Count; i++)
                {
                    var values = groupsNameList[i].Split("/");
                    var fileName = values[values.Length-1];
                    var filePath = StorageManager.CreateFile(fileName,"Tables",tables[i],"html");
                    SftpManager.FileUploadSFTP(fileName + ".html", filePath);
                }
            }
            else
            {
                Console.WriteLine("No new file\n");
            }
            timer.Change(0, 1000);
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
