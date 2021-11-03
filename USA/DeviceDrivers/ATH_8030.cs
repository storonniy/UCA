using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;

namespace UCA.DeviceDrivers
{
    public class ATH_8030 : Modbus_device
    {
        public ATH_8030 (string portName) : base(portName)
        {

        }

        public bool PowerOn()
        {
            var cmdArr = new ushort[] { 0x2a };
            return writeHoldingRegInt(0x0A00, cmdArr);
        }

        public bool PowerOff()
        {
            var cmdArr = new ushort[] { 0x2b };
            return writeHoldingRegInt(0x0A00, cmdArr);
        }

        public bool SetCurrent(float current)
        {
            return writeHoldingRegFloat(0x0A00, current);
        }  
    }
}
