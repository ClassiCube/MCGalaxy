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
using System.IO;
namespace MCGalaxy.Commands
{
    public sealed class CmdRules : Command
    {
        public override string name { get { return "rules"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public override CommandPerm[] ExtraPerms {
            get { return new[] { new CommandPerm(LevelPermission.Builder, "can send rules to other players") }; }
        }

        public override void Use(Player p, string message) {
            if (!File.Exists("text/rules.txt")) {
                CP437Writer.WriteAllText("text/rules.txt", "No rules entered yet!");
            }
            List<string> rules = CP437Reader.ReadAllLines("text/rules.txt");

            Player who = p;
            if (message != "") {
            	if (!CheckExtraPerm(p)) { MessageNeedExtra(p, "can send the rules to a player."); return; }
                who = PlayerInfo.FindMatches(p, message);
                if (who == null) return;
            }
            
            if (who != null) {
                who.hasreadrules = true;
                if (who.level == Server.mainLevel && Server.mainLevel.permissionbuild == LevelPermission.Guest)
                    Player.Message(who, "You are currently on the guest map where anyone can build");
            }
            Player.Message(who, "Server Rules:");
            foreach (string s in rules)
                Player.Message(who, s);
            
            if (who != null && who.name != p.name) {
                Player.Message(p, "Sent the rules to " + who.ColoredName + "%S.");
                Player.Message(who, p.ColoredName + " %Ssent you the rules.");
            }
        }

        public override void Help(Player p) {
        	if (CheckExtraPerm(p)) {
                Player.Message(p, "%T/rules [player]");
                Player.Message(p, "%HDisplays server rules to [player].");
                Player.Message(p, "%HIf no [player] is given, the rules will be sent to you.");
            } else {
                Player.Message(p, "%T/rules");
                Player.Message(p, "%HDisplays the server rules.");
            }
        }
    }
}
