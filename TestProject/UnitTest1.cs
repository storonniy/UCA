using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using UCA.Devices;
using UCA.DeviceDrivers;
using System.Collections.Generic;

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
        public void meow ()
        {
            var expectedDict = new Dictionary<int, List<int>>();
            expectedDict.Add(0, new List<int>() { 1, 3, 5, 7 });
            var actualDict = PCI_1762.GetPortNumDictionary(new int[] { 1, 3, 5, 7 });
            CheckDictionary(expectedDict, actualDict);
        }

        [TestMethod]
        public void CheckTwoPorts()
        {
            var expectedDict = new Dictionary<int, List<int>>();
            expectedDict.Add(0, new List<int>() { 1, 3, 7 });
            expectedDict.Add(1, new List<int>() { 0, 7, 2 });
            var actualDict = PCI_1762.GetPortNumDictionary(new int[] { 8, 1, 3, 7, 15, 10});
            CheckDictionary(expectedDict, actualDict);
        }

        private void CheckDictionary(Dictionary<int, List<int>> expectedDict, Dictionary<int, List<int>> actualDict)
        {
            CollectionAssert.AreEquivalent(expectedDict.Keys, actualDict.Keys);
            foreach (var key in expectedDict.Keys)
            {
                CollectionAssert.AreEquivalent(expectedDict[key], actualDict[key]);
            }
        }

        /*
        [TestMethod]
        public void CloseRelay_Test()
        {
            var relayNumbers = PCI1762_device.GetRelayNumbersArray("1, 2, 3, 4, 5, 6, 7, 8");
            var deviceResult = PCI1762_device.CloseRelay(relayNumbers, 0);
            Assert.AreEqual(DeviceResult.ResultError, deviceResult);
        }
        */
        #endregion
    }
}
