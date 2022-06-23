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
        private SerialPort serialPort;
        private readonly int delay = 2000;

        public AKIP_3407(SerialPort serialPort)
        {
            this.serialPort = serialPort;
            this.serialPort.Open();
        }

        #region Auxiliary methods
        private string DoCommandAndGetResult(string command)
        {
            DoCommand(command);
            return serialPort.ReadExisting();
        }

        #endregion

        private void DoCommand(string command)
        {
            var bytes = GetBytes(command);
            serialPort.Write(bytes, 0, bytes.Length);
            //serialPort.Write(command);
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
            //var command = $"SOUR2:VOLT {str}";
            DoCommand($"SOUR1:VOLT {str}");
            var result = DoCommandAndGetResult("SOUR1:VOLT?").Replace("\n", "");
            return double.Parse(result, CultureInfo.InvariantCulture);
        }

        public double SetFrequency (string frequency)
        {
            var command = $"SOUR1:FREQ {frequency}";
            // TODO: проверить формат возвращаемого значения от AKIP_3407 и верно его распарсить
            var result = DoCommandAndGetResult("SOUR1:FREQ?");
            return double.Parse(result, CultureInfo.InvariantCulture);
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
            var command = $"OUTP1 {status}";
            DoCommand(command);
            var result = DoCommandAndGetResult(command);
            return result == "1";
        }

        #endregion


        ~AKIP_3407()
        {
            //serialPort.Close();
        }
    }
}
