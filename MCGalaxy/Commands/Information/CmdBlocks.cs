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
using BlockID = System.UInt16;

namespace MCGalaxy.Commands.Info {
    public sealed class CmdBlocks : Command {
        public override string name { get { return "Blocks"; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool UseableWhenFrozen { get { return true; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("Materials") }; }
        }

        public override void Use(Player p, string message) {
            string[] args = message.SplitSpaces();
            string modifier = args.Length > 1 ? args[1] : "";            
            string type = args[0];
            
            if (type.Length == 0 || type.CaselessEq("basic")) {
                Player.Message(p, "Basic blocks: ");
                MultiPageOutput.Output(p, BasicBlocks(), 
                                       b => FormatBlockName(p, b),
                                       "Blocks basic", "blocks", modifier, false);
            } else if (type.CaselessEq("all") || type.CaselessEq("complex")) {
                Player.Message(p, "Complex blocks: ");
                MultiPageOutput.Output(p, ComplexBlocks(), 
                                       b => FormatBlockName(p, b),
                                       "Blocks complex", "blocks", modifier, false);
            } else if (Block.Parse(p, type) != Block.Invalid) {
                OutputBlockData(p, type);
            } else if (Group.Find(type) != null) {
                Group grp = Group.Find(type);
                Player.Message(p, "Blocks which {0} %Scan place: ", grp.ColoredName);
                MultiPageOutput.Output(p, RankBlocks(grp.Permission), 
                                       b => FormatBlockName(p, b),
                                       "Blocks " + type, "blocks", modifier, false);
            } else if (args.Length > 1) {
                Help(p);
            } else {
                Player.Message(p, "Unable to find block or rank");
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
        
        static void OutputBlockData(Player p, string blockName) {
            BlockID block = Block.Parse(p, blockName);
            if (Block.IsPhysicsType(block)) {
                Player.Message(p, "&bComplex information for \"{0}\":", blockName);
                Player.Message(p, "&cBlock will appear as a \"{0}\" block", Block.GetName(p, Block.Convert(block)));
                OutputBlockProps(p, block);
                return;
            }
            
            string msg = "";
            for (BlockID b = Block.CpeCount; b < Block.Count; b++) {
                if (Block.Convert(b) != block) continue;
                msg += Block.GetName(p, b) + ", ";
            }

            if (msg.Length > 0) {
                Player.Message(p, "Blocks which look like \"{0}\":", blockName);
                Player.Message(p, msg.Remove(msg.Length - 2));
            } else {
                Player.Message(p, "No Complex Blocks look like \"{0}\"", blockName);
            }
        }
        
        static void OutputBlockProps(Player p, BlockID b) {
            BlockProps[] scope = Player.IsSuper(p) ? Block.Props : p.level.Props;
            BlockProps props = scope[b];

            if (Block.LightPass(b))
                Player.Message(p, "Block will allow light through");
            if (Physics(scope, b))
                Player.Message(p, "Block affects physics in some way"); //AFFECT!
            else
                Player.Message(p, "Block will not affect physics in any way"); //It's AFFECT!
            if (Block.NeedRestart(b)) Player.Message(p, "The block's physics will auto-start");

            if (props.OPBlock) Player.Message(p, "Block is unaffected by explosions");

            if (Block.AllowBreak(b)) Player.Message(p, "Anybody can activate the block");
            if (Block.Walkthrough(b)) Player.Message(p, "Block can be walked through");
            if (props.KillerBlock) Player.Message(p, "Walking through block will kill you");

            if (props.IsDoor) Player.Message(p, "Block is an ordinary door");
            if (props.IsTDoor) Player.Message(p, "Block is a tdoor, which allows other blocks through when open");
            if (props.oDoorBlock != Block.Invalid) Player.Message(p, "Block is an odoor, which can be toggled by doors and toggles other odoors");

            if (Mover(scope, b)) Player.Message(p, "Block can be activated by walking through it");
        }
        
        static bool Mover(BlockProps[] scope, BlockID b) {
            bool nonSolid = Block.Walkthrough(Block.Convert(b));
            return BlockBehaviour.GetWalkthroughHandler(b, scope, nonSolid) != null;
        }
        
        static bool Physics(BlockProps[] scope, BlockID b) {
            if (scope[b].IsMessageBlock || scope[b].IsPortal) return false;
            if (scope[b].IsDoor || scope[b].IsTDoor) return false;
            if (scope[b].OPBlock) return false;
            
            return BlockBehaviour.GetPhysicsHandler(b, Block.Props) != null;
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/Blocks %H- Lists all basic blocks");
            Player.Message(p, "%T/Blocks complex %H- Lists all complex blocks");
            Player.Message(p, "%T/Blocks [basic block] %H- Lists all blocks which look the same");
            Player.Message(p, "%T/Blocks [complex block] %H- Lists specific info on that block");
            Player.Message(p, "%T/Blocks [rank] %H- Lists all blocks [rank] can use");
            Player.Message(p, "%HTo see available ranks, type %T/ViewRanks");
        }
    }
}
