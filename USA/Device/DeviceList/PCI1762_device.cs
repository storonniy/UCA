using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCA.DeviceDrivers;
using static UCA.Devices.DeviceResult;

namespace UCA.Devices
{
    public class PCI1762_device : IDeviceInterface
    {
        readonly PCI_1762 pci1762;
        public PCI1762_device (int deviceNumber, int deviceID)
        {
            this.pci1762 = new PCI_1762(deviceNumber, deviceID);
        }

        public override DeviceResult DoCommand(DeviceData deviceData)
        {
            int portStart = 0;
            switch (deviceData.Command)
            {
                case DeviceCommands.Commutate_0:
                    portStart = 0;
                    break;
                case DeviceCommands.Commutate_1:
                    portStart = 1;
                    break;
                default:
                    return ResultError($"Неизвестная команда {deviceData.Command}");
            }
            byte[] relayNumbers = new byte[1] { byte.Parse(deviceData.Argument) };
            var resultOK = pci1762.PCI1762Command(relayNumbers, portStart);
            if (resultOK)
                return ResultOk("Реле замнкуты успешно");
            else
                return ResultError("ОШИБКА: при замыкании реле произошла ошибка");
        }
    }
}
