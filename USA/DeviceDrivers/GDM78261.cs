using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.IO;
using System.Threading;
using System.Globalization;

namespace UCA.DeviceDrivers
{
    public class GDM78261
    {
        readonly SerialPort serialPort;
        public GDM78261 (SerialPort serialPort)
        {
            this.serialPort = serialPort;
            this.serialPort.Open();
        }

        ~GDM78261()
        {
            this.serialPort.Close();
        }

        public double MeasureVoltageAC()
        {
            serialPort.WriteLine("MEAS:VOLT:AC?#013#010");
            Thread.Sleep(2000);
            return ParseValue(serialPort.ReadLine());
        }


        /// <summary>
        /// Returns the DC voltage measurement as N mV.
        /// </summary>
        /// <returns></returns>
        public double MeasureVoltageDC()
        {
            serialPort.WriteLine("MEAS:VOLT:DC?#013#010");
            Thread.Sleep(2000);
            return ParseValue(serialPort.ReadLine());
        }
        /// <summary>
        /// Returns the DC current measurement, mA.
        /// </summary>
        /// <returns></returns>
        public double MeasureCurrentDC()
        {
            Thread.Sleep(2000);
            serialPort.WriteLine("MEAS:CURR:DC?#013#010");
            Thread.Sleep(2000);
            return ParseValue(serialPort.ReadLine());
        }

        /// <summary>
        /// Sets measurement to DC Current on the first display and specifies range
        /// </summary>
        /// <param name="range"></param>
        public void SetMeasurementToCurrentDC(double range)
        {
            serialPort.WriteLine($"CONF:CURR:DC {range}#013#010");
            Thread.Sleep(1000);
        }
        /// <summary>
        /// Sets measurement to DC Voltage on the first display and specifies the range
        /// </summary>
        /// <param name="range"></param>
        public void SetMeasurementToVoltageDC(double range)
        {
            serialPort.WriteLine($"CONF:VOLT:DC {range}#013#010");
            Thread.Sleep(1000);
        }


        private enum Polarity
        {
            AC,
            DC
        }

        public void Configure()
        {

        }

        public static double ParseValue (string value)
        {
            return (double)Decimal.Parse(value.Replace("\r", ""), NumberStyles.Float, CultureInfo.InvariantCulture);
        }
    }
}
