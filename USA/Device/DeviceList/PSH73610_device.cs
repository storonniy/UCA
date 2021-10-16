using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCA.DeviceDrivers;
using System.IO.Ports;
using static UCA.Devices.DeviceResult;

namespace UCA.Devices
{
    class PSH73610_device : IDeviceInterface
    {
        readonly PSH73610 psh73610;

        public PSH73610_device(SerialPort serialPort)
        {
            this.psh73610 = new PSH73610(serialPort);
        }

        public DeviceResult DoCommand(DeviceData deviceData)
        {
            switch (deviceData.Command)
            {
                case DeviceCommands.SetVoltage:
                    var expectedVoltage = float.Parse(deviceData.Argument);
                    
                    var actualVoltage = psh73610.SetVoltage(expectedVoltage);
                    if (Math.Abs(expectedVoltage - actualVoltage) < 0.01)
                    {
                        return ResultOk($"Установка напряжения {actualVoltage} прошла успешно");
                    }
                    else
                    {
                        return ResultError($"ОШИБКА: Установлено напряжение {actualVoltage}, ожидалось {expectedVoltage}");
                    }
                case DeviceCommands.SetCurrent:
                    var expectedCurrent = float.Parse(deviceData.Argument);
                    var actualCurrent = psh73610.SetCurrent(expectedCurrent);
                    if (Math.Abs(expectedCurrent - actualCurrent) < 0.01)
                    {
                        return ResultOk($"Установка тока {actualCurrent} прошла успешно");
                    }
                    else
                    {
                        return ResultError($"ОШИБКА: Установлен ток {actualCurrent}, ожидался {expectedCurrent}");
                    }
                case DeviceCommands.ChangeOutputStatus:
                    var expectedStatus = float.Parse(deviceData.Argument);
                    var actualStatus = psh73610.ChangeOutputStatus(expectedStatus);
                    if (expectedStatus == actualStatus)
                    {
                        return ResultOk($"Состояние выхода PSH_73610 успешно изменено на {actualStatus}");
                    }
                    else
                    {
                        return ResultError($"ОШИБКА: Ожидалось переключение состояния PSH_73610 в {expectedStatus}, фактическое {actualStatus}");
                    }
                default:
                    return ResultError($"Неизвестная команда {deviceData.Command}");
            }
        }

    }
}
