using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using UCA.DeviceDrivers;
using System.Globalization;
using static UCA.Auxiliary.UnitValuePair;
using UPD.Device;

namespace UCA.Devices
{
    class AKIP3407_device : IDeviceInterface
    {
        int delay = 1000;
        private AKIP_3407 akip3407;
        public AKIP3407_device (SerialPort serialPort)
        {
            serialPort.NewLine = "\r";
            akip3407 = new AKIP_3407(serialPort);
        }
        public override DeviceResult DoCommand (DeviceData deviceData)
        {
            switch (deviceData.Command)
            {
                case DeviceCommands.SetVoltage:
                    return IDeviceInterface.SetVoltage(deviceData, akip3407.SetVoltage);
                case DeviceCommands.SetFrequency:
                    var actualFrequency = akip3407.SetFrequency(deviceData.Argument);
                    return GetResult("Установлена частота", deviceData, UnitType.Frequency, actualFrequency);
                case DeviceCommands.PowerOn:
                    return IDeviceInterface.PowerOn(deviceData, akip3407.PowerOn);
                case DeviceCommands.PowerOff:
                    return IDeviceInterface.PowerOff(deviceData, akip3407.PowerOff);
                default:
                    return DeviceResult.ResultError($"Неизвестная команда {deviceData.Command}");
            }
        }
    }
}
