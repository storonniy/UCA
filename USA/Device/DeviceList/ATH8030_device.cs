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
                    var lowerLimitCurrent = deviceData.LowerLimit;
                    var upperLimitCurrent = deviceData.UpperLimit;
                    var current = float.Parse(deviceData.Argument);
                    resultOK = ath8030.SetCurrent(current);
                    var resultCurrent = $"Уcтановлено значение тока {GetValueUnitPair(current, UnitType.Current)} \t Нижний предел: {GetValueUnitPair(lowerLimitCurrent, UnitType.Current)}\t Верхний предел {GetValueUnitPair(upperLimitCurrent, UnitType.Current)}";
                    if (resultOK)
                        return DeviceResult.ResultOk(resultCurrent);
                    else
                        return DeviceResult.ResultError($"Ошибка: не удалось установить сигнал {GetValueUnitPair(current, UnitType.Current)}");
                case DeviceCommands.SetCurrentControlMode:
                    if (ath8030.SetCurrentControlMode())
                    {
                        return DeviceResult.ResultOk($"Устройство {deviceData.DeviceName} установлено в режим стабилизации тока");
                    }
                    else
                    {
                        return DeviceResult.ResultError($"Ошибка: устройство {deviceData.DeviceName} не установлено в режим стабилизации тока");
                    }
                default:
                    return DeviceResult.ResultError($"Неизвестная команда {deviceData.Command}");
            }
        }
    }
}
