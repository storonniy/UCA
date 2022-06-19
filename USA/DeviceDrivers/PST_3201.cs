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
        private SerialPort serialPort = new SerialPort();

        public PST_3201(SerialPort serialPort)
        {
            this.serialPort = serialPort;
            //this.serialPort.Open();
        }

        private string SendAndParseData(string command)
        {
            serialPort.WriteLine(command);
            Thread.Sleep(1000);
            return serialPort.ReadLine();
        }
        
        private int SendAndParseDataNR1(string command)
        {
            return int.Parse(SendAndParseData(command));
        }

        private double SendAndParseDataNR2(string command)
        {
            return double.Parse(SendAndParseData(command));
        }
        
        private float SendAndParseDataNR3(string command)
        {
            return float.Parse(SendAndParseData(command));
        }
        
        private bool SendAndParseDataBoolean(string command)
        {
            return SendAndParseData(command) == "1";
        }

        /// <summary>
        /// Sets the value of voltage
        /// </summary>
        /// <param name="voltage"></param>
        /// <param name="channel"></param>
        /// <returns> Returns actual output voltage </returns>
        public double SetVoltage(double voltage, int channel)
        {
            if (1 > channel || channel > 3)
                throw new ArgumentException("Номер канала PST_3201 может быть равен 1, 2, 3");
            var command = $":CHAN {channel}:VOLT {voltage};VOLT ?/n";
            return SendAndParseDataNR2(command);
        }

        public double SetCurrent(double current, int channel)
        {
            if (1 > channel || channel > 3)
                throw new ArgumentException("Номер канала PST_3201 может быть равен 1, 2, 3");
            var command = $":CHAN {channel}:CURR {current};CURR ?/n";
            return SendAndParseDataNR2(command);
        }

        private bool ChangeOutputState(string outputState)
        {
            if (outputState != "0" && outputState != "1")
                throw new Exception("Состояние может принимать значение 0 (выключено) и 1 (включено)");
            var command = $":outp:stat {outputState}/n:outp:stat?";
            return SendAndParseDataBoolean(command);
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
            //serialPort.Close();
        }
    }
}