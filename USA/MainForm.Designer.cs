
namespace UCA
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.buttonCheckingStart = new System.Windows.Forms.Button();
            this.buttonCheckingPause = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBoxPreferences = new System.Windows.Forms.GroupBox();
            this.checkBoxCycle = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.comboBoxCheckingMode = new System.Windows.Forms.ComboBox();
            this.comboBoxVoltageSupply = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.labelOperatorName = new System.Windows.Forms.Label();
            this.labelFactoryNumber = new System.Windows.Forms.Label();
            this.labelComment = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.treeOfChecking = new System.Windows.Forms.TreeView();
            this.groupBoxCheckingManagement = new System.Windows.Forms.GroupBox();
            this.buttonSelectAllSteps = new System.Windows.Forms.Button();
            this.buttonStop = new System.Windows.Forms.Button();
            this.buttonOpenDataBase = new System.Windows.Forms.Button();
            this.checkBoxDebug = new System.Windows.Forms.CheckBox();
            this.button2 = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.groupBoxPreferences.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBoxCheckingManagement.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonCheckingStart
            // 
            resources.ApplyResources(this.buttonCheckingStart, "buttonCheckingStart");
            this.buttonCheckingStart.Name = "buttonCheckingStart";
            this.buttonCheckingStart.UseVisualStyleBackColor = true;
            this.buttonCheckingStart.Click += new System.EventHandler(this.buttonCheckingStart_Click);
            // 
            // buttonCheckingPause
            // 
            resources.ApplyResources(this.buttonCheckingPause, "buttonCheckingPause");
            this.buttonCheckingPause.Name = "buttonCheckingPause";
            this.buttonCheckingPause.UseVisualStyleBackColor = true;
            this.buttonCheckingPause.Click += new System.EventHandler(this.buttonCheckingPause_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBoxPreferences);
            this.panel1.Controls.Add(this.labelOperatorName);
            this.panel1.Controls.Add(this.labelFactoryNumber);
            this.panel1.Controls.Add(this.labelComment);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.groupBox2);
            this.panel1.Controls.Add(this.groupBoxCheckingManagement);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // groupBoxPreferences
            // 
            this.groupBoxPreferences.Controls.Add(this.comboBoxCheckingMode);
            this.groupBoxPreferences.Controls.Add(this.comboBoxVoltageSupply);
            this.groupBoxPreferences.Controls.Add(this.label6);
            this.groupBoxPreferences.Controls.Add(this.label1);
            resources.ApplyResources(this.groupBoxPreferences, "groupBoxPreferences");
            this.groupBoxPreferences.Name = "groupBoxPreferences";
            this.groupBoxPreferences.TabStop = false;
            // 
            // checkBoxCycle
            // 
            resources.ApplyResources(this.checkBoxCycle, "checkBoxCycle");
            this.checkBoxCycle.Name = "checkBoxCycle";
            this.checkBoxCycle.UseVisualStyleBackColor = true;
            this.checkBoxCycle.CheckedChanged += new System.EventHandler(this.checkBoxCycle_CheckedChanged);
            // 
            // button1
            // 
            resources.ApplyResources(this.button1, "button1");
            this.button1.Name = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // comboBoxCheckingMode
            // 
            this.comboBoxCheckingMode.FormattingEnabled = true;
            resources.ApplyResources(this.comboBoxCheckingMode, "comboBoxCheckingMode");
            this.comboBoxCheckingMode.Name = "comboBoxCheckingMode";
            this.comboBoxCheckingMode.SelectedIndexChanged += new System.EventHandler(this.comboBoxCheckingMode_SelectedIndexChanged);
            // 
            // comboBoxVoltageSupply
            // 
            this.comboBoxVoltageSupply.FormattingEnabled = true;
            resources.ApplyResources(this.comboBoxVoltageSupply, "comboBoxVoltageSupply");
            this.comboBoxVoltageSupply.Name = "comboBoxVoltageSupply";
            this.comboBoxVoltageSupply.SelectedIndexChanged += new System.EventHandler(this.comboBoxVoltageSupply_SelectedIndexChanged);
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // labelOperatorName
            // 
            resources.ApplyResources(this.labelOperatorName, "labelOperatorName");
            this.labelOperatorName.Name = "labelOperatorName";
            // 
            // labelFactoryNumber
            // 
            resources.ApplyResources(this.labelFactoryNumber, "labelFactoryNumber");
            this.labelFactoryNumber.Name = "labelFactoryNumber";
            // 
            // labelComment
            // 
            resources.ApplyResources(this.labelComment, "labelComment");
            this.labelComment.Name = "labelComment";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.button2);
            this.groupBox2.Controls.Add(this.checkBoxDebug);
            this.groupBox2.Controls.Add(this.checkBoxCycle);
            this.groupBox2.Controls.Add(this.buttonSelectAllSteps);
            this.groupBox2.Controls.Add(this.button1);
            this.groupBox2.Controls.Add(this.treeOfChecking);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // treeOfChecking
            // 
            this.treeOfChecking.CheckBoxes = true;
            resources.ApplyResources(this.treeOfChecking, "treeOfChecking");
            this.treeOfChecking.Name = "treeOfChecking";
            this.treeOfChecking.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeOfChecking_AfterSelect);
            // 
            // groupBoxCheckingManagement
            // 
            this.groupBoxCheckingManagement.Controls.Add(this.buttonStop);
            this.groupBoxCheckingManagement.Controls.Add(this.buttonOpenDataBase);
            this.groupBoxCheckingManagement.Controls.Add(this.buttonCheckingStart);
            this.groupBoxCheckingManagement.Controls.Add(this.buttonCheckingPause);
            resources.ApplyResources(this.groupBoxCheckingManagement, "groupBoxCheckingManagement");
            this.groupBoxCheckingManagement.Name = "groupBoxCheckingManagement";
            this.groupBoxCheckingManagement.TabStop = false;
            // 
            // buttonSelectAllSteps
            // 
            resources.ApplyResources(this.buttonSelectAllSteps, "buttonSelectAllSteps");
            this.buttonSelectAllSteps.Name = "buttonSelectAllSteps";
            this.buttonSelectAllSteps.UseVisualStyleBackColor = true;
            this.buttonSelectAllSteps.Click += new System.EventHandler(this.buttonSelectAllSteps_Click);
            // 
            // buttonStop
            // 
            resources.ApplyResources(this.buttonStop, "buttonStop");
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.UseVisualStyleBackColor = true;
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // buttonOpenDataBase
            // 
            resources.ApplyResources(this.buttonOpenDataBase, "buttonOpenDataBase");
            this.buttonOpenDataBase.Name = "buttonOpenDataBase";
            this.buttonOpenDataBase.UseVisualStyleBackColor = true;
            this.buttonOpenDataBase.Click += new System.EventHandler(this.buttonOpenDataBase_Click);
            // 
            // checkBoxDebug
            // 
            resources.ApplyResources(this.checkBoxDebug, "checkBoxDebug");
            this.checkBoxDebug.Checked = true;
            this.checkBoxDebug.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxDebug.Name = "checkBoxDebug";
            this.checkBoxDebug.Tag = "Показывает неинформативные шаги проверки";
            this.checkBoxDebug.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            resources.ApplyResources(this.button2, "button2");
            this.button2.Name = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // Form1
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Name = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBoxPreferences.ResumeLayout(false);
            this.groupBoxPreferences.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBoxCheckingManagement.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button buttonCheckingStart;
        private System.Windows.Forms.Button buttonCheckingPause;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBoxCheckingManagement;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TreeView treeOfChecking;
        private System.Windows.Forms.Button buttonOpenDataBase;
        private System.Windows.Forms.Label labelOperatorName;
        private System.Windows.Forms.Label labelFactoryNumber;
        private System.Windows.Forms.Label labelComment;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBoxVoltageSupply;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBoxCheckingMode;
        private System.Windows.Forms.Button buttonStop;
        private System.Windows.Forms.GroupBox groupBoxPreferences;
        private System.Windows.Forms.Button buttonSelectAllSteps;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox checkBoxCycle;
        private System.Windows.Forms.CheckBox checkBoxDebug;
        private System.Windows.Forms.Button button2;
    }
}

