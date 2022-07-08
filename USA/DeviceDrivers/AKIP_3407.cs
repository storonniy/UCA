using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;
using System.Globalization;
using UPD.DeviceDrivers;

namespace UCA.DeviceDrivers
{
    public class AKIP_3407
    {
        private SerialPort serialPort;
        private readonly int delay = 2000;

        public AKIP_3407(SerialPort serialPort)
        {
            this.serialPort = serialPort;
            this.serialPort.Open();
        }

        private void SendCommand(string command)
        {
            var bytes = GetBytes(command);
            serialPort.Write(bytes, 0, bytes.Length);
            Thread.Sleep(delay);
        }

        private byte[] GetBytes(string command)
        {
            var bytes = Encoding.ASCII.GetBytes(command);
            byte[] result = new byte[bytes.Length + 2];
            Array.Copy(bytes, result, bytes.Length);
            result[bytes.Length] = 0x0D;
            result[bytes.Length + 1] = 0x0A;
            return result;
        }

        public double SetVoltage (double voltage)
        {
            var str = voltage.ToString().Replace(",", ".");
            SendCommand($"SOUR1:VOLT {str}");
            SendCommand("SOUR1:VOLT?");
            return serialPort.ReadDouble();
        }

        public double SetFrequency (string frequency)
        {
            SendCommand($"SOUR1:FREQ {frequency}");
            SendCommand("SOUR1:FREQ?");
            return serialPort.ReadDouble();
        }

        #region Power Status
        public bool PowerOn()
        {
            return ChangePowerStatus("1");
        }

        public bool PowerOff()
        {
            return !ChangePowerStatus("0");
        }

        private bool ChangePowerStatus(string status)
        {
            SendCommand($"OUTP1 {status}");
            return serialPort.ReadExisting() == "1";
        }

        #endregion

        ~AKIP_3407()
        {
            if (serialPort.IsOpen)
                serialPort.Close();
        }
    }
}
