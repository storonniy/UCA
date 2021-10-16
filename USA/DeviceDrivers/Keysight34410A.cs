using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace UCA.DeviceDrivers
{
    public class Keysight34410
    {
        readonly SerialPort serialPort;
        public Keysight34410(SerialPort serialPort)
        {
            this.serialPort = serialPort;
            //serialPort.Open();
        }
        public double MeasureVoltageDC()
        {
            serialPort.WriteLine("line");
            return -9999;
        }

        public double MeasureCurrentDC()
        {
            serialPort.WriteLine("line");
            return -9999;
        }
    }
}
