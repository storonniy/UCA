using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using UCA.Devices;
using UCA.DeviceDrivers;

namespace TestProject
{
    [TestClass]
    public class UnitTest1
    {
        #region PCI_1762

        [TestMethod]
        public void GetRelayNumbersArray_Test()
        {
            var actual = PCI1762_device.GetRelayNumbersArray("1, 2, 3, 4, 5, 6, 7, 8");
            var expected = new byte[] {1, 2, 3, 4, 5, 6, 7, 8 };
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetRelayNumbersArray_Test_lessThan8Relays()
        {
            var actual = PCI1762_device.GetRelayNumbersArray("1, 2, 3, 4, 5, 6");
            var expected = new byte[] { 1, 2, 3, 4, 5, 6, 0, 0 };
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void GetRelayNumbersArray_Test_greaterThan8Relays()
        {
            PCI1762_device.GetRelayNumbersArray("1, 2, 3, 4, 5, 6, 7, 8, 9");
        }

        [TestMethod]
        public void CloseRelay_Test()
        {
            var relayNumbers = PCI1762_device.GetRelayNumbersArray("1, 2, 3, 4, 5, 6, 7, 8");
            var deviceResult = PCI1762_device.CloseRelay(relayNumbers, 0);
            Assert.AreEqual(DeviceResult.ResultError, deviceResult);
        }

        #endregion
    }
}
