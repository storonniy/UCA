using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UCA.Devices;
using static UCA.Auxiliary.UnitValuePair;


namespace UPD.Device
{
    public abstract class IDeviceInterface
    {
        public abstract DeviceResult DoCommand(DeviceData deviceData);

        protected static DeviceResult SetVoltage(DeviceData deviceData, Func<double, double> setVoltage)
        {
            var voltage = double.Parse(deviceData.Argument, CultureInfo.InvariantCulture);
            var result = setVoltage(voltage);
            return GetResultOfSetting($"{deviceData.DeviceName}: Установлено напряжение", UnitType.Voltage, result, voltage);
        }

        protected static DeviceResult SetVoltage(DeviceData deviceData, Func<double, int, double> setVoltage)
        {
            var channel = int.Parse(deviceData.AdditionalArg);
            var voltage = double.Parse(deviceData.Argument, CultureInfo.InvariantCulture);
            var result = setVoltage(voltage, channel);
            return GetResultOfSetting($"{ deviceData.DeviceName}: Установлено напряжение", UnitType.Voltage, result, voltage);
        }

        protected static DeviceResult SetCurrent(DeviceData deviceData, Func<double, int, double> setCurrent)
        {
            var channel = int.Parse(deviceData.AdditionalArg);
            var current = double.Parse(deviceData.Argument, CultureInfo.InvariantCulture);
            var result = setCurrent(current, channel);
            return GetResultOfSetting($"{deviceData.DeviceName}: Установлен ток", UnitType.Current, result, current);
        }

        protected static DeviceResult SetCurrentLimit(DeviceData deviceData, Func<double, double> setCurrentLimit)
        {
            var currentLimit = double.Parse(deviceData.Argument, CultureInfo.InvariantCulture);
            var result = setCurrentLimit(currentLimit);
            return GetResultOfSetting($"{deviceData.DeviceName}: Установлен предел по току", UnitType.Current, result, currentLimit);
        }

        public static DeviceResult SetCurrentLimit(DeviceData deviceData, Func<double, int, double> setCurrentLimit)
        {
            var channel = int.Parse(deviceData.AdditionalArg);
            var currentLimit = double.Parse(deviceData.Argument, CultureInfo.InvariantCulture);
            var result = setCurrentLimit(currentLimit, channel);
            return GetResultOfSetting($"{deviceData.DeviceName}: Установлен предел по току", UnitType.Current, result, currentLimit);
        }

        protected static DeviceResult PowerOn(DeviceData deviceData, Action powerOn)
        {
            powerOn();
            return DeviceResult.ResultOk($"{deviceData.DeviceName}: подан входной сигнал");
        }

        protected static DeviceResult PowerOff(DeviceData deviceData, Action powerOff)
        {
            powerOff();
            return DeviceResult.ResultOk($"{deviceData.DeviceName}: снят входной сигнал");
        }

        protected static DeviceResult PowerOn(DeviceData deviceData, Func<bool> powerOn)
        {
            var status = powerOn();
            return status ? DeviceResult.ResultOk($"{deviceData.DeviceName}: подан входной сигнал") : DeviceResult.ResultError($"{deviceData.DeviceName}: ошибка при подаче входного сигнала");
        }

        protected static DeviceResult PowerOff(DeviceData deviceData, Func<bool> powerOff)
        {
            var status = powerOff();
            return status ? DeviceResult.ResultOk($"{deviceData.DeviceName}: снят входной сигнал") : DeviceResult.ResultError($"{deviceData.DeviceName}: ошибка при снятии входного сигнала");
        }

        protected static DeviceResult GetResult(string message, DeviceData deviceData, UnitType unitType, double value)
        {
            var result = $"{message}: {GetValueUnitPair(value, unitType)} \tНижний предел: {GetValueUnitPair(deviceData.LowerLimit, unitType)}\t Верхний предел {GetValueUnitPair(deviceData.UpperLimit, unitType)}";
            if (value >= deviceData.LowerLimit && value <= deviceData.UpperLimit)
                return DeviceResult.ResultOk(result);
            return DeviceResult.ResultError(result);
        }

        public static DeviceResult GetResultOfSetting(string message, UnitType unitType, double value, double expectedValue)
        {
            var result = $"{message}: {GetValueUnitPair(value, unitType)}";
            return Math.Abs(value - expectedValue) <= 0.1 * Math.Abs(expectedValue) ? DeviceResult.ResultOk(result) : DeviceResult.ResultError(result);
        }

        protected static DeviceResult CloseRelays(DeviceData deviceData, Func<int[], bool> closeRelays)
        {
            var relayNumbers = GetRelayNumbersArray(deviceData.Argument);
            var status = closeRelays(relayNumbers);
            if (status)
                return DeviceResult.ResultOk($"{deviceData.DeviceName}: Реле {string.Join(", ", relayNumbers)} замкнуты успешно");
            return DeviceResult.ResultError($"{deviceData.DeviceName}: При замыкании реле {string.Join(", ", relayNumbers)} произошла ошибка");
        }

