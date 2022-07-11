using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCA.Devices;
using UCA.Steps;
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

        public override DeviceResult DoCommand(Step step)
        {
            switch (step.Command)
            {
                case DeviceCommands.GetClosedRelayNames:
                    var closedRelays = mk.GetClosedRelayNames();
                    return ResultOk(string.Join("\n", closedRelays));
                case DeviceCommands.OpenAllRelays:
                    return OpenAllRelays(step, mk.EmergencyBreak);
                case DeviceCommands.CloseRelays:
                    return CloseRelays(step, mk.CloseRelays);
                case DeviceCommands.OpenRelays:
                    return OpenRelays(step, mk.OpenRelays);
                default:
                    return ResultError($"Неизвестная команда {step.Command}");
            }
        }
    }
}
