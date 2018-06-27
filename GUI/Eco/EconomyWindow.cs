using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MCGalaxy.Eco;
using Microsoft.Win32;

namespace MCGalaxy.Gui.Eco {
    public partial class EconomyWindow : Form {
        public EconomyWindow() {
            InitializeComponent();
        }

        void EconomyWindow_Load(object sender, EventArgs e) {
            UpdateRanks();
            UpdateLevels();
            UpdateEnabledItems();
            
            ttl_numPrice.Value = Economy.Title.Price;
            col_numPrice.Value = Economy.Color.Price;
            tcl_numPrice.Value = Economy.TitleColor.Price;
            CheckLevelEnables();

            SystemEvents.UserPreferenceChanged += SystemEvents_UserPreferenceChanged;
            this.Font = SystemFonts.IconTitleFont;
        }

        void SystemEvents_UserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e) {
            if (e.Category == UserPreferenceCategory.Window) {
                this.Font = SystemFonts.IconTitleFont;
            }
        }

        void PropertyWindow_FormClosing(object sender, FormClosingEventArgs e) {
            SystemEvents.UserPreferenceChanged -= SystemEvents_UserPreferenceChanged;
        }
        
        void UpdateEnabledItems() {
            eco_cbEnabled.Checked = Economy.Enabled;
            
            ttl_cbEnabled.Checked = eco_cbEnabled.Checked && Economy.Title.Enabled;
            ttl_gb.Enabled        = ttl_cbEnabled.Checked;
            ttl_lblPrice.Enabled  = ttl_cbEnabled.Checked;
            ttl_numPrice.Enabled  = ttl_cbEnabled.Checked;
            
            col_cbEnabled.Checked = eco_cbEnabled.Checked && Economy.Color.Enabled;
            col_gb.Enabled        = col_cbEnabled.Checked;
            col_lblPrice.Enabled  = col_cbEnabled.Checked;
            col_numPrice.Enabled  = col_cbEnabled.Checked;
            
            tcl_cbEnabled.Checked = eco_cbEnabled.Checked && Economy.TitleColor.Enabled;
            tcl_gb.Enabled        = tcl_cbEnabled.Checked;
            tcl_lblPrice.Enabled  = tcl_cbEnabled.Checked;
            tcl_numPrice.Enabled  = tcl_cbEnabled.Checked;
            
            rnk_cbEnabled.Checked = eco_cbEnabled.Checked && Economy.Ranks.Enabled;
            rnk_gb.Enabled        = rnk_cbEnabled.Checked;
            rnk_lbRanks.Enabled   = rnk_cbEnabled.Checked;
            rnk_lblPrice.Enabled  = rnk_cbEnabled.Checked;
            rnk_numPrice.Enabled  = rnk_cbEnabled.Checked;
            
            lvl_cbEnabled.Checked = eco_cbEnabled.Checked && Economy.Levels.Enabled;
            lvl_gb.Enabled        = lvl_cbEnabled.Checked;      
            lvl_dgvMaps.Enabled   = lvl_cbEnabled.Checked;
            lvl_btnAdd.Enabled    = lvl_cbEnabled.Checked;            
        }

        void UpdateRanks() {
            rnk_lbRanks.Items.Clear();
            for (int i = 0; i < GuiPerms.RankPerms.Length; i++) {
                RankItem.RankEntry rank = Economy.Ranks.Find(GuiPerms.RankPerms[i]);
                int price = rank == null ? 0 : rank.Price;
                rnk_lbRanks.Items.Add(GuiPerms.RankNames[i] + " - " + price);
            }
            rnk_lbRanks_SelectedIndexChanged(null, null);
        }

        public void UpdateLevels() {
            lvl_dgvMaps.Rows.Clear();
            foreach (LevelItem.LevelPreset preset in Economy.Levels.Presets) {
                lvl_dgvMaps.Rows.Add(preset.name, preset.price, preset.x, preset.y, preset.z, preset.type);
            }
        }

        public void CheckLevelEnables() {
            lvl_btnRemove.Enabled = lvl_cbEnabled.Checked && lvl_dgvMaps.SelectedRows.Count > 0;
            lvl_btnEdit.Enabled   = lvl_cbEnabled.Checked && lvl_dgvMaps.SelectedRows.Count > 0;
        }

        void eco_cbEnabled_CheckedChanged(object sender, EventArgs e) {
            UpdateEnabledItems();
        }

