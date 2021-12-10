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
                    var lowerLimit = deviceData.LowerLimit;
                    var upperLimit = deviceData.UpperLimit;
                    var expectedVoltage = float.Parse(deviceData.Argument, CultureInfo.InvariantCulture);             
                    var actualVoltage = psh73610.SetVoltage(expectedVoltage);
                    var result = $"Уcтановлено напряжение {GetValueUnitPair(actualVoltage, UnitType.Voltage)} \t Нижний предел: {GetValueUnitPair(lowerLimit, UnitType.Voltage)}\t Верхний предел {GetValueUnitPair(upperLimit, UnitType.Voltage)}";
                    if (Math.Abs(actualVoltage) >= Math.Abs(lowerLimit) && Math.Abs(actualVoltage) <= Math.Abs(upperLimit))
                    {
                        return ResultOk(result);
                    }
                    else
                    {
                        return ResultError($"Ошибка: {result}");
                    }
                case DeviceCommands.SetCurrentLimit:
                    var lowerLimitCurrent = deviceData.LowerLimit;
                    var upperLimitCurrent = deviceData.UpperLimit;
                    var expectedCurrent = float.Parse(deviceData.Argument);
                    var actualCurrent = psh73610.SetCurrentLimit(expectedCurrent);
                    var resultCurrent = $"Уcтановлено значение тока {GetValueUnitPair(actualCurrent, UnitType.Current)} \t Нижний предел: {GetValueUnitPair(lowerLimitCurrent, UnitType.Current)}\t Верхний предел {GetValueUnitPair(upperLimitCurrent, UnitType.Current)}";
                    if (Math.Abs(actualCurrent) >= Math.Abs(lowerLimitCurrent) && Math.Abs(actualCurrent) <= Math.Abs(upperLimitCurrent))
                    {
                        return ResultOk(resultCurrent);
                    }
                    else
                    {
                        return ResultError($"Ошибка: {resultCurrent}");
                    }
                case DeviceCommands.PowerOff:
                    var actualStatus = psh73610.ChangeOutputStatus(0);
                    if (actualStatus == 0)
                    {
                        return DeviceResult.ResultOk($"Снятие входного сигнала с {deviceData.DeviceName}");
                    }
                    else
                    {
                        return DeviceResult.ResultError($"ОШИБКА: не удалось отключить входной сигнал с {deviceData.DeviceName}");
                    }
                case DeviceCommands.PowerOn:
                    var status = psh73610.ChangeOutputStatus(1);
                    if (status == 1)
                    {
                        return DeviceResult.ResultOk($"Подача входного сигнала с {deviceData.DeviceName}");
                    }
                    else
                    {
                        return DeviceResult.ResultError($"Ошибка: не удалось подать входной сигнал с {deviceData.DeviceName}");
                    }
                default:
                    return ResultError($"Неизвестная команда {deviceData.Command}");
            }
        }

    }
}
