using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static UCA.Auxiliary.UnitValuePair;
using static UCA.Devices.IDeviceInterface;


namespace UCA.Devices
{
    public abstract class IDeviceInterface
    {
        public abstract DeviceResult DoCommand(DeviceData deviceData);

        public static DeviceResult SetVoltage(DeviceData deviceData, Func<double, double> setVoltage)
        {
            var voltage = double.Parse(deviceData.Argument, CultureInfo.InvariantCulture);
            var result = setVoltage(voltage);
            return GetResult($"{deviceData.DeviceName}: Установлено напряжение", deviceData, UnitType.Voltage, result);
        }

        public static DeviceResult SetVoltage(DeviceData deviceData, Func<double, int, double> setVoltage)
        {
            var channel = int.Parse(deviceData.AdditionalArg);
            var voltage = double.Parse(deviceData.Argument, CultureInfo.InvariantCulture);
            var result = setVoltage(voltage, channel);
            return GetResult($"{ deviceData.DeviceName}: Установлено напряжение", deviceData, UnitType.Voltage, result);
        }

        public static DeviceResult SetCurrent(DeviceData deviceData, Func<double, int, double> setCurrent)
        {
            var channel = int.Parse(deviceData.AdditionalArg);
            var current = double.Parse(deviceData.Argument, CultureInfo.InvariantCulture);
            var result = setCurrent(current, channel);
            return GetResult($"{deviceData.DeviceName}: Установлен ток", deviceData, UnitType.Current, result);
        }

        public static DeviceResult SetCurrentLimit(DeviceData deviceData, Func<double, double> setCurrentLimit)
        {
            var currentLimit = double.Parse(deviceData.Argument, CultureInfo.InvariantCulture);
            var result = setCurrentLimit(currentLimit);
            return GetResult("Установлен предел по току", deviceData, UnitType.Current, result);
        }

        public static DeviceResult SetCurrentLimit(DeviceData deviceData, Func<double, int, double> setCurrentLimit)
        {
            var channel = int.Parse(deviceData.AdditionalArg);
            var currentLimit = double.Parse(deviceData.Argument, CultureInfo.InvariantCulture);
            var result = setCurrentLimit(currentLimit, channel);
            return GetResult("Установлен предел по току", deviceData, UnitType.Current, result);
        }

        public static DeviceResult PowerOn(DeviceData deviceData, Action powerOn)
        {
            powerOn();
            return DeviceResult.ResultOk($"{deviceData.DeviceName}: подан входной сигнал");
        }

        public static DeviceResult PowerOff(DeviceData deviceData, Action powerOff)
        {
            powerOff();
            return DeviceResult.ResultOk($"{deviceData.DeviceName}: снят входной сигнал");
        }

        public static DeviceResult GetResult(string message, DeviceData deviceData, UnitType unitType, double value)
        {
            var result = $"{message}: {GetValueUnitPair(value, unitType)} \tНижний предел: {GetValueUnitPair(deviceData.LowerLimit, unitType)}\t Верхний предел {GetValueUnitPair(deviceData.UpperLimit, unitType)}";
            if (value >= deviceData.LowerLimit && value <= deviceData.UpperLimit)
            {
                return DeviceResult.ResultOk(result);
            }
            else
            {
                return DeviceResult.ResultError($"ОШИБКА: {result}");
            }
        }

        #region Coefficient
        public struct InputData
        {
            public InputData(int channel, double inputValue)
            {
                Channel = channel;
                InputValue = inputValue;
            }
            public int Channel;
            public double InputValue;
        }

        private static Dictionary<InputData, List<double>> coefficientValuesDictionary = new Dictionary<InputData, List<double>>();

        public static void ClearCoefficientDictionary()
        {
            coefficientValuesDictionary.Clear();
        }

        public static void AddCoefficientData(int channel, double expectedValue, double value)
        {
            if (channel > 0)
            {
                InputData inputData = new InputData(channel, expectedValue);
                if (!coefficientValuesDictionary.ContainsKey(inputData))
                {
                    coefficientValuesDictionary.Add(inputData, new List<double> { value });
                }
                else
                {
                    coefficientValuesDictionary[inputData].Add(value);
                }
            }
        }

        public static List<double> GetCoefficientValues(int channel, double value)
        {
            InputData inputData = new InputData(channel, value);
            return coefficientValuesDictionary[inputData];
        }

        #endregion

        #region GDM78261 Saving Values

        public static void AddValues(string key, double measuredValue)
        {
            if (valuesDictionary.ContainsKey(key))
            {
                valuesDictionary[key] = measuredValue;
            }
            else
            {
                valuesDictionary.Add(key, measuredValue);
            }
        }

        public static double GetValue(string key)
        {
            return valuesDictionary[key];
        }

        private static Dictionary<string, double> valuesDictionary = new Dictionary<string, double>();

        /// <summary>
        /// Возвращает библиотеку с сохраненными парами ключ-значение (измеренное GDM)
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, double> GetValuesDictionary()
        {
            return valuesDictionary;
        }

        public static void ClearValuesDictionary()
        {
            coefficientValuesDictionary.Clear();
        }

        #endregion
    }
}
