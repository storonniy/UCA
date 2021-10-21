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

namespace UCA.Devices
{
    class PSH73610_device : IDeviceInterface
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
                    var expectedVoltage = float.Parse(deviceData.Argument, CultureInfo.InvariantCulture);             
                    var actualVoltage = psh73610.SetVoltage(expectedVoltage);
                    if (Math.Abs(expectedVoltage - actualVoltage) <= 0.1 * Math.Abs(expectedVoltage))
                    {
                        return ResultOk($"Установка напряжения {GetValueUnitPair(expectedVoltage, UnitType.Voltage)} прошла успешно");
                    }
                    else
                    {
                        return ResultError($"ОШИБКА: Установлено напряжение {GetValueUnitPair(actualVoltage, UnitType.Current)}, ожидалось {GetValueUnitPair(expectedVoltage, UnitType.Current)}");
                    }
                case DeviceCommands.SetCurrent:
                    var expectedCurrent = float.Parse(deviceData.Argument);
                    var actualCurrent = psh73610.SetCurrent(expectedCurrent);
                    if (Math.Abs(expectedCurrent - actualCurrent) <= 0.1 * Math.Abs(expectedCurrent))
                    {
                        return ResultOk($"Установка тока {GetValueUnitPair(actualCurrent, UnitType.Current)} прошла успешно");
                    }
                    else
                    {
                        return ResultError($"ОШИБКА: Установлен ток {GetValueUnitPair(actualCurrent, UnitType.Current)}, ожидался {GetValueUnitPair(expectedCurrent, UnitType.Current)}");
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
