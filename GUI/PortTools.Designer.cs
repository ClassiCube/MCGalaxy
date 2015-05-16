namespace MCGalaxy.Gui.Popups {
    partial class PortTools {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnCheck = new System.Windows.Forms.Button();
            this.linkManually = new System.Windows.Forms.LinkLabel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnDelete = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txtPortForward = new System.Windows.Forms.TextBox();
            this.lblForward = new System.Windows.Forms.Label();
            this.btnForward = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Port:";
            // 
            // txtPort
            // 
            this.txtPort.Location = new System.Drawing.Point(57, 25);
            this.txtPort.MaxLength = 5;
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(192, 20);
            this.txtPort.TabIndex = 1;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.ForeColor = System.Drawing.SystemColors.ButtonShadow;
            this.lblStatus.Location = new System.Drawing.Point(22, 69);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(70, 13);
            this.lblStatus.TabIndex = 2;
            this.lblStatus.Text = "Not Checked";
            // 
            // btnCheck
            // 
            this.btnCheck.Location = new System.Drawing.Point(174, 69);
            this.btnCheck.Name = "btnCheck";
            this.btnCheck.Size = new System.Drawing.Size(75, 23);
            this.btnCheck.TabIndex = 3;
            this.btnCheck.Text = "Check Port";
            this.btnCheck.UseVisualStyleBackColor = true;
            this.btnCheck.Click += new System.EventHandler(this.btnCheck_Click);
            // 
            // linkManually
            // 
            this.linkManually.AutoSize = true;
            this.linkManually.Location = new System.Drawing.Point(166, 125);
            this.linkManually.Name = "linkManually";
            this.linkManually.Size = new System.Drawing.Size(83, 13);
            this.linkManually.TabIndex = 4;
            this.linkManually.TabStop = true;
            this.linkManually.Text = "Check Manually";
            this.linkManually.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkManually_LinkClicked);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.txtPort);
            this.groupBox1.Controls.Add(this.linkManually);
            this.groupBox1.Controls.Add(this.lblStatus);
            this.groupBox1.Controls.Add(this.btnCheck);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(274, 157);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Port Checker";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnDelete);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.txtPortForward);
            this.groupBox2.Controls.Add(this.lblForward);
            this.groupBox2.Controls.Add(this.btnForward);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Location = new System.Drawing.Point(12, 176);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(274, 170);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Port Forward";
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(156, 140);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(93, 23);
            this.btnDelete.TabIndex = 12;
            this.btnDelete.Text = "Delete Forward";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(22, 69);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Port:";
            // 
            // txtPortForward
            // 
            this.txtPortForward.Location = new System.Drawing.Point(57, 66);
            this.txtPortForward.MaxLength = 5;
            this.txtPortForward.Name = "txtPortForward";
            this.txtPortForward.Size = new System.Drawing.Size(192, 20);
            this.txtPortForward.TabIndex = 7;
            // 
            // lblForward
            // 
            this.lblForward.AutoSize = true;
            this.lblForward.ForeColor = System.Drawing.SystemColors.ButtonShadow;
            this.lblForward.Location = new System.Drawing.Point(22, 91);
            this.lblForward.Name = "lblForward";
            this.lblForward.Size = new System.Drawing.Size(0, 13);
            this.lblForward.TabIndex = 8;
            // 
            // btnForward
            // 
            this.btnForward.Location = new System.Drawing.Point(174, 110);
            this.btnForward.Name = "btnForward";
            this.btnForward.Size = new System.Drawing.Size(75, 23);
            this.btnForward.TabIndex = 9;
            this.btnForward.Text = "Forward";
            this.toolTip1.SetToolTip(this.btnForward, "This method does not work for everyone, keep trying or manually forward.\r\n");
            this.btnForward.UseVisualStyleBackColor = true;
            this.btnForward.Click += new System.EventHandler(this.btnForward_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(29, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(213, 36);
            this.label2.TabIndex = 0;
            this.label2.Text = "Warning! This method uses UPnP \r\nwhich does have known security issues. \r\nUse at " +
    "your own risk.";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // PortTools
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(296, 354);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "PortTools";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Port Tools";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PortChecker_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnCheck;
        private System.Windows.Forms.LinkLabel linkManually;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtPortForward;
        private System.Windows.Forms.Label lblForward;
        private System.Windows.Forms.Button btnForward;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}