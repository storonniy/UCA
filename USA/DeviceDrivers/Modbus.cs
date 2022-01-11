using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modbus.Device;
using System.IO.Ports;
using System.Threading;

namespace UCA.DeviceDrivers
{
    public abstract class Modbus_device
    {//[01 10 0A 00 00 01 02 00 2B 4C 4F] >> [01 10 0A 00 00 01 02 11]
        SerialPort serialPort;
        IModbusSerialMaster master;
        byte slaveId;
        public Modbus_device(string portName)
        {
            serialPort = new SerialPort(portName);

            // configure serial port
            serialPort.BaudRate = 9600;
            serialPort.DataBits = 8;
            serialPort.Parity = Parity.None;
            serialPort.StopBits = StopBits.One;
            this.master = ModbusSerialMaster.CreateRtu(serialPort);
            this.slaveId = 1;
            serialPort.Open();
        }

        public bool WriteHoldingRegInt(ushort address, ushort[] value)
        {
            var ans = master.WriteMultipleRegistersAsync(slaveId, address, value);
            return ans.IsCompleted;
        }

        public bool WriteHoldingRegFloat(ushort address, float value)
        {
            byte[] fData = BitConverter.GetBytes(value);
            //var fData = new byte[] { 0x40, 0x40 };
            var reversedFData = fData;
            for (int i = 0; i < fData.Length; i++)
            {
                reversedFData[fData.Length - 1 - i] = fData[i];
            }
            ushort[] uData = new ushort[4];
            for (int i = 0; i < fData.Length; i++)
            {
                uData[i] = reversedFData[i];
            }
            //ushort[] fData = BitConverter.ToUInt16(BitConverter.GetBytes(write_data),0);
            var ans = master.WriteMultipleRegistersAsync(slaveId, address, new ushort[] { 0x40, 0x40, 0x40, 0x40 });
            Thread.Sleep(1000);
            return ans.IsCompleted;
        }

        public float ReadHoldingReg(ushort address, ushort count)
        {
            ushort[] uShortAnswer = master.ReadHoldingRegisters(slaveId, address, count);
            byte[] byteAnswer = BitConverter.GetBytes(uShortAnswer[0]);
            byte[] answer = new byte[4];
            var reverse = new byte[byteAnswer.Length];
            for (int i = 0; i < byteAnswer.Length; i++)
            {
                reverse[byteAnswer.Length - i - 1] = byteAnswer[i];
            }
            for (int i = 0; i < reverse.Length; i++)
            {
                answer[answer.Length - 1 - i] = reverse[i];
            }
            return BitConverter.ToSingle(answer, 0);
        }
    }
}
