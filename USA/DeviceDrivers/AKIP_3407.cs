using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;

namespace UCA.DeviceDrivers
{
    public class AKIP_3407
    {
        private SerialPort serialPort = new SerialPort();

        public AKIP_3407(SerialPort serialPort)
        {
            this.serialPort = serialPort;
            this.serialPort.Open();
        }

        ~AKIP_3407()
        {
            //serialPort.Close();
        }
    }
}
