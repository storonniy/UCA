using System.Collections.Generic;
using System.IO.Ports;

namespace UCA.Devices
{
    public struct Device
    {
        public SerialPort SerialPort;
        public DeviceNames Name;
        public DeviceStatus Status;
        public string Description;
/*        public Device(DeviceNames name, DeviceStatus status, SerialPort serialPort, string description)
        {
            SerialPort = serialPort;
            Name = name;
            Status = status;
            Description = description;
        }*/
    }

    public enum DeviceStatus
    {
        ERROR,
        OK,
        NOT_CONNECTED
    }

    public struct DeviceData
    {
        public DeviceNames DeviceName;
        public DeviceCommands Command;
        public string Argument;
        public string ExpectedValue;
        public double LowerLimit;
        public double UpperLimit;
        public int Channel;
        public bool ShowStep;
    }

    public struct DeviceResult
    {
        public DeviceStatus State;
        public string Description;

        public static DeviceResult ResultOk(string description) => new DeviceResult()
        {
            State = DeviceStatus.OK,
            Description = description
        };

        public static DeviceResult ResultError(string description) => new DeviceResult()
        {
            State = DeviceStatus.ERROR,
            Description = description
        };
        public static DeviceResult ResultNotConnected(string description) => new DeviceResult()
        {
            State = DeviceStatus.NOT_CONNECTED,
            Description = description
        };
    }

    public enum DeviceCommands
    {
        // GDM
        GetVoltageAndSave,
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
        SetCurrentControlMode,
        GetMaxCurrent,
        SetMaxCurrent,
        GetLoadCurrent,
        // PCI_1762
        Commutate,
        ReadPCI1762Data,
        //
        CalculateCoefficient_UCAT,
        SetMeasurementToCurrent, // GDM
        SetMeasurementToVoltage,
        // AKIP_3407
        SetFrequency,
        // None
        Divide,
        Substract,
        Save,
        MultiplyAndSave,
        //
        Sleep
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
        AKIP_3407,
        Keithley2401_1,
        Keithley2401_2,
        Commutator_1
    }
}