using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using MCGalaxy.Eco;

namespace MCGalaxy.GUI.Eco {
    public partial class EconomyWindow : Form {
        public EconomyWindow() {
            InitializeComponent();
        }

        private void EconomyWindow_Load(object sender, EventArgs e) {
			numericUpDownTitle.Value = ((SimpleItem)Economy.GetItem("title")).Price;
			numericUpDownColor.Value = ((SimpleItem)Economy.GetItem("color")).Price;
			numericUpDownTcolor.Value = ((SimpleItem)Economy.GetItem("titlecolor")).Price;
            checkBoxEco.Checked = Economy.Settings.Enabled;
            checkBoxTitle.Checked = Economy.GetItem("title").Enabled;
            checkBoxColor.Checked = Economy.GetItem("color").Enabled;
            checkBoxTcolor.Checked = Economy.GetItem("titlecolor").Enabled;
            checkBoxRank.Checked = Economy.Settings.Ranks;
            checkBoxLevel.Checked = Economy.Settings.Levels;

            //load all ranks in combobox
            List<string> groupList = new List<string>();
            foreach (Group group in Group.GroupList)
                if(group.Permission > 0 && (int)group.Permission < 120)
                    groupList.Add(group.name);
            comboBoxRank.DataSource = groupList;
            comboBoxRank.SelectedItem = Economy.Settings.MaxRank;

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
            int lasttrueprice = 0;
            foreach (Group group in Group.GroupList) {
                if (group.Permission > Group.Find(Economy.Settings.MaxRank).Permission) { break; }
                if (!(group.Permission <= Group.Find(Server.defaultRank).Permission)) {
                    Economy.Settings.Rank rank = new Economy.Settings.Rank();
                    rank = Economy.FindRank(group.name);
                    if (rank == null) {
                        rank = new Economy.Settings.Rank();
                        rank.group = group;
                        if (lasttrueprice == 0) { rank.price = 1000; } else { rank.price = lasttrueprice + 250; }
                        Economy.Settings.RanksList.Add(rank);
                    } else { lasttrueprice = rank.price; }
                }
            }
            List<string> ranklist = new List<string>();
            foreach (Economy.Settings.Rank rank in Economy.Settings.RanksList) {
                ranklist.Add(rank.group.name);
                if (rank.group.name == Economy.Settings.MaxRank)
                    break;
            }
            listBoxRank.DataSource = ranklist;
            listBoxRank.SelectedItem = comboBoxRank.SelectedItem;
            numericUpDownRank.Value = Economy.Settings.RanksList.Find(rank => rank.group.name == comboBoxRank.SelectedItem.ToString()).price;
        }

        public void UpdateLevels() {
            dataGridView1.Rows.Clear();
            foreach (Economy.Settings.Level lvl in Economy.Settings.LevelsList) {
                dataGridView1.Rows.Add(lvl.name, lvl.price, lvl.x, lvl.y, lvl.z, lvl.type);
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
            Economy.Settings.Enabled = checkBoxEco.Checked;
        }

        private void checkBoxTitle_CheckedChanged(object sender, EventArgs e) {
            labelPriceTitle.Enabled = checkBoxTitle.Checked;
            numericUpDownTitle.Enabled = checkBoxTitle.Checked;
            Economy.GetItem("title").Enabled = checkBoxTitle.Checked;
            ((SimpleItem)Economy.GetItem("title")).Price = (int)numericUpDownTitle.Value;
        }

        private void checkBoxColor_CheckedChanged(object sender, EventArgs e) {
            labelPriceColor.Enabled = checkBoxColor.Checked;
            numericUpDownColor.Enabled = checkBoxColor.Checked;
            Economy.GetItem("color").Enabled = checkBoxColor.Checked;
            ((SimpleItem)Economy.GetItem("color")).Price = (int)numericUpDownColor.Value;
        }

        private void checkBoxTcolor_CheckedChanged(object sender, EventArgs e) {
            labelPriceTcolor.Enabled = checkBoxTcolor.Checked;
            numericUpDownTcolor.Enabled = checkBoxTcolor.Checked;
            Economy.GetItem("titlecolor").Enabled = checkBoxTcolor.Checked;
            ((SimpleItem)Economy.GetItem("titlecolor")).Price = (int)numericUpDownTcolor.Value;
        }

        private void numericUpDownTitle_ValueChanged(object sender, EventArgs e) {
			((SimpleItem)Economy.GetItem("title")).Price = (int)numericUpDownTitle.Value;
        }

        private void numericUpDownColor_ValueChanged(object sender, EventArgs e) {
            ((SimpleItem)Economy.GetItem("color")).Price = (int)numericUpDownColor.Value;
        }

        private void numericUpDownTcolor_ValueChanged(object sender, EventArgs e) {
            ((SimpleItem)Economy.GetItem("titlecolor")).Price = (int)numericUpDownTcolor.Value;
        }

        private void checkBoxRank_CheckedChanged(object sender, EventArgs e) {
            labelMaxrank.Enabled = checkBoxRank.Checked;
            comboBoxRank.Enabled = checkBoxRank.Checked;
            listBoxRank.Enabled = checkBoxRank.Checked;
            labelPriceRank.Enabled = checkBoxRank.Checked;
            numericUpDownRank.Enabled = checkBoxRank.Checked;
            Economy.Settings.Ranks = checkBoxRank.Checked;
        }

        private void comboBoxRank_SelectionChangeCommitted(object sender, EventArgs e) {
            Economy.Settings.MaxRank = comboBoxRank.SelectedItem.ToString();
            UpdateRanks();
        }

        private void numericUpDownRank_ValueChanged(object sender, EventArgs e) {
            Economy.FindRank(listBoxRank.SelectedItem.ToString()).price = (int)numericUpDownRank.Value;
        }

        private void listBoxRank_SelectedIndexChanged(object sender, EventArgs e) {
            numericUpDownRank.Value = Economy.FindRank(listBoxRank.SelectedItem.ToString()).price;
        }

        private void EconomyWindow_FormClosing(object sender, FormClosingEventArgs e) {
            Dispose();
            Economy.Save();
        }

        private void checkBoxLevel_CheckedChanged(object sender, EventArgs e) {
            dataGridView1.Enabled = checkBoxLevel.Checked;
            buttonAdd.Enabled = checkBoxLevel.Checked;
            CheckLevelEnables();
            Economy.Settings.Levels = checkBoxLevel.Checked;
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
            Economy.Settings.LevelsList.Remove(Economy.FindLevel(dataGridView1.SelectedRows[0].Cells[0].Value.ToString()));
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