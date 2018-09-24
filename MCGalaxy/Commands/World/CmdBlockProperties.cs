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
using System.Collections.Generic;
using MCGalaxy.Blocks;
using BlockID = System.UInt16;

namespace MCGalaxy.Commands.World {
    public sealed class CmdBlockProperties : Command2 {
        public override string name { get { return "BlockProperties"; } }
        public override string shortcut { get { return "BlockProps"; } }
        public override string type { get { return CommandTypes.World; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }

        public override void Use(Player p, string message, CommandData data) {
            if (message.Length == 0) { Help(p); return; }
            string[] args = message.SplitSpaces(4);
            if (args.Length < 2) { Help(p); return; }
            
            BlockProps[] scope = GetScope(p, data, args[0]);
            if (scope == null) return;           
            if (IsListCommand(args[1]) && (args.Length == 2 || IsListModifier(args[2]))) {
            	ListProps(p, scope, args); return;
            }
            
            BlockID block = GetBlock(p, scope, args[1]);
            if (block == Block.Invalid) return;
            if (args.Length < 3) { Help(p); return; }            
            string opt = args[2];
            
            if (opt.CaselessEq("copy")) {
                CopyProps(p, scope, block, args);
            } else if (opt.CaselessEq("reset") || IsDeleteCommand(opt)) {
                ResetProps(p, scope, block);
            } else {
                SetProps(p, scope, block, args);
            }
        }
        
        static BlockProps[] GetScope(Player p, CommandData data, string scope) {
            if (scope.CaselessEq("core") || scope.CaselessEq("global")) return Block.Props;

            if (scope.CaselessEq("level")) {
                if (p.IsSuper) { p.Message("Cannot use level scope from {0}.", p.SuperName); return null; }
                if (!LevelInfo.Check(p, data.Rank, p.level, "change properties of blocks in this level")) return null;
                return p.level.Props;
            }
            
            p.Message("%WScope must be: global or level");
            return null;
        }
        
        static BlockID GetBlock(Player p, BlockProps[] scope, string str) {
        	Player pScope = scope == Block.Props ? Player.Console : p;
            BlockID block = Block.Parse(pScope, str);
            
            if (block == Block.Invalid) {
                p.Message("%WThere is no block \"{0}\".", str);
            }
            return block;
        }
        
        internal static void Detail(Player p, BlockProps[] scope, BlockID block) {
            BlockProps props = scope[block];
            string name = BlockOptions.Name(scope, p, block);
            p.Message("%TProperties of {0}:", name);
            
            if (props.KillerBlock)          p.Message("  Kills players who collide with this block");
            if (props.DeathMessage != null) p.Message("  Death message: %S" + props.DeathMessage);
            
            if (props.IsDoor)  p.Message("  Is an ordinary door");
            if (props.IsTDoor) p.Message("  Is a tdoor (allows other blocks through when open)");
            if (props.oDoorBlock != Block.Invalid) 
                p.Message("  Is an odoor (can be toggled by doors, and toggles other odoors)");
            
            if (props.IsPortal)       p.Message("  Can be used as a %T/Portal");
            if (props.IsMessageBlock) p.Message("  Can be used as a %T/MessageBlock");
            
            if (props.WaterKills) p.Message("  Is destroyed by flooding water");
            if (props.LavaKills)  p.Message("  Is destroyed by flooding lava");
            
            if (props.OPBlock) p.Message("  Is not affected by explosions");
            if (props.IsRails) p.Message("  Can be used as rails for %T/Train");
            
            if (props.AnimalAI != AnimalAI.None) {
                p.Message("  Has the {0} AI behaviour", props.AnimalAI);
            }
            if (props.StackBlock != Block.Air) {
                p.Message("  Stacks as {0} when placed on top of itself", 
                          BlockOptions.Name(scope, p, props.StackBlock));
            }
            if (props.Drownable) p.Message("%H  Players can drown in this block");
            
            if (props.GrassBlock != Block.Invalid) {
                p.Message("  Grows into {0} when in sunlight", 
                          BlockOptions.Name(scope, p, props.GrassBlock));
            }
            if (props.DirtBlock != Block.Invalid) {
                p.Message("  Decays into {0} when in shadow", 
                          BlockOptions.Name(scope, p, props.DirtBlock));
            }
        }
        
        static List<BlockID> FilterProps(BlockProps[] scope) {
            int changed = BlockOptions.ScopeId(scope);
            List<BlockID> filtered = new List<BlockID>();
            
            for (int b = 0; b < scope.Length; b++) {
                if ((scope[b].ChangedScope & changed) == 0) continue;                
                filtered.Add((BlockID)b);
            }
            return filtered;
        }
        
        void ListProps(Player p, BlockProps[] scope, string[] args) {
            List<BlockID> filtered = FilterProps(scope);
            string cmd      = "BlockProps " + args[0] + " list";
            string modifier = args.Length > 2 ? args[2] : "";
            
            MultiPageOutput.Output(p, filtered, b => BlockOptions.Name(scope, p, b),
                                   cmd, "modified blocks", modifier, false);
        }
        
        void CopyProps(Player p, BlockProps[] scope, BlockID block, string[] args) {
        	if (args.Length < 4) { Help(p); return; }
        	BlockID dst = GetBlock(p, scope, args[3]);
        	if (dst == Block.Invalid) return;
        	
        	scope[dst] = scope[block];
        	scope[dst].ChangedScope |= BlockOptions.ScopeId(scope);
            
            p.Message("Copied properties of {0} to {1}",
                      BlockOptions.Name(scope, p, block),
                      BlockOptions.Name(scope, p, dst));
            BlockOptions.ApplyChanges(scope, p.level, block, true);
        }
        
        void ResetProps(Player p, BlockProps[] scope, BlockID block) {
            scope[block] = BlockOptions.DefaultProps(scope, p.level, block);
            string name  = BlockOptions.Name(scope, p, block);
            
            p.Message("Reset properties of {0} to default", name);
            BlockOptions.ApplyChanges(scope, p.level, block, true);
        }
        
        void SetProps(Player p, BlockProps[] scope, BlockID block, string[] args) {
            BlockOption opt = BlockOptions.Find(args[2]);
            if (opt == null) { Help(p); return; }
            string value = args.Length > 3 ? args[3] : "";
            
            opt.SetFunc(p, scope, block, value);
            scope[block].ChangedScope |= BlockOptions.ScopeId(scope);
            BlockOptions.ApplyChanges(scope, p.level, block, true);
        }
        
        public override void Help(Player p) {                	
            p.Message("%T/BlockProps global/level list");
            p.Message("%HLists blocks which have non-default properties");
            p.Message("%T/BlockProps global/level [id/name] copy [new id]");
            p.Message("%HCopies properties of that block to another");
            p.Message("%T/BlockProps global/level [id/name] reset");
            p.Message("%HResets properties of that block to their default");
            p.Message("%T/BlockProps global/level [id/name] [property] <value>");
            p.Message("%HSets various properties of that block");
            p.Message("%H  Use %T/Help BlockProps props %Hfor a list of properties");
            p.Message("%H  Use %T/Help BlockProps [property] %Hfor more details");
        }
        
        public override void Help(Player p, string message) {
            if (message.CaselessEq("props") || message.CaselessEq("properties")) {
                p.Message("%HProperties: &f{0}", BlockOptions.Options.Join(o => o.Name));
                p.Message("%HUse %T/Help BlockProps [property] %Hfor more details");
                return;
            }
            
            BlockOption opt = BlockOptions.Find(message);
            if (opt != null) {
                p.Message(opt.Help);
            } else {
                p.Message("%WUnrecognised property \"{0}\"", message);
            }
        }
    }
}
