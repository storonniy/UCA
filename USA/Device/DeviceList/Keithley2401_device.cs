using System;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Threading;
using UCA.DeviceDrivers;
using UPD.Device;
using static UCA.Auxiliary.UnitValuePair;


namespace UCA.Devices
{
    class Keithley2401_device : IDeviceInterface
    {
        readonly Keithley2401 keithley2401;

        public Keithley2401_device(SerialPort serialPort)
        {
            serialPort.NewLine = "\r";
            keithley2401 = new Keithley2401(serialPort);
        }

        public override DeviceResult DoCommand(DeviceData deviceData)
        {
            switch (deviceData.Command)
            {
                case DeviceCommands.SetVoltage:
                    return IDeviceInterface.SetVoltage(deviceData, keithley2401.SetVoltage);
                case DeviceCommands.SetCurrentLimit:
                    return IDeviceInterface.SetCurrentLimit(deviceData, keithley2401.SetCurrentLimit);
                case DeviceCommands.PowerOn:
                    return IDeviceInterface.PowerOn(deviceData, keithley2401.PowerOn);
                case DeviceCommands.PowerOff:
                    return IDeviceInterface.PowerOff(deviceData, keithley2401.PowerOff);
                case DeviceCommands.SetVoltageSourceMode:
                    var status = keithley2401.SetVoltageSourceMode();
                    if (status)
                        return DeviceResult.ResultOk($"{deviceData.DeviceName} переведен в режим стабилизации напряжения");
                    return DeviceResult.ResultError($"{deviceData.DeviceName} не удалось перевести в режим стабилизации напряжения");
                default:
                    return DeviceResult.ResultError($"Неизвестная команда {deviceData.Command}");
            }
        }
    }
}