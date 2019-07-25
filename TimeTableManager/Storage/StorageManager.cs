using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace TimeTableManager.MailService
{
    public class StorageManager
    {
        public static string CreateFilePath(string filename, string folderName)
        {
            var strWorkingDirectory = Directory.GetCurrentDirectory();
            var fullFolderPath = Path.Combine(strWorkingDirectory, folderName);

            if (!Directory.Exists(fullFolderPath))
            {
                Directory.CreateDirectory(fullFolderPath);
                Console.WriteLine($"Folder {folderName} created\n");
            }

            var filePath = Path.Combine(fullFolderPath, filename);

            return filePath;
        }

        public static void CreateFile(string filename, string folderName, string fileText, string expansion)
        {
            var path = CreateFilePath(filename, folderName) + $".{expansion}";

            using (FileStream fs = File.Create(path))
            {
                // Add some text to file    
                Byte[] title = new UTF8Encoding(true).GetBytes(fileText);
                fs.Write(title, 0, title.Length);
                Console.WriteLine("File created: " + filename + "\n");
            }
        }

        public static string ReadFilePath(string _path)
        {
            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), _path);

            string text = File.ReadAllText(path);

            return text;
        }
    }
}
