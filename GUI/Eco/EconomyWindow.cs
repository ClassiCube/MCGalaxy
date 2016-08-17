using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MCGalaxy.Eco;
using Microsoft.Win32;

namespace MCGalaxy.GUI.Eco {
    public partial class EconomyWindow : Form {
        public EconomyWindow() {
            InitializeComponent();
        }

        private void EconomyWindow_Load(object sender, EventArgs e) {
            RankItem rankItem = Economy.Ranks;
            numericUpDownTitle.Value = Economy.Title.Price;
            numericUpDownColor.Value = Economy.Color.Price;
            numericUpDownTcolor.Value = Economy.TitleColor.Price;
            checkBoxEco.Checked = Economy.Enabled;
            checkBoxTitle.Checked = Economy.Title.Enabled;
            checkBoxColor.Checked = Economy.Color.Enabled;
            checkBoxTcolor.Checked = Economy.TitleColor.Enabled;
            checkBoxRank.Checked = rankItem.Enabled;
            checkBoxLevel.Checked = Economy.Levels.Enabled;

            //load all ranks in combobox
            List<string> groupList = new List<string>();
            foreach (Group group in Group.GroupList)
                if (group.Permission > LevelPermission.Guest && group.Permission < LevelPermission.Nobody)
                    groupList.Add(group.name);
            comboBoxRank.DataSource = groupList;
            comboBoxRank.SelectedItem = rankItem.MaxRank;

            UpdateRanks();
            UpdateLevels();
            
            //initialize enables
            groupBoxTitle.Enabled = checkBoxEco.Checked;
            groupBoxColor.Enabled = checkBoxEco.Checked;
            groupBoxTcolor.Enabled = checkBoxEco.Checked;
            groupBoxRank.Enabled = checkBoxEco.Checked;
            groupBoxLevel.Enabled = checkBoxEco.Checked;
            labelPriceTitle.Enabled = checkBoxTitle.Checked;
            labelPriceColor.Enabled = checkBoxColor.Checked;
            labelPriceTcolor.Enabled = checkBoxTcolor.Checked;
            numericUpDownTitle.Enabled = checkBoxTitle.Checked;
            numericUpDownColor.Enabled = checkBoxColor.Checked;
            numericUpDownTcolor.Enabled = checkBoxTcolor.Checked;
            labelMaxrank.Enabled = checkBoxRank.Checked;
            comboBoxRank.Enabled = checkBoxRank.Checked;
            listBoxRank.Enabled = checkBoxRank.Checked;
            labelPriceRank.Enabled = checkBoxRank.Checked;
            numericUpDownRank.Enabled = checkBoxRank.Checked;
            dataGridView1.Enabled = checkBoxLevel.Checked;
            buttonAdd.Enabled = checkBoxLevel.Checked;
            CheckLevelEnables();

            SystemEvents.UserPreferenceChanged += new UserPreferenceChangedEventHandler(SystemEvents_UserPreferenceChanged);
            this.Font = SystemFonts.IconTitleFont;
        }

        private void SystemEvents_UserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e) {
            if (e.Category == UserPreferenceCategory.Window) {
                this.Font = SystemFonts.IconTitleFont;
            }
        }

        private void PropertyWindow_FormClosing(object sender, FormClosingEventArgs e) {
            SystemEvents.UserPreferenceChanged -= new UserPreferenceChangedEventHandler(SystemEvents_UserPreferenceChanged);
        }

        private void UpdateRanks() {
            RankItem rankItem = Economy.Ranks;
            rankItem.UpdatePrices();

            List<string> ranklist = new List<string>();
            foreach (RankItem.Rank rank in rankItem.RanksList) {
                ranklist.Add(rank.group.name);
                if (rank.group.name == rankItem.MaxRank) break;
            }
            listBoxRank.DataSource = ranklist;
            listBoxRank.SelectedItem = comboBoxRank.SelectedItem;
            numericUpDownRank.Value = rankItem.FindRank(comboBoxRank.SelectedItem.ToString()).price;
        }

        public void UpdateLevels() {
            dataGridView1.Rows.Clear();
            foreach (LevelItem.LevelPreset preset in Economy.Levels.Presets) {
                dataGridView1.Rows.Add(preset.name, preset.price, preset.x, preset.y, preset.z, preset.type);
            }
        }

        public void CheckLevelEnables() {
            buttonRemove.Enabled = checkBoxLevel.Checked && dataGridView1.SelectedRows.Count > 0;
            buttonEdit.Enabled = checkBoxLevel.Checked && dataGridView1.SelectedRows.Count > 0;
        }

        private void checkBoxEco_CheckedChanged(object sender, EventArgs e) {
            groupBoxTitle.Enabled = checkBoxEco.Checked;
            groupBoxColor.Enabled = checkBoxEco.Checked;
            groupBoxTcolor.Enabled = checkBoxEco.Checked;
            groupBoxRank.Enabled = checkBoxEco.Checked;
            groupBoxLevel.Enabled = checkBoxEco.Checked;
            Economy.Enabled = checkBoxEco.Checked;
        }

