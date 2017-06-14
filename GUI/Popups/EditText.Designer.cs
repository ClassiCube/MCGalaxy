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
            this.btnLoad = new System.Windows.Forms.Button();
            this.txtEdit = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnDiscard = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // EdittxtCombo
            // 
            this.cmbList.FormattingEnabled = true;
            this.cmbList.Location = new System.Drawing.Point(94, 13);
            this.cmbList.Name = "EdittxtCombo";
            this.cmbList.Size = new System.Drawing.Size(178, 21);
            this.cmbList.TabIndex = 0;
            this.cmbList.SelectedIndexChanged += new System.EventHandler(this.EditText_SelectedIndexChanged);
            // 
            // EditTextTxtBox
            // 
            this.txtEdit.Location = new System.Drawing.Point(13, 43);
            this.txtEdit.Multiline = true;
            this.txtEdit.Name = "EditTextTxtBox";
            this.txtEdit.Size = new System.Drawing.Size(259, 244);
            this.txtEdit.TabIndex = 2;
            // 
            // SaveEditTxtBt
            // 
            this.btnSave.Location = new System.Drawing.Point(13, 293);
            this.btnSave.Name = "SaveEditTxtBt";
            this.btnSave.Size = new System.Drawing.Size(126, 23);
            this.btnSave.TabIndex = 3;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.SaveEditTxtBt_Click);
            // 
            // DiscardEdittxtBt
            // 
            this.btnDiscard.Location = new System.Drawing.Point(145, 293);
            this.btnDiscard.Name = "DiscardEdittxtBt";
            this.btnDiscard.Size = new System.Drawing.Size(127, 23);
            this.btnDiscard.TabIndex = 4;
            this.btnDiscard.Text = "Discard";
            this.btnDiscard.UseVisualStyleBackColor = true;
            this.btnDiscard.Click += new System.EventHandler(this.DiscardEdittxtBt_Click);
            // 
            // EditText
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 328);
            this.Controls.Add(this.btnDiscard);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.txtEdit);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.cmbList);
            this.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "EditText";
            this.Text = "Edit Text";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.EditTxt_Unload);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbList;
        private System.Windows.Forms.TextBox txtEdit;
    }
}