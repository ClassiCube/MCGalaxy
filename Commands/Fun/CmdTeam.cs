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
            if (p == null) { MessageInGameOnly(p); return; }
            if (message == "") { Help(p); return; }
            string[] args = message.Split(' ');

            switch (args[0].ToLower()) {
                case "owner":
                    HandleOwner(p, args); break;
                case "kick":
                    HandleKick(p, args); break;
                case "color":
                    HandleColor(p, args); break;
                case "create":
                    HandleCreate(p, args); break;
                case "join":
                    HandleJoin(p, args); break;
                case "invite":
                    HandleInvite(p, args); break;
                case "leave":
                    HandleLeave(p, args); break;
                case "members":
                    HandleMembers(p, args); break;
                default:
                    Team team = p.Game.Team;
                    if (team == null) { 
                        Player.Message(p, "You need to be in a team first to send a team message."); return; 
                    }
                    team.Chat(p, message); break;
            }
        }

        void HandleOwner(Player p, string[] args) {
            Team team = p.Game.Team;
            if (team == null) { Player.Message(p, "You need to be in a team first."); return; }
            
            if (args.Length == 1) {
                Player.Message(p, "The current owner of the team is: " + team.Owner); return;
            }
            
            Player who = PlayerInfo.FindOrShowMatches(p, args[1]);
            if (who == null) return;
            team.Owner = who.name;
            team.Action(who, "set the team owner to " + who.ColoredName);
            Team.SaveList();
        }

        void HandleKick(Player p, string[] args) {
            Team team = p.Game.Team;
            if (team == null) { Player.Message(p, "You need to be in a team first."); return; }
            if (args.Length == 1) {
                Player.Message(p, "You need to provide the name of the player to kick."); return;
            }
            if (p.name != team.Owner) {
                Player.Message(p, "Only the team owner can kick players from the team."); return;
            }
            
            if (team.Remove(args[1])) {
                team.Action(p, "kicked " + args[1] + " from the team.");
                Player who = PlayerInfo.FindExact(args[1]);
                if (who != null) {
                    who.Game.Team = null;
                    who.SetPrefix();
                }
                Team.SaveList();
            } else {
                Player.Message(p, "The given player was not found. You need to use their full account name.");
            }
        }
        
        void HandleColor(Player p, string[] args) {
            Team team = p.Game.Team;
            if (team == null) { Player.Message(p, "You need to be in a team first."); return; }
            if (args.Length == 1) {
                Player.Message(p, "You need to provide the new color."); return;
            }
            
            string color = Colors.Parse(args[1]);
            if (color == "") {
                Player.Message(p, "\"" + color + "\" is not a valid color."); return;
            }
            team.Color = color;
            team.Action(p, "changed the team color to: " + args[1]);
            team.UpdatePrefix();
            Team.SaveList();
        }
        
        void HandleCreate(Player p, string[] args) {
            Team team = p.Game.Team;
            if (team != null) { Player.Message(p, "You need to leave your current team before you can create one."); return; }
            if (args.Length == 1) {
                Player.Message(p, "You need to provide the name of the new team."); return;
            }
            team = Team.FindTeam(args[1]);
            if (team != null) { Player.Message(p, "There is already an existing team with that name."); return; }
            if (args[1].Length > 8) {
                Player.SendMessage(p, "Team names must be 8 characters or less."); return;
            }
            
            team = new Team(args[1], p.name);
            p.Game.Team = team;
            p.SetPrefix();
            Team.TeamsList[team.Name] = team;
            Team.SaveList();
            Player.GlobalMessage(p.ColoredName + " %Sjust created the &a" + args[1] + " %Steam.");
        }
        
        void HandleJoin(Player p, string[] args) {
            Team team = p.Game.Team;
            if (p.Game.TeamInvite == null) { Player.Message(p, "You do not currently have any invitation to join a team."); return; }
            if (team != null) { Player.Message(p, "You need to leave your current team before you can join another one."); return; }
            
            team = Team.FindTeam(p.Game.TeamInvite);
            if (team == null) { Player.Message(p, "The team you were invited to no longer exists."); return; }
            team.Members.Add(p.name);
            team.Action(p, "joined the team.");
            p.Game.Team = team;
            p.SetPrefix();
            Team.SaveList();
        }
        
        void HandleInvite(Player p, string[] args) {
            Team team = p.Game.Team;
            if (team == null) { Player.Message(p, "You need to be in a team first to invite players."); return; }
            if (args.Length == 1) {
                Player.Message(p, "You need to provide the name of the person to invite."); return;
            }
            Player who = PlayerInfo.FindOrShowMatches(p, args[1]);
            if (who == null) return;
            
            Player.Message(p, "Invited " + who.ColoredName + " %Sto join your team.");
            Player.Message(who, p.ColoredName + " %Sinvited you to join the " + team.Color + team.Name + " %Steam.");
            who.Game.TeamInvite = team.Name;
        }
        
        void HandleLeave(Player p, string[] args) {
            Team team = p.Game.Team;
            if (team == null) { Player.Message(p, "You need to be in a team first to leave one."); return; }
            
            team.Action(p, "left the team.");
            team.Remove(p.name);
            p.Game.Team = null;
            p.SetPrefix();
            Team.SaveList();
        }
        
        void HandleMembers(Player p, string[] args) {
            Team team = p.Game.Team;
            if (args.Length == 1) {
                if (team == null) { Player.Message(p, "You are not in a team, so must provide a team name."); return; }
            } else {
                team = Team.FindTeam(args[1]);
                if (team == null) { Player.Message(p, "No team found with the name \"" + args[1] + "\"."); return; }
            }
            Player.Message(p, "Team owner: " + team.Owner);
            Player.Message(p, "Members: " + String.Join(", ", team.Members));
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/team owner <name> %H-Sets the player who has owner privileges for the team.");
            Player.Message(p, "%T/team kick [name] %H-Removes that player from the team you are in.");
            Player.Message(p, "%T/team color [color] %H-Sets the color of the team name shown in chat.");

            Player.Message(p, "%T/team create %H- Creates a new team.");
            Player.Message(p, "%T/team join %H-Joins the team you last received an invite to.");
            Player.Message(p, "%T/team invite [name] %H-Invites that player to join your team.");
            Player.Message(p, "%T/team leave %H-Removes you from the team you are in.");
            Player.Message(p, "%T/team members [name] %H-Lists the players within that team.");
            Player.Message(p, "%HAnything else is sent as a message to all members of the team.");
        }
    }
}