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

        private ICanMessage GetAnswer(byte validFirstByte)
        {
            ICanMessage answer;
            do
            {
                answer = vciDevice.GetData();
                if (answer == null)
                    throw new Exception("Устройство не отвечает");
            } while (answer[0] != validFirstByte);
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
            var answer = GetAnswer(0xFE);
            return answer[2];
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
            Thread.Sleep(30);
            byte finalStatus = 0x00;
            for (int i = 0; i < blockDataList.Count; i++)
            {
                var answer = GetAnswer(0xFD);
                finalStatus = answer[2];
            }
            return finalStatus;
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
            var answer = GetAnswer(0xFC);
            var status = answer[2];
            return status;
        }

        #endregion

        #region 4 Request status of all relays

        /// <summary>
        /// Запрос состояния всех реле.
        /// </summary>
        public int[] RequestAllRelayStatus(int blockNumber)
        {
            uint id = blockDataList[blockNumber].Id;
            byte[] canMessage = { 0x04, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
            vciDevice.TransmitData(canMessage, id);
            var answer1 = GetAnswer(0xFB);
            var answer2 = GetAnswer(0xFB);
            var list =  new List<ICanMessage>
            {
                answer1,
                answer2
            };
            return GetRelayNumbers(ParseRelayStatus(list));
        }

        public static byte[] ParseRelayStatus(List<ICanMessage> canMessages)
        {
            return canMessages
                .Select(b => Tuple.Create(b[0], b))
                .OrderBy(tuple => tuple)
                .Select(tuple => tuple.Item2)
                .SelectMany(b => new byte[] { b[3], b[4], b[5], b[6], b[7] })
                .ToArray();
        }

        public static int[] GetRelayNumbers(byte[] rawData)
        {
            //var states = new bool[8 * rawData.Length];
            var states = new List<int>();
            for (int i = 0; i < rawData.Length; i++)
            {
                for (int position = 0; position < 8; position++)
                {
                    var bit = (rawData[i] & (1 << position)) >> position == 1;
                    if (bit)
                        states.Add(8 * i + position + 1);
                }
            }
            return states.ToArray();
        }

        public static int[] GetRelayNumbersReversedOrder(byte[] rawData)
        {
            //var states = new bool[8 * rawData.Length];
            var states = new List<int>();
            for (int i = 0; i < rawData.Length; i++)
            {
                for (int position = 0; position < 8; position++)
                {
                    var bit = (rawData[i] & (1 << position)) >> position == 1;
                    if (bit)
                        states.Add(8 * i +  7 - position + 1);
                }
            }
            return states.ToArray();
        }

        public string[] GetAllRelaysStatus()
        {
            var status = new List<string>();
            for (int blockNumber = 0; blockNumber < blockDataList.Count; blockNumber++)
            {
                var relayNumbers = RequestAllRelayStatus(blockNumber);
                status.Add($"MK{blockNumber + 1}: {string.Join(", ", relayNumbers)}");
            }
            return status.ToArray();
        }

        #endregion

        #region 5 Change relay state

        /// <summary>
        /// Изменяет состояние одного реле.
        /// </summary>
        /// <param name="relayNumber"> Номер реле (от 0 до 79) </param>
        /// <param name="relayState"> Состояние реле. True - замкнуть, false - разомкнуть </param>
        private bool ChangeRelayState(int blockNumber, int relayNumber, bool relayState)
        {
            uint id = blockDataList[blockNumber].Id;
            if (relayNumber < 0 || relayNumber > 79)
            {
                throw new Exception("Номер реле должен быть в диапазоне от 0 до 79");
            }
            byte stateByte = (byte)(relayState ? 0x01 : 0x00);
            byte[] canMessage = { 0x05, (byte)relayNumber, stateByte, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
            vciDevice.TransmitData(canMessage, id);
            Thread.Sleep(150);
            var answer = GetAnswer(0xFA);
            var returnedRelayNumber = answer[1];
            var status = answer[2];
            if (returnedRelayNumber != (byte)relayNumber)
            {
                EmergencyBreak();
                throw new Exception($"МК не изменил состояние нужных реле: {String.Join(" ", answer)}");
            }
            byte requestedRelayStatus = (byte)(relayState ? 0x01 : 0x00);
            var actualStatus = RequestSingleRelayStatus(blockNumber, relayNumber);
            return requestedRelayStatus == actualStatus;
        }

        private bool CloseRelay(int blockNumber, int relayNumber)
        {
            return ChangeRelayState(blockNumber, relayNumber, true);
        }

        private bool OpenRelay(int blockNumber, int relayNumber)
        {
            return ChangeRelayState(blockNumber, relayNumber, false);
        }

        public bool CloseRelays(int blockNumber, int[] relayNumbers)
        {
            var operationSucceed = true;
            foreach (var relayNumber in relayNumbers)
            {
                var status = CloseRelay(blockNumber, relayNumber);
                operationSucceed = operationSucceed & status;
            }
            return operationSucceed;
        }

        public bool OpenRelays(int blockNumber, int[] relayNumbers)
        {
            var operationSucceed = true;
            foreach (var relayNumber in relayNumbers)
            {
                var status = OpenRelay(blockNumber, relayNumber);
                operationSucceed = operationSucceed & status;
            }
            return operationSucceed;
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
            Thread.Sleep(100);
            var answer = GetAnswer(0xF9);
            var returnedRelayNumber = answer[1];
            var status = answer[2];
            if (returnedRelayNumber == (byte)relayNumber)
            {
                return status;
            }
            throw new Exception($"МК вернул статус не запрашиваемых реле: {String.Join(" ", answer)}");
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
            var answer = GetAnswer(0xF9);
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
            for (int i = 0; i < numberOfBlocks; i++)
            {
                Thread.Sleep(100);
                var answer = GetAnswer(0xF7);
                blockDataList.Add(new BlockData(answer.Identifier, 256 * answer[3] + answer[2]));
            }
            blockDataList.Sort(new BlockDataComparer());
            return blockDataList;         
        }

        #endregion
    }
}
