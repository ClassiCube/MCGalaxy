namespace MCGalaxy.Gui
{
    partial class UpdateWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpdateWindow));
            this.panel1 = new System.Windows.Forms.Panel();
            this.cmdUpdate = new System.Windows.Forms.Button();
            this.listRevisions = new System.Windows.Forms.ListBox();
            this.chkAutoUpdate = new System.Windows.Forms.CheckBox();
            this.cmdDiscard = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.revisions_downloading = new System.Windows.Forms.Label();
            this.txtCountdown = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.chkNotify = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.cmdUpdate);
            this.panel1.Controls.Add(this.listRevisions);
            this.panel1.Location = new System.Drawing.Point(8, 7);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(148, 173);
            this.panel1.TabIndex = 0;
            // 
            // cmdUpdate
            // 
            this.cmdUpdate.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdUpdate.Location = new System.Drawing.Point(4, 70);
            this.cmdUpdate.Name = "cmdUpdate";
            this.cmdUpdate.Size = new System.Drawing.Size(82, 23);
            this.cmdUpdate.TabIndex = 2;
            this.cmdUpdate.Text = "Update";
            this.cmdUpdate.UseVisualStyleBackColor = true;
            this.cmdUpdate.Click += new System.EventHandler(this.cmdUpdate_Click);
            // 
            // listRevisions
            // 
            this.listRevisions.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listRevisions.FormattingEnabled = true;
            this.listRevisions.Location = new System.Drawing.Point(89, 10);
            this.listRevisions.Name = "listRevisions";
            this.listRevisions.Size = new System.Drawing.Size(53, 147);
            this.listRevisions.TabIndex = 0;
            this.listRevisions.SelectedValueChanged += new System.EventHandler(this.listRevisions_SelectedValueChanged);
            // 
            // chkAutoUpdate
            // 
            this.chkAutoUpdate.AutoSize = true;
            this.chkAutoUpdate.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkAutoUpdate.Location = new System.Drawing.Point(28, 4);
            this.chkAutoUpdate.Name = "chkAutoUpdate";
            this.chkAutoUpdate.Size = new System.Drawing.Size(133, 17);
            this.chkAutoUpdate.TabIndex = 1;
            this.chkAutoUpdate.Text = "Auto update to newest";
            this.chkAutoUpdate.UseVisualStyleBackColor = true;
            // 
            // cmdDiscard
            // 
            this.cmdDiscard.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdDiscard.Location = new System.Drawing.Point(111, 280);
            this.cmdDiscard.Name = "cmdDiscard";
            this.cmdDiscard.Size = new System.Drawing.Size(59, 23);
            this.cmdDiscard.TabIndex = 2;
            this.cmdDiscard.Text = "Discard";
            this.cmdDiscard.UseVisualStyleBackColor = true;
            this.cmdDiscard.Click += new System.EventHandler(this.cmdDiscard_Click);
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(35, 279);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(59, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "Save";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.revisions_downloading);
            this.panel2.Controls.Add(this.txtCountdown);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.chkNotify);
            this.panel2.Controls.Add(this.chkAutoUpdate);
            this.panel2.Location = new System.Drawing.Point(8, 189);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(209, 82);
            this.panel2.TabIndex = 4;
            // 
            // revisions_downloading
            // 
            this.revisions_downloading.AutoSize = true;
            this.revisions_downloading.BackColor = System.Drawing.Color.Transparent;
            this.revisions_downloading.ForeColor = System.Drawing.Color.Red;
            this.revisions_downloading.Location = new System.Drawing.Point(-3, 68);
            this.revisions_downloading.Name = "revisions_downloading";
            this.revisions_downloading.Size = new System.Drawing.Size(216, 13);
            this.revisions_downloading.TabIndex = 5;
            this.revisions_downloading.Text = "Please Wait while downloading revisions list.";
            // 
            // txtCountdown
            // 
            this.txtCountdown.Location = new System.Drawing.Point(161, 45);
            this.txtCountdown.Name = "txtCountdown";
            this.txtCountdown.Size = new System.Drawing.Size(42, 20);
            this.txtCountdown.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(2, 49);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(158, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Time (in seconds) to countdown:";
            // 
            // chkNotify
            // 
            this.chkNotify.AutoSize = true;
            this.chkNotify.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkNotify.Location = new System.Drawing.Point(28, 23);
            this.chkNotify.Name = "chkNotify";
            this.chkNotify.Size = new System.Drawing.Size(139, 17);
            this.chkNotify.TabIndex = 2;
            this.chkNotify.Text = "Notify in-game of restart";
            this.chkNotify.UseVisualStyleBackColor = true;
            // 
            // UpdateWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(223, 318);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.cmdDiscard);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "UpdateWindow";
            this.Text = "Update";
            this.Load += new System.EventHandler(this.UpdateWindow_Load);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button cmdUpdate;
        private System.Windows.Forms.ListBox listRevisions;
        private System.Windows.Forms.CheckBox chkAutoUpdate;
        private System.Windows.Forms.Button cmdDiscard;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.CheckBox chkNotify;
        private System.Windows.Forms.TextBox txtCountdown;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label revisions_downloading;
    }
}