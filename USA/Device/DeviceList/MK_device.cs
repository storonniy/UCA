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
        readonly MK mk;
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
                    return DeviceResult.ResultOk(string.Join("\n", closedRelays));
                case DeviceCommands.OpenAllRelays:
                    {
                        var status = mk.EmergencyBreak();
                        if (status)
                            return ResultOk($"{deviceData.DeviceName}: разомкнуты все реле");
                        return ResultError($"{deviceData.DeviceName}: не удалось разомкнуть все реле");
                    }
                case DeviceCommands.CloseRelays:
                    {
                        var relayNumbers = ParseRelayNumbers(deviceData.Argument);
                        var blockNumber = int.Parse(deviceData.AdditionalArg) - 1;
                        var status = mk.CloseRelays(blockNumber, relayNumbers);
                        if (status)
                            return ResultOk($"{deviceData.DeviceName}{deviceData.AdditionalArg} замкнуты реле {String.Join(", ", relayNumbers)}");
                        return ResultError($"ОШИБКА: {deviceData.DeviceName}{deviceData.AdditionalArg} не замкнуты реле {String.Join(", ", relayNumbers)}");
                    }
                case DeviceCommands.OpenRelays:
                    {
                        var relayNumbers = ParseRelayNumbers(deviceData.Argument);
                        var blockNumber = int.Parse(deviceData.AdditionalArg) - 1;
                        var status = mk.OpenRelays(blockNumber, relayNumbers);
                        if (status)
                            return ResultOk($"{deviceData.DeviceName}{deviceData.AdditionalArg} разомкнуты реле {String.Join(", ", relayNumbers)}");
                        return ResultError($"ОШИБКА: {deviceData.DeviceName}{deviceData.AdditionalArg} не разомкнуты реле {String.Join(", ", relayNumbers)}");
                    }
                default:
                    return ResultError($"Неизвестная команда {deviceData.Command}");
            }
        }

        public static int[] ParseRelayNumbers(string request)
        {
            var relayNames = request.Replace(" ", "").Split(',');
            var relayNumbers = new int[relayNames.Length];
            for (int i = 0; i < relayNumbers.Length; i++)
            {
                relayNumbers[i] = int.Parse(relayNames[i]) - 1; /// TODO: -1 добавлено
            }
            return relayNumbers;
        }
    }
}
