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
    public class PST3201_device : IDeviceInterface
    {
        PST_3201 pst3201;
        public PST3201_device (SerialPort serialPort)
        {
            this.pst3201 = new PST_3201(serialPort);
        }

        public DeviceResult DoCommand (DeviceData deviceData)
        {
            switch (deviceData.Command)
            {
                case DeviceCommands.SetVoltage:
                    var expectedVoltage = Double.Parse(deviceData.Argument);
                    var actualVoltage = pst3201.SetVoltage(expectedVoltage, 1);
                    if (Math.Abs(expectedVoltage - actualVoltage) < 0.01)
                        return ResultOk($"Напряжение на {deviceData.DeviceName} успешно установлено");
                    else
                        return ResultError($"ОШИБКА: установлено напряжение {actualVoltage}, ожидалось {expectedVoltage}");
                case DeviceCommands.SetCurrent:
                    var expectedCurrent = Double.Parse(deviceData.Argument);
                    var actualCurrent = pst3201.SetCurrent(expectedCurrent, 1);
                    if (Math.Abs(expectedCurrent - actualCurrent) < 0.01)
                        return ResultOk($"Напряжение на {deviceData.DeviceName} успешно установлено");
                    else
                        return ResultError($"ОШИБКА: установлено напряжение {actualCurrent}, ожидалось {expectedCurrent}");
                case DeviceCommands.ChangeOutputStatus:
                    var status = deviceData.Argument;
                    bool expectedStatus = status == "1";
                    var actualStatus = pst3201.ChangeOutputState(status);
                    if (actualStatus == expectedStatus)
                        return ResultOk($"Состояние выхода {deviceData.DeviceName} успешно установлено в {deviceData.Argument}");
                    else
                        return ResultError($"ОШИБКА: в процессе изменения состояния выхода {deviceData.DeviceName} в {deviceData.Argument} произошла ошибка");
                default:
                    return ResultError($"Неизвестная команда {deviceData.Command}");
            }
        }
    }
}
