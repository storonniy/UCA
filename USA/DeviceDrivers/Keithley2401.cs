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

        public void SelectVoltageSource()
        {
            SendCommand($":SOUR:FUNC VOLT#013#010");
        }

        /// <summary>
        /// Select fixed sourcing mode for V source
        /// </summary>
        public void SelectFixedSourcingModeVoltage()
        {
            SendCommand(":SOUR:VOLT:MODE FIX#013#010");
        }

        /// <summary>
        /// Select fixed sourcing mode for I source
        /// </summary>
        public void SelectFixedSourcingModeCurrent()
        {
            SendCommand(":SOUR:CURR:MODE FIX#013#010");
        }

        /// <summary>
        /// Set V-source amplitude
        /// </summary>
        /// <param name="voltage"> Amplitude in volts </param>
        public void SetVoltage(double voltage)
        {
            SendCommand($":SOUR:VOLT:LEV:AMPL {voltage}#013#010");
        }

        /// <summary>
        /// Select range for I-Source
        /// </summary>
        /// <param name="up"> Minimum, A </param>
        /// <param name="to"> Maximum, A </param>
        public void SetCurrentRange(double up, double to)
        {
            SendCommand($":SOUR:CURR:RANG {up} to {to}#013#010");
        }

        public double GetCurrentRange()
        {
            SendCommand($"CURR? MAX#013#010");
            return ParseValue(serialPort.ReadLine());
        }

        /// <summary>
        /// Select range for V-Source
        /// </summary>
        /// <param name="up"> Minimum, V </param>
        /// <param name="to"> Maximum, V </param>
        public void SetVoltageRange(double up, double to)
        {
            SendCommand($":SOUR:VOLT:RANG {up} to {to}#013#010");
        }

        public double GetSourceVoltage()
        {
            SendCommand($"VOLT?#013#010");
            return ParseValue(serialPort.ReadLine());
        }

        /// <summary>
        /// Query voltage level
        /// </summary>
        /// <returns></returns>
        public double GetVoltage()
        {
            //SendCommand(":FORM:ELEM VOLT#013#010");
            //SendCommand(":READ?#013#010");
            //SendCommand(":FORM:ELEM VOLT#013#010");
            SendCommand(":SOUR:VOLT:LEV:IMM:AMPL?#013#010");
            return ParseValue(serialPort.ReadLine());
        }

        /// <summary>
        /// Set I-source amplitude
        /// </summary>
        /// <param name="current"> Amplitude in amps </param>
        public void SetCurrent(double current)
        {
            SendCommand($":SOUR:CURR:LEV:AMPL {current}#013#010");
        }


        public double GetCurrent()
        {
            SendCommand(":FORM:ELEM CURR#013#010");
            SendCommand(":READ?#013#010");
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
            SendCommand($":OUTP {state}#013#010");
            SendCommand(":OUTP:STAT?013#010");
            // TODO: проверить, отвечает 0 or OFF
            return serialPort.ReadLine() == state.ToString(); // return serialPort.ReadLine() == Convert.ToInt32(state);
        }

        private void SendCommand(string command)
        {
            serialPort.WriteLine(command);
            Thread.Sleep(delay);
        }

        private enum PowerState
        {
            OFF,
            ON       
        }
    }
}
