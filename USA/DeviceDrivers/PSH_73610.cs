using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;

namespace UCA.DeviceDrivers
{
    public class PSH73610
    {
        readonly SerialPort serialPort;
        public PSH73610(SerialPort serialPort)
        {
            this.serialPort = serialPort;
            if (SerialPort.GetPortNames().ToList().Contains(serialPort.PortName))
                this.serialPort.Open();
        }

        ~PSH73610()
        {
            if (serialPort.IsOpen)
                serialPort.Close();
            if (serialPort != null)
                serialPort.Dispose();
        }

        private double DoCommandAndGetResult(string command)
        {
            DoCommand(command);
            var data = serialPort.ReadLine();
            return ParseInputData(data);
        }

        private void DoCommand(string command)
        {
            serialPort.WriteLine(command);
            Thread.Sleep(1000);
        }


        /// <summary>
        /// Sets the output voltage (unit: V).
        /// </summary>
        /// <param name="voltage"></param>
        /// <returns> Returns the output voltage (unit: V). </returns>
        public double SetVoltage(double voltage)
        {
            var str = voltage.ToString().Replace(",", ".");
            var command = $":chan1:volt {str};:chan1:volt?\n";
            DoCommand($":chan1:volt {str}\n");
            Thread.Sleep(500);
            return DoCommandAndGetResult(":chan1: volt ?\n");
        }

        /// <summary>
        /// Returns the actual output load voltage (unit: V).
        /// </summary>
        /// <returns></returns>
        public double GetActualOutputLoadVoltage()
        {
            var command = $":chan1:meas:volt?\n";
            return DoCommandAndGetResult(command);
        }

        /// <summary>
        /// Returns the actual output load current (unit: A).
        /// </summary>
        /// <returns></returns>
        public double GetActualOutputLoadCurrent()
        {
            var command = $":chan1:meas:curr?\n";
            return DoCommandAndGetResult(command);
        }


        /// <summary>
        /// Sets the output current (unit: A).
        /// </summary>
        /// <param name="current">Range: 0.01~rating curren t</param>
        /// <returns>Returns the output current(unit: A).</returns>
        public double SetCurrentLimit(double current)
        {
            var str = current.ToString().Replace(",", ".");
            var command = $":chan1:prot:curr 1;:chan1:curr {str};:chan1:curr?\n";
            return DoCommandAndGetResult(command);
        }

        /// <summary>
        /// Sets the Over Voltage Protection value.
        /// </summary>
        /// <param name="voltageProtection"> Range: 0 .01~rating (unit: </param>
        /// <returns> Over Voltage Protection value </returns>
        public double SetOverVoltageProtectionValue(double voltageProtection)
        {
            var str = voltageProtection.ToString().Replace(",", ".");
            var command = $":chan1:prot:volt {str};:chan1:prot:volt?\n";
            return DoCommandAndGetResult(command);
        }

        /// <summary>
        /// Sets the Over Current Protection. Range: false (Off), true (On)
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>

        public void SetCurrentProtection(bool state)
        {
            var currProtection = state ? "1" : "0";
            var command = $":chan1:prot:curr {currProtection};:chan1:prot:curr?\n";
            DoCommand(command);
        }

        private void ChangeOutputStatus(int value)
        {
            Thread.Sleep(2000);
            var command = $":outp:stat {value};:outp:stat?";
            serialPort.WriteLine(command);
            Thread.Sleep(1000);
        }

        public void PowerOn()
        {
            ChangeOutputStatus(1);
        }

        public void PowerOff()
        {
            ChangeOutputStatus(0);
        }

        private double ParseInputData(string data)
        {
            return (double)Single.Parse(data.Replace(".", ","));
        }
    }

}
