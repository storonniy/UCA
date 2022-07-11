using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using UCA.DeviceDrivers;
using UCA.Steps;
using UPD.Device;
using static UCA.Auxiliary.UnitValuePair;

namespace UCA.Devices
{
    class GDM78261_device : IDeviceInterface
    {
        private readonly GDM78261 gdm78261;

        public GDM78261_device(SerialPort serialPort)
        {
            gdm78261 = new GDM78261(serialPort);
        }

        public override DeviceResult DoCommand(Step step)
        {
            switch (step.Command)
            {
                case DeviceCommands.GetVoltageDC:
                    {
                        var voltage = gdm78261.MeasureVoltageDC();
/*                        if (deviceData.Argument != "-")
                        {
                            //var key = double.Parse(deviceData.Argument, NumberStyles.Float);
                            //AddCoefficientData(deviceData.Channel, key, voltage);
                        }*/
                        return GetResult("Измерено", step, UnitType.Voltage, voltage);
                    }
                case DeviceCommands.GetVoltageAC:
                    {
                        var voltage = gdm78261.MeasureVoltageAC();
                        return GetResult("Измерено", step, UnitType.Voltage, voltage);
                    }
                case DeviceCommands.GetVoltageACAndSave:
                    {
                        var voltage = gdm78261.MeasureVoltageAC();
                        var key = step.Argument;
                        AddValues(key, voltage);
                        return GetResult("Измерено", step, UnitType.Voltage, voltage);
                    }
                case DeviceCommands.GetVoltageDCAndSave:
                    {
                        var voltage = gdm78261.MeasureVoltageDC();
                        var key = step.Argument;
                        AddValues(key, voltage);
                        return GetResult("Измерено", step, UnitType.Voltage, voltage);
                    }
                case DeviceCommands.GetCurrentAndSave:
                    {
                        var current = gdm78261.MeasureCurrentDC();
                        var key = step.Argument;
                        AddValues(key, current);
                        return GetResult("Измерено", step, UnitType.Current, current);
                    }
                case DeviceCommands.GetCurrent:
                    {
                        var current = gdm78261.MeasureCurrentDC();
/*                        if (deviceData.Argument != "-")
                        {
                            var keyCurrent = double.Parse(deviceData.Argument, NumberStyles.Float);
                            AddCoefficientData(deviceData.Channel, keyCurrent, current);
                        }*/
                        return GetResult("Измерено", step, UnitType.Current, current);
                    }
                case DeviceCommands.SetMeasurementToCurrent:
                    var currentRange = double.Parse(step.Argument, CultureInfo.InvariantCulture);
                    gdm78261.SetMeasurementToCurrentDC(currentRange);
                    return DeviceResult.ResultOk($"{step.DeviceName} переведен в режим измерения тока");
                case DeviceCommands.SetMeasurementToVoltageAC:
                    gdm78261.SetMeasurementToVoltageAC();
                    return DeviceResult.ResultOk($"{step.DeviceName} переключён в режим измерения переменного напряжения");
                case DeviceCommands.SetMeasurementToVoltageDC:
                    gdm78261.SetMeasurementToVoltageDC();
                    return DeviceResult.ResultOk($"{step.DeviceName} переключён в режим измерения постоянного напряжения");
                default:
                    return DeviceResult.ResultError($"Неизвестная команда {step.Command}");
            }
        }
    }
}