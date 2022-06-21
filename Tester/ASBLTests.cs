using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using FTD2XX_NET;
using UPD.DeviceDrivers;

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
        public void TestASBL()
        {
            var asbl = new ASBL();
            asbl.SetLineDirection(97);
        }

    }
}
