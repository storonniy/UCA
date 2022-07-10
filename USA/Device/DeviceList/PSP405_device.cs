using System;
using System.IO;
using System.IO.Ports;
using UCA.DeviceDrivers;
using static UCA.Devices.DeviceResult;
using System.Threading;
using System.Globalization;
using static UCA.Auxiliary.UnitValuePair;
using UPD.Device;

namespace UCA.Devices
{
    class PSP405_device : IDeviceInterface
    {
        readonly int delay = 500;
        readonly PSP405 Psp405;

        public PSP405_device(SerialPort serialPort)
        {
            Psp405 = new PSP405(serialPort);
        }

        public override DeviceResult DoCommand(DeviceData deviceData)
        {
            switch (deviceData.Command)
            {
                case DeviceCommands.SetVoltage:
                    return SetVoltage(deviceData, Psp405.SetVoltage);
                case DeviceCommands.SetCurrent:
                    var actualCurrent = SetCurrent(deviceData);
                    return GetResult($"{deviceData.DeviceName}: Установлен ток", deviceData, UnitType.Current, actualCurrent);
                case DeviceCommands.PowerOn:
                    return PowerOn(deviceData, Psp405.PowerOn);
                case DeviceCommands.PowerOff:
                    return PowerOff(deviceData, Psp405.PowerOff);
                case DeviceCommands.SetCurrentLimit:
                    return SetCurrentLimit(deviceData, Psp405.SetCurrentLimit);
                default:
                    return ResultError($"Неизвестная команда {deviceData.Command}");
            }
        }

        public double SetCurrent(DeviceData deviceData)
        {
            Psp405.SetVoltageLimit(40);
            Psp405.SetCurrentLimit(deviceData.UpperLimit);
            double current = Math.Abs(double.Parse(deviceData.Argument));
            Thread.Sleep(delay);
            double resistance = 480000.0;
            double voltage = current * resistance;
            Thread.Sleep(delay);
            Psp405.SetVoltage(voltage);
            Thread.Sleep(delay);
            var volt = Psp405.GetOutputVoltage();
            var actualCurrent = volt / resistance;
            return actualCurrent;
        }
    }
}