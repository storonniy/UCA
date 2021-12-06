using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using UCA.Devices;
using UCA.Logging;
using UCA.Steps;
using static UCA.ControlObjectSettings.ControlObjectSettings;
using System.ComponentModel;

namespace UCA
{
    public partial class Form1 : Form
    {
        private StepsInfo stepsInfo;
        private DeviceInit DeviceHandler;
        Settings settings = new Settings();
        Dictionary<TreeNode, Step> treeviewNodeStep = new Dictionary<TreeNode, Step>();
        Dictionary<Step, TreeNode> treeviewStepNode = new Dictionary<Step, TreeNode>();
        Dictionary<DeviceNames, Label> deviceLabelDictionary = new Dictionary<DeviceNames, Label>();
        Thread mainThread = new Thread(some);//stepsDictionary
        Log mainLog = new Log();

        private static void some()
        {

        }

        public Form1(Settings settings)
        {
            InitializeComponent();
            EventSend = this;
            this.settings = settings;
            ShowSettings();
            InitialActions();
            buttonStop.Enabled = false;
            buttonCheckingPause.Enabled = false;
        }

        private void CleanAll()
        {
            treeOfChecking.Nodes.Clear();
            comboBoxCheckingMode.Items.Clear();
            comboBoxVoltageSupply.Items.Clear();
            treeviewNodeStep.Clear();
            treeviewStepNode.Clear();
        }

        private void ShowSettings()
        {
            labelComment.Text = (settings.Comment != "") ? settings.Comment : "Не указано";
            labelFactoryNumber.Text = settings.FactoryNumber.ToString();
            labelOperatorName.Text = (settings.OperatorName != "") ? settings.OperatorName : "Не указано";
        }

        private void SetVoltageSupplyModes()
        {
            foreach (var modeName in stepsInfo.VoltageSupplyModesDictionary.Keys)
            {
                comboBoxVoltageSupply.Items.Add(modeName);
            }
            comboBoxVoltageSupply.SelectedItem = comboBoxVoltageSupply.Items[1];
        }

        private void ShowCheckingModes()
        {
            foreach (var modeName in stepsInfo.ModesDictionary.Keys)
            {
                comboBoxCheckingMode.Items.Add(modeName);
            }
            comboBoxCheckingMode.SelectedItem = comboBoxCheckingMode.Items[1];
        }

        private void InitDevices()
        {
            DeviceHandler = new DeviceInit(stepsInfo.DeviceList);
            foreach (var deviceName in DeviceHandler.Devices.Keys)
            {
                var device = DeviceHandler.Devices[deviceName];
                if (device == null)
                {
                    break;
                }
            }
            InitDevices();
        }

        private void ShowDevicesOnForm()
        {
            var deviceList = stepsInfo.DeviceList;
            foreach (var device in deviceList)
            {
                var labelDevice = new Label()
                {
                    Text = device.Name.ToString(),
                    Location = new Point(20, 30 + 25 * groupBoxDevices.Controls.Count)
                };
                groupBoxDevices.Controls.Add(labelDevice);
                deviceLabelDictionary.Add(device.Name, labelDevice);
            }
        }

        private void UpdateDevicesOnForm()
        {
            foreach (var device in stepsInfo.DeviceList)
            {
                var deviceLabel = deviceLabelDictionary[device.Name];
                if (device.SerialPort.IsOpen)
                {
                    deviceLabel.ForeColor = Color.Green;
                }
                else
                {
                    deviceLabel.ForeColor = Color.Red;
                }
            }
        }

        private void InitialActions(string pathToDataBase)
        {
            try
            {
                string connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0}; Extended Properties=Excel 12.0;", pathToDataBase);//"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + pathToDataBase;
                var dbReader = new DBReader(connectionString);
                var dataSet = dbReader.GetDataSet();
                this.stepsInfo = Step.GetStepsInfo(dataSet);
                SetVoltageSupplyModes();
                ShowCheckingModes();
                //ReplaceVoltageSupplyInStepsDictionary();
                ShowDevicesOnForm();
                UpdateDevicesOnForm();
                DeviceHandler = new DeviceInit(stepsInfo.DeviceList);

            }
            catch (System.Data.OleDb.OleDbException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (System.InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message);
            }
            /*
            catch (System.IO.IOException ex)
            {
                MessageBox.Show(ex.Message);
            }
            
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (FormatException ex)
            {
                MessageBox.Show(ex.Message);
            }
            */
        }