        protected static DeviceResult CloseRelays(DeviceData deviceData, Func<int, int[], bool> closeRelays)
        {
            var relayNumbers = GetRelayNumbersArray(deviceData.Argument);
            var blockNumber = int.Parse(deviceData.AdditionalArg) - 1;
            var status = closeRelays(blockNumber, relayNumbers);
            if (status)
                return DeviceResult.ResultOk($"{deviceData.DeviceName}{deviceData.AdditionalArg} замкнуты реле {String.Join(", ", relayNumbers)}");
            return DeviceResult.ResultError($"ОШИБКА: {deviceData.DeviceName}{deviceData.AdditionalArg} не замкнуты реле {String.Join(", ", relayNumbers)}");
        }
        
        protected static DeviceResult OpenRelays(DeviceData deviceData, Func<int, int[], bool> openRelays)
        {
            var relayNumbers = GetRelayNumbersArray(deviceData.Argument);
            var blockNumber = int.Parse(deviceData.AdditionalArg) - 1;
            var status = openRelays(blockNumber, relayNumbers);
            if (status)
                return DeviceResult.ResultOk($"{deviceData.DeviceName}{deviceData.AdditionalArg} разомкнуты реле {String.Join(", ", relayNumbers)}");
            return DeviceResult.ResultError($"ОШИБКА: {deviceData.DeviceName}{deviceData.AdditionalArg} не разомкнуты реле {String.Join(", ", relayNumbers)}");
        }
        
        protected static DeviceResult OpenRelays(DeviceData deviceData, Func<int[], bool> openRelays)
        {
            var relayNumbers = GetRelayNumbersArray(deviceData.Argument);
            var status = openRelays(relayNumbers);
            if (status)
                return DeviceResult.ResultOk($"{deviceData.DeviceName}: Реле {string.Join(", ", relayNumbers)} разомкнуты успешно");
            return DeviceResult.ResultError($"{deviceData.DeviceName}: При размыкании реле {string.Join(", ", relayNumbers)} произошла ошибка");
        }

        protected static DeviceResult OpenAllRelays(DeviceData deviceData, Func<bool> openAllRelays)
        {
            var status = openAllRelays();
            return status ? DeviceResult.ResultOk($"{deviceData.DeviceName}: разомкнуты все реле") : DeviceResult.ResultError($"{deviceData.DeviceName}: не удалось разомкнуть все реле");
        }
        
        public static int[] GetRelayNumbersArray(string relayNames)
        {
            return relayNames.Replace(" ", "").Split(',')
                .Select(int.Parse)
                .ToArray();
        }
        
        public virtual void Die()
        {

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

        private static readonly Dictionary<InputData, List<double>> coefficientValuesDictionary = new Dictionary<InputData, List<double>>();

        public static readonly Action ClearCoefficientDictionary = () => coefficientValuesDictionary.Clear();

        public static void AddCoefficientData(int channel, double expectedValue, double value)
        {
            if (channel <= 0) return;
            var inputData = new InputData(channel, expectedValue);
            if (!coefficientValuesDictionary.ContainsKey(inputData))
            {
                coefficientValuesDictionary.Add(inputData, new List<double> { value });
            }
            else
            {
                coefficientValuesDictionary[inputData].Add(value);
            }
        }

        protected static List<double> GetCoefficientValues(int channel, double value)
        {
            var inputData = new InputData(channel, value);
            return coefficientValuesDictionary[inputData];
        }

        #endregion

        #region GDM78261 Saving Values

        public static readonly Action<string, double> AddValues = (key, measuredValue) =>
            ValuesDictionary.SafeAdd(key, measuredValue);

        public static readonly Func<string, double> GetValue = (key) => ValuesDictionary[key];

        private static readonly Dictionary<string, double> ValuesDictionary = new Dictionary<string, double>();

        /// <summary>
        /// Возвращает библиотеку с сохраненными парами ключ-значение (измеренное GDM)
        /// </summary>
        /// <returns></returns>
        public static readonly Func<Dictionary<string, double>> GetValuesDictionary = () => ValuesDictionary;

        public static readonly Action ClearValuesDictionary = () => ValuesDictionary.Clear();

        #endregion
    }

    public static class DictionaryExtensions
    {
        public static void SafeAdd<Tkey, Tvalue>(this Dictionary<Tkey, Tvalue> dictionary, Tkey key, Tvalue value)
        {
            if (dictionary.ContainsKey(key))
                dictionary[key] = value;
            else
                dictionary.Add(key, value);
        }
    }
}
