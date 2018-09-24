/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
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
using System.Collections.Generic;
using MCGalaxy.Blocks;
using MCGalaxy.Commands.World;
using BlockID = System.UInt16;

namespace MCGalaxy.Commands.Info {
    public sealed class CmdBlocks : Command2 {
        public override string name { get { return "Blocks"; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool UseableWhenFrozen { get { return true; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("Materials") }; }
        }

        public override void Use(Player p, string message, CommandData data) {
            string[] args = message.SplitSpaces();
            string modifier = args.Length > 1 ? args[1] : "";            
            string type = args[0];
            BlockID block;
            
            if (type.Length == 0 || type.CaselessEq("basic")) {
                p.Message("Basic blocks: ");
                MultiPageOutput.Output(p, BasicBlocks(), 
                                       b => FormatBlockName(p, b),
                                       "Blocks basic", "blocks", modifier, false);
            } else if (type.CaselessEq("all") || type.CaselessEq("complex")) {
                p.Message("Complex blocks: ");
                MultiPageOutput.Output(p, ComplexBlocks(), 
                                       b => FormatBlockName(p, b),
                                       "Blocks complex", "blocks", modifier, false);
            } else if ((block = Block.Parse(p, type)) != Block.Invalid) {
                OutputBlockInfo(p, block);
            } else if (Group.Find(type) != null) {
                Group grp = Group.Find(type);
                p.Message("Blocks which {0} %Scan place: ", grp.ColoredName);
                MultiPageOutput.Output(p, RankBlocks(grp.Permission), 
                                       b => FormatBlockName(p, b),
                                       "Blocks " + type, "blocks", modifier, false);
            } else if (args.Length > 1) {
                Help(p);
            } else {
                p.Message("Unable to find block or rank");
            }
        }
        
        static List<BlockID> BasicBlocks() {
            List<BlockID> blocks = new List<BlockID>(Block.CpeCount);
            for (BlockID block = Block.Air; block < Block.CpeCount; block++) { 
                blocks.Add(block); 
            }
            return blocks;
        }
        
        static List<BlockID> ComplexBlocks() {
            List<BlockID> blocks = new List<BlockID>(Block.Count);
            for (BlockID block = Block.CpeCount; block < Block.Count; block++) {
                if (Block.ExistsGlobal(block)) blocks.Add(block);
            }
            return blocks;
        }
        
        static List<BlockID> RankBlocks(LevelPermission perm) {
            List<BlockID> blocks = new List<BlockID>(Block.Count);
            foreach (BlockPerms perms in BlockPerms.List) {
                if (!perms.UsableBy(perm)) continue;
                if (!Block.ExistsGlobal(perms.ID)) continue;
                blocks.Add(perms.ID);
            }
            return blocks;
        }
        
        internal static string FormatBlockName(Player p, BlockID block) {
            BlockPerms perms = BlockPerms.Find(block);
            return Group.GetColor(perms.MinRank) + Block.GetName(p, block);
        }
        
        static void OutputBlockInfo(Player p, BlockID block) {
            string name = Block.GetName(p, block);
            BlockProps[] scope = p.IsSuper ? Block.Props : p.level.Props;
            CmdBlockProperties.Detail(p, scope, block);
            
            if (Block.IsPhysicsType(block)) {
                p.Message("&bComplex information for \"{0}\":", name);
                OutputPhysicsInfo(p, scope, block); return;
            }
            
            string msg = "";
            for (BlockID b = Block.CpeCount; b < Block.Count; b++) {
                if (Block.Convert(b) != block) continue;
                msg += FormatBlockName(p, b) + ", ";
            }

            if (msg.Length > 0) {
                p.Message("Blocks which look like \"{0}\":", name);
                p.Message(msg.Remove(msg.Length - 2));
            } else {
                p.Message("No complex blocks look like \"{0}\"", name);
            }
        }
        
        static void OutputPhysicsInfo(Player p, BlockProps[] scope, BlockID b) {
            BlockProps props = scope[b];
            BlockID conv = Block.Convert(b);
            p.Message("&c  Appears as a \"{0}\" block", Block.GetName(p, conv));

            if (Block.LightPass(b))   p.Message("  Allows light through");
            if (Block.NeedRestart(b)) p.Message("  The block's physics will auto-start");
            
            if (Physics(scope, b)) {
                p.Message("  Affects physics in some way"); //AFFECT!
            } else {
                p.Message("  Does not affect physics in any way"); //It's AFFECT!
            }

            if (Block.AllowBreak(b))     p.Message("  Anybody can activate this block");
            if (Block.Walkthrough(conv)) p.Message("  Can be walked through");
            if (Mover(scope, conv))      p.Message("  Can be activated by walking through it");
        }
        
        static bool Mover(BlockProps[] scope, BlockID conv) {
            bool nonSolid = Block.Walkthrough(conv);
            return BlockBehaviour.GetWalkthroughHandler(conv, scope, nonSolid) != null;
        }
        
        static bool Physics(BlockProps[] scope, BlockID b) {
            if (scope[b].IsMessageBlock || scope[b].IsPortal) return false;
            if (scope[b].IsDoor || scope[b].IsTDoor) return false;
            if (scope[b].OPBlock) return false;
            
            return BlockBehaviour.GetPhysicsHandler(b, Block.Props) != null;
        }
        
        public override void Help(Player p) {
            p.Message("%T/Blocks %H- Lists all basic blocks");
            p.Message("%T/Blocks complex %H- Lists all complex blocks");
            p.Message("%T/Blocks [block] %H- Lists information about that block");
            p.Message("%T/Blocks [rank] %H- Lists all blocks [rank] can use");
            p.Message("%HTo see available ranks, type %T/ViewRanks");
        }
    }
}
