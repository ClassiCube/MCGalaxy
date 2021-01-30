/*
    Copyright 2010 MCLawl Team - Written by Valek (Modified for use with MCGalaxy)
 
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
using MCGalaxy.DB;

namespace MCGalaxy.Commands.Chatting {
    public class CmdTColor : EntityPropertyCmd {
        public override string name { get { return "TColor"; } }
        public override string type { get { return CommandTypes.Chat; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        public override CommandPerm[] ExtraPerms {
            get { return new[] { new CommandPerm(LevelPermission.Operator, "can change the title color of others") }; }
        }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("TColour"), new CommandAlias("XTColor", "-own") }; }
        }
        public override void Use(Player p, string message, CommandData data) { 
            UsePlayer(p, data, message, "title color"); 
        }
        
        protected override void SetPlayerData(Player p, string target, string colName) {
            string col = "";
            Player who = PlayerInfo.FindExact(target);
            
            if (colName.Length == 0) {
                MessageFrom(target, who, "had their title color removed");
            } else  {
                col = Matcher.FindColor(p, colName);
                if (col == null) return;
                MessageFrom(target, who, "had their title color changed to " + col + Colors.Name(col));
            }
            
            if (who != null) who.titlecolor = col;
            if (who != null) who.SetPrefix();
            PlayerDB.Update(target, PlayerData.ColumnTColor, col);
        }

        public override void Help(Player p) {
            p.Message("&T/TColor [player] [color]");
            p.Message("&HSets the title color of [player]");
            p.Message("&H  If [color] is not given, title color is removed.");
            p.Message("&HTo see a list of all colors, use &T/Help colors.");
        }
    }
}
