namespace MCGalaxy.Gui
{
    partial class EditText
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
        	this.cmbList = new System.Windows.Forms.ComboBox();
        	this.txtEdit = new System.Windows.Forms.TextBox();
        	this.SuspendLayout();
        	// 
        	// cmbList
        	// 
        	this.cmbList.FormattingEnabled = true;
        	this.cmbList.Location = new System.Drawing.Point(392, 9);
        	this.cmbList.Name = "cmbList";
        	this.cmbList.Size = new System.Drawing.Size(108, 21);
        	this.cmbList.TabIndex = 0;
        	this.cmbList.SelectedIndexChanged += new System.EventHandler(this.cmbList_SelectedIndexChanged);
        	// 
        	// txtEdit
        	// 
        	this.txtEdit.Location = new System.Drawing.Point(7, 39);
        	this.txtEdit.Multiline = true;
        	this.txtEdit.WordWrap = false;
        	this.txtEdit.Name = "txtEdit";
        	this.txtEdit.Size = new System.Drawing.Size(493, 282);
        	this.txtEdit.TabIndex = 2;
        	// 
        	// EditText
        	// 
        	this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        	this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        	this.ClientSize = new System.Drawing.Size(507, 328);
        	this.Controls.Add(this.txtEdit);
        	this.Controls.Add(this.cmbList);
        	this.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
        	this.Name = "EditText";
        	this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
        	this.Text = "Editing (none)";
        	this.ShowIcon = false;
        	this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.EditTxt_Unload);
        	this.ResumeLayout(false);
        	this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ComboBox cmbList;
        private System.Windows.Forms.TextBox txtEdit;
    }
}