        void ttl_cbEnabled_CheckedChanged(object sender, EventArgs e) {
            UpdateEnabledItems();
            Economy.Title.Enabled = ttl_cbEnabled.Checked;
            Economy.Title.Price = (int)ttl_numPrice.Value;
        }

        void col_cbEnabled_CheckedChanged(object sender, EventArgs e) {
            UpdateEnabledItems();
            Economy.Color.Enabled = col_cbEnabled.Checked;
            Economy.Color.Price = (int)col_numPrice.Value;
        }

        void tcl_cbEnabled_CheckedChanged(object sender, EventArgs e) {
            UpdateEnabledItems();
            Economy.TitleColor.Enabled = tcl_cbEnabled.Checked;
            Economy.TitleColor.Price = (int)tcl_numPrice.Value;
        }

        void ttl_numPrice_ValueChanged(object sender, EventArgs e) {
            Economy.Title.Price = (int)ttl_numPrice.Value;
        }

        void col_numPrice_ValueChanged(object sender, EventArgs e) {
            Economy.Color.Price = (int)col_numPrice.Value;
        }

        void tcl_numPrice_ValueChanged(object sender, EventArgs e) {
            Economy.TitleColor.Price = (int)tcl_numPrice.Value;
        }

        void rnk_cbEnabled_CheckedChanged(object sender, EventArgs e) {
            UpdateEnabledItems();
            Economy.Ranks.Enabled = rnk_cbEnabled.Checked;
        }

        void rnk_numPrice_ValueChanged(object sender, EventArgs e) {
            int selI = rnk_lbRanks.SelectedIndex;
            if (selI == -1) return;
            LevelPermission perm = GuiPerms.RankPerms[selI];
            
            RankItem.RankEntry rank = Economy.Ranks.GetOrAdd(perm);
            rank.Price = (int)rnk_numPrice.Value;
            if (rank.Price == 0) Economy.Ranks.Remove(perm);
            rnk_lbRanks.Items[selI] = GuiPerms.RankNames[selI] + " - " + rank.Price;
        }

        void rnk_lbRanks_SelectedIndexChanged(object sender, EventArgs e) {
            int selI = rnk_lbRanks.SelectedIndex;
            if (selI == -1) { rnk_numPrice.Value = 0; return; }
            LevelPermission perm = GuiPerms.RankPerms[selI];
            
            RankItem.RankEntry rank = Economy.Ranks.Find(perm);
            rnk_numPrice.Value = rank == null ? 0 : rank.Price;
        }

        void EconomyWindow_FormClosing(object sender, FormClosingEventArgs e) {
            Dispose();
            Economy.Save();
        }

        void lvl_cbEnabled_CheckedChanged(object sender, EventArgs e) {
            lvl_dgvMaps.Enabled = lvl_cbEnabled.Checked;
            lvl_btnAdd.Enabled = lvl_cbEnabled.Checked;
            CheckLevelEnables();
            Economy.Levels.Enabled = lvl_cbEnabled.Checked;
        }

        void lvl_btnAdd_Click(object sender, EventArgs e) {
            new EcoLevelWindow("", 1000, "64", "64", "64", "flat").ShowDialog();
        }

        void lvl_btnEdit_Click(object sender, EventArgs e) {
            DataGridViewRow row = lvl_dgvMaps.SelectedRows[0];
            string name = row.Cells[0].Value.ToString();
            int price   = (int)row.Cells[1].Value;
            string x    = row.Cells[2].Value.ToString();
            string y    = row.Cells[3].Value.ToString();
            string z    = row.Cells[4].Value.ToString();
            string type = row.Cells[5].Value.ToString();
            new EcoLevelWindow(name, price, x, y, z, type).ShowDialog();
        }

        void lvl_btnRemove_Click(object sender, EventArgs e) {
            LevelItem item = Economy.Levels;
            DataGridViewRow row = lvl_dgvMaps.SelectedRows[0];
            item.Presets.Remove(item.FindPreset(row.Cells[0].Value.ToString()));
            lvl_dgvMaps.Rows.RemoveAt(row.Index);
            CheckLevelEnables();
        }

        void lvl_dgvMaps_CellContentClick(object sender, DataGridViewCellEventArgs e) {
            lvl_btnEdit.Enabled   = true;
            lvl_btnRemove.Enabled = true;
        }
    }
}