using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Renci.SshNet;

namespace TimeTableManager.NetworkService
{
    static class SftpManager
    {
        public static void FileUploadSFTP(string fileName, string filePath)
        {
            var host = "kp11.ru";
            var port = 24;
            var username = "ek";
            var password = "Cisco123$";

            // path for file you want to upload
            var uploadFile = filePath;

            using (var client = new SftpClient(host, port, username, password))
            {
                client.Connect();
                if (client.IsConnected)
                {
                    Debug.WriteLine("I'm connected to the client");

                    using (var fileStream = new FileStream(uploadFile, FileMode.Open))
                    {

                        client.BufferSize = 4 * 1024; // bypass Payload error large files
                        client.UploadFile(fileStream, $@"/var/www/html/ek.kp11.ru/rasp/{fileName}");
                    }
                }
                else
                {
                    Debug.WriteLine("I couldn't connect");
                }
            }
        }
    }
}
