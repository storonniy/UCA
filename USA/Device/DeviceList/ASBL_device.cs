using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCA.Devices;
using UPD.DeviceDrivers;
using static UCA.Devices.DeviceResult;

namespace UPD.Device.DeviceList
{
    public class ASBL_device : IDeviceInterface
    {
        readonly ASBL asbl;
        public ASBL_device()
        {
            asbl = new ASBL();
        }

        public override DeviceResult DoCommand(DeviceData deviceData)
        {
            var lineNumber = uint.Parse(deviceData.Argument);
            switch (deviceData.Command)
            {
                case DeviceCommands.SetLineDirection:
                    asbl.SetLineDirection(lineNumber);
                    return ResultOk("");
                case DeviceCommands.UnSetLineDirection:
                    asbl.ClearLineDirection(lineNumber);
                    return ResultOk("");
                case DeviceCommands.SetLineData:
                    asbl.SetLineData(lineNumber);
                    return ResultOk("");
                case DeviceCommands.UnSetLineData:
                    asbl.ClearLineData(lineNumber);
                    return ResultOk("");
                default:
                    return ResultError($"Неизвестная команда {deviceData.Command}");
            }
        }
    }
}
