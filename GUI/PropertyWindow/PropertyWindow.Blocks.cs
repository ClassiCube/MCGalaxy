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
using System.Collections.Generic;
using System.Windows.Forms;
using MCGalaxy.Blocks;

namespace MCGalaxy.Gui {
    public partial class PropertyWindow : Form {

        void listBlocks_SelectedIndexChanged(object sender, EventArgs e) {
            byte b = Block.Byte(listBlocks.SelectedItem.ToString());
            BlockPerms bs = storedBlocks.Find(bS => bS.BlockID == b);

            txtBlLowest.Text = (int)bs.MinRank + "";

            bool foundOne = false;
            txtBlDisallow.Text = "";
            if (bs.Disallowed != null) {
                foreach ( LevelPermission perm in bs.Disallowed ) {
                    foundOne = true;
                    txtBlDisallow.Text += "," + (int)perm;
                }
            }
            if ( foundOne ) txtBlDisallow.Text = txtBlDisallow.Text.Remove(0, 1);

            foundOne = false;
            txtBlAllow.Text = "";
            if (bs.Allowed != null) {
                foreach ( LevelPermission perm in bs.Allowed ) {
                    foundOne = true;
                    txtBlAllow.Text += "," + (int)perm;
                }
            }
            if ( foundOne ) txtBlAllow.Text = txtBlAllow.Text.Remove(0, 1);
        }
        void txtBlLowest_TextChanged(object sender, EventArgs e) {
        	fillLowest(ref txtBlLowest, ref storedBlocks[Block.Byte(listBlocks.SelectedItem.ToString())].MinRank);
        }
        void txtBlDisallow_TextChanged(object sender, EventArgs e) {
            if (storedBlocks[listBlocks.SelectedIndex].Disallowed == null)
                storedBlocks[listBlocks.SelectedIndex].Disallowed = new List<LevelPermission>();
            fillAllowance(ref txtBlDisallow, ref storedBlocks[listBlocks.SelectedIndex].Disallowed);
        }
        void txtBlAllow_TextChanged(object sender, EventArgs e) {
            if (storedBlocks[listBlocks.SelectedIndex].Allowed == null)
                storedBlocks[listBlocks.SelectedIndex].Allowed = new List<LevelPermission>();
            fillAllowance(ref txtBlAllow, ref storedBlocks[listBlocks.SelectedIndex].Allowed);
        }

        void btnBlHelp_Click(object sender, EventArgs e) {
            getHelp(listBlocks.SelectedItem.ToString());
        }
    }
}
