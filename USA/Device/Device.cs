using System.IO.Ports;

namespace UCA.Devices
{
    public interface IDeviceInterface
    {
        DeviceResult DoCommand(DeviceData deviceData);
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
        public int Tolerance;
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
        SetVoltage,
        SetCurrent,
        PowerOn,
        PowerOff,
        CloseRelays,
        OpenRelays,
        CheckClosedRelays,
        OpenAllRelays,
        GetVoltage,
        GetCurrent,
        CalculateCoefficient,
        // УСА_Т
        // PSH_73610, PSH_73630 (относятся к типу PSH, свести к одному устройству PSH)
        SetVoltageProtection,
        SetCurrentProtection,
        ChangeOutputStatus,
        // ATH_8030
        OnOff,
        // PCI_1762
        Commutate_0,
        Commutate_1,
        CalculateCoeff_UCA_T
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