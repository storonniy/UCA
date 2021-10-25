using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCA.Devices
{
    class None : IDeviceInterface
    {
        private double CalculateUCACoefficient(int channel, double value)
        {
            var valuesAtZero = GetCoefficientValues(channel, 0);
            var values = GetCoefficientValues(channel, value);
            var coeff = (values[1] - valuesAtZero[1]) / (values[0] - valuesAtZero[0]);
            switch (channel)
            {
                case 1:
                case 2:
                    return 480000 * coeff;
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                    return coeff;
                default:
                    throw new Exception($"Номер канала должен быть от 1 до 10 для УСА, указан номер канала {channel}");
            }
        }

        public override DeviceResult DoCommand(DeviceData deviceData)
        {
            switch (deviceData.Command)
            {
                case DeviceCommands.CalculateCoefficient:
                    var value = double.Parse(deviceData.Argument, CultureInfo.InvariantCulture);
                    var expectedCoefficient = double.Parse(deviceData.ExpectedValue, CultureInfo.InvariantCulture);
                    var tolerance = double.Parse(deviceData.Tolerance, CultureInfo.InvariantCulture);
                    var actualCoefficient = CalculateUCACoefficient(deviceData.Channel, value);
                    if (actualCoefficient >= expectedCoefficient - tolerance && actualCoefficient <= expectedCoefficient + tolerance)
                        return DeviceResult.ResultOk($"Коэффициент равен {actualCoefficient} В/мкА");//($"Напиши метод {deviceData.Command}");
                    else
                        return DeviceResult.ResultError($"Коэффициент должен быть в диапазоне {expectedCoefficient} +/- {tolerance} В/мкА");
                case DeviceCommands.CalculateCoeff_UCA_T:
                    return DeviceResult.ResultError($"Напиши метод,  {deviceData.Command}");
                default:
                    return DeviceResult.ResultError($"Неизвестная команда {deviceData.Command}");
            }
        }
    }
}
