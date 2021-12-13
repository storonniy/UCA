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
            pci1762 = new PCI_1762(deviceNumber, deviceID);
        }

        public override DeviceResult DoCommand(DeviceData deviceData)
        {
            int portStart = 0;
            switch (deviceData.Command)
            {
                case DeviceCommands.Commutate_0:
                    portStart = 0;
                    return CommutatePCI(deviceData, portStart);
                case DeviceCommands.Commutate_1:
                    portStart = 1;
                    return CommutatePCI(deviceData, portStart);
                case DeviceCommands.ReadPCI1762Data:
                    var port = int.Parse(deviceData.Argument);
                    var portByte = pci1762.Read(port);
                    if (portByte == 0x00)
                    {
                        return ResultOk($"{portByte}");
                    }
                    else
                    {
                        return ResultError($"Ошибка: {portByte}");
                    }
                default:
                    return ResultError($"Неизвестная команда {deviceData.Command}");
            }
        }

        private DeviceResult CommutatePCI(DeviceData deviceData, int portStart)
        {
            byte[] relayNumbers = new byte[1] { byte.Parse(deviceData.Argument) };
            var resultOK = pci1762.Write(relayNumbers, portStart);
            if (resultOK)
                return ResultOk($"Реле {relayNumbers[0]} замнкуты успешно");
            else
                return ResultError($"Ошибка: при замыкании реле  {relayNumbers[0]} произошла ошибка");
        }
    }
}
