using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCA.DeviceDrivers;

namespace UCA.Devices
{
    class ATH8030_device : IDeviceInterface
    {
        readonly ATH_8030 ath8030;  

        public ATH8030_device (string portName)
        {
            this.ath8030 = new ATH_8030(portName);
        }

        public override DeviceResult DoCommand(DeviceData deviceData)
        {
            switch (deviceData.Command)
            {
                case DeviceCommands.OnOff:
                    var resultOK = ath8030.OnOff(float.Parse(deviceData.Argument));
                    if (resultOK)
                        return DeviceResult.ResultOk($"Снятие/подключение входного сигнала с {deviceData.DeviceName} прошло успешно");
                    else
                        return DeviceResult.ResultError($"ОШИБКА: Снятие/подключение входного сигнала с {deviceData.DeviceName} завершилось с ошибкой");
                case DeviceCommands.SetCurrent:
                    resultOK = ath8030.SetCurrent(float.Parse(deviceData.Argument));
                    if (resultOK)
                        return DeviceResult.ResultOk($"Установка контролируемого сигнала {deviceData.Argument} А прошла успешно");
                    else
                        return DeviceResult.ResultError($"ОШИБКА: Установка контролируемого сигнала {deviceData.Argument} А завершилась с ошибкой");
                default:
                    return DeviceResult.ResultError($"Неизвестная команда {deviceData.Command}");
            }
        }
    }
}
