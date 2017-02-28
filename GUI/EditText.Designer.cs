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
            this.EdittxtCombo = new System.Windows.Forms.ComboBox();
            this.LoadTxt = new System.Windows.Forms.Button();
            this.EditTextTxtBox = new System.Windows.Forms.TextBox();
            this.SaveEditTxtBt = new System.Windows.Forms.Button();
            this.DiscardEdittxtBt = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // EdittxtCombo
            // 
            this.EdittxtCombo.FormattingEnabled = true;
            this.EdittxtCombo.Items.AddRange(new object[] {
            "Autoload",
            "AwardsList",
            "Badwords",
            "CmdAutoload",
            "Custom$s",
            "Emotelist",
            "Joker",
            "Messages",
            "PlayerAwards",
            "Rules",
            "Welcome"});
            this.EdittxtCombo.Location = new System.Drawing.Point(94, 13);
            this.EdittxtCombo.Name = "EdittxtCombo";
            this.EdittxtCombo.Size = new System.Drawing.Size(178, 21);
            this.EdittxtCombo.TabIndex = 0;
            // 
            // LoadTxt
            // 
            this.LoadTxt.Location = new System.Drawing.Point(13, 13);
            this.LoadTxt.Name = "LoadTxt";
            this.LoadTxt.Size = new System.Drawing.Size(75, 23);
            this.LoadTxt.TabIndex = 1;
            this.LoadTxt.Text = "Load:";
            this.LoadTxt.UseVisualStyleBackColor = true;
            this.LoadTxt.Click += new System.EventHandler(this.LoadTxt_Click);
            // 
            // EditTextTxtBox
            // 
            this.EditTextTxtBox.Location = new System.Drawing.Point(13, 43);
            this.EditTextTxtBox.Multiline = true;
            this.EditTextTxtBox.Name = "EditTextTxtBox";
            this.EditTextTxtBox.Size = new System.Drawing.Size(259, 244);
            this.EditTextTxtBox.TabIndex = 2;
            // 
            // SaveEditTxtBt
            // 
            this.SaveEditTxtBt.Location = new System.Drawing.Point(13, 293);
            this.SaveEditTxtBt.Name = "SaveEditTxtBt";
            this.SaveEditTxtBt.Size = new System.Drawing.Size(126, 23);
            this.SaveEditTxtBt.TabIndex = 3;
            this.SaveEditTxtBt.Text = "Save";
            this.SaveEditTxtBt.UseVisualStyleBackColor = true;
            this.SaveEditTxtBt.Click += new System.EventHandler(this.SaveEditTxtBt_Click);
            // 
            // DiscardEdittxtBt
            // 
            this.DiscardEdittxtBt.Location = new System.Drawing.Point(145, 293);
            this.DiscardEdittxtBt.Name = "DiscardEdittxtBt";
            this.DiscardEdittxtBt.Size = new System.Drawing.Size(127, 23);
            this.DiscardEdittxtBt.TabIndex = 4;
            this.DiscardEdittxtBt.Text = "Discard";
            this.DiscardEdittxtBt.UseVisualStyleBackColor = true;
            this.DiscardEdittxtBt.Click += new System.EventHandler(this.DiscardEdittxtBt_Click);
            // 
            // EditText
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 328);
            this.Controls.Add(this.DiscardEdittxtBt);
            this.Controls.Add(this.SaveEditTxtBt);
            this.Controls.Add(this.EditTextTxtBox);
            this.Controls.Add(this.LoadTxt);
            this.Controls.Add(this.EdittxtCombo);
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

        private System.Windows.Forms.ComboBox EdittxtCombo;
        private System.Windows.Forms.Button LoadTxt;
        private System.Windows.Forms.TextBox EditTextTxtBox;
        private System.Windows.Forms.Button SaveEditTxtBt;
        private System.Windows.Forms.Button DiscardEdittxtBt;
    }
}