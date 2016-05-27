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
namespace MCGalaxy.Commands {
    
    public sealed class CmdCmdSet : Command {
        
        public override string name { get { return "cmdset"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }

        public override void Use(Player p, string message) {
            string[] args = message.Split(' ');
            if (message == "" || args.Length == 1) { Help(p); return; }
            Command cmd = Command.all.Find(args[0]);
            if (cmd == null) { Player.Message(p, "Could not find command entered"); return; }
            if (p != null && !p.group.CanExecute(cmd)) { Player.Message(p, "Your rank cannot use this command."); return; }

            Group grp = Group.Find(args[1]);
            if (grp == null) { Player.Message(p, "Could not find rank specified"); return; }
            if (p != null && grp.Permission > p.group.Permission) { 
                Player.Message(p, "Cannot set permissions to a rank higher than yours."); return; 
            }

            int otherPermIndex = 0;
            if (args.Length == 2) {
                var allowed = GrpCommands.allowedCommands.Find(rA => rA.commandName == cmd.name);
                allowed.lowestRank = grp.Permission;
                UpdatePermissions(cmd, p, "'s permission was set to " + grp.ColoredName);
            } else if (args[2].CaselessEq("allow")) {
                var allowed = GrpCommands.allowedCommands.Find(rA => rA.commandName == cmd.name);
                allowed.disallow.Remove(grp.Permission);
                if (!allowed.allow.Contains(grp.Permission))
                    allowed.allow.Add(grp.Permission);
                UpdatePermissions(cmd, p, " can now be used by " + grp.ColoredName);
            } else if (args[2].CaselessEq("disallow")) {
                if (p != null && p.group.Permission == grp.Permission) {
                    Player.Message(p, "You cannot disallow your own rank from using a command."); return;
                }
                
                var allowed = GrpCommands.allowedCommands.Find(rA => rA.commandName == cmd.name);
                allowed.allow.Remove(grp.Permission);
                if (!allowed.disallow.Contains(grp.Permission))
                    allowed.disallow.Add(grp.Permission);
                UpdatePermissions(cmd, p, " is no longer usable by " + grp.ColoredName);
            } else if (!int.TryParse(args[2], out otherPermIndex)) {
                Player.Message(p, "\"{0}\" must be \"allow\", \"disallow\", or an integer.", args[2]);
            } else {
                CommandOtherPerms.OtherPerms perms = CommandOtherPerms.Find(cmd, otherPermIndex);
                if (perms == null) {
                    Player.Message(p, "This command has no additional permission with that number."); return;
                }
                
                perms.Permission = (int)grp.Permission;
                CommandOtherPerms.Save();
                string permName = "additional permission " + otherPermIndex;
                Player.GlobalMessage("&d" + cmd.name + "%S's " + permName + " was set to " + grp.ColoredName);
                Player.Message(p, cmd.name + "'s " + permName + " was set to " + grp.ColoredName);
            }
        }
        
        static void UpdatePermissions(Command cmd, Player p, string message) {
             GrpCommands.Save(GrpCommands.allowedCommands);
             GrpCommands.fillRanks();
             Player.GlobalMessage("&d" + cmd.name + "%S" + message);
             Player.Message(p, cmd.name + message);
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/cmdset [cmd] [rank]");
            Player.Message(p, "%HSets lowest rank that can use [cmd] to [rank]");
            Player.Message(p, "%T/cmdset [cmd] [rank] allow");
            Player.Message(p, "%HAllows a specific rank to use [cmd]");
            Player.Message(p, "%T/cmdset [cmd] [rank] disallow");
            Player.Message(p, "%HPrevents a specific rank from using [cmd]");
            Player.Message(p, "%T/cmdset [cmd] [rank] <additional permission number>");
            Player.Message(p, "%HSet the lowest rank that has that additional permission for [cmd] " +
                           "(Most commands do not use these)");
            Player.Message(p, "To see available ranks, type %T/viewranks");
        }
    }
}
