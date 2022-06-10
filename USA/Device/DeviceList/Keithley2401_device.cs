using System;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using UCA.DeviceDrivers;
using static UCA.Auxiliary.UnitValuePair;


namespace UCA.Devices
{
    class Keithley2401_device : IDeviceInterface
    {
        readonly Keithley2401 keithley2401;

        public Keithley2401_device(SerialPort serialPort)
        {
            this.keithley2401 = new Keithley2401(serialPort);
        }

        public override DeviceResult DoCommand(DeviceData deviceData)
        {
            switch (deviceData.Command)
            {
                case DeviceCommands.GetVoltage:
                    {
                        var actualVoltage = keithley2401.MeasureVoltage();
                        var tmp = double.Parse(deviceData.Argument, NumberStyles.Float);
                        var result = $"Измерено напряжение {GetValueUnitPair(actualVoltage, UnitType.Voltage)} \tНижний предел: {GetValueUnitPair(deviceData.LowerLimit, UnitType.Voltage)}\t Верхний предел {GetValueUnitPair(deviceData.UpperLimit, UnitType.Voltage)}";
                        if (actualVoltage >= deviceData.LowerLimit && actualVoltage <= deviceData.UpperLimit)
                        {
                            return DeviceResult.ResultOk(result);
                        }
                        else
                        {
                            return DeviceResult.ResultError("Ошибка: " + result);
                        }
                    }
                case DeviceCommands.GetCurrent:
                    {
                        var actualCurrent = keithley2401.MeasureCurrent();
                        var tmp = double.Parse(deviceData.Argument, NumberStyles.Float);
                        var resultCurrent = $"Измерен ток {GetValueUnitPair(actualCurrent, UnitType.Current)} \tНижний предел: {GetValueUnitPair(deviceData.LowerLimit, UnitType.Current)}\t Верхний предел {GetValueUnitPair(deviceData.UpperLimit, UnitType.Current)}";
                        if (actualCurrent >= deviceData.LowerLimit && actualCurrent <= deviceData.UpperLimit)
                        {
                            return DeviceResult.ResultOk(resultCurrent);
                        }
                        else
                        {
                            return DeviceResult.ResultError($"Ошибка: {resultCurrent}");
                        }
                    }
                case DeviceCommands.PowerOn:
                    var actualStatus = keithley2401.PowerOn();
                    if (actualStatus)
                    {
                        return DeviceResult.ResultOk($"Подача входного сигнала с {deviceData.DeviceName}");
                    }
                    else
                    {
                        return DeviceResult.ResultError($"Ошибка: не удалось подать входной сигнал с {deviceData.DeviceName}");
                    }
                case DeviceCommands.PowerOff:
                    var actualPowerStatus = keithley2401.PowerOff();
                    if (!actualPowerStatus)
                    {
                        return DeviceResult.ResultOk($"Снятие входного сигнала с {deviceData.DeviceName}");
                    }
                    else
                    {
                        return DeviceResult.ResultError($"ОШИБКА: не удалось отключить входной сигнал с {deviceData.DeviceName}");
                    }
                default:
                    return DeviceResult.ResultError($"Неизвестная команда {deviceData.Command}");
            }
        }
    }
}