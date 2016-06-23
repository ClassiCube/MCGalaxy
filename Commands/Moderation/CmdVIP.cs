/*
    Copyright 2011 MCForge
        
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
using System.Collections.Generic;

namespace MCGalaxy.Commands {
    public sealed class CmdVIP : Command {
        public override string name { get { return "vip"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        public CmdVIP() { }

        public override void Use(Player p, string message) {
            if (message == "") { Help(p); return; }
            string[] args = message.Split(' ');
            
            if (args[0] == "add") {
                if (args.Length < 2) { Help(p); return; }
                Player pl = PlayerInfo.Find(args[1]);
                if (pl != null) args[1] = pl.name;
                
                if (Server.vip.Contains(args[1])) {
                    Player.Message(p, Name(pl, args[1]) + " %Sis already a VIP.");
                } else {
                    Server.vip.Add(args[1]);
                    Server.vip.Save(false);
                    Player.Message(p, Name(pl, args[1]) + " %Sis now a VIP.");
                    if (pl != null) Player.Message(pl, "You are now a VIP!");
                }
            } else if (args[0] == "remove") {
                if (args.Length < 2) { Help(p); return; }
                Player pl = PlayerInfo.Find(args[1]);
                if (pl != null) args[1] = pl.name;
                
                if (Server.vip.Contains(args[1])) {
                    Player.Message(p, Name(pl, args[1]) + " %Sis not a VIP.");
                } else {
                    Server.vip.Remove(args[1]);
                    Server.vip.Save(false);
                    Player.Message(p, Name(pl, args[1]) + " %Sis no longer a VIP.");
                    if (pl != null) Player.Message(pl, "You are no longer a VIP!");
                }
            } else if (args[0] == "list") {
                List<string> list = Server.vip.All();
                if (list.Count == 0) {
                    Player.Message(p, "There are no VIPs.");
                } else {
                    string count = list.Count > 1 ? "is 1" : "are " + list.Count;
                    Player.Message(p, "There " + count + " VIPs:");
                    Player.Message(p, list.Concatenate(", "));
                }
            } else {
                Help(p);
            }
        }
        
        static string Name(Player pl, string name) { return pl == null ? name : pl.ColoredName; }
        
        public override void Help(Player p) {           
            Player.Message(p, "%T/vip <add/remove> [player]");
            Player.Message(p, "%HAdds or removes [player] from the VIP list.");
            Player.Message(p, "%T/vip list");
            Player.Message(p, "%HLists all players who are on the VIP list.");
            Player.Message(p, "%H  VIPs can join regardless of the player limit.");
        }
    }
}
