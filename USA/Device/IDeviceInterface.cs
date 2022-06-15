using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static UCA.Auxiliary.UnitValuePair;


namespace UCA.Devices
{
    public abstract class IDeviceInterface
    {
        public abstract DeviceResult DoCommand(DeviceData deviceData);

        public DeviceResult GetResult(string message, DeviceData deviceData, UnitType unitType, double value)
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
