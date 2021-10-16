using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using UCA.Devices;
using System.IO.Ports;

namespace UCA.Steps
{
    struct StepsInfo
    {
        public DeviceInit DeviceHandler;
        public Dictionary<string, List<Step>> StepsDictionary;
        public List<Step> EmergencyStepList;
        public int StepNumber;
    }

    class Step
    {
        public string ExpectedValue;
        public int Channel;
        public string Device;
        public string Command;
        public string Argument;
        public string Description;

        private static SerialPort GetSerialPort(string portName, int baudrate)
        {
            SerialPort serialPort = new SerialPort()
            {
                PortName = portName,
                BaudRate = baudrate,
                DataBits = 8,
                Parity = Parity.None,
                StopBits = StopBits.One,
                DtrEnable = true,
                RtsEnable = true,
                Handshake = Handshake.None,
                ReadTimeout = 1000,
                WriteTimeout = 2500
            };
            return serialPort;
        }

        private static List<Device> GetDeviceList(DataSet dataSet)
        {
            var deviceList = new List<Device>();
            foreach (DataRow row in dataSet.Tables["DeviceInformation$"].Rows)
            {
                var deviceName = row["device"].ToString();
                var portName = row["portName"].ToString();
                var baudRate = int.Parse(row["baudRate"].ToString());
                var device = new Device();
                //// ВНИМАНИЕ! КОСТЫЛЬ!
                switch (deviceName)
                {
                    case "Commutator":
                        portName = "COM4";
                        baudRate = 115200;
                        break;
                    case "PSP_405_power":
                        portName = "COM5";
                        baudRate = 2400;
                        break;
                    case "PSP_405":
                        portName = "COM6";
                        baudRate = 2400;
                        break;
                }
                /// КОНЕЦ КОСТЫЛЯ

                device.SerialPort = GetSerialPort(portName, baudRate);
                try
                {
                    device.Name = (DeviceNames)Enum.Parse(typeof(DeviceNames), deviceName);
                    deviceList.Add(device);
                }
                catch (ArgumentException ex)
                {
                    throw new ArgumentException($"Устройство {deviceName} не найдено в списке доступных устройств");
                }
            }
            dataSet.Tables.Remove(dataSet.Tables["DeviceInformation$"]);
            return deviceList;
        }

        public static StepsInfo GetStepsInfo(DataSet dataSet)
        {
            var deviceList = GetDeviceList(dataSet);
            DataSet dataSetEmergency = GetEmergencyDataSet(dataSet);
            var emergencyStepsDictionary = GetStepsDictionary(dataSetEmergency);
            var stepsDictionary = GetStepsDictionary(dataSet);
            var info = new StepsInfo()
            {
                StepsDictionary = stepsDictionary,
                EmergencyStepList = emergencyStepsDictionary["EmergencyBreaking$"],
                DeviceHandler = new DeviceInit(deviceList)
            };
            return info;
        }

        private static Dictionary<string, List<Step>> GetStepsDictionary(DataSet dataSet)
        {
            var dictionary = new Dictionary<string, List<Step>>();           
            foreach (DataTable table in dataSet.Tables)
            {
                var stepList = new List<Step>();
                var tableName = table.ToString();
                foreach (DataRow row in table.Rows)
                {
                    var step = GetStep(row);
                    stepList.Add(step);
                }
                dictionary.Add(tableName, stepList);
            }
            return dictionary;
        }

        private static DataSet GetEmergencyDataSet(DataSet dataSet)
        {
            var tableEmergencyBreaking = dataSet.Tables["EmergencyBreaking$"];
            dataSet.Tables.Remove(dataSet.Tables["EmergencyBreaking$"]);
            var dataSetEmergency = new DataSet();
            dataSetEmergency.Tables.Add(tableEmergencyBreaking);
            return dataSetEmergency;
        }

        private static Step GetStep(DataRow row)
        {
            var step = new Step();
            step.Device = row["device"].ToString();
            step.Command = row["command"].ToString();
            step.Argument = row["argument"].ToString();
            step.Description = row["description"].ToString();           
            step.Channel = int.Parse(row["channel"].ToString());
            step.ExpectedValue = row["expectedValue"].ToString();
            return step;
        }
    }
}
