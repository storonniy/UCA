using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCA.DeviceDrivers;
using UPD.Device;
using static UCA.Devices.DeviceResult;

namespace UCA.Devices
{
    public class PCI1762_device : IDeviceInterface
    {
        readonly PCI_1762 pci1762;
        public PCI1762_device (string description)
        {
            pci1762 = new PCI_1762(description);
        }

        public override DeviceResult DoCommand(DeviceData deviceData)
        {
            switch (deviceData.Command)
            {
                case DeviceCommands.CloseRelays:
                    return CloseRelays(deviceData, pci1762.CloseRelays);
                case DeviceCommands.OpenRelays:
                    return OpenRelays(deviceData, pci1762.OpenRelays);
                case DeviceCommands.ReadPCI1762Data:
                    var port = int.Parse(deviceData.Argument);
                    var signal = int.Parse(deviceData.AdditionalArg);
                    var portByte = pci1762.Read(port);
                    if (portByte == (byte)signal)
                        return ResultOk($"Сигнал {portByte} присутствует");
                    return ResultError($"Ошибка: сигнал {portByte} отсутствует");
                case DeviceCommands.OpenAllRelays:
                    return OpenAllRelays(deviceData, pci1762.OpenAllRelays);
                case DeviceCommands.GetClosedRelayNames:
                    return ResultOk($"{deviceData.DeviceName}: {string.Join(", ", pci1762.GetClosedRelaysNumbers())}");
                default:
                    return ResultError($"Неизвестная команда {deviceData.Command}");
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
