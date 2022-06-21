using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using FTDIWrapper;
//using FTD2XXHelper;
using D2XXHelper;
using System.Threading;
using FTD2XX_NET;

namespace Tester
{
    [TestClass]
    public class ASBLTests
    {
        [TestMethod]
        public void ASBLNewTest()
        {
            var ftdi = new FTDI("FTD2XX.dll");
            var f = new FTDI.FT_DEVICE_INFO_NODE();
            //var status = ftdi.GetDeviceList(new FTDI.FT_DEVICE_INFO_NODE[] {f  });
        }

    }
}
