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

            LevelPermission perm = Level.PermissionFromName(args[1]);
            if (perm == LevelPermission.Null) { Player.Message(p, "Could not find rank specified"); return; }
            if (p != null && perm > p.group.Permission) { Player.Message(p, "Cannot set permissions to a rank higher than yours."); return; }

            int otherPermIndex = 0;
            string permName = "permission";
            if (args.Length == 2) {
                GrpCommands.allowedCommands.Find(rA => rA.commandName == cmd.name).lowestRank = perm;
                GrpCommands.Save(GrpCommands.allowedCommands);
                GrpCommands.fillRanks();
            } else if (!int.TryParse(args[2], out otherPermIndex)) {
                Player.Message(p, "\"" + args[2] + "\" is not an integer.");
            } else {
                CommandOtherPerms.OtherPerms perms = CommandOtherPerms.Find(cmd, otherPermIndex);
                if (perms == null) {
                    Player.Message(p, "This command has no additional permission with that number."); return;
                }
                perms.Permission = (int)perm;
                CommandOtherPerms.Save();
                permName = "additional permission " + otherPermIndex;
            }
            Player.GlobalMessage("&d" + cmd.name + "%S's " + permName + " was set to " + Level.PermissionToName(perm));
            Player.Message(p, cmd.name + "'s " + permName + " was set to " + Level.PermissionToName(perm));
        }
        
        public override void Help(Player p) {
            Player.Message(p, "/cmdset [cmd] [rank] <otherperm> - Changes [cmd] rank to [rank]");
            Player.Message(p, "Only commands you can use can be modified.");
            Player.Message(p, "<otherperm> is optional and is used to set the additional " +
                               "permissions for several commands. Most commands do not use this.");
            Player.Message(p, "Available ranks: " + Group.concatList());
        }
    }
}
