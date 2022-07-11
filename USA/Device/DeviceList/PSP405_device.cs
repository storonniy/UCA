using System;
using System.IO;
using System.IO.Ports;
using UCA.DeviceDrivers;
using static UCA.Devices.DeviceResult;
using System.Threading;
using System.Globalization;
using UCA.Steps;
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

        public override DeviceResult DoCommand(Step step)
        {
            switch (step.Command)
            {
                case DeviceCommands.SetVoltage:
                    return SetVoltage(step, Psp405.SetVoltage);
                case DeviceCommands.SetCurrent:
                    var actualCurrent = SetCurrent(step);
                    return GetResult($"{step.DeviceName}: Установлен ток", step, UnitType.Current, actualCurrent);
                case DeviceCommands.PowerOn:
                    return PowerOn(step, Psp405.PowerOn);
                case DeviceCommands.PowerOff:
                    return PowerOff(step, Psp405.PowerOff);
                case DeviceCommands.SetCurrentLimit:
                    return SetCurrentLimit(step, Psp405.SetCurrentLimit);
                default:
                    return ResultError($"Неизвестная команда {step.Command}");
            }
        }

        public double SetCurrent(Step step)
        {
            Psp405.SetVoltageLimit(40);
            Psp405.SetCurrentLimit(step.UpperLimit);
            double current = Math.Abs(double.Parse(step.Argument));
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