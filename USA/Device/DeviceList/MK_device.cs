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
        public MK_device(string description)
        {
            var parameters = ParseDescription(description);
            mk = new MK(parameters.deviceNumber, parameters.blockType, parameters.moduleNumber, parameters.placeNumber, parameters.factoryNumber);
        }

        public override DeviceResult DoCommand(DeviceData deviceData)
        {
            switch (deviceData.Command)
            {
                case DeviceCommands.CloseRelays:
                    {
                        var relayNumbers = ParseRelayNumbers(deviceData.Argument);
                        // TODO: выяснить, какие ID у CAN-сообщений для МК
                        mk.CloseRelays(0, relayNumbers);
                        return ResultOk($"{deviceData.DeviceName} замкнуты реле {String.Join(", ", relayNumbers)}");
                    }
                case DeviceCommands.OpenRelays:
                    {
                        var relayNumbers = ParseRelayNumbers(deviceData.Argument);
                        // TODO: выяснить, какие ID у CAN-сообщений для МК
                        mk.CloseRelays(0, relayNumbers);
                        return ResultOk($"{deviceData.DeviceName} разомкнуты реле {String.Join(", ", relayNumbers)}");
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
                relayNumbers[i] = int.Parse(relayNames[i]);
            }
            return relayNumbers;
        }

        private struct MKParameters
        {
            public byte deviceNumber { get; set; }
            public int blockType { get; set; }
            public int moduleNumber { get; set; }
            public int placeNumber { get; set; }
            public int factoryNumber { get; set; }

            public MKParameters(byte deviceNumber, int blockType, int moduleNumber, int placeNumber, int factoryNumber)
            {
                this.deviceNumber = deviceNumber;
                this.blockType = blockType;
                this.moduleNumber = moduleNumber;
                this.placeNumber = placeNumber;
                this.factoryNumber = factoryNumber;
            }

        }

        private MKParameters ParseDescription(string description)
        {
            var parameters = description.Split(',');
            if (parameters.Length != 5)
                throw new Exception("Должно быть указано 5 параметров МК");
            return new MKParameters(byte.Parse(parameters[0]), int.Parse(parameters[1]), int.Parse(parameters[2]), int.Parse(parameters[3]), int.Parse(parameters[4]));
        }
    }
}
