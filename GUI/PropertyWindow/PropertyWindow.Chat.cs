/*
Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
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
using System.Windows.Forms;
using MCGalaxy.Gui.Popups;

namespace MCGalaxy.Gui {
    
    public partial class PropertyWindow : Form {
        
        void LoadChatProps() {
            ParseColor(Server.DefaultColor, chat_cmbDefault);
            ParseColor(Server.IRCColour, chat_cmbIRC);
            ParseColor(Server.HelpSyntaxColor, chat_cmbSyntax);
            ParseColor(Server.HelpDescriptionColor, chat_cmbDesc);
            
            chat_txtConsole.Text = Server.ZallState;
            chat_cbTabRank.Checked = Server.TablistRankSorted;
            chat_cbTabLevel.Checked = !Server.TablistGlobal;
            chat_cbTabBots.Checked = Server.TablistBots;
            
            chat_txtShutdown.Text = Server.shutdownMessage;
            chat_chkCheap.Checked = Server.cheapMessage;
            chat_txtCheap.Text = Server.cheapMessageGiven;
            chat_txtBan.Text = Server.defaultBanMessage;
            chat_txtPromote.Text = Server.defaultPromoteMessage;
            chat_txtDemote.Text = Server.defaultDemoteMessage;
        }
        
        void ApplyChatProps() {
            Server.DefaultColor = Colors.Parse(chat_cmbDefault.SelectedItem.ToString());
            Server.IRCColour = Colors.Parse(chat_cmbIRC.SelectedItem.ToString());
            Server.HelpSyntaxColor = Colors.Parse(chat_cmbSyntax.SelectedItem.ToString());
            Server.HelpDescriptionColor = Colors.Parse(chat_cmbDesc.SelectedItem.ToString());
            
            Server.ZallState = chat_txtConsole.Text;
            Server.TablistRankSorted = chat_cbTabRank.Checked;
            Server.TablistGlobal = !chat_cbTabLevel.Checked;
            Server.TablistBots = chat_cbTabBots.Checked;
            
            Server.shutdownMessage = chat_txtShutdown.Text;
            Server.cheapMessage = chat_chkCheap.Checked;
            Server.cheapMessageGiven = chat_txtCheap.Text;
            Server.defaultBanMessage = chat_txtBan.Text;
            Server.defaultPromoteMessage = chat_txtPromote.Text;
            Server.defaultDemoteMessage = chat_txtDemote.Text;
        }

		

        void chat_cmbDefault_SelectedIndexChanged(object sender, EventArgs e) {
            chat_colDefault.BackColor = GetColor(chat_cmbDefault.Items[chat_cmbDefault.SelectedIndex].ToString());
        }

        void chat_cmbIRC_SelectedIndexChanged(object sender, EventArgs e) {
            chat_colIRC.BackColor = GetColor(chat_cmbIRC.Items[chat_cmbIRC.SelectedIndex].ToString());
        }
        
        void chat_cmbSyntax_SelectedIndexChanged(object sender, EventArgs e) {
            chat_colSyntax.BackColor = GetColor(chat_cmbSyntax.Items[chat_cmbSyntax.SelectedIndex].ToString());
        }

        void chat_cmbDesc_SelectedIndexChanged(object sender, EventArgs e) {
            chat_colDesc.BackColor = GetColor(chat_cmbDesc.Items[chat_cmbDesc.SelectedIndex].ToString());
        }
    }
}
