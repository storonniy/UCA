using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCA.Devices
{
    public abstract class IDeviceInterface
    {
        public abstract DeviceResult DoCommand(DeviceData deviceData);
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
    }
}
