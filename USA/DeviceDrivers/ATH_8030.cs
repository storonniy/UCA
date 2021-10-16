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

        public bool OnOff(float value)
        {
            // ToDO: кинуть исключение, если value не 0 и не 1, тут написана поебень
            bool ans;
            var cmdArr = new ushort[1];
            if (value == 1)
                cmdArr[0] = 0x2a;
            else
                cmdArr[0] = 0x2b;
            ans = writeHoldingRegInt(0x0A00, cmdArr);
            return ans;
        }

        public bool SetCurrent(float current)
        {
            return writeHoldingRegFloat(0x0A00, current);
        }  
    }
}
