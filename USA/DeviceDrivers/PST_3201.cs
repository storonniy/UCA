using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using UCA.Devices;
using System.Threading;
using UPD.DeviceDrivers;

namespace UCA.DeviceDrivers
{
    class PST_3201
    {
        private readonly SerialPort serialPort;

        public PST_3201(SerialPort serialPort)
        {
            this.serialPort = serialPort;
            if (SerialPort.GetPortNames().ToList().Contains(serialPort.PortName))
                this.serialPort.Open();
        }
        
        private int ReadDataNr1()
        {
            return int.Parse(serialPort.ReadLine());
        }
        
        private float ReadDataNr3(string command)
        {
            return float.Parse(serialPort.ReadLine());
        }
        
        private bool ReadBoolean()
        {
            return serialPort.ReadLine() == "1";
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
            var str = voltage.ToString().Replace(",", ".");
            serialPort.SendCommand($":CHAN{channel}:VOLT {str};VOLT?\n");
            return ParseValue();
        }

        private double ParseValue()
        {
            Thread.Sleep(1000);
            return double.Parse(serialPort.ReadLine().Replace(".", ","));
        }

        public double GetOutputVoltage(int channel)
        {
            if (1 > channel || channel > 3)
                throw new ArgumentException("Номер канала PST_3201 может быть равен 1, 2, 3");
            serialPort.SendCommand($":CHAN{channel}:MEAS:VOLT?\n");
            return ParseValue();
        }

        public double SetCurrent(double current, int channel)
        {
            if (1 > channel || channel > 3)
                throw new ArgumentException("Номер канала PST_3201 может быть равен 1, 2, 3");
            var str = current.ToString().Replace(",", ".");
            serialPort.SendCommand($":CHAN{channel}:CURR {str};CURR?\n");
            return ParseValue();
        }

        private bool ChangeOutputState(string outputState)
        {
            if (outputState != "0" && outputState != "1")
                throw new Exception("Состояние может принимать значение 0 (выключено) и 1 (включено)");
            serialPort.SendCommand($":outp:stat {outputState}\n:outp:stat?");
            return ReadBoolean();
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
            serialPort.SendCommand(command);
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