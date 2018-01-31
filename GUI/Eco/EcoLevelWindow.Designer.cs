namespace MCGalaxy.Gui.Eco {
    partial class EcoLevelWindow {
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
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.txtName = new System.Windows.Forms.TextBox();
            this.lblName = new System.Windows.Forms.Label();
            this.lblPrice = new System.Windows.Forms.Label();
            this.lblX = new System.Windows.Forms.Label();
            this.lblY = new System.Windows.Forms.Label();
            this.lblZ = new System.Windows.Forms.Label();
            this.lblType = new System.Windows.Forms.Label();
            this.numPrice = new System.Windows.Forms.NumericUpDown();
            this.cmbX = new System.Windows.Forms.ComboBox();
            this.cmbY = new System.Windows.Forms.ComboBox();
            this.cmbZ = new System.Windows.Forms.ComboBox();
            this.cmbType = new System.Windows.Forms.ComboBox();
            this.panel = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.numPrice)).BeginInit();
            this.panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.AutoSize = true;
            this.btnOk.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnOk.Enabled = false;
            this.btnOk.Location = new System.Drawing.Point(12, 252);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(36, 27);
            this.btnOk.TabIndex = 0;
            this.btnOk.Text = "Ok";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.AutoSize = true;
            this.btnCancel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(54, 252);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(61, 27);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(70, 9);
            this.txtName.Margin = new System.Windows.Forms.Padding(3, 3, 10, 3);
            this.txtName.MaxLength = 32;
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(112, 22);
            this.txtName.TabIndex = 2;
            this.txtName.Tag = "";
            this.txtName.TextChanged += new System.EventHandler(this.txtName_TextChanged);
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(12, 12);
            this.lblName.Margin = new System.Windows.Forms.Padding(10);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(45, 17);
            this.lblName.TabIndex = 3;
            this.lblName.Text = "Name";
            // 
            // lblPrice
            // 
            this.lblPrice.AutoSize = true;
            this.lblPrice.Location = new System.Drawing.Point(12, 49);
            this.lblPrice.Margin = new System.Windows.Forms.Padding(10);
            this.lblPrice.Name = "lblPrice";
            this.lblPrice.Size = new System.Drawing.Size(40, 17);
            this.lblPrice.TabIndex = 4;
            this.lblPrice.Text = "Price";
            // 
            // lblX
            // 
            this.lblX.AutoSize = true;
            this.lblX.Location = new System.Drawing.Point(12, 86);
            this.lblX.Margin = new System.Windows.Forms.Padding(10);
            this.lblX.Name = "lblX";
            this.lblX.Size = new System.Drawing.Size(17, 17);
            this.lblX.TabIndex = 5;
            this.lblX.Text = "X";
            // 
            // lblY
            // 
            this.lblY.AutoSize = true;
            this.lblY.Location = new System.Drawing.Point(12, 123);
            this.lblY.Margin = new System.Windows.Forms.Padding(10);
            this.lblY.Name = "lblY";
            this.lblY.Size = new System.Drawing.Size(17, 17);
            this.lblY.TabIndex = 6;
            this.lblY.Text = "Y";
            // 
            // lblZ
            // 
            this.lblZ.AutoSize = true;
            this.lblZ.Location = new System.Drawing.Point(12, 160);
            this.lblZ.Margin = new System.Windows.Forms.Padding(10);
            this.lblZ.Name = "lblZ";
            this.lblZ.Size = new System.Drawing.Size(17, 17);
            this.lblZ.TabIndex = 7;
            this.lblZ.Text = "Z";
            // 
            // lblType
            // 
            this.lblType.AutoSize = true;
            this.lblType.Location = new System.Drawing.Point(12, 197);
            this.lblType.Margin = new System.Windows.Forms.Padding(10);
            this.lblType.Name = "lblType";
            this.lblType.Size = new System.Drawing.Size(40, 17);
            this.lblType.TabIndex = 8;
            this.lblType.Text = "Type";
            // 
            // numPrice
            // 
            this.numPrice.AutoSize = true;
            this.numPrice.Location = new System.Drawing.Point(70, 47);
            this.numPrice.Maximum = new decimal(new int[] {
            16777215,
            0,
            0,
            0});
            this.numPrice.Name = "numPrice";
            this.numPrice.Size = new System.Drawing.Size(88, 22);
            this.numPrice.TabIndex = 9;
            this.numPrice.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // cmbX
            // 
            this.cmbX.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbX.FormattingEnabled = true;
            this.cmbX.Location = new System.Drawing.Point(70, 83);
            this.cmbX.MaxDropDownItems = 6;
            this.cmbX.Name = "cmbX";
            this.cmbX.Size = new System.Drawing.Size(64, 24);
            this.cmbX.TabIndex = 10;
            // 
            // cmbY
            // 
            this.cmbY.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbY.FormattingEnabled = true;
            this.cmbY.Location = new System.Drawing.Point(70, 120);
            this.cmbY.MaxDropDownItems = 6;
            this.cmbY.Name = "cmbY";
            this.cmbY.Size = new System.Drawing.Size(64, 24);
            this.cmbY.TabIndex = 11;
            // 
            // cmbZ
            // 
            this.cmbZ.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbZ.FormattingEnabled = true;
            this.cmbZ.Location = new System.Drawing.Point(70, 157);
            this.cmbZ.MaxDropDownItems = 6;
            this.cmbZ.Name = "cmbZ";
            this.cmbZ.Size = new System.Drawing.Size(64, 24);
            this.cmbZ.TabIndex = 12;
            // 
            // cmbType
            // 
            this.cmbType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbType.FormattingEnabled = true;
            this.cmbType.Location = new System.Drawing.Point(70, 194);
            this.cmbType.Margin = new System.Windows.Forms.Padding(3, 3, 10, 3);
            this.cmbType.MaxDropDownItems = 6;
            this.cmbType.Name = "cmbType";
            this.cmbType.Size = new System.Drawing.Size(112, 24);
            this.cmbType.TabIndex = 13;
            // 
            // panel
            // 
            this.panel.AutoSize = true;
            this.panel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel.Controls.Add(this.cmbType);
            this.panel.Controls.Add(this.cmbZ);
            this.panel.Controls.Add(this.cmbY);
            this.panel.Controls.Add(this.cmbX);
            this.panel.Controls.Add(this.numPrice);
            this.panel.Controls.Add(this.lblType);
            this.panel.Controls.Add(this.lblZ);
            this.panel.Controls.Add(this.lblY);
            this.panel.Controls.Add(this.lblX);
            this.panel.Controls.Add(this.lblPrice);
            this.panel.Controls.Add(this.lblName);
            this.panel.Controls.Add(this.txtName);
            this.panel.Location = new System.Drawing.Point(12, 12);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(192, 224);
            this.panel.TabIndex = 14;
            // 
            // EcoLevelWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(216, 287);
            this.Controls.Add(this.panel);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EcoLevelWindow";
            this.ShowIcon = false;
            this.Text = "Add Level Preset";
            this.Load += new System.EventHandler(this.EcoLevelWindow_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numPrice)).EndInit();
            this.panel.ResumeLayout(false);
            this.panel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label lblPrice;
        private System.Windows.Forms.Label lblX;
        private System.Windows.Forms.Label lblY;
        private System.Windows.Forms.Label lblZ;
        private System.Windows.Forms.Label lblType;
        private System.Windows.Forms.NumericUpDown numPrice;
        private System.Windows.Forms.ComboBox cmbX;
        private System.Windows.Forms.ComboBox cmbY;
        private System.Windows.Forms.ComboBox cmbZ;
        private System.Windows.Forms.ComboBox cmbType;
        private System.Windows.Forms.Panel panel;
    }
}