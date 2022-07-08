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
        private static int delay = 1000;

        public static double ReadDouble(this SerialPort serialPort)
        {
            var value = serialPort.ReadExisting().Replace("\r", "").Replace("\n", "").Replace(".", ",");
            try
            {
                return (double)decimal.Parse(value, NumberStyles.Float, CultureInfo.InvariantCulture);
            }
            catch (FormatException)
            {
                throw new FormatException($"[{value}] не может быть преобразован в double");
            }
        }

        public static void SendCommand(this SerialPort serialPort, string command)
        {
            serialPort.WriteLine(command);
            Thread.Sleep(delay);
        }


    }
}
