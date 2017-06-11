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
            
            byte block = GetBlock(p, scope, args[1]);
            if (block == Block.Invalid) return;
            string prop = args[2].ToLower();
            SetProperty(p, scope, block, prop, args);
        }
        
        BlockProps[] GetScope(Player p, string scope) {
            if (scope.CaselessEq("core")) return Block.Props;
            if (scope.CaselessEq("global")) return BlockDefinition.GlobalProps;
            
            if (scope.CaselessEq("level")) {
                if (Player.IsSuper(p)) {
                    string src = p == null ? "console" : "IRC";
                    Player.Message(p, "Cannot use level scope from {0}.",  src);
                    return null;
                }
                return p.level.CustomBlockProps;
            }
            
            Player.Message(p, "&cScope must \"core\", \"global\", or \"level\"");
            return null;
        }
        
        byte GetBlock(Player p, BlockProps[] scope, string input) {
            byte block = 0;
            if (scope == Block.Props) {
                if (!byte.TryParse(input, out block))
                    block = Block.Byte(input);
                
                if (Block.Name(block).CaselessEq("unknown")) {
                    Player.Message(p, "&cThere is no block with id or name \"{0}\"", input);
                    block = Block.Invalid;
                }
            } else if (scope == BlockDefinition.GlobalProps) {
                block = BlockDefinition.GetBlock(input, BlockDefinition.GlobalDefs);
                if (block == Block.Invalid)
                    Player.Message(p, "&cThere is no global custom block with id or name \"{0}\"", input);
            } else {
                block = BlockDefinition.GetBlock(input, p.level.CustomBlockDefs);
                if (block == Block.Invalid)
                    Player.Message(p, "&cThere is no level custom block with id or name \"{0}\"", input);
                if (p.level.CustomBlockDefs[block] == BlockDefinition.GlobalDefs[block]) {
                    Player.Message(p, "&cUse %T/blockprops global &cto modify this custom block."); return Block.Invalid;
                }
            }
            return block;
        }
        
        
        void SetProperty(Player p, BlockProps[] scope, byte id,
                         string prop, string[] args) {
            if (prop == "portal") {
                Toggle(p, scope, id, "a portal",
                       (ref BlockProps props) => props.IsPortal = !props.IsPortal,
                       (BlockProps props) => props.IsPortal);
            } else if (prop == "mb" || prop == "messageblock") {
                Toggle(p, scope, id, "a message block",
                       (ref BlockProps props) => props.IsMessageBlock = !props.IsMessageBlock,
                       (BlockProps props) => props.IsMessageBlock);
            } else if (prop == "rails") {
                Toggle(p, scope, id, "train rails",
                       (ref BlockProps props) => props.IsRails = !props.IsRails,
                       (BlockProps props) => props.IsRails);
            } else if (prop == "waterkills") {
                Toggle(p, scope, id, "killed by water",
                       (ref BlockProps props) => props.WaterKills = !props.WaterKills,
                       (BlockProps props) => props.WaterKills);
            } else if (prop == "lavakills") {
                Toggle(p, scope, id, "killed by lava",
                       (ref BlockProps props) => props.LavaKills = !props.LavaKills,
                       (BlockProps props) => props.LavaKills);
            } else if (prop == "door") {
                Toggle(p, scope, id, "a door",
                       (ref BlockProps props) => props.IsDoor = !props.IsDoor,
                       (BlockProps props) => props.IsDoor);
            } else if (prop == "tdoor") {
                Toggle(p, scope, id, "a tdoor",
                       (ref BlockProps props) => props.IsTDoor = !props.IsTDoor,
                       (BlockProps props) => props.IsTDoor);
            } else if (prop == "killer" || prop == "death") {
                Toggle(p, scope, id, "a killer block",
                       (ref BlockProps props) => props.KillerBlock = !props.KillerBlock,
                       (BlockProps props) => props.KillerBlock);
            } else if (prop == "deathmsg" || prop == "deathmessage") {
                string msg = args.Length > 3 ? args[3] : null;
                SetDeathMessage(p, scope, id, msg);
            } else if (prop == "animalai" || prop == "animal") {
                string msg = args.Length > 3 ? args[3] : null;
                SetEnum(p, scope, id, msg);
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
            Level lvl = Player.IsSuper(p) ? null : p.level;
            
            Player.Message(p, "Block {0} is {1}: {2}",
                           BlockName(scope, lvl, id),
                           type, getter(props) ? "&aYes" : "&cNo");
            OnPropsChanged(scope, lvl, id, false);
        }
        
        static void SetEnum(Player p, BlockProps[] scope, byte id, string msg) {
            Level lvl = Player.IsSuper(p) ? null : p.level;            
            AnimalAI ai = AnimalAI.None;
            if (!CommandParser.GetEnum(p, msg, "Animal AI", ref ai)) return;
            
            scope[id].AnimalAI = ai;
            Player.Message(p, "Animal AI for {0} set to: {1}",
                               BlockName(scope, lvl, id), ai);
            OnPropsChanged(scope, lvl, id, true);
        }
        
        static void SetDeathMessage(Player p, BlockProps[] scope, byte id, string msg) {
            scope[id].DeathMessage = msg;
            Level lvl = Player.IsSuper(p) ? null : p.level;
            
            if (msg == null) {
                Player.Message(p, "Death message for {0} removed.",
                               BlockName(scope, lvl, id));
            } else {
                Player.Message(p, "Death message for {0} set to: {1}",
                               BlockName(scope, lvl, id), msg);
            }
            OnPropsChanged(scope, lvl, id, false);
        }
        

        static void OnPropsChanged(BlockProps[] scope, Level level, byte id, bool physics) {
            scope[id].Changed = true;
            
            if (scope == Block.Props) {
                BlockBehaviour.SetDefaultHandler(id);
                BlockProps.Save("core", scope);
            } else if (scope == BlockDefinition.GlobalProps) {
                Level[] loaded = LevelInfo.Loaded.Items;
                foreach (Level lvl in loaded) {
                    if (lvl.CustomBlockDefs[id] != BlockDefinition.GlobalDefs[id]) continue;
                    lvl.CustomBlockProps[id] = BlockDefinition.GlobalProps[id];
                }
                BlockProps.Save("global", scope);
            } else {
                BlockProps.Save("lvl_" + level.name, scope);
            }
        }
        
        static string BlockName(BlockProps[] scope, Level lvl, byte raw) {
            if (scope == Block.Props) return Block.Name(raw);
            BlockDefinition def = null;
            
            if (scope == BlockDefinition.GlobalProps) {
                def = BlockDefinition.GlobalDefs[raw];
            } else {
                def = lvl.CustomBlockDefs[raw];
            }
            return def == null ? raw.ToString() : def.Name.Replace(" ", "");
        }
        
        
        public override void Help(Player p) {
            Player.Message(p, "%T/blockprops [scope] [id/name] [property] <value>");
            Player.Message(p, "%HSets various properties for blocks.");
            Player.Message(p, "%H[scope] can be: %Score, global, level");
            
            Player.Message(p, "%Hproperties: %Sportal, messageblock, rails, waterkills, " +
                           "lavakills, door, tdoor, killer, deathmessage, animalai");
            Player.Message(p, "%HType %T/help blockprops [property] %Hfor more details");
        }
        
        public override void Help(Player p, string message) {
            if (message.CaselessEq("portal")) {
                Player.Message(p, "%HToggles whether the block is a %T/portal");
            } else if (message.CaselessEq("messageblock")) {
                Player.Message(p, "%HToggles whether the block is a %T/messageblock");
            } else if (message.CaselessEq("rails")) {
                Player.Message(p, "%HToggles whether %Strain %Hblocks can run over this block");
            } else if (message.CaselessEq("waterkills")) {
                Player.Message(p, "%HToggles whether flooding water kills this block");
            } else if (message.CaselessEq("lavakills")) {
                Player.Message(p, "%HToggles whether flooding lava kills this block");
            } else if (message.CaselessEq("door")) {
                Player.Message(p, "%HToggles whether this block is a Door block");
            } else if (message.CaselessEq("tdoor")) {
                Player.Message(p, "%HToggles whether this block is a TDoor block");
            } else if (message.CaselessEq("killer")) {
                Player.Message(p, "%HToggles whether this block kills players who collide with it");
            } else if (message.CaselessEq("deathmessage")) {
                Player.Message(p, "%HSets or removes the death message for this block");
                Player.Message(p, "%H  Note: %S@p %His a placeholder for the player's name");
            } else if (message.CaselessEq("animalai")) {
                Player.Message(p, "%HSets the flying or swimming animal AI for this block.");
                string[] aiNames = Enum.GetNames(typeof(AnimalAI));
                Player.Message(p, "%H  Types: &f{0}", aiNames.Join());
            } else {
                Player.Message(p, "&cUnrecognised property \"{0}\"", message);
            }
        }
    }
}