        private void checkBoxTitle_CheckedChanged(object sender, EventArgs e) {
            labelPriceTitle.Enabled = checkBoxTitle.Checked;
            numericUpDownTitle.Enabled = checkBoxTitle.Checked;
            Economy.Title.Enabled = checkBoxTitle.Checked;
            Economy.Title.Price = (int)numericUpDownTitle.Value;
        }

        private void checkBoxColor_CheckedChanged(object sender, EventArgs e) {
            labelPriceColor.Enabled = checkBoxColor.Checked;
            numericUpDownColor.Enabled = checkBoxColor.Checked;
            Economy.Color.Enabled = checkBoxColor.Checked;
            Economy.Color.Price = (int)numericUpDownColor.Value;
        }

        private void checkBoxTcolor_CheckedChanged(object sender, EventArgs e) {
            labelPriceTcolor.Enabled = checkBoxTcolor.Checked;
            numericUpDownTcolor.Enabled = checkBoxTcolor.Checked;
            Economy.TitleColor.Enabled = checkBoxTcolor.Checked;
            Economy.TitleColor.Price = (int)numericUpDownTcolor.Value;
        }

        private void numericUpDownTitle_ValueChanged(object sender, EventArgs e) {
            Economy.Title.Price = (int)numericUpDownTitle.Value;
        }

        private void numericUpDownColor_ValueChanged(object sender, EventArgs e) {
            Economy.Color.Price = (int)numericUpDownColor.Value;
        }

        private void numericUpDownTcolor_ValueChanged(object sender, EventArgs e) {
            Economy.TitleColor.Price = (int)numericUpDownTcolor.Value;
        }

        private void checkBoxRank_CheckedChanged(object sender, EventArgs e) {
            labelMaxrank.Enabled = checkBoxRank.Checked;
            comboBoxRank.Enabled = checkBoxRank.Checked;
            listBoxRank.Enabled = checkBoxRank.Checked;
            labelPriceRank.Enabled = checkBoxRank.Checked;
            numericUpDownRank.Enabled = checkBoxRank.Checked;
            Economy.Ranks.Enabled = checkBoxRank.Checked;
        }

        private void comboBoxRank_SelectionChangeCommitted(object sender, EventArgs e) {
            Economy.Ranks.MaxRank = comboBoxRank.SelectedItem.ToString();
            UpdateRanks();
        }

        private void numericUpDownRank_ValueChanged(object sender, EventArgs e) {
            Economy.Ranks.FindRank(listBoxRank.SelectedItem.ToString()).price = (int)numericUpDownRank.Value;
        }

        private void listBoxRank_SelectedIndexChanged(object sender, EventArgs e) {
            numericUpDownRank.Value = Economy.Ranks.FindRank(listBoxRank.SelectedItem.ToString()).price;
        }

        private void EconomyWindow_FormClosing(object sender, FormClosingEventArgs e) {
            Dispose();
            Economy.Save();
        }

        private void checkBoxLevel_CheckedChanged(object sender, EventArgs e) {
            dataGridView1.Enabled = checkBoxLevel.Checked;
            buttonAdd.Enabled = checkBoxLevel.Checked;
            CheckLevelEnables();
            Economy.Levels.Enabled = checkBoxLevel.Checked;
        }

        private void buttonAdd_Click(object sender, EventArgs e) {
            new EcoLevelWindow(this).ShowDialog();
        }

        private void buttonEdit_Click(object sender, EventArgs e) {
            string name = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
            int price = (int)dataGridView1.SelectedRows[0].Cells[1].Value;
            string x = dataGridView1.SelectedRows[0].Cells[2].Value.ToString();
            string y = dataGridView1.SelectedRows[0].Cells[3].Value.ToString();
            string z = dataGridView1.SelectedRows[0].Cells[4].Value.ToString();
            string type = dataGridView1.SelectedRows[0].Cells[5].Value.ToString();
            new EcoLevelWindow(this, name, price, x, y, z, type, true).ShowDialog();
        }

        private void buttonRemove_Click(object sender, EventArgs e) {
			LevelItem item = Economy.Levels;
            item.Presets.Remove(item.FindPreset(dataGridView1.SelectedRows[0].Cells[0].Value.ToString()));
            dataGridView1.Rows.RemoveAt(dataGridView1.SelectedRows[0].Index);
            buttonRemove.Enabled = checkBoxLevel.Checked && dataGridView1.SelectedRows.Count > 0;
            buttonEdit.Enabled = checkBoxLevel.Checked && dataGridView1.SelectedRows.Count > 0;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) {
            buttonEdit.Enabled = true;
            buttonRemove.Enabled = true;
        }
    }
}