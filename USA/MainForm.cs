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
        Settings settings = new Settings();
        Dictionary<TreeNode, Step> treeviewNodeStep = new Dictionary<TreeNode, Step>();
        Dictionary<Step, TreeNode> treeviewStepNode = new Dictionary<Step, TreeNode>();
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

        private void SetVoltageSupplyModes(StepsInfo allStepsInfo)
        {
            foreach (var modeName in allStepsInfo.VoltageSupplyDictionary.Keys)
            {
                comboBoxVoltageSupply.Items.Add(modeName);
            }
            comboBoxVoltageSupply.SelectedItem = comboBoxVoltageSupply.Items[1];
        }

        private void ShowCheckingModes(StepsInfo stepsInfo)
        {
            foreach (var modeName in stepsInfo.ModesDictionary.Keys)
            {
                comboBoxCheckingMode.Items.Add(modeName);
            }
            comboBoxCheckingMode.SelectedItem = comboBoxCheckingMode.Items[1];
        }

        private void InitialActions(string pathToDataBase)
        {
            try
            {
                string connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0}; Extended Properties=Excel 12.0;", pathToDataBase);//"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + pathToDataBase;
                var dbReader = new DBReader(connectionString);
                var dataSet = dbReader.GetDataSet();
                this.stepsInfo = Step.GetStepsInfo(dataSet);
                ShowCheckingModes(stepsInfo);
                SetVoltageSupplyModes(stepsInfo);
                ReplaceVoltageSupplyInStepsDictionary();
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

        private void DoCheckingStepByStep(StepsInfo stepsInfo, string modeName)//(Dictionary<string, List<Step>> stepsDictionary)
        {
            var log = CreateLog();
            var stepsDictionary = stepsInfo.ModesDictionary[modeName];
            var stepNumber = 0;
            foreach (var tableName in stepsDictionary.Keys)
            {
                foreach (var step in stepsDictionary[tableName])
                {
                    DoStepOfChecking(step, stepsInfo, log);
                    Thread.Sleep(0);
                }
            }
            /*
            buttonCheckingStart.Enabled = true;
            buttonCheckingStart.Enabled = false;
            buttonStop.Enabled = false;
            buttonOpenDataBase.Enabled = true;
            checkBoxDebug.Enabled = true;
            */
            MessageBox.Show("Проверка завершена, результаты проверки записаны в файл");
        }
        private void DoStepOfChecking(Step step, StepsInfo stepsInfo, Log log)
        {
            if (step.ShowStep || checkBoxDebug.Checked)
            {
                var node = treeviewStepNode[step];
                var indexOf = node.Text.IndexOf(' ');
                var stepNumber = int.Parse(node.Text.Substring(0, indexOf));
                HighlightTreeNode(treeOfChecking, node, Color.Blue);
                var stepParser = new StepParser(stepsInfo.DeviceHandler, step);
                var deviceResult = stepParser.DoStep();
                UpdateTreeNode(node, deviceResult.Description);
                if (deviceResult.State == DeviceState.OK)
                {
                    HighlightTreeNode(treeOfChecking, node, Color.Green);
                }
                else
                {
                    HighlightTreeNode(treeOfChecking, node, Color.Red);
                    // Аварийная остановка проверки

                    //EmergencyBreak(stepsInfo);
                    //UpdateTreeNodes(treeOfChecking, nodeNumbers, "Аварийная остановка проверки");
                    //Thread.CurrentThread.Abort();

                }

                var result = $"Шаг {stepNumber}: {step.Description}\r\n{deviceResult.Description}\r\n\r\n";
                log.AddItem(result);
            }
            else
            {
                var stepParser = new StepParser(stepsInfo.DeviceHandler, step);
                var deviceResult = stepParser.DoStep();
                if (deviceResult.State == DeviceState.ERROR)
                {
                    // Аварийная остановка проверки

                    //EmergencyBreak(stepsInfo);
                    //UpdateTreeNodes(treeOfChecking, nodeNumbers, "Аварийная остановка проверки");
                    //Thread.CurrentThread.Abort();
                }
            }
        }

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

        private void EmergencyBreak(StepsInfo stepsInfo)
        {
            foreach (var step in stepsInfo.EmergencyStepList)
            {
                var stepParser = new StepParser(stepsInfo.DeviceHandler, step);
                var result = stepParser.DoStep();
                //var node = treeviewStepNode[step];
                //UpdateTreeNode(node, result.Description);
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

        private void HighlightTreeNode(TreeView treeView, TreeNode treeNode, Color color)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate { HighlightTreeNode(treeView, treeNode, color); });
                return;
            }
            var imageKey = "ok";
            //treeView.ImageList.Images.Add(imageKey, new System.Drawing.Icon(Application.StartupPath + "\\Images\\ok.png"));
            //treeView.Nodes[nodeNumber].Nodes[subNodeNumber].ImageIndex = 0;
            //treeView.Nodes[nodeNumber].EnsureVisible();
            treeNode.ForeColor = color;
            //treeView.Nodes[nodeNumber].Nodes[subNodeNumber].ForeColor = color;
        }

        private void UpdateTreeNode(TreeNode treeNode, string stepResult)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate { UpdateTreeNode(treeNode, stepResult); });
                return;
            }
            treeNode.Nodes.Add(stepResult);
            treeNode.Expand();
            /*
            var nodeNumber = nodeNumbers[0];
            var subNodeNumber = nodeNumbers[1];
            treeView.Nodes[nodeNumber].Nodes[subNodeNumber].Nodes.Add(stepResult);
            treeView.Nodes[nodeNumber].Nodes[subNodeNumber].Expand();
            */
        }

        #endregion

        public static Form1 EventSend;

        private void buttonCheckingStart_Click(object sender, EventArgs e)
        {
            buttonCheckingStart.Enabled = false;
            buttonStop.Enabled = true;
            buttonCheckingPause.Enabled = true;
            buttonOpenDataBase.Enabled = false;
            checkBoxDebug.Enabled = false;
            var modeName = comboBoxCheckingMode.SelectedItem.ToString();          
            mainThread = new Thread(delegate () { DoCheckingStepByStep(stepsInfo, modeName); });
            mainThread.Start();
        }

        private void buttonOpenDataBase_Click(object sender, EventArgs e)
        {
            EmergencyBreak(stepsInfo);
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
            if (mainThread.ThreadState != ThreadState.Suspended)
            {
                buttonStop.Enabled = false;
                buttonCheckingPause.Text = "Продолжить";
                mainThread.Suspend();
            }
            else
            {
                buttonStop.Enabled = true;
                buttonCheckingPause.Text = "Пауза";
                mainThread.Resume();
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
            }
            else
            {
                labelAttention.Text = "";
            }
        }

        private void comboBoxVoltageSupply_SelectedIndexChanged(object sender, EventArgs e)
        {
            ReplaceVoltageSupplyInStepsDictionary();
        }

        private void Stop()
        {
            var modeName = comboBoxCheckingMode.SelectedItem.ToString();
            EmergencyBreak(stepsInfo);
            mainThread.Abort();
        }

        private void CleanTreeView()
        {
            treeOfChecking.Nodes.Clear();
            FillTreeView(treeOfChecking, stepsInfo.StepsDictionary);
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            CleanTreeView();
            buttonCheckingStart.Enabled = true;
            buttonOpenDataBase.Enabled = true;
            Stop();
        }

        private void Form1_Closing(object sender, CancelEventArgs e)
        {
            Stop();
            Close();
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

        private void buttonSelectAllSteps_Click(object sender, EventArgs e)
        {

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
                Thread.Sleep(1000);
                DoStepOfChecking(step, stepsInfo, log);
            }
            while (checkBoxCycle.Checked)
            {
                foreach (var step in stepList)
                {
                    Thread.Sleep(1000);
                    DoStepOfChecking(step, stepsInfo, log);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var stepList = GetSelectedSteps();
            var thread = new Thread(delegate () { DoSelectedSteps(stepList); });
            thread.Start();
        }

        private void checkBoxCycle_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Stop();
            CleanTreeView();
        }

        private void checkBoxDebug_CheckedChanged(object sender, EventArgs e)
        {
            SelectCheckingMode();
        }
    }
}
