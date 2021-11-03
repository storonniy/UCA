using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using UCA.DeviceDrivers;
using System.Globalization;
using static UCA.Auxiliary.UnitValuePair;

namespace UCA.Devices
{
    class AKIP3407_device : IDeviceInterface
    {
        int delay = 1000;
        private AKIP_3407 akip3407;
        public AKIP3407_device (SerialPort serialPort)
        {
            akip3407 = new AKIP_3407(serialPort);
        }
        public override DeviceResult DoCommand (DeviceData deviceData)
        {
            switch (deviceData.Command)
            {
                case DeviceCommands.SetVoltage:
                    var expectedVoltage = double.Parse(deviceData.Argument, CultureInfo.InvariantCulture);
                    var actualVoltage = akip3407.SetVoltage(expectedVoltage);
                    if (Math.Abs(expectedVoltage - actualVoltage) <= 0.1 * Math.Abs(expectedVoltage))
                    {
                        return DeviceResult.ResultOk($"Установлено напряжение {GetValueUnitPair(actualVoltage, UnitType.Voltage)}");
                    }
                    else
                    {
                        return DeviceResult.ResultError($"ОШИБКА: установлено напряжение {GetValueUnitPair(actualVoltage, UnitType.Voltage)}, ожидалось {GetValueUnitPair(expectedVoltage, UnitType.Voltage)}");
                    }
                case DeviceCommands.SetFrequency:
                    var expectedFrequency = double.Parse(deviceData.Argument, CultureInfo.InvariantCulture);
                    var actualFrequency = akip3407.SetVoltage(expectedFrequency);
                    if (Math.Abs(expectedFrequency - actualFrequency) <= 0.1 * Math.Abs(expectedFrequency))
                    {
                        return DeviceResult.ResultOk($"Установлено значение {actualFrequency} Гц");
                    }
                    else
                    {
                        return DeviceResult.ResultError($"ОШИБКА: установлено значение {actualFrequency} Гц, ожидалось {expectedFrequency} Гц");
                    }
                case DeviceCommands.PowerOn:
                    var actualStatus = akip3407.PowerOn();
                    if (actualStatus)
                    {
                        return DeviceResult.ResultOk($"{deviceData.DeviceName} успешно включён");
                    }
                    else
                    {
                        return DeviceResult.ResultError($"ОШИБКА: не удалось включить {deviceData.DeviceName}");
                    }
                case DeviceCommands.PowerOff:
                    var actualPowerStatus = akip3407.PowerOn();
                    if (!actualPowerStatus)
                    {
                        return DeviceResult.ResultOk($"{deviceData.DeviceName} успешно отключён");
                    }
                    else
                    {
                        return DeviceResult.ResultError($"ОШИБКА: не удалось отключить {deviceData.DeviceName}");
                    }
                default:
                    return DeviceResult.ResultError($"Неизвестная команда {deviceData.Command}");
            }
        }
    }
}
