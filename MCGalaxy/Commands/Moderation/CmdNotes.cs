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

namespace MCGalaxy.Commands {    
    public class CmdNotes : Command {        
        public override string name { get { return "notes"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }

        public override void Use(Player p, string message) {
        	if (!Server.LogNotes) {
        		Player.Message(p, "The server does not have notes logging enabled."); return;
        	}
            if (CheckSuper(p, message, "player name")) return;
            if (message == "") message = p.name;
            
            int matches = 1;
            Player who = message == "" ? p : PlayerInfo.FindMatches(p, message, out matches);
            if (matches > 1) return;
            if (who != null) message = who.name;
            
            Player.Message(p, "Notes for " + message + ":");
            bool foundAny = false;
            foreach (string line in Server.Notes.Find(message)) {
                foundAny = true;
                string[] args = line.Split(' ');
                if (args.Length <= 3) continue;
                
                if (args.Length == 4)
                	Player.Message(p, Action(args[1]) + " by " + args[2] + " on " + args[3]);
                else
                    Player.Message(p, Action(args[1]) + " by " + args[2] + " on " + args[3]
                	                   + " - " + args[4].Replace("%20", " "));
            }
            if (!foundAny)
                Player.Message(p, "No notes found.");
        }
        
        static string Action(string arg) {
            if (arg.CaselessEq("W")) return "Warned";
            if (arg.CaselessEq("K")) return "Kicked";
            if (arg.CaselessEq("M")) return "Muted";
            if (arg.CaselessEq("B")) return "Banned";
            if (arg.CaselessEq("J")) return "Jailed";
            if (arg.CaselessEq("F")) return "Frozen";
            if (arg.CaselessEq("T")) return "Temp-Banned";
            return arg;
        }

        public override void Help(Player p) {
           Player.Message(p, "%T/notes [name] %H- views that player's notes.");
           Player.Message(p, "%HNotes are things such as bans, kicks, warns, mutes.");
        }
    }
    
    public sealed class CmdMyNotes : CmdNotes {        
        public override string name { get { return "mynotes"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }

        public override void Use(Player p, string message) {
            if (Player.IsSuper(p)) { MessageInGameOnly(p); return; }
            base.Use(p, p.name);
        }

        public override void Help(Player p) {
            Player.Message(p, "%T/mynotes %H- views your own notes.");
            Player.Message(p, "%HNotes are things such as bans, kicks, warns, mutes.");
        }
    }
}
