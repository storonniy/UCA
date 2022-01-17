﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;

namespace UCA.DeviceDrivers
{
    public class ATH_8030 : Modbus_device
    {
        public ATH_8030 (string portName) : base(portName)
        {

        }

        public bool SetCurrentControlMode()
        {
            var cmd = new ushort[] { 1 };
            var meow = WriteHoldingRegInt(0x0A00, cmd);
            return WriteHoldingRegInt(0x0A00, cmd);
        }

        public bool PowerOn()
        {
            var cmdArr = new ushort[] { 0x2a };
            return WriteHoldingRegInt(0x0A00, cmdArr);
        }

        public bool PowerOff()
        {
            var cmdArr = new ushort[] { 0x2b };
            return WriteHoldingRegInt(0x0A00, cmdArr);
        }

        public bool SetCurrent(float current)
        {
            return WriteHoldingRegFloat(0x0A01, current);
        }

        public bool SetMaxCurrent(float current)
        {
            return WriteHoldingRegFloat(0x0A34, current);
        }

        public bool SetConstantCurrent(float current)
        {
            return WriteHoldingRegFloat(0x0A01, current);
        }

        public double GetLoadCurrent()
        {
            Thread.Sleep(500);
            var current = ReadHoldingReg(0x0B02, 2);
            return current;
        }

        public double GetConstantCurrent()
        {
            var current = ReadHoldingReg(0x0A01, 2);
            return current;
        }

        public double GetMaxCurrent()
        {
            var current = ReadHoldingReg(0x0A34, 1);
            return current;
        }
    }
}
