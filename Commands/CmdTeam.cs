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
using MCGalaxy.Games;

namespace MCGalaxy.Commands {

    public sealed class CmdTeam : Command {
        
        public override string name { get { return "team"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Games; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        
        public override void Use(Player p, string message) {
            if (message == "") { Help(p); return; }
            string[] args = message.Split(' ');

            switch (args[0].ToLower()) {
                case "owner":
                    HandleOwner(p, args); break;
                case "kick":
                    break;
                case "color":
                    break;
                    
                case "create":
                    break;
                case "join":
                    break;
                case "invite":
                    break;
                case "leave":
                    break;
                case "members":
                    break;
            }
        }

        void HandleOwner(Player p, string[] args) {
            Team team = Team.FindTeam(p);
            if (team == null) { Player.SendMessage(p, "You need to be in a team first."); return; }
            
            if (args.Length == 1) {
                Player.SendMessage(p, "The current owner of the team is: " + team.Owner); return;
            }
            
            Player who = PlayerInfo.FindOrShowMatches(p, args[1]);
            if (who == null) return;
            team.Owner = who.name;
            team.Action(who, "set the team owner to " + who.FullName);
            Team.SaveList();
        }

        void HandleKick(Player p, string[] args) {
            Team team = Team.FindTeam(p);
            if (team == null) { Player.SendMessage(p, "You need to be in a team first."); return; }
            if (args.Length == 1) {
                Player.SendMessage(p, "You need to provide the name of the player to kick."); return;
            }
            if (p.name != team.Owner) {
                Player.SendMessage(p, "Only the team owner can kick players from the team."); return;
            }
            
            Player who = PlayerInfo.FindOrShowMatches(p, args[1]);
            if (who == null) return;
            team.Action(who, "kicked " + args[1] + " from the team.");
            Team.SaveList();
        }
        
        void HandleColor(Player p, string[] args) {
        	Team team = Team.FindTeam(p);
            if (team == null) { Player.SendMessage(p, "You need to be in a team first."); return; }
            if (args.Length == 1) {
                Player.SendMessage(p, "You need to provide the new color."); return;
            }
            string color = Colors.Parse(args[1]);
        }
        
        public override void Help(Player p) {
            //.. team message?
            Player.SendMessage(p, "%T/team owner <name> %H-Sets the player who has owner priveliges for the team.");
            Player.SendMessage(p, "%T/team kick [name] %H-Removes that player from the team you are in.");
            Player.SendMessage(p, "%T/team color [color] %H-Sets the color of the team name shown in chat.");

            Player.SendMessage(p, "%T/team create %H- Creates a new team.");
            Player.SendMessage(p, "%T/team join %H-Joins the team you last received an invite to.");
            Player.SendMessage(p, "%T/team invite [name] %H-Invites that player to join your team.");
            Player.SendMessage(p, "%T/team leave %H-Removes you from the team you are in.");
            Player.SendMessage(p, "%T/team members [name] %H-Lists the players within that team.");
        }
    }
}