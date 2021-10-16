using System;
using System.IO;
using System.IO.Ports;
using UCA.DeviceDrivers;

namespace UCA.Devices
{
    class GDM78261_device : IDeviceInterface
    {
        readonly GDM78261 gdm78261;

        public GDM78261_device(SerialPort serialPort)
        {
            this.gdm78261 = new GDM78261(serialPort);
        }

        public DeviceResult DoCommand(DeviceData deviceData)
        {
            switch (deviceData.Command)
            {
                case DeviceCommands.GetVoltage:
                    var expectedVoltage = double.Parse(deviceData.ExpectedValue);
                    var actualVoltage = gdm78261.MeasureVoltageDC();
                    if (Math.Abs(expectedVoltage - actualVoltage) < 0.01)
                    {
                        return DeviceResult.ResultOk($"Измерение напряжения {actualVoltage} прошло успешно");
                    }
                    else
                    {
                        return DeviceResult.ResultError($"ОШИБКА: Измерено напряжение {actualVoltage}, ожидалось {expectedVoltage}");
                    }
                case DeviceCommands.GetCurrent:
                    var expectedCurrent = double.Parse(deviceData.ExpectedValue);
                    var actualCurrent = gdm78261.MeasureCurrentDC();
                    if (Math.Abs(expectedCurrent - actualCurrent) < 0.01)
                    {
                        return DeviceResult.ResultOk($"Измерение напряжения {actualCurrent.ToString()} прошло успешно");
                    }
                    else
                    {
                        return DeviceResult.ResultError($"ОШИБКА: Измерено напряжение {actualCurrent}, ожидалось {expectedCurrent}");
                    }
                default:
                    return DeviceResult.ResultError($"Неизвестная команда {deviceData.Command}");
            }
        }
    }
}