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

        Thread mainThread = new Thread(some);//stepsDictionary

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
            comboBoxCheckingMode.SelectedItem = comboBoxCheckingMode.Items[0];
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

        private void DoCheckingStepByStep(StepsInfo stepsInfo, string modeName)//(Dictionary<string, List<Step>> stepsDictionary)
        {
            var log = new Log();
            log.AddItem($"Время начала проверки: {DateTime.Now}\n");
            log.AddItem($"Имя оператора: {settings.OperatorName}\n");
            log.AddItem($"Комментарий: {settings.Comment}\n");
            log.AddItem($"Заводской номер: {settings.FactoryNumber}\n");
            log.AddItem($"Режим: {settings.Regime}\r\n");
            var stepsDictionary = stepsInfo.ModesDictionary[modeName];
            foreach (var tableName in stepsDictionary.Keys)
            {
                foreach (var step in stepsDictionary[tableName])
                {
                    DoStepOfChecking(step, stepsInfo, log);
                    stepsInfo.StepNumber++;
                    Thread.Sleep(1500);
                }
            }
            MessageBox.Show("Проверка завершена, результаты проверки записаны в файл");
        }
        private void DoStepOfChecking(Step step, StepsInfo stepsInfo, Log log)
        {
            HighlightTreeNode(treeOfChecking, GetNodeNumber(treeOfChecking, stepsInfo.StepNumber), Color.Blue);
            var stepParser = new StepParser(stepsInfo.DeviceHandler, step);
            var deviceResult = stepParser.DoStep();
            var nodeNumbers = GetNodeNumber(treeOfChecking, stepsInfo.StepNumber);
            UpdateTreeNodes(treeOfChecking, nodeNumbers, deviceResult.Description);
            if (deviceResult.State == DeviceState.OK)
            {
                HighlightTreeNode(treeOfChecking, nodeNumbers, Color.Green);
            }
            else
            {
                HighlightTreeNode(treeOfChecking, nodeNumbers, Color.Red);
                // Аварийная остановка проверки

                //EmergencyBreak(stepsInfo);
                //UpdateTreeNodes(treeOfChecking, nodeNumbers, "Аварийная остановка проверки");
                //Thread.CurrentThread.Abort();

            }
            log.AddItem($"Шаг {stepsInfo.StepNumber + 1}: {step.Description}\r\n{deviceResult.Description}\r\n\r\n");
        }

        private void EmergencyBreak (StepsInfo stepsInfo)
        {
            foreach (var step in stepsInfo.EmergencyStepList)
            {
                var stepParser = new StepParser(stepsInfo.DeviceHandler, step);
                stepParser.DoStep();
            }
        }

        #region TreeNodes
        private void FillTreeView(TreeView treeView, Dictionary<string, List<Step>> stepDictionary)
        {
            treeView.Nodes.Clear();
            treeView.BeginUpdate();
            var nodesCount = 0;
            foreach (var tableName in stepDictionary.Keys)
            {
                var treeNode = new TreeNode(tableName);
                treeView.Nodes.Add(treeNode);
                foreach (var step in stepDictionary[tableName])
                {
                    nodesCount++;
                    try
                    {
                        var nodeName = $"{nodesCount} {step.Description}";
                        treeView.Nodes[treeNode.Index].Nodes.Add(new TreeNode(nodeName));
                        treeView.Nodes[treeNode.Index].Expand();
                    }
                    catch (ArgumentException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }

            }
            treeView.EndUpdate();
        }

        private void HighlightTreeNode(TreeView treeView, int[] nodeNumbers, Color color)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate { HighlightTreeNode(treeView, nodeNumbers, color); });
                return;
            }
            var nodeNumber = nodeNumbers[0];
            var subNodeNumber = nodeNumbers[1];
            var imageKey = "ok";
            //treeView.ImageList.Images.Add(imageKey, new System.Drawing.Icon(Application.StartupPath + "\\Images\\ok.png"));
            //treeView.Nodes[nodeNumber].Nodes[subNodeNumber].ImageIndex = 0;
            //treeView.Nodes[nodeNumber].EnsureVisible();
            treeView.Nodes[nodeNumber].Nodes[subNodeNumber].ForeColor = color;
        }

        private void UpdateTreeNodes(TreeView treeView, int[] nodeNumbers, string stepResult)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate { UpdateTreeNodes(treeView, nodeNumbers, stepResult); });
                return;
            }
            var nodeNumber = nodeNumbers[0];
            var subNodeNumber = nodeNumbers[1];
            treeView.Nodes[nodeNumber].Nodes[subNodeNumber].Nodes.Add(stepResult);
            treeView.Nodes[nodeNumber].Nodes[subNodeNumber].Expand();
        }

        private int[] GetNodeNumber(TreeView treeView, int stepNumber)
        {
            int subNodeNumber = stepNumber;
            int nodeNumber = 0;
            foreach (TreeNode node in treeView.Nodes)
            {
                nodeNumber = node.Index;
                if (subNodeNumber > node.Nodes.Count - 1)
                {
                    subNodeNumber -= node.Nodes.Count;
                }
                else
                    break;
            }
            return new int[] { nodeNumber, subNodeNumber };
        }

        #endregion

        public static Form1 EventSend;

        private void buttonCheckingStart_Click(object sender, EventArgs e)
        {
            var modeName = comboBoxCheckingMode.SelectedItem.ToString();
            mainThread = new Thread(delegate () { DoCheckingStepByStep(stepsInfo, modeName); });
            mainThread.Start();
        }

        private void buttonOpenDataBase_Click(object sender, EventArgs e)
        {
            OpenFileDialog openBinFileDialog = new OpenFileDialog();
            openBinFileDialog.Filter = "Файлы *.xls* | *xls*";//"Файлы *.accdb | *.accdb | Файлы *.mdb | *.mdb"; "Файлы *.*db | *.*db"
            if (openBinFileDialog.ShowDialog() == DialogResult.OK)
            {
                InitialActions(openBinFileDialog.FileName);
            }
        }

        bool isPause = false;

        private void buttonCheckingPause_Click(object sender, EventArgs e)
        {
            if (mainThread.ThreadState != ThreadState.Suspended)
            {
                buttonCheckingPause.Text = "Продолжить";
                mainThread.Suspend();
            }
            else
            {
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
            var modeName = comboBoxCheckingMode.SelectedItem.ToString();
            FillTreeView(treeOfChecking, stepsInfo.ModesDictionary[modeName]);
        }

        private void comboBoxVoltageSupply_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Stop()
        {
            var modeName = comboBoxCheckingMode.SelectedItem.ToString();
            EmergencyBreak(stepsInfo);
            var nodeNumbers = GetNodeNumber(treeOfChecking, stepsInfo.StepNumber);
            UpdateTreeNodes(treeOfChecking, nodeNumbers, "Аварийная остановка проверки");
            mainThread.Abort();
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            Stop();
        }

        private void Form1_Closing(object sender, CancelEventArgs e)
        {
            Stop();
            Close();
        }
    }
}
