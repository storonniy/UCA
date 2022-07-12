using System;
using System.Collections.Generic;
using System.IO;
using Checker.Settings;
using static Checker.Settings.ControlObjectSettings;

namespace Checker.Logging
{
    class Log
    {
        public string FileName;
        private readonly StreamWriter streamWriter;
        private ControlObjectSettings.Settings settings;

        public Log(ControlObjectSettings.Settings settings)
        {
            this.settings = settings;
            var path = @"C:\NS03\";
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
            fileName = $"{settings.FactoryNumber} {settings.Comment} {settings.OperatorName} {fileName.Replace(":", "").Replace(year, year + "_").Replace(" ", "").Replace(".", "")}";//fileName.Replace(":", "").Replace(year, year + "_").Replace(" ", "").Replace(".", "");
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
