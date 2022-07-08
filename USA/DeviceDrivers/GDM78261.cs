using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.IO;
using System.Threading;
using System.Globalization;
using UPD.DeviceDrivers;

namespace UCA.DeviceDrivers
{
    public class GDM78261
    {
        readonly SerialPort serialPort;
        public GDM78261 (SerialPort serialPort)
        {
            this.serialPort = serialPort;
            if (SerialPort.GetPortNames().ToList().Contains(serialPort.PortName))
                this.serialPort.Open();
        }

        ~GDM78261()
        {
            serialPort.Close();
        }

        public double MeasureVoltageAC()
        {
            Thread.Sleep(4000);
            serialPort.SendCommand("MEAS:VOLT:AC?#013#010");
            return serialPort.ReadDouble();
        }

        /// <summary>
        /// Returns the DC voltage measurement as N mV.
        /// </summary>
        /// <returns></returns>
        public double MeasureVoltageDC()
        {
            Thread.Sleep(1000);
            serialPort.SendCommand("MEAS:VOLT:DC?#013#010");
            return serialPort.ReadDouble();
        }
        /// <summary>
        /// Returns the DC current measurement, mA.
        /// </summary>
        /// <returns></returns>
        public double MeasureCurrentDC()
        {
            Thread.Sleep(2000);
            serialPort.SendCommand("MEAS:CURR:DC?#013#010");
            return serialPort.ReadDouble();
        }

        /// <summary>
        /// Sets measurement to DC Current on the first display and specifies range
        /// </summary>
        /// <param name="range"></param>
        public void SetMeasurementToCurrentDC(double range)
        {
            serialPort.SendCommand($"CONF:CURR:DC {range}#013#010");
        }
        /// <summary>
        /// Sets measurement to DC Voltage on the first display
        /// </summary>
        public void SetMeasurementToVoltageDC()
        {
            serialPort.SendCommand("CONF:VOLT:DC#013#010");
        }

        /// <summary>
        /// Sets measurement to AC Voltage on the first display
        /// </summary>
        public void SetMeasurementToVoltageAC()
        {
            serialPort.SendCommand($"CONF:VOLT:AC#013#010");
        }
    }
}
