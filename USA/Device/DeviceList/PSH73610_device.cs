using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using static Checker.Devices.DeviceResult;
using System.Globalization;
using Checker.DeviceDrivers;
using Checker.Steps;
using static Checker.Auxiliary.UnitValuePair;
using Checker.Device;
using Checker.DeviceInterface;

namespace Checker.Devices
{
    class PSH73610_device : IDeviceInterface
    {
        readonly Psh73610 psh73610;

        public PSH73610_device(SerialPort serialPort)
        {
            this.psh73610 = new Psh73610(serialPort);
        }

        public override DeviceResult DoCommand(Step step)
        {
            switch (step.Command)
            {
                case DeviceCommands.SetVoltage:
                    return SetVoltage(step, psh73610.SetVoltage);
                case DeviceCommands.SetCurrentLimit:
                    return SetCurrentLimit(step, psh73610.SetCurrentLimit);
                case DeviceCommands.PowerOff:
                    return PowerOff(step, psh73610.PowerOff);
                case DeviceCommands.PowerOn:
                    return PowerOn(step, psh73610.PowerOn);
                default:
                    return ResultError($"Неизвестная команда {step.Command}");
            }
        }
    }
}
