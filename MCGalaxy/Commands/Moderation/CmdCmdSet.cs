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
namespace MCGalaxy.Commands.Moderation {
    public sealed class CmdCmdSet : Command {        
        public override string name { get { return "CmdSet"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }

        public override void Use(Player p, string message) {
            string[] args = message.SplitSpaces();
            if (args.Length == 1) { Help(p); return; }
            
            Command cmd = Command.all.Find(args[0]);
            if (cmd == null) { Player.Message(p, "Could not find command entered"); return; }
            if (p != null && !p.group.CanExecute(cmd)) {
                Player.Message(p, "Your rank cannot use this command."); return;
            }

            if (args.Length == 2 && args[1][0] == '+') {
                Group grp = GetGroup(p, args[1].Substring(1));
                if (grp == null) return;
                CommandPerms perms = CommandPerms.Find(cmd.name);

                if (perms.Disallowed.Contains(grp.Permission)) {
                    perms.Disallowed.Remove(grp.Permission);
                } else if (!perms.Allowed.Contains(grp.Permission)) {
                    perms.Allowed.Add(grp.Permission);
                }
                
                UpdatePermissions(cmd, p, " can now be used by " + grp.ColoredName);
            } else if (args.Length == 2 && args[1][0] == '-') {
                Group grp = GetGroup(p, args[1].Substring(1));
                if (grp == null) return;
                CommandPerms perms = CommandPerms.Find(cmd.name);
                
                if (p != null && p.Rank == grp.Permission) {
                    Player.Message(p, "You cannot disallow your own rank from using a command."); return;
                }
                
                if (perms.Allowed.Contains(grp.Permission)) {
                    perms.Allowed.Remove(grp.Permission);
                } else if (!perms.Disallowed.Contains(grp.Permission)) {
                    perms.Disallowed.Add(grp.Permission);
                }
                
                UpdatePermissions(cmd, p, " is no longer usable by " + grp.ColoredName);
            } else if (args.Length == 2) {
                Group grp = GetGroup(p, args[1]);
                if (grp == null) return;
                CommandPerms perms = CommandPerms.Find(cmd.name);
                
                perms.MinRank = grp.Permission;
                UpdatePermissions(cmd, p, "'s permission was set to " + grp.ColoredName);
            } else {
                int otherPermIndex = 0;
                if (!CommandParser.GetInt(p, args[2], "Extra permission number", ref otherPermIndex)) return;
                
                CommandExtraPerms perms = CommandExtraPerms.Find(cmd.name, otherPermIndex);
                if (perms == null) {
                    Player.Message(p, "This command has no extra permission by that number."); return;
                }
                if (p != null && p.Rank < perms.MinRank) {
                    Player.Message(p, "Your rank cannot modify this extra permission."); return;
                }
                
                Group grp = GetGroup(p, args[1]);
                if (grp == null) return;
                perms.MinRank = grp.Permission;
                CommandExtraPerms.Save();
                
                string permName = "extra permission " + otherPermIndex;
                Chat.MessageGlobal("&d{0}%S's {1} was set to {2}", cmd.name, permName, grp.ColoredName);
                if (Player.IsSuper(p))
                    Player.Message(p, "{0}'s {1} was set to {2}", cmd.name, permName, grp.ColoredName);
            }
        }
        
        static Group GetGroup(Player p, string grpName) {
            Group grp = Matcher.FindRanks(p, grpName);
            if (grp == null) return null;
            
            if (p != null && grp.Permission > p.Rank) {
                Player.Message(p, "Cannot set permissions to a rank higher than yours."); return null;
            }
            return grp;
        }
        
        static void UpdatePermissions(Command cmd, Player p, string message) {
            CommandPerms.Save();
            CommandPerms.Load();
            
            Chat.MessageGlobal("&d{0}%S{1}", cmd.name, message);
            if (Player.IsSuper(p))
                Player.Message(p, cmd.name + message);
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/CmdSet [cmd] [rank]");
            Player.Message(p, "%HSets lowest rank that can use [cmd] to [rank]");
            Player.Message(p, "%T/CmdSet [cmd] +[rank]");
            Player.Message(p, "%HAllows a specific rank to use [cmd]");
            Player.Message(p, "%T/CmdSet [cmd] -[rank]");
            Player.Message(p, "%HPrevents a specific rank from using [cmd]");
            Player.Message(p, "%T/CmdSet [cmd] [rank] [extra permission number]");
            Player.Message(p, "%HSet the lowest rank that has that extra permission for [cmd]");
            Player.Message(p, "%HTo see available ranks, type %T/ViewRanks");
        }
    }
}
