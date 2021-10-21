using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using UCA.Devices;
using UCA.Logging;
using UCA.Steps;
using static UCA.ControlObjectSettings.ControlObjectSettings;

namespace UCA
{
    public partial class Form1 : Form
    {
        Settings settings = new Settings();
        Dictionary<string, StepsInfo> modeStepDictionary;
 
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

        private void InitialActions(string pathToDataBase)
        {
            try
            {
                string connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0}; Extended Properties=Excel 12.0;", pathToDataBase);//"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + pathToDataBase;
                var dbReader = new DBReader(connectionString);
                var dataSet = dbReader.GetDataSet();
                var allStepsInfo = Step.GetStepsInfo(dataSet);
                var modeStepDictionary = new Dictionary<string, StepsInfo>();
                foreach (var modeName in allStepsInfo.ModesDictionary.Keys)
                {
                    var stepsDictionary = new Dictionary<string, List<Step>>();
                    var tableNames = allStepsInfo.ModesDictionary[modeName];
                    foreach (var tableName in tableNames)
                    {
                        if (tableName.Split(' ').Length > 1)
                            stepsDictionary.Add(tableName, allStepsInfo.StepsDictionary[$"'{tableName}'"]);
                        else
                            stepsDictionary.Add(tableName, allStepsInfo.StepsDictionary[tableName]);
                    }
                    comboBoxCheckingMode.Items.Add(modeName);
                    var stepsInfo = new StepsInfo()
                    {
                        DeviceHandler = allStepsInfo.DeviceHandler,
                        EmergencyStepList = allStepsInfo.EmergencyStepList,
                        StepsDictionary = stepsDictionary,
                        ModesDictionary = allStepsInfo.ModesDictionary,                   
                    };
                    modeStepDictionary.Add(modeName, stepsInfo);
                }
                this.modeStepDictionary = modeStepDictionary;
            }
            catch (System.Data.OleDb.OleDbException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (System.InvalidOperationException ex)
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
            comboBoxCheckingMode.SelectedItem = comboBoxCheckingMode.Items[0];
        }

        // TODO use InitialActions(path)
        private void InitialActions()
        {
            string connectionString = "UCA.xlsx;";
            InitialActions(connectionString);
        }

        private void ExecuteDataSetRowByRow(StepsInfo stepsInfo)//(Dictionary<string, List<Step>> stepsDictionary)
        {
            var log = new Log();
            log.AddItem($"Время начала проверки: {DateTime.Now}\n");
            log.AddItem($"Имя оператора: {settings.OperatorName}\n");
            log.AddItem($"Комментарий: {settings.Comment}\n");
            log.AddItem($"Заводской номер: {settings.FactoryNumber}\n");
            log.AddItem($"Режим: {settings.Regime}\r\n");
            foreach (var tableName in stepsInfo.StepsDictionary.Keys)
            {
                foreach (var step in stepsInfo.StepsDictionary[tableName])
                {
                    DoStepOfChecking(step, stepsInfo, log);
                    stepsInfo.StepNumber++;
                    Thread.Sleep(1500);
                    //MessageBox.Show("meow");
                }
            }
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
                        var nodeName = nodesCount + " " + step.Description;
                        treeView.Nodes[treeNode.Index].Nodes.Add(new TreeNode(nodeName));
                        treeView.Nodes[treeNode.Index].Expand();
                    }
                    catch (ArgumentException ex)
                    {
                        richTextBoxCheckingProtocol.Text += ex;
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
            treeView.Nodes[nodeNumber].Nodes[subNodeNumber].ImageIndex = 2;
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
            //treeView.Nodes[nodeNumber].Nodes[subNodeNumber].Expand();
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

        private void buttonSaveProtocol_Click(object sender, EventArgs e)
        {

        }

        private void buttonCheckingStart_Click(object sender, EventArgs e)
        {
            var modeName = comboBoxCheckingMode.SelectedItem.ToString();
            var stepsInfo = modeStepDictionary[modeName];
            Thread thread = new Thread(delegate () { ExecuteDataSetRowByRow(stepsInfo); });//stepsDictionary
            thread.Start();
        }

        private void buttonOpenDataBase_Click(object sender, EventArgs e)
        {
            OpenFileDialog openBinFileDialog = new OpenFileDialog();
            openBinFileDialog.Filter = "Файлы *.xls* | *xls*";//"Файлы *.accdb | *.accdb | Файлы *.mdb | *.mdb"; "Файлы *.*db | *.*db"
            if (openBinFileDialog.ShowDialog() == DialogResult.OK)
            {
                richTextBoxCheckingProtocol.Text += $"Файл {openBinFileDialog.FileName} открыт";
                InitialActions(openBinFileDialog.FileName);
            }
        }

        public void ShowMessage(string message)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<string>(ShowMessage), new object[] { message });
                return;
            }
            richTextBoxCheckingProtocol.Text += message;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channelNumber"> Номер канала </param>
        /// <param name="inputAction"> Значение входного воздействия при заданном входном воздействии </param>
        /// <param name="inputAction0"> Значение входного воздействия при отсутствии входного воздействия </param>
        /// <param name="outputAction"> Значение выходного воздействия при заданном входном воздействии </param>
        /// <param name="outputAction0"> Значение выходного воздействия при отсутствии входного воздействия </param>
        /// <returns> Возвращает коэффициент преобразования В/мкА </returns>
        public double GetCoefficient(int channelNumber, double outputAction, double outputAction0, double inputAction, double inputAction0)
        {
            if (channelNumber == 1 || channelNumber == 2)
            {
                var totalInputResistance = 470.0;
                return totalInputResistance * (outputAction - outputAction0) / (inputAction - inputAction0);
            }
            if (channelNumber >= 3 && channelNumber <= 10)
            {
                return (outputAction - outputAction0) / (inputAction - inputAction0);
            }
            return -1;
        }

        bool isPause = false;

        private void buttonCheckingPause_Click(object sender, EventArgs e)
        {
            isPause = true;
        }

        private void buttonCheckingStop_Click(object sender, EventArgs e)
        {
  
        }


        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void richTextBoxCheckingProtocol_TextChanged(object sender, EventArgs e)
        {
            richTextBoxCheckingProtocol.SelectionStart = richTextBoxCheckingProtocol.Text.Length;
            richTextBoxCheckingProtocol.ScrollToCaret();
        }

        private void comboBoxCheckingMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            var modeName = comboBoxCheckingMode.SelectedItem.ToString();
            var stepsInfo = modeStepDictionary[modeName];
            FillTreeView(treeOfChecking, stepsInfo.StepsDictionary);
        }
    }
}
