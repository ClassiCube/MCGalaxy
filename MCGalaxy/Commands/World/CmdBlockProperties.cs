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
            if (args.Length < 3) { Help(p); return; }
            
            BlockProps[] scope = GetScope(p, data, args[0]);
            if (scope == null) return;
            
            Player pScope = scope == Block.Props ? Player.Console : p;
            BlockID block = Block.Parse(pScope, args[1]);
            if (block == Block.Invalid) {
                p.Message("%WThere is no block \"{0}\".", args[1]); return;
            }
            
            if (args[2].CaselessEq("reset")) {
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
        
        void ResetProps(Player p, BlockProps[] scope, BlockID block) {
        	if (scope == Block.Props) {
        	    scope[block] = Block.MakeDefaultProps(block);
        	} else {
        		scope[block] = Block.Props[block];
        	}
        	
        	string name = BlockOptions.BlockName(scope, p, block);
            p.Message("Reset properties of block {0} to default", name);
        	BlockOptions.SaveChanges(scope, p.level, block);
        }
        
        void SetProps(Player p, BlockProps[] scope, BlockID block, string[] args) {      	
            BlockOption opt = BlockOptions.Find(args[2]);
            if (opt == null) { Help(p); return; }

            string value = args.Length > 3 ? args[3] : "";            
            opt.SetFunc(p, scope, block, value);
            
            int scopeId = (scope == Block.Props) ? 1 : 2;
            scope[block].ChangedScope |= (byte)scopeId;
            BlockOptions.SaveChanges(scope, p.level, block);
        }
        
        public override void Help(Player p) {
            p.Message("%T/BlockProps global/level [id/name] [property] <value>");
            p.Message("%HSets various properties of that block");
            p.Message("%H  Use %T/Help BlockProps props %Hfor a list of properties");
            p.Message("%H  Use %T/Help BlockProps [property] %Hfor more details");
            p.Message("%T/BlockProps global/level [id/name] reset");
            p.Message("%HResets properties of that block to their default");
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
