using System;
using System.Threading;

namespace TimeTableManager
{
    class Program
    {
        static MailService.Mailer mailer = new MailService.Mailer();
        private static Timer timer = new Timer(Callback, null, 0, 0);
        private static bool isStart = true;
        private static int seconds = 180;

        static void Main(string[] args)
        {
            Start();
            Console.Read();
        }

        private static void Start()
        {
            Console.WriteLine();

            var attachmet = mailer.GetAttachment();
            if (attachmet != "")
            {

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
