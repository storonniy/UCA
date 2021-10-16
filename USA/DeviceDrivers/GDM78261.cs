using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.IO;
using System.Threading;
using System.Globalization;

namespace UCA.DeviceDrivers
{
    public class GDM78261
    {
        readonly SerialPort serialPort;
        public GDM78261 (SerialPort serialPort)
        {
            this.serialPort = serialPort;
            //this.serialPort.Open();
        }

        /// <summary>
        /// Returns the DC voltage measurement as N mV.
        /// </summary>
        /// <returns></returns>
        public double MeasureVoltageDC()
        {
            serialPort.WriteLine("MEAS:VOLT:DC?#013");
            Thread.Sleep(1000);
            return ParseValue(serialPort.ReadLine());
        }
        /// <summary>
        /// Returns the DC current measurement, mA.
        /// </summary>
        /// <returns></returns>
        public double MeasureCurrentDC()
        {
            serialPort.WriteLine("MEAS:CURR:DC#013");
            Thread.Sleep(1000);
            return ParseValue(serialPort.ReadLine());
        }

        public static double ParseValue (string value)
        {
            value = value.Replace(">", "");
            int indexOfDecimalPart = value.IndexOf("E");
            int power = int.Parse(value.Substring(indexOfDecimalPart + 1));
            value = value.Remove(indexOfDecimalPart);
            double mainPartOfValue = double.Parse(value, CultureInfo.InvariantCulture);
            return mainPartOfValue * Math.Pow(10, power + 3);
        }

        ~GDM78261()
        {
            serialPort.Close();
        }
    }
}