        // TODO use InitialActions(path)
        private void InitialActions()
        {
            string connectionString = "UCA.xlsx;";
            InitialActions(connectionString);
        }

        private Log CreateLog()
        {
            var log = new Log();
            log.AddItem($"Время начала проверки: {DateTime.Now}\n");
            log.AddItem($"Имя оператора: {settings.OperatorName}\n");
            log.AddItem($"Комментарий: {settings.Comment}\n");
            log.AddItem($"Заводской номер: {settings.FactoryNumber}\n");
            log.AddItem($"Режим: {settings.Regime}\r\n");
            return log;
        }

        private void DoCheckingStepByStep(string modeName)//(Dictionary<string, List<Step>> stepsDictionary)
        {
            var log = CreateLog();
            var stepsDictionary = stepsInfo.ModesDictionary[modeName];
            foreach (var tableName in stepsDictionary.Keys)
            {
                foreach (var step in stepsDictionary[tableName])
                {
                    DoStepOfChecking(step, stepsInfo, log);
                    Thread.Sleep(0);
                }
            }
            MessageBox.Show("Проверка завершена, результаты проверки записаны в файл. ОК исправен.");
            BlockControls(false);
        }

        private void ChangeControlState(Control control, bool state)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate { ChangeControlState(control, state); });
                return;
            }
            control.Enabled = state;
        }

        private void DoStepOfChecking(Step step, StepsInfo stepsInfo, Log log)
        {
            if (step.ShowStep || checkBoxDebug.Checked)
            {
                var node = treeviewStepNode[step];
                var indexOf = node.Text.IndexOf(' ');
                var stepNumber = int.Parse(node.Text.Substring(0, indexOf));
                HighlightTreeNode(node, Color.Blue);
                var stepParser = new StepParser(DeviceHandler, step);
                var deviceResult = stepParser.DoStep();
                AddSubTreeNode(node, deviceResult.Description);
                if (deviceResult.State == DeviceStatus.OK)
                {
                    HighlightTreeNode(node, Color.Green);
                }
                else
                {
                    HighlightTreeNode(node, Color.Red);
                    // Аварийная остановка проверки
                    var description = $"В ходе проверки произошла ошибка:\r\nШаг {stepNumber}: {step.Description}\r\nРезультат шага: {deviceResult.Description}\r\nОстановить проверку?";
                    var dialogResult = MessageBox.Show(description, "Внимание!", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        AddSubTreeNode(node, "Аварийная остановка проверки");
                        MessageBox.Show("Проверка остановлена. ОК неисправен.");
                        AbortChecking();
                    }
                    else if (dialogResult == DialogResult.No)
                    {
                        PauseChecking();
                    }
                }
                var result = $"Шаг {stepNumber}: {step.Description}\r\n{deviceResult.Description}\r\n\r\n";
                log.AddItem(result);
            }
            else
            {
                var stepParser = new StepParser(DeviceHandler, step);
                var deviceResult = stepParser.DoStep();
                if (deviceResult.State == DeviceStatus.ERROR)
                {
                    // Аварийная остановка проверки
                    var description = $"В ходе проверки произошла ошибка:\r\nШаг: {step.Description}\r\nРезультат шага: {deviceResult.Description}\r\nОстановить проверку?";
                    var dialogResult = MessageBox.Show(description, "Внимание!", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        //AddSubTreeNode(node, "Аварийная остановка проверки");
                        MessageBox.Show("Проверка остановлена. ОК неисправен.");
                        AbortChecking();
                    }
                    else if (dialogResult == DialogResult.No)
                    {
                        PauseChecking();
                    }
                    //AbortChecking();
                    //Thread.CurrentThread.Abort();
                }
            }
        }

        /*
        private void ReplaceVoltageSupplyInStepsDictionary()
        {
            var stepsDictionary = stepsInfo.StepsDictionary;
            foreach (var tableName in stepsDictionary.Keys)
            {
                foreach (var step in stepsDictionary[tableName])
                {
                    if (tableName == "'Установка напряжения питания'" && step.Command == "SetVoltage" && step.Device.Contains("power") || step.Device.Contains("Power"))
                    {
                        var voltage = stepsInfo.VoltageSupplyDictionary[comboBoxVoltageSupply.SelectedItem.ToString()];
                        step.Argument = voltage.ToString();
                        step.Description = $"Установка напряжения питания {step.Argument} на ИП1";
                    }
                }
            }
            stepsInfo.StepsDictionary = stepsDictionary;
        }
        */

        private void DoStepList(List<Step> stepList)
        {
            foreach (var step in stepList)
            {
                var stepParser = new StepParser(DeviceHandler, step);
                stepParser.DoStep();
            }
        }

        private void DoStepList(List<Step> stepList, TreeNode node)
        {
            foreach (var step in stepList)
            {
                var stepParser = new StepParser(DeviceHandler, step);
                var result = stepParser.DoStep();
                AddSubTreeNode(node, result.Description);
                if (result.State == DeviceStatus.OK)
                {
                    HighlightTreeNode(node, Color.Green);
                }
            }
        }

        #region TreeNodes

        private void FillTreeView(TreeView treeView, Dictionary<string, List<Step>> stepDictionary)
        {
            treeviewNodeStep.Clear();
            treeviewStepNode.Clear();
            treeView.Nodes.Clear();
            treeView.BeginUpdate();
            var nodesCount = 0;
            foreach (var tableName in stepDictionary.Keys)
            {
                var treeNode = new TreeNode(tableName);
                treeView.Nodes.Add(treeNode);
                foreach (var step in stepDictionary[tableName])
                {
                    if (step.ShowStep || checkBoxDebug.Checked)
                    {
                        nodesCount++;
                        try
                        {
                            var nodeName = $"{nodesCount} {step.Description}";
                            var stepNode = new TreeNode(nodeName);
                            treeviewNodeStep.Add(stepNode, step);
                            //stepNode.Checked = true;
                            treeviewStepNode.Add(step, stepNode);
                            treeNode.Nodes.Add(stepNode);
                            treeNode.Expand();
                        }
                        catch (ArgumentException ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                }

            }
            treeView.EndUpdate();
        }

        private void HighlightTreeNode(TreeNode treeNode, Color color)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate { HighlightTreeNode(treeNode, color); });
                return;
            }
            var imageKey = "ok";
            treeNode.ForeColor = color;
        }

        private void AddSubTreeNode(TreeNode parentTreeNode, string stepResult)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate { AddSubTreeNode(parentTreeNode, stepResult); });
                return;
            }
            parentTreeNode.Nodes.Add(stepResult);
            parentTreeNode.Expand();
        }

        #endregion

        public static Form1 EventSend;

        private void buttonCheckingStart_Click(object sender, EventArgs e)
        {
            BlockControls(true);
            var modeName = comboBoxCheckingMode.SelectedItem.ToString();          
            mainThread = new Thread(delegate () { DoCheckingStepByStep(modeName); });
            mainThread.Start();
        }

        private void BlockControls(bool state)
        {
            ChangeControlState(buttonStop, state);
            ChangeControlState(buttonCheckingPause, state);
            ChangeControlState(buttonCheckingStart, !state);
            ChangeControlState(buttonOpenDataBase, !state);
            ChangeControlState(checkBoxDebug, !state);
            ChangeControlState(groupBoxPreferences, !state);
            ChangeControlState(groupBoxManualStep, !state);
        }

        private void buttonOpenDataBase_Click(object sender, EventArgs e)
        {
            DoStepList(stepsInfo.EmergencyStepList);
            OpenFileDialog openBinFileDialog = new OpenFileDialog();
            openBinFileDialog.Filter = "Файлы *.xls* | *xls*";//"Файлы *.accdb | *.accdb | Файлы *.mdb | *.mdb"; "Файлы *.*db | *.*db"
            if (openBinFileDialog.ShowDialog() == DialogResult.OK)
            {
                CleanAll();
                InitialActions(openBinFileDialog.FileName);
            }
        }

        bool isPause = false;

        private void buttonCheckingPause_Click(object sender, EventArgs e)
        {
            PauseResumeChecking();
        }

        private void PauseChecking()
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate { PauseChecking(); });
                return;
            }
            buttonStop.Enabled = false;
            buttonCheckingPause.Text = "Продолжить";
            mainThread.Suspend();
            if (mainThread.ThreadState == ThreadState.Running)
            {
                mainThread.Suspend();
            }
        }

        private void ResumeChecking()
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate { ResumeChecking(); });
                return;
            }
            buttonStop.Enabled = true;
            buttonCheckingPause.Text = "Пауза";
            mainThread.Resume();
        }

        private void PauseResumeChecking()
        {
            if (mainThread.ThreadState != ThreadState.Suspended)
            {
                PauseChecking();
            }
            else
            {
                ResumeChecking();
            }
        }

        private void buttonCheckingStop_Click(object sender, EventArgs e)
        {
  
        }


        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void comboBoxCheckingMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectCheckingMode(); 
        }

        private void SelectCheckingMode ()
        {
            var modeName = comboBoxCheckingMode.SelectedItem.ToString();
            FillTreeView(treeOfChecking, stepsInfo.ModesDictionary[modeName]);
            if (modeName == "Режим самопроверки")
            {
                labelAttention.ForeColor = Color.Red;
                labelAttention.Text = "Объект контроля должен быть отстыкован!";
                comboBoxVoltageSupply.Enabled = false;
            }
            else
            {
                comboBoxVoltageSupply.Enabled = true;
                labelAttention.Text = "";
            }
            if (modeName == "Полная проверка")
            {
                SetVoltageSupplyMode();
            }    
        }

        private void comboBoxVoltageSupply_SelectedIndexChanged(object sender, EventArgs e)
        {
            //ReplaceVoltageSupplyInStepsDictionary();
            SetVoltageSupplyMode();
        }

        private void SetVoltageSupplyMode()
        {
            var modeName = comboBoxVoltageSupply.SelectedItem.ToString();
            FillTreeView(treeOfChecking, stepsInfo.VoltageSupplyModesDictionary[modeName]);
        }

        private void AbortChecking()
        {
            DoStepList(stepsInfo.EmergencyStepList);
            CleanTreeView();
            BlockControls(false);
            if (mainThread.ThreadState == ThreadState.Running)
                mainThread.Abort();
        }

        private void CleanTreeView()
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate { CleanTreeView(); });
                return;
            }
            treeOfChecking.Nodes.Clear();
            SelectCheckingMode();
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            AbortChecking();
        }

        private void Form1_Closing(object sender, CancelEventArgs e)
        {
            AbortChecking();
        }

        private void treeOfChecking_AfterSelect(object sender, TreeViewEventArgs e)
        {
            /*
            if (treeviewNodeStep.ContainsKey(e.Node) && mainThread.ThreadState != ThreadState.Running)
            {
                var thisStep = treeviewNodeStep[e.Node];
                DoStepOfChecking(thisStep, stepsInfo, mainLog);
            }
            */
        }

        public void treeOfChecking_AfterCheckNode(object sender, TreeViewEventArgs e)
        {
            e.Node.Text = "meow";
            var state = e.Node.Checked;
            foreach (TreeNode node in e.Node.Nodes)
            {
                node.Checked = state;
            }
        }

        private List<Step> GetSelectedSteps()
        {
            var stepList = new List<Step>();
            foreach (var node in treeviewNodeStep.Keys)
            {
                if (node.Checked)
                {
                    var step = treeviewNodeStep[node];
                    stepList.Add(step);
                }
            }
            return stepList;
        }

        private void DoSelectedSteps(List<Step> stepList)
        {
            var log = CreateLog();
            log.AddItem("Выполнение выбранных оператором шагов проверки: \r\n");
            foreach (var step in stepList)
            {
                DoStepOfChecking(step, stepsInfo, log);
            }
            while (checkBoxCycle.Checked)
            {
                foreach (var step in stepList)
                {
                    DoStepOfChecking(step, stepsInfo, log);
                }
            }
            BlockControls(false);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var stepList = GetSelectedSteps();
            BlockControls(true);
            mainThread = new Thread(delegate () { DoSelectedSteps(stepList); });
            mainThread.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            AbortChecking();
            CleanTreeView();
        }

        private void checkBoxDebug_CheckedChanged(object sender, EventArgs e)
        {
            SelectCheckingMode();
        }
    }
}
