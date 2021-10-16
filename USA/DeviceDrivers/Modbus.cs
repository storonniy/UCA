using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modbus.Device;
using System.IO.Ports;

namespace UCA.DeviceDrivers
{
    public abstract class Modbus_device
    {//[01 10 0A 00 00 01 02 00 2B 4C 4F] >> [01 10 0A 00 00 01 02 11]
        SerialPort _serialPort;
        IModbusSerialMaster master;
        byte slaveId;
        //public static recive_type device_data;
        public Modbus_device(string portName)
        {
            _serialPort = new SerialPort(portName);

            // configure serial port
            _serialPort.BaudRate = 9600;
            _serialPort.DataBits = 8;
            _serialPort.Parity = Parity.None;
            _serialPort.StopBits = StopBits.One;
            try
            {

                //_serialPort.Open();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            master = ModbusSerialMaster.CreateRtu(_serialPort);
            slaveId = 1;
            //ushort startAddress = 0x0a00;
            //ushort numRegisters = 5;
            // ushort[] write_data = { 0x2b };
            // ushort[] registers = master.ReadHoldingRegisters(slaveId, startAddress, numRegisters);
            //ushort[] registers =  master.ReadWriteMultipleRegisters(slaveId, startAddress, numRegisters, 0x2a, write_data);
            //master.WriteMultipleRegisters(slaveId, startAddress, write_data);

        }
        public bool writeHoldingRegInt(ushort adres, ushort[] write_data)
        {
            if (!_serialPort.IsOpen)
            {
                try
                {
                    _serialPort.Open();
                }
                catch (Exception ex)
                {
                    // MessageBox.Show("Не могу открыть порт "+_serialPort.PortName);
                    return false;
                }
            }
            var ans = master.WriteMultipleRegistersAsync(slaveId, adres, write_data);
            if (ans.IsCompleted)
                return true;
            else
                return false;
        }
        public bool writeHoldingRegFloat(ushort adres, float write_data)
        {
            if (!_serialPort.IsOpen)
            {
                try
                {
                    _serialPort.Open();
                }
                catch (Exception ex)
                {
                    // MessageBox.Show("Не могу открыть порт "+_serialPort.PortName);
                    return false;
                }
            }
            byte[] fData = BitConverter.GetBytes(write_data);
            ushort[] uData = new ushort[4];
            foreach (int i in fData)
            {
                uData[i] = fData[i];
            }
            //ushort[] fData = BitConverter.ToUInt16(BitConverter.GetBytes(write_data),0);
            var ans = master.WriteMultipleRegistersAsync(slaveId, adres, uData);
            if (ans.IsCompleted)
                return true;
            else
                return false;
        }
        public float readHoldingReg(ushort adres, ushort count)
        {
            if (!_serialPort.IsOpen)
            {
                try
                {
                    _serialPort.Open();
                }
                catch (Exception ex)
                {
                    // MessageBox.Show("Не могу открыть порт "+_serialPort.PortName);
                    return 0;
                }
            }
            ushort[] uAns = master.ReadHoldingRegisters(slaveId, adres, count);
            byte[] bAns = new byte[4];
            foreach (int i in uAns)
            {
                bAns[i] = (byte)uAns[i];
            }
            float fAns = BitConverter.ToSingle(bAns, 0);
            return fAns;
        }
        //public static void port_not_open()
        //{
        //    //если не могу открыть порт
        //    device_data.data = "-1";
        //    device_data.status = state.NOT_CONNECTION;
        //    CallBackMy.msg_from_device_Handler(device_data);
        //}

    }
}
