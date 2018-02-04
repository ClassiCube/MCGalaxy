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
        public override string name { get { return "BlockProperties"; } }
        public override string shortcut { get { return "BlockProps"; } }
        public override string type { get { return CommandTypes.World; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }

        public override void Use(Player p, string message) {
            if (message.Length == 0) { Help(p); return; }
            string[] args = message.SplitSpaces(4);
            if (args.Length < 3) { Help(p); return; }
            
            BlockProps[] scope = GetScope(p, args[0]);
            if (scope == null) return;
            ushort block = GetBlock(p, scope, args[1]);
            if (block == Block.Invalid) return;
            
            string prop = args[2].ToLower();
            SetProperty(p, scope, block, prop, args);
        }
        
        static BlockProps[] GetScope(Player p, string scope) {
            if (scope.CaselessEq("core")) return Block.Props;
            if (scope.CaselessEq("global")) return BlockDefinition.GlobalProps;
            
            if (scope.CaselessEq("level")) {
                if (Player.IsSuper(p)) {
                    string src = p == null ? "console" : "IRC";
                    Player.Message(p, "Cannot use level scope from {0}.",  src);
                    return null;
                }
                
                if (!LevelInfo.ValidateAction(p, p.level.name, "change block properties in this level")) return null;
                return p.level.Props;
            }
            
            Player.Message(p, "&cScope must \"core\", \"global\", or \"level\"");
            return null;
        }
        
        static ushort GetBlock(Player p, BlockProps[] scope, string input) {
            if (scope == Block.Props) {
                byte raw;
                if (!byte.TryParse(input, out  raw))
                    raw = Block.Byte(input);
                
                if (Block.Name(raw).CaselessEq("unknown")) {
                    Player.Message(p, "&cThere is no block with id or name \"{0}\"", input);
                    return Block.Invalid;
                }
                return new ushort(raw, 0);
            } else if (scope == BlockDefinition.GlobalProps) {
                int raw = BlockDefinition.GetBlock(input, BlockDefinition.GlobalDefs);
                if (raw == -1) {
                    Player.Message(p, "&cThere is no global custom block with id or name \"{0}\"", input);
                    return Block.Invalid;
                }
                return Block.FromRaw((byte)raw);
            } else {
                int raw = BlockDefinition.GetBlock(input, p.level.CustomBlockDefs);
                if (raw == -1) {
                    Player.Message(p, "&cThere is no level custom block with id or name \"{0}\"", input);
                    return Block.Invalid;
                }
                
                if (p.level.CustomBlockDefs[raw] == BlockDefinition.GlobalDefs[raw]) {
                    Player.Message(p, "&cUse %T/BlockProps global &cto modify this custom block."); return Block.Invalid;
                }
                return Block.FromRaw((byte)raw);
            }
        }
        
        static int GetIndex(BlockProps[] scope, ushort block) {
            return scope == BlockDefinition.GlobalProps ? block.RawID : block;
        }
        
        
        void SetProperty(Player p, BlockProps[] scope, ushort block,
                         string prop, string[] args) {
            int i = GetIndex(scope, block);
            string text = args.Length > 3 ? args[3] : null;
            
            if (prop == "portal") {
                Toggle(p, scope, block, "a portal", ref scope[i].IsPortal);
            } else if (prop == "mb" || prop == "messageblock") {
                Toggle(p, scope, block, "a message block", ref scope[i].IsMessageBlock);
            } else if (prop == "rails") {
                Toggle(p, scope, block, "train rails", ref scope[i].IsRails);
            } else if (prop == "waterkills") {
                Toggle(p, scope, block, "killed by water", ref scope[i].WaterKills);
            } else if (prop == "lavakills") {
                Toggle(p, scope, block, "killed by lava", ref scope[i].LavaKills);
            } else if (prop == "door") {
                Toggle(p, scope, block, "a door", ref scope[i].IsDoor);
            } else if (prop == "tdoor") {
                Toggle(p, scope, block, "a tdoor", ref scope[i].IsTDoor);
            } else if (prop == "killer" || prop == "death") {
                Toggle(p, scope, block, "a killer block", ref scope[i].KillerBlock);
            } else if (prop == "deathmsg" || prop == "deathmessage") {
                SetDeathMessage(p, scope, block, i, text);
            } else if (prop == "animalai" || prop == "animal") {
                SetEnum(p, scope, block, i, text);
            }  else if (prop == "stackid" || prop == "stackblock") {
                SetStackId(p, scope, block, i, text);
            } else if (prop == "opblock" || prop == "op") {
                Toggle(p, scope, block, "an OP block", ref scope[i].OPBlock);
            } else if (prop == "odoor") {
                SetBlock(p, scope, block, i, text, ref scope[i].oDoorBlock, "oDoor");
            } else if (prop == "grass") {
                SetBlock(p, scope, block, i, text, ref scope[i].GrassBlock, "Grass form");
            } else if (prop == "dirt") {
                SetBlock(p, scope, block, i, text, ref scope[i].DirtBlock, "Dirt form");
            } else if (prop == "drownable" || prop == "drown") {
                Toggle(p, scope, block, "drowns players", ref scope[i].Drownable);
            } else {
                Help(p);
            }
        }

        
        static void Toggle(Player p, BlockProps[] scope, ushort block, 
                           string type, ref bool on) {
            on = !on;
            Level lvl = Player.IsSuper(p) ? null : p.level;
            Player.Message(p, "Block {0} is {1}: {2}",
                           BlockName(scope, lvl, block),
                           type, on ? "&aYes" : "&cNo");
            OnPropsChanged(scope, lvl, block);
        }
        
        static void SetEnum(Player p, BlockProps[] scope, ushort block, 
                            int i, string msg) {
            Level lvl = Player.IsSuper(p) ? null : p.level;
            AnimalAI ai = AnimalAI.None;
            if (!CommandParser.GetEnum(p, msg, "Animal AI", ref ai)) return;
            
            scope[i].AnimalAI = ai;
            Player.Message(p, "Animal AI for {0} set to: {1}",
                           BlockName(scope, lvl, block), ai);
            OnPropsChanged(scope, lvl, block);
        }
        
        static void SetDeathMessage(Player p, BlockProps[] scope, ushort block, 
                                    int i, string msg) {
            scope[i].DeathMessage = msg;
            Level lvl = Player.IsSuper(p) ? null : p.level;
            
            if (msg == null) {
                Player.Message(p, "Death message for {0} removed.",
                               BlockName(scope, lvl, block));
            } else {
                Player.Message(p, "Death message for {0} set to: {1}",
                               BlockName(scope, lvl, block), msg);
            }
            OnPropsChanged(scope, lvl, block);
        }
        
        static void SetStackId(Player p, BlockProps[] scope, ushort block, 
                               int i, string msg) {
            Level lvl = Player.IsSuper(p) ? null : p.level;
            
            ushort stackBlock;
            if (msg == null) {
                stackBlock = Block.Air;
            } else {
                if (!CommandParser.GetBlock(p, msg, out stackBlock)) return;
            }
            scope[i].StackId = stackBlock.RawID;
            
            if (stackBlock.IsAir) {
                Player.Message(p, "Removed stack block for {0}", BlockName(scope, lvl, block));
            } else {
                string stackBlockName = Player.IsSuper(p) ?
                    BlockName(scope, lvl, stackBlock) : p.level.BlockName(stackBlock);
                Player.Message(p, "Stack block for {0} set to: {1}",
                               BlockName(scope, lvl, block), stackBlockName);
            }
            OnPropsChanged(scope, lvl, block);
        }
        
        static void SetBlock(Player p, BlockProps[] scope, ushort block, 
                             int i, string msg, ref ushort target, string type) {
            Level lvl = Player.IsSuper(p) ? null : p.level;           
            if (msg == null) {
                target = Block.Invalid;
                Player.Message(p, "{1} for {0} removed.", BlockName(scope, lvl, block), type);
            } else {
                ushort other;
                if (!CommandParser.GetBlock(p, msg, out other)) return;
                if (other == block) { Player.Message(p, "ID of {0} must be different.", type); return; }
                
                target = other;        
                Player.Message(p, "{2} for {0} set to: {1}", BlockName(scope, lvl, block), 
                               BlockName(scope, lvl, other), type);
            }
            OnPropsChanged(scope, lvl, block);
        }        

        static void OnPropsChanged(BlockProps[] scope, Level level, ushort block) {
            scope[GetIndex(scope, block)].Changed = true;            
            if (scope == Block.Props) {
                BlockProps.Save("core", scope, Block.CorePropsLock, null);
                Level[] loaded = LevelInfo.Loaded.Items;                
                if (!block.IsPhysicsType)
                    BlockDefinition.GlobalProps[block.RawID] = BlockDefinition.DefaultProps(block);
                
                foreach (Level lvl in loaded) {
                    if (lvl.HasCustomProps(block)) continue;
                    
                    lvl.Props[block] = BlockDefinition.DefaultProps(block);
                    lvl.UpdateBlockHandler(block);
                }
            } else if (scope == BlockDefinition.GlobalProps) {
                Level[] loaded = LevelInfo.Loaded.Items;
                BlockProps.Save("global", scope, BlockDefinition.GlobalPropsLock, null);
                
                byte raw = block.RawID;
                foreach (Level lvl in loaded) {
                    if (lvl.CustomBlockDefs[raw] != BlockDefinition.GlobalDefs[raw]) continue;
                    if (lvl.HasCustomProps(block)) continue;
                    
                    lvl.Props[block] = BlockDefinition.DefaultProps(block);
                    lvl.UpdateBlockHandler(block);
                }
            } else {
                BlockProps.Save("lvl_" + level.name, scope, level.PropsLock, idx => SelectLevel(level, idx));
                level.UpdateBlockHandler(block);
            }
        }
        
        static bool SelectLevel(Level lvl, int i) {
            return lvl.HasCustomProps((ushort)i);
        }
        
        static string BlockName(BlockProps[] scope, Level lvl, ushort block) {
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
            Player.Message(p, "%T/BlockProps [scope] [id/name] [property] <value>");
            Player.Message(p, "%HSets various properties for blocks.");
            Player.Message(p, "%H[scope] can be: %Score, global, level");
            
            Player.Message(p, "%Hproperties: %Sportal, messageblock, rails, waterkills, lavakills, door, tdoor, " +
                           "killer, deathmessage, animalai, stackblock, opblock, odoor, drownable, grass, dirt");
            Player.Message(p, "%HType %T/Help BlockProps [property] %Hfor more details");
        }
        
        public override void Help(Player p, string message) {
            if (message.CaselessEq("portal")) {
                Player.Message(p, "%HToggles whether the block is a %T/Portal");
            } else if (message.CaselessEq("messageblock")) {
                Player.Message(p, "%HToggles whether the block is a %T/MessageBlock");
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
            } else if (message.CaselessEq("stackblock")) {
                Player.Message(p, "%HSets the block this block is converted into, when placed on top of " +
                               "another of the same block. (e.g. placing two slabs on each other becomes a double slab)");
            } else if (message.CaselessEq("opblock")) {
                Player.Message(p, "%HMarks the block as being on OP block. OP blocks can't be blown up by explosions, " +
                               "and can't be replaced in games when build type is ModifyOnly.");
            } else if (message.CaselessEq("odoor")) {
                Player.Message(p, "%HSets the block that this block is changed into, when activated by a neighbouring door.");
            } else if (message.CaselessEq("drownable")) {
                Player.Message(p, "%HSets whether this block can drown players.");
                Player.Message(p, "%T/Map death %Hmust be enabled for players to drown.");
            } else if (message.CaselessEq("grass")) {
                Player.Message(p, "%HSets the block that this block is changed into, when exposed to sunlight");
                Player.Message(p, "%HLeave block blank to remove this behaviour.");
            } else if (message.CaselessEq("dirt")) {
                Player.Message(p, "%HSets the block that this block is changed into, when no longer exposed to sunlight");
                Player.Message(p, "%HLeave block blank to remove this behaviour.");
            }  else {
                Player.Message(p, "&cUnrecognised property \"{0}\"", message);
            }
        }
    }
}
