using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;
using Checker.Auxiliary;
using Checker.DeviceDrivers;

namespace Checker.DeviceDrivers
{
    public class Psh73610
    {
        readonly SerialPort serialPort;
        public Psh73610(SerialPort serialPort)
        {
            this.serialPort = serialPort;
            if (SerialPort.GetPortNames().ToList().Contains(serialPort.PortName))
                this.serialPort.Open();
        }

        ~Psh73610()
        {
            if (serialPort.IsOpen)
                serialPort.Close();
            serialPort?.Dispose();
        }

        /// <summary>
        /// Sets the output voltage (unit: V).
        /// </summary>
        /// <param name="voltage"></param>
        /// <returns> Returns the output voltage (unit: V). </returns>
        public double SetVoltage(double voltage)
        {
            var str = voltage.ToString().Replace(",", ".");
            serialPort.SendCommand($":chan1:volt {str}\n");
            serialPort.SendCommand(":chan1: volt ?\n");
            return serialPort.ReadDouble();
        }

        /// <summary>
        /// Returns the actual output load voltage (unit: V).
        /// </summary>
        /// <returns></returns>
        public double GetActualOutputLoadVoltage()
        {
            serialPort.SendCommand($":chan1:meas:volt?\n");
            return serialPort.ReadDouble();
        }

        private double ParseValue()
        {
            var data = serialPort.ReadLine();
            return (double)Single.Parse(data.Replace(".", ","));
        }

        /// <summary>
        /// Returns the actual output load current (unit: A).
        /// </summary>
        /// <returns></returns>
        public double GetActualOutputLoadCurrent()
        {
            serialPort.SendCommand($":chan1:meas:curr?\n");
            return serialPort.ReadDouble();
        }


        /// <summary>
        /// Sets the output current (unit: A).
        /// </summary>
        /// <param name="current">Range: 0.01~rating curren t</param>
        /// <returns>Returns the output current(unit: A).</returns>
        public double SetCurrentLimit(double current)
        {
            var str = current.ToString().Replace(",", ".");
            serialPort.SendCommand($":chan1:prot:curr 1;:chan1:curr {str};:chan1:curr?\n");
            return serialPort.ReadDouble();
        }

        /// <summary>
        /// Sets the Over Voltage Protection value.
        /// </summary>
        /// <param name="voltageProtection"> Range: 0 .01~rating (unit: </param>
        /// <returns> Over Voltage Protection value </returns>
        public double SetOverVoltageProtectionValue(double voltageProtection)
        {
            var str = voltageProtection.ToString().Replace(",", ".");
            serialPort.SendCommand($":chan1:prot:volt {str};:chan1:prot:volt?\n");
            return serialPort.ReadDouble();
        }

        /// <summary>
        /// Sets the Over Current Protection. Range: false (Off), true (On)
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>

        public void SetCurrentProtection(bool state)
        {
            var currProtection = state ? "1" : "0";
            serialPort.SendCommand($":chan1:prot:curr {currProtection};:chan1:prot:curr?\n");
        }

        private void ChangeOutputStatus(int value)
        {
            serialPort.SendCommand($":outp:stat {value};:outp:stat?");
        }

        public void PowerOn()
        {
            ChangeOutputStatus(1);
        }

        public void PowerOff()
        {
            ChangeOutputStatus(0);
        }
    }

}
