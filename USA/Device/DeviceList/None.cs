using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCA.Devices
{
    class None : IDeviceInterface
    {
        public override DeviceResult DoCommand(DeviceData deviceData)
        {
            switch (deviceData.Command)
            {
                case DeviceCommands.CalculateCoefficient:
                    return DeviceResult.ResultOk($"{deviceData.Command}");//($"Напиши метод {deviceData.Command}");
                case DeviceCommands.CalculateCoeff_UCA_T:
                    return DeviceResult.ResultError($"Напиши метод,  {deviceData.Command}");
                default:
                    return DeviceResult.ResultError($"Неизвестная команда {deviceData.Command}");
            }
        }
    }
}
