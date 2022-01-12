using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using UCA.Devices;
using UCA.DeviceDrivers;
using System.Collections.Generic;
using System.Threading;

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
            var expected = new int[] {1, 2, 3, 4, 5, 6, 7, 8 };
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetRelayNumbersArray_Test_lessThan8Relays()
        {
            var actual = PCI1762_device.GetRelayNumbersArray("1, 2, 3, 4, 5, 6");
            var expected = new int[] { 1, 2, 3, 4, 5, 6 };
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CheckOnePort ()
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
        public void GetRelaysAsByte()
        {
            int expected = 135;
            int actual = PCI_1762.GetRelaysAsByte(new List<int>() { 0, 1, 2, 7 });
            Assert.AreEqual(expected, actual);
            expected = 172;
            actual = PCI_1762.GetRelaysAsByte(new List<int>() { 2, 5, 3, 7 });
            Assert.AreEqual(expected, actual);
            expected = 30;
            actual = PCI_1762.GetRelaysAsByte(new List<int>() { 1, 2, 3, 4 });
            Assert.AreEqual(expected, actual);
            expected = 62;
            actual = PCI_1762.GetRelaysAsByte(new List<int>() { 1, 2, 3, 4, 5 });
            Assert.AreEqual(expected, actual);
        }  
        
        [TestMethod]
        public void CheckDataWithRepeatingNumbers()
        {
            int expected = 44;
            int actual = PCI_1762.GetRelaysAsByte(new List<int>() { 2, 3, 5, 5 });
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetDataAsRelayNumbers()
        {
            var expected = new List<int>() { 1, 3, 7 };
            var actual = PCI_1762.ConvertDataToRelayNumbers(138);
            CollectionAssert.AreEquivalent(expected, actual);
        }

        [TestMethod]
        public void OpenAllRelays()
        {
            PCI_1762 pci1762 = new PCI_1762("PCI-1762,BID#1");
            pci1762.Write(new int[] { 0, 7, 8, 15 });
            Thread.Sleep(100);
            pci1762.OpenAllRelays();
            var expected = new int[] { };
            var actual = pci1762.GetClosedRelaysNumbers();
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CloseRelaysPort0()
        {
            PCI_1762 pci1762 = new PCI_1762("PCI-1762,BID#1");
            pci1762.OpenAllRelays();
            Thread.Sleep(100);
            var expected = new int[] { 0, 3, 7 };
            pci1762.Write(expected);
            Thread.Sleep(100);
            var actual = pci1762.GetClosedRelaysNumbers();
            CollectionAssert.AreEquivalent(expected, actual);
        }

        [TestMethod]
        public void CloseRelaysPort1()
        {
            PCI_1762 pci1762 = new PCI_1762("PCI-1762,BID#1");
            pci1762.OpenAllRelays();
            Thread.Sleep(100);
            var expected = new int[] { 8, 11, 15 };
            pci1762.Write(expected);
            Thread.Sleep(100);
            var actual = pci1762.GetClosedRelaysNumbers();
            CollectionAssert.AreEquivalent(expected, actual);
        }

        [TestMethod]
        public void CloseRelaysPort1Port2()
        {
            PCI_1762 pci1762 = new PCI_1762("PCI-1762,BID#1");
            pci1762.OpenAllRelays();
            Thread.Sleep(100);
            var expected = new int[] { 2, 4, 6, 9, 12, 15 };
            pci1762.Write(expected);
            Thread.Sleep(100);
            var actual = pci1762.GetClosedRelaysNumbers();
            CollectionAssert.AreEquivalent(expected, actual);
        }

        [TestMethod]
        public void OpenRelaysPort1Port2()
        {
            PCI_1762 pci1762 = new PCI_1762("PCI-1762,BID#1");
            pci1762.OpenAllRelays();
            Thread.Sleep(100);
            var expected = new int[] { 2, 4, 6, 9, 12, 15 };
            pci1762.Write(expected);
            Thread.Sleep(100);
            var actual = pci1762.GetClosedRelaysNumbers();
            CollectionAssert.AreEquivalent(expected, actual);
            pci1762.OpenRelays(new int[] { 2, 9, 15});
            expected = new int[] { 4, 6, 12 };
            actual = pci1762.GetClosedRelaysNumbers();
            CollectionAssert.AreEquivalent(expected, actual);
        }

        #endregion
    }
}
