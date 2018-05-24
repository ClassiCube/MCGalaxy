namespace MCGalaxy.Gui.Eco {
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
            this.eco_gb = new System.Windows.Forms.GroupBox();
            this.lvl_gb = new System.Windows.Forms.GroupBox();
            this.lvl_btnEdit = new System.Windows.Forms.Button();
            this.lvl_dgvMaps = new System.Windows.Forms.DataGridView();
            this.ColumnName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnX = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnY = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnZ = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lvl_btnRemove = new System.Windows.Forms.Button();
            this.lvl_btnAdd = new System.Windows.Forms.Button();
            this.lvl_cbEnabled = new System.Windows.Forms.CheckBox();
            this.rnk_gb = new System.Windows.Forms.GroupBox();
            this.rnk_numPrice = new System.Windows.Forms.NumericUpDown();
            this.rnk_lblPrice = new System.Windows.Forms.Label();
            this.rnk_lbRanks = new System.Windows.Forms.ListBox();
            this.rnk_cbEnabled = new System.Windows.Forms.CheckBox();
            this.tcl_gb = new System.Windows.Forms.GroupBox();
            this.tcl_numPrice = new System.Windows.Forms.NumericUpDown();
            this.tcl_lblPrice = new System.Windows.Forms.Label();
            this.tcl_cbEnabled = new System.Windows.Forms.CheckBox();
            this.col_gb = new System.Windows.Forms.GroupBox();
            this.col_numPrice = new System.Windows.Forms.NumericUpDown();
            this.col_lblPrice = new System.Windows.Forms.Label();
            this.col_cbEnabled = new System.Windows.Forms.CheckBox();
            this.ttl_gb = new System.Windows.Forms.GroupBox();
            this.ttl_numPrice = new System.Windows.Forms.NumericUpDown();
            this.ttl_lblPrice = new System.Windows.Forms.Label();
            this.ttl_cbEnabled = new System.Windows.Forms.CheckBox();
            this.eco_cbEnabled = new System.Windows.Forms.CheckBox();
            this.eco_gb.SuspendLayout();
            this.lvl_gb.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lvl_dgvMaps)).BeginInit();
            this.rnk_gb.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rnk_numPrice)).BeginInit();
            this.tcl_gb.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tcl_numPrice)).BeginInit();
            this.col_gb.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.col_numPrice)).BeginInit();
            this.ttl_gb.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ttl_numPrice)).BeginInit();
            this.SuspendLayout();
            // 
            // eco_gb
            // 
            this.eco_gb.AutoSize = true;
            this.eco_gb.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.eco_gb.Controls.Add(this.lvl_gb);
            this.eco_gb.Controls.Add(this.rnk_gb);
            this.eco_gb.Controls.Add(this.tcl_gb);
            this.eco_gb.Controls.Add(this.col_gb);
            this.eco_gb.Controls.Add(this.ttl_gb);
            this.eco_gb.Controls.Add(this.eco_cbEnabled);
            this.eco_gb.Location = new System.Drawing.Point(12, 12);
            this.eco_gb.Name = "eco_gb";
            this.eco_gb.Size = new System.Drawing.Size(432, 560);
            this.eco_gb.TabIndex = 0;
            this.eco_gb.TabStop = false;
            this.eco_gb.Text = "Economy";
            // 
            // lvl_gb
            // 
            this.lvl_gb.Controls.Add(this.lvl_btnEdit);
            this.lvl_gb.Controls.Add(this.lvl_dgvMaps);
            this.lvl_gb.Controls.Add(this.lvl_btnRemove);
            this.lvl_gb.Controls.Add(this.lvl_btnAdd);
            this.lvl_gb.Controls.Add(this.lvl_cbEnabled);
            this.lvl_gb.Enabled = false;
            this.lvl_gb.Location = new System.Drawing.Point(6, 288);
            this.lvl_gb.Name = "lvl_gb";
            this.lvl_gb.Size = new System.Drawing.Size(420, 251);
            this.lvl_gb.TabIndex = 4;
            this.lvl_gb.TabStop = false;
            this.lvl_gb.Text = "Level";
            // 
            // lvl_btnEdit
            // 
            this.lvl_btnEdit.Enabled = false;
            this.lvl_btnEdit.Location = new System.Drawing.Point(108, 217);
            this.lvl_btnEdit.Name = "lvl_btnEdit";
            this.lvl_btnEdit.Size = new System.Drawing.Size(96, 28);
            this.lvl_btnEdit.TabIndex = 10;
            this.lvl_btnEdit.Text = "Edit";
            this.lvl_btnEdit.UseVisualStyleBackColor = true;
            this.lvl_btnEdit.Click += new System.EventHandler(this.lvl_btnEdit_Click);
            // 
            // dataGridView1
            // 
            this.lvl_dgvMaps.AllowUserToAddRows = false;
            this.lvl_dgvMaps.AllowUserToDeleteRows = false;
            this.lvl_dgvMaps.AllowUserToResizeRows = false;
            this.lvl_dgvMaps.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            this.lvl_dgvMaps.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.lvl_dgvMaps.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnName,
            this.ColumnPrice,
            this.ColumnX,
            this.ColumnY,
            this.ColumnZ,
            this.ColumnType});
            this.lvl_dgvMaps.Enabled = false;
            this.lvl_dgvMaps.Location = new System.Drawing.Point(6, 48);
            this.lvl_dgvMaps.MultiSelect = false;
            this.lvl_dgvMaps.Name = "dataGridView1";
            this.lvl_dgvMaps.ReadOnly = true;
            this.lvl_dgvMaps.RowTemplate.Height = 24;
            this.lvl_dgvMaps.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.lvl_dgvMaps.Size = new System.Drawing.Size(408, 163);
            this.lvl_dgvMaps.TabIndex = 9;
            this.lvl_dgvMaps.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.lvl_dgvMaps_CellContentClick);
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
            // lvl_btnRemove
            // 
            this.lvl_btnRemove.Enabled = false;
            this.lvl_btnRemove.Location = new System.Drawing.Point(318, 217);
            this.lvl_btnRemove.Name = "lvl_btnRemove";
            this.lvl_btnRemove.Size = new System.Drawing.Size(96, 28);
            this.lvl_btnRemove.TabIndex = 8;
            this.lvl_btnRemove.Text = "Remove";
            this.lvl_btnRemove.UseVisualStyleBackColor = true;
            this.lvl_btnRemove.Click += new System.EventHandler(this.lvl_btnRemove_Click);
            // 
            // lvl_btnAdd
            // 
            this.lvl_btnAdd.Enabled = false;
            this.lvl_btnAdd.Location = new System.Drawing.Point(6, 217);
            this.lvl_btnAdd.Name = "lvl_btnAdd";
            this.lvl_btnAdd.Size = new System.Drawing.Size(96, 28);
            this.lvl_btnAdd.TabIndex = 7;
            this.lvl_btnAdd.Text = "Add";
            this.lvl_btnAdd.UseVisualStyleBackColor = true;
            this.lvl_btnAdd.Click += new System.EventHandler(this.lvl_btnAdd_Click);
            // 
            // lvl_cbEnabled
            // 
            this.lvl_cbEnabled.AutoSize = true;
            this.lvl_cbEnabled.Location = new System.Drawing.Point(6, 21);
            this.lvl_cbEnabled.Name = "lvl_cbEnabled";
            this.lvl_cbEnabled.Size = new System.Drawing.Size(74, 21);
            this.lvl_cbEnabled.TabIndex = 6;
            this.lvl_cbEnabled.Text = "Enable";
            this.lvl_cbEnabled.UseVisualStyleBackColor = true;
            this.lvl_cbEnabled.CheckedChanged += new System.EventHandler(this.lvl_cbEnabled_CheckedChanged);
            // 
            // rnk_gb
            // 
            this.rnk_gb.Controls.Add(this.rnk_numPrice);
            this.rnk_gb.Controls.Add(this.rnk_lblPrice);
            this.rnk_gb.Controls.Add(this.rnk_lbRanks);
            this.rnk_gb.Controls.Add(this.rnk_cbEnabled);
            this.rnk_gb.Enabled = false;
            this.rnk_gb.Location = new System.Drawing.Point(176, 48);
            this.rnk_gb.Name = "rnk_gb";
            this.rnk_gb.Size = new System.Drawing.Size(250, 234);
            this.rnk_gb.TabIndex = 3;
            this.rnk_gb.TabStop = false;
            this.rnk_gb.Text = "Rank";
            // 
            // rnk_numPrice
            // 
            this.rnk_numPrice.Enabled = false;
            this.rnk_numPrice.Location = new System.Drawing.Point(6, 68);
            this.rnk_numPrice.Maximum = new decimal(new int[] {
            16777215,
            0,
            0,
            0});
            this.rnk_numPrice.Name = "rnk_numPrice";
            this.rnk_numPrice.Size = new System.Drawing.Size(102, 22);
            this.rnk_numPrice.TabIndex = 5;
            this.rnk_numPrice.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.rnk_numPrice.ValueChanged += new System.EventHandler(this.rnk_numPrice_ValueChanged);
            // 
            // rnk_lblPrice
            // 
            this.rnk_lblPrice.AutoSize = true;
            this.rnk_lblPrice.Enabled = false;
            this.rnk_lblPrice.Location = new System.Drawing.Point(6, 48);
            this.rnk_lblPrice.Name = "rnk_lblPrice";
            this.rnk_lblPrice.Size = new System.Drawing.Size(40, 17);
            this.rnk_lblPrice.TabIndex = 5;
            this.rnk_lblPrice.Text = "Price";
            // 
            // rnk_lbRanks
            // 
            this.rnk_lbRanks.Enabled = false;
            this.rnk_lbRanks.FormattingEnabled = true;
            this.rnk_lbRanks.ItemHeight = 16;
            this.rnk_lbRanks.Location = new System.Drawing.Point(114, 48);
            this.rnk_lbRanks.Name = "rnk_lbRanks";
            this.rnk_lbRanks.Size = new System.Drawing.Size(127, 180);
            this.rnk_lbRanks.TabIndex = 8;
            this.rnk_lbRanks.SelectedIndexChanged += new System.EventHandler(this.rnk_lbRanks_SelectedIndexChanged);
            // 
            // rnk_cbEnabled
            // 
            this.rnk_cbEnabled.AutoSize = true;
            this.rnk_cbEnabled.Location = new System.Drawing.Point(6, 21);
            this.rnk_cbEnabled.Name = "rnk_cbEnabled";
            this.rnk_cbEnabled.Size = new System.Drawing.Size(74, 21);
            this.rnk_cbEnabled.TabIndex = 5;
            this.rnk_cbEnabled.Text = "Enable";
            this.rnk_cbEnabled.UseVisualStyleBackColor = true;
            this.rnk_cbEnabled.CheckedChanged += new System.EventHandler(this.rnk_cbEnabled_CheckedChanged);
            // 
            // tcl_gb
            // 
            this.tcl_gb.Controls.Add(this.tcl_numPrice);
            this.tcl_gb.Controls.Add(this.tcl_lblPrice);
            this.tcl_gb.Controls.Add(this.tcl_cbEnabled);
            this.tcl_gb.Enabled = false;
            this.tcl_gb.Location = new System.Drawing.Point(6, 208);
            this.tcl_gb.Name = "tcl_gb";
            this.tcl_gb.Size = new System.Drawing.Size(164, 74);
            this.tcl_gb.TabIndex = 2;
            this.tcl_gb.TabStop = false;
            this.tcl_gb.Text = "Titlecolor";
            // 
            // tcl_numPrice
            // 
            this.tcl_numPrice.Enabled = false;
            this.tcl_numPrice.Location = new System.Drawing.Point(52, 43);
            this.tcl_numPrice.Maximum = new decimal(new int[] {
            16777215,
            0,
            0,
            0});
            this.tcl_numPrice.Name = "tcl_numPrice";
            this.tcl_numPrice.Size = new System.Drawing.Size(102, 22);
            this.tcl_numPrice.TabIndex = 6;
            this.tcl_numPrice.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.tcl_numPrice.ValueChanged += new System.EventHandler(this.tcl_numPrice_ValueChanged);
            // 
            // tcl_lblPrice
            // 
            this.tcl_lblPrice.AutoSize = true;
            this.tcl_lblPrice.Enabled = false;
            this.tcl_lblPrice.Location = new System.Drawing.Point(6, 45);
            this.tcl_lblPrice.Name = "tcl_lblPrice";
            this.tcl_lblPrice.Size = new System.Drawing.Size(40, 17);
            this.tcl_lblPrice.TabIndex = 6;
            this.tcl_lblPrice.Text = "Price";
            // 
            // tcl_cbEnabled
            // 
            this.tcl_cbEnabled.AutoSize = true;
            this.tcl_cbEnabled.Location = new System.Drawing.Point(6, 21);
            this.tcl_cbEnabled.Name = "tcl_cbEnabled";
            this.tcl_cbEnabled.Size = new System.Drawing.Size(74, 21);
            this.tcl_cbEnabled.TabIndex = 6;
            this.tcl_cbEnabled.Text = "Enable";
            this.tcl_cbEnabled.UseVisualStyleBackColor = true;
            this.tcl_cbEnabled.CheckedChanged += new System.EventHandler(this.tcl_cbEnabled_CheckedChanged);
            // 
            // col_gb
            // 
            this.col_gb.Controls.Add(this.col_numPrice);
            this.col_gb.Controls.Add(this.col_lblPrice);
            this.col_gb.Controls.Add(this.col_cbEnabled);
            this.col_gb.Enabled = false;
            this.col_gb.Location = new System.Drawing.Point(6, 128);
            this.col_gb.Name = "col_gb";
            this.col_gb.Size = new System.Drawing.Size(164, 74);
            this.col_gb.TabIndex = 2;
            this.col_gb.TabStop = false;
            this.col_gb.Text = "Color";
            // 
            // col_numPrice
            // 
            this.col_numPrice.Enabled = false;
            this.col_numPrice.Location = new System.Drawing.Point(52, 43);
            this.col_numPrice.Maximum = new decimal(new int[] {
            16777215,
            0,
            0,
            0});
            this.col_numPrice.Name = "col_numPrice";
            this.col_numPrice.Size = new System.Drawing.Size(102, 22);
            this.col_numPrice.TabIndex = 5;
            this.col_numPrice.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.col_numPrice.ValueChanged += new System.EventHandler(this.col_numPrice_ValueChanged);
            // 
            // col_lblPrice
            // 
            this.col_lblPrice.AutoSize = true;
            this.col_lblPrice.Enabled = false;
            this.col_lblPrice.Location = new System.Drawing.Point(6, 45);
            this.col_lblPrice.Name = "col_lblPrice";
            this.col_lblPrice.Size = new System.Drawing.Size(40, 17);
            this.col_lblPrice.TabIndex = 5;
            this.col_lblPrice.Text = "Price";
            // 
            // col_cbEnabled
            // 
            this.col_cbEnabled.AutoSize = true;
            this.col_cbEnabled.Location = new System.Drawing.Point(6, 21);
            this.col_cbEnabled.Name = "col_cbEnabled";
            this.col_cbEnabled.Size = new System.Drawing.Size(74, 21);
            this.col_cbEnabled.TabIndex = 5;
            this.col_cbEnabled.Text = "Enable";
            this.col_cbEnabled.UseVisualStyleBackColor = true;
            this.col_cbEnabled.CheckedChanged += new System.EventHandler(this.col_cbEnabled_CheckedChanged);
            // 
            // ttl_gb
            // 
            this.ttl_gb.Controls.Add(this.ttl_numPrice);
            this.ttl_gb.Controls.Add(this.ttl_lblPrice);
            this.ttl_gb.Controls.Add(this.ttl_cbEnabled);
            this.ttl_gb.Enabled = false;
            this.ttl_gb.Location = new System.Drawing.Point(6, 48);
            this.ttl_gb.Name = "ttl_gb";
            this.ttl_gb.Size = new System.Drawing.Size(164, 74);
            this.ttl_gb.TabIndex = 1;
            this.ttl_gb.TabStop = false;
            this.ttl_gb.Text = "Title";
            // 
            // ttl_numPrice
            // 
            this.ttl_numPrice.Enabled = false;
            this.ttl_numPrice.Location = new System.Drawing.Point(52, 43);
            this.ttl_numPrice.Maximum = new decimal(new int[] {
            16777215,
            0,
            0,
            0});
            this.ttl_numPrice.Name = "ttl_numPrice";
            this.ttl_numPrice.Size = new System.Drawing.Size(102, 22);
            this.ttl_numPrice.TabIndex = 4;
            this.ttl_numPrice.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.ttl_numPrice.ValueChanged += new System.EventHandler(this.ttl_numPrice_ValueChanged);
            // 
            // ttl_lblPrice
            // 
            this.ttl_lblPrice.AutoSize = true;
            this.ttl_lblPrice.Enabled = false;
            this.ttl_lblPrice.Location = new System.Drawing.Point(6, 45);
            this.ttl_lblPrice.Name = "ttl_lblPrice";
            this.ttl_lblPrice.Size = new System.Drawing.Size(40, 17);
            this.ttl_lblPrice.TabIndex = 4;
            this.ttl_lblPrice.Text = "Price";
            // 
            // ttl_cbEnabled
            // 
            this.ttl_cbEnabled.AutoSize = true;
            this.ttl_cbEnabled.Location = new System.Drawing.Point(6, 21);
            this.ttl_cbEnabled.Name = "ttl_cbEnabled";
            this.ttl_cbEnabled.Size = new System.Drawing.Size(74, 21);
            this.ttl_cbEnabled.TabIndex = 3;
            this.ttl_cbEnabled.Text = "Enable";
            this.ttl_cbEnabled.UseVisualStyleBackColor = true;
            this.ttl_cbEnabled.CheckedChanged += new System.EventHandler(this.ttl_cbEnabled_CheckedChanged);
            // 
            // eco_cbEnabled
            // 
            this.eco_cbEnabled.AutoSize = true;
            this.eco_cbEnabled.Location = new System.Drawing.Point(6, 21);
            this.eco_cbEnabled.Name = "eco_cbEnabled";
            this.eco_cbEnabled.Size = new System.Drawing.Size(74, 21);
            this.eco_cbEnabled.TabIndex = 0;
            this.eco_cbEnabled.Text = "Enable";
            this.eco_cbEnabled.UseVisualStyleBackColor = true;
            this.eco_cbEnabled.CheckedChanged += new System.EventHandler(this.eco_cbEnabled_CheckedChanged);
            // 
            // EconomyWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(454, 579);
            this.Controls.Add(this.eco_gb);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EconomyWindow";
            this.ShowIcon = false;
            this.Text = "Economy Settings";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.EconomyWindow_FormClosing);
            this.Load += new System.EventHandler(this.EconomyWindow_Load);
            this.eco_gb.ResumeLayout(false);
            this.eco_gb.PerformLayout();
            this.lvl_gb.ResumeLayout(false);
            this.lvl_gb.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lvl_dgvMaps)).EndInit();
            this.rnk_gb.ResumeLayout(false);
            this.rnk_gb.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rnk_numPrice)).EndInit();
            this.tcl_gb.ResumeLayout(false);
            this.tcl_gb.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tcl_numPrice)).EndInit();
            this.col_gb.ResumeLayout(false);
            this.col_gb.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.col_numPrice)).EndInit();
            this.ttl_gb.ResumeLayout(false);
            this.ttl_gb.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ttl_numPrice)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox eco_gb;
        private System.Windows.Forms.GroupBox tcl_gb;
        private System.Windows.Forms.GroupBox col_gb;
        private System.Windows.Forms.GroupBox ttl_gb;
        private System.Windows.Forms.NumericUpDown ttl_numPrice;
        private System.Windows.Forms.Label ttl_lblPrice;
        private System.Windows.Forms.CheckBox ttl_cbEnabled;
        private System.Windows.Forms.CheckBox eco_cbEnabled;
        private System.Windows.Forms.GroupBox lvl_gb;
        private System.Windows.Forms.CheckBox lvl_cbEnabled;
        private System.Windows.Forms.GroupBox rnk_gb;
        private System.Windows.Forms.ListBox rnk_lbRanks;
        private System.Windows.Forms.CheckBox rnk_cbEnabled;
        private System.Windows.Forms.NumericUpDown tcl_numPrice;
        private System.Windows.Forms.Label tcl_lblPrice;
        private System.Windows.Forms.CheckBox tcl_cbEnabled;
        private System.Windows.Forms.NumericUpDown col_numPrice;
        private System.Windows.Forms.Label col_lblPrice;
        private System.Windows.Forms.CheckBox col_cbEnabled;
        private System.Windows.Forms.Button lvl_btnRemove;
        private System.Windows.Forms.Button lvl_btnAdd;
        private System.Windows.Forms.NumericUpDown rnk_numPrice;
        private System.Windows.Forms.Label rnk_lblPrice;
        private System.Windows.Forms.DataGridView lvl_dgvMaps;
        private System.Windows.Forms.Button lvl_btnEdit;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnPrice;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnX;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnY;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnZ;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnType;

    }
}