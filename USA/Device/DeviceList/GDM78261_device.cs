using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using UCA.DeviceDrivers;
using UPD.Device;
using static UCA.Auxiliary.UnitValuePair;

namespace UCA.Devices
{
    class GDM78261_device : Multimeter
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
                        var voltage = GetVoltage();
                        var key = double.Parse(deviceData.Argument, NumberStyles.Float);
                        AddCoefficientData(deviceData.Channel, key, voltage);
                        return GetResult(message, deviceData, UnitType.Voltage, voltage);
                    }
                case DeviceCommands.GetVoltageAndSave:
                    {
                        var voltage = GetVoltage();
                        var key = deviceData.Argument;
                        AddValues(key, voltage);
                        return GetResult(message, deviceData, UnitType.Voltage, voltage);
                    }
                case DeviceCommands.GetCurrent:
                    {
                        var current = GetCurrent();
                        var keyCurrent = double.Parse(deviceData.Argument, NumberStyles.Float);
                        AddCoefficientData(deviceData.Channel, keyCurrent, current);
                        return GetResult(message, deviceData, UnitType.Current, current);
                    }
                case DeviceCommands.SetMeasurementToCurrent:
                    var currentRange = double.Parse(deviceData.Argument, CultureInfo.InvariantCulture);
                    gdm78261.SetMeasurementToCurrentDC(currentRange);
                    return DeviceResult.ResultOk($"{deviceData.DeviceName} переведен в режим измерения тока");
                case DeviceCommands.GetVoltageRipple:
                    var ripple = gdm78261.MeasureVoltageAC();
                    return GetResult(message, deviceData, UnitType.Voltage, ripple);
                default:
                    return DeviceResult.ResultError($"Неизвестная команда {deviceData.Command}");
            }
        }

        public override double GetCurrent()
        {
            return gdm78261.MeasureCurrentDC();
        }

        public override double GetVoltage()
        {
            return gdm78261.MeasureVoltageDC();
        }
    }
}