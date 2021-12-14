using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;
using Automation.BDaq;

namespace UCA.DeviceDrivers
{
    public class PCI_1762
    {
        private InstantDoCtrl instantDoCtrl = new InstantDoCtrl();
        BDaqDevice device;
        

        public PCI_1762 (int deviceNumber, int deviceID)
        {         
            instantDoCtrl.SelectedDevice = new DeviceInformation(deviceNumber);
            //var errorCode = BDaqDevice.Open(deviceNumber, AccessMode.ModeWrite, out device);
            if (!instantDoCtrl.Initialized)
            {
                
            }
        }

        public bool Write(byte[] relayNumbers, int portStart)
        {
            //if (!instantDoCtrl.Initialized)
                //throw new Exception("Ошибка инициализации PCI_1762");
            ErrorCode errorCode = ErrorCode.ErrorUndefined;
            int portCount = 1;
           // instantDoCtrl.Write()
            errorCode = instantDoCtrl.Write(portStart, portCount, relayNumbers);
            return errorCode == ErrorCode.Success;
        }

        public byte Read(int port)
        {
            byte data;
            instantDoCtrl.Read(port, out data);
            return data;
        }
    }
}
