using System.Collections.Generic;
using System.IO.Ports;

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

    public struct Device
    {
        public SerialPort SerialPort;
        public DeviceNames Name;
    }

    public struct DeviceData
    {
        public DeviceNames DeviceName;
        public DeviceCommands Command;
        public string Argument;
        public string ExpectedValue;
        public string LowerLimit;
        public string UpperLimit;
        public int Channel;
        public bool ShowStep;
    }

    public struct DeviceResult
    {
        public DeviceState State;
        public string Description;

        public static DeviceResult ResultOk(string description) => new DeviceResult()
        {
            State = DeviceState.OK,
            Description = description
        };

        public static DeviceResult ResultError(string description) => new DeviceResult()
        {
            State = DeviceState.ERROR,
            Description = description
        };
    }

    public enum DeviceCommands
    {
        // УСА
        SetCurrentLimit,
        SetPowerLimit,
        SetVoltageLimit,
        SetVoltage,
        SetCurrent,
        PowerOn,
        PowerOff,
        CloseRelays,
        OpenRelays,
        CheckClosedRelays,
        OpenAllRelays,
        GetSignals,
        GetVoltage,
        GetCurrent,
        CalculateCoefficient,
        GetVoltageRipple,
        // УСА_Т
        // PSH_73610, PSH_73630 (относятся к типу PSH, свести к одному устройству PSH)
        SetVoltageProtection,
        SetCurrentProtection,
        // ATH_8030
        // PCI_1762
        Commutate_0,
        Commutate_1,
        CalculateCoeff_UCA_T,
        SetMeasurementToCurrent, // GDM
        SetMeasurementToVoltage,
        // AKIP_3407
        SetFrequency
    }

    public enum DeviceNames
    {
        PSP_405,
        PSP_405_power,
        Commutator,
        GDM_78261,
        Keysight_34410,
        None,
        // УСА_Т
        PSH_73610,
        PSH_73630,
        ATH_8030,
        PCI_1762,
        // УПД
        GDM_78341, // the same as GDM_78261
        PST_3201,
        PCI_1761_1,
        PCI_1761_2,
        PCI_1762_1,
        PCI_1762_2,
        PCI_1762_3,
        PCI_1762_4,
        PCI_1762_5,
        ASBL,
        AKIP_3407
    }

    public enum DeviceState
    {
        ERROR,
        OK
    }
}