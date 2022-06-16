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
    class PSP405_device : Source
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
                    var actualVoltage = SetVoltage(deviceData);
                    return GetResult(deviceData, UnitType.Voltage, actualVoltage);

                case DeviceCommands.SetCurrent:
                    var actualCurrent = SetCurrent(deviceData);
                    return GetResult(deviceData, UnitType.Current, actualCurrent);
                case DeviceCommands.PowerOn:
                    PowerOn();
                    return ResultOk($"Подан входной сигнал с {deviceData.DeviceName}");
                case DeviceCommands.PowerOff:
                    PowerOff();
                    return ResultOk($"Снят входной сигнал с {deviceData.DeviceName}");
                case DeviceCommands.SetCurrentLimit:            
                    var currentLimit = SetCurrentLimit(deviceData);
                    return GetResult("Установлен предел по току", deviceData, UnitType.Current, currentLimit);
                default:
                    return ResultError($"Неизвестная команда {deviceData.Command}");
            }
        }

        public override void PowerOff()
        {
            Psp405.TurnOff();
            Psp405.SetVoltage(0.0);
            Thread.Sleep(delay);
        }

        public override void PowerOn()
        {
            Psp405.TurnOn();
            Thread.Sleep(delay);
        }

        public override double SetCurrent(DeviceData deviceData)
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

        public override double SetCurrentLimit(DeviceData deviceData)
        {
            var currentLimit = double.Parse(deviceData.Argument, CultureInfo.InvariantCulture);
            Psp405.SetCurrentLimit(currentLimit);
            Thread.Sleep(delay);
            return Psp405.GetCurrentLimit();
        }

        public override double SetVoltage(DeviceData deviceData)
        {
            Psp405.SetVoltageLimit((int)deviceData.UpperLimit);
            Thread.Sleep(delay);
            var voltage = double.Parse(deviceData.Argument);
            Psp405.SetVoltage(voltage);
            Thread.Sleep(delay);
            return Psp405.GetOutputVoltage();
        }
    }
}