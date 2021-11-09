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
                    var lowerLimit = double.Parse(deviceData.LowerLimit, CultureInfo.InvariantCulture);
                    var upperLimit = double.Parse(deviceData.UpperLimit, CultureInfo.InvariantCulture);
                    bool lessThan = deviceData.ExpectedValue.Contains("<");
                    deviceData.ExpectedValue = deviceData.ExpectedValue.Replace("<", "");
                    var expectedVoltage = double.Parse(deviceData.ExpectedValue, CultureInfo.InvariantCulture);
                    var actualVoltage = gdm78261.MeasureVoltageDC();
                    var tmp = double.Parse(deviceData.Argument, CultureInfo.InvariantCulture);
                    AddCoefficientData(deviceData.Channel, tmp, actualVoltage);
                    var result = $"Измерено напряжение {GetValueUnitPair(actualVoltage, UnitType.Voltage)} \t Нижний предел: {GetValueUnitPair(lowerLimit, UnitType.Voltage)}\t Верхний предел {GetValueUnitPair(upperLimit, UnitType.Voltage)}";
                    if (lessThan)
                    {
                        if (expectedVoltage <= actualVoltage)
                            return DeviceResult.ResultOk(result);
                        else
                            return DeviceResult.ResultError($"Ошибка: {result}");
                    }
                    else
                    {
                        if (Math.Abs(actualVoltage) <= Math.Abs(upperLimit))
                        {
                            return DeviceResult.ResultOk(result);
                        }
                        else
                        {
                            return DeviceResult.ResultError($"Ошибка: {result}");
                        }
                    }
                case DeviceCommands.GetCurrent:
                    var lowerLimitCurrent = double.Parse(deviceData.LowerLimit, CultureInfo.InvariantCulture);
                    var upperLimitCurrent = double.Parse(deviceData.UpperLimit, CultureInfo.InvariantCulture);
                    var expectedCurrent = double.Parse(deviceData.ExpectedValue.Replace(",", "."), CultureInfo.InvariantCulture);
                    var actualCurrent = gdm78261.MeasureCurrentDC();
                    var tmp1 = double.Parse(deviceData.Argument, CultureInfo.InvariantCulture);
                    var resultCurrent = $"Измерен ток {GetValueUnitPair(actualCurrent, UnitType.Current)} \t Нижний предел: {GetValueUnitPair(lowerLimitCurrent, UnitType.Current)}\t Верхний предел {GetValueUnitPair(upperLimitCurrent, UnitType.Current)}";
                    AddCoefficientData(deviceData.Channel, tmp1, actualCurrent);
                    if (Math.Abs(actualCurrent) <= Math.Abs(upperLimitCurrent))
                    {
                        return DeviceResult.ResultOk(resultCurrent);
                    }
                    else
                    {
                        return DeviceResult.ResultError($"Ошибка {resultCurrent}");
                    }
                case DeviceCommands.SetMeasurementToCurrent:
                    var currentRange = double.Parse(deviceData.Argument, CultureInfo.InvariantCulture);
                    gdm78261.SetMeasurementToCurrentDC(currentRange);
                    return DeviceResult.ResultOk($"{deviceData.DeviceName} переведен в режим измерения тока");
                default:
                    return DeviceResult.ResultError($"Неизвестная команда {deviceData.Command}");
            }
        }
    }
}