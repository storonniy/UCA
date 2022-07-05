using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using UCA.Devices;
using System.IO.Ports;
using System.Globalization;

namespace UCA.Steps
{ 
    struct StepsInfo
    {
        public Dictionary<string, Dictionary<string, List<Step>>> VoltageSupplyModesDictionary { get; set; }
        public Dictionary<string, Dictionary<string, List<Step>>> ModesDictionary { get; set; }
        public List<Step> EmergencyStepList { get; set; }
        //public DeviceInit DeviceHandler;
        public Dictionary<string, List<Step>> StepsDictionary { get; set; }
        public int StepNumber { get; private set; }
        public List<Device> DeviceList { get; set; }
        public string ProgramName { get; set; }
    }

    class Step
    {
        public string AdditionalArg { get; private set; }
        public int Channel { get; private set; }
        public string Device { get; private set; }
        public string Command { get; private set; }
        public string Argument { get; private set; }
        public string Description { get; private set; }
        public double LowerLimit { get; private set; }
        public double UpperLimit { get; private set; }
        public bool ShowStep { get; private set; }

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
                ReadTimeout = 2000,
                WriteTimeout = 2500
            };
            return serialPort;
        }

        private static string GetProgramName(DataSet dataSet)
        {
            var table = dataSet.Tables["ProgramName"];
            var programName = table.Rows[0]["ProgramName"].ToString();
            dataSet.Tables.Remove(dataSet.Tables[table.TableName]);
            return programName;
        }

        private static Dictionary<string, Dictionary<string, List<Step>>> GetModeStepDictionary(Dictionary<string, List<string>> modesDictionary, Dictionary<string, List<Step>> allStepsDictionary)
        {
            var modeStepDictionary = new Dictionary<string, Dictionary<string, List<Step>>>();
            foreach (var modeName in modesDictionary.Keys)
            {
                var stepsDictionary = new Dictionary<string, List<Step>>();
                var tableNames = modesDictionary[modeName];
                foreach (var tableName in tableNames)
                {
                    if (tableName.Split(' ').Length > 1)
                        stepsDictionary.Add(tableName, allStepsDictionary[$"'{tableName}'"]);
                    else
                        stepsDictionary.Add(tableName, allStepsDictionary[tableName]);
                }
                modeStepDictionary.Add(modeName, stepsDictionary);
            }
            return modeStepDictionary;
        }

        private static List<Device> GetDeviceList(DataSet dataSet)
        {
            var deviceList = new List<Device>();
            foreach (DataRow row in dataSet.Tables["DeviceInformation"].Rows)
            {
                var deviceName = row["device"].ToString();
                var portName = row["portName"].ToString();
                var baudRate = int.Parse(row["baudRate"].ToString());
                var description = row["description"].ToString();
                var device = new Device();
                device.SerialPort = GetSerialPort(portName, baudRate);
                device.Description = description;
                try
                {
                    device.Name = (DeviceNames)Enum.Parse(typeof(DeviceNames), deviceName);
                    deviceList.Add(device);
                }
                catch (ArgumentException ex)
                {
                    throw new ArgumentException($"Устройство {deviceName} не найдено в списке доступных устройств {description}");
                }
            }
            dataSet.Tables.Remove(dataSet.Tables["DeviceInformation"]);
            return deviceList;
        }

        public static Dictionary<string, List<string>> GetVoltageSupplyModesDictionary(DataSet dataSet)
        {
            var modesDictionary = new Dictionary<string, List<string>>();
            var table = dataSet.Tables["VoltageSupply"];
            foreach (DataRow row in table.Rows)
            {
                var checkingMode = row["VoltageSupplyMode"].ToString();
                var names = row["TableNames"].ToString();
                List<string> tableNames = names.Split(';').ToList();
                for (int i = 0; i < tableNames.Count; i++)
                {
                    while (tableNames[i].StartsWith(" "))
                        tableNames[i] = tableNames[i].Remove(0, 1);
                }
                modesDictionary.Add(checkingMode, tableNames);
            }
            dataSet.Tables.Remove(dataSet.Tables[table.TableName]);
            return modesDictionary;
        }

        public static Dictionary<string, List<string>> GetModesDictionary (DataSet dataSet)
        {
            var modesDictionary = new Dictionary<string, List<string>>();
            var table = dataSet.Tables["Settings"];
            foreach (DataRow row in table.Rows)
            {
                var checkingMode = row["CheckingMode"].ToString();
                List<string> tableNames = row["TableNames"].ToString().Split(';').ToList();
                for (int i = 0; i < tableNames.Count; i++)
                {
                    while (tableNames[i].StartsWith(" "))
                        tableNames[i] = tableNames[i].Remove(0, 1);
                }
                modesDictionary.Add(checkingMode, tableNames);
            }
            dataSet.Tables.Remove(dataSet.Tables[table.TableName]);
            return modesDictionary;
        }

        public static StepsInfo GetStepsInfo(DataSet dataSet)
        {
            var programName = GetProgramName(dataSet);
            var voltageModesDictionary = GetVoltageSupplyModesDictionary(dataSet);
            var modesDictionary = GetModesDictionary(dataSet);
            var deviceList = GetDeviceList(dataSet);
            var emergencyStepsDictionary = GetEmergencyStepsDictionary(dataSet);
            var stepsDictionary = GetStepsDictionary(dataSet);
            var voltageSupplyDictionary = GetModeStepDictionary(voltageModesDictionary, stepsDictionary);
            var modeStepDictionary = GetModeStepDictionary(modesDictionary, stepsDictionary);
            var info = new StepsInfo()
            {
                VoltageSupplyModesDictionary = voltageSupplyDictionary,
                ModesDictionary = modeStepDictionary,
                StepsDictionary = stepsDictionary,
                EmergencyStepList = emergencyStepsDictionary["EmergencyBreaking"],
                //DeviceHandler = new DeviceInit(deviceList),
                DeviceList = deviceList,
                ProgramName = programName
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

        private static Dictionary<string, List<Step>> GetEmergencyStepsDictionary(DataSet dataSet)
        {
            var tableEmergencyBreaking = dataSet.Tables["EmergencyBreaking"];
            dataSet.Tables.Remove(dataSet.Tables[tableEmergencyBreaking.TableName]);
            var dataSetEmergency = new DataSet();
            dataSetEmergency.Tables.Add(tableEmergencyBreaking);
            var dictionary = GetStepsDictionary(dataSetEmergency);
            return dictionary;
        }

        private static Step GetStep(DataRow row)
        {
            var step = new Step();
            step.Device = row["device"].ToString();
            step.Command = row["command"].ToString();
            step.Argument = row["argument"].ToString();
            step.Description = row["description"].ToString();
            step.Channel = int.Parse(row["channel"].ToString());
            var lowerLimit = row["lowerLimit"].ToString();
            step.LowerLimit = double.Parse(lowerLimit, NumberStyles.Float);
            var upperLimit = row["upperLimit"].ToString();
            step.UpperLimit = double.Parse(upperLimit, NumberStyles.Float);
            var meow = row["showStep"];
            step.ShowStep = Convert.ToBoolean(meow);
            if (step.Channel > 0)
                step.Description = $"Канал {step.Channel}: {step.Description}";
            step.AdditionalArg = row["additionalArg"].ToString();
            return step;
        }
    }
}
