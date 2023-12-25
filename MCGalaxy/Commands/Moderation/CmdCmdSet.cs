/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCForge)
    
    Dual-licensed under the    Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    https://opensource.org/license/ecl-2-0/
    https://www.gnu.org/licenses/gpl-3.0.html
    
    Unless required by applicable law or agreed to in writing,
    software distributed under the Licenses are distributed on an "AS IS"
    BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
    or implied. See the Licenses for the specific language governing
    permissions and limitations under the Licenses.
 */
namespace MCGalaxy.Commands.Moderation 
{
    public sealed class CmdCmdSet : ItemPermsCmd 
    {
        public override string name { get { return "CmdSet"; } }

        public override void Use(Player p, string message, CommandData data) {
            string[] args = message.SplitSpaces(3);
            if (args.Length < 2) { Help(p); return; }
            
            string cmdName = args[0], cmdArgs = "", msg;
            Command.Search(ref cmdName, ref cmdArgs);
            Command cmd = Command.Find(cmdName);
            
            if (cmd == null) { p.Message("Could not find command entered"); return; }
            
            if (!p.CanUse(cmd)) {
                cmd.Permissions.MessageCannotUse(p);
                p.Message("Therefore you cannot change the permissions of &T/{0}", cmd.name); return;
            }
            
            if (args.Length == 2) {
                msg = SetPerms(p, args, data, cmd.Permissions, "command", "use", "usable");
                if (msg != null) 
                    UpdateCommandPerms(cmd.Permissions, p, msg);
            } else {
                
                int num = 0;
                if (!CommandParser.GetInt(p, args[2], "Extra permission number", ref num)) return;
                
                CommandExtraPerms perms = CommandExtraPerms.Find(cmd.name, num);
                if (perms == null) {
                    p.Message("This command has no extra permission by that number."); return;
                }
                
                msg = SetPerms(p, args, data, perms, "extra permission", "use", "usable");
                if (msg != null) 
                    UpdateExtraPerms(perms, p, msg);
            }
        }
        
        void UpdateCommandPerms(ItemPerms perms, Player p, string msg) {
            CommandPerms.Save();
            CommandPerms.ApplyChanges();
            Announce(p, perms.ItemName + msg);
        }
        
        void UpdateExtraPerms(CommandExtraPerms perms, Player p, string msg) {
            CommandExtraPerms.Save();
            //Announce(p, cmd.name + "&S's extra permission " + idx + " was set to " + grp.ColoredName);
            Announce(p, perms.CmdName + " extra permission #" + perms.Num + msg);
        }
        
        public override void Help(Player p) {
            p.Message("&T/CmdSet [cmd] [rank]");
            p.Message("&HSets lowest rank that can use [cmd] to [rank]");
            p.Message("&T/CmdSet [cmd] [rank] [extra permission number]");
            p.Message("&HSet the lowest rank that has that extra permission for [cmd]");
            p.Message("&H- For more advanced permissions, see &T/Help cmdset advanced");
            p.Message("&H- To see available ranks, type &T/ViewRanks");
        }
        
        public override void Help(Player p, string message) {
            if (!message.CaselessEq("advanced")) { base.Help(p, message); return; }
            
            p.Message("&T/CmdSet [cmd] +[rank]");
            p.Message("&HAllows a specific rank to use [cmd]");
            p.Message("&T/CmdSet [cmd] -[rank]");
            p.Message("&HPrevents a specific rank from using [cmd]");
        }
    }
}
