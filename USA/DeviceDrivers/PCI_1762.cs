using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;
using Automation.BDaq;

namespace UCA.DeviceDrivers
{
    public class PCI_1762
    {
        private InstantDoCtrl instantDoCtrl;
        BDaqDevice device;
        public PCI_1762 (string description)
        {
            instantDoCtrl = new InstantDoCtrl();
            instantDoCtrl.SelectedDevice = new DeviceInformation(description);
            if (!instantDoCtrl.Initialized)
            {
                
            }
        }

        public ErrorCode Write(int[] relayNumbers)
        {
            ErrorCode errorCode = ErrorCode.ErrorUndefined;
            var dict = GetPortNumDictionary(relayNumbers);
            foreach (int portNum in dict.Keys)
            {
                errorCode = instantDoCtrl.Write(portNum, (byte)GetRelaysAsByte(dict[portNum]));
                if (errorCode != ErrorCode.Success)
                {
                    return errorCode;
                }
            }
            return errorCode;
        }

        public ErrorCode OpenRelays(int[] relayNumbers)
        {
            var dict = GetPortNumDictionary(relayNumbers);
            ErrorCode errorCode = ErrorCode.ErrorUndefined;
            foreach (var portNum in dict.Keys)
            {
                var portByte = Read(portNum);
                var data = GetRelaysAsByte(dict[portNum]);
                byte res = (byte)(portByte & data);
                errorCode = instantDoCtrl.Write(portNum, (byte)(portByte - res));
                if (errorCode != ErrorCode.Success)
                {
                    return errorCode;
                }
            }
            return errorCode;
        }

        public ErrorCode OpenAllRelays()
        {
            var errorCode = instantDoCtrl.Write(0, 0x00);
            if (errorCode != ErrorCode.Success)
            {
                return errorCode;
            }
            errorCode = instantDoCtrl.Write(1, 0x00);
            return errorCode;
        }

        public static byte GetRelaysAsByte(List<int> relayNumbers)
        {
            var hashSet = new HashSet<int>(relayNumbers);
            byte data = 0;
            foreach (var relayNumber in hashSet)
            {
                if (relayNumber < 0 || relayNumber > 7)
                {
                    throw new Exception($"Номер реле не может быть равен {relayNumber}.");
                }
                data += (byte)Math.Pow(2, relayNumber);
            }
            return data;
        }

        public static Dictionary<int, List<int>> GetPortNumDictionary(int[] relayNumbers)
        {
            var dict = new Dictionary<int, List<int>>();
            foreach (int relayNumber in relayNumbers)
            {
                int portNum = relayNumber / 8;
                int relayNum = relayNumber % 8;
                if (dict.ContainsKey(portNum))
                {
                    dict[portNum].Add(relayNum);
                }
                else
                {
                    dict.Add(portNum, new List<int>() { relayNum });
                }
            }
            return dict;
        }

        public byte Read(int port)
        {
            byte data;
            instantDoCtrl.Read(port, out data);
            return data;
        }

        public int[] GetClosedRelaysNumbers()
        {
            int maxPortNumber = 2;
            var relayNumbers = new List<int>();
            for (int portNum = 0; portNum < maxPortNumber; portNum++)
            {
                var data = Read(portNum);
                relayNumbers.AddRange(ConvertDataToRelayNumbers(data));
            }
            return relayNumbers.ToArray();            
        }

        public static List<int> ConvertDataToRelayNumbers(byte data)
        {
            var relayNumbers = new List<int>();
            var binary = Convert.ToString(data, 2);
            for (int i = 0; i < binary.Length; i++)
            {
                if (binary[i] == '1')
                {
                    relayNumbers.Add(7 - i);
                }
            }
            return relayNumbers;
        }
    }
}
