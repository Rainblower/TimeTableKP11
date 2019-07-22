using System;
using System.IO;

namespace TimeTableManager.MailService
{
    public class StorageManager
    {
        public static string CreateFilePath(String filename)
        {
            var strWorkingDirectory = Directory.GetCurrentDirectory();
            var fullFolderPath = Path.Combine(strWorkingDirectory, "Attachments");

            if (!Directory.Exists(fullFolderPath))
            {
                Directory.CreateDirectory(fullFolderPath);
                Console.WriteLine("Folder attachments created\n");
            }

            var filePath = Path.Combine(fullFolderPath, filename);

            return filePath;
        }
    }
}
