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
using System.Collections.Generic;
using System.ComponentModel;
using MCGalaxy.DB;

namespace MCGalaxy.Gui {
    public sealed class PlayerProperties {
        readonly Player p;
        string inMsg, outMsg;
        
        public PlayerProperties(Player player) {
            this.p = player;
            inMsg = PlayerDB.GetLoginMessage(player);
            outMsg = PlayerDB.GetLogoutMessage(player);
        }

        [Category("Properties")]
        [DisplayName("Color")]
        [TypeConverter(typeof(ColorConverter))]
        public string Color { get { return Colors.Name(p.color); } set { DoCmd("color", value); } }
        
        [Category("Properties")]
        [DisplayName("IP address")]
        public string IP { get { return p.ip; } set { } }
        
        [Category("Properties")]
        [DisplayName("Login message")]
        public string LoginMsg { get { return inMsg; } set { inMsg = DoCmd("loginmessage", value); } }
        
        [Category("Properties")]
        [DisplayName("Logout message")]
        public string LogoutMsg { get { return outMsg; } set { outMsg = DoCmd("logoutmessage", value); } }
        
        [Category("Properties")]
        [DisplayName("Rank")]
        [TypeConverter(typeof(RankConverter))]
        public string Rank { get { return p.group.Name; } set { DoCmd("setrank", value); } }
        
        [Category("Properties")]
        [DisplayName("Title")]
        public string Title { get { return p.title; } set { DoCmd("title", value); } }

        [Category("Properties")]
        [DisplayName("Title color")]
        [TypeConverter(typeof(ColorConverter))]
        public string TColor { get { return Colors.Name(p.titlecolor); } set { DoCmd("tcolor", value); } }
        

        [Category("Stats")]
        [DisplayName("Blocks modified")]
        public long BlocksModified { get { return p.TotalModified; } set { p.TotalModified = value; } }
        
        [Category("Stats")]
        [DisplayName("Number of deaths")]
        public int TimesDied { get { return p.TimesDied; } set { p.TimesDied = value; } }
        
        [Category("Stats")]
        [DisplayName("Times been kicked")]
        public int TimesKicked { get { return p.TimesBeenKicked; } set { p.TimesBeenKicked = value; } }

        [Category("Stats")]
        [DisplayName("Number of logins")]
        public int TimesLogins { get { return p.TimesVisited; } set { p.TimesVisited = value; } }
        

        [Category("Status")]
        [DisplayName("AFK")]
        public bool AFK { get { return p.IsAfk; } set { DoCmd("sendcmd", "afk"); } }
        
        [Category("Status")]
        [DisplayName("Frozen")]
        public bool Frozen { get { return p.frozen; } set { DoCmd("freeze"); } }
        
        [Category("Status")]
        [DisplayName("Hidden")]
        public bool Hidden { get { return p.hidden; } set { DoCmd("ohide"); } }

        [Category("Status")]
        [DisplayName("Jailed")]
        public bool Jailed { get { return p.jailed; } set { DoCmd("jail"); } }
        
        [Category("Status")]
        [DisplayName("Jokered")]
        public bool Jokered { get { return p.joker; } set { DoCmd("joker"); } }
        
        [Category("Status")]
        [DisplayName("Map")]
        [TypeConverter(typeof(LevelConverter))]
        public string Map { get { return p.level.name; } set { DoCmd("sendcmd", "goto " + value); } }
        
        [Category("Status")]
        [DisplayName("Muted")]
        public bool Muted { get { return p.muted; } set { DoCmd("mute"); } }
        
        [Category("Status")]
        [DisplayName("Voiced")]
        public bool Voiced { get { return p.voice; } set { DoCmd("voice"); } }
        
        void DoCmd(string cmd) { DoCmd(cmd, ""); }
        
        string DoCmd(string cmd, string args) {
            // Is the player still on the server?
            Player pl = PlayerInfo.FindExact(p.name);
            if (pl == null) return args;

            try {
                string cmdArgs = args == "" ? p.name : p.name + " " + args;
                Command.all.Find(cmd).Use(null, cmdArgs);
            } catch (Exception ex) {
                Logger.LogError(ex);
            }
            return args;
        }
    }
}
