using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using UCA.Devices;
using System.Threading;

namespace UCA.DeviceDrivers
{
    class PST_3201
    {
        private SerialPort serialPort;

        public PST_3201(SerialPort serialPort)
        {
            this.serialPort = serialPort;
            if (SerialPort.GetPortNames().ToList().Contains(serialPort.PortName))
                this.serialPort.Open();
        }

        private string DoCommandAndGetResult(string command)
        {
            serialPort.WriteLine(command);
            Thread.Sleep(1000);
            return serialPort.ReadLine();
        }
        
        private int SendAndParseDataNR1(string command)
        {
            return int.Parse(DoCommandAndGetResult(command));
        }

        private double SendAndParseDataNR2(string command)
        {
            var answer = DoCommandAndGetResult(command).Replace(".", ",");
            return double.Parse(answer);
        }
        
        private float SendAndParseDataNR3(string command)
        {
            return float.Parse(DoCommandAndGetResult(command));
        }
        
        private bool SendAndParseDataBoolean(string command)
        {
            return DoCommandAndGetResult(command) == "1";
        }

        /// <summary>
        /// Sets the output voltage of tyhe specific channel
        /// </summary>
        /// <param name="voltage"></param>
        /// <param name="channel"></param>
        /// <returns> Returns actual output voltage </returns>
        public double SetVoltage(double voltage, int channel)
        {
            if (1 > channel || channel > 3)
                throw new ArgumentException("Номер канала PST_3201 может быть равен 1, 2, 3");
            //var command = $":CHAN {channel}:VOLT {voltage};VOLT ?\n";
            var str = voltage.ToString().Replace(",", ".");
            var command = $":CHAN{channel}:VOLT {str};VOLT?\n";
            return SendAndParseDataNR2(command);
        }

        public double GetOutputVoltage(int channel)
        {
            if (1 > channel || channel > 3)
                throw new ArgumentException("Номер канала PST_3201 может быть равен 1, 2, 3");
            var command = $":CHAN{channel}:MEAS:VOLT?\n";
            return SendAndParseDataNR2(command);
        }

        public double SetCurrent(double current, int channel)
        {
            if (1 > channel || channel > 3)
                throw new ArgumentException("Номер канала PST_3201 может быть равен 1, 2, 3");
            var str = current.ToString().Replace(",", ".");
            var command = $":CHAN{channel}:CURR {str};CURR?\n";
            return SendAndParseDataNR2(command);
        }

        private bool ChangeOutputState(string outputState)
        {
            if (outputState != "0" && outputState != "1")
                throw new Exception("Состояние может принимать значение 0 (выключено) и 1 (включено)");
            var command = $":outp:stat {outputState}\n:outp:stat?";
            return SendAndParseDataBoolean(command);
        }
        /// <summary>
        /// Sets the Over Current Protection. Range: false (Off), true (On)
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>

        public void SetCurrentProtection(int channel, bool state)
        {
            if (1 > channel || channel > 3)
                throw new ArgumentException("Номер канала PST_3201 может быть равен 1, 2, 3");
            var currProtection = state ? "1" : "0";
            var command = $":chan{channel}:prot:curr {currProtection};:chan1:prot:curr?\n";
            DoCommand(command);
        }

        private void DoCommand(string command)
        {
            serialPort.WriteLine(command);
            Thread.Sleep(1000);
        }


        public void PowerOn()
        {
            ChangeOutputState("1");
        }
        public void PowerOff()
        {
            ChangeOutputState("0");
        }

        ~PST_3201()
        {
            if (serialPort.IsOpen)
                serialPort.Close();
        }
    }
}