namespace MCGalaxy.GUI.Eco {
    partial class EconomyWindow {
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
            this.groupBoxEco = new System.Windows.Forms.GroupBox();
            this.groupBoxLevel = new System.Windows.Forms.GroupBox();
            this.buttonEdit = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.ColumnName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnX = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnY = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnZ = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.buttonRemove = new System.Windows.Forms.Button();
            this.buttonAdd = new System.Windows.Forms.Button();
            this.checkBoxLevel = new System.Windows.Forms.CheckBox();
            this.groupBoxRank = new System.Windows.Forms.GroupBox();
            this.numericUpDownRank = new System.Windows.Forms.NumericUpDown();
            this.labelPriceRank = new System.Windows.Forms.Label();
            this.listBoxRank = new System.Windows.Forms.ListBox();
            this.comboBoxRank = new System.Windows.Forms.ComboBox();
            this.labelMaxrank = new System.Windows.Forms.Label();
            this.checkBoxRank = new System.Windows.Forms.CheckBox();
            this.groupBoxTcolor = new System.Windows.Forms.GroupBox();
            this.numericUpDownTcolor = new System.Windows.Forms.NumericUpDown();
            this.labelPriceTcolor = new System.Windows.Forms.Label();
            this.checkBoxTcolor = new System.Windows.Forms.CheckBox();
            this.groupBoxColor = new System.Windows.Forms.GroupBox();
            this.numericUpDownColor = new System.Windows.Forms.NumericUpDown();
            this.labelPriceColor = new System.Windows.Forms.Label();
            this.checkBoxColor = new System.Windows.Forms.CheckBox();
            this.groupBoxTitle = new System.Windows.Forms.GroupBox();
            this.numericUpDownTitle = new System.Windows.Forms.NumericUpDown();
            this.labelPriceTitle = new System.Windows.Forms.Label();
            this.checkBoxTitle = new System.Windows.Forms.CheckBox();
            this.checkBoxEco = new System.Windows.Forms.CheckBox();
            this.groupBoxEco.SuspendLayout();
            this.groupBoxLevel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.groupBoxRank.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRank)).BeginInit();
            this.groupBoxTcolor.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTcolor)).BeginInit();
            this.groupBoxColor.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownColor)).BeginInit();
            this.groupBoxTitle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTitle)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBoxEco
            // 
            this.groupBoxEco.AutoSize = true;
            this.groupBoxEco.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBoxEco.Controls.Add(this.groupBoxLevel);
            this.groupBoxEco.Controls.Add(this.groupBoxRank);
            this.groupBoxEco.Controls.Add(this.groupBoxTcolor);
            this.groupBoxEco.Controls.Add(this.groupBoxColor);
            this.groupBoxEco.Controls.Add(this.groupBoxTitle);
            this.groupBoxEco.Controls.Add(this.checkBoxEco);
            this.groupBoxEco.Location = new System.Drawing.Point(12, 12);
            this.groupBoxEco.Name = "groupBoxEco";
            this.groupBoxEco.Size = new System.Drawing.Size(432, 560);
            this.groupBoxEco.TabIndex = 0;
            this.groupBoxEco.TabStop = false;
            this.groupBoxEco.Text = "Economy";
            // 
            // groupBoxLevel
            // 
            this.groupBoxLevel.Controls.Add(this.buttonEdit);
            this.groupBoxLevel.Controls.Add(this.dataGridView1);
            this.groupBoxLevel.Controls.Add(this.buttonRemove);
            this.groupBoxLevel.Controls.Add(this.buttonAdd);
            this.groupBoxLevel.Controls.Add(this.checkBoxLevel);
            this.groupBoxLevel.Enabled = false;
            this.groupBoxLevel.Location = new System.Drawing.Point(6, 288);
            this.groupBoxLevel.Name = "groupBoxLevel";
            this.groupBoxLevel.Size = new System.Drawing.Size(420, 251);
            this.groupBoxLevel.TabIndex = 4;
            this.groupBoxLevel.TabStop = false;
            this.groupBoxLevel.Text = "Level";
            // 
            // buttonEdit
            // 
            this.buttonEdit.Enabled = false;
            this.buttonEdit.Location = new System.Drawing.Point(108, 217);
            this.buttonEdit.Name = "buttonEdit";
            this.buttonEdit.Size = new System.Drawing.Size(96, 28);
            this.buttonEdit.TabIndex = 10;
            this.buttonEdit.Text = "Edit";
            this.buttonEdit.UseVisualStyleBackColor = true;
            this.buttonEdit.Click += new System.EventHandler(this.buttonEdit_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnName,
            this.ColumnPrice,
            this.ColumnX,
            this.ColumnY,
            this.ColumnZ,
            this.ColumnType});
            this.dataGridView1.Enabled = false;
            this.dataGridView1.Location = new System.Drawing.Point(6, 48);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowTemplate.Height = 24;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(408, 163);
            this.dataGridView1.TabIndex = 9;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // ColumnName
            // 
            this.ColumnName.HeaderText = "Name";
            this.ColumnName.Name = "ColumnName";
            this.ColumnName.ReadOnly = true;
            // 
            // ColumnPrice
            // 
            this.ColumnPrice.HeaderText = "Price";
            this.ColumnPrice.Name = "ColumnPrice";
            this.ColumnPrice.ReadOnly = true;
            // 
            // ColumnX
            // 
            this.ColumnX.HeaderText = "X";
            this.ColumnX.Name = "ColumnX";
            this.ColumnX.ReadOnly = true;
            // 
            // ColumnY
            // 
            this.ColumnY.HeaderText = "Y";
            this.ColumnY.Name = "ColumnY";
            this.ColumnY.ReadOnly = true;
            // 
            // ColumnZ
            // 
            this.ColumnZ.HeaderText = "Z";
            this.ColumnZ.Name = "ColumnZ";
            this.ColumnZ.ReadOnly = true;
            // 
            // ColumnType
            // 
            this.ColumnType.HeaderText = "Type";
            this.ColumnType.Name = "ColumnType";
            this.ColumnType.ReadOnly = true;
            // 
            // buttonRemove
            // 
            this.buttonRemove.Enabled = false;
            this.buttonRemove.Location = new System.Drawing.Point(318, 217);
            this.buttonRemove.Name = "buttonRemove";
            this.buttonRemove.Size = new System.Drawing.Size(96, 28);
            this.buttonRemove.TabIndex = 8;
            this.buttonRemove.Text = "Remove";
            this.buttonRemove.UseVisualStyleBackColor = true;
            this.buttonRemove.Click += new System.EventHandler(this.buttonRemove_Click);
            // 
            // buttonAdd
            // 
            this.buttonAdd.Enabled = false;
            this.buttonAdd.Location = new System.Drawing.Point(6, 217);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(96, 28);
            this.buttonAdd.TabIndex = 7;
            this.buttonAdd.Text = "Add";
            this.buttonAdd.UseVisualStyleBackColor = true;
            this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
            // 
            // checkBoxLevel
            // 
            this.checkBoxLevel.AutoSize = true;
            this.checkBoxLevel.Location = new System.Drawing.Point(6, 21);
            this.checkBoxLevel.Name = "checkBoxLevel";
            this.checkBoxLevel.Size = new System.Drawing.Size(74, 21);
            this.checkBoxLevel.TabIndex = 6;
            this.checkBoxLevel.Text = "Enable";
            this.checkBoxLevel.UseVisualStyleBackColor = true;
            this.checkBoxLevel.CheckedChanged += new System.EventHandler(this.checkBoxLevel_CheckedChanged);
            // 
            // groupBoxRank
            // 
            this.groupBoxRank.Controls.Add(this.numericUpDownRank);
            this.groupBoxRank.Controls.Add(this.labelPriceRank);
            this.groupBoxRank.Controls.Add(this.listBoxRank);
            this.groupBoxRank.Controls.Add(this.comboBoxRank);
            this.groupBoxRank.Controls.Add(this.labelMaxrank);
            this.groupBoxRank.Controls.Add(this.checkBoxRank);
            this.groupBoxRank.Enabled = false;
            this.groupBoxRank.Location = new System.Drawing.Point(176, 48);
            this.groupBoxRank.Name = "groupBoxRank";
            this.groupBoxRank.Size = new System.Drawing.Size(250, 234);
            this.groupBoxRank.TabIndex = 3;
            this.groupBoxRank.TabStop = false;
            this.groupBoxRank.Text = "Rank";
            // 
            // numericUpDownRank
            // 
            this.numericUpDownRank.Enabled = false;
            this.numericUpDownRank.Location = new System.Drawing.Point(6, 100);
            this.numericUpDownRank.Maximum = new decimal(new int[] {
            16777215,
            0,
            0,
            0});
            this.numericUpDownRank.Name = "numericUpDownRank";
            this.numericUpDownRank.Size = new System.Drawing.Size(102, 22);
            this.numericUpDownRank.TabIndex = 5;
            this.numericUpDownRank.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDownRank.ValueChanged += new System.EventHandler(this.numericUpDownRank_ValueChanged);
            // 
            // labelPriceRank
            // 
            this.labelPriceRank.AutoSize = true;
            this.labelPriceRank.Enabled = false;
            this.labelPriceRank.Location = new System.Drawing.Point(6, 80);
            this.labelPriceRank.Name = "labelPriceRank";
            this.labelPriceRank.Size = new System.Drawing.Size(40, 17);
            this.labelPriceRank.TabIndex = 5;
            this.labelPriceRank.Text = "Price";
            // 
            // listBoxRank
            // 
            this.listBoxRank.Enabled = false;
            this.listBoxRank.FormattingEnabled = true;
            this.listBoxRank.ItemHeight = 16;
            this.listBoxRank.Location = new System.Drawing.Point(114, 80);
            this.listBoxRank.Name = "listBoxRank";
            this.listBoxRank.Size = new System.Drawing.Size(127, 148);
            this.listBoxRank.TabIndex = 8;
            this.listBoxRank.SelectedIndexChanged += new System.EventHandler(this.listBoxRank_SelectedIndexChanged);
            // 
            // comboBoxRank
            // 
            this.comboBoxRank.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxRank.Enabled = false;
            this.comboBoxRank.FormattingEnabled = true;
            this.comboBoxRank.Location = new System.Drawing.Point(77, 42);
            this.comboBoxRank.Name = "comboBoxRank";
            this.comboBoxRank.Size = new System.Drawing.Size(164, 24);
            this.comboBoxRank.TabIndex = 7;
            this.comboBoxRank.SelectionChangeCommitted += new System.EventHandler(this.comboBoxRank_SelectionChangeCommitted);
            // 
            // labelMaxrank
            // 
            this.labelMaxrank.AutoSize = true;
            this.labelMaxrank.Enabled = false;
            this.labelMaxrank.Location = new System.Drawing.Point(6, 45);
            this.labelMaxrank.Name = "labelMaxrank";
            this.labelMaxrank.Size = new System.Drawing.Size(65, 17);
            this.labelMaxrank.TabIndex = 6;
            this.labelMaxrank.Text = "Max rank";
            // 
            // checkBoxRank
            // 
            this.checkBoxRank.AutoSize = true;
            this.checkBoxRank.Location = new System.Drawing.Point(6, 21);
            this.checkBoxRank.Name = "checkBoxRank";
            this.checkBoxRank.Size = new System.Drawing.Size(74, 21);
            this.checkBoxRank.TabIndex = 5;
            this.checkBoxRank.Text = "Enable";
            this.checkBoxRank.UseVisualStyleBackColor = true;
            this.checkBoxRank.CheckedChanged += new System.EventHandler(this.checkBoxRank_CheckedChanged);
            // 
            // groupBoxTcolor
            // 
            this.groupBoxTcolor.Controls.Add(this.numericUpDownTcolor);
            this.groupBoxTcolor.Controls.Add(this.labelPriceTcolor);
            this.groupBoxTcolor.Controls.Add(this.checkBoxTcolor);
            this.groupBoxTcolor.Enabled = false;
            this.groupBoxTcolor.Location = new System.Drawing.Point(6, 208);
            this.groupBoxTcolor.Name = "groupBoxTcolor";
            this.groupBoxTcolor.Size = new System.Drawing.Size(164, 74);
            this.groupBoxTcolor.TabIndex = 2;
            this.groupBoxTcolor.TabStop = false;
            this.groupBoxTcolor.Text = "Titlecolor";
            // 
            // numericUpDownTcolor
            // 
            this.numericUpDownTcolor.Enabled = false;
            this.numericUpDownTcolor.Location = new System.Drawing.Point(52, 43);
            this.numericUpDownTcolor.Maximum = new decimal(new int[] {
            16777215,
            0,
            0,
            0});
            this.numericUpDownTcolor.Name = "numericUpDownTcolor";
            this.numericUpDownTcolor.Size = new System.Drawing.Size(102, 22);
            this.numericUpDownTcolor.TabIndex = 6;
            this.numericUpDownTcolor.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numericUpDownTcolor.ValueChanged += new System.EventHandler(this.numericUpDownTcolor_ValueChanged);
            // 
            // labelPriceTcolor
            // 
            this.labelPriceTcolor.AutoSize = true;
            this.labelPriceTcolor.Enabled = false;
            this.labelPriceTcolor.Location = new System.Drawing.Point(6, 45);
            this.labelPriceTcolor.Name = "labelPriceTcolor";
            this.labelPriceTcolor.Size = new System.Drawing.Size(40, 17);
            this.labelPriceTcolor.TabIndex = 6;
            this.labelPriceTcolor.Text = "Price";
            // 
            // checkBoxTcolor
            // 
            this.checkBoxTcolor.AutoSize = true;
            this.checkBoxTcolor.Location = new System.Drawing.Point(6, 21);
            this.checkBoxTcolor.Name = "checkBoxTcolor";
            this.checkBoxTcolor.Size = new System.Drawing.Size(74, 21);
            this.checkBoxTcolor.TabIndex = 6;
            this.checkBoxTcolor.Text = "Enable";
            this.checkBoxTcolor.UseVisualStyleBackColor = true;
            this.checkBoxTcolor.CheckedChanged += new System.EventHandler(this.checkBoxTcolor_CheckedChanged);
            // 
            // groupBoxColor
            // 
            this.groupBoxColor.Controls.Add(this.numericUpDownColor);
            this.groupBoxColor.Controls.Add(this.labelPriceColor);
            this.groupBoxColor.Controls.Add(this.checkBoxColor);
            this.groupBoxColor.Enabled = false;
            this.groupBoxColor.Location = new System.Drawing.Point(6, 128);
            this.groupBoxColor.Name = "groupBoxColor";
            this.groupBoxColor.Size = new System.Drawing.Size(164, 74);
            this.groupBoxColor.TabIndex = 2;
            this.groupBoxColor.TabStop = false;
            this.groupBoxColor.Text = "Color";
            // 
            // numericUpDownColor
            // 
            this.numericUpDownColor.Enabled = false;
            this.numericUpDownColor.Location = new System.Drawing.Point(52, 43);
            this.numericUpDownColor.Maximum = new decimal(new int[] {
            16777215,
            0,
            0,
            0});
            this.numericUpDownColor.Name = "numericUpDownColor";
            this.numericUpDownColor.Size = new System.Drawing.Size(102, 22);
            this.numericUpDownColor.TabIndex = 5;
            this.numericUpDownColor.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numericUpDownColor.ValueChanged += new System.EventHandler(this.numericUpDownColor_ValueChanged);
            // 
            // labelPriceColor
            // 
            this.labelPriceColor.AutoSize = true;
            this.labelPriceColor.Enabled = false;
            this.labelPriceColor.Location = new System.Drawing.Point(6, 45);
            this.labelPriceColor.Name = "labelPriceColor";
            this.labelPriceColor.Size = new System.Drawing.Size(40, 17);
            this.labelPriceColor.TabIndex = 5;
            this.labelPriceColor.Text = "Price";
            // 
            // checkBoxColor
            // 
            this.checkBoxColor.AutoSize = true;
            this.checkBoxColor.Location = new System.Drawing.Point(6, 21);
            this.checkBoxColor.Name = "checkBoxColor";
            this.checkBoxColor.Size = new System.Drawing.Size(74, 21);
            this.checkBoxColor.TabIndex = 5;
            this.checkBoxColor.Text = "Enable";
            this.checkBoxColor.UseVisualStyleBackColor = true;
            this.checkBoxColor.CheckedChanged += new System.EventHandler(this.checkBoxColor_CheckedChanged);
            // 
            // groupBoxTitle
            // 
            this.groupBoxTitle.Controls.Add(this.numericUpDownTitle);
            this.groupBoxTitle.Controls.Add(this.labelPriceTitle);
            this.groupBoxTitle.Controls.Add(this.checkBoxTitle);
            this.groupBoxTitle.Enabled = false;
            this.groupBoxTitle.Location = new System.Drawing.Point(6, 48);
            this.groupBoxTitle.Name = "groupBoxTitle";
            this.groupBoxTitle.Size = new System.Drawing.Size(164, 74);
            this.groupBoxTitle.TabIndex = 1;
            this.groupBoxTitle.TabStop = false;
            this.groupBoxTitle.Text = "Title";
            // 
            // numericUpDownTitle
            // 
            this.numericUpDownTitle.Enabled = false;
            this.numericUpDownTitle.Location = new System.Drawing.Point(52, 43);
            this.numericUpDownTitle.Maximum = new decimal(new int[] {
            16777215,
            0,
            0,
            0});
            this.numericUpDownTitle.Name = "numericUpDownTitle";
            this.numericUpDownTitle.Size = new System.Drawing.Size(102, 22);
            this.numericUpDownTitle.TabIndex = 4;
            this.numericUpDownTitle.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numericUpDownTitle.ValueChanged += new System.EventHandler(this.numericUpDownTitle_ValueChanged);
            // 
            // labelPriceTitle
            // 
            this.labelPriceTitle.AutoSize = true;
            this.labelPriceTitle.Enabled = false;
            this.labelPriceTitle.Location = new System.Drawing.Point(6, 45);
            this.labelPriceTitle.Name = "labelPriceTitle";
            this.labelPriceTitle.Size = new System.Drawing.Size(40, 17);
            this.labelPriceTitle.TabIndex = 4;
            this.labelPriceTitle.Text = "Price";
            // 
            // checkBoxTitle
            // 
            this.checkBoxTitle.AutoSize = true;
            this.checkBoxTitle.Location = new System.Drawing.Point(6, 21);
            this.checkBoxTitle.Name = "checkBoxTitle";
            this.checkBoxTitle.Size = new System.Drawing.Size(74, 21);
            this.checkBoxTitle.TabIndex = 3;
            this.checkBoxTitle.Text = "Enable";
            this.checkBoxTitle.UseVisualStyleBackColor = true;
            this.checkBoxTitle.CheckedChanged += new System.EventHandler(this.checkBoxTitle_CheckedChanged);
            // 
            // checkBoxEco
            // 
            this.checkBoxEco.AutoSize = true;
            this.checkBoxEco.Location = new System.Drawing.Point(6, 21);
            this.checkBoxEco.Name = "checkBoxEco";
            this.checkBoxEco.Size = new System.Drawing.Size(74, 21);
            this.checkBoxEco.TabIndex = 0;
            this.checkBoxEco.Text = "Enable";
            this.checkBoxEco.UseVisualStyleBackColor = true;
            this.checkBoxEco.CheckedChanged += new System.EventHandler(this.checkBoxEco_CheckedChanged);
            // 
            // EconomyWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(454, 579);
            this.Controls.Add(this.groupBoxEco);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EconomyWindow";
            this.ShowIcon = false;
            this.Text = "Economy Settings";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.EconomyWindow_FormClosing);
            this.Load += new System.EventHandler(this.EconomyWindow_Load);
            this.groupBoxEco.ResumeLayout(false);
            this.groupBoxEco.PerformLayout();
            this.groupBoxLevel.ResumeLayout(false);
            this.groupBoxLevel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.groupBoxRank.ResumeLayout(false);
            this.groupBoxRank.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRank)).EndInit();
            this.groupBoxTcolor.ResumeLayout(false);
            this.groupBoxTcolor.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTcolor)).EndInit();
            this.groupBoxColor.ResumeLayout(false);
            this.groupBoxColor.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownColor)).EndInit();
            this.groupBoxTitle.ResumeLayout(false);
            this.groupBoxTitle.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTitle)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxEco;
        private System.Windows.Forms.GroupBox groupBoxTcolor;
        private System.Windows.Forms.GroupBox groupBoxColor;
        private System.Windows.Forms.GroupBox groupBoxTitle;
        private System.Windows.Forms.NumericUpDown numericUpDownTitle;
        private System.Windows.Forms.Label labelPriceTitle;
        private System.Windows.Forms.CheckBox checkBoxTitle;
        private System.Windows.Forms.CheckBox checkBoxEco;
        private System.Windows.Forms.GroupBox groupBoxLevel;
        private System.Windows.Forms.CheckBox checkBoxLevel;
        private System.Windows.Forms.GroupBox groupBoxRank;
        private System.Windows.Forms.ListBox listBoxRank;
        private System.Windows.Forms.ComboBox comboBoxRank;
        private System.Windows.Forms.Label labelMaxrank;
        private System.Windows.Forms.CheckBox checkBoxRank;
        private System.Windows.Forms.NumericUpDown numericUpDownTcolor;
        private System.Windows.Forms.Label labelPriceTcolor;
        private System.Windows.Forms.CheckBox checkBoxTcolor;
        private System.Windows.Forms.NumericUpDown numericUpDownColor;
        private System.Windows.Forms.Label labelPriceColor;
        private System.Windows.Forms.CheckBox checkBoxColor;
        private System.Windows.Forms.Button buttonRemove;
        private System.Windows.Forms.Button buttonAdd;
        private System.Windows.Forms.NumericUpDown numericUpDownRank;
        private System.Windows.Forms.Label labelPriceRank;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button buttonEdit;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnPrice;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnX;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnY;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnZ;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnType;

    }
}