using System;
using System.IO;
using System.IO.Ports;
using UCA.DeviceDrivers;
using static UCA.Devices.DeviceResult;

namespace UCA.Devices
{
    class PSP405_device : IDeviceInterface
    {
        readonly PSP405 Psp405;

        public PSP405_device(SerialPort serialPort)
        {
            Psp405 = new PSP405(serialPort);
        }

        /// TODO - допиши это говно
        public DeviceResult DoCommand(DeviceData deviceData)
        {
            switch (deviceData.Command)
            {
                case DeviceCommands.SetVoltage:
                    var voltage = double.Parse(deviceData.Argument);
                    Psp405.SetVoltage(voltage);
                    var actualVoltage = Psp405.GetOutputVoltage();
                    if (Math.Abs(voltage - actualVoltage) < 0.01)
                    {
                        return ResultOk($"Установка напряжения {voltage} прошла успешно");
                    }
                    else
                    {
                        return ResultError($"Установлено неверное напряжение {actualVoltage}");
                    }
                case DeviceCommands.SetCurrent:
                    double current = double.Parse(deviceData.Argument) * Math.Pow(10, -6); // мкА
                    Psp405.SetCurrentLimit(current + 0.1 * current);
                    double voltage1 = current * 480;
                    Psp405.SetVoltage(voltage1);
                    var actualCurrent = Psp405.GetOutputCurrent() * Math.Pow(10, -6);
                    var dA = actualCurrent - current;
                    if (Math.Abs(dA) < 0.01)
                    {
                        return ResultOk($"Установка тока {current} прошла успешно");
                    }
                    else
                    {
                        return ResultError($"Установлено неверное значение тока {actualCurrent}");
                    }
                case DeviceCommands.PowerOn:
                    Psp405.TurnOn();
                    if (Psp405.GetRelayStatus())
                    {
                        return ResultOk("Включение устройства прошло успешно");
                    }
                    else
                    {
                        return ResultError("Не удалось включить устройство");
                    }
                case DeviceCommands.PowerOff:
                    Psp405.TurnOff();
                    if (Psp405.GetRelayStatus())
                    {
                        return ResultOk("Отключение устройства прошло успешно");
                    }
                    else
                    {
                        return ResultError("Не удалось отключить устройство");
                    }
                default:
                    return ResultError($"Неизвестная команда {deviceData.Command}");
            }
        }
    }
}