/*
 
    Copyright 2012 MCForge
    Dual-licensed under the Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
 
    http://www.opensource.org/licenses/ecl2.php
    http://www.gnu.org/licenses/gpl-3.0.html
 
    Unless required by applicable law or agreed to in writing,
    software distributed under the Licenses are distributed on an "AS IS"
    BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
    or implied. See the Licenses for the specific language governing
    permissions and limitations under the Licenses.
*/
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Net.Sockets;
using System.Windows.Forms;
using MCGalaxy.Core;

namespace MCGalaxy.Gui.Popups {
    public partial class PortTools : Form {

        readonly BackgroundWorker worker;
        public PortTools() {
            InitializeComponent();
            worker = new BackgroundWorker { WorkerSupportsCancellation = true };
            worker.DoWork += mWorkerForwarder_DoWork;
            worker.RunWorkerCompleted += mWorkerForwarder_RunWorkerCompleted;
        }

        private void linkManually_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            try { Process.Start("http://www.canyouseeme.org/"); }
            catch { }
        }

        private void PortChecker_FormClosing(object sender, FormClosingEventArgs e) {
            worker.CancelAsync();
        }

        private void linkHelpForward_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            try { Process.Start("http://portforward.com"); }
            catch { }
        }

        private void btnForward_Click(object sender, EventArgs e) {
            int port = 25565;
            if (String.IsNullOrEmpty(txtPortForward.Text.Trim()))
                txtPortForward.Text = "25565";

            try {
                port = int.Parse(txtPortForward.Text);
            } catch {
                txtPortForward.Text = "25565";
            }
            
            SetUPnPEnabled(false);
            worker.RunWorkerAsync(new object[] { port, true });
        }

        private void btnDelete_Click(object sender, EventArgs e) {
            int port = 25565;
            if (String.IsNullOrEmpty(txtPortForward.Text.Trim()))
                txtPortForward.Text = "25565";

            try {
                port = int.Parse(txtPortForward.Text);
            } catch {
                txtPortForward.Text = "25565";
            }

            SetUPnPEnabled(false);
            worker.RunWorkerAsync(new object[] { port, false });
        }

        void mWorkerForwarder_DoWork(object sender, DoWorkEventArgs e) {
            int tries = 0;
            int port = (int)((object[])e.Argument)[0];
            bool adding = (bool)((object[])e.Argument)[1];
            
            retry:
            try {
                tries++;
                if (!UPnP.Discover()) {
                    e.Result = 0;
                } else if (adding) {                   
                    UPnP.ForwardPort(port, ProtocolType.Tcp, Server.SoftwareName + "Server");
                    e.Result = 1;
                } else {
                    UPnP.DeleteForwardingRule(port, ProtocolType.Tcp);
                    e.Result = 3;
                }
            } catch {
                if (tries < 2) goto retry;
                e.Result = 2;
            }
        }

        void mWorkerForwarder_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            if (e.Cancelled) return;
            SetUPnPEnabled(true);

            int result = (int)e.Result;
            switch (result) {
                case 0:
                    lblForward.Text = "Error contacting router.";
                    lblForward.ForeColor = Color.Red;
                    return;
                case 1:
                    lblForward.Text = "Port forwarded automatically using UPnP";
                    lblForward.ForeColor = Color.Green;
                    return;
                case 2:
                    lblForward.Text = "Something Weird just happened, try again.";
                    lblForward.ForeColor = Color.Black;
                    return;
                case 3:
                    lblForward.Text = "Deleted Port Forward Rule.";
                    lblForward.ForeColor = Color.Green;
                    return;
            }
        }
        
        void SetUPnPEnabled(bool enabled) {
            btnDelete.Enabled = enabled;
            btnForward.Enabled = enabled;
            txtPortForward.Enabled = enabled;            
        }
    }
}
