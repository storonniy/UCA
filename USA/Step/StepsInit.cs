using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCA;
using System.IO;
using System.IO.Ports;
using UCA.Devices;

namespace UCA.Steps
{
    class StepParser
    {
        private readonly DeviceInit deviceHandler;
        private readonly Step step;
        public StepParser(DeviceInit deviceHandler, Step step)
        {
            this.deviceHandler = deviceHandler;
            this.step = step;
        }

        public DeviceResult DoStep()
        {
            DeviceData deviceData = new DeviceData();
            deviceData.Argument = step.Argument;
            deviceData.AdditionalArg = step.AdditionalArg;
            deviceData.Channel = step.Channel;
            deviceData.LowerLimit = step.LowerLimit;
            deviceData.UpperLimit = step.UpperLimit;
            deviceData.ShowStep = step.ShowStep;
            try
            {
                deviceData.Command = (DeviceCommands)Enum.Parse(typeof(DeviceCommands), step.Command);
            }
            catch (ArgumentException)
            {
                return new DeviceResult { State = DeviceStatus.ERROR, Description = $"Команда не найдена: {step.Command}" };
            }
            try
            {
                deviceData.DeviceName = (DeviceNames)Enum.Parse(typeof(DeviceNames), step.Device);
            }
            catch (ArgumentException)
            {
                return new DeviceResult { State = DeviceStatus.ERROR, Description = $"Тип устройства не найден: {step.Device }" };
            }
            if (deviceHandler == null)
            {
                return DeviceResult.ResultError($"Устройство {deviceData.DeviceName} не подключено");
            }
            else
                return deviceHandler.ProcessDevice(deviceData);
        }
    }
}
