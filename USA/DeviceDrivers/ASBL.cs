using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FTD2XX_NET;
using static FTD2XX_NET.FTDI;
using System.Threading;

namespace UPD.DeviceDrivers
{
    public class ASBL
    {
        internal FTDI deviceA;
        internal FTDI deviceB;
        UInt32 ftdiDeviceCount;
        FT_DEVICE_INFO_NODE[] ftdiDeviceList;

        private const int highPinsDir = 0x0C;
        private const int lowPinsDir = 0x6B;
        private byte lowPinsState = 0x68;
        private byte highPinsState = 0x0C;

        /// <summary>
        /// Создать фиктивное устройство (для тестов)
        /// </summary>
        /// <param name="n"></param>
        public ASBL(uint n)
        {

        }

        ~ASBL()
        {
            deviceA.Close();
            deviceB.Close();
        }

        public ASBL()
        {
            deviceA = new FTDI();
            // Determine the number of FTDI devices connected to the machine
            var ftStatus = deviceA.GetNumberOfDevices(ref ftdiDeviceCount);
            if (ftStatus != FT_STATUS.FT_OK)
            {
                throw new Exception("Failed to get number of devices (error " + ftStatus.ToString() + ")");
            }
            // Allocate storage for device info list
            ftdiDeviceList = new FT_DEVICE_INFO_NODE[ftdiDeviceCount];
            // Populate our device list
            ftStatus = deviceA.GetDeviceList(ftdiDeviceList);
            if (ftStatus != FT_STATUS.FT_OK)
                throw new Exception("Failed to populate device list");
            // Open first device in our list by serial number
            ftStatus = deviceA.OpenByIndex(0);//OpenBySerialNumber(ftdiDeviceList[0].SerialNumber);
            if (ftStatus != FT_STATUS.FT_OK)
            {
                throw new Exception("Failed to open device (error " + ftStatus.ToString() + ")");
            }
            //Check("SetBaudRate", () => device.SetBaudRate(9600));
            Check("SetDataCharacteristics", () => deviceA.SetDataCharacteristics(FT_DATA_BITS.FT_BITS_8, FT_STOP_BITS.FT_STOP_BITS_1, FT_PARITY.FT_PARITY_NONE));
            Check("SetFlowControl", () => deviceA.SetFlowControl(FT_FLOW_CONTROL.FT_FLOW_RTS_CTS, 0x11, 0x13));
            Check("SetTimeouts", () => deviceA.SetTimeouts(100, 100));
            Check("ResetDevice()", () => deviceA.ResetDevice());
            // настраиваем канал А найденного адаптера как надо
            uint numBytesWritten = 0;
            Check("deviceA.Write(new byte[] { 0x00 }, 1, ref numBytesWritten);", () => deviceA.Write(new byte[] { 0x00 }, 1, ref numBytesWritten));
            if (numBytesWritten != 1)
                throw new Exception($"Записано {numBytesWritten} вместо 1");
            Check("ResetDevice()", () => deviceA.ResetDevice());
            Check("SetLatency", () => deviceA.SetLatency(16));
            // Сброс контроллера MPSSE в канале А м/с FTDI*****************
            Check("ResetDevice()", () => deviceA.ResetDevice());
            Check("FT_ResetController(deviceA)", () => FT_ResetController(deviceA));
            Check("FT_EnableJTAGController(deviceA)", () => FT_EnableJTAGController(deviceA));
            Check("ResetDevice()", () => deviceA.ResetDevice());
            lowPinsState = 0x68;
            FT_W_LowPins(lowPinsState, lowPinsDir);
            lowPinsState = 0x68;
            FT_W_LowPins(lowPinsState, lowPinsDir);
            highPinsState = 0x0C;
            FT_W_Highpins(highPinsState, highPinsDir);
            highPinsState = 0x0C;
            FT_W_Highpins(highPinsState, highPinsDir);
            // находим в сипске устройств FTDI адаптеры с нужными серийными номерами и заполянем значение хэндла для канала B
            deviceB = new FTDI();
            deviceB.OpenByIndex(1);
            // устанавливаем время ожидания окончания записи/чтения 100мс
            Check("SetTimeouts()", () => deviceB.SetTimeouts(100, 100));
            // пустая передача байта**************************************
            // для решения проблемы зависания после первого включения
            uint count = 0;
            Check("deviceB.Write(new byte[] { 0x00 }, 1, ref numBytesWritten);", () => deviceB.Write(new byte[] { 0x00 }, 1, ref count));
            // Сброс канала В м/с FTDI ************************************
            deviceB.ResetDevice();
            AS_ResetFPGA();
        }

        private void AS_ResetFPGA()
        {
            deviceB.ResetDevice();
            SetRSTn();
            Thread.Sleep(1);
            ClrRSTn();         
        }

