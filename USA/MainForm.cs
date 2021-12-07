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

        #region Глобальные переменные

        static Dictionary<TreeNode, Step> treeviewNodeStep = new Dictionary<TreeNode, Step>();
        static Dictionary<Step, TreeNode> treeviewStepNode = new Dictionary<Step, TreeNode>();
        static Dictionary<DeviceNames, Label> deviceLabelDictionary = new Dictionary<DeviceNames, Label>();
        static StepsInfo stepsInfo;
        static DeviceInit DeviceHandler;
        Settings settings = new Settings();

        static Form1 form;
        static Log log;

        Thread mainThread = new Thread(some);//stepsDictionary
        static Queue<Step> queue = new Queue<Step>();
        static Form1 EventSend;
        static bool checkingResult = true;
        static bool isCheckingStarted = false;
        static bool isCheckingInterrupted = false;

        private static void some()
        {
            while (true)
            {
                Step step = null;
                lock (queue)
                {
                    if (queue.Count != 0 && isCheckingStarted)
                    {
                        step = queue.Dequeue();
                    }
                }
                if (step != null)
                {
                    var stepResult = DoStep(step);
                    if (step.ShowStep) //|| form.checkBoxDebug.Checked
                    {
                        ShowStepResult(step, stepResult);
                    }
                    //TODO убери нах
                    //Thread.Sleep(1000);
                }
                else if (isCheckingStarted)
                {
                    isCheckingStarted = false;
                    form.CleanTreeView();
                    form.BlockControls(false);
                    var result = checkingResult ? "ОК исправен." : "ОК неисправен";
                    if (isCheckingInterrupted)
                    {
                        result = $"Проверка прервана, результаты проверки записаны в файл.";
                    }
                    else
                    {
                        result = $"Проверка завершена, результаты проверки записаны в файл. {result}";
                    }
                    MessageBox.Show(result);
                    log.Send(result);
                }
                else
                {
                    Thread.Sleep(42);
                }
            }
        }

        #endregion

        #region Конструктор Form1
        public Form1(Settings settings)
        {
            //
            form = this;
            //
            InitializeComponent();
            EventSend = this;
            this.settings = settings;
            //log = new Log(form.settings);
            ShowSettings();
            InitialActions();
            buttonStop.Enabled = false;
            buttonCheckingPause.Enabled = false;
            //
            mainThread.Start();
            //
        }

        #endregion

        #region Показать режимы и настройки

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

        private void SelectCheckingMode()
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

        private void SetVoltageSupplyMode()
        {
            var modeName = comboBoxVoltageSupply.SelectedItem.ToString();
            FillTreeView(treeOfChecking, stepsInfo.VoltageSupplyModesDictionary[modeName]);
        }

        #endregion

        #region Устройства
        private void InitDevices()
        {
            DeviceHandler = new DeviceInit(stepsInfo.DeviceList);
            foreach (var device in stepsInfo.DeviceList)
            {
                if (device.Status == DeviceStatus.OK)
                {
                    break;
                }
                //else if (device.Status == DeviceStatus.ERROR)
            }
            InitDevices();
        }

        private void ShowDevicesOnForm()
        {
            var deviceList = stepsInfo.DeviceList;
            foreach (var device in deviceList)
            {
                if (device.Name.ToString() == "None")
                    continue;
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
                if (device.Name.ToString() == "None")
                    continue;
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

        #endregion

        #region Инициализация 

        private void InitialActions(string pathToDataBase)
        {
            try
            {
                string connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.16.0;Data Source={0}; Extended Properties=Excel 12.0;", pathToDataBase);//"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + pathToDataBase;
                var dbReader = new DBReader(connectionString);
                var dataSet = dbReader.GetDataSet();
                stepsInfo = Step.GetStepsInfo(dataSet);
                SetVoltageSupplyModes();
                ShowCheckingModes();
                //ReplaceVoltageSupplyInStepsDictionary();
                ShowDevicesOnForm();
                //InitDevices();
                UpdateDevicesOnForm();
                DeviceHandler = new DeviceInit(stepsInfo.DeviceList);
                UpdateDevicesOnForm();
                //DeviceHandler = new DeviceInit(stepsInfo.DeviceList);
            }
            catch (System.Data.OleDb.OleDbException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (System.InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message);
            }
            
            catch (System.IO.IOException ex)
            {
                MessageBox.Show(ex.Message);
            }
            
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }            
            /*         
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

        private void InitialActions()
        {
            string connectionString = "UCA.xlsx;";
            InitialActions(connectionString);
        }

        #endregion

        #region Логирование

        private void CreateLog()
        {
            log = new Log(settings);
            log.Send($"Время начала проверки: {DateTime.Now}\n");
            log.Send($"Имя оператора: {settings.OperatorName}\n");
            log.Send($"Комментарий: {settings.Comment}\n");
            log.Send($"Заводской номер: {settings.FactoryNumber}\n");
            var modeName = GetModeName();
            log.Send($"Режим: {modeName}\r\n");
        }

        private string GetModeName()
        {
            var modeName = comboBoxCheckingMode.SelectedItem.ToString();
            if (modeName != "Режим самопроверки")
                modeName = comboBoxVoltageSupply.SelectedItem.ToString();
            return modeName;
        }

        #endregion

        #region Проверка

        private void EnQueueCheckingSteps(string modeName)//(Dictionary<string, List<Step>> stepsDictionary)
        {
            var stepsDictionary = (modeName == "Режим самопроверки") ? stepsInfo.ModesDictionary[modeName] : stepsInfo.VoltageSupplyModesDictionary[modeName];
            foreach (var tableName in stepsDictionary.Keys)
            {
                foreach (var step in stepsDictionary[tableName])
                {
                    lock (queue)
                    {
                        queue.Enqueue(step);
                    }
                }
            }
        }

        private static void ShowStepResult(Step step, DeviceResult deviceResult)
        {
            var node = treeviewStepNode[step];
            form.HighlightTreeNode(node, Color.Blue);
            var indexOf = node.Text.IndexOf(' ');
            var stepNumber = int.Parse(node.Text.Substring(0, indexOf));
            var result = $"Шаг {stepNumber}: {step.Description}\r\n{deviceResult.Description}\r\n\r\n";
            log.Send(result);
            form.AddSubTreeNode(node, deviceResult.Description);
            if (deviceResult.State == DeviceStatus.OK)
            {
                form.HighlightTreeNode(node, Color.Green);
            }
            if (deviceResult.State == DeviceStatus.ERROR)
            {
                form.HighlightTreeNode(node, Color.Red);
            }
        }

        private static void ShowErrorDialog(string description)
        {
            var dialogResult = MessageBox.Show(description, "Внимание!", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                //form.AddSubTreeNode(node, "Аварийная остановка проверки");
                MessageBox.Show("Проверка остановлена. ОК неисправен.");
                form.AbortChecking();
            }
            else if (dialogResult == DialogResult.No)
            {
                isCheckingStarted = true;
                form.ChangeControlState(form.groupBoxManualStep, true);
                //form.ChangeCheckingState(false);
            }
        }

        private static DeviceResult DoStep(Step step)
        {
            var stepParser = new StepParser(DeviceHandler, step);
            var deviceResult = stepParser.DoStep();
            if (deviceResult.State == DeviceStatus.ERROR)
            {             
                checkingResult = false;
                if (!form.checkBoxIgnoreErrors.Checked)
                {
                    isCheckingInterrupted = true;
                    isCheckingStarted = false;
                    var description = $"В ходе проверки произошла ошибка:\r\nШаг: {step.Description}\r\nРезультат шага: {deviceResult.Description}\r\nОстановить проверку?";
                    ShowErrorDialog(description);
                }
            }
            return deviceResult;
        }

        #endregion

        #region Выборочная проверка

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
            CreateLog();
            log.Send("Выполнение выбранных оператором шагов проверки: \r\n");
            foreach (var step in stepList)
            {
                lock (queue)
                {
                    queue.Enqueue(step);
                }
            }
            while (checkBoxCycle.Checked)
            {
                foreach (var step in stepList)
                {
                    lock (queue)
                    {
                        queue.Enqueue(step);
                    }
                }
            }
            BlockControls(false);
        }

        private void DoStepList(List<Step> stepList)
        {
            foreach (var step in stepList)
            {
                lock (queue)
                {
                    queue.Enqueue(step);
                }
            }
        }

        #endregion

        #region Управление потоком проверки

        private void AbortChecking()
        {         
            isCheckingStarted = false;
            ChangeControlState(buttonStop, isCheckingStarted);
            lock (queue)
            {
                queue.Clear();
                isCheckingStarted = true;
                foreach (var step in stepsInfo.EmergencyStepList)
                {
                    queue.Enqueue(step);
                }
            }
            CleanTreeView();
            BlockControls(false);
        }

        private void ChangeCheckingState()
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate { ChangeCheckingState(); });
                return;
            }
            isCheckingStarted = !isCheckingStarted;
            var buttonText = isCheckingStarted ? "Пауза" : "Продолжить";
            buttonCheckingPause.Text = buttonText;
        }

        private void ChangeCheckingState(bool state)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate { ChangeCheckingState(state); });
                return;
            }
            isCheckingStarted = state;
            var buttonText = isCheckingStarted ? "Пауза" : "Продолжить";
            buttonCheckingPause.Text = buttonText;
        }

        #endregion

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
            //parentTreeNode.Expand();
        }

        #endregion

        #region Блокировка и очистка элементов формы

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

        private void CleanAll()
        {
            treeOfChecking.Nodes.Clear();
            comboBoxCheckingMode.Items.Clear();
            comboBoxVoltageSupply.Items.Clear();
            treeviewNodeStep.Clear();
            treeviewStepNode.Clear();
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
        private void ChangeControlState(Control control, bool state)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate { ChangeControlState(control, state); });
                return;
            }
            control.Enabled = state;
        }

        #endregion     

        #region Методы элементов управления

        private void buttonCheckingStart_Click(object sender, EventArgs e)
        {
            checkingResult = true;
            isCheckingInterrupted = false;
            CreateLog();
            var modeName = GetModeName();
            EnQueueCheckingSteps(modeName);
            isCheckingStarted = true;
            BlockControls(isCheckingStarted);
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
        private void buttonCheckingPause_Click(object sender, EventArgs e)
        {
            ChangeCheckingState();
        }

        private void comboBoxCheckingMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectCheckingMode();
        }

        private void comboBoxVoltageSupply_SelectedIndexChanged(object sender, EventArgs e)
        {
            //ReplaceVoltageSupplyInStepsDictionary();
            SetVoltageSupplyMode();
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            isCheckingInterrupted = true;
            AbortChecking();      
        }

        private void Form1_FormClosing(object sender, CancelEventArgs e)
        {
            AbortChecking();

        }

        private void treeOfChecking_AfterSelect(object sender, TreeViewEventArgs e)
        {
            /*
            if (treeviewNodeStep.ContainsKey(e.Node) && mainThread.ThreadState != ThreadState.Running)
            {
                var thisStep = treeviewNodeStep[e.Node];
                var node = treeviewStepNode[thisStep];
                DoStepOfChecking(thisStep, stepsInfo, mainLog);
            }
            */
            
        }
        private void button1_Click(object sender, EventArgs e)
        {
            var stepList = GetSelectedSteps();
            BlockControls(true);
            DoSelectedSteps(stepList);

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

        #endregion

        #region Закомментированные методы
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
        #endregion

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            isCheckingInterrupted = true;
            if (queue.Count != 0)
            {
                AbortChecking();
                Thread.Sleep(3000);
            }
            if (isCheckingStarted)
            {
                var result = $"Проверка прервана, результаты проверки записаны в файл.";
                log.Send(result);
            }
            Application.Exit();
            mainThread.Abort();

            /*
            while (true)
            {

                if (queue.Count == 0)
                {
                    var result = $"Проверка прервана, результаты проверки записаны в файл.";
                    //log.Send(result);
                    mainThread.Abort();
                    break;
                }   
            }
            */
        }
    }
}
