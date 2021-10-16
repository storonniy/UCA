
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.режимПроверкиToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.самопроверкаААПToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.режимЦиклическойПроверкиToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.оПрограммеToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.richTextBoxCheckingProtocol = new System.Windows.Forms.RichTextBox();
            this.buttonCheckingStart = new System.Windows.Forms.Button();
            this.buttonCheckingStop = new System.Windows.Forms.Button();
            this.buttonCheckingPause = new System.Windows.Forms.Button();
            this.buttonSaveProtocol = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.buttonOpenDataBase = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.treeOfChecking = new System.Windows.Forms.TreeView();
            this.groupBoxCheckingManagement = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.labelControlObjectName = new System.Windows.Forms.Label();
            this.labelRegime = new System.Windows.Forms.Label();
            this.labelComment = new System.Windows.Forms.Label();
            this.labelFactoryNumber = new System.Windows.Forms.Label();
            this.labelOperatorName = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBoxCheckingManagement.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.режимПроверкиToolStripMenuItem,
            this.оПрограммеToolStripMenuItem});
            resources.ApplyResources(this.menuStrip1, "menuStrip1");
            this.menuStrip1.Name = "menuStrip1";
            // 
            // режимПроверкиToolStripMenuItem
            // 
            this.режимПроверкиToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.самопроверкаААПToolStripMenuItem,
            this.режимЦиклическойПроверкиToolStripMenuItem});
            this.режимПроверкиToolStripMenuItem.Name = "режимПроверкиToolStripMenuItem";
            resources.ApplyResources(this.режимПроверкиToolStripMenuItem, "режимПроверкиToolStripMenuItem");
            // 
            // самопроверкаААПToolStripMenuItem
            // 
            this.самопроверкаААПToolStripMenuItem.Name = "самопроверкаААПToolStripMenuItem";
            resources.ApplyResources(this.самопроверкаААПToolStripMenuItem, "самопроверкаААПToolStripMenuItem");
            // 
            // режимЦиклическойПроверкиToolStripMenuItem
            // 
            this.режимЦиклическойПроверкиToolStripMenuItem.Name = "режимЦиклическойПроверкиToolStripMenuItem";
            resources.ApplyResources(this.режимЦиклическойПроверкиToolStripMenuItem, "режимЦиклическойПроверкиToolStripMenuItem");
            // 
            // оПрограммеToolStripMenuItem
            // 
            this.оПрограммеToolStripMenuItem.Name = "оПрограммеToolStripMenuItem";
            resources.ApplyResources(this.оПрограммеToolStripMenuItem, "оПрограммеToolStripMenuItem");
            // 
            // richTextBoxCheckingProtocol
            // 
            resources.ApplyResources(this.richTextBoxCheckingProtocol, "richTextBoxCheckingProtocol");
            this.richTextBoxCheckingProtocol.Name = "richTextBoxCheckingProtocol";
            this.richTextBoxCheckingProtocol.TextChanged += new System.EventHandler(this.richTextBoxCheckingProtocol_TextChanged);
            // 
            // buttonCheckingStart
            // 
            resources.ApplyResources(this.buttonCheckingStart, "buttonCheckingStart");
            this.buttonCheckingStart.Name = "buttonCheckingStart";
            this.buttonCheckingStart.UseVisualStyleBackColor = true;
            this.buttonCheckingStart.Click += new System.EventHandler(this.buttonCheckingStart_Click);
            // 
            // buttonCheckingStop
            // 
            resources.ApplyResources(this.buttonCheckingStop, "buttonCheckingStop");
            this.buttonCheckingStop.Name = "buttonCheckingStop";
            this.buttonCheckingStop.UseVisualStyleBackColor = true;
            this.buttonCheckingStop.Click += new System.EventHandler(this.buttonCheckingStop_Click);
            // 
            // buttonCheckingPause
            // 
            resources.ApplyResources(this.buttonCheckingPause, "buttonCheckingPause");
            this.buttonCheckingPause.Name = "buttonCheckingPause";
            this.buttonCheckingPause.UseVisualStyleBackColor = true;
            this.buttonCheckingPause.Click += new System.EventHandler(this.buttonCheckingPause_Click);
            // 
            // buttonSaveProtocol
            // 
            resources.ApplyResources(this.buttonSaveProtocol, "buttonSaveProtocol");
            this.buttonSaveProtocol.Name = "buttonSaveProtocol";
            this.buttonSaveProtocol.UseVisualStyleBackColor = true;
            this.buttonSaveProtocol.Click += new System.EventHandler(this.buttonSaveProtocol_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.labelOperatorName);
            this.panel1.Controls.Add(this.labelFactoryNumber);
            this.panel1.Controls.Add(this.labelComment);
            this.panel1.Controls.Add(this.labelRegime);
            this.panel1.Controls.Add(this.labelControlObjectName);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.richTextBoxCheckingProtocol);
            this.panel1.Controls.Add(this.groupBox2);
            this.panel1.Controls.Add(this.groupBoxCheckingManagement);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // buttonOpenDataBase
            // 
            resources.ApplyResources(this.buttonOpenDataBase, "buttonOpenDataBase");
            this.buttonOpenDataBase.Name = "buttonOpenDataBase";
            this.buttonOpenDataBase.UseVisualStyleBackColor = true;
            this.buttonOpenDataBase.Click += new System.EventHandler(this.buttonOpenDataBase_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.treeOfChecking);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // treeOfChecking
            // 
            resources.ApplyResources(this.treeOfChecking, "treeOfChecking");
            this.treeOfChecking.Name = "treeOfChecking";
            // 
            // groupBoxCheckingManagement
            // 
            this.groupBoxCheckingManagement.Controls.Add(this.buttonOpenDataBase);
            this.groupBoxCheckingManagement.Controls.Add(this.buttonCheckingStop);
            this.groupBoxCheckingManagement.Controls.Add(this.buttonCheckingStart);
            this.groupBoxCheckingManagement.Controls.Add(this.buttonCheckingPause);
            this.groupBoxCheckingManagement.Controls.Add(this.buttonSaveProtocol);
            resources.ApplyResources(this.groupBoxCheckingManagement, "groupBoxCheckingManagement");
            this.groupBoxCheckingManagement.Name = "groupBoxCheckingManagement";
            this.groupBoxCheckingManagement.TabStop = false;
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
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
            // labelControlObjectName
            // 
            resources.ApplyResources(this.labelControlObjectName, "labelControlObjectName");
            this.labelControlObjectName.Name = "labelControlObjectName";
            // 
            // labelRegime
            // 
            resources.ApplyResources(this.labelRegime, "labelRegime");
            this.labelRegime.Name = "labelRegime";
            // 
            // labelComment
            // 
            resources.ApplyResources(this.labelComment, "labelComment");
            this.labelComment.Name = "labelComment";
            // 
            // labelFactoryNumber
            // 
            resources.ApplyResources(this.labelFactoryNumber, "labelFactoryNumber");
            this.labelFactoryNumber.Name = "labelFactoryNumber";
            // 
            // labelOperatorName
            // 
            resources.ApplyResources(this.labelOperatorName, "labelOperatorName");
            this.labelOperatorName.Name = "labelOperatorName";
            // 
            // Form1
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBoxCheckingManagement.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem режимПроверкиToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem самопроверкаААПToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem режимЦиклическойПроверкиToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem оПрограммеToolStripMenuItem;
        private System.Windows.Forms.RichTextBox richTextBoxCheckingProtocol;
        private System.Windows.Forms.Button buttonCheckingStart;
        private System.Windows.Forms.Button buttonCheckingStop;
        private System.Windows.Forms.Button buttonCheckingPause;
        private System.Windows.Forms.Button buttonSaveProtocol;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBoxCheckingManagement;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TreeView treeOfChecking;
        private System.Windows.Forms.Button buttonOpenDataBase;
        private System.Windows.Forms.Label labelOperatorName;
        private System.Windows.Forms.Label labelFactoryNumber;
        private System.Windows.Forms.Label labelComment;
        private System.Windows.Forms.Label labelRegime;
        private System.Windows.Forms.Label labelControlObjectName;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
    }
}

