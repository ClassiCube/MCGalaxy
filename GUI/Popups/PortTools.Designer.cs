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
            this.linkManually = new System.Windows.Forms.LinkLabel();
            this.gbUpnp = new System.Windows.Forms.GroupBox();
            this.btnForward = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.lblResult = new System.Windows.Forms.Label();
            this.lblInfo = new System.Windows.Forms.Label();
            this.linkHelpForward = new System.Windows.Forms.LinkLabel();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.gbLogs = new System.Windows.Forms.GroupBox();
            this.txtLogs = new System.Windows.Forms.TextBox();
            this.gbUpnp.SuspendLayout();
            this.gbLogs.SuspendLayout();
            this.SuspendLayout();
            // 
            // linkManually
            // 
            this.linkManually.AutoSize = true;
            this.linkManually.Location = new System.Drawing.Point(12, 10);
            this.linkManually.Name = "linkManually";
            this.linkManually.Size = new System.Drawing.Size(86, 13);
            this.linkManually.TabIndex = 1;
            this.linkManually.TabStop = true;
            this.linkManually.Text = "Check port open";
            this.linkManually.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkManually_LinkClicked);
            // 
            // gbUpnp
            // 
            this.gbUpnp.Controls.Add(this.btnForward);
            this.gbUpnp.Controls.Add(this.btnDelete);
            this.gbUpnp.Controls.Add(this.lblResult);
            this.gbUpnp.Controls.Add(this.lblInfo);
            this.gbUpnp.Location = new System.Drawing.Point(11, 42);
            this.gbUpnp.Name = "gbUpnp";
            this.gbUpnp.Size = new System.Drawing.Size(274, 107);
            this.gbUpnp.TabIndex = 7;
            this.gbUpnp.TabStop = false;
            this.gbUpnp.Text = "Auto port forward";
            // 
            // btnForward
            // 
            this.btnForward.Location = new System.Drawing.Point(22, 66);
            this.btnForward.Name = "btnForward";
            this.btnForward.Size = new System.Drawing.Size(86, 23);
            this.btnForward.TabIndex = 3;
            this.btnForward.Text = "Forward 25565";
            this.toolTip1.SetToolTip(this.btnForward, "This does not work for everyone, keep trying or manually port forward.\r\n");
            this.btnForward.UseVisualStyleBackColor = true;
            this.btnForward.Click += new System.EventHandler(this.btnForward_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(156, 66);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(93, 23);
            this.btnDelete.TabIndex = 4;
            this.btnDelete.Text = "Delete forward";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // lblResult
            // 
            this.lblResult.AutoSize = true;
            this.lblResult.ForeColor = System.Drawing.SystemColors.ButtonShadow;
            this.lblResult.Location = new System.Drawing.Point(22, 91);
            this.lblResult.Name = "lblResult";
            this.lblResult.Size = new System.Drawing.Size(0, 13);
            this.lblResult.TabIndex = 11;
            // 
            // lblInfo
            // 
            this.lblInfo.AutoSize = true;
            this.lblInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblInfo.Location = new System.Drawing.Point(5, 17);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(266, 39);
            this.lblInfo.TabIndex = 12;
            this.lblInfo.Text = "This uses UPnP, which not all routers support.\r\nIf this doesn\'t work, you will ha" +
            "ve to\r\n manually port forward in your router.";
            this.lblInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // linkHelpForward
            // 
            this.linkHelpForward.AutoSize = true;
            this.linkHelpForward.Location = new System.Drawing.Point(172, 10);
            this.linkHelpForward.Name = "linkHelpForward";
            this.linkHelpForward.Size = new System.Drawing.Size(114, 13);
            this.linkHelpForward.TabIndex = 2;
            this.linkHelpForward.TabStop = true;
            this.linkHelpForward.Text = "Need help forwarding?";
            this.linkHelpForward.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkHelpForward_LinkClicked);
            // 
            // gbLogs
            // 
            this.gbLogs.Controls.Add(this.txtLogs);
            this.gbLogs.Location = new System.Drawing.Point(11, 153);
            this.gbLogs.Name = "gbLogs";
            this.gbLogs.Size = new System.Drawing.Size(274, 165);
            this.gbLogs.TabIndex = 13;
            this.gbLogs.TabStop = false;
            this.gbLogs.Text = "UPnP logs";
            this.gbLogs.Visible = false;
            // 
            // txtLogs
            // 
            this.txtLogs.BackColor = System.Drawing.SystemColors.Window;
            this.txtLogs.Location = new System.Drawing.Point(7, 20);
            this.txtLogs.Multiline = true;
            this.txtLogs.Name = "txtLogs";
            this.txtLogs.ReadOnly = true;
            this.txtLogs.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtLogs.Size = new System.Drawing.Size(261, 139);
            this.txtLogs.TabIndex = 14;
            // 
            // PortTools
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(296, 160);
            this.Controls.Add(this.gbLogs);
            this.Controls.Add(this.gbUpnp);
            this.Controls.Add(this.linkManually);
            this.Controls.Add(this.linkHelpForward);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PortTools";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Port forward tools";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PortChecker_FormClosing);
            this.Load += new System.EventHandler(this.PortTools_Load);
            this.gbUpnp.ResumeLayout(false);
            this.gbUpnp.PerformLayout();
            this.gbLogs.ResumeLayout(false);
            this.gbLogs.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
        private System.Windows.Forms.TextBox txtLogs;
        private System.Windows.Forms.GroupBox gbLogs;

        #endregion

        private System.Windows.Forms.LinkLabel linkManually;
        private System.Windows.Forms.GroupBox gbUpnp;
        private System.Windows.Forms.LinkLabel linkHelpForward;
        private System.Windows.Forms.Label lblResult;
        private System.Windows.Forms.Button btnForward;
        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}