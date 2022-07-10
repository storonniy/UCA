using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using UCA.DeviceDrivers;
using UPD.DeviceDrivers;
using UCA.Devices;

namespace UPD.Device.DeviceList
{
    class Simulator_device : IDeviceInterface
    {
        readonly Simulator Simulator;

        public Simulator_device(SerialPort serialPort)
        {
            serialPort.NewLine = "\r";
            Simulator = new Simulator(serialPort);
        }

        /// дописать OpenAllRelays
        public override DeviceResult DoCommand(DeviceData deviceData)
        {
            switch (deviceData.Command)
            {
                case DeviceCommands.GetClosedRelayNames:
                    var relays = Simulator.GetClosedRelayNames();
                    return DeviceResult.ResultOk($"\nSimulator: {string.Join(", ", relays)}");
                case DeviceCommands.CloseRelays:
                    return CloseRelays(deviceData, Simulator.CloseRelays);
                case DeviceCommands.OpenRelays:
                    return OpenRelays(deviceData, Simulator.OpenRelays);
                case DeviceCommands.OpenAllRelays:
                    return OpenAllRelays(deviceData, Simulator.OpenAllRelays);
                default:
                    return DeviceResult.ResultError($"{deviceData.DeviceName} Неизвестная команда {deviceData.Command}");
            }
        }
    }
}
