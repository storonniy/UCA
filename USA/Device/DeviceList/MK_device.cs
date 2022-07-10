using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCA.Devices;
using UPD.DeviceDrivers;
using static UCA.Devices.DeviceResult;

namespace UPD.Device.DeviceList
{
    public class MK_device : IDeviceInterface
    {
        private readonly MK mk;
        public MK_device()
        {
            mk = new MK();
        }

        public override void Die()
        {
            mk.Die();
        }

        public override DeviceResult DoCommand(DeviceData deviceData)
        {
            switch (deviceData.Command)
            {
                case DeviceCommands.GetClosedRelayNames:
                    var closedRelays = mk.GetClosedRelayNames();
                    return ResultOk(string.Join("\n", closedRelays));
                case DeviceCommands.OpenAllRelays:
                    return OpenAllRelays(deviceData, mk.EmergencyBreak);
                case DeviceCommands.CloseRelays:
                    return CloseRelays(deviceData, mk.CloseRelays);
                case DeviceCommands.OpenRelays:
                    return OpenRelays(deviceData, mk.OpenRelays);
                default:
                    return ResultError($"Неизвестная команда {deviceData.Command}");
            }
        }
    }
}
