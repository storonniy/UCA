using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;
using Automation.BDaq;
using UPD.Auxiliary;
using UPD.Device;

namespace UCA.DeviceDrivers
{
    public class PCI_1762
    {
        private readonly InstantDoCtrl instantDoCtrl;
        private BDaqDevice device;

        public PCI_1762(string description)
        {
            instantDoCtrl = new InstantDoCtrl();
            instantDoCtrl.SelectedDevice = new DeviceInformation(description);
            if (!instantDoCtrl.Initialized)
            {
            }
        }

        public bool CloseRelays(int[] relayNumbers)
        {
            return ChangeRelayState(relayNumbers, getCloseRelayData);
        }

        public bool OpenRelays(int[] relayNumbers)
        {
            return ChangeRelayState(relayNumbers, getOpenRelayData);
        }

        static Func<byte, byte, byte> getOpenRelayData =
            (currentData, newData) => (byte) (currentData - (byte) (currentData & newData));

        static Func<byte, byte, byte> getCloseRelayData =
            (currentData, newData) => (byte) (currentData | newData);

        private bool ChangeRelayState(IEnumerable<int> relayNumbers, Func<byte, byte, byte> changePortData)
        {
            var dict = GetPortBytesDictionary(relayNumbers);
            foreach (var portNum in dict.Keys)
            {
                var currentData = Read(portNum);
                var newData = dict[portNum];
                var status = instantDoCtrl.Write(portNum, changePortData(currentData, newData));
                if (status != ErrorCode.Success)
                {
                    return false;
                }
            }
            return true;
        }

        public bool OpenAllRelays()
        {
            var statusPort1 = instantDoCtrl.Write(0, 0x00);
            var statusPort2 = instantDoCtrl.Write(1, 0x00);
            return statusPort1 == ErrorCode.Success && statusPort2 == ErrorCode.Success;
        }

        public static byte ConvertRelayNumbersToByte(IEnumerable<int> relayNumbers)
        {
            return (byte) relayNumbers
                .Distinct()
                .Where(relayNumber => relayNumber >= 0 && relayNumber <= 7)
                .Select(relayNumber => (byte) (1 << relayNumber))
                .Sum(x => x);
        }


        public static Dictionary<int, byte> GetPortBytesDictionary(IEnumerable<int> relayNumbers)
        {
            return relayNumbers
                .Select(r => Tuple.Create(r / 8, r % 8))
                .GroupBy(r => r.Item1)
                .ToDictionary(group => group.Key,
                    group => ConvertRelayNumbersToByte(group.Select(x => x.Item2).ToList()));
        }


        public byte Read(int port)
        {
            instantDoCtrl.Read(port, out var data);
            return data;
        }

        public List<int> GetClosedRelaysNumbers()
        {
            var maxPortNumber = 2;
            return Enumerable.Range(0, maxPortNumber)
                .SelectMany(portNum => ConvertDataToRelayNumbers(Read(portNum), portNum))
                .ToList();
        }

        public static IEnumerable<int> ConvertDataToRelayNumbers(byte data, int portNum)
        {
            return Enumerable.Range(0, 8)
                .Where(bitNumber => data.BitState(bitNumber))
                .Select(bitNumber => 8 * portNum + bitNumber);
        }
    }
}