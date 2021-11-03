using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;
using System.Globalization;

namespace UCA.DeviceDrivers
{
    public class AKIP_3407
    {
        private SerialPort serialPort = new SerialPort();
        private readonly int delay = 1000;

        public AKIP_3407(SerialPort serialPort)
        {
            this.serialPort = serialPort;
            //this.serialPort.Open();
        }

        
        public double SetVoltage (double voltage)
        {
            var command = $"VOLT {voltage};VOLT ?/n";
            serialPort.WriteLine(command);
            Thread.Sleep(delay);
            return double.Parse(serialPort.ReadLine(), CultureInfo.InvariantCulture);
        }
        

        ~AKIP_3407()
        {
            //serialPort.Close();
        }
    }
}
