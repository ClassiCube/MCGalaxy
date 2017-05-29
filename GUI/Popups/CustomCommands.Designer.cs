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
            this.cus_lblLoaded = new System.Windows.Forms.Label();
            this.cus_lstLoaded = new System.Windows.Forms.ListBox();
            this.cus_grpCreate = new System.Windows.Forms.GroupBox();
            this.cus_radPanel = new System.Windows.Forms.Panel();
            this.cus_radVB = new System.Windows.Forms.RadioButton();
            this.cus_radCS = new System.Windows.Forms.RadioButton();
            this.cus_btnCreate = new System.Windows.Forms.Button();
            this.cus_txtCmdName = new System.Windows.Forms.TextBox();
            this.cus_lblTxtName = new System.Windows.Forms.Label();
            this.cus_btnLoad = new System.Windows.Forms.Button();
            this.cus_btnUnload = new System.Windows.Forms.Button();
            this.cus_grpCreate.SuspendLayout();
            this.cus_radPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // cus_lblLoaded
            // 
            this.cus_lblLoaded.AutoSize = true;
            this.cus_lblLoaded.Location = new System.Drawing.Point(13, 129);
            this.cus_lblLoaded.Name = "cus_lblLoaded";
            this.cus_lblLoaded.Size = new System.Drawing.Size(134, 13);
            this.cus_lblLoaded.TabIndex = 45;
            this.cus_lblLoaded.Text = "Loaded custom commands";
            // 
            // cus_lstLoaded
            // 
            this.cus_lstLoaded.FormattingEnabled = true;
            this.cus_lstLoaded.Location = new System.Drawing.Point(13, 145);
            this.cus_lstLoaded.Name = "cus_lstLoaded";
            this.cus_lstLoaded.Size = new System.Drawing.Size(458, 303);
            this.cus_lstLoaded.TabIndex = 44;
            // 
            // cus_grpCreate
            // 
            this.cus_grpCreate.Controls.Add(this.cus_radPanel);
            this.cus_grpCreate.Controls.Add(this.cus_btnCreate);
            this.cus_grpCreate.Controls.Add(this.cus_txtCmdName);
            this.cus_grpCreate.Controls.Add(this.cus_lblTxtName);
            this.cus_grpCreate.Location = new System.Drawing.Point(12, 12);
            this.cus_grpCreate.Name = "cus_grpCreate";
            this.cus_grpCreate.Size = new System.Drawing.Size(459, 100);
            this.cus_grpCreate.TabIndex = 43;
            this.cus_grpCreate.TabStop = false;
            this.cus_grpCreate.Text = "Create command";
            // 
            // cus_radPanel
            // 
            this.cus_radPanel.Controls.Add(this.cus_radVB);
            this.cus_radPanel.Controls.Add(this.cus_radCS);
            this.cus_radPanel.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cus_radPanel.Location = new System.Drawing.Point(13, 58);
            this.cus_radPanel.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.cus_radPanel.Name = "cus_radPanel";
            this.cus_radPanel.Size = new System.Drawing.Size(84, 29);
            this.cus_radPanel.TabIndex = 37;
            // 
            // cus_radVB
            // 
            this.cus_radVB.AutoSize = true;
            this.cus_radVB.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cus_radVB.Location = new System.Drawing.Point(41, 6);
            this.cus_radVB.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.cus_radVB.Name = "cus_radVB";
            this.cus_radVB.Size = new System.Drawing.Size(36, 16);
            this.cus_radVB.TabIndex = 27;
            this.cus_radVB.Text = "VB";
            this.cus_radVB.UseVisualStyleBackColor = true;
            // 
            // cus_radCS
            // 
            this.cus_radCS.AutoSize = true;
            this.cus_radCS.Checked = true;
            this.cus_radCS.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cus_radCS.Location = new System.Drawing.Point(2, 6);
            this.cus_radCS.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.cus_radCS.Name = "cus_radCS";
            this.cus_radCS.Size = new System.Drawing.Size(35, 16);
            this.cus_radCS.TabIndex = 0;
            this.cus_radCS.TabStop = true;
            this.cus_radCS.Text = "C#";
            this.cus_radCS.UseVisualStyleBackColor = true;
            // 
            // cus_btnCreate
            // 
            this.cus_btnCreate.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cus_btnCreate.Location = new System.Drawing.Point(374, 71);
            this.cus_btnCreate.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.cus_btnCreate.Name = "cus_btnCreate";
            this.cus_btnCreate.Size = new System.Drawing.Size(80, 23);
            this.cus_btnCreate.TabIndex = 29;
            this.cus_btnCreate.Text = "Create command";
            this.cus_btnCreate.UseVisualStyleBackColor = true;
            // 
            // cus_txtCmdName
            // 
            this.cus_txtCmdName.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cus_txtCmdName.Location = new System.Drawing.Point(93, 20);
            this.cus_txtCmdName.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.cus_txtCmdName.Name = "cus_txtCmdName";
            this.cus_txtCmdName.Size = new System.Drawing.Size(355, 18);
            this.cus_txtCmdName.TabIndex = 27;
            // 
            // cus_lblTxtName
            // 
            this.cus_lblTxtName.AutoSize = true;
            this.cus_lblTxtName.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cus_lblTxtName.Location = new System.Drawing.Point(11, 23);
            this.cus_lblTxtName.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.cus_lblTxtName.Name = "cus_lblTxtName";
            this.cus_lblTxtName.Size = new System.Drawing.Size(78, 12);
            this.cus_lblTxtName.TabIndex = 28;
            this.cus_lblTxtName.Text = "Command Name:";
            // 
            // cus_btnLoad
            // 
            this.cus_btnLoad.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cus_btnLoad.Location = new System.Drawing.Point(13, 454);
            this.cus_btnLoad.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.cus_btnLoad.Name = "cus_btnLoad";
            this.cus_btnLoad.Size = new System.Drawing.Size(80, 23);
            this.cus_btnLoad.TabIndex = 41;
            this.cus_btnLoad.Text = "Load";
            this.cus_btnLoad.UseVisualStyleBackColor = true;
            // 
            // cus_btnUnload
            // 
            this.cus_btnUnload.Enabled = false;
            this.cus_btnUnload.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cus_btnUnload.Location = new System.Drawing.Point(391, 454);
            this.cus_btnUnload.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.cus_btnUnload.Name = "cus_btnUnload";
            this.cus_btnUnload.Size = new System.Drawing.Size(80, 23);
            this.cus_btnUnload.TabIndex = 42;
            this.cus_btnUnload.Text = "Unload";
            this.cus_btnUnload.UseVisualStyleBackColor = true;
            // 
            // CustomCommands
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 479);
            this.Controls.Add(this.cus_lblLoaded);
            this.Controls.Add(this.cus_lstLoaded);
            this.Controls.Add(this.cus_grpCreate);
            this.Controls.Add(this.cus_btnLoad);
            this.Controls.Add(this.cus_btnUnload);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "CustomCommands";
            this.Text = "Custom commands";
            this.cus_grpCreate.ResumeLayout(false);
            this.cus_grpCreate.PerformLayout();
            this.cus_radPanel.ResumeLayout(false);
            this.cus_radPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
        private System.Windows.Forms.Button cus_btnUnload;
        private System.Windows.Forms.Button cus_btnLoad;
        private System.Windows.Forms.Label cus_lblTxtName;
        private System.Windows.Forms.TextBox cus_txtCmdName;
        private System.Windows.Forms.Button cus_btnCreate;
        private System.Windows.Forms.RadioButton cus_radCS;
        private System.Windows.Forms.RadioButton cus_radVB;
        private System.Windows.Forms.Panel cus_radPanel;
        private System.Windows.Forms.GroupBox cus_grpCreate;
        private System.Windows.Forms.ListBox cus_lstLoaded;
        private System.Windows.Forms.Label cus_lblLoaded;
    }
}
