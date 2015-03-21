using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MCGalaxy.GUI
{
    public partial class WoM : Form
    {
        public WoM()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            if (textBox3.Text.ToCharArray().Length > 15)
            {
                MessageBox.Show("Only 15 characters allowed!", "Warning");
                return;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (WOMBeat.SetSettings(Server.IP, "" + Server.port, textBox1.Text, textBox2.Text, textBox3.Text))
            {
                MessageBox.Show("Your settings have been saved!", "Results", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Server.Server_ALT = textBox1.Text;
                Server.Server_Disc = textBox2.Text;
                Server.Server_Flag = textBox3.Text;
                this.Close();
            }
            else
                MessageBox.Show("There was an error, check the error log for more details!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void WoM_Load(object sender, EventArgs e)
        {
            textBox1.Text = Server.Server_ALT;
            textBox2.Text = Server.Server_Disc;
            textBox3.Text = Server.Server_Flag;
        }
    }
}
