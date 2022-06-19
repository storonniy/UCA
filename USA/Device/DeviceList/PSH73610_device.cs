using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCA.DeviceDrivers;
using System.IO.Ports;
using static UCA.Devices.DeviceResult;
using System.Globalization;
using static UCA.Auxiliary.UnitValuePair;
using UPD.Device;

namespace UCA.Devices
{
    class PSH73610_device : Source
    {
        readonly PSH73610 psh73610;

        public PSH73610_device(SerialPort serialPort)
        {
            this.psh73610 = new PSH73610(serialPort);
        }

        public override DeviceResult DoCommand(DeviceData deviceData)
        {
            switch (deviceData.Command)
            {
                case DeviceCommands.SetVoltage:
                    return IDeviceInterface.SetVoltage(deviceData, psh73610.SetVoltage);
                    var voltage = SetVoltage(deviceData);
                    return GetResult(deviceData, UnitType.Voltage, voltage);
                case DeviceCommands.SetCurrent:
                    var current = SetCurrent(deviceData);
                    return GetResult(deviceData, UnitType.Current, current);
                case DeviceCommands.PowerOff:
                    PowerOff();
                    return ResultOk($"Снят входной сигнал с {deviceData.DeviceName}");
                case DeviceCommands.PowerOn:
                    PowerOn();
                    return ResultOk($"Подан входной сигнал с {deviceData.DeviceName}");
                default:
                    return ResultError($"Неизвестная команда {deviceData.Command}");
            }
        }

        public override void PowerOff()
        {
            psh73610.ChangeOutputStatus(0);        }

        public override void PowerOn()
        {
            psh73610.ChangeOutputStatus(1);
        }

        public override double SetCurrent(DeviceData deviceData)
        {
            var current = double.Parse(deviceData.Argument, NumberStyles.Float);
            return psh73610.SetCurrentLimit(current);
        }

        public override double SetCurrentLimit(DeviceData deviceData)
        {
            throw new NotImplementedException();
        }

        public override double SetVoltage(DeviceData deviceData)
        {
            var voltage = Double.Parse(deviceData.Argument, NumberStyles.Float);
            return psh73610.SetVoltage(voltage);
        }
    }
}
