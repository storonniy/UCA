using System;
using System.Globalization;
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

        public override DeviceResult DoCommand(DeviceData deviceData)
        {
            switch (deviceData.Command)
            {
                case DeviceCommands.GetVoltage:
                    bool lessThan = deviceData.ExpectedValue.Contains("<");
                    deviceData.ExpectedValue = deviceData.ExpectedValue.Replace("<", "");
                    var expectedVoltage = double.Parse(deviceData.ExpectedValue, CultureInfo.InvariantCulture);
                    var actualVoltage = gdm78261.MeasureVoltageDC();
                    if (lessThan)
                    {
                        if (actualVoltage < expectedVoltage)
                            return DeviceResult.ResultOk($"Измерение напряжения {actualVoltage} прошло успешно");
                        else
                            return DeviceResult.ResultError($"ОШИБКА: Измерено напряжение {actualVoltage}, ожидалось {expectedVoltage}");
                    }
                    else
                    {
                        if (Math.Abs(expectedVoltage - actualVoltage) < 0.1 * Math.Abs(expectedVoltage))
                        {
                            return DeviceResult.ResultOk($"Измерение напряжения {actualVoltage} прошло успешно");
                        }
                        else
                        {
                            return DeviceResult.ResultError($"ОШИБКА: Измерено напряжение {actualVoltage}, ожидалось {expectedVoltage}");
                        }
                    }
                case DeviceCommands.GetCurrent:
                    var expectedCurrent = double.Parse(deviceData.ExpectedValue.Replace("E", "E").Replace(",", "."), CultureInfo.InvariantCulture);
                    var actualCurrent = gdm78261.MeasureCurrentDC();
                    if (Math.Abs(expectedCurrent - actualCurrent) < 0.1 * Math.Abs(expectedCurrent))
                    {
                        return DeviceResult.ResultOk($"Измерение тока {actualCurrent} прошло успешно");
                    }
                    else
                    {
                        return DeviceResult.ResultError($"ОШИБКА: Измерен ток {actualCurrent}, ожидалось значение {expectedCurrent}");
                    }
                case DeviceCommands.SetMeasurementToCurrent:
                    var currentRange = double.Parse(deviceData.Argument, CultureInfo.InvariantCulture);
                    gdm78261.SetMeasurementToCurrentDC(currentRange);
                    return DeviceResult.ResultOk($"Измерение напряжения {currentRange} прошло успешно");
                default:
                    return DeviceResult.ResultError($"Неизвестная команда {deviceData.Command}");
            }
        }
    }
}