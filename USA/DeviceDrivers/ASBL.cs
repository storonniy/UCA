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
            ftStatus = device.SetBaudRate(9600);
            if (ftStatus != FT_STATUS.FT_OK)
            {
                throw new Exception("Failed to set Baud rate (error " + ftStatus.ToString() + ")");
            }
            ftStatus = device.SetDataCharacteristics(FT_DATA_BITS.FT_BITS_8, FT_STOP_BITS.FT_STOP_BITS_1, FT_PARITY.FT_PARITY_NONE);
            if (ftStatus != FT_STATUS.FT_OK)
            {
                throw new Exception("Failed to set data characteristics (error " + ftStatus.ToString() + ")");
            }
            ftStatus = device.SetFlowControl(FT_FLOW_CONTROL.FT_FLOW_RTS_CTS, 0x11, 0x13);
            if (ftStatus != FT_STATUS.FT_OK)
            {
                throw new Exception("Failed to set flow control (error " + ftStatus.ToString() + ")");
            }
            ftStatus = device.SetTimeouts(100, 100);
            if (ftStatus != FT_STATUS.FT_OK)
            {
                throw new Exception("Failed to set timeouts (error " + ftStatus.ToString() + ")");
            }

            FT_W_LowPins(lowPinsState, lowPinsDir);
            FT_W_LowPins(lowPinsState, lowPinsDir);
            FT_W_Highpins(highPinsState, highPinsDir);
            FT_W_Highpins(highPinsState, highPinsDir);

        }

        private const int highPinsDir = 0x0C;
        private const int lowPinsDir = 0x6B;
        private byte lowPinsState = 0x68;
        private byte highPinsState = 0x0C;

        public void UnSetLineDirection(uint lineNumber)
        {
            var line = new Line(lineNumber, this);
            line.UnsetDirection();
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

        public void UnSetLineData(uint lineNumber)
        {
            var line = new Line(lineNumber, this);
            line.UnsetData();
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
            var status = device.Write(buffOut, 3, ref numBytesWritten);
            Thread.Sleep(10);
            if (numBytesWritten != 3)
                status = device.Write(buffOut, 3, ref numBytesWritten);
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
            FT_W_Highpins((byte)(highPinsState & 0x0B), highPinsDir);
        }

        /// <summary>
        /// ClrADRn Выставляет признак данных / Снимает признак адреса
        /// </summary>
        private void SetDataFlag()
        {
            FT_W_Highpins((byte)(highPinsState | 0x04), highPinsDir);
        }

        /// <summary>
        /// SetR_Wn выставить признак чтения/снять признак записи
        /// </summary>
        private void SetReadFlag()
        {
            FT_W_Highpins((byte)(highPinsState | 0x08), highPinsDir);
        }

        /// <summary>
        /// ClrR_Wn выставить признак записи / снять признак чтения
        /// </summary>
        private void SetWriteFlag()
        {
            FT_W_Highpins((byte)(highPinsState & 0x07), highPinsDir);
        }

        /// <summary>
        /// SetRSTn выставить сигнал сброса для ПЛИС
        /// </summary>
        private void SetResetFRAGFlag()
        {
            FT_W_LowPins((byte)(lowPinsState & 0xDF), lowPinsDir);
        }

        /// <summary>
        /// ClrRSTn снять сигнал сброса для ПЛИС
        /// </summary>
        private void ClearResetFRAGFlag()
        {
            FT_W_LowPins((byte)(lowPinsState | 0x20), lowPinsDir);
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

    class Line : ASBL
    {
        readonly ASBL device;
        public uint number { get; private set; }
        public uint DirectionRegister { get; private set; }
        public uint DataRegister { get; private set; }

        public uint Position { get; private set; }
        public Line(uint number, ASBL device)
        {
            this.device = device;
            if (number < 1 && number > 120)
                throw new Exception("Номер линии должен быть от 1 до 120");
            this.number = number;
            SetRegisters();
            Position = (number % 20);
        }

        private void Set(uint register)
        {
            var currentData = device.ReadData(register);
            var newData = currentData - (currentData & (uint)Math.Pow(2, Position));
            device.WriteData(register, newData);
        }

        private void UnSet(uint register)
        {
            var currentData = ReadData(register);
            var newData = currentData - (currentData & (uint)Math.Pow(2, Position));
            WriteData(register, newData);
        }

        public void SetDirection()
        {
            Set(DirectionRegister);
        }

        public void UnsetDirection()
        {
            UnSet(DirectionRegister);
        }

        public void SetData()
        {
            Set(DataRegister);
        }

        public void UnsetData()
        {
            UnSet(DataRegister);
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
