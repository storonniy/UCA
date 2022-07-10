using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static UCA.Auxiliary.UnitValuePair;
using System.Threading;
using UPD.Device;

namespace UCA.Devices
{
    public class None : IDeviceInterface
    {
        private double CalculateUCACoefficient(int channel, double value)
        {
            var valuesAtZero = GetCoefficientValues(channel, 0);
            var values = GetCoefficientValues(channel, value);
            var coeff = (values[1] - valuesAtZero[1]) / (values[0] - valuesAtZero[0]);
            switch (channel)
            {
                case 1:
                case 2:
                    return coeff;//Math.Abs(coeff * 672000.0 / 1000000.0);
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                    return Math.Abs(coeff / 1000000.0);
                default:
                    throw new Exception($"Номер канала должен быть от 1 до 10 для УСА, указан номер канала {channel}");
            }
        }

        public override DeviceResult DoCommand(DeviceData deviceData)
        {
            //try
            //{
                switch (deviceData.Command)
                {
                    case DeviceCommands.CalculateCoefficient:
                        {
                            var value = double.Parse(deviceData.Argument, NumberStyles.Float);
                            var lowerLimit = deviceData.LowerLimit;
                            var upperLimit = deviceData.UpperLimit;
                            try
                            {
                                var actualCoefficient = CalculateUCACoefficient(deviceData.Channel, value);
                                var result = $"Коэффициент равен {string.Format("{0:0.000}", actualCoefficient)} В/мкА \tНижний предел  {lowerLimit} В/мкА \tВерхний предел {upperLimit} В/мкА";
                                if (actualCoefficient >= lowerLimit && actualCoefficient <= upperLimit)
                                    return DeviceResult.ResultOk(result);
                                return DeviceResult.ResultError($"Ошибка: {result}");
                            }
                            catch (KeyNotFoundException)
                            {
                                var unitType = (deviceData.Channel > 2) ? UnitType.Current : UnitType.Voltage;
                                var data = $"lowerLimit {deviceData.LowerLimit}; upperLimit {deviceData.UpperLimit}";
                                return DeviceResult.ResultError($"{data} \n Для входного воздействия {GetValueUnitPair(value, unitType)} и канала {deviceData.Channel} не измерялись входные и выходные воздействия");
                            }
                        }
                    case DeviceCommands.CalculateCoefficient_UCAT:
                        return GetCoefficient_UCAT(deviceData);
                    case DeviceCommands.Sleep:
                        var timeInSeconds = int.Parse(deviceData.Argument);
                        var t = TimeSpan.FromSeconds(timeInSeconds);
                        var startTime = DateTime.Now;
                        while (DateTime.Now - startTime < t)
                        {
                            Thread.Sleep(1000);
                        }
                        return DeviceResult.ResultOk("");
                    case DeviceCommands.Divide:
                        {
                            var keys = GetKeys(deviceData.Argument);
                            var value = Divide(GetValue(keys.Keys[0]), GetValue(keys.Keys[1]));
                            return GetResult("Получено значение", deviceData, keys.UnitType, value);
                        }
                    case DeviceCommands.Substract:
                        {
                            var keys = GetKeys(deviceData.Argument);
                            var value = Substract(GetValue(keys.Keys[0]), GetValue(keys.Keys[1]));
                            return GetResult("Получено значение", deviceData, keys.UnitType, value);
                        }
                    case DeviceCommands.Save:
                        {
                            var keys = GetKeys(deviceData.Argument);
                            var value = double.Parse(deviceData.AdditionalArg, CultureInfo.InvariantCulture);
                            AddValues(keys.Keys[0], value);
                            var result = $"Сохранено значение {GetValueUnitPair(value, keys.UnitType)}";
                            return DeviceResult.ResultOk(result);
                        }
                    case DeviceCommands.MultiplyAndSave:
                        {
                            var keys = GetKeys(deviceData.Argument);
                            var value = Multiply(GetValue(keys.Keys[0]), GetValue(keys.Keys[1]));
                            var keyToSave = deviceData.AdditionalArg;
                            AddValues(keyToSave, value);
                            return GetResult("Получено значение", deviceData, keys.UnitType, value);
                        }
                    default:
                        return DeviceResult.ResultError($"Неизвестная команда {deviceData.Command}");
                }
            //}
/*            catch (KeyNotFoundException)
            {
                return DeviceResult.ResultError($"ОШИБКА: {deviceData.Command}: Ключ не найден");
            }*/
        }

        public static double Divide(double arg1, double arg2)
        {
            return arg1 / arg2;
        }

        public static double Multiply(double arg1, double arg2)
        {
            return arg1 * arg2;
        }

        private static double Substract(double arg1, double arg2)
        {
            return arg1 - arg2;
        }

        public static ValueKeys GetKeys(string rawArgument)
        {
            rawArgument = rawArgument.Replace(" ", "").Replace("\r", "");
            int unitIndex = rawArgument.IndexOf(';');
            string unitName = rawArgument.Substring(unitIndex + 1);
            UnitType unitType = GetUnitType(unitName);
            var keyString = rawArgument.Remove(unitIndex);
            string[] keys = keyString.Split(',');
            return new ValueKeys(unitType, keys);
        }

        private DeviceResult GetCoefficient_UCAT(DeviceData deviceData)
        {
            var value = double.Parse(deviceData.Argument, NumberStyles.Float);
            var lowerLimit = deviceData.LowerLimit;
            var upperLimit = deviceData.UpperLimit;
            var actualCoefficient = CalculateCoefficient_UCAT(deviceData.Channel, value);
            var result = $"Коэффициент равен {string.Format("{0:0.000}", actualCoefficient)} В/мкА \tНижний предел  {lowerLimit} В/мкА \tВерхний предел {upperLimit} В/мкА";
            if (actualCoefficient >= lowerLimit && actualCoefficient <= upperLimit)
                return DeviceResult.ResultOk(result);
            else
                return DeviceResult.ResultError($"Ошибка: {result}");
/*            try
            {
                var actualCoefficient = CalculateCoefficient_UCAT(deviceData.Channel, value);
                var result = $"Коэффициент равен {string.Format("{0:0.000}", actualCoefficient)} В/мкА \tНижний предел  {lowerLimit} В/мкА \tВерхний предел {upperLimit} В/мкА";
                if (actualCoefficient >= lowerLimit && actualCoefficient <= upperLimit)
                    return DeviceResult.ResultOk(result);
                else
                    return DeviceResult.ResultError($"Ошибка: {result}");
            }
            catch (KeyNotFoundException)
            {
                UnitType unitType = (deviceData.Channel > 2) ? UnitType.Current : UnitType.Voltage;
                var data = $"lowerLimit {deviceData.LowerLimit}; upperLimit {deviceData.UpperLimit}";
                return DeviceResult.ResultError($"{data} \n Для входного воздействия {GetValueUnitPair(value, unitType)} и канала {deviceData.Channel} не измерялись входные и выходные воздействия");
            }*/
        }

        private double CalculateCoefficient_UCAT(int channel, double value)
        {
            var valuesAtZero = GetCoefficientValues(channel, 0);
            var values = GetCoefficientValues(channel, value);
            var meow = (values[0] - Math.Abs(valuesAtZero[0])) / value;
            return meow;    
        }
    }
}
