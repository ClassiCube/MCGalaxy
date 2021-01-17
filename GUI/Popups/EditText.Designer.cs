namespace MCGalaxy.Gui.Popups
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
        	this.btnColor = new System.Windows.Forms.Button();
        	this.btnToken = new System.Windows.Forms.Button();
        	this.btnSave = new System.Windows.Forms.Button();
        	this.SuspendLayout();
        	// 
        	// cmbList
        	// 
        	this.cmbList.BackColor = System.Drawing.SystemColors.Window;
        	this.cmbList.FormattingEnabled = true;
        	this.cmbList.Location = new System.Drawing.Point(392, 10);
        	this.cmbList.Name = "cmbList";
        	this.cmbList.Size = new System.Drawing.Size(108, 21);
        	this.cmbList.TabIndex = 0;
        	this.cmbList.SelectedIndexChanged += new System.EventHandler(this.cmbList_SelectedIndexChanged);
        	// 
        	// txtEdit
        	// 
        	this.txtEdit.BackColor = System.Drawing.SystemColors.Window;
        	this.txtEdit.Location = new System.Drawing.Point(7, 39);
        	this.txtEdit.Multiline = true;
        	this.txtEdit.Name = "txtEdit";
        	this.txtEdit.Size = new System.Drawing.Size(493, 282);
        	this.txtEdit.TabIndex = 2;
        	this.txtEdit.WordWrap = false;
        	// 
        	// btnColor
        	// 
        	this.btnColor.Location = new System.Drawing.Point(7, 9);
        	this.btnColor.Name = "btnColor";
        	this.btnColor.Size = new System.Drawing.Size(75, 23);
        	this.btnColor.TabIndex = 3;
        	this.btnColor.Text = "Insert color";
        	this.btnColor.UseVisualStyleBackColor = true;
        	this.btnColor.Click += new System.EventHandler(this.btnColor_Click);
        	// 
        	// btnToken
        	// 
        	this.btnToken.Location = new System.Drawing.Point(90, 9);
        	this.btnToken.Name = "btnToken";
        	this.btnToken.Size = new System.Drawing.Size(75, 23);
        	this.btnToken.TabIndex = 4;
        	this.btnToken.Text = "Insert token";
        	this.btnToken.UseVisualStyleBackColor = true;
        	this.btnToken.Click += new System.EventHandler(this.btnToken_Click);
        	// 
        	// btnSave
        	// 
        	this.btnSave.Location = new System.Drawing.Point(321, 9);
        	this.btnSave.Name = "btnSave";
        	this.btnSave.Size = new System.Drawing.Size(65, 23);
        	this.btnSave.TabIndex = 5;
        	this.btnSave.Text = "Save";
        	this.btnSave.UseVisualStyleBackColor = true;
        	this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
        	// 
        	// EditText
        	// 
        	this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        	this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        	this.ClientSize = new System.Drawing.Size(507, 328);
        	this.Controls.Add(this.btnSave);
        	this.Controls.Add(this.btnToken);
        	this.Controls.Add(this.btnColor);
        	this.Controls.Add(this.txtEdit);
        	this.Controls.Add(this.cmbList);
        	this.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
        	this.Name = "EditText";
        	this.ShowIcon = false;
        	this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
        	this.Text = "Editing (none)";
        	this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.EditTxt_Unload);
        	this.ResumeLayout(false);
        	this.PerformLayout();
        }
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnToken;
        private System.Windows.Forms.Button btnColor;

        #endregion

        private System.Windows.Forms.ComboBox cmbList;
        private System.Windows.Forms.TextBox txtEdit;
    }
}