using System;
using System.Collections.Generic;
using System.IO;

namespace UCA.Logging
{
    class Log
    {
        public string FileName;
        private readonly StreamWriter streamWriter;

        public Log()
        {
            var path = @"C:\UCAlogs\";
            var dirInfo = new DirectoryInfo(path);
            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }
            FileName = path + GetFileName();
            streamWriter = GetStreamWriter();
        }

        private StreamWriter GetStreamWriter()
        {
            string pathToFile = FileName;
            FileStream aFile = new FileStream(pathToFile, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            return new StreamWriter(aFile);
        }

        private string GetFileName()
        {
            DateTime date = DateTime.Now;
            string fileName = date.ToString();
            var year = date.Year.ToString();
            fileName = fileName.Replace(":", "").Replace(year, year + "_").Replace(" ", "").Replace(".", "");
            return fileName + ".txt";
        }

        public void Send(string item)
        {
            lock (streamWriter)
            {
                streamWriter.WriteLine(item);
                streamWriter.Flush();
            }
        }

        /*
        ~Log()
        {
            streamWriter.Close();
        }
        */
    }
}
