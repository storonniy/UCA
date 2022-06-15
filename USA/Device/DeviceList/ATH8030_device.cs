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
                    if (ath8030.PowerOff())
                        return DeviceResult.ResultOk($"Снятие входного сигнала с {deviceData.DeviceName} прошло успешно");
                    else
                        return DeviceResult.ResultError($"ОШИБКА: Снятие входного сигнала с {deviceData.DeviceName} завершилось с ошибкой");
                case DeviceCommands.PowerOn:
                    if (ath8030.PowerOn())
                        return DeviceResult.ResultOk($"Подан входной сигнал с {deviceData.DeviceName}");
                    else
                        return DeviceResult.ResultError($"Ошибка: попытка подачи входного сигнала с {deviceData.DeviceName} завершилась с ошибкой");
                case DeviceCommands.SetCurrent:
                    var current = float.Parse(deviceData.Argument);
                    var fData = BitConverter.GetBytes(current);
                    var resultCurrent = $"Уcтановлено значение тока {GetValueUnitPair(current, UnitType.Current)} \t Нижний предел: {GetValueUnitPair(lowerLimit, UnitType.Current)}\t Верхний предел {GetValueUnitPair(upperLimit, UnitType.Current)}";
                    if (ath8030.SetCurrent(current))
                        return DeviceResult.ResultOk(resultCurrent);
                    else
                        return DeviceResult.ResultError($"Ошибка: не удалось установить сигнал {GetValueUnitPair(current, UnitType.Current)}");
                case DeviceCommands.SetMaxCurrent:
                    var maxCurrent = float.Parse(deviceData.Argument);
                    var resultMaxCurrent = $"Уcтановлено максимальное значение тока {GetValueUnitPair(maxCurrent, UnitType.Current)} \t Нижний предел: {GetValueUnitPair(lowerLimit, UnitType.Current)}\t Верхний предел {GetValueUnitPair(upperLimit, UnitType.Current)}";
                    if (ath8030.SetMaxCurrent(maxCurrent))
                        return DeviceResult.ResultOk(resultMaxCurrent);
                    else
                        return DeviceResult.ResultError($"Ошибка: {resultMaxCurrent}");
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
                    return GetResult("Измерено значение тока", deviceData, UnitType.Current, actualCurrent);

                case DeviceCommands.GetLoadCurrent:                  
                    var actualLoadCurrent = ath8030.GetLoadCurrent();
                    return GetResult("Измерено значение тока", deviceData, UnitType.Current, actualLoadCurrent);
                case DeviceCommands.GetMaxCurrent:
                    var maxCurrent1 = ath8030.GetMaxCurrent();
                    return GetResult("Измерен ток", deviceData, UnitType.Current, maxCurrent1);
                default:
                    return DeviceResult.ResultError($"Неизвестная команда {deviceData.Command}");
            }
        }
    }
}
