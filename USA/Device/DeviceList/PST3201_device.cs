using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using static Checker.Devices.DeviceResult;
using static Checker.Auxiliary.UnitValuePair;
using System.Globalization;
using Checker.DeviceDrivers;
using Checker.Steps;
using Checker.Device;
using Checker.DeviceInterface;

namespace Checker.Devices
{
    public class PST3201_device : IDeviceInterface
    {
        Pst3201 pst3201;


        public PST3201_device (SerialPort serialPort)
        {
            pst3201 = new Pst3201(serialPort);
        }

        public override DeviceResult DoCommand (Step step)
        {
            switch (step.Command)
            {
                case DeviceCommands.SetVoltage:
                    return SetVoltage(step, pst3201.SetVoltage);
                case DeviceCommands.SetCurrentLimit:
                    return SetCurrent(step, pst3201.SetCurrent);
                case DeviceCommands.PowerOff:
                    return PowerOff(step, pst3201.PowerOff);
                case DeviceCommands.PowerOn:
                    return PowerOn(step, pst3201.PowerOn);
                default:
                    return ResultError($"Неизвестная команда {step.Command}");
            }
        }
    }
}
