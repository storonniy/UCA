using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using FTD2XX_NET;
using UPD.DeviceDrivers;
using UCA.DeviceDrivers;
using System.IO.Ports;

namespace Tester
{
    [TestClass]
    public class ASBLTests
    {
        [TestMethod]
        public void InitAKIP()
        {
            var serialPort = new SerialPort("COM5", 9600);
            var akip = new AKIP_3407(serialPort);
            Assert.AreEqual(3.24, akip.SetVoltage(3.24));
        }


        /// <summary>
        /// Показывает, что массив - это ссылочный тип
        /// </summary>
        [TestMethod]
        public void ChangeArrayTest()
        {
            var bytes = new byte[] { 5, 1, 2, 4, 3};
            var extpectedBytes = new byte[] { 0, 1, 2, 3, 4 };
            ChangeArray(bytes);
            CollectionAssert.AreEqual(extpectedBytes, bytes);
        }

        public void ChangeArray(byte[] bytes)
        {
            for (int i = 0; i < bytes.Length; i++)
                bytes[i] = (byte)i;
        }

        [TestMethod]
        public void ExponentiateTest()
        {
            Assert.AreEqual((uint)4, Line.getPowerOfTwo(2));
            Assert.AreEqual((uint)8, Line.getPowerOfTwo(3));
            Assert.AreEqual((uint)512, Line.getPowerOfTwo(9));
            Assert.AreEqual((uint)2048, Line.getPowerOfTwo(11));
        }

        [TestMethod]
        public void ClearLineTest()
        {
            uint currentData = 24965;
            uint newData = currentData - (currentData & Line.getPowerOfTwo(2));
            Assert.AreEqual("110000110000001", Convert.ToString(newData, 2));
            newData = currentData - (currentData & Line.getPowerOfTwo(3));
            Assert.AreEqual("110000110000101", Convert.ToString(newData, 2));
        }

        [TestMethod]
        public void SetLineTest()
        {
            uint currentData = 24965;
            var newData = currentData | Line.getPowerOfTwo(3);
            Assert.AreEqual("110000110001101", Convert.ToString(newData, 2));
            newData = currentData | Line.getPowerOfTwo(2);
            Assert.AreEqual("110000110000101", Convert.ToString(newData, 2));
        }

        [TestMethod]
        public void TestASBL()
        {
            var asbl = new ASBL();
            asbl.SetLineDirection(97);
        }

        [TestMethod]
        public void TestLinesParameters()
        {
            TestNewLineParameters((uint)1, Line.ADR_DIR_REG1, Line.ADR_DATA_REG1);
            TestNewLineParameters((uint)21, Line.ADR_DIR_REG2, Line.ADR_DATA_REG2);
            TestNewLineParameters((uint)41, Line.ADR_DIR_REG3, Line.ADR_DATA_REG3);
            TestNewLineParameters((uint)61, Line.ADR_DIR_REG4, Line.ADR_DATA_REG4);
            TestNewLineParameters((uint)81, Line.ADR_DIR_REG5, Line.ADR_DATA_REG5);
            TestNewLineParameters((uint)101, Line.ADR_DIR_REG6, Line.ADR_DATA_REG6);
        }

        public void TestNewLineParameters(uint firstLineNumber, uint expectedDirectionRegister, uint expectedDataRegister)
        {
            uint linePosition = 0;
            for (uint lineNumber = firstLineNumber; lineNumber < firstLineNumber + 19; lineNumber++)
            {
                var line = new Line(lineNumber, new ASBL(0));
                Assert.AreEqual(line.DirectionRegister, expectedDirectionRegister);
                Assert.AreEqual(line.DataRegister, expectedDataRegister);
                Assert.AreEqual(linePosition, line.Position);
                linePosition++;
            }
        }

    }
}
