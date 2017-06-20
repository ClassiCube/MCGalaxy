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
using MCGalaxy.Blocks;
using MCGalaxy.Network;

namespace MCGalaxy.Commands.Moderation {
    public sealed class CmdBlockSet : Command {
        public override string name { get { return "blockset"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        
        public override void Use(Player p, string message) {
            string[] args = message.SplitSpaces();
            if (args.Length == 1) { Help(p); return; }
            
            byte block = Block.Byte(args[0]);
            if (block == Block.Invalid) { Player.Message(p, "Could not find block entered"); return; }
            if (!CommandParser.IsBlockAllowed(p, "change permissions of", (ExtBlock)block)) return;

            if (args.Length == 2 && args[1][0] == '+') {
                Group grp = GetGroup(p, args[1].Substring(1));
                if (grp == null) return;
                BlockPerms perms = BlockPerms.List[block];

                if (perms.Disallowed.Contains(grp.Permission)) {
                    perms.Disallowed.Remove(grp.Permission);
                } else if (!perms.Allowed.Contains(grp.Permission)) {
                    perms.Allowed.Add(grp.Permission);
                }
                
                UpdatePermissions(block, p, " can now be used by " + grp.ColoredName);
            } else if (args.Length == 2 && args[1][0] == '-') {
                Group grp = GetGroup(p, args[1].Substring(1));
                if (grp == null) return;
                BlockPerms perms = BlockPerms.List[block];
                
                if (p != null && p.Rank == grp.Permission) {
                    Player.Message(p, "You cannot disallow your own rank from using a block."); return;
                }
                
                if (perms.Allowed.Contains(grp.Permission)) {
                    perms.Allowed.Remove(grp.Permission);
                } else if (!perms.Disallowed.Contains(grp.Permission)) {
                    perms.Disallowed.Add(grp.Permission);
                }
                
                UpdatePermissions(block, p, " is no longer usable by " + grp.ColoredName);
            } else if (args.Length == 2) {
                Group grp = GetGroup(p, args[1]);
                if (grp == null) return;
                BlockPerms perms = BlockPerms.List[block];
                
                perms.MinRank = grp.Permission;
                UpdatePermissions(block, p, "'s permission was set to " + grp.ColoredName);
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
        
        static void UpdatePermissions(byte block, Player p, string message) {
            BlockPerms.Save();
            BlockPerms.Load();
            if (block < Block.CpeCount) {
                BlockPerms.ResendBlockPermissions(block);
            }
            
            Chat.MessageGlobal("&d{0}%S{1}", Block.Name(block), message);
            if (Player.IsSuper(p))
                Player.Message(p, Block.Name(block) + message);
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/blockset [block] [rank]");
            Player.Message(p, "%HSets lowest rank that can modify/use [block] to [rank]");
            Player.Message(p, "%T/blockset [block] +[rank]");
            Player.Message(p, "%HAllows a specific rank to modify/use [block]");
            Player.Message(p, "%T/blockset [block] -[rank]");
            Player.Message(p, "%HPrevents a specific rank from modifying/using [block]");
            Player.Message(p, "%HTo see available ranks, type %T/viewranks");
        }
    }
}
