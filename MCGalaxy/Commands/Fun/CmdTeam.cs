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

namespace MCGalaxy.Commands.Fun {
    public sealed class CmdTeam : Command2 {        
        public override string name { get { return "Team"; } }
        public override string type { get { return CommandTypes.Games; } }
        public override bool SuperUseable { get { return false; } }
        public override CommandPerm[] ExtraPerms {
            get { return new[] { new CommandPerm(LevelPermission.AdvBuilder, "can create teams") }; }
        }
        
        public override void Use(Player p, string message, CommandData data) {
            if (message.Length == 0) { Help(p); return; }
            string[] args = message.SplitSpaces(2);

            switch (args[0].ToLower()) {
                case "owner": HandleOwner(p, args); return;
                case "kick": HandleKick(p, args); return;
                case "color": HandleColor(p, args); return;
                case "create": HandleCreate(p, args, data); return;
                case "join": HandleJoin(p, args); return;
                case "invite": HandleInvite(p, args); return;
                case "leave": HandleLeave(p, args); return;
                case "members": HandleMembers(p, args); return;
                case "list": HandleList(p, args); return;
            }
            
            Team team = p.Game.Team;
            if (team == null) {
                p.Message("You need to be in a team first to send a team message."); return;
            }
            team.Message(p, message);
        }

        void HandleOwner(Player p, string[] args) {
            Team team = p.Game.Team;
            if (team == null) { p.Message("You need to be in a team first."); return; }
            
            if (args.Length == 1) {
                p.Message("The current owner of the team is: " + team.Owner); return;
            }
            
            Player who = PlayerInfo.FindMatches(p, args[1]);
            if (who == null) return;
            
            if (!p.name.CaselessEq(team.Owner)) {
                p.Message("Only the team owner can set a new team owner."); return;
            }
            team.Owner = who.name;
            team.Action(p, "set the team owner to " + who.ColoredName);
            Team.SaveList();
        }

        void HandleKick(Player p, string[] args) {
            Team team = p.Game.Team;
            if (team == null) { p.Message("You need to be in a team first."); return; }
            if (args.Length == 1) {
                p.Message("You need to provide the name of the player to kick."); return;
            }
            if (!p.name.CaselessEq(team.Owner)) {
                p.Message("Only the team owner can kick players from the team."); return;
            }
            
            if (team.Remove(args[1])) {
                team.Action(p, "kicked " + args[1] + " from the team.");
                Player who = PlayerInfo.FindExact(args[1]);
                if (who != null) {
                    who.Game.Team = null;
                    who.SetPrefix();
                }
                
                team.DeleteIfEmpty();
                Team.SaveList();
            } else {
                p.Message("The given player was not found. You need to use their full account name.");
            }
        }
        
        void HandleColor(Player p, string[] args) {
            Team team = p.Game.Team;
            if (team == null) { p.Message("You need to be in a team first."); return; }
            if (args.Length == 1) {
                p.Message("You need to provide the new color."); return;
            }
            
            string color = Matcher.FindColor(p, args[1]);
            if (color == null) return;
            
            team.Color = color;
            team.Action(p, "changed the team color to: " + args[1]);
            team.UpdatePrefix();
            Team.SaveList();
        }
        
        void HandleCreate(Player p, string[] args, CommandData data) {
            if (!CheckExtraPerm(p, data, 1)) return;
            Team team = p.Game.Team;
            if (team != null) { p.Message("You need to leave your current team before you can create one."); return; }
            if (args.Length == 1) {
                p.Message("You need to provide the name of the new team."); return;
            }
            team = Team.Find(args[1]);
            if (team != null) { p.Message("There is already an existing team with that name."); return; }
            if (args[1].Length > 8) {
                p.Message("Team names must be 8 characters or less."); return;
            }
            
            team = new Team(args[1], p.name);
            p.Game.Team = team;
            p.SetPrefix();
            Team.Add(team);
            Team.SaveList();
            Chat.MessageFrom(p, "λNICK &Screated the &a" + args[1] + " &Steam");
        }
        
        void HandleJoin(Player p, string[] args) {
            Team team = p.Game.Team;
            if (p.Game.TeamInvite == null) { p.Message("You do not currently have any invitation to join a team."); return; }
            if (team != null) { p.Message("You need to leave your current team before you can join another one."); return; }
            
            team = Team.Find(p.Game.TeamInvite);
            if (team == null) { p.Message("The team you were invited to no longer exists."); return; }
            
            p.Game.Team = team;
            p.Game.TeamInvite = null;
            p.SetPrefix();
            
            team.Members.Add(p.name);
            team.Action(p, "joined the team.");
            Team.SaveList();
        }
        
        void HandleInvite(Player p, string[] args) {
            Team team = p.Game.Team;
            if (team == null) { p.Message("You need to be in a team first to invite players."); return; }
            if (args.Length == 1) {
                p.Message("You need to provide the name of the person to invite."); return;
            }
            Player target = PlayerInfo.FindMatches(p, args[1]);
            if (target == null) return;            
            
            DateTime cooldown = p.NextTeamInvite;
            DateTime now = DateTime.UtcNow;
            if (now < cooldown) {
                p.Message("You can invite a player to join your team in another {0} seconds",
                               (int)(cooldown - now).TotalSeconds);
                return;
            }
            p.NextTeamInvite = now.AddSeconds(5);
            
            p.Message("Invited {0} &Sto join your team.", p.FormatNick(target));
            target.Message(p.ColoredName + " &Sinvited you to join the " + team.Color + team.Name + " &Steam.");
            target.Game.TeamInvite = team.Name;
        }
        
        void HandleLeave(Player p, string[] args) {
            Team team = p.Game.Team;
            if (team == null) { p.Message("You need to be in a team first to leave one."); return; }
            
            // handle '/team leave me alone', for example
            if (args.Length > 1) {
                team.Message(p, args.Join(" ")); return;
            }
            
            team.Action(p, "left the team.");
            team.Remove(p.name);
            p.Game.Team = null;
            
            team.DeleteIfEmpty();
            p.SetPrefix();
            Team.SaveList();
        }
        
        void HandleMembers(Player p, string[] args) {
            Team team = p.Game.Team;
            if (args.Length == 1) {
                if (team == null) { p.Message("You are not in a team, so must provide a team name."); return; }
            } else {
                team = Team.Find(args[1]);
                if (team == null) { p.Message("No team found with the name \"" + args[1] + "\"."); return; }
            }
            p.Message("Team owner: " + team.Owner);
            p.Message("Members: " + team.Members.Join());
        }
        
        void HandleList(Player p, string[] args) {
            string modifier = args.Length > 1 ? args[1] : "";
            MultiPageOutput.Output(p, Team.Teams, team => team.Color + team.Name,
                                   "team list", "teams", modifier, false);
        }
        
        public override void Help(Player p) {
            p.Message("&T/Team owner [name] &H- Sets the player who has owner privileges for the team.");
            p.Message("&T/Team kick [name] &H- Removes that player from the team you are in.");
            p.Message("&T/Team color [color] &H- Sets the color of the team name shown in chat.");
            p.Message("&T/Team create &H- Creates a new team.");
            p.Message("&T/Team join &H- Joins the team you last received an invite to.");
            p.Message("&T/Team invite [name] &H- Invites that player to join your team.");
            p.Message("&T/Team leave &H- Removes you from the team you are in.");
            p.Message("&T/Team members [name] &H- Lists the players within that team.");
            p.Message("&T/Team list &H- Lists all teams.");
            p.Message("&HAnything else is sent as a message to all members of the team.");
        }
    }
}