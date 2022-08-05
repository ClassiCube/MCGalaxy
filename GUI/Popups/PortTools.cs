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
using System.Drawing;
using System.Windows.Forms;

namespace MCGalaxy.Gui.Popups 
{
    public partial class PortTools : Form 
    {
        readonly BackgroundWorker worker;
        int port;
        
        public PortTools(int port) {
            InitializeComponent();
            worker = new BackgroundWorker { WorkerSupportsCancellation = true };
            worker.DoWork += mWorkerForwarder_DoWork;
            worker.RunWorkerCompleted += mWorkerForwarder_RunWorkerCompleted;
            
            this.port = port;
            btnForward.Text = "Forward " + port;
        }
        
        void PortTools_Load(object sender, EventArgs e) {
            GuiUtils.SetIcon(this);
        }

        void linkManually_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            GuiUtils.OpenBrowser("https://www.canyouseeme.org/");
        }

        void PortChecker_FormClosing(object sender, FormClosingEventArgs e) {
            worker.CancelAsync();
        }

        void linkHelpForward_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            GuiUtils.OpenBrowser("https://portforward.com");
        }

        void btnForward_Click(object sender, EventArgs e) {
            SetUPnPEnabled(false);
            worker.RunWorkerAsync(true);
        }

        void btnDelete_Click(object sender, EventArgs e) {
            SetUPnPEnabled(false);
            worker.RunWorkerAsync(false);
        }

        void mWorkerForwarder_DoWork(object sender, DoWorkEventArgs e) {
            bool adding = (bool)e.Argument;
            
            try {
                if (!UPnP.Discover()) {
                    e.Result = 0;
                } else if (adding) {                   
                    UPnP.ForwardPort(port, UPnP.TCP_PROTOCOL, Server.SoftwareName + "Server");
                    e.Result = 1;
                } else {
                    UPnP.DeleteForwardingRule(port, UPnP.TCP_PROTOCOL);
                    e.Result = 3;
                }
            } catch (Exception ex) {
                Logger.LogError("Unexpected UPnP error", ex);
                e.Result = 2;
            }
        }

        void mWorkerForwarder_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            if (e.Cancelled) return;
            SetUPnPEnabled(true);

            int result = (int)e.Result;
            switch (result) {
                case 0:
                    lblResult.Text = "Error contacting router.";
                    lblResult.ForeColor = Color.Red;
                    return;
                case 1:
                    lblResult.Text = "Port forwarded automatically using UPnP";
                    lblResult.ForeColor = Color.Green;
                    return;
                case 2:
                    lblResult.Text = "Unexpected error, see Error Logs";
                    lblResult.ForeColor = Color.Red;
                    return;
                case 3:
                    lblResult.Text = "Deleted port forward rule";
                    lblResult.ForeColor = Color.Green;
                    return;
            }
        }
        
        void SetUPnPEnabled(bool enabled) {
            btnDelete.Enabled  = enabled;
            btnForward.Enabled = enabled;        
        }
    }
}
