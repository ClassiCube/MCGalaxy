using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace MCGalaxy.Gui
{
    public partial class EditText : Form
    {
        public EditText()
        {
            InitializeComponent();
        }

        private void EditTxt_Unload(object sender, EventArgs e)
        {
            checkforsave(sender, e);
        }
        bool loaded = false;
        string oldtxt;
        string loadedfile;

        private void LoadTxt_Click(object sender, EventArgs e)
        {
            if (EdittxtCombo.Text == null)
            {
                MessageBox.Show("Please select a file");
                return;
            }
            checkforsave(sender, e);
            switch (EdittxtCombo.Text)
            {
                case "Autoload":
                    loadedfile = "autoload";
                    break;
                case "AwardsList":
                    loadedfile = "awardsList";
                    break;
                case "Badwords":
                    loadedfile = "badwords";
                    break;
                case "CmdAutoload":
                    loadedfile = "cmdautoload";
                    break;
                case "Custom$s":
                    loadedfile = "custom$s";
                    break;
                case "Emotelist":
                    loadedfile = "emotelist";
                    break;
                case "Joker":
                    loadedfile = "joker";
                    break;
                case "Messages":
                    loadedfile = "messages";
                    break;
                case "PlayerAwards":
                    loadedfile = "playerAwards";
                    break;
                case "Rules":
                    loadedfile = "rules";
                    if (!File.Exists("text/rules.txt"))
                    {
                        File.Create("text/rules.txt").Dispose();
                        Server.s.Log("Created rules.txt");
                    }
                    break;
                case "Welcome":
                    loadedfile = "welcome";
                    break;
                default:
                    loaded = false;
                    loadedfile = null;
                    MessageBox.Show("Something went wrong!!");
                    return;
            }
            loaded = true;
            try
            {
                if (File.Exists("text/" + loadedfile + ".txt")) { oldtxt = File.ReadAllText("text/" + loadedfile + ".txt"); }
                else { MessageBox.Show("File doesn't exist!!"); loaded = false; loadedfile = null; return; }
            }
            catch { MessageBox.Show("Something went wrong!!"); loaded = false; loadedfile = null; return; }
            EditTextTxtBox.Text = oldtxt;
        }

        private void checkforsave(object sender, EventArgs e)
        {
            if (loaded == true && EditTextTxtBox.Text != oldtxt)
            {
                if (MessageBox.Show("Do you want to save what you were editing?", "Save?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    save(sender, e);
                }
            }
        }

        private void save(object sender, EventArgs e)
        {
            File.WriteAllText("text/" + loadedfile + ".txt", EditTextTxtBox.Text);
            oldtxt = File.ReadAllText("text/" + loadedfile + ".txt");
            MessageBox.Show("Saved Text");
        }

        private void SaveEditTxtBt_Click(object sender, EventArgs e)
        {
            if (loaded == true)
            {
                save(sender, e);
            }
            else
            {
                MessageBox.Show("No file is loaded!!");
                return;
            }
        }

        private void DiscardEdittxtBt_Click(object sender, EventArgs e)
        {
            if (loaded == true)
            {
                File.WriteAllText("text/" + loadedfile + ".txt", oldtxt);
                EditTextTxtBox.Text = File.ReadAllText("text/" + loadedfile + ".txt");
                MessageBox.Show("Discarded Text");
            }
            else
            {
                MessageBox.Show("No file is loaded!!");
                return;
            }
        }
    }
}
