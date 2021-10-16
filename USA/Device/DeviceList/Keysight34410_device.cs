using System;
using System.IO;
using System.IO.Ports;
using UCA.DeviceDrivers;
namespace UCA.Devices
{
    class Keysight34410_device : IDeviceInterface
    {
        readonly Keysight34410 keysight34410;

        public Keysight34410_device(SerialPort serialPort)
        {
            this.keysight34410 = new Keysight34410(serialPort);
        }

        public DeviceResult DoCommand(DeviceData deviceData)
        {
            switch (deviceData.Command)
            {
                case DeviceCommands.GetCurrent:
                    var expectedCurrent = double.Parse(deviceData.ExpectedValue);
                    var actualCurrent = keysight34410.MeasureCurrentDC();
                    if (Math.Abs(expectedCurrent - actualCurrent) < 0.01)
                    {
                        return DeviceResult.ResultOk($"Измерение напряжения {actualCurrent} прошло успешно");
                    }
                    else
                    {
                        return DeviceResult.ResultError("Измерено напряжение {actualCurrent}, ожидалось {expectedCurrent}");
                    }
                case DeviceCommands.GetVoltage:
                    var expectedVoltage = double.Parse(deviceData.ExpectedValue);
                    var actualVoltage = keysight34410.MeasureVoltageDC();
                    if (Math.Abs(expectedVoltage - actualVoltage) < 0.01)
                    {
                        return DeviceResult.ResultOk($"Измерение напряжения {actualVoltage.ToString()} прошло успешно");
                    }
                    else
                    {
                        return DeviceResult.ResultError($"Измерено напряжение {actualVoltage}, ожидалось {expectedVoltage}");
                    }

                    break;
                default:
                    return DeviceResult.ResultError($"Неизвестная команда {deviceData.Command}");
            }
        }
    }
}