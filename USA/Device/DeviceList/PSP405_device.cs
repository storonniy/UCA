using System;
using System.IO;
using System.IO.Ports;
using UCA.DeviceDrivers;
using static UCA.Devices.DeviceResult;
using System.Threading;
using System.Globalization;
using static UCA.Auxiliary.UnitValuePair;


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
                    var lowerLimit = deviceData.LowerLimit;
                    var upperLimit = deviceData.UpperLimit;
                    Psp405.SetVoltageLimit((int)upperLimit);
                    Thread.Sleep(delay);
                    var voltage = double.Parse(deviceData.Argument);
                    Psp405.SetVoltage(voltage);
                    Thread.Sleep(delay);
                    var actualVoltage = Psp405.GetOutputVoltage();
                    var result = $"Уcтановлено напряжение {GetValueUnitPair(actualVoltage, UnitType.Voltage)} \t Нижний предел: {GetValueUnitPair(lowerLimit, UnitType.Voltage)}\t Верхний предел {GetValueUnitPair(upperLimit, UnitType.Voltage)}";
                    if (actualVoltage >= lowerLimit && actualVoltage <= upperLimit)
                    {
                        return ResultOk(result);
                    }
                    else
                    {
                        return ResultError("Ошибка: " + result);
                    }
                case DeviceCommands.SetCurrent:
                    Psp405.SetVoltageLimit(40);
                    var lowerLimitCurrent = deviceData.LowerLimit;
                    var upperLimitCurrent = deviceData.UpperLimit;
                    Psp405.SetCurrentLimit(upperLimitCurrent);
                    double current = Math.Abs(double.Parse(deviceData.Argument));
                    Thread.Sleep(delay);
                    double resistance = 480000.0;
                    double voltage1 = current * resistance;
                    Thread.Sleep(delay);
                    Psp405.SetVoltage(voltage1);
                    Thread.Sleep(delay);
                    var volt = Psp405.GetOutputVoltage();
                    var actualCurrent = volt / resistance;
                    var resultCurrent = $"Уcтановлено значение тока {GetValueUnitPair(actualCurrent, UnitType.Current)} \t Нижний предел: {GetValueUnitPair(lowerLimitCurrent, UnitType.Current)}\t Верхний предел {GetValueUnitPair(upperLimitCurrent, UnitType.Current)}";
                    if (actualCurrent >= lowerLimitCurrent && actualCurrent <= upperLimitCurrent)
                    {
                        return ResultOk(resultCurrent);
                    }
                    else
                    {
                        return ResultError("Ошибка: " + resultCurrent);
                    }
                case DeviceCommands.PowerOn:
                    Psp405.TurnOn();
                    Thread.Sleep(delay);
                    return ResultOk($"Включение {deviceData.DeviceName} прошло успешно");
                /*
                if (Psp405.GetRelayStatus())
                {
                    return ResultOk("Включение устройства прошло успешно");
                }
                else
                {
                    return ResultError("Не удалось включить устройство");
                }
                */
                case DeviceCommands.PowerOff:
                    Psp405.TurnOff();
                    Psp405.SetVoltage(0.0);
                    Thread.Sleep(delay);
                    return ResultOk("Отключение устройства прошло успешно");
                    /*
                    if (Psp405.GetRelayStatus())
                    {
                        return ResultOk("Отключение устройства прошло успешно");
                    }
                    else
                    {
                        return ResultError("Не удалось отключить устройство");
                    }
                    */
                case DeviceCommands.SetCurrentLimit:
                    var currentLimit = double.Parse(deviceData.Argument, CultureInfo.InvariantCulture);
                    Psp405.SetCurrentLimit(currentLimit);
                    Thread.Sleep(delay);
                    var actualCurrentLimit = Psp405.GetCurrentLimit();
                    if (Math.Abs(currentLimit - actualCurrentLimit) <= 0.1 * Math.Abs(currentLimit))
                    {
                        return ResultOk($"Установка предела по току {GetValueUnitPair(actualCurrentLimit, UnitType.Current)} прошла успешно");
                    }
                    else
                    {
                        return ResultError($"ОШИБКА: установлен неверный предел по току: {GetValueUnitPair(actualCurrentLimit, UnitType.Current)}, необходим {GetValueUnitPair(currentLimit, UnitType.Current)}");
                    }
                default:
                    return ResultError($"Неизвестная команда {deviceData.Command}");
            }
        }
    }
}