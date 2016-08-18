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
        string inMsg, outMsg;
        
        public PlayerProperties(Player player) {
            this.player = player;
            inMsg = PlayerDB.GetLoginMessage(player);
            outMsg = PlayerDB.GetLogoutMessage(player);
        }
        
        [Category("Properties")]
        [DisplayName("Login message")]
        public string LoginMsg { get { return inMsg; } set { inMsg = DoCmd("loginmessage", value); } }
        
        [Category("Properties")]
        [DisplayName("Logout message")]
        public string LogoutMsg { get { return outMsg; } set { outMsg = DoCmd("logoutmessage", value); } }
        
        
        [Category("Status")]
        [DisplayName("Frozen")]
        public bool Frozen { get { return player.frozen; } set { DoCmd("freeze"); } }
        
        [Category("Status")]
        [DisplayName("Hidden")]
        public bool Hidden { get { return player.hidden; } set { DoCmd("ohide"); } }
        
        [Category("Status")]
        [DisplayName("Muted")]
        public bool Muted { get { return player.muted; } set { DoCmd("mute"); } }
        
        [Category("Status")]
        [DisplayName("Voiced")]
        public bool Voiced { get { return player.voice; } set { DoCmd("voice"); } }
        
        void DoCmd(string cmd) { DoCmd(cmd, ""); }
        
        string DoCmd(string cmd, string args) {
            // Is the player still on the server?
            Player p = PlayerInfo.FindExact(player.name);
            if (p == null) return args;           

            try {
                string cmdArgs = args == "" ? p.name : p.name + " " + args;
                Command.all.Find(cmd).Use(null, cmdArgs);
            } catch (Exception ex) {
                Server.ErrorLog(ex);
            }
            return args;
        }
    }
}
