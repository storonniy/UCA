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
                    var relayNumbers = GetRelayNumbersArray(deviceData.Argument);
                    return CloseRelay(relayNumbers, 0);
                case DeviceCommands.Commutate_1:
                    return CloseRelay(GetRelayNumbersArray(deviceData.Argument), 1);
                case DeviceCommands.ReadPCI1762Data:
                    var port = int.Parse(deviceData.Argument);
                    var portByte = pci1762.Read(port);
                    if (portByte == 0xFF)
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

        public static DeviceResult CloseRelay(byte[] relayNumbers, int portStart)
        {
            var resultOK = pci1762.Write(relayNumbers, portStart);
            if (resultOK)
                return ResultOk($"Реле замнкуты успешно");
            else
                return ResultError($"Ошибка: при замыкании реле произошла ошибка");
        }

        public static byte[] GetRelayNumbersArray(string relayNames)
        {
            try
            {
                var relays = relayNames.Replace(" ", "").Split(',');
                var relayNumbers = new byte[8];
                for (var i = 0; i < relays.Length; i++)
                {
                    relayNumbers[i] = byte.Parse(relays[i]);
                }
                return relayNumbers;
            }
            catch (IndexOutOfRangeException)
            {
                throw new IndexOutOfRangeException("Число реле PCI_1762 не должно превышать 8");
            }
            catch (FormatException)
            {
                throw new FormatException("Реле PCI_1762 должны быть указаны в формате int через запятую");
            }
        }
    }
}
