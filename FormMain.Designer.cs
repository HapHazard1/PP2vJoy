namespace PP2vJoyFeeder
{
    partial class FormMain
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
          this.txtOutput = new System.Windows.Forms.TextBox();
          this.ckOutputOn = new System.Windows.Forms.CheckBox();
          this.cbJoyType = new System.Windows.Forms.ComboBox();
          this.label1 = new System.Windows.Forms.Label();
          this.cbVJoyDev = new System.Windows.Forms.ComboBox();
          this.label2 = new System.Windows.Forms.Label();
          this.SuspendLayout();
          // 
          // txtOutput
          // 
          this.txtOutput.Location = new System.Drawing.Point(2, 80);
          this.txtOutput.Multiline = true;
          this.txtOutput.Name = "txtOutput";
          this.txtOutput.ReadOnly = true;
          this.txtOutput.Size = new System.Drawing.Size(277, 90);
          this.txtOutput.TabIndex = 0;
          // 
          // ckOutputOn
          // 
          this.ckOutputOn.AutoSize = true;
          this.ckOutputOn.Location = new System.Drawing.Point(2, 57);
          this.ckOutputOn.Name = "ckOutputOn";
          this.ckOutputOn.Size = new System.Drawing.Size(146, 17);
          this.ckOutputOn.TabIndex = 1;
          this.ckOutputOn.Text = "Enable Output Monitoring";
          this.ckOutputOn.UseVisualStyleBackColor = true;
          // 
          // cbJoyType
          // 
          this.cbJoyType.FormattingEnabled = true;
          this.cbJoyType.Location = new System.Drawing.Point(2, 25);
          this.cbJoyType.Name = "cbJoyType";
          this.cbJoyType.Size = new System.Drawing.Size(166, 21);
          this.cbJoyType.TabIndex = 2;
          this.cbJoyType.SelectedIndexChanged += new System.EventHandler(this.cbJoyType_SelectedIndexChanged);
          // 
          // label1
          // 
          this.label1.AutoSize = true;
          this.label1.Location = new System.Drawing.Point(2, 5);
          this.label1.Name = "label1";
          this.label1.Size = new System.Drawing.Size(111, 13);
          this.label1.TabIndex = 3;
          this.label1.Text = "Choose Joystick Type";
          // 
          // cbVJoyDev
          // 
          this.cbVJoyDev.FormattingEnabled = true;
          this.cbVJoyDev.Location = new System.Drawing.Point(181, 25);
          this.cbVJoyDev.Name = "cbVJoyDev";
          this.cbVJoyDev.Size = new System.Drawing.Size(97, 21);
          this.cbVJoyDev.TabIndex = 4;
          this.cbVJoyDev.SelectedIndexChanged += new System.EventHandler(this.cbVJoyDev_SelectedIndexChanged);
          // 
          // label2
          // 
          this.label2.AutoSize = true;
          this.label2.Location = new System.Drawing.Point(173, 5);
          this.label2.Name = "label2";
          this.label2.Size = new System.Drawing.Size(105, 13);
          this.label2.TabIndex = 5;
          this.label2.Text = "Choose vJoy Device";
          // 
          // FormMain
          // 
          this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
          this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
          this.ClientSize = new System.Drawing.Size(282, 170);
          this.Controls.Add(this.label2);
          this.Controls.Add(this.cbVJoyDev);
          this.Controls.Add(this.label1);
          this.Controls.Add(this.cbJoyType);
          this.Controls.Add(this.ckOutputOn);
          this.Controls.Add(this.txtOutput);
          this.Name = "FormMain";
          this.Text = "PP2vJoyFeeder";
          this.ResumeLayout(false);
          this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.TextBox txtOutput;
        public System.Windows.Forms.CheckBox ckOutputOn;
        public System.Windows.Forms.ComboBox cbJoyType;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.ComboBox cbVJoyDev;
        private System.Windows.Forms.Label label2;


    }
}