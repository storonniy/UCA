using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCA.DeviceDrivers;
using System.IO.Ports;
using static UCA.Devices.DeviceResult;
using static UCA.Auxiliary.UnitValuePair;
using System.Globalization;
using UPD.Device;

namespace UCA.Devices
{
    public class PST3201_device : IDeviceInterface
    {
        PST_3201 pst3201;


        public PST3201_device (SerialPort serialPort)
        {
            pst3201 = new PST_3201(serialPort);
        }

        public override DeviceResult DoCommand (DeviceData deviceData)
        {
            switch (deviceData.Command)
            {
                case DeviceCommands.SetVoltage:
                    return IDeviceInterface.SetVoltage(deviceData, pst3201.SetVoltage);
                case DeviceCommands.SetCurrentLimit:
                    return IDeviceInterface.SetCurrent(deviceData, pst3201.SetCurrent);
                case DeviceCommands.PowerOff:
                    return IDeviceInterface.PowerOff(deviceData, pst3201.PowerOff);
                case DeviceCommands.PowerOn:
                    return IDeviceInterface.PowerOn(deviceData, pst3201.PowerOn);
                default:
                    return ResultError($"Неизвестная команда {deviceData.Command}");
            }
        }
    }
}
