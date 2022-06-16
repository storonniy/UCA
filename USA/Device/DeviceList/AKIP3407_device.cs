using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using UCA.DeviceDrivers;
using System.Globalization;
using static UCA.Auxiliary.UnitValuePair;
using UPD.Device;

namespace UCA.Devices
{
    class AKIP3407_device : Source
    {
        int delay = 1000;
        private AKIP_3407 akip3407;
        public AKIP3407_device (SerialPort serialPort)
        {
            akip3407 = new AKIP_3407(serialPort);
        }
        public override DeviceResult DoCommand (DeviceData deviceData)
        {
            switch (deviceData.Command)
            {
                case DeviceCommands.SetVoltage:
                    var voltage = SetVoltage(deviceData);
                    return GetResult(deviceData, UnitType.Voltage, voltage);
                case DeviceCommands.SetFrequency:
                    var actualFrequency = akip3407.SetFrequency(deviceData.Argument);
                    return GetResult(deviceData, UnitType.Frequency, actualFrequency);
                case DeviceCommands.PowerOn:
                    PowerOn();
                    return DeviceResult.ResultOk($"Подан входной сигнал с {deviceData.DeviceName}");
                case DeviceCommands.PowerOff:
                    PowerOff();
                    return DeviceResult.ResultOk($"Снят входной сигнал с {deviceData.DeviceName}");
                default:
                    return DeviceResult.ResultError($"Неизвестная команда {deviceData.Command}");
            }
        }

        public override void PowerOff()
        {
            akip3407.PowerOff();        }

        public override void PowerOn()
        {
            akip3407.PowerOn();
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
            var voltage = double.Parse(deviceData.Argument, CultureInfo.InvariantCulture);
            return akip3407.SetVoltage(voltage);
        }
    }
}
