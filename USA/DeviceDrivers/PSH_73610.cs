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

        private float DoCommandAndGetResult(string command)
        {
            serialPort.WriteLine(command);
            Thread.Sleep(1000);
            return ParseInputData(serialPort.ReadLine());
        }

        // public float Voltage
        // {
        //     set
        //     {
        //         DoCommandAndGetResult($":chan1:volt {value}/n");
        //     }
        //     get
        //     {
        //         return DoCommandAndGetResult($":chan1:volt?/n");
        //     }
        // }

        public float SetVoltage(float voltage)
        {
            var command = $":chan1:volt {voltage};:chan1:volt?/n";
            return DoCommandAndGetResult(command);
        }

        public float SetCurrent(float current)
        {
            var command = $":chan1:curr {current};:chan1:curr?/n";
            return DoCommandAndGetResult(command);
        }

        public float SetVoltageProtection(float voltageProtection)
        {
            var command = $":chan1:prot:volt {voltageProtection};:chan1:prot:volt?/n";
            return DoCommandAndGetResult(command);
        }

        public float SetCurrentProtection(float currentProtection)
        {
            var command = $":chan1:prot:curr {currentProtection};:chan1:prot:curr?/n";
            return DoCommandAndGetResult(command);
        }

        public float ChangeOutputStatus(float value)
        {
            var command = $":outp:stat {value};:outp:stat?";
            return DoCommandAndGetResult(command);
        }

        private float ParseInputData(string data)
        {
            return Single.Parse(data.Replace(".", ","));
        }

        ~PSH73610()
        {
            //serialPort.Close();
        }
    }

}
