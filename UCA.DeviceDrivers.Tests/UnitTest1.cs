using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using UCA.DeviceDrivers;
using System.Threading;
using System.IO.Ports;

namespace UCA.DeviceDrivers.Tests
{
    [TestClass]
    public class UnitTest1
    {
        SerialPort serialPort = new SerialPort()
        {
            PortName = "COM4",
            BaudRate = 115200,
            DataBits = 8,
            Parity = Parity.None,
            StopBits = StopBits.One,
            DtrEnable = true,
            RtsEnable = true,
            Handshake = Handshake.None,
            ReadTimeout = 1000,
            WriteTimeout = 2500
        };

        [TestMethod]
        public void TestSignal()
        {
            Commutator comm = new Commutator(serialPort);
            var value = comm.GetSignals();
            Assert.AreEqual("none", value[0]);
        }
        /*
       [TestMethod]
       public void TestPSP1()
       {
           var svalue = "2,08333333333333E-06";
           var value = Double.Parse(svalue);
           Assert.AreEqual(2.08, value);
       }

       [TestMethod]
       public void TestPSP1()
       {
           PSP405 psp = new PSP405(serialPort);
           psp.SetCurrentLimit(0.4);
           Thread.Sleep(500);
           var actualVoltage = psp.GetCurrentLimit();
           Assert.AreEqual(0.4, actualVoltage);
       }

       [TestMethod]
       public void TestPSP()
       {
           PSP405 psp = new PSP405(serialPort);
           psp.SetVoltage(9);
           var actualVoltage =  psp.GetOutputVoltage();
           Assert.AreEqual(9, actualVoltage);
       }

       [TestMethod]
       public void TestTestr1r2()
       {
           Commutator comm = new Commutator(serialPort);

           var actualNames = comm.GetClosedRelayNames();
           Thread.Sleep(2000);
           var expectedNames = new string[] { "none" };
           CollectionAssert.AreEqual(expectedNames, actualNames);
       }

       [TestMethod]
       public void Test()
       {
           serialPort.Open();
           serialPort.WriteLine("*idn?");
           Thread.Sleep(1000);
           var answer = serialPort.ReadLine();
           var request = "meow";
           Assert.AreEqual(request, answer);
           serialPort.Close();
       }
       */
    }
}
