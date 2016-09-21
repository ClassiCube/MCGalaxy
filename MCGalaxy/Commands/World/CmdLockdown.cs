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
using System.IO;
using System.Threading;
namespace MCGalaxy.Commands {
    
    public sealed class CmdLockdown : Command {
        public override string name { get { return "lockdown"; } }
        public override string shortcut { get { return "ld"; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }

        public override void Use(Player p, string message) {
            if (!Directory.Exists("text/lockdown"))
                Directory.CreateDirectory("text/lockdown");
            if (!Directory.Exists("text/lockdown/map"))
                Directory.CreateDirectory("text/lockdown/map");

            string[] args = message.Split(' ');
            if (args.Length != 2 || !(args[0].CaselessEq("map") || args[0].CaselessEq("player"))) {
                Help(p); return;
            }

            if (args[0].CaselessEq("map")) {
                if (!Formatter.ValidName(p, args[1], "level")) return;
                
                bool unlocking = Server.lockdown.Contains(args[1]);
                Chat.MessageAll("The map {0} has been {1}locked", args[1], unlocking ? "un" : "");
                string srcName = (p == null) ? "(console)" : p.ColoredName;
                
                if (unlocking) {
                    Server.lockdown.Remove(args[1]);
                    Chat.MessageOps("Unlocked by: " + srcName);
                } else {
                    Server.lockdown.AddOrReplace(args[1]);
                    Chat.MessageOps("Locked by: " + srcName);
                }
                Server.lockdown.Save();
            } else {
                Player who = PlayerInfo.FindMatches(p, args[1]);
                if (who == null) return;

                if (!who.jailed) {
                    if (p != null && who.Rank >= p.Rank) {
                        Player.Message(p, "Cannot lock down someone of equal or greater rank."); return;
                    }
                    if (p != null && who.level != p.level) {
                        Player.Message(p, "Moving player to your map...");
                        PlayerActions.ChangeMap(who, p.level);
                        who.BlockUntilLoad(500);
                    }
                    Chat.MessageAll("{0} %Shas been locked down!", who.ColoredName);
                    Chat.MessageOps("Locked by: " + ((p == null) ? "Console" : p.name));
                } else {
                    Chat.MessageAll("{0} %Shas been unlocked.", who.ColoredName);
                    Chat.MessageOps("Unlocked by: " + ((p == null) ? "Console" : p.name));
                }
                who.jailed = !who.jailed;
            }
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/lockdown map/player [name]");
            Player.Message(p, "%H\"map\" - prevents new players from joining that map.");
            Player.Message(p, "%H\"player\" - prevents that player from using commands.");
            Player.Message(p, "%HUsing /lockdown again will unlock that map/player.");
        }
    }
}
