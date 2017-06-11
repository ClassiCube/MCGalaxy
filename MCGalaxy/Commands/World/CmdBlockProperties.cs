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
            ExtBlock block = GetBlock(p, scope, args[1]);
            if (block.IsInvalid) return;
            
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
                return p.level.BlockProps;
            }
            
            Player.Message(p, "&cScope must \"core\", \"global\", or \"level\"");
            return null;
        }
        
        ExtBlock GetBlock(Player p, BlockProps[] scope, string input) {
            if (scope == Block.Props) {
                byte raw;
                if (!byte.TryParse(input, out  raw))
                    raw = Block.Byte(input);
                
                if (Block.Name(raw).CaselessEq("unknown")) {
                    Player.Message(p, "&cThere is no block with id or name \"{0}\"", input);
                    return ExtBlock.Invalid;
                }
                return new ExtBlock(raw, 0);
            } else if (scope == BlockDefinition.GlobalProps) {
                byte raw = BlockDefinition.GetBlock(input, BlockDefinition.GlobalDefs);
                if (raw == Block.Invalid)
                    Player.Message(p, "&cThere is no global custom block with id or name \"{0}\"", input);
                return ExtBlock.FromRaw(raw);
            } else {
                byte raw = BlockDefinition.GetBlock(input, p.level.CustomBlockDefs);
                if (raw == Block.Invalid)
                    Player.Message(p, "&cThere is no level custom block with id or name \"{0}\"", input);
                
                if (p.level.CustomBlockDefs[raw] == BlockDefinition.GlobalDefs[raw]) {
                    Player.Message(p, "&cUse %T/blockprops global &cto modify this custom block."); return ExtBlock.Invalid;
                }
                return ExtBlock.FromRaw(raw);
            }
        }
        
        
        void SetProperty(Player p, BlockProps[] scope, ExtBlock block,
                         string prop, string[] args) {
            if (prop == "portal") {
                Toggle(p, scope, block, "a portal",
                       (ref BlockProps props) => props.IsPortal = !props.IsPortal,
                       (BlockProps props) => props.IsPortal);
            } else if (prop == "mb" || prop == "messageblock") {
                Toggle(p, scope, block, "a message block",
                       (ref BlockProps props) => props.IsMessageBlock = !props.IsMessageBlock,
                       (BlockProps props) => props.IsMessageBlock);
            } else if (prop == "rails") {
                Toggle(p, scope, block, "train rails",
                       (ref BlockProps props) => props.IsRails = !props.IsRails,
                       (BlockProps props) => props.IsRails);
            } else if (prop == "waterkills") {
                Toggle(p, scope, block, "killed by water",
                       (ref BlockProps props) => props.WaterKills = !props.WaterKills,
                       (BlockProps props) => props.WaterKills);
            } else if (prop == "lavakills") {
                Toggle(p, scope, block, "killed by lava",
                       (ref BlockProps props) => props.LavaKills = !props.LavaKills,
                       (BlockProps props) => props.LavaKills);
            } else if (prop == "door") {
                Toggle(p, scope, block, "a door",
                       (ref BlockProps props) => props.IsDoor = !props.IsDoor,
                       (BlockProps props) => props.IsDoor);
            } else if (prop == "tdoor") {
                Toggle(p, scope, block, "a tdoor",
                       (ref BlockProps props) => props.IsTDoor = !props.IsTDoor,
                       (BlockProps props) => props.IsTDoor);
            } else if (prop == "killer" || prop == "death") {
                Toggle(p, scope, block, "a killer block",
                       (ref BlockProps props) => props.KillerBlock = !props.KillerBlock,
                       (BlockProps props) => props.KillerBlock);
            } else if (prop == "deathmsg" || prop == "deathmessage") {
                string msg = args.Length > 3 ? args[3] : null;
                SetDeathMessage(p, scope, block, msg);
            } else if (prop == "animalai" || prop == "animal") {
                string msg = args.Length > 3 ? args[3] : null;
                SetEnum(p, scope, block, msg);
            } else {
                Help(p);
            }
        }
        
        
        delegate void BoolSetter(ref BlockProps props);
        static void Toggle(Player p, BlockProps[] scope, ExtBlock block, string type,
                           BoolSetter setter, Func<BlockProps, bool> getter) {
            BlockProps props = scope[block.Index];
            setter(ref props);
            scope[block.Index] = props;
            Level lvl = Player.IsSuper(p) ? null : p.level;
            
            Player.Message(p, "Block {0} is {1}: {2}",
                           BlockName(scope, lvl, block),
                           type, getter(props) ? "&aYes" : "&cNo");
            OnPropsChanged(scope, lvl, block, false);
        }
        
        static void SetEnum(Player p, BlockProps[] scope, ExtBlock block, string msg) {
            Level lvl = Player.IsSuper(p) ? null : p.level;            
            AnimalAI ai = AnimalAI.None;
            if (!CommandParser.GetEnum(p, msg, "Animal AI", ref ai)) return;
            
            scope[block.Index].AnimalAI = ai;
            Player.Message(p, "Animal AI for {0} set to: {1}",
                               BlockName(scope, lvl, block), ai);
            OnPropsChanged(scope, lvl, block, true);
        }
        
        static void SetDeathMessage(Player p, BlockProps[] scope, ExtBlock block, string msg) {
            scope[block.Index].DeathMessage = msg;
            Level lvl = Player.IsSuper(p) ? null : p.level;
            
            if (msg == null) {
                Player.Message(p, "Death message for {0} removed.",
                               BlockName(scope, lvl, block));
            } else {
                Player.Message(p, "Death message for {0} set to: {1}",
                               BlockName(scope, lvl, block), msg);
            }
            OnPropsChanged(scope, lvl, block, false);
        }
        

        static void OnPropsChanged(BlockProps[] scope, Level level, ExtBlock block, bool physics) {
            scope[block.Index].Changed = true;
            
            if (scope == Block.Props) {
                BlockProps.Save("core", scope, true);
                Level[] loaded = LevelInfo.Loaded.Items;
                
                foreach (Level lvl in loaded) {
                    lvl.SetBlockHandler(block);
                    lvl.BlockProps[block.Index] = BlockDefinition.GlobalProps[block.Index];
                }
            } else if (scope == BlockDefinition.GlobalProps) {
                Level[] loaded = LevelInfo.Loaded.Items;
                BlockProps.Save("global", scope, false);
                
                byte raw = block.RawID;
                foreach (Level lvl in loaded) {
                    if (lvl.CustomBlockDefs[raw] != BlockDefinition.GlobalDefs[raw]) continue;                    
                    lvl.BlockProps[block.Index] = BlockDefinition.GlobalProps[block.Index];
                    lvl.SetBlockHandler(block);
                }                
            } else {
                BlockProps.Save("lvl_" + level.name, scope, false);
                level.SetBlockHandler(block);
            }
        }
        
        static string BlockName(BlockProps[] scope, Level lvl, ExtBlock block) {
            if (scope == Block.Props) return Block.Name(block.RawID);
            BlockDefinition def = null;
            
            if (scope == BlockDefinition.GlobalProps) {
                def = BlockDefinition.GlobalDefs[block.RawID];
            } else {
                def = lvl.CustomBlockDefs[block.RawID];
            }
            return def == null ? block.RawID.ToString() : def.Name.Replace(" ", "");
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
