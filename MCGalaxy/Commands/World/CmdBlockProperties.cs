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
            
            BlockProps[] props = GetScopeBlocks(p, args[0]);
            if (props == null) return;
            
            byte id;
            if (!byte.TryParse(args[1], out id)) {
                Player.Message(p, "&c\"{0}\" is not a valid block id.", id); return;
            }
            string prop = args[2].ToLower();
            
            // TODO: global and level custom blocks
            // TODO: adding core blocks, changing core block names
            if (scope != "core") {
                Player.Message(p, "Sorry! Custom blocks still a WIP."); return;
            }
            if (Block.Name(id).CaselessEq("unknown")) {
                Player.Message(p, "Sorry! Adding blocks still a WIP."); return;
            }
            
            SetProperty(p, prop, args);
        }
        
        BlockProps[] GetScopeBlocks(Player p, string scope) {
        	if (scope.CaselessEq("core")) return Block.Props;
        	if (scope.CaselessEq("global")) return BlockDefinition.GlobalProps;
        	if (scope.CaselessEq("level")) return p.level.CustomBlockDefs;
        	
        	Player.Message(p, "&cScope must \"core\", \"global\", or \"level\""); 
        	return null;
        }
        
        void SetProperty(Player p, string prop, string[] args) {
            if (prop == "portal") {
                ToggleBool(p, id, "a portal",
                           (ref BlockProps props) => props.IsPortal = !props.IsPortal,
                           (BlockProps props) => props.IsPortal);
            } else if (prop == "rails") {
                ToggleBool(p, id, "train rails",
                           (ref BlockProps props) => props.IsRails = !props.IsRails,
                           (BlockProps props) => props.IsRails);
            } else if (prop == "mb" || prop == "messageblock") {
                ToggleBool(p, id, "a message block",
                           (ref BlockProps props) => props.IsMessageBlock = !props.IsMessageBlock,
                           (BlockProps props) => props.IsMessageBlock);
            } else if (prop == "waterkills") {
                ToggleBool(p, id, "killed by water",
                           (ref BlockProps props) => props.WaterKills = !props.WaterKills,
                           (BlockProps props) => props.WaterKills);
            } else if (prop == "lavakills") {
                ToggleBool(p, id, "killed by lava",
                           (ref BlockProps props) => props.LavaKills = !props.LavaKills,
                           (BlockProps props) => props.LavaKills);
            } else if (prop == "killer" || prop == "death") {
                ToggleBool(p, id, "a killer block",
                           (ref BlockProps props) => props.KillerBlock = !props.KillerBlock,
                           (BlockProps props) => props.KillerBlock);
            } else if (prop == "deathmsg" || prop == "deathmessage") {
                string msg = args.Length > 3 ? args[3] : null;
                Block.Props[id].DeathMessage = msg;
                
                if (msg == null) {
                    Player.Message(p, "Death message for {0} removed.", Block.Name(id));
                } else {
                    Player.Message(p, "Death message for {0} set to: {1}", Block.Name(id), msg);
                }
                OnPropsChanged(Block.Props, id);
            } else {
                Help(p);
            }
        }
        
        delegate void BoolSetter(ref BlockProps props);
        static void ToggleBool(Player p, byte id, string name, BoolSetter setter,
                               Func<BlockProps, bool> getter) {
            BlockProps props = Block.Props[id];
            setter(ref props);
            Block.Props[id] = props;
            OnPropsChanged(Block.Props, id);
            
            Player.Message(p, "Block {0} is {1}: {2}", Block.Name(id),
                           name, getter(props) ? "&aYes" : "&cNo");
        }

        static void OnPropsChanged(BlockProps[] props, byte id) {
            if (props == Block.Props) {
                BlockBehaviour.SetupCoreHandlers();
            } else if (props == BlockDefinition.GlobalProps) {
                Level[] loaded = LevelInfo.Loaded.Items;
                
                foreach (Level lvl in loaded) {
                    if (lvl.CustomBlockDefs[id] != BlockDefinition.GlobalDefs[id]) continue;
                    lvl.CustomBlockProps[id] = BlockDefinition.GlobalDefs[id];
                }
            }
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/blockprops [scope] [id] [property] <value>");
            Player.Message(p, "%HSets various properties for blocks.");
            Player.Message(p, "%H[scope] can be \"core\", \"global\", or \"level");
        }
    }
}
