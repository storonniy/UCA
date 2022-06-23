using System;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Threading;
using UCA.DeviceDrivers;
using UPD.Device;
using static UCA.Auxiliary.UnitValuePair;


namespace UCA.Devices
{
    class Keithley2401_device : Source
    {
        readonly Keithley2401 keithley2401;

        public Keithley2401_device(SerialPort serialPort)
        {
            serialPort.NewLine = "\r";
            keithley2401 = new Keithley2401(serialPort);
        }

        public override DeviceResult DoCommand(DeviceData deviceData)
        {
            switch (deviceData.Command)
            {
                case DeviceCommands.SetVoltage:
                    var actualVoltage = SetVoltage(deviceData);
                    return GetResult(deviceData, UnitType.Voltage, actualVoltage);
                case DeviceCommands.GetVoltage:
                    {
                        var voltage = GetVoltage();
                        return GetResult(deviceData, UnitType.Voltage, voltage);
                    }
                case DeviceCommands.GetCurrent:
                    {
                        var current = GetCurrent();
                        return GetResult(deviceData, UnitType.Current, current);
                    }
                case DeviceCommands.PowerOn:
                    return IDeviceInterface.PowerOn(deviceData, keithley2401.PowerOn);
                case DeviceCommands.PowerOff:
                    return IDeviceInterface.PowerOff(deviceData, keithley2401.PowerOff);
                default:
                    return DeviceResult.ResultError($"Неизвестная команда {deviceData.Command}");
            }
        }

        public double GetCurrent()
        {
            return keithley2401.GetCurrent();
        }

        public double GetVoltage()
        {
            return keithley2401.GetVoltage();
        }

        public override void PowerOff()
        {
            throw new NotImplementedException();
        }

        public override void PowerOn()
        {
            throw new NotImplementedException();
        }

        public override double SetCurrent(DeviceData deviceData)
        {
            throw new NotImplementedException();
        }

        public override double SetCurrentLimit(DeviceData deviceData)
        {
            keithley2401.SetCurrentRange(0, (int)deviceData.UpperLimit);
            throw new NotImplementedException();
        }

        public override double SetVoltage(DeviceData deviceData)
        {

            var voltage = double.Parse(deviceData.Argument.Replace(".", ","));
            return keithley2401.SetVoltage(voltage);
        }
    }
}