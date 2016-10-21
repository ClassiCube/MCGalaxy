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

namespace MCGalaxy.Commands.World {
    public sealed class CmdBlockProperties : Command {
        public override string name { get { return "blockproperties"; } }
        public override string shortcut { get { return "blockprops"; } }
        public override string type { get { return CommandTypes.World; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }

        public override void Use(Player p, string message) {
            if (message == "") { Help(p); return; }
            string[] args = message.SplitSpaces(4);
            if (args.Length < 3) { Help(p); return; }
            
            BlockProps[] scope = GetScope(p, args[0]);
            if (scope == null) return;
            
            byte id = GetBlock(p, scope, args[1]);
            if (id == Block.Zero) return;
            string prop = args[2].ToLower();
            
            // TODO: global and level custom blocks
            // TODO: adding core blocks, changing core block names
            SetProperty(p, scope, id, prop, args);
        }
        
        BlockProps[] GetScope(Player p, string scope) {
            if (scope.CaselessEq("core")) return Block.Props;
            if (scope.CaselessEq("global")) return BlockDefinition.GlobalProps;
            if (scope.CaselessEq("level")) return p.level.CustomBlockProps;
            
            Player.Message(p, "&cScope must \"core\", \"global\", or \"level\"");
            return null;
        }
        
        byte GetBlock(Player p, BlockProps[] scope, string block) {
            byte id = 0;
            if (scope == Block.Props) {
                if (!byte.TryParse(block, out id))
                    id = Block.Byte(block);
                
                if (Block.Name(id).CaselessEq("unknown")) {
                    Player.Message(p, "&cThere is no block with id or name \"{0}\"", block);
                    id = Block.Zero;
                }
            } else if (scope == BlockDefinition.GlobalProps) {
                id = BlockDefinition.GetBlock(block, BlockDefinition.GlobalDefs);
                if (id == Block.Zero)
                    Player.Message(p, "&cThere is no global custom block with id or name \"{0}\"", block);
            } else {
                id = BlockDefinition.GetBlock(block, p.level.CustomBlockDefs);
                if (id == Block.Zero)
                    Player.Message(p, "&cThere is no level custom block with id or name \"{0}\"", block);
                if (p.level.CustomBlockDefs[id] == BlockDefinition.GlobalDefs[id]) {
                    Player.Message(p, "&cUse %T/blockprops global &cto modify this custom block."); return Block.Zero;
                }
            }
            return id;
        }
        
        
        void SetProperty(Player p, BlockProps[] scope, byte id,
                         string prop, string[] args) {
            if (prop == "portal") {
                Toggle(p, scope, id, "a portal",
                       (ref BlockProps props) => props.IsPortal = !props.IsPortal,
                       (BlockProps props) => props.IsPortal);
            } else if (prop == "rails") {
                Toggle(p, scope, id, "train rails",
                       (ref BlockProps props) => props.IsRails = !props.IsRails,
                       (BlockProps props) => props.IsRails);
            } else if (prop == "mb" || prop == "messageblock") {
                Toggle(p, scope, id, "a message block",
                       (ref BlockProps props) => props.IsMessageBlock = !props.IsMessageBlock,
                       (BlockProps props) => props.IsMessageBlock);
            } else if (prop == "waterkills") {
                Toggle(p, scope, id, "killed by water",
                       (ref BlockProps props) => props.WaterKills = !props.WaterKills,
                       (BlockProps props) => props.WaterKills);
            } else if (prop == "lavakills") {
                Toggle(p, scope, id, "killed by lava",
                       (ref BlockProps props) => props.LavaKills = !props.LavaKills,
                       (BlockProps props) => props.LavaKills);
            } else if (prop == "killer" || prop == "death") {
                Toggle(p, scope, id, "a killer block",
                       (ref BlockProps props) => props.KillerBlock = !props.KillerBlock,
                       (BlockProps props) => props.KillerBlock);
            } else if (prop == "deathmsg" || prop == "deathmessage") {
                string msg = args.Length > 3 ? args[3] : null;
                scope[id].DeathMessage = msg;
                
                if (msg == null) {
                    Player.Message(p, "Death message for {0} removed.",
                                   BlockName(scope, p.level, id));
                } else {
                    Player.Message(p, "Death message for {0} set to: {1}",
                                   BlockName(scope, p.level, id), msg);
                }
                OnPropsChanged(scope, id);
            } else {
                Help(p);
            }
        }
        
        delegate void BoolSetter(ref BlockProps props);
        static void Toggle(Player p, BlockProps[] scope, byte id, string type,
                           BoolSetter setter, Func<BlockProps, bool> getter) {
            BlockProps props = scope[id];
            setter(ref props);
            scope[id] = props;
            OnPropsChanged(scope, id);
            
            Player.Message(p, "Block {0} is {1}: {2}",
                           BlockName(scope, p.level, id),
                           type, getter(props) ? "&aYes" : "&cNo");
        }

        static void OnPropsChanged(BlockProps[] scope, byte id) {
            if (scope == Block.Props) {
                BlockBehaviour.SetupCoreHandlers();
            } else if (scope == BlockDefinition.GlobalProps) {
                Level[] loaded = LevelInfo.Loaded.Items;
                
                foreach (Level lvl in loaded) {
                    if (lvl.CustomBlockDefs[id] != BlockDefinition.GlobalDefs[id]) continue;
                    lvl.CustomBlockProps[id] = BlockDefinition.GlobalProps[id];
                }
            }
        }
        
        static string BlockName(BlockProps[] scope, Level lvl, byte id) {
            byte block = id, extBlock = 0;
            if (scope != Block.Props) {
                block = Block.custom_block; extBlock = id;
            }
            return lvl.BlockName(block, extBlock);
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/blockprops [scope] [id] [property] <value>");
            Player.Message(p, "%HSets various properties for blocks.");
            Player.Message(p, "%H[scope] can be \"core\", \"global\", or \"level");
        }
    }
}
