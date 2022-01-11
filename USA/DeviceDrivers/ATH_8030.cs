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

        public bool SetCurrentControlMode()
        {
            var cmd = new ushort[] { 0x01 };
            return WriteHoldingRegInt(0x0A00, cmd);
        }

        public bool PowerOn()
        {
            var cmdArr = new ushort[] { 0x2a };
            return WriteHoldingRegInt(0x0A00, cmdArr);
        }

        public bool PowerOff()
        {
            var cmdArr = new ushort[] { 0x2b };
            return WriteHoldingRegInt(0x0A00, cmdArr);
        }

        public bool SetCurrent(float current)
        {
            return WriteHoldingRegFloat(0x0A00, current);
        }

        public bool SetConstantCurrent(float current)
        {
            return WriteHoldingRegFloat(0x0A01, current);
        }

        public float GetConstantCurrent()
        {
            return ReadHoldingReg(0x0A01, 1);
        }

        public float GetMaxCurrent()
        {
            return ReadHoldingReg(0x0A34, 1);
        }
    }
}
