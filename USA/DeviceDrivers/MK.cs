using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VciCAN;
using Ixxat.Vci4.Bal.Can;
using System.Collections;
using System.Threading;

namespace UPD.DeviceDrivers
{
    public class MK
    {
        CanConNet vciDevice;
        readonly List<BlockData> blockDataList;

        public MK()
        {
            vciDevice = new CanConNet();
            blockDataList = WakeUp(); 
        }

        ~MK()
        {
            vciDevice.FinalizeApp();
        }

        public class BlockData
        {
            public BlockData(uint id, int factoryNumber)
            {
                Id = id;
                FactoryNumber = factoryNumber;
            }

            public uint Id { get; private set; }
            public int FactoryNumber { get; private set; }

            public override bool Equals(object obj)
            {
                if (!(obj is BlockData))
                    return false;
                var blockData = (BlockData)obj;
                return Id.Equals(blockData.Id) && FactoryNumber.Equals(blockData.FactoryNumber);
            }
        }

        public class BlockDataComparer : IComparer<BlockData>
        {
            public int Compare(BlockData blockData1, BlockData blockData2)
            {
                return blockData1.Id.CompareTo(blockData2.Id);
            }
        }

        private ICanMessage GetAnswer()
        {

            var answer = vciDevice.GetData();
            if (answer == null)
                throw new Exception("Устройство не отвечает");
            return answer;
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
            canMessage[0] = 0x01;
            canMessage[1] = 0xFF;
            canMessage[2] = BitConverter.GetBytes(blockType)[0];
            canMessage[2] = BitConverter.GetBytes(moduleNumber)[0];
            canMessage[4] = BitConverter.GetBytes(placeNumber)[0];
            canMessage[5] = BitConverter.GetBytes(factoryNumber)[0];
            canMessage[6] = BitConverter.GetBytes(factoryNumber)[1];
            canMessage[7] = 0xFF;
            vciDevice.TransmitData(canMessage, msgID);
            ICanMessage answer = GetAnswer();
            if (answer[0] == 0xFE /*&& answerFromMC[2] == 0x01*/)
            {
                return answer[2];
            }
            throw new Exception($"МК вернул ошибочный ответ: {String.Join(" ", answer)}");
        }

        #endregion

