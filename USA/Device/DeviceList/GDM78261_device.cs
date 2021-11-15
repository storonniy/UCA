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
                    var lowerLimit = deviceData.LowerLimit;
                    var upperLimit = deviceData.UpperLimit;
                    deviceData.ExpectedValue = deviceData.ExpectedValue.Replace("<", "");
                    var actualVoltage = gdm78261.MeasureVoltageDC();
                    var tmp = double.Parse(deviceData.Argument, CultureInfo.InvariantCulture);
                    AddCoefficientData(deviceData.Channel, tmp, actualVoltage);
                    var result = $"Измерено напряжение {GetValueUnitPair(actualVoltage, UnitType.Voltage)} \tНижний предел: {GetValueUnitPair(lowerLimit, UnitType.Voltage)}\t Верхний предел {GetValueUnitPair(upperLimit, UnitType.Voltage)}";
                    if (Math.Abs(actualVoltage) >= Math.Abs(lowerLimit) && Math.Abs(actualVoltage) <= Math.Abs(upperLimit))
                    {
                        return DeviceResult.ResultOk(result);
                    }
                    else
                    {
                        return DeviceResult.ResultError("Ошибка: " + result);
                    }
                case DeviceCommands.GetCurrent:
                    var lowerLimitCurrent = deviceData.LowerLimit;
                    var upperLimitCurrent = deviceData.UpperLimit;
                    var actualCurrent = gdm78261.MeasureCurrentDC();
                    var tmp1 = double.Parse(deviceData.Argument, CultureInfo.InvariantCulture);
                    var resultCurrent = $"Измерен ток {GetValueUnitPair(actualCurrent, UnitType.Current)} \tНижний предел: {GetValueUnitPair(lowerLimitCurrent, UnitType.Current)}\t Верхний предел {GetValueUnitPair(upperLimitCurrent, UnitType.Current)}";
                    AddCoefficientData(deviceData.Channel, tmp1, actualCurrent);
                    if (Math.Abs(actualCurrent) <= Math.Abs(upperLimitCurrent))
                    {
                        return DeviceResult.ResultOk(resultCurrent);
                    }
                    else
                    {
                        return DeviceResult.ResultError($"Ошибка: {resultCurrent}");
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
                default:
                    return DeviceResult.ResultError($"Неизвестная команда {deviceData.Command}");
            }
        }
    }
}