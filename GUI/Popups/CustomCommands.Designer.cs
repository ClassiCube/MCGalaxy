namespace MCGalaxy.Gui.Popups {
    partial class CustomCommands {
        /// <summary>
        /// Designer variable used to keep track of non-visual components.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        
        /// <summary>
        /// Disposes resources used by the form.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
        
        /// <summary>
        /// This method is required for Windows Forms designer support.
        /// Do not change the method contents inside the source code editor. The Forms designer might
        /// not be able to load this method if it was changed manually.
        /// </summary>
        private void InitializeComponent() {
        	this.lblCommands = new System.Windows.Forms.Label();
        	this.lstCommands = new System.Windows.Forms.ListBox();
        	this.grpCreate = new System.Windows.Forms.GroupBox();
        	this.radPanel = new System.Windows.Forms.Panel();
        	this.radVB = new System.Windows.Forms.RadioButton();
        	this.radCS = new System.Windows.Forms.RadioButton();
        	this.btnCreate = new System.Windows.Forms.Button();
        	this.txtCmdName = new System.Windows.Forms.TextBox();
        	this.lblTxtName = new System.Windows.Forms.Label();
        	this.btnLoad = new System.Windows.Forms.Button();
        	this.btnUnload = new System.Windows.Forms.Button();
        	this.grpCreate.SuspendLayout();
        	this.radPanel.SuspendLayout();
        	this.SuspendLayout();
        	// 
        	// lblCommands
        	// 
        	this.lblCommands.AutoSize = true;
        	this.lblCommands.Location = new System.Drawing.Point(13, 111);
        	this.lblCommands.Name = "lblCommands";
        	this.lblCommands.Size = new System.Drawing.Size(133, 13);
        	this.lblCommands.TabIndex = 45;
        	this.lblCommands.Text = "Loaded custom commands";
        	// 
        	// lstCommands
        	// 
        	this.lstCommands.BackColor = System.Drawing.SystemColors.Window;
        	this.lstCommands.FormattingEnabled = true;
        	this.lstCommands.Location = new System.Drawing.Point(12, 127);
        	this.lstCommands.MultiColumn = true;
        	this.lstCommands.Name = "lstCommands";
        	this.lstCommands.Size = new System.Drawing.Size(459, 134);
        	this.lstCommands.TabIndex = 44;
        	this.lstCommands.SelectedIndexChanged += new System.EventHandler(this.lstCommands_SelectedIndexChanged);
        	// 
        	// grpCreate
        	// 
        	this.grpCreate.Controls.Add(this.radPanel);
        	this.grpCreate.Controls.Add(this.btnCreate);
        	this.grpCreate.Controls.Add(this.txtCmdName);
        	this.grpCreate.Controls.Add(this.lblTxtName);
        	this.grpCreate.Location = new System.Drawing.Point(12, 12);
        	this.grpCreate.Name = "grpCreate";
        	this.grpCreate.Size = new System.Drawing.Size(459, 84);
        	this.grpCreate.TabIndex = 43;
        	this.grpCreate.TabStop = false;
        	this.grpCreate.Text = "Create command";
        	// 
        	// radPanel
        	// 
        	this.radPanel.Controls.Add(this.radVB);
        	this.radPanel.Controls.Add(this.radCS);
        	this.radPanel.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.radPanel.Location = new System.Drawing.Point(11, 44);
        	this.radPanel.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
        	this.radPanel.Name = "radPanel";
        	this.radPanel.Size = new System.Drawing.Size(84, 29);
        	this.radPanel.TabIndex = 37;
        	// 
        	// radVB
        	// 
        	this.radVB.AutoSize = true;
        	this.radVB.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.radVB.Location = new System.Drawing.Point(41, 6);
        	this.radVB.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
        	this.radVB.Name = "radVB";
        	this.radVB.Size = new System.Drawing.Size(36, 16);
        	this.radVB.TabIndex = 27;
        	this.radVB.Text = "VB";
        	this.radVB.UseVisualStyleBackColor = true;
        	// 
        	// radCS
        	// 
        	this.radCS.AutoSize = true;
        	this.radCS.Checked = true;
        	this.radCS.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.radCS.Location = new System.Drawing.Point(2, 6);
        	this.radCS.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
        	this.radCS.Name = "radCS";
        	this.radCS.Size = new System.Drawing.Size(35, 16);
        	this.radCS.TabIndex = 0;
        	this.radCS.TabStop = true;
        	this.radCS.Text = "C#";
        	this.radCS.UseVisualStyleBackColor = true;
        	// 
        	// btnCreate
        	// 
        	this.btnCreate.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.btnCreate.Location = new System.Drawing.Point(374, 48);
        	this.btnCreate.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
        	this.btnCreate.Name = "btnCreate";
        	this.btnCreate.Size = new System.Drawing.Size(80, 23);
        	this.btnCreate.TabIndex = 29;
        	this.btnCreate.Text = "Create command";
        	this.btnCreate.UseVisualStyleBackColor = true;
        	this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click);
        	// 
        	// txtCmdName
        	// 
        	this.txtCmdName.BackColor = System.Drawing.SystemColors.Window;
        	this.txtCmdName.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.txtCmdName.Location = new System.Drawing.Point(93, 20);
        	this.txtCmdName.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
        	this.txtCmdName.Name = "txtCmdName";
        	this.txtCmdName.Size = new System.Drawing.Size(355, 18);
        	this.txtCmdName.TabIndex = 27;
        	// 
        	// lblTxtName
        	// 
        	this.lblTxtName.AutoSize = true;
        	this.lblTxtName.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.lblTxtName.Location = new System.Drawing.Point(11, 23);
        	this.lblTxtName.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
        	this.lblTxtName.Name = "lblTxtName";
        	this.lblTxtName.Size = new System.Drawing.Size(78, 12);
        	this.lblTxtName.TabIndex = 28;
        	this.lblTxtName.Text = "Command Name:";
        	// 
        	// btnLoad
        	// 
        	this.btnLoad.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.btnLoad.Location = new System.Drawing.Point(13, 267);
        	this.btnLoad.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
        	this.btnLoad.Name = "btnLoad";
        	this.btnLoad.Size = new System.Drawing.Size(80, 23);
        	this.btnLoad.TabIndex = 41;
        	this.btnLoad.Text = "Load";
        	this.btnLoad.UseVisualStyleBackColor = true;
        	this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
        	// 
        	// btnUnload
        	// 
        	this.btnUnload.Enabled = false;
        	this.btnUnload.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.btnUnload.Location = new System.Drawing.Point(391, 267);
        	this.btnUnload.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
        	this.btnUnload.Name = "btnUnload";
        	this.btnUnload.Size = new System.Drawing.Size(80, 23);
        	this.btnUnload.TabIndex = 42;
        	this.btnUnload.Text = "Unload";
        	this.btnUnload.UseVisualStyleBackColor = true;
        	this.btnUnload.Click += new System.EventHandler(this.btnUnload_Click);
        	// 
        	// CustomCommands
        	// 
        	this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        	this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        	this.ClientSize = new System.Drawing.Size(484, 301);
        	this.Controls.Add(this.lblCommands);
        	this.Controls.Add(this.lstCommands);
        	this.Controls.Add(this.grpCreate);
        	this.Controls.Add(this.btnLoad);
        	this.Controls.Add(this.btnUnload);
        	this.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
        	this.Name = "CustomCommands";
        	this.Text = "Custom commands";
        	this.grpCreate.ResumeLayout(false);
        	this.grpCreate.PerformLayout();
        	this.radPanel.ResumeLayout(false);
        	this.radPanel.PerformLayout();
        	this.ResumeLayout(false);
        	this.PerformLayout();
        }
        private System.Windows.Forms.Button btnUnload;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Label lblTxtName;
        private System.Windows.Forms.TextBox txtCmdName;
        private System.Windows.Forms.Button btnCreate;
        private System.Windows.Forms.RadioButton radCS;
        private System.Windows.Forms.RadioButton radVB;
        private System.Windows.Forms.Panel radPanel;
        private System.Windows.Forms.GroupBox grpCreate;
        private System.Windows.Forms.ListBox lstCommands;
        private System.Windows.Forms.Label lblCommands;
    }
}