        #region 2 Emergency Breaking
        /// <summary>
        /// Разомкнуть все реле МК
        /// </summary>
        /// <returns> </returns>
        public byte EmergencyBreak()
        {
            uint ID = 0x00;
            byte[] canMessage = { 0x02, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
            vciDevice.TransmitData(canMessage, ID);
            var answer = GetAnswer();
            var status = answer[2];
            if (answer[0] == 0xFD)
            {
                return status;
            }
            throw new Exception($"МК вернул ошибочный ответ: {String.Join(" ", answer)}");
        }

        #endregion

        #region 3 Connect array of relays
        /// <summary>
        /// Замкнуть массив реле. В случае внутренней ошибки блока МК размыкает все реле МК.
        /// </summary>
        /// <returns> Возвращает статус операции (реле МК успешно разомкнуты/произошла ошибка) </returns>

        public byte CloseRelaysArray(int blockNumber)
        {
            uint id = blockDataList[blockNumber].Id;
            byte[] message1 = { 0x03, 0x01, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
            vciDevice.TransmitData(message1, id);
            byte[] message2 = { 0x03, 0x02, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
            vciDevice.TransmitData(message2, id);
            var answer = GetAnswer();
            var status = answer[2];
            if (answer[0] == 0xFC && status != 0x00)
            {
                return status;
            }
            EmergencyBreak();
            throw new Exception($"МК вернул ошибочный ответ: {String.Join(" ", answer)}");
        }

        #endregion

        #region 4 Request status of all relays

        /// <summary>
        /// Запрос состояния всех реле.
        /// </summary>
        /// <returns> Возвращает список объектов типа CanConNet.DataBuf, включающий CAN-сообщения от блока МК и его ID. </returns>
        public List<ICanMessage> RequestAllRelayStatus(int blockNumber)
        {
            uint id = blockDataList[blockNumber].Id;
            byte[] canMessage = { 0x04, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
            vciDevice.TransmitData(canMessage, id);
            var answer1 = GetAnswer();
            var answer2 = GetAnswer();
            return new List<ICanMessage>
            {
                answer1,
                answer2
            };
        }

        #endregion

        #region 5 Change relay state

        /// <summary>
        /// Изменяет состояние одного реле.
        /// </summary>
        /// <param name="relayNumber"> Номер реле (от 0 до 79) </param>
        /// <param name="relayState"> Состояние реле. True - замкнуть, false - разомкнуть </param>
        private byte ChangeRelayState(int blockNumber, int relayNumber, bool relayState)
        {
            uint id = blockDataList[blockNumber].Id;
            if (relayNumber < 0 || relayNumber > 79)
            {
                throw new Exception("Номер реле должен быть в диапазоне от 0 до 79");
            }
            byte stateByte = (byte)(relayState ? 0x01 : 0x00);
            byte[] canMessage = { 0x05, (byte)relayNumber, stateByte, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
            vciDevice.TransmitData(canMessage, id);
            var answer = GetAnswer();
            var returnedRelayNumber = answer[1];
            var status = answer[2];
            if (answer[0] == 0xFA)
            {
                if (returnedRelayNumber != (byte)relayNumber)
                {
                    EmergencyBreak();
                    throw new Exception($"МК не замкнул нужные реле: {String.Join(" ", answer)}");
                }
                return status;
            }          
            throw new Exception($"МК вернул ошибочный ответ: {String.Join(" ", answer)}");
        }

        private byte CloseRelay(int blockNumber, int relayNumber)
        {
            return ChangeRelayState(blockNumber, relayNumber, false);
        }

        private byte OpenRelay(int blockNumber, int relayNumber)
        {
            return ChangeRelayState(blockNumber, relayNumber, true);
        }

        public void CloseRelays(int blockNumber, int[] relayNumbers)
        {
            foreach (var relayNumber in relayNumbers)
            {
                CloseRelay(blockNumber, relayNumber);
            }
        }

        public void OpenRelays(int blockNumber, int[] relayNumbers)
        {
            foreach (var relayNumber in relayNumbers)
            {
                OpenRelay(blockNumber, relayNumber);
            }
        }

        #endregion

        #region 6 Request status of one of the relays

        /// <summary>
        /// Запрос состояния одного реле.
        /// </summary>
        /// <param name="relayNumber"> Номер запрашиваемого реле. </param>
        /// <returns> Возвращает объект типа CanConNet.DataBuf, содержащий данные об ответе МК. </returns>
        public byte RequestSingleRelayStatus(int blockNumber, int relayNumber)
        {
            uint id = blockDataList[blockNumber].Id;
            byte[] canMessage = { 0x06, (byte)relayNumber, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
            vciDevice.TransmitData(canMessage, id);
            var answer = GetAnswer();
            var returnedRelayNumber = answer[1];
            var status = answer[2];
            if (answer[0] == 0xF9 && returnedRelayNumber == (byte)relayNumber)
            {
                return status;
            }
            throw new Exception($"МК вернул ошибочный ответ: {String.Join(" ", answer)}");
        }

        #endregion

        #region 7 Request REC relay state

        /// <summary>
        /// Запрос состояния реле РЭК по факту
        /// </summary>
        public ICanMessage RequestRECRelayState(int blockNumber)
        {
            uint id = blockDataList[blockNumber].Id;
            byte byte1 = 0x07;
            byte[] canMessage = { byte1, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
            vciDevice.TransmitData(canMessage, id);
            var answer = GetAnswer();
            if (answer[0] != 0xF9)
            {
                throw new Exception($"МК вернул ошибочный ответ: {String.Join(" ", answer)}");
            }
            return answer;
        }

        #endregion 

        #region 8 Wake Up

        /// <summary>
        /// Проверка наличия оборудования.
        /// </summary>
        public List<BlockData> WakeUp()
        {
            uint id = 0x00;
            byte[] canMessage = { 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            vciDevice.TransmitData(canMessage, id);
            int numberOfBlocks = 7; // Комментарии никто не читает. Но в этой переменной отражено реальное число блоков.
            List<BlockData> blockDataList = new List<BlockData>();
            for (int i = 0; i < numberOfBlocks + 4; i++)
            {
                Thread.Sleep(100);
                var answer = GetAnswer();
                if (i == 0 | i == 1 | i == 2 | i == 3)
                    continue;
                if (answer[0] != 0xF7)
                    throw new Exception($"МК вернул ошибочный ответ: {String.Join(" ", answer)}");
                blockDataList.Add(new BlockData(answer.Identifier, 256 * answer[3] + answer[2]));
            }
            blockDataList.Sort(new BlockDataComparer());
            return blockDataList;         
        }

        #endregion
    }
}
