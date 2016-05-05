/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
    Dual-licensed under the    Educational Community License, Version 2.0 and
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
using System.IO;
using System.Text;

namespace MCGalaxy.Commands
{
    public sealed class CmdBanlist : Command
    {
        public override string name { get { return "banlist"; } }
        public override string shortcut { get { return "bl"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public CmdBanlist() { }

        public override void Use(Player p, string message) {
            StringBuilder list = new StringBuilder();
            foreach (string line in File.ReadAllLines("ranks/banned.txt")) {
                string col = Ban.IsBanned(line) ? "&a" : "&c";
                list.Append(col).Append(line).Append("%S, ");
            }
            
            if (list.Length == 0) {
                Player.Message(p, "There are no players banned");
            } else {
                string msg = "&9Banned players: %S" + list.ToString(0, list.Length - 2) + "%S.";
                Player.Message(p, msg);
            }
        }
        
        public override void Help(Player p) {
            Player.Message(p, "/banlist - shows who is banned on the server");
        }
    }
}
