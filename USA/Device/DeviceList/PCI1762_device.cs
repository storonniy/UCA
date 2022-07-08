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
        public PCI1762_device (string description)
        {
            pci1762 = new PCI_1762(description);
        }

        public override DeviceResult DoCommand(DeviceData deviceData)
        {
            switch (deviceData.Command)
            {
                case DeviceCommands.CloseRelays:
                    {
                        var relayNumbers = GetRelayNumbersArray(deviceData.Argument);
                        var errorCode = pci1762.CloseRelays(relayNumbers);
                        var resultOK = errorCode == Automation.BDaq.ErrorCode.Success;
                        if (resultOK)
                            return ResultOk($"Реле {string.Join(", ", relayNumbers)} замнкуты успешно");
                        return ResultError($"При замыкании реле {string.Join(", ", relayNumbers)} произошла ошибка: {errorCode}");
                    }
                case DeviceCommands.OpenRelays:
                    {
                        var relayNumbers = GetRelayNumbersArray(deviceData.Argument);
                        var errorCode = pci1762.OpenRelays(relayNumbers);
                        var resultOK = errorCode == Automation.BDaq.ErrorCode.Success;
                        if (resultOK)
                            return ResultOk($"Реле {string.Join(", ", relayNumbers)} разомкнуты успешно");
                        return ResultError($"При размыкании реле {string.Join(", ", relayNumbers)} произошла ошибка: {errorCode}");
                    }
                case DeviceCommands.ReadPCI1762Data:
                    var port = int.Parse(deviceData.Argument);
                    var signal = int.Parse(deviceData.AdditionalArg);
                    var portByte = pci1762.Read(port);
                    if (portByte == (byte)signal)
                        return ResultOk($"Сигнал {portByte} присутствует");
                    return ResultError($"Ошибка: сигнал {portByte} отсутствует");
                case DeviceCommands.OpenAllRelays:
                    return OpenAllRelays();
                default:
                    return ResultError($"Неизвестная команда {deviceData.Command}");
            }
        }
       

        public DeviceResult OpenAllRelays()
        {
            var errorCode = pci1762.OpenAllRelays();
            var resultOK = errorCode == Automation.BDaq.ErrorCode.Success;
            if (resultOK)
                return ResultOk($"Реле разомкнуты успешно");
            return ResultError($"При размыкании реле произошла ошибка: {errorCode}");
        }


        public static int[] GetRelayNumbersArray(string relayNames)
        {
            string[] relays = relayNames.Replace(" ", "").Split(',');
            var relayNumbers = new int[relays.Length];
            for (int i = 0; i < relays.Length; i++)
            {
                relayNumbers[i] = int.Parse(relays[i]);
            }
            return relayNumbers;
        }
    }
}
