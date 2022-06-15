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
        private int delay = 1000;

        public Keithley2401_device(SerialPort serialPort)
        {
            this.keithley2401 = new Keithley2401(serialPort);
        }

        public override DeviceResult DoCommand(DeviceData deviceData)
        {
            switch (deviceData.Command)
            {
                case DeviceCommands.SetVoltage:
                    var actualVoltage = SetVoltage(deviceData);
                    return GetResult(message, deviceData, UnitType.Voltage, actualVoltage);
                case DeviceCommands.GetVoltage:
                    {
                        var voltage = GetVoltage();
                        return GetResult(message, deviceData, UnitType.Voltage, voltage);
                    }
                case DeviceCommands.GetCurrent:
                    {
                        var current = GetCurrent();
                        return GetResult(message, deviceData, UnitType.Current, current);
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
            throw new NotImplementedException();
        }

        public override double SetVoltage(DeviceData deviceData)
        {
            keithley2401.SelectFixedSourcingModeVoltage();
            keithley2401.SetVoltageLimit((int)deviceData.UpperLimit);
            Thread.Sleep(delay);
            var voltage = double.Parse(deviceData.Argument);
            keithley2401.SetVoltage(voltage);
            Thread.Sleep(delay);
            return keithley2401.GetOutputVoltage();
        }
    }
}