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
            var command = $"SOUR1:VOLT {voltage}#013#010;VOLT?#013#010";
            return double.Parse(SendAndParseData(command), CultureInfo.InvariantCulture);
        }

        public double SetFrequency (string frequency)
        {
            var command = $"FREQ {frequency} #013#010;SOUR1:FREQ?#013#010";
            // TODO: проверить формат возвращаемого значения от AKIP_3407 и верно его распарсить
            return double.Parse(SendAndParseData(command), CultureInfo.InvariantCulture);
        }

        #region Power Status
        public bool PowerOn()
        {
            return ChangePowerStatus("ON");
        }

        public bool PowerOff()
        {
            return ChangePowerStatus("OFF");
        }

        private bool ChangePowerStatus(string status)
        {
            var command = $"OUTP1 {status}#013#010; OUTP1:STAT?#013#010";
            return SendAndParseData(command) == "1";
        }

        #endregion


        ~AKIP_3407()
        {
            //serialPort.Close();
        }
    }
}
