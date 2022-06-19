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
            //this.serialPort.Open();
        }

        private double DoCommandAndGetResult(string command)
        {
            serialPort.WriteLine(command);
            Thread.Sleep(1000);
            return ParseInputData(serialPort.ReadLine());
        }

        public double SetVoltage(double voltage)
        {
            var str = voltage.ToString().Replace(",", ".");
            var command = $":chan1:volt {str};:chan1:volt?/n";
            return DoCommandAndGetResult(command);
        }

        public double SetCurrentLimit(double current)
        {
            var str = current.ToString().Replace(",", ".");
            var command = $":chan1:curr {str};:chan1:curr?/n";
            return DoCommandAndGetResult(command);
        }

        public double SetVoltageProtection(double voltageProtection)
        {
            var command = $":chan1:prot:volt {voltageProtection};:chan1:prot:volt?/n";
            return DoCommandAndGetResult(command);
        }

        public double SetCurrentProtection(double currentProtection)
        {
            var command = $":chan1:prot:curr {currentProtection};:chan1:prot:curr?/n";
            return DoCommandAndGetResult(command);
        }

        public void ChangeOutputStatus(int value)
        {
            Thread.Sleep(2000);
            var command = $":outp:stat {value};:outp:stat?";
            serialPort.WriteLine(command);
            Thread.Sleep(1000);
            //return DoCommandAndGetResult(command);
        }

        private double ParseInputData(string data)
        {
            return (double)Single.Parse(data.Replace(".", ","));
        }

        ~PSH73610()
        {
            //serialPort.Close();
        }
    }

}
