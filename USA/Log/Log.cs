using System;
using System.Collections.Generic;
using System.IO;

namespace UCA.Logging
{
    class Log
    {
        public Log()
        {
            FileName = GetFileName();
            streamWriter = GetStreamWriter();
        }

        private List<string> log;
        public string FileName;
        private readonly StreamWriter streamWriter;

        public void AddItem(string item)
        {
            log.Add(item);
        }

        private StreamWriter GetStreamWriter()
        {
            string pathToFile = Directory.GetCurrentDirectory() + "/" + FileName; //"Data" + "/" +
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

        public void Write()
        {
            foreach (var line in log)
                streamWriter.WriteLine(line);
            streamWriter.Flush();
        }

        public void WriteItem(string item)
        {
            streamWriter.WriteLine(item);
            streamWriter.Flush();
        }

        ~Log()
        {
            streamWriter.Close();
        }
    }
}
