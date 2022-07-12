using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Checker.DeviceDrivers;
using Checker.Steps;
using Checker.Device;
using Checker.DeviceInterface;
using static Checker.Devices.DeviceResult;

namespace Checker.Devices
{
    public class PCI1762_device : IDeviceInterface
    {
        readonly Pci1762 pci1762;
        public PCI1762_device (string description)
        {
            pci1762 = new Pci1762(description);
        }

        public override DeviceResult DoCommand(Step step)
        {
            switch (step.Command)
            {
                case DeviceCommands.CloseRelays:
                    return CloseRelays(step, pci1762.CloseRelays);
                case DeviceCommands.OpenRelays:
                    return OpenRelays(step, pci1762.OpenRelays);
                case DeviceCommands.ReadPCI1762Data:
                    var port = int.Parse(step.Argument);
                    var signal = int.Parse(step.AdditionalArg);
                    var portByte = pci1762.Read(port);
                    if (portByte == (byte)signal)
                        return ResultOk($"Сигнал {portByte} присутствует");
                    return ResultError($"Ошибка: сигнал {portByte} отсутствует");
                case DeviceCommands.OpenAllRelays:
                    return OpenAllRelays(step, pci1762.OpenAllRelays);
                case DeviceCommands.GetClosedRelayNames:
                    return ResultOk($"{step.DeviceName}: {string.Join(", ", pci1762.GetClosedRelaysNumbers())}");
                default:
                    return ResultError($"Неизвестная команда {step.Command}");
            }
        }
        
        public static int[] GetRelayNumbersArray(string relayNames)
        {
            return relayNames.Replace(" ", "").Split(',')
                .Select(int.Parse)
                .ToArray();
        }
    }
}
