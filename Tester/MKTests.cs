using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Ixxat.Vci4;
using Ixxat.Vci4.Bal;
using Ixxat.Vci4.Bal.Can;
using UPD.DeviceDrivers;
using System.Collections.Generic;

namespace MKTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void GetRelayStatesBytesTest()
        {
            for (int j = 0; j < 100; j++)
            {
                byte[] canMsg1 = new byte[8];
                byte[] canMsg2 = new byte[8];
                var rnd = new Random();
                canMsg1[0] = 0xFB;
                canMsg1[1] = 0x01;
                canMsg2[0] = 0xFB;
                canMsg2[1] = 0x02;
                for (var i = 2; i < 8; i++)
                {
                    canMsg1[i] = (byte)rnd.Next(0, 255);
                    canMsg2[i] = (byte)rnd.Next(0, 255);
                }
                var expected = new byte[10];
                Array.Copy(canMsg1, 3, expected, 0, 5);
                Array.Copy(canMsg2, 3, expected, 5, 5);
                var actual = MK.GetRelayStatesBytes(new List<byte[]> { canMsg1, canMsg2 });
                CollectionAssert.AreEqual(expected, actual);
                var reversedActual = MK.GetRelayStatesBytes(new List<byte[]> { canMsg2, canMsg1 });
                CollectionAssert.AreEqual(expected, reversedActual);
            }
        }

        [TestMethod]
        public void GetRelayNumbersTest()
        {
            byte[] canMsg1 = new byte[]{ 0xFB, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01 };
            byte[] canMsg2 = new byte[]{ 0xFB, 0x02, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01 };
            var relayNumbers = new int[] { 1, 9, 17, 25, 33, 41, 49, 57, 65, 73 };
            var actual = MK.GetRelayNumbers(MK.GetRelayStatesBytes(new List<byte[]> { canMsg1, canMsg2 }));
            CollectionAssert.AreEquivalent(relayNumbers, actual);
            actual = MK.GetRelayNumbers(MK.GetRelayStatesBytes(new List<byte[]> { canMsg2, canMsg1 }));
            CollectionAssert.AreEquivalent(relayNumbers, actual);
            var byteStates = new byte[] { 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01 };
            actual = MK.GetRelayNumbers(byteStates);
            CollectionAssert.AreEquivalent(relayNumbers, actual);
        }
    }
}
