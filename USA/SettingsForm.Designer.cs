
namespace UCA
{
    partial class SettingsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.comboBoxRegime = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxComment = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxOperatorName = new System.Windows.Forms.TextBox();
            this.textBoxFactoryNumber = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonApplySettings = new System.Windows.Forms.Button();
            this.textBoxControlObjectName = new System.Windows.Forms.TextBox();
            this.labelErrorFactoryNumber = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // comboBoxRegime
            // 
            this.comboBoxRegime.BackColor = System.Drawing.SystemColors.Window;
            this.comboBoxRegime.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.comboBoxRegime.FormattingEnabled = true;
            this.comboBoxRegime.Location = new System.Drawing.Point(190, 71);
            this.comboBoxRegime.Name = "comboBoxRegime";
            this.comboBoxRegime.Size = new System.Drawing.Size(179, 24);
            this.comboBoxRegime.TabIndex = 24;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.label6.Location = new System.Drawing.Point(19, 71);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(51, 17);
            this.label6.TabIndex = 23;
            this.label6.Text = "Режим";
            // 
            // textBoxComment
            // 
            this.textBoxComment.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBoxComment.Location = new System.Drawing.Point(190, 118);
            this.textBoxComment.Name = "textBoxComment";
            this.textBoxComment.Size = new System.Drawing.Size(179, 23);
            this.textBoxComment.TabIndex = 22;
            this.textBoxComment.Text = "1";
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label5.Location = new System.Drawing.Point(19, 28);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(166, 17);
            this.label5.TabIndex = 21;
            this.label5.Text = "Обозначение ОК по КД";
            // 
            // textBoxOperatorName
            // 
            this.textBoxOperatorName.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.textBoxOperatorName.Location = new System.Drawing.Point(190, 208);
            this.textBoxOperatorName.Name = "textBoxOperatorName";
            this.textBoxOperatorName.Size = new System.Drawing.Size(179, 23);
            this.textBoxOperatorName.TabIndex = 20;
            this.textBoxOperatorName.Text = "1";
            // 
            // textBoxFactoryNumber
            // 
            this.textBoxFactoryNumber.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.textBoxFactoryNumber.Location = new System.Drawing.Point(190, 162);
            this.textBoxFactoryNumber.Name = "textBoxFactoryNumber";
            this.textBoxFactoryNumber.Size = new System.Drawing.Size(179, 23);
            this.textBoxFactoryNumber.TabIndex = 19;
            this.textBoxFactoryNumber.Text = "1";
            this.textBoxFactoryNumber.TextChanged += new System.EventHandler(this.textBoxFactoryNumber_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.label4.Location = new System.Drawing.Point(19, 211);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(74, 17);
            this.label4.TabIndex = 18;
            this.label4.Text = "Оператор";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.label3.Location = new System.Drawing.Point(19, 165);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(123, 17);
            this.label3.TabIndex = 17;
            this.label3.Text = "Заводской номер";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.label2.Location = new System.Drawing.Point(19, 118);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 17);
            this.label2.TabIndex = 16;
            this.label2.Text = "Коментарий";
            // 
            // buttonApplySettings
            // 
            this.buttonApplySettings.Location = new System.Drawing.Point(144, 245);
            this.buttonApplySettings.Name = "buttonApplySettings";
            this.buttonApplySettings.Size = new System.Drawing.Size(107, 28);
            this.buttonApplySettings.TabIndex = 15;
            this.buttonApplySettings.Text = "Запуск проверки";
            this.buttonApplySettings.UseVisualStyleBackColor = true;
            this.buttonApplySettings.Click += new System.EventHandler(this.buttonApplySettings_Click);
            // 
            // textBoxControlObjectName
            // 
            this.textBoxControlObjectName.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBoxControlObjectName.Location = new System.Drawing.Point(190, 25);
            this.textBoxControlObjectName.Name = "textBoxControlObjectName";
            this.textBoxControlObjectName.Size = new System.Drawing.Size(179, 23);
            this.textBoxControlObjectName.TabIndex = 14;
            this.textBoxControlObjectName.Text = "1";
            // 
            // labelErrorFactoryNumber
            // 
            this.labelErrorFactoryNumber.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelErrorFactoryNumber.AutoSize = true;
            this.labelErrorFactoryNumber.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelErrorFactoryNumber.Location = new System.Drawing.Point(187, 188);
            this.labelErrorFactoryNumber.Name = "labelErrorFactoryNumber";
            this.labelErrorFactoryNumber.Size = new System.Drawing.Size(0, 17);
            this.labelErrorFactoryNumber.TabIndex = 25;
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(416, 302);
            this.Controls.Add(this.labelErrorFactoryNumber);
            this.Controls.Add(this.comboBoxRegime);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.textBoxComment);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBoxOperatorName);
            this.Controls.Add(this.textBoxFactoryNumber);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.buttonApplySettings);
            this.Controls.Add(this.textBoxControlObjectName);
            this.Name = "SettingsForm";
            this.Text = "Settings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxRegime;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBoxComment;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxOperatorName;
        private System.Windows.Forms.TextBox textBoxFactoryNumber;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonApplySettings;
        private System.Windows.Forms.TextBox textBoxControlObjectName;
        private System.Windows.Forms.Label labelErrorFactoryNumber;
    }
}