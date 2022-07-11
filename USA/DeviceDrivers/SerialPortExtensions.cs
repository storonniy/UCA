using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UPD.DeviceDrivers
{
    public static class SerialPortExtensions
    {
        private const int delay = 1000;

        public static double ReadDouble(this SerialPort serialPort)
        {
            var value = serialPort.ReadLine().Replace("\r", "").Replace("\n", "");
            var result = double.NaN;
            value = value.Trim();
            if (!double.TryParse(value, NumberStyles.Any, CultureInfo.GetCultureInfo("ru-RU"), out result))
            {
                if (!double.TryParse(value, NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out result))
                {
                    throw new FormatException();
                }
            }
            return result;
        }

        public static void SendCommand(this SerialPort serialPort, string command)
        {
            serialPort.WriteLine(command);
            Thread.Sleep(delay);
        }
    }
}
