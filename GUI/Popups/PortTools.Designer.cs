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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnDelete = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.linkHelpForward = new System.Windows.Forms.LinkLabel();
            this.txtPortForward = new System.Windows.Forms.TextBox();
            this.lblForward = new System.Windows.Forms.Label();
            this.btnForward = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip( this.components );
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add( this.btnDelete );
            this.groupBox2.Controls.Add( this.label3 );
            this.groupBox2.Controls.Add( this.txtPortForward );
            this.groupBox2.Controls.Add( this.lblForward );
            this.groupBox2.Controls.Add( this.btnForward );
            this.groupBox2.Controls.Add( this.label2 );           
            this.groupBox2.Location = new System.Drawing.Point( 12, 42 );
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size( 274, 150 );
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Port Forward";
            // 
            // linkManually
            // 
            this.linkManually.AutoSize = true;
            this.linkManually.Location = new System.Drawing.Point( 12, 10 );
            this.linkManually.Name = "linkManually";
            this.linkManually.Size = new System.Drawing.Size( 83, 13 );
            this.linkManually.TabIndex = 4;
            this.linkManually.TabStop = true;
            this.linkManually.Text = "Check port open";
            this.linkManually.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler( this.linkManually_LinkClicked ); 
            // 
            // linkHelpForward
            // 
            this.linkHelpForward.AutoSize = true;
            this.linkHelpForward.Location = new System.Drawing.Point( 222, 10 );
            this.linkHelpForward.Name = "linkHelpForward";
            this.linkHelpForward.Size = new System.Drawing.Size( 64, 13 );
            this.linkHelpForward.TabIndex = 11;
            this.linkHelpForward.TabStop = true;
            this.linkHelpForward.Text = "Need Help?";
            this.linkHelpForward.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler( this.linkHelpForward_LinkClicked );            
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point( 156, 110 );
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size( 93, 23 );
            this.btnDelete.TabIndex = 12;
            this.btnDelete.Text = "Delete Forward";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler( this.btnDelete_Click );
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point( 22, 69 );
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size( 29, 13 );
            this.label3.TabIndex = 6;
            this.label3.Text = "Port:";
            // 
            // txtPortForward
            // 
            this.txtPortForward.Location = new System.Drawing.Point( 57, 66 );
            this.txtPortForward.MaxLength = 5;
            this.txtPortForward.Name = "txtPortForward";
            this.txtPortForward.Size = new System.Drawing.Size( 192, 20 );
            this.txtPortForward.TabIndex = 7;
            // 
            // lblForward
            // 
            this.lblForward.AutoSize = true;
            this.lblForward.ForeColor = System.Drawing.SystemColors.ButtonShadow;
            this.lblForward.Location = new System.Drawing.Point( 22, 91 );
            this.lblForward.Name = "lblForward";
            this.lblForward.Size = new System.Drawing.Size( 0, 13 );
            this.lblForward.TabIndex = 8;
            // 
            // btnForward
            // 
            this.btnForward.Location = new System.Drawing.Point( 30, 110 );
            this.btnForward.Name = "btnForward";
            this.btnForward.Size = new System.Drawing.Size( 75, 23 );
            this.btnForward.TabIndex = 9;
            this.btnForward.Text = "Forward";
            this.toolTip1.SetToolTip( this.btnForward, "This method does not work for everyone, keep trying or manually forward.\r\n" );
            this.btnForward.UseVisualStyleBackColor = true;
            this.btnForward.Click += new System.EventHandler( this.btnForward_Click );
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font( "Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ( (byte) ( 0 ) ) );
            this.label2.Location = new System.Drawing.Point( 29, 16 );
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size( 220, 36 );
            this.label2.TabIndex = 0;
            this.label2.Text = "Warning! This method uses UPnP \r\nwhich does have known security issues. \r\nUse " +
                "at your own risk.";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // PortTools
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size( 296, 204 );
            this.Controls.Add( this.groupBox2 );
            this.Controls.Add( this.linkManually );
            this.Controls.Add( this.linkHelpForward );
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "PortTools";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Port Tools";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler( this.PortChecker_FormClosing );
            this.groupBox2.ResumeLayout( false );
            this.groupBox2.PerformLayout();
            this.ResumeLayout( false );

        }

        #endregion

        private System.Windows.Forms.LinkLabel linkManually;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.LinkLabel linkHelpForward;
        private System.Windows.Forms.TextBox txtPortForward;
        private System.Windows.Forms.Label lblForward;
        private System.Windows.Forms.Button btnForward;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}