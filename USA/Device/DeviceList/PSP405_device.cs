using System;
using System.IO;
using System.IO.Ports;
using UCA.DeviceDrivers;
using static UCA.Devices.DeviceResult;
using System.Threading;
using System.Globalization;

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

        /// TODO - допиши это говно
        public override DeviceResult DoCommand(DeviceData deviceData)
        {
            switch (deviceData.Command)
            {
                case DeviceCommands.SetVoltage:
                    var voltage = double.Parse(deviceData.Argument);
                    Psp405.SetVoltage(voltage);
                    Thread.Sleep(delay);
                    Psp405.SetVoltageLimit(40);
                    Thread.Sleep(delay);
                    var actualVoltage = Psp405.GetOutputVoltage();
                    if (Math.Abs(voltage - actualVoltage) < 0.1 * Math.Abs(voltage))
                    {
                        return ResultOk($"Установка напряжения {voltage} прошла успешно");
                    }
                    else
                    {
                        return ResultError($"Установлено неверное напряжение {actualVoltage}");
                    }
                case DeviceCommands.SetCurrent:
                    double current = Math.Abs(double.Parse(deviceData.Argument)); // мкА
                    Thread.Sleep(delay);
                    double voltage1 = current * 480000;
                    Psp405.SetVoltage(voltage1);
                    var actualVolt = Psp405.GetOutputVoltage();
                    if (Math.Abs(actualVolt - voltage1) < 0.1 * Math.Abs(actualVolt))
                    {
                        return ResultOk($"Установка тока {current} прошла успешно");
                    }
                    else
                    {
                        return ResultError($"Установлено неверное значение напряжения {actualVolt}");
                    }
                case DeviceCommands.PowerOn:
                    Psp405.TurnOn();
                    Thread.Sleep(delay);
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
                    Thread.Sleep(delay);
                    if (Psp405.GetRelayStatus())
                    {
                        return ResultOk("Отключение устройства прошло успешно");
                    }
                    else
                    {
                        return ResultError("Не удалось отключить устройство");
                    }
                case DeviceCommands.SetCurrentLimit:
                    var currentLimit = double.Parse(deviceData.Argument, CultureInfo.InvariantCulture);
                    Psp405.SetCurrentLimit(currentLimit);
                    Thread.Sleep(delay);
                    var actualCurrentLimit = Psp405.GetCurrentLimit();
                    if (Math.Abs(currentLimit - actualCurrentLimit) < 0.1)
                    {
                        return ResultOk("Установка предела по току прошла успешно");
                    }
                    else
                    {
                        return ResultError($"ОШИБКА: установлен неверный предел по току: {actualCurrentLimit}, необходим {currentLimit}");
                    }
                default:
                    return ResultError($"Неизвестная команда {deviceData.Command}");
            }
        }
    }
}