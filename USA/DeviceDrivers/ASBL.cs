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
        FTDI device;
        UInt32 ftdiDeviceCount;
        FT_DEVICE_INFO_NODE[] ftdiDeviceList;

        private const int highPinsDir = 0x0C;
        private const int lowPinsDir = 0x6B;
        public ASBL()
        {
            device = new FTDI();
            // Determine the number of FTDI devices connected to the machine
            var ftStatus = device.GetNumberOfDevices(ref ftdiDeviceCount);
            if (ftStatus != FT_STATUS.FT_OK)
            {
                throw new Exception("Failed to get number of devices (error " + ftStatus.ToString() + ")");
            }
            // Allocate storage for device info list
            ftdiDeviceList = new FT_DEVICE_INFO_NODE[ftdiDeviceCount];
            // Populate our device list
            ftStatus = device.GetDeviceList(ftdiDeviceList);
            if (ftStatus != FT_STATUS.FT_OK)
                throw new Exception("Failed to populate device list");
            // Open first device in our list by serial number
            ftStatus = device.OpenBySerialNumber(ftdiDeviceList[0].SerialNumber);
            if (ftStatus != FT_STATUS.FT_OK)
            {
                throw new Exception("Failed to open device (error " + ftStatus.ToString() + ")");
            }
            Check("SetBaudRate", () => device.SetBaudRate(9600));
            Check("SetDataCharacteristics", () => device.SetDataCharacteristics(FT_DATA_BITS.FT_BITS_8, FT_STOP_BITS.FT_STOP_BITS_1, FT_PARITY.FT_PARITY_NONE));
            Check("SetFlowControl", () => device.SetFlowControl(FT_FLOW_CONTROL.FT_FLOW_RTS_CTS, 0x11, 0x13));
            Check("SetTimeouts", () => device.SetTimeouts(100, 100));
            // настраиваем канал А найденного адаптера как надо
            Check("ResetDevice()", () => device.ResetDevice());
            Check("SetLatency", () => device.SetLatency(16));
            Check("ResetDevice()", () => device.ResetDevice());
            Thread.Sleep(100);
            FT_W_LowPins(lowPinsState, lowPinsDir);
            FT_W_LowPins(lowPinsState, lowPinsDir);
            FT_W_Highpins(highPinsState, highPinsDir);
            FT_W_Highpins(highPinsState, highPinsDir);

        }
        ~ASBL()
        {
            device.Close();
        }

        private void Check(string errorMsg, Func<FT_STATUS> command)
        {
            var status = command();
            if (status != FT_STATUS.FT_OK)
                throw new Exception($"{errorMsg} : {status}");
        }

        private byte lowPinsState = 0x68;
        private byte highPinsState = 0x0C;

        private void ResetController()
        {
            var status = device.SetBitMode(0, 0);
            if (status != FT_STATUS.FT_OK)
                throw new Exception($"SetBitMode завершился с ошибкой {status}");
        }

        private void EnableJtagController()
        {
            var status = device.SetBitMode(0, 2);
            if (status != FT_STATUS.FT_OK)
                throw new Exception($"SetBitMode завершился с ошибкой {status}");
        }

        public void ClearLineDirection(uint lineNumber)
        {
            var line = new Line(lineNumber, this);
            line.ClearDirection();
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

        private enum RegisterType
        {
            Direction,
            Data
        }

        public void WriteData(uint address, uint data)
        {
            SetAddressFlag(); // SetADRn
            SetWriteFlag(); // ClrR_Wn
            uint numBytesWritten = 0;
            var addressBuffer = GetFilledBuffer(address);
            var addrStatus = device.Write(addressBuffer, addressBuffer.Length, ref numBytesWritten);
            if (addrStatus != FT_STATUS.FT_OK)
                throw new Exception($"АСБЛ: операция записи {address} завершилась с ошибкой");
            SetDataFlag(); // ClrADRn
            var dataBuffer = GetFilledBuffer(data);
            var dataStatus = device.Write(dataBuffer, dataBuffer.Length, ref numBytesWritten);
            if (dataStatus != FT_STATUS.FT_OK)
                throw new Exception($"АСБЛ: операция записи {data} завершилась с ошибкой");
            SetReadFlag(); // SetR_Wn
        }

        public uint ReadData(uint address)
        {
            SetAddressFlag(); //SetADRn
            SetReadFlag(); // SetR_Wn
            uint numBytesWritten = 0;
            var addressBuffer = GetFilledBuffer(address);
            var addrStatus = device.Write(addressBuffer, addressBuffer.Length, ref numBytesWritten);
            if (addrStatus != FT_STATUS.FT_OK)
                throw new Exception($"АСБЛ: операция записи {address} завершилась с ошибкой");
            SetDataFlag(); //ClrADRn
            // ждём данные из ПЛИС
            Thread.Sleep(10);
            uint numBytesRead = 0;
            var buffer = new byte[4];
            var readStatus = device.Read(buffer, (uint)buffer.Length, ref numBytesRead);
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
            var buffer = new byte[] { 0x82, (byte)(valPin & 0xF), (byte)(dirPin & 0xF) };
            Thread.Sleep(10);
            uint numBytesWritten = 0;
            var status = device.Write(buffer, 3, ref numBytesWritten);
            if (numBytesWritten != 3)
                status = device.Write(buffer, 3, ref numBytesWritten);
            if (numBytesWritten != 3)
                throw new Exception($"Записано {numBytesWritten} вместо 3");
            if (status != FT_STATUS.FT_OK)
                throw new Exception(status.ToString());
        }

        private void FT_W_LowPins(byte valPin, byte dirPin)
        {
            var buffOut = new byte[] { 0x80, valPin, dirPin };
            Thread.Sleep(10);
            uint numBytesWritten = 0;
            var status = device.Write(buffOut, buffOut.Length, ref numBytesWritten);
            Thread.Sleep(10);
            if (numBytesWritten != 3)
                status = device.Write(buffOut, buffOut.Length, ref numBytesWritten);
            if (numBytesWritten != 3)
                throw new Exception($"Записано {numBytesWritten} вместо 3");
            if (status != FT_STATUS.FT_OK)
                throw new Exception(status.ToString());
        }

        /// <summary>
        /// SetADRn Выставляет признак адреса / Снимает признак данных
        /// </summary>
        private void SetAddressFlag()
        {
            highPinsState &= 0x0B;
            FT_W_Highpins(highPinsState, highPinsDir);
        }

        /// <summary>
        /// ClrADRn Выставляет признак данных / Снимает признак адреса
        /// </summary>
        private void SetDataFlag()
        {
            highPinsState |= 0x04;
            FT_W_Highpins(highPinsState, highPinsDir);
        }

        /// <summary>
        /// SetR_Wn выставить признак чтения/снять признак записи
        /// </summary>
        private void SetReadFlag()
        {
            highPinsState |= 0x08;
            FT_W_Highpins(highPinsState, highPinsDir);
        }

        /// <summary>
        /// ClrR_Wn выставить признак записи / снять признак чтения
        /// </summary>
        private void SetWriteFlag()
        {
            highPinsState &= 0x07;
            FT_W_Highpins(highPinsState, highPinsDir);
        }

        /// <summary>
        /// SetRSTn выставить сигнал сброса для ПЛИС
        /// </summary>
        private void SetResetFRAGFlag()
        {
            lowPinsState &= 0xDF;
            FT_W_LowPins(lowPinsState, lowPinsDir);
        }

        /// <summary>
        /// ClrRSTn снять сигнал сброса для ПЛИС
        /// </summary>
        private void ClearResetFRAGFlag()
        {
            lowPinsState |= 0x20;
            FT_W_LowPins(lowPinsState, lowPinsDir);
        }

        private byte[] GetFilledBuffer(uint data)
        {
            var dataBuffer = new byte[4];
            for (int i = 0; i < dataBuffer.Length; i++)
            {
                dataBuffer[i] = (byte)((byte)(data & (0xFF << (8 * i))) >> (8 * i));
            }
            return dataBuffer;
        }
    }

    public class Line 
    {
        readonly ASBL device;
        public uint number { get; private set; }
        public uint DirectionRegister { get; private set; }
        public uint DataRegister { get; private set; }

        public uint Position { get; private set; }
        public Line(uint number, ASBL device)
        {
            this.device = device;
            if (number < 1 || number > 120)
                throw new Exception("Номер линии должен быть от 1 до 120");
            this.number = number;
            SetRegisters();
            Position = (number % 20) - 1;
        }

        public static Func<uint, uint> getPowerOfTwo = (degree) => (uint)(1 << (int)degree);
        private void Set(uint register)
        {
            var currentData = device.ReadData(register);
            var newData = currentData | getPowerOfTwo(Position);
            device.WriteData(register, newData);
        }

        private void Clear(uint register)
        {
            var currentData = device.ReadData(register);
            var newData = currentData - (currentData & getPowerOfTwo(Position));
            device.WriteData(register, newData);
        }



        public void SetDirection()
        {
            Set(DirectionRegister);
        }

        public void ClearDirection()
        {
            Clear(DirectionRegister);
        }

        public void SetData()
        {
            Set(DataRegister);
        }

        public void ClearData()
        {
            Clear(DataRegister);
        }

        private void SetRegisters()
        {
            if (number > 0 && number < 21)
            {
                DirectionRegister = ADR_DIR_REG1;
                DataRegister = ADR_DATA_REG1;
            }
            else if (number < 41)
            {
                DirectionRegister = ADR_DIR_REG2;
                DataRegister = ADR_DATA_REG2;
            }
            else if (number < 61)
            {
                DirectionRegister = ADR_DIR_REG3;
                DataRegister = ADR_DATA_REG3;
            }
            else if (number < 81)
            {
                DirectionRegister = ADR_DIR_REG4;
                DataRegister = ADR_DATA_REG4;
            }
            else if (number < 101)
            {
                DirectionRegister = ADR_DIR_REG5;
                DataRegister = ADR_DATA_REG5;
            }
            else if (number < 121)
            {
                DirectionRegister = ADR_DIR_REG6;
                DataRegister = ADR_DATA_REG6;
            }
        }
        /// <summary>
        /// Управление направлением линий I/O1…I/O20 (записать «1» в разряд – настроить линию на выход, «0» - на вход)
        /// </summary>
        private const uint ADR_DIR_REG1 = 0x00000000;
        /// <summary>
        /// Управление направлением линий I/O21…I/O40
        /// </summary>
        private const uint ADR_DIR_REG2 = 0x00000001;
        /// <summary>
        /// Хранение состояния линий I/O1…I/O20. Записав «1» на соответствующей линии (если она настроена на выход) будет выставлена «1»
        /// Записав «0» на соответствующей линии(если она настроена на выход) будет выставлен «0». 
        /// При чтение по этому адресу возвращается текущее состояние линий, если на линию подана снаружи или выставлена «1»  в соответствующем разряде будет «1»
        /// </summary>
        private const uint ADR_DATA_REG1 = 0x00000002;
        /// <summary>
        /// Хранение состояния линий I/O21…I/O40.
        /// </summary>
        private const uint ADR_DATA_REG2 = 0x00000003;
        /// <summary>
        /// Управление направлением линий I/O41…I/O60
        /// </summary>
        private const uint ADR_DIR_REG3 = 0x01000000;
        /// <summary>
        /// Управление направлением линий I/O61…I/O80
        /// </summary>
        private const uint ADR_DIR_REG4 = 0x01000001;
        /// <summary>
        /// Управление состоянием линий I/O41…I/O60
        /// </summary>
        private const uint ADR_DATA_REG3 = 0x01000002;
        /// <summary>
        /// Управление состоянием линий I/O61…I/O80
        /// </summary>
        private const uint ADR_DATA_REG4 = 0x01000003;
        /// <summary>
        /// Управление направлением линий I/O81…I/O100
        /// </summary>
        private const uint ADR_DIR_REG5 = 0x02000000;
        /// <summary>
        /// Управление направлением линий I/O101…I/O120
        /// </summary>
        private const uint ADR_DIR_REG6 = 0x02000001;
        /// <summary>
        /// Управление состоянием линий I/O81…I/O100
        /// </summary>
        private const uint ADR_DATA_REG5 = 0x02000002;
        /// <summary>
        /// Управление состоянием линий I/O101…I/O120
        /// </summary>
        private const uint ADR_DATA_REG6 = 0x02000003;
    }
}
