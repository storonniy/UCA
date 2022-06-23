using System;
using System.Collections.Generic;
using System.Linq;
using System.IO.Ports;
using System.Threading;
using System.Text;

namespace UPD.DeviceDrivers
{
    class Simulator
    {
        readonly SerialPort serialPort;
        readonly int delay = 1500;
        public Simulator(SerialPort serialPort)
        {
            this.serialPort = serialPort;
            serialPort.Open();
        }

        ~Simulator()
        {
            this.serialPort.Close();
        }

        public static string[] GetRelayNamesAsAnArray(string relayNamesString)
        {
            relayNamesString = relayNamesString.Replace(" ", "").Replace("\r", "").Replace("\n", "");
            return relayNamesString.Split(',');
        }

        public static string DeleteIdentifierFromAnswer(string relayNamesString, string identifier)
        {
            if (!relayNamesString.Contains(identifier))
                throw new Exception("Simulator вернул неверный ответ на запрос.");
            return relayNamesString.Replace(identifier, "");
        }

        /// <summary>
        ///  Запрашивает имена замкнутых реле адаптера стыковки с ААП
        /// </summary>
        /// <returns>Возвращает массив типа string[], содержащий имена замкнутых реле</returns>
        public string[] GetClosedRelayNames()
        {
            SendCommand("*GetClosedRelayNemes");
            string closedRelayNamesString = serialPort.ReadExisting();
            string checkedClosedRelayNamesString = DeleteIdentifierFromAnswer(closedRelayNamesString, "*ClosedRelayNames:");
            return GetRelayNamesAsAnArray(checkedClosedRelayNamesString);
        }

        public string PrepareCommandForAdapter(params string[] relays)
        {
            return string.Join(",", relays);// "#010";
        }

        public void CloseRelays(params string[] relays)
        {
            string command = "*CloseRelays:" + PrepareCommandForAdapter(relays);
            SendCommand(command);
            Thread.Sleep(delay);
            var answerFromAdapter = serialPort.ReadExisting();
            if (answerFromAdapter != "*CloseRelays:Ok\r")
                throw new Exception($"При замыкании реле {command} возникла ошибка");
        }

        private byte[] GetBytes(string command)
        {
            var bytes = Encoding.ASCII.GetBytes(command);
            byte[] result = new byte[bytes.Length + 1];
            Array.Copy(bytes, result, bytes.Length);
            result[bytes.Length] = 0x0A;
            return result;
        }


        public void OpenRelays(params string[] relays)
        {
            string command = "*OpenRelays:" + PrepareCommandForAdapter(relays);
            SendCommand(command);
            Thread.Sleep(delay);
            var answerFromAdapter = serialPort.ReadExisting();
            if (answerFromAdapter != "*OpenRelays:Ok\r")
                throw new Exception($"При размыкании реле {command} возникла ошибка");
        }

        public void ConnectRelays(params string[] relays)
        {
            string command = "*ConnectRelays:" + PrepareCommandForAdapter(relays);
            SendCommand(command);
            var answerFromAdapter = serialPort.ReadExisting();
            if (answerFromAdapter.ToLower() != "*DisconnectRelays:Ok\r".ToLower())
                throw new Exception($"При коннекте реле {command} возникла ошибка");
        }

        private void SendCommand(string command)
        {
            var bytes = GetBytes(command);
            serialPort.Write(bytes, 0, bytes.Length);
            Thread.Sleep(delay);
        }

        public string GetIdentifier()
        {
            var command = "*IDN?\r";
            serialPort.WriteLine(command);
            Thread.Sleep(delay);
            return serialPort.ReadLine();
        }

        public string[] GetSignals()
        {
            var command = "*GetID\r";
            serialPort.WriteLine(command);
            Thread.Sleep(delay);
            var signalsString = serialPort.ReadExisting();
            string checkedSignals = DeleteIdentifierFromAnswer(signalsString, "*ID:");
            return GetRelayNamesAsAnArray(checkedSignals);
        }
    }
}
