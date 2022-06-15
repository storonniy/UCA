using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCA.DeviceDrivers;
using System.IO.Ports;
using static UCA.Devices.DeviceResult;
using static UCA.Auxiliary.UnitValuePair;
using System.Globalization;
using UPD.Device;

namespace UCA.Devices
{
    public class PST3201_device : Source
    {
        PST_3201 pst3201;


        public PST3201_device (SerialPort serialPort)
        {
            this.pst3201 = new PST_3201(serialPort);
        }

        public override DeviceResult DoCommand (DeviceData deviceData)
        {
            switch (deviceData.Command)
            {
                case DeviceCommands.SetVoltage:
                    var actualVoltage = SetVoltage(deviceData);
                    return GetResult(message, deviceData, UnitType.Voltage, actualVoltage);
                case DeviceCommands.SetCurrent:
                    var actualCurrent = SetCurrent(deviceData);
                    return GetResult(message, deviceData, UnitType.Current, actualCurrent);
                case DeviceCommands.PowerOff:
                    PowerOff();
                    return ResultOk($"Подан входной сигнал с {deviceData.DeviceName}");
                case DeviceCommands.PowerOn:
                    PowerOn();
                    return ResultOk($"Снят входной сигнал с {deviceData.DeviceName}");
                default:
                    return ResultError($"Неизвестная команда {deviceData.Command}");
            }
        }

        public override void PowerOff()
        {
            pst3201.ChangeOutputState("0");
        }

        public override void PowerOn()
        {
            pst3201.ChangeOutputState("1");
        }

        public override double SetCurrent(DeviceData deviceData)
        {
            var current = Double.Parse(deviceData.Argument);
            return pst3201.SetCurrent(current, 1);
        }

        public override double SetCurrentLimit(DeviceData deviceData)
        {
            throw new NotImplementedException();
        }

        public override double SetVoltage(DeviceData deviceData)
        {
            var voltage = Double.Parse(deviceData.Argument);
            return pst3201.SetVoltage(voltage, 1);
        }
    }
}
