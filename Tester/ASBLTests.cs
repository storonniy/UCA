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
        public void TestWriteRead()
        {
            TestASBL(0, 10);
            TestASBL(1048565, 1048575);
        }

        ASBL asbl = new ASBL();

        public void TestASBL(uint start, uint end)
        {
            for (uint writeData = start; writeData < end + 1; writeData++)
            {
                asbl.WriteData(Line.ADR_DIR_REG1, writeData);
                uint readData = asbl.ReadData(Line.ADR_DIR_REG1);
                Assert.AreEqual(writeData, readData);
                asbl.WriteData(Line.ADR_DIR_REG2, writeData);
                readData = asbl.ReadData(Line.ADR_DIR_REG2);
                Assert.AreEqual(writeData, readData);
                asbl.WriteData(Line.ADR_DIR_REG3, writeData);
                readData = asbl.ReadData(Line.ADR_DIR_REG3);
                Assert.AreEqual(writeData, readData);
                asbl.WriteData(Line.ADR_DIR_REG4, writeData);
                readData = asbl.ReadData(Line.ADR_DIR_REG4);
                Assert.AreEqual(writeData, readData);
                asbl.WriteData(Line.ADR_DIR_REG5, writeData);
                readData = asbl.ReadData(Line.ADR_DIR_REG5);
                Assert.AreEqual(writeData, readData);
                asbl.WriteData(Line.ADR_DIR_REG6, writeData);
                readData = asbl.ReadData(Line.ADR_DIR_REG6);
                Assert.AreEqual(writeData, readData);
            }
        }

        [TestMethod]
        public void TestWriteReadDataReg()
        {
            TestDataRegisters(0, 10);
            //TestASBL(1048565, 1048575);
        }

        public void TestDataRegisters(uint start, uint end)
        {
            for (uint writeData = start; writeData < end + 1; writeData++)
            {
                asbl.WriteData(Line.ADR_DATA_REG1, writeData);
                uint readData = asbl.ReadData(Line.ADR_DATA_REG1);
                Assert.AreEqual(writeData, readData);
                asbl.WriteData(Line.ADR_DATA_REG2, writeData);
                readData = asbl.ReadData(Line.ADR_DATA_REG2);
                Assert.AreEqual(writeData, readData);
                asbl.WriteData(Line.ADR_DATA_REG3, writeData);
                readData = asbl.ReadData(Line.ADR_DATA_REG3);
                Assert.AreEqual(writeData, readData);
                asbl.WriteData(Line.ADR_DATA_REG4, writeData);
                readData = asbl.ReadData(Line.ADR_DATA_REG4);
                Assert.AreEqual(writeData, readData);
                asbl.WriteData(Line.ADR_DATA_REG5, writeData);
                readData = asbl.ReadData(Line.ADR_DATA_REG5);
                Assert.AreEqual(writeData, readData);
                asbl.WriteData(Line.ADR_DATA_REG6, writeData);
                readData = asbl.ReadData(Line.ADR_DATA_REG6);
                Assert.AreEqual(writeData, readData);
            }
        }

        [TestMethod]
        public void TestClear()
        {
            asbl.ClearAll();
        }

        [TestMethod]
        public void TestLines()
        {
            asbl.ClearAll();
            asbl.SetLineData(81);
        }

        [TestMethod]
        public void TestLines_1()
        {
            asbl.WriteData(Line.ADR_DIR_REG1, 4);
            asbl.SetLineData(3);
        }

        [TestMethod]
        public void TestLines_2()
        {
            asbl.SetLineDirection(3);
            asbl.SetLineData(3);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void TestLinesWith0Direction_1()
        {
            asbl.ClearLineDirection(3);
            asbl.SetLineData(3);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void TestLinesWith0Direction()
        {
            asbl.WriteData(Line.ADR_DIR_REG1, 0);
            asbl.SetLineData(3);
            asbl.WriteData(Line.ADR_DIR_REG1, 0x1F5);
            asbl.SetLineData(3);
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
