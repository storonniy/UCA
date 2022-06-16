using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VciCAN;
using Ixxat.Vci4.Bal.Can;


namespace UPD.DeviceDrivers
{
    public class MK
    {
        CanConNet vciDevice;
        public readonly uint mcID;

        public MK(byte deviceNumber, int blockType, int moduleNumber, int placeNumber, int factoryNumber)
        {
            vciDevice = new CanConNet(deviceNumber);
            // TODO: выяснить, где брать инфу про blockType, moduleNumber, ...
            mcID = AssignBlockID(blockType, moduleNumber, placeNumber, factoryNumber);
        }

        ~MK()
        {
            vciDevice.FinalizeApp();
        }

        #region CAN Initialization

        private ICanMessage GetAnswerFromMC()
        {
            while (true)
            {
                ICanMessage message = vciDevice.GetData();
                //WARNING: no cycle time limit. If MC does not respond, the program will freeze
                if (vciDevice.ThereIsANewMessage())
                {
                    return message;
                }
            }
        }
        #endregion

        private enum ErrorCode : uint
        {
            None = 0x00,
            InvalidResponse = 0x01,
            InvalidStatus = 0x02,
            InvalidRelayNumber = 0x03,
            BlockReturnedAWrongRelayNumber = 0x04
        }


        #region 1 Assign Block ID

        /// <summary>
        /// Присвоение ID блоку МК, подключённому к ПК.
        /// </summary>
        /// <param name="blockType"> Тип блока </param>
        /// <param name="moduleNumber"> Номер модуля </param>
        /// <param name="placeNumber"> Номер платоместа </param>
        /// <param name="factoryNumber"> Заводской номер </param>
        /// <returns> Возвращает ID блока МК, подключённого к ПК. </returns>

        public uint AssignBlockID(int blockType, int moduleNumber, int placeNumber, int factoryNumber)
        {
            uint msgID = 0x00;
            byte[] canMessage = new byte[8];
            canMessage[0] = 0x00;
            canMessage[1] = 0xFF;
            canMessage[2] = BitConverter.GetBytes(blockType)[0];
            canMessage[3] = BitConverter.GetBytes(placeNumber)[0];
            canMessage[4] = BitConverter.GetBytes(factoryNumber)[0];
            canMessage[5] = BitConverter.GetBytes(factoryNumber)[1];
            canMessage[6] = 0xFF;
            canMessage[7] = 0xFF;
            vciDevice.TransmitData(canMessage, msgID);
            ICanMessage answerFromMC = GetAnswerFromMC();
            if (answerFromMC[0] == 0xFE && answerFromMC[2] == 0x01)
            {
                return answerFromMC[2];
            }
            throw new Exception($"МК вернул ошибочный ответ: {String.Join(" ", answerFromMC)}");
        }

        #endregion

