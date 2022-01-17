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
                case DeviceCommands.Commutate:
                    var argument = deviceData.Argument;
                    var relayNumbers = GetRelayNumbersArray(deviceData.Argument);
                    return CloseRelay(relayNumbers);
                case DeviceCommands.ReadPCI1762Data:
                    var port = int.Parse(deviceData.Argument);
                    var signal = int.Parse(deviceData.ExpectedValue);
                    var portByte = pci1762.Read(port);
                    if (portByte == (byte)signal)
                    {
                        return ResultOk($"Сигнал {portByte} присутствует");
                    }
                    else
                    {
                        return ResultError($"Ошибка: сигнал {portByte} отсутствует");
                    }
                case DeviceCommands.OpenRelays:
                    return OpenRelays(deviceData.Argument);
                case DeviceCommands.OpenAllRelays:
                    return OpenAllRelays();
                default:
                    return ResultError($"Неизвестная команда {deviceData.Command}");
            }
        }

        private DeviceResult OpenRelays(string argument)
        {
            var relayNumbers = GetRelayNumbersArray(argument);
            var errorCode = pci1762.OpenRelays(relayNumbers);
            var resultOK = errorCode == Automation.BDaq.ErrorCode.Success;
            if (resultOK)
                return ResultOk($"Реле разомкнуты успешно");
            else
                return ResultError($"При размыкании реле произошла ошибка: {errorCode}");
        }
        
        public DeviceResult CloseRelay(int[] relayNumbers)
        {
            var errorCode = pci1762.Write(relayNumbers);
            var resultOK = errorCode == Automation.BDaq.ErrorCode.Success;
            if (resultOK)
                return ResultOk($"Реле замнкуты успешно");
            else
                return ResultError($"При замыкании реле произошла ошибка: {errorCode}");
        }

        public DeviceResult OpenAllRelays()
        {
            var errorCode = pci1762.OpenAllRelays();
            var resultOK = errorCode == Automation.BDaq.ErrorCode.Success;
            if (resultOK)
                return ResultOk($"Реле разомкнуты успешно");
            else
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
            try
            {

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
