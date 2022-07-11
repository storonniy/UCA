using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCA.Devices;
using UCA.Steps;
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

        public static uint[] GetLineNumbers(string argument)
        {
            var lineNumbers = argument.Trim().Split(';');
            var uintLineNumbers = new List<uint>();
            for (int i = 0; i < lineNumbers.Length; i++)
            {
                if (lineNumbers[i] != "")
                    uintLineNumbers.Add(uint.Parse(lineNumbers[i]));
            }
            return uintLineNumbers.ToArray();
        }

        public override DeviceResult DoCommand(Step step)
        {
            try
            {
                switch (step.Command)
                {
                    case DeviceCommands.SetLineDirection:
                        var lineNumbers = GetLineNumbers(step.Argument);
                        asbl.SetLineDirection(lineNumbers);
                        return ResultOk($"Линии {string.Join(", ", lineNumbers)} установлены на вход");
                    case DeviceCommands.ClearLineDirection:
                        lineNumbers = GetLineNumbers(step.Argument); 
                        asbl.ClearLineDirection(lineNumbers);
                        return ResultOk($"Линии {string.Join(", ", lineNumbers)} установлены на выход");
                    case DeviceCommands.SetLineData:
                        lineNumbers = GetLineNumbers(step.Argument);
                        asbl.SetLineData(lineNumbers);
                        return ResultOk($"Линии {string.Join(", ", lineNumbers)} установлены в 1");
                    case DeviceCommands.ClearLineData:
                        try
                        {
                            lineNumbers = GetLineNumbers(step.Argument);
                            asbl.ClearLineData(lineNumbers);
                            return ResultOk($"Линии {string.Join(", ", lineNumbers)} установлены в 0");
                        }
                        catch (FailedToSetLineException ex)
                        {
                            return ResultError(ex.Message);
                        }
                    case DeviceCommands.ClearAll:
                        asbl.ClearAll();
                        return ResultOk("");
                    default:
                        return ResultError($"Неизвестная команда {step.Command}");
                }
            }
            catch (FailedToSetLineException ex)
            {
                return DeviceResult.ResultError($"{step.DeviceName}: {ex.Message}");
            }
            catch (LineIsSetToReceiveException ex)
            {
                return DeviceResult.ResultError($"{step.DeviceName}: {ex.Message}");
            }
            catch (ASBLException ex)
            {
                return DeviceResult.ResultError($"{step.DeviceName}: {ex.Message}");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return DeviceResult.ResultError($"{step.DeviceName} : {step.Command} : {ex.Message}");
            }
        }
    }
}
