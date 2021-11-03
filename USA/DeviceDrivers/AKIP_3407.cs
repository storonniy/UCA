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

        #region Auxiliary methods
        private string SendAndParseData(string command)
        {
            serialPort.WriteLine(command);
            Thread.Sleep(delay);
            return serialPort.ReadLine();
        }

        #endregion


        public double SetVoltage (double voltage)
        {
            var command = $"VOLT {voltage};VOLT ?/n";
            return double.Parse(SendAndParseData(command), CultureInfo.InvariantCulture);
        }

        public double SetFrequency (double frequency)
        {
            var command = $"FREQ {frequency};FREQ ?/n";
            return double.Parse(SendAndParseData(command), CultureInfo.InvariantCulture);
        }

        #region Power Status
        public bool PowerOn()
        {
            return ChangePowerStatus("1");
        }

        public bool PowerOff()
        {
            return ChangePowerStatus("0");
        }

        private bool ChangePowerStatus(string status)
        {
            var command = $":outp:stat {status};:outp:stat?";
            return SendAndParseData(command) == "1";
        }

        #endregion


        ~AKIP_3407()
        {
            //serialPort.Close();
        }
    }
}
