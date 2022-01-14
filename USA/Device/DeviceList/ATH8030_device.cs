using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCA.DeviceDrivers;
using static UCA.Auxiliary.UnitValuePair;


namespace UCA.Devices
{
    class ATH8030_device : IDeviceInterface
    {
        readonly ATH_8030 ath8030;  

        public ATH8030_device (string portName)
        {
            ath8030 = new ATH_8030(portName);
        }

        public override DeviceResult DoCommand(DeviceData deviceData)
        {
            var lowerLimit = deviceData.LowerLimit;
            var upperLimit = deviceData.UpperLimit;
            switch (deviceData.Command)
            {
                case DeviceCommands.PowerOff:
                    var resultOK = ath8030.PowerOff();
                    if (resultOK)
                        return DeviceResult.ResultOk($"Снятие входного сигнала с {deviceData.DeviceName} прошло успешно");
                    else
                        return DeviceResult.ResultError($"ОШИБКА: Снятие входного сигнала с {deviceData.DeviceName} завершилось с ошибкой");
                case DeviceCommands.PowerOn:
                    var resultOK1 = ath8030.PowerOn();
                    if (resultOK1)
                        return DeviceResult.ResultOk($"Подан входной сигнал с {deviceData.DeviceName}");
                    else
                        return DeviceResult.ResultError($"Ошибка: попытка подачи входного сигнала с {deviceData.DeviceName} завершилась с ошибкой");
                case DeviceCommands.SetCurrent:
                    var current = float.Parse(deviceData.Argument);
                    var fData = BitConverter.GetBytes(current);
                    resultOK = ath8030.SetCurrent(current);
                    var resultCurrent = $"Уcтановлено значение тока {GetValueUnitPair(current, UnitType.Current)} \t Нижний предел: {GetValueUnitPair(lowerLimit, UnitType.Current)}\t Верхний предел {GetValueUnitPair(upperLimit, UnitType.Current)}";
                    if (resultOK)
                        return DeviceResult.ResultOk(resultCurrent);
                    else
                        return DeviceResult.ResultError($"Ошибка: не удалось установить сигнал {GetValueUnitPair(current, UnitType.Current)}");
                case DeviceCommands.SetMaxCurrent:
                    var maxCurrent = float.Parse(deviceData.Argument);
                    resultOK = ath8030.SetMaxCurrent(maxCurrent);
                    var resultMaxCurrent = $"Уcтановлено максимальное значение тока {GetValueUnitPair(maxCurrent, UnitType.Current)} \t Нижний предел: {GetValueUnitPair(lowerLimit, UnitType.Current)}\t Верхний предел {GetValueUnitPair(upperLimit, UnitType.Current)}";
                    if (resultOK)
                        return DeviceResult.ResultOk(resultMaxCurrent);
                    else
                        return DeviceResult.ResultError($"Ошибка: не удалось установить максимальное значение тока {GetValueUnitPair(maxCurrent, UnitType.Current)}");
                case DeviceCommands.SetCurrentControlMode:
                    if (ath8030.SetCurrentControlMode())
                    {
                        return DeviceResult.ResultOk($"Устройство {deviceData.DeviceName} установлено в режим стабилизации тока");
                    }
                    else
                    {
                        return DeviceResult.ResultError($"Ошибка: устройство {deviceData.DeviceName} не установлено в режим стабилизации тока");
                    }

                case DeviceCommands.GetCurrent:
                    var actualCurrent = ath8030.GetConstantCurrent();
                    var resultGetCurrent = $"Измерено значение тока {GetValueUnitPair(actualCurrent, UnitType.Current)} \t Нижний предел: {GetValueUnitPair(lowerLimit, UnitType.Current)}\t Верхний предел {GetValueUnitPair(upperLimit, UnitType.Current)}";
                    if (actualCurrent >= lowerLimit && actualCurrent <= upperLimit)
                    {
                        return DeviceResult.ResultOk(resultGetCurrent);
                    }
                    else
                    {
                        return DeviceResult.ResultError($"Ошибка: {resultGetCurrent}");
                    }
                case DeviceCommands.GetLoadCurrent:
                    var actualLoadCurrent = ath8030.GetLoadCurrent();
                    var resultLoadCurrent = $"Измерено значение тока {GetValueUnitPair(actualLoadCurrent, UnitType.Current)} \t Нижний предел: {GetValueUnitPair(lowerLimit, UnitType.Current)}\t Верхний предел {GetValueUnitPair(upperLimit, UnitType.Current)}";
                    if (actualLoadCurrent >= lowerLimit && actualLoadCurrent <= upperLimit)
                    {
                        return DeviceResult.ResultOk(resultLoadCurrent);
                    }
                    else
                    {
                        return DeviceResult.ResultError($"Ошибка: {resultLoadCurrent}");
                    }
                case DeviceCommands.GetMaxCurrent:
                    var maxCurrent1 = ath8030.GetMaxCurrent();
                    if (maxCurrent1 >= lowerLimit && maxCurrent1 <= upperLimit)
                    {
                        return DeviceResult.ResultOk($"Измерен ток {maxCurrent1}");
                    }
                    else
                    {
                        return DeviceResult.ResultError($"Ошибка: не удалось измерить ток {maxCurrent1}");
                    }
                default:
                    return DeviceResult.ResultError($"Неизвестная команда {deviceData.Command}");
            }
        }
    }
}
