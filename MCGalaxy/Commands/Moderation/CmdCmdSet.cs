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
    public sealed class CmdCmdSet : ItemPermsCmd {
        public override string name { get { return "CmdSet"; } }

        public override void Use(Player p, string message) {
            string[] args = message.SplitSpaces(3);
            if (args.Length < 2) { Help(p); return; }
            
            string cmdName = args[0], cmdArgs = "";
            Command.Search(ref cmdName, ref cmdArgs);
            Command cmd = Command.Find(cmdName);
            
            if (cmd == null) { Player.Message(p, "Could not find command entered"); return; }
            if (p != null && !p.group.CanExecute(cmd)) {
                Player.Message(p, "Your rank cannot use this command."); return;
            }
            
            if (args.Length == 2) {
                CommandPerms perms = CommandPerms.Find(cmd.name);
                SetPerms(p, args, perms);
            } else {
                int idx = 0;
                if (!CommandParser.GetInt(p, args[2], "Extra permission number", ref idx)) return;
                
                CommandExtraPerms perms = CommandExtraPerms.Find(cmd.name, idx);
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
                
                string msg = "extra permission " + idx;
                Announce(p, cmd.name + "%S's " + msg + " was set to " + grp.ColoredName);
                return;
            }
        }
        
        protected override void UpdatePerms(ItemPerms perms, Player p, string msg) {
            CommandPerms.Save();
            CommandPerms.Load();
            
            Announce(p, perms.ItemName + msg);
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
