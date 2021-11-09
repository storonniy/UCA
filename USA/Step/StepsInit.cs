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
            try
            {
                deviceData.Argument = step.Argument;
                deviceData.ExpectedValue = step.ExpectedValue;
                deviceData.Command = (DeviceCommands)Enum.Parse(typeof(DeviceCommands), step.Command);
                deviceData.Channel = step.Channel;
                deviceData.LowerLimit = step.LowerLimit;
                deviceData.UpperLimit = step.UpperLimit;
            }
            catch (ArgumentException)
            {
                return new DeviceResult { State = DeviceState.ERROR, Description = $"Команда не найдена: {step.Command}" };
            }
            try
            {
                deviceData.DeviceName = (DeviceNames)Enum.Parse(typeof(DeviceNames), step.Device);
            }
            catch (ArgumentException)
            {
                return new DeviceResult { State = DeviceState.ERROR, Description = $"Тип устройства не найден: {step.Device }" };
            }
            var deviceResult = deviceHandler.ProcessDevice(deviceData);
            return deviceResult;
        }
    }
}
