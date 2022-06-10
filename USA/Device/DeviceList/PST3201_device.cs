using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCA.DeviceDrivers;
using System.IO.Ports;
using static UCA.Devices.DeviceResult;
using static UCA.Auxiliary.UnitValuePair;
using System.Globalization;

namespace UCA.Devices
{
    public class PST3201_device : IDeviceInterface
    {
        PST_3201 pst3201;

        public PST3201_device (SerialPort serialPort)
        {
            this.pst3201 = new PST_3201(serialPort);
        }

        public override DeviceResult DoCommand (DeviceData deviceData)
        {
            switch (deviceData.Command)
            {
                case DeviceCommands.SetVoltage:
                    var lowerLimit = deviceData.LowerLimit;
                    var upperLimit = deviceData.UpperLimit;
                    var expectedVoltage = Double.Parse(deviceData.Argument);
                    var actualVoltage = pst3201.SetVoltage(expectedVoltage, 1);
                    var result = $"Уcтановлено напряжение {GetValueUnitPair(actualVoltage, UnitType.Voltage)} \t Нижний предел: {GetValueUnitPair(lowerLimit, UnitType.Voltage)}\t Верхний предел {GetValueUnitPair(upperLimit, UnitType.Voltage)}";
                    if (Math.Abs(actualVoltage) >= Math.Abs(lowerLimit) && Math.Abs(actualVoltage) <= Math.Abs(upperLimit))
                        return ResultOk(result);
                    else
                        return ResultError($"Ошибка: {result}");
                case DeviceCommands.SetCurrent:
                    var lowerLimitCurrent = deviceData.LowerLimit;
                    var upperLimitCurrent = deviceData.UpperLimit;
                    var expectedCurrent = Double.Parse(deviceData.Argument);
                    var actualCurrent = pst3201.SetCurrent(expectedCurrent, 1);
                    var resultCurrent = $"Уcтановлен ток {GetValueUnitPair(actualCurrent, UnitType.Current)} \t Нижний предел: {GetValueUnitPair(lowerLimitCurrent, UnitType.Current)}\t Верхний предел {GetValueUnitPair(upperLimitCurrent, UnitType.Current)}";
                    if (Math.Abs(actualCurrent) >= Math.Abs(lowerLimitCurrent) && Math.Abs(actualCurrent) <= Math.Abs(upperLimitCurrent))
                        return ResultOk(resultCurrent);
                    else
                        return ResultError($"Ошибка: {resultCurrent}");
                case DeviceCommands.PowerOff:
                    var actualStatus = pst3201.ChangeOutputState("0");
                    if (!actualStatus)
                    {
                        return DeviceResult.ResultOk($"Снятие входного сигнала с {deviceData.DeviceName}");
                    }
                    else
                    {
                        return DeviceResult.ResultError($"ОШИБКА: не удалось отключить входной сигнал с {deviceData.DeviceName}");
                    }
                case DeviceCommands.PowerOn:
                    var status = pst3201.ChangeOutputState("1");
                    if (status)
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
