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

        //PCI_1762 pci1762 = new PCI_1762("PCI-1762,BID#1");
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

        
        [TestMethod]
        public void GetData()
        {
            int expected = 135;
            int actual = PCI_1762.GetData(new List<int>() { 0, 1, 2, 7 });
            Assert.AreEqual(expected, actual);
            expected = 44;
            actual = PCI_1762.GetData(new List<int>() { 2, 3, 5, 5 });
            Assert.AreEqual(expected, actual);
            expected = 172;
            actual = PCI_1762.GetData(new List<int>() { 2, 5, 3, 7 });
            Assert.AreEqual(expected, actual);
            expected = 30;
            actual = PCI_1762.GetData(new List<int>() { 1, 2, 3, 4 });
            Assert.AreEqual(expected, actual);
            expected = 62;
            actual = PCI_1762.GetData(new List<int>() { 1, 2, 3, 4, 5 });
            Assert.AreEqual(expected, actual);
        }     
        #endregion
    }
}
