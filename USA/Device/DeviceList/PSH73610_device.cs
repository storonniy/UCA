using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCA.DeviceDrivers;
using System.IO.Ports;
using static UCA.Devices.DeviceResult;
using System.Globalization;
using static UCA.Auxiliary.UnitValuePair;
using UPD.Device;

namespace UCA.Devices
{
    class PSH73610_device : IDeviceInterface
    {
        readonly PSH73610 psh73610;

        public PSH73610_device(SerialPort serialPort)
        {
            this.psh73610 = new PSH73610(serialPort);
        }

        public override DeviceResult DoCommand(DeviceData deviceData)
        {
            switch (deviceData.Command)
            {
                case DeviceCommands.SetVoltage:
                    return SetVoltage(deviceData, psh73610.SetVoltage);
                case DeviceCommands.SetCurrentLimit:
                    return SetCurrentLimit(deviceData, psh73610.SetCurrentLimit);
                case DeviceCommands.PowerOff:
                    return PowerOff(deviceData, psh73610.PowerOff);
                case DeviceCommands.PowerOn:
                    return PowerOn(deviceData, psh73610.PowerOn);
                default:
                    return ResultError($"Неизвестная команда {deviceData.Command}");
            }
        }
    }
}
