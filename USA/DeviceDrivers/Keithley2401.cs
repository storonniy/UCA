using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Globalization;
using System.Threading;

namespace UCA.DeviceDrivers
{
    public class Keithley2401
    {
        readonly SerialPort serialPort;
        public Keithley2401(SerialPort serialPort)
        {
            this.serialPort = serialPort;
            //serialPort.Open();
        }

        private static readonly int delay = 1000;

        public static double ParseValue(string value)
        {
            return (double)Decimal.Parse(value.Replace("\r", ""), NumberStyles.Float, CultureInfo.InvariantCulture);
        }

        public double MeasureVoltage()
        {
            serialPort.WriteLine(":FORM:ELEM VOLT#013#010");
            Thread.Sleep(delay);
            serialPort.WriteLine(":READ?#013#010");
            Thread.Sleep(delay);
            return ParseValue(serialPort.ReadLine());
        }

        public double MeasureCurrent()
        {
            serialPort.WriteLine(":FORM:ELEM CURR#013#010");
            Thread.Sleep(delay);
            serialPort.WriteLine(":READ?#013#010");
            Thread.Sleep(delay);
            return ParseValue(serialPort.ReadLine());
        }


        /// <summary>
        /// Turn the output of the Keithley 2401 OFF.
        /// </summary>

        public bool PowerOff()
        {
            return ChangePowerStatus(PowerState.OFF);
        }

        /// <summary>
        /// Turn the output of the Keithley 2401 ON.
        /// </summary>
        public bool PowerOn()
        {
            return ChangePowerStatus(PowerState.ON);
/*            serialPort.WriteLine(":OUTP ON#013#010");
            Thread.Sleep(delay);*/
        }

        private bool ChangePowerStatus(PowerState state)
        {
            serialPort.WriteLine($":OUTP {state}#013#010");
            Thread.Sleep(delay);
            serialPort.WriteLine(":OUTP:STAT?013#010");
            Thread.Sleep(delay);
            // TODO: проверить, отвечает 0 or OFF
            return serialPort.ReadLine() == state.ToString(); // return serialPort.ReadLine() == Convert.ToInt32(state);
        }

        private enum PowerState
        {
            OFF,
            ON       
        }
    }
}
