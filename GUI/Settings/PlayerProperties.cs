/*
    Copyright 2015 MCGalaxy
        
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

namespace MCGalaxy.Gui {
    public sealed class PlayerProperties {       
        readonly Player player;
        public PlayerProperties(Player player) {
            this.player = player;
        }
        
        [Description("Whether the player is frozen or not.")]
        [Category("Status")]
        [DisplayName("Frozen")]
        public bool Frozen { get { return player.frozen; } set { DoCommand("freeze"); } }
        
        [Description("Whether the player is hidden or not.")]
        [Category("Status")]
        [DisplayName("Hidden")]
        public bool Hidden { get { return player.hidden; } set { DoCommand("ohide"); } }
        
        [Description("Whether the player is muted or not.")]
        [Category("Status")]
        [DisplayName("Muted")]
        public bool Muted { get { return player.hidden; } set { DoCommand("mute"); } }
        
        void DoCommand(string cmd) {
            Player p = PlayerInfo.FindExact(player.name);
            if (p == null) return;

            try {
                Command.all.Find(cmd).Use(null, p.name);
            } catch (Exception ex) {
                Server.ErrorLog(ex);
            }
        }
    }
}
