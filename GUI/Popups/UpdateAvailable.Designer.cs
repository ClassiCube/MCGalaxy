
namespace MCGalaxy.Gui.Popups 
{
	partial class UpdateAvailable
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.btnUpdate = new System.Windows.Forms.Button();
			this.btnNo = new System.Windows.Forms.Button();
			this.lblText = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// btnUpdate
			// 
			this.btnUpdate.Location = new System.Drawing.Point(130, 96);
			this.btnUpdate.Name = "btnUpdate";
			this.btnUpdate.Size = new System.Drawing.Size(88, 26);
			this.btnUpdate.TabIndex = 0;
			this.btnUpdate.Text = "Update";
			this.btnUpdate.UseVisualStyleBackColor = true;
			this.btnUpdate.Click += new System.EventHandler(this.BtnUpdate_Click);
			// 
			// btnNo
			// 
			this.btnNo.Location = new System.Drawing.Point(226, 96);
			this.btnNo.Name = "btnNo";
			this.btnNo.Size = new System.Drawing.Size(88, 26);
			this.btnNo.TabIndex = 1;
			this.btnNo.Text = "No";
			this.btnNo.UseVisualStyleBackColor = true;
			this.btnNo.Click += new System.EventHandler(this.BtnNo_Click);
			// 
			// lblText
			// 
			this.lblText.AutoSize = true;
			this.lblText.Location = new System.Drawing.Point(63, 35);
			this.lblText.Name = "lblText";
			this.lblText.Size = new System.Drawing.Size(226, 13);
			this.lblText.TabIndex = 2;
			this.lblText.Text = "New version found. Would you like to update?";
			// 
			// UpdateCheck
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(323, 133);
			this.Controls.Add(this.lblText);
			this.Controls.Add(this.btnNo);
			this.Controls.Add(this.btnUpdate);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "UpdateAvailable";
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Update?";
			this.Closed += new System.EventHandler(this.UpdateCheck_Closed);
			this.Load += new System.EventHandler(this.UpdateCheck_Load);
			this.ResumeLayout(false);
			this.PerformLayout();
		}

		private System.Windows.Forms.Label lblText;
		private System.Windows.Forms.Button btnNo;
		private System.Windows.Forms.Button btnUpdate;
	}
}