        #region 2 Emergency Breaking
        /// <summary>
        /// Разомкнуть все реле МК
        /// </summary>
        /// <returns> Возвращает true, если размыкание прошло успешно, и false, если произошла ошибка </returns>
        public byte EmergencyBreak()
        {
            uint ID = 0x00;
            byte[] canMessage = { 0x02, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
            vciDevice.TransmitData(canMessage, ID);
            var mkAnswer = GetAnswerFromMC();
            var status = mkAnswer[2];
            if (mkAnswer[0] == 0xFD)
            {
                return status;
            }
            throw new Exception($"МК вернул ошибочный ответ: {String.Join(" ", mkAnswer)}");
        }

        #endregion

        #region 3 Connect array of relays
        /// <summary>
        /// Замкнуть массив реле. В случае внутренней ошибки блока МК размыкает все реле МК.
        /// </summary>
        /// <returns> Возвращает статус операции (реле МК успешно разомкнуты/произошла ошибка) </returns>

        public byte CloseRelaysArray(uint ID)
        {
            byte someByte = 0xFF;
            byte[] message1 = { 0x03, 0x01, someByte, someByte, someByte, someByte, someByte, someByte };
            vciDevice.TransmitData(message1, ID);
            byte[] message2 = { 0x03, 0x02, someByte, someByte, someByte, someByte, someByte, someByte };
            vciDevice.TransmitData(message2, ID);
            var answerFromMC = GetAnswerFromMC();
            var status = answerFromMC[2];
            if (answerFromMC[0] == 0xFC && status != 0x00)
            {
                return status;
            }
            EmergencyBreak();
            throw new Exception($"МК вернул ошибочный ответ: {String.Join(" ", answerFromMC)}");
        }

        #endregion

        #region 4 Request status of all relays

        /// <summary>
        /// Запрос состояния всех реле.
        /// </summary>
        /// <returns> Возвращает список объектов типа CanConNet.DataBuf, включающий CAN-сообщения от блока МК и его ID. </returns>
        public List<ICanMessage> RequestAllRelayStatus(uint ID)
        {
            byte someByte = 0xFF;
            byte[] canMessage = { 0x04, someByte, someByte, someByte, someByte, someByte, someByte, someByte };
            vciDevice.TransmitData(canMessage, ID);
            var answerFromMC1 = GetAnswerFromMC();
            var answerFromMC2 = GetAnswerFromMC();
            return new List<ICanMessage>
            {
                answerFromMC1,
                answerFromMC2
            };
        }

        #endregion

        #region 5 Change relay state

        /// <summary>
        /// Изменяет состояние одного реле.
        /// </summary>
        /// <param name="relayNumber"> Номер реле (от 0 до 79) </param>
        /// <param name="relayState"> Состояние реле. True - замкнуть, false - разомкнуть </param>
        private byte ChangeRelayState(uint msgID, int relayNumber, bool relayState)
        {
            if (relayNumber < 0 || relayNumber > 79)
            {
                throw new Exception("Номер реле должен быть в диапазоне от 0 до 79");
            }
            byte stateByte = (byte)(relayState ? 0x01 : 0x00);
            byte[] canMessage = { 0x05, (byte)relayNumber, stateByte, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
            vciDevice.TransmitData(canMessage, msgID);
            var answerFromMC = GetAnswerFromMC();
            var returnedRelayNumber = answerFromMC[1];
            var status = answerFromMC[2];
            if (answerFromMC[0] == 0xFA && returnedRelayNumber == (byte)relayNumber)
            {
                return status;
            }
            EmergencyBreak();
            throw new Exception($"МК вернул ошибочный ответ: {String.Join(" ", answerFromMC)}");

        }

        private byte CloseRelay(uint msgID, int relayNumber)
        {
            return ChangeRelayState(msgID, relayNumber, true);
        }

        private byte OpenRelay(uint msgID, int relayNumber)
        {
            return ChangeRelayState(msgID, relayNumber, true);
        }

        public void CloseRelays(uint msgID, int[] relayNumbers)
        {
            foreach (var relayNumber in relayNumbers)
            {
                CloseRelay(msgID, relayNumber);
            }
        }

        public void OpenRelays(uint msgID, int[] relayNumbers)
        {
            foreach (var relayNumber in relayNumbers)
            {
                OpenRelay(msgID, relayNumber);
            }
        }

        #endregion

        #region 6 Request status of one of the relays

        /// <summary>
        /// Запрос состояния одного реле.
        /// </summary>
        /// <param name="relayNumber"> Номер запрашиваемого реле. </param>
        /// <returns> Возвращает объект типа CanConNet.DataBuf, содержащий данные об ответе МК. </returns>
        public byte RequestSingleRelayStatus(uint ID, int relayNumber)
        {
            byte someByte = 0xFF;
            byte[] canMessage = { 0x06, (byte)relayNumber, someByte, someByte, someByte, someByte, someByte, someByte };
            vciDevice.TransmitData(canMessage, ID);
            var answerFromMC = GetAnswerFromMC();
            var returnedRelayNumber = answerFromMC[1];
            var status = answerFromMC[2];
            if (answerFromMC[0] == 0xF9 && returnedRelayNumber == (byte)relayNumber)
            {
                return status;
            }
            throw new Exception($"МК вернул ошибочный ответ: {String.Join(" ", answerFromMC)}");
        }

        #endregion

        #region 7 Request REC relay state

        /// <summary>
        /// Запрос состояния реле РЭК по факту
        /// </summary>
        public ICanMessage RequestRECRelayState(uint ID)
        {
            byte byte1 = 0x07;
            byte someByte = 0xFF;
            byte[] canMessage = { byte1, someByte, someByte, someByte, someByte, someByte, someByte, someByte };
            vciDevice.TransmitData(canMessage, ID);
            var answerFromMC = GetAnswerFromMC();
            if (answerFromMC[0] != 0xF9)
            {
                throw new Exception($"МК вернул ошибочный ответ: {String.Join(" ", answerFromMC)}");
            }
            return answerFromMC;
        }

        #endregion 

        #region 8 Wake Up

        /// <summary>
        /// Проверка наличия оборудования.
        /// </summary>
        public void WakeUp()
        {
            uint ID = 0x00;
            byte someByte = 0xFF;
            byte[] canMessage = { 0x08, someByte, someByte, someByte, someByte, someByte, someByte, someByte };
            vciDevice.TransmitData(canMessage, ID);
            int numberOfBlocks = 78; // Комментарии никто не читает. Но в этой переменной отражено реальное число блоков.
            List<ICanMessage> allBlocks = new List<ICanMessage>();
            for (int i = 0; i < numberOfBlocks; i++)
            {
                var answerFromMC = GetAnswerFromMC();
                allBlocks.Add(answerFromMC);
            }
        }

        #endregion
    }
}
