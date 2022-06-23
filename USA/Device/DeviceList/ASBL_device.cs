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
            
            switch (deviceData.Command)
            {
                case DeviceCommands.SetLineDirection:
                    var lineNumber = uint.Parse(deviceData.Argument);
                    asbl.SetLineDirection(lineNumber);
                    return ResultOk("");
                case DeviceCommands.ClearLineDirection:
                    lineNumber = uint.Parse(deviceData.Argument); asbl.ClearLineDirection(lineNumber);
                    return ResultOk("");
                case DeviceCommands.SetLineData:
                    lineNumber = uint.Parse(deviceData.Argument);
                    asbl.SetLineData(lineNumber);
                    return ResultOk("");
                case DeviceCommands.ClearLineData:
                    lineNumber = uint.Parse(deviceData.Argument);
                    asbl.ClearLineData(lineNumber);
                    return ResultOk("");
                case DeviceCommands.ClearAll:
                    asbl.ClearAll();
                    return ResultOk("");
                default:
                    return ResultError($"Неизвестная команда {deviceData.Command}");
            }
        }
    }
}
