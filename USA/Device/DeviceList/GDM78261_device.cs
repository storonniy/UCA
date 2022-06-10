using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using UCA.DeviceDrivers;
using static UCA.Auxiliary.UnitValuePair;

namespace UCA.Devices
{
    class GDM78261_device : IDeviceInterface
    {
        readonly GDM78261 gdm78261;

        public GDM78261_device(SerialPort serialPort)
        {
            gdm78261 = new GDM78261(serialPort);
        }

        public override DeviceResult DoCommand(DeviceData deviceData)
        {
            switch (deviceData.Command)
            {
                case DeviceCommands.GetVoltage:
                    {
                        var actualVoltage = gdm78261.MeasureVoltageDC();
                        var tmp = double.Parse(deviceData.Argument, NumberStyles.Float);
                        AddCoefficientData(deviceData.Channel, tmp, actualVoltage);
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
                        var actualCurrent = gdm78261.MeasureCurrentDC();
                        var tmp1 = double.Parse(deviceData.Argument, NumberStyles.Float);
                        var resultCurrent = $"Измерен ток {GetValueUnitPair(actualCurrent, UnitType.Current)} \tНижний предел: {GetValueUnitPair(deviceData.LowerLimit, UnitType.Current)}\t Верхний предел {GetValueUnitPair(deviceData.UpperLimit, UnitType.Current)}";
                        AddCoefficientData(deviceData.Channel, tmp1, actualCurrent);
                        if (actualCurrent >= deviceData.LowerLimit && actualCurrent <= deviceData.UpperLimit)
                        {
                            return DeviceResult.ResultOk(resultCurrent);
                        }
                        else
                        {
                            return DeviceResult.ResultError($"Ошибка: {resultCurrent}");
                        }
                    }
                case DeviceCommands.SetMeasurementToCurrent:
                    var currentRange = double.Parse(deviceData.Argument, CultureInfo.InvariantCulture);
                    gdm78261.SetMeasurementToCurrentDC(currentRange);
                    return DeviceResult.ResultOk($"{deviceData.DeviceName} переведен в режим измерения тока");
                case DeviceCommands.GetVoltageRipple:
                    var lowerLimitRipple = deviceData.LowerLimit;
                    var upperLimitRipple = deviceData.UpperLimit;
                    var actualRipple = gdm78261.MeasureVoltageAC();
                    var resultRipple = $"Измерена пульсация напряжения {GetValueUnitPair(actualRipple, UnitType.Voltage)} \t Нижний предел: {GetValueUnitPair(lowerLimitRipple, UnitType.Voltage)}\t Верхний предел {GetValueUnitPair(upperLimitRipple, UnitType.Voltage)}";
                    if (Math.Abs(actualRipple) >= Math.Abs(lowerLimitRipple) && Math.Abs(actualRipple) <= Math.Abs(upperLimitRipple))
                    {
                        return DeviceResult.ResultOk(resultRipple);
                    }
                    else
                    {
                        return DeviceResult.ResultError("Ошибка: " + resultRipple);
                    }
                case DeviceCommands.GetVoltageAndSave:
                    {
                        var voltage = gdm78261.MeasureVoltageDC();
                        var key = deviceData.Argument;
                        AddValues(key, voltage);
                        var result = $"Измерено напряжение {GetValueUnitPair(voltage, UnitType.Voltage)}";
                        if (voltage >= deviceData.LowerLimit && voltage <= deviceData.UpperLimit)
                        {
                            return DeviceResult.ResultOk(result);
                        }
                        else
                        {
                            return DeviceResult.ResultError("Ошибка: " + result);
                        }
                    }
                default:
                    return DeviceResult.ResultError($"Неизвестная команда {deviceData.Command}");
            }
        }
    }
}