        private void Check(string errorMsg, Func<FT_STATUS> command)
        {
            var status = command();
            if (status != FT_STATUS.FT_OK)
                throw new Exception($"{errorMsg} : {status}");
        }

        private FT_STATUS FT_ResetController(FTDI dev)
        {
            var status = dev.SetBitMode(0, 0);
            return status;
        }

        private FT_STATUS FT_EnableJTAGController(FTDI dev)
        {
            var status = dev.SetBitMode(0, 2);
            return status;
        }

        public void ClearLineDirection(uint lineNumber)
        {
            var line = new Line(lineNumber, this);
            line.ClearDirection();
        }

        public void ClearLineDirection(params uint[] lineNumbers)
        {
            foreach (var lineNumber in lineNumbers)
            {
                var line = new Line(lineNumber, this);
                line.ClearDirection();
            }
        }

        public void SetLineDirection(uint lineNumber)
        {
            var line = new Line(lineNumber, this);
            line.SetDirection();
        }

        public void SetLineData(uint lineNumber)
        {
            var line = new Line(lineNumber, this);
            line.SetData();
        }

        public void ClearLineData(uint lineNumber)
        {
            var line = new Line(lineNumber, this);
            line.ClearData();
        }

        public void ClearAll()
        {
            uint dirState = 0xFFFFF;
            WriteData(Line.ADR_DIR_REG1, dirState);
            WriteData(Line.ADR_DIR_REG2, dirState);
            WriteData(Line.ADR_DIR_REG3, dirState);
            WriteData(Line.ADR_DIR_REG4, dirState);
            WriteData(Line.ADR_DIR_REG5, dirState);
            WriteData(Line.ADR_DIR_REG6, dirState);
            uint dataState = 0;
            WriteData(Line.ADR_DATA_REG1, dataState);
            WriteData(Line.ADR_DATA_REG2, dataState);
            WriteData(Line.ADR_DATA_REG3, dataState);
            WriteData(Line.ADR_DATA_REG4, dataState);
            WriteData(Line.ADR_DATA_REG5, dataState);
            WriteData(Line.ADR_DATA_REG6, dataState);
            var readData = ReadData(Line.ADR_DATA_REG1);
            var dirData = ReadData(Line.ADR_DIR_REG1);
            if (readData != dataState || dirData != dirState)
                throw new Exception($"Хьюстон, у нас проблемы: readData = {readData}, expected {dataState}; dirdata = {dirData}, expected {dirState}");
        }

        private enum RegisterType
        {
            Direction,
            Data
        }

        public void WriteData(uint address, uint data)
        {
            SetADRn(); // SetADRn
            ClrR_Wn(); // ClrR_Wn
            uint numBytesWritten = 0;
            var addressBuffer = GetFilledBuffer(address);
            var addrStatus = deviceB.Write(addressBuffer, addressBuffer.Length, ref numBytesWritten);
            if (addrStatus != FT_STATUS.FT_OK)
                throw new Exception($"АСБЛ: операция записи {address} завершилась с ошибкой");
            ClrADRn(); // ClrADRn
            var dataBuffer = GetFilledBuffer(data);
            var dataStatus = deviceB.Write(dataBuffer, dataBuffer.Length, ref numBytesWritten);
            if (dataStatus != FT_STATUS.FT_OK)
                throw new Exception($"АСБЛ: операция записи {data} завершилась с ошибкой");
            SetR_Wn(); // SetR_Wn
        }

        public uint ReadData(uint address)
        {
            SetADRn(); //SetADRn
            SetR_Wn(); // SetR_Wn
            uint numBytesWritten = 0;
            var addressBuffer = GetFilledBuffer(address);
            var addrStatus = deviceB.Write(addressBuffer, addressBuffer.Length, ref numBytesWritten);
            if (addrStatus != FT_STATUS.FT_OK)
                throw new Exception($"АСБЛ: операция записи {address} завершилась с ошибкой");
            ClrADRn(); //ClrADRn
            // ждём данные из ПЛИС
            Thread.Sleep(10);
            uint numBytesRead = 0;
            var buffer = new byte[12];
            var readStatus = deviceB.Read(buffer, (uint)buffer.Length, ref numBytesRead);
            if (readStatus != FT_STATUS.FT_OK)
                throw new Exception($"АСБЛ: операция чтения завершилась с ошибкой");
            uint data = 0;
            for (int i = 0; i < buffer.Length; i++)
            {
                data += (uint)buffer[i] << (i * 8);
            }
            return data;
        }

        private void FT_W_Highpins(byte valPin, byte dirPin)
        {
            //var deviceList = d2xx.GetDeviceList(new FT_DEVICE_INFO_NODE[] { new FT_DEVICE_INFO_NODE() });
            Thread.Sleep(100);
            var buffer = new byte[] { 0x82, (byte)(valPin & 0xF), (byte)(dirPin & 0xF) };
            FT_W(buffer);
/*            uint numBytesWritten = 0;
            var status = deviceA.Write(buffer, buffer.Length, ref numBytesWritten);
            Thread.Sleep(10);
            if (numBytesWritten != 3)
                status = deviceA.Write(buffer, buffer.Length, ref numBytesWritten);
            if (numBytesWritten != 3)
                throw new Exception($"Записано {numBytesWritten} вместо 3");
            if (status != FT_STATUS.FT_OK)
                throw new Exception(status.ToString());*/
        }

        private void FT_W_LowPins(byte valPin, byte dirPin)
        {
            var buffOut = new byte[] { 0x80, valPin, dirPin };
            FT_W(buffOut);
/*            uint numBytesWritten = 0;
            var status = deviceA.Write(buffOut, buffOut.Length, ref numBytesWritten);
            Thread.Sleep(10);
            if (numBytesWritten != 3)
                status = deviceA.Write(buffOut, buffOut.Length, ref numBytesWritten);
            if (numBytesWritten != 3)
                throw new Exception($"Записано {numBytesWritten} вместо 3");
            if (status != FT_STATUS.FT_OK)
                throw new Exception(status.ToString());*/
        }

        private void FT_W(byte[] buffer)
        {
            uint numBytesWritten = 0;
            Thread.Sleep(10);
            var status = deviceA.Write(buffer, buffer.Length, ref numBytesWritten);
            Thread.Sleep(10);
            if (numBytesWritten != 3)
                status = deviceA.Write(buffer, buffer.Length, ref numBytesWritten);
            if (numBytesWritten != 3)
                throw new Exception($"Записано {numBytesWritten} вместо 3");
            if (status != FT_STATUS.FT_OK)
                throw new Exception(status.ToString());
        }

        /// <summary>
        /// SetADRn Выставляет признак адреса / Снимает признак данных
        /// </summary>
        private void SetADRn()
        {
            highPinsState = (byte)(highPinsState & 0x0B);
            FT_W_Highpins((byte)(highPinsState), highPinsDir);
        }

        /// <summary>
        /// ClrADRn Выставляет признак данных / Снимает признак адреса
        /// </summary>
        private void ClrADRn()
        {
            highPinsState = (byte)(highPinsState | 0x04);
            FT_W_Highpins((byte)(highPinsState), highPinsDir);
        }

        /// <summary>
        /// SetR_Wn выставить признак чтения/снять признак записи
        /// </summary>
        private void SetR_Wn()
        {
            highPinsState = (byte)(highPinsState | 0x08);
            FT_W_Highpins((byte)(highPinsState), highPinsDir);
        }

        /// <summary>
        /// ClrR_Wn выставить признак записи / снять признак чтения
        /// </summary>
        private void ClrR_Wn()
        {
            highPinsState = (byte)(highPinsState & 0x07);
            FT_W_Highpins(highPinsState, highPinsDir);
        }

        /// <summary>
        /// SetRSTn выставить сигнал сброса для ПЛИС
        /// </summary>
        private void SetRSTn()
        {
            lowPinsState = (byte)(lowPinsState & 0xDF);
            FT_W_LowPins(lowPinsState, lowPinsDir);
        }

        /// <summary>
        /// ClrRSTn снять сигнал сброса для ПЛИС
        /// </summary>
        private void ClrRSTn()
        {
            lowPinsState = (byte)(lowPinsState | 0x20);
            FT_W_LowPins(lowPinsState, lowPinsDir);
        }

        private byte[] GetFilledBuffer(uint data)
        {
            var buf = BitConverter.GetBytes(data);
            return buf;
        }
    }

    public class Line 
    {
        readonly ASBL asbl;
        public uint number { get; private set; }
        public uint DirectionRegister { get; private set; }
        public uint DataRegister { get; private set; }

        public uint Position { get; private set; }
        public Line(uint number, ASBL asbl)
        {
            this.asbl = asbl;           
            if (number < 1 || number > 120)
                throw new Exception("Номер линии должен быть от 1 до 120");
            this.number = number;
            SetRegisters();
            Position = (number % 20) - 1;
        }

        public static Func<uint, uint> getPowerOfTwo = (degree) => (uint)(1 << (int)degree);
        private void Set(uint register)
        {
            uint currentData = asbl.ReadData(register);
            uint newData = currentData | getPowerOfTwo(Position);
            asbl.WriteData(register, newData);
        }

        private void Clear(uint register)
        {
            uint currentData = asbl.ReadData(register);
            uint newData = currentData - (currentData & getPowerOfTwo(Position));
            asbl.WriteData(register, newData);
        }

        public void SetDirection()
        {
            Set(DirectionRegister);
            uint writtenData = asbl.ReadData(DirectionRegister);
            if ((writtenData & (1 << (int)Position)) == 0)
                throw new Exception($"Не удалось выставить линию {number} в 1 (на выдачу)");
        }

        public void ClearDirection()
        {
            Clear(DirectionRegister);
            uint writtenData = asbl.ReadData(DirectionRegister);
            if ((writtenData & (1 << (int)Position)) == 1)
                throw new Exception($"Не удалось выставить линию {number} в 0 (на прием)");
        }

        public void SetData()
        {
            uint dirRegData = asbl.ReadData(DirectionRegister);
            if ((dirRegData & (1 << (int)Position)) == 0)
                throw new Exception($"Попытка выставить в 1 линию {number}, которая настроена на приём");
            Set(DataRegister);
            uint writtenData = asbl.ReadData(DataRegister);
            if ((writtenData & (1 << (int)Position)) == 0)
                throw new Exception($"Не удалось выставить линию {number} в 1");
        }

        public void ClearData()
        {
            uint dirRegData = asbl.ReadData(DirectionRegister);
            if ((dirRegData & (1 << (int)Position)) == 0)
                throw new Exception($"Попытка выставить в 0 линию {number}, которая настроена на приём");
            Clear(DataRegister);
            uint writtenData = asbl.ReadData(DataRegister);
            if ((writtenData & (1 << (int)Position)) == 1)
                throw new Exception($"Не удалось выставить линию {number} в 0");
        }

        private void SetRegisters()
        {
            if (number > 0 && number < 21)
            {
                DirectionRegister = ADR_DIR_REG1;
                DataRegister = ADR_DATA_REG1;
                return;
            }
            if (number < 41)
            {
                DirectionRegister = ADR_DIR_REG2;
                DataRegister = ADR_DATA_REG2;
                return;
            }
            if (number < 61)
            {
                DirectionRegister = ADR_DIR_REG3;
                DataRegister = ADR_DATA_REG3;
                return;
            }
            if (number < 81)
            {
                DirectionRegister = ADR_DIR_REG4;
                DataRegister = ADR_DATA_REG4;
                return;
            }
            if (number < 101)
            {
                DirectionRegister = ADR_DIR_REG5;
                DataRegister = ADR_DATA_REG5;
                return;
            }
            if (number < 121)
            {
                DirectionRegister = ADR_DIR_REG6;
                DataRegister = ADR_DATA_REG6;
                return;
            }
        }
        /// <summary>
        /// Управление направлением линий I/O1…I/O20 (записать «1» в разряд – настроить линию на выход, «0» - на вход)
        /// </summary>
        public const uint ADR_DIR_REG1 = 0x00000000;
        /// <summary>
        /// Управление направлением линий I/O21…I/O40
        /// </summary>
        public const uint ADR_DIR_REG2 = 0x00000001;
        /// <summary>
        /// Хранение состояния линий I/O1…I/O20. Записав «1» на соответствующей линии (если она настроена на выход) будет выставлена «1»
        /// Записав «0» на соответствующей линии(если она настроена на выход) будет выставлен «0». 
        /// При чтение по этому адресу возвращается текущее состояние линий, если на линию подана снаружи или выставлена «1»  в соответствующем разряде будет «1»
        /// </summary>
        public const uint ADR_DATA_REG1 = 0x00000002;
        /// <summary>
        /// Хранение состояния линий I/O21…I/O40.
        /// </summary>
        public const uint ADR_DATA_REG2 = 0x00000003;
        /// <summary>
        /// Управление направлением линий I/O41…I/O60
        /// </summary>
        public const uint ADR_DIR_REG3 = 0x01000000;
        /// <summary>
        /// Управление направлением линий I/O61…I/O80
        /// </summary>
        public const uint ADR_DIR_REG4 = 0x01000001;
        /// <summary>
        /// Управление состоянием линий I/O41…I/O60
        /// </summary>
        public const uint ADR_DATA_REG3 = 0x01000002;
        /// <summary>
        /// Управление состоянием линий I/O61…I/O80
        /// </summary>
        public const uint ADR_DATA_REG4 = 0x01000003;
        /// <summary>
        /// Управление направлением линий I/O81…I/O100
        /// </summary>
        public const uint ADR_DIR_REG5 = 0x02000000;
        /// <summary>
        /// Управление направлением линий I/O101…I/O120
        /// </summary>
        public const uint ADR_DIR_REG6 = 0x02000001;
        /// <summary>
        /// Управление состоянием линий I/O81…I/O100
        /// </summary>
        public const uint ADR_DATA_REG5 = 0x02000002;
        /// <summary>
        /// Управление состоянием линий I/O101…I/O120
        /// </summary>
        public const uint ADR_DATA_REG6 = 0x02000003;
    }
}
