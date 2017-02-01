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
using MCGalaxy.Commands.Building;

namespace MCGalaxy.Commands.CPE {
    internal static class CustomBlockCommand {
        
        public static void Execute(Player p, string message, bool global, string cmd) {
            string[] parts = message.SplitSpaces(4);
            for (int i = 0; i < Math.Min(parts.Length, 3); i++)
                parts[i] = parts[i].ToLower();
            
            if (message == "") {
                if (GetBD(p, global) != null) {
                    SendStepHelp(p, global);
                } else {
                    Help(p, cmd);
                }
                return;
            }
            
            switch (parts[0]) {
                case "add":
                case "create":
                    AddHandler(p, parts, global, cmd); break;
                case "copy":
                case "clone":
                case "duplicate":
                    CopyHandler(p, parts, global, cmd); break;
                case "delete":
                case "remove":
                    RemoveHandler(p, parts, global, cmd); break;
                case "info":
                case "about":
                    InfoHandler(p, parts, global, cmd); break;
                case "list":
                case "ids":
                    ListHandler(p, parts, global, cmd); break;
                case "abort":
                    Player.Message(p, "Aborted the custom block creation process.");
                    SetBD(p, global, null); break;
                case "edit":
                    EditHandler(p, parts, global, cmd); break;
                default:
                    if (GetBD(p, global) != null)
                        DefineBlockStep(p, message, global, cmd);
                    else
                        Help(p, cmd);
                    break;
            }
        }
        
        static void AddHandler(Player p, string[] parts, bool global, string cmd) {
            int targetId;
            if (parts.Length >= 2 ) {
                string id = parts[1];
                if (!CheckBlockId(p, id, global, out targetId)) return;
                BlockDefinition[] defs = global ? BlockDefinition.GlobalDefs : p.level.CustomBlockDefs;
                BlockDefinition def = defs[targetId];
                
                if (ExistsInScope(def, targetId, global)) {
                    Player.Message(p, "There is already a custom block with the id " + id +
                                   ", you must either use a different id or use \"" + cmd + " remove " + id + "\"");
                    return;
                }
            } else {
                targetId = GetFreeId(global, p == null ? null : p.level);
                if (targetId == Block.Invalid) {
                    Player.Message(p, "There are no custom block ids left, " +
                                   "you must " + cmd +" remove a custom block first.");
                    return;
                }
            }
            
            SetBD(p, global, new BlockDefinition());
            GetBD(p, global).Version2 = true;
            GetBD(p, global).BlockID = (byte)targetId;
            Player.Message(p, "Use %T{0} abort %Sat anytime to abort the creation process.", cmd);
            Player.Message(p, "  Use %T{0} revert %Sto go back a step", cmd);
            Player.Message(p, "  Use %T{0} [input] %Sto provide input", cmd);
            Player.Message(p, "%f----------------------------------------------------------");
            
            SetStep(p, global, 2);
            SendStepHelp(p, global);
        }
        
        static void CopyHandler(Player p, string[] parts, bool global, string cmd) {
            if (parts.Length <= 2) { Help(p, cmd); return; }
            int srcId, dstId;
            if (!CheckBlockId(p, parts[1], global, out srcId)) return;
            if (!CheckBlockId(p, parts[2], global, out dstId)) return;
            BlockDefinition[] defs = global ? BlockDefinition.GlobalDefs : p.level.CustomBlockDefs;
            
            BlockDefinition src = defs[srcId], dst = defs[dstId];
            if (src == null && srcId < Block.CpeCount)
                src = DefaultSet.MakeCustomBlock((byte)srcId);
            if (src == null) { MessageNoBlock(p, srcId, global, cmd); return; }
            if (ExistsInScope(dst, dstId, global)) { MessageAlreadyBlock(p, dstId, global, cmd); return; }
            
            BlockProps props = global ? BlockDefinition.GlobalProps[srcId] : p.level.CustomBlockProps[srcId];
            dst = src.Copy();
            props.BlockId = (byte)dstId;
            dst.BlockID = (byte)dstId;
            
            AddBlock(p, dst, global, cmd, props);
            string scope = global ? "global" : "level";
            Player.Message(p, "Duplicated the {0} custom block with id \"{1}\" to \"{2}\".", scope, srcId, dstId);
        }
        
        static void InfoHandler(Player p, string[] parts, bool global, string cmd) {
            if (parts.Length == 1) { Help(p, cmd); return; }
            int id;
            if (!CheckBlockId(p, parts[1], global, out id)) return;
            
            BlockDefinition[] defs = global ? BlockDefinition.GlobalDefs : p.level.CustomBlockDefs;
            BlockDefinition def = defs[id];
            if (!ExistsInScope(def, id, global)) { MessageNoBlock(p, id, global, cmd); return; }
            
            Player.Message(p, "About {0} ({1})", def.Name, def.BlockID);
            Player.Message(p, "  DrawType: {0}, BlocksLight: {1}, Solidity: {2}", 
                           def.BlockDraw, def.BlocksLight, def.CollideType);
            Player.Message(p, "  Fallback ID: {0}, Sound: {1}, Speed: {2}", 
                           def.FallBack, def.WalkSound, def.Speed.ToString("F2"));
            
            if (def.FogDensity == 0) {
                Player.Message(p, "  Block does not use fog");
            } else {
                Player.Message(p, "  Fog density: {0}, color: {1}",
                               def.FogDensity, Utils.Hex(def.FogR, def.FogG, def.FogB));
            }
            
            if (def.Shape == 0) {
                Player.Message(p, "  Block is a sprite");
            } else {
                Player.Message(p, "  Block is a cube from ({0}, {1}, {2}) to ({3}, {4}, {5})",
                               def.MinX, def.MinZ, def.MinY, def.MaxX, def.MaxZ, def.MaxY);
            }
        }
        
        static void ListHandler(Player p, string[] parts, bool global, string cmd) {
            string modifier = parts.Length > 1 ? parts[1] : "";
            BlockDefinition[] defs = global ? BlockDefinition.GlobalDefs : p.level.CustomBlockDefs;
            List<BlockDefinition> defsInScope = new List<BlockDefinition>();
            
            for( int i = 1; i < 256; i++ ) {
                BlockDefinition def = defs[i];
                if (!ExistsInScope(def, i, global)) continue;
                defsInScope.Add(def);
            }
            MultiPageOutput.Output(p, defsInScope, FormatBlock, cmd.Substring(1) + " list",
                                   "custom blocks", modifier, true);
        }
        
        static string FormatBlock(BlockDefinition def, int i) {
            return "Custom block %T" + def.BlockID + " %Shas name %T" + def.Name;
        }
        
        static void RemoveHandler(Player p, string[] parts, bool global, string cmd) {
            if (parts.Length <= 1) { Help(p, cmd); return; }
            int id;
            if (!CheckBlockId(p, parts[1], global, out id)) return;
            
            BlockDefinition[] defs = global ? BlockDefinition.GlobalDefs : p.level.CustomBlockDefs;
            BlockDefinition def = defs[id];
            if (!ExistsInScope(def, id, global)) { MessageNoBlock(p, id, global, cmd); return; }
            
            RemoveBlockProps(global, (byte)id, p);
            BlockDefinition.Remove(def, defs, p == null ? null : p.level);
            
            BlockDefinition globalDef = BlockDefinition.GlobalDefs[id];
            if (!global && globalDef != null)
                BlockDefinition.Add(globalDef, defs, p.level);
        }
        
        static void DefineBlockStep(Player p, string value, bool global, string cmd) {
            string opt = value.ToLower();
            int step = GetStep(p, global);
            BlockDefinition bd = GetBD(p, global);
            
            if (opt == "revert" && step > 2) {
                if (step == 17 && bd.FogDensity == 0) step -= 2;
                else if (step == 9 && bd.Shape == 0) step -= 5;
                else step--;

                SetStep(p, global, step);
                SendStepHelp(p, global);
                return;
            }

            if (step == 2) {
                bd.Name = value;
                step++;
            } else if (step == 3) {
                if (value == "0" || value == "1") {
                    bd.Shape = value == "1" ? (byte)0 : (byte)16;
                    step++;
                }
            } else if (step == 4) {
                if (byte.TryParse(value, out bd.TopTex)) {
                    step += (bd.Shape == 0 ? 5 : 1); // skip other texture steps for sprites
                    if (bd.Shape == 0) bd.SetAllTex(bd.TopTex);
                }
            } else if (step == 5) {
                if (byte.TryParse(value, out bd.SideTex)) {
                    bd.SetSideTex(bd.SideTex);
                    step++;
                }
            } else if (step == 6) {
                if (byte.TryParse(value, out bd.BottomTex))
                    step++;
            } else if (step == 7) {
                if (ParseCoords(value, ref bd.MinX, ref bd.MinY, ref bd.MinZ))
                    step++;
            } else if (step == 8) {
                if (ParseCoords(value, ref bd.MaxX, ref bd.MaxY, ref bd.MaxZ))
                    step++;
                bd.Shape = bd.MaxY;
            } else if (step == 9) {
                if (value == "0" || value == "1" || value == "2") {
                    bd.CollideType = byte.Parse(value);
                    step++;
                }
            } else if (step == 10) {
                if (Utils.TryParseDecimal(value, out bd.Speed) && bd.Speed >= 0.25f && bd.Speed <= 3.96f)
                    step++;
            } else if (step == 11) {
                if (value == "0" || value == "1") {
                    bd.BlocksLight = value != "0";
                    step++;
                }
            } else if (step == 12) {
                bool result = byte.TryParse(value, out bd.WalkSound);
                if (result && bd.WalkSound <= 11)
                    step++;
            } else if (step == 13) {
                if (value == "0" || value == "1") {
                    bd.FullBright = value != "0";
                    step++;
                }
            } else if (step == 14) {
                bool result = byte.TryParse(value, out bd.BlockDraw);
                if (result && bd.BlockDraw >= 0 && bd.BlockDraw <= 4)
                    step++;
            } else if (step == 15) {
                if (byte.TryParse(value, out bd.FogDensity))
                    step += (bd.FogDensity == 0 ? 2 : 1);
            } else if (step == 16) {
                if (Utils.CheckHex(p, ref value)) {
                    CustomColor rgb = Colors.ParseHex(value);
                    bd.FogR = rgb.R; bd.FogG = rgb.G; bd.FogB = rgb.B;
                    step++;
                }
            } else if (step == 17) {
                byte fallback = GetFallback(p, value);
                if (fallback == Block.Invalid) { SendStepHelp(p, global); return; }
                bd.FallBack = fallback;
                
                if (!AddBlock(p, bd, global, cmd, new BlockProps(bd.BlockID))) return;
                SetBD(p, global, null);
                SetStep(p, global, 0);
                return;
            }
            
            SetStep(p, global, step);
            SendStepHelp(p, global);
        }
        
        static void EditHandler(Player p, string[] parts, bool global, string cmd) {
            if (parts.Length <= 3) {
                if (parts.Length == 1) {
                    Player.Message(p, "Valid properties: name, collide, speed, toptex, alltex, sidetex, " +
                                   "bottomtex, blockslight, sound, fullbright, shape, blockdraw, min, max, " +
                                   "fogdensity, fogcolor, fallback, lefttex, righttex, fronttex, backtex");
                } else {
                    Help(p, cmd);
                }
                return;
            }
            int blockId;
            if (!CheckBlockId(p, parts[1], global, out blockId)) return;
            
            BlockDefinition[] defs = global ? BlockDefinition.GlobalDefs : p.level.CustomBlockDefs;
            BlockDefinition def = defs[blockId];
            if (def == null && blockId < Block.CpeCount) {
                def = DefaultSet.MakeCustomBlock((byte)blockId);
                AddBlock(p, def, global, cmd, new BlockProps((byte)blockId));
            }
            if (!ExistsInScope(def, blockId, global)) { MessageNoBlock(p, blockId, global, cmd); return; }
            
            string value = parts[3], blockName = def.Name;
            float fTemp;
            
            switch (parts[2].ToLower()) {
                case "name":
                    def.Name = value; break;
                    
                case "collide":
                    if( !(value == "0" || value == "1" || value == "2")) {
                        SendEditHelp(p, 9, 0); return;
                    }
                    def.CollideType = byte.Parse(value); break;
                    
                case "speed":
                    if (!Utils.TryParseDecimal(value, out fTemp) || fTemp < 0.25f || fTemp > 3.96f) {
                        SendEditHelp(p, 10, 0); return;
                    }
                    def.Speed = fTemp; break;
                    
                case "top":
                case "toptex":
                    if (!EditByte(p, value, "Top texture", ref def.TopTex)) return;
                    break;
                    
                case "all":
                case "alltex":
                    if (!EditByte(p, value, "All textures", ref def.SideTex)) return;
                    def.SetAllTex(def.SideTex);
                    break;
                    
                case "side":
                case "sidetex":
                    if (!EditByte(p, value, "Side texture", ref def.SideTex)) return;
                    def.SetSideTex(def.SideTex);
                    break;
                    
                case "left":
                case "lefttex":
                    if (!EditByte(p, value, "Left texture", ref def.LeftTex)) return;
                    break;
                    
                case "right":
                case "righttex":
                    if (!EditByte(p, value, "Right texture", ref def.RightTex)) return;
                    break;
                    
                case "front":
                case "fronttex":
                    if (!EditByte(p, value, "Front texture", ref def.FrontTex)) return;
                    break;
                    
                case "back":
                case "backtex":
                    if (!EditByte(p, value, "Back texture", ref def.BackTex)) return;
                    break;
                    
                case "bottom":
                case "bottomtex":
                    if (!EditByte(p, value, "Bottom texture", ref def.BottomTex)) return;
                    break;
                    
                case "light":
                case "blockslight":
                    if( !(value == "0" || value == "1")) {
                        SendEditHelp(p, 11, 0); return;
                    }
                    def.BlocksLight = value != "0";
                    break;
                    
                case "sound":
                case "walksound":
                    if (!EditByte(p, value, "Walk sound", ref def.WalkSound, 12, 1, 0, 11)) return;
                    break;
                    
                case "bright":
                case "fullbright":
                    if( !(value == "0" || value == "1")) {
                        SendEditHelp(p, 13, 0); return;
                    }
                    def.FullBright = value != "0";
                    break;
                    
                case "shape":
                    if( !(value == "0" || value == "1")) {
                        SendEditHelp(p, 3, 0); return;
                    }
                    def.Shape = value == "1" ? (byte)0 : def.MaxZ;
                    break;
                    
                case "draw":
                case "blockdraw":
                    if (!EditByte(p, value, "Block draw", ref def.BlockDraw, 14, 1, 0, 4)) return;
                    break;
                    
                case "min":
                case "mincoords":
                    if (!ParseCoords(value, ref def.MinX, ref def.MinY, ref def.MinZ)) {
                        SendEditHelp(p, 7, 0); return;
                    }
                    
                    break;
                case "max":
                case "maxcoords":
                    if (!ParseCoords(value, ref def.MaxX, ref def.MaxY, ref def.MaxZ)) {
                        SendEditHelp(p, 8, 0); return;
                    }
                    break;
                    
                case "density":
                case "fogdensity":
                    if (!EditByte(p, value, "Fog density", ref def.FogDensity)) return;
                    break;
                    
                case "col":
                case "fogcol":
                case "fogcolor":
                    if (!Utils.CheckHex(p, ref value)) return;
                    CustomColor rgb = Colors.ParseHex(value);
                    def.FogR = rgb.R; def.FogG = rgb.G; def.FogB = rgb.B;
                    break;
                    
                case "fallback":
                case "fallbackid":
                case "fallbackblock":
                    byte fallback = GetFallback(p, value);
                    if (fallback == Block.Invalid) return;
                    def.FallBack = fallback; break;
                default:
                    Player.Message(p, "Unrecognised property: " + parts[2]); return;
            }
            
            Player.Message(p, "Set {0} for {1} to {2}", parts[2], blockName, value);
            BlockDefinition.Add(def, defs, p == null ? null : p.level);
            ReloadMap(p, global);
        }
        
        
        static bool AddBlock(Player p, BlockDefinition bd, bool global, string cmd, BlockProps props) {
            BlockDefinition[] defs = global ? BlockDefinition.GlobalDefs : p.level.CustomBlockDefs;
            BlockDefinition def = defs[bd.BlockID];
            if (!global && def == BlockDefinition.GlobalDefs[bd.BlockID]) def = null;
            
            // in case the list is modified before we finish the command.
            if (def != null) {
                bd.BlockID = GetFreeId(global, p == null ? null : p.level);
                if (bd.BlockID == Block.Invalid) {
                    Player.Message(p, "There are no custom block ids left, " +
                                   "you must " + cmd + " remove a custom block first.");
                    if (!global) {
                        Player.Message(p, "You may also manually specify the same existing id of a global custom block.");
                    }
                    return false;
                }
            }
            
            string scope = global ? "global" : "level";
            Player.Message(p, "Created a new " + scope + " custom block " + bd.Name + "(" + bd.BlockID + ")");
            UpdateBlockProps(global, p, props);
            BlockDefinition.Add(bd, defs, p == null ? null : p.level);
            return true;
        }
        
        static byte GetFallback(Player p, string value) {
            byte extBlock;
            int block = DrawCmd.GetBlock(p, value, out extBlock);
            
            if (block == Block.custom_block) {
                Player.Message(p, "&cCustom blocks cannot be used as fallback blocks.");
                return Block.Invalid;
            }
            if (block >= Block.CpeCount) {
                Player.Message(p, "&cPhysics block cannot be used as fallback blocks.");
                return Block.Invalid;
            }
            if (block == Block.Invalid) {
                Player.Message(p, "&cCannot use 'skip block' as fallback block.");
                return Block.Invalid;
            }
            return (byte)block;
        }
        
        static void ReloadMap(Player p, bool global) {
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player pl in players) {
                if (!pl.hasBlockDefs) continue;
                if (!global && p.level != pl.level) continue;
                if (pl.level == null || !pl.level.HasCustomBlocks) continue;
                if (!pl.outdatedClient) continue;
                
                LevelActions.ReloadMap(p, pl, true);
            }
        }
        
        static byte GetFreeId(bool global, Level lvl) {
            // Start from opposite ends to avoid overlap.
            if (global) {
                BlockDefinition[] defs = BlockDefinition.GlobalDefs;
                for (int i = Block.CpeCount; i < Block.Invalid; i++) {
                    if (defs[i] == null) return (byte)i;
                }
            } else {
                BlockDefinition[] defs = lvl.CustomBlockDefs;
                for (int i = Block.Invalid - 1; i >= Block.CpeCount; i--) {
                    if (defs[i] == null) return (byte)i;
                }
            }
            return Block.Invalid;
        }
        
        static void MessageNoBlock(Player p, int id, bool global, string cmd) {
            string scope = global ? "global" : "level";
            Player.Message(p, "&cThere is no {1} custom block with the id \"{0}\".", id, scope);
            Player.Message(p, "Type \"%T{0} list\" %Sto see a list of {1} custom blocks.", cmd, scope);
        }
        
        static void MessageAlreadyBlock(Player p, int id, bool global, string cmd) {
            string scope = global ? "global" : "level";
            Player.Message(p, "&cThere is already a {1} custom block with the id \"{0}\".", id, scope);
            Player.Message(p, "Type \"%T{0} list\" %Sto see a list of {1} custom blocks.", cmd, scope);
        }
        
        static bool EditByte(Player p, string arg, string propName, ref byte target) {
            return EditByte(p, arg, propName, ref target, -1, 0, 0, 255);
        }
        
        static bool EditByte(Player p, string value, string propName, ref byte target,
                             int step, int offset, byte min, byte max) {
            int temp = 0;
            if (!int.TryParse(value, out temp) || temp < min || temp > max) {
                Player.Message(p, propName + " must be an integer between {0} and {1}.", min, max);
                if (step != -1) SendEditHelp(p, step, offset);
                return false;
            }
            target = (byte)temp;
            return true;
        }
        
        static bool ParseCoords(string parts, ref byte x, ref byte y, ref byte z) {
            string[] coords = parts.Split(' ');
            if (coords.Length != 3) return false;
            
            byte tx = 0, ty = 0, tz = 0;
            if (!byte.TryParse(coords[0], out tx) || !byte.TryParse(coords[1], out ty)
                || !byte.TryParse(coords[2], out tz)) return false;
            if (tx > 16 || ty > 16 || tz > 16) return false;
            
            x = tx; z = ty; y = tz; // blockdef files have z being height, we use y being height
            return true;
        }
        
        static bool CheckBlockId(Player p, string arg, bool global, out int blockId) {
            if (!int.TryParse(arg, out blockId)) {
                Player.Message(p, "&cProvided block id is not a number."); return false;
            }
            if (blockId <= 0 || blockId >= Block.Invalid) {
                Player.Message(p, "&cBlock id must be between 1-254"); return false;
            }
            return true;
        }
        
        
        static void UpdateBlockProps(bool global, Player p, BlockProps props) {
            byte id = props.BlockId;
            if (!global) {
                p.level.CustomBlockProps[id] = props;
            } else {
                BlockDefinition.GlobalProps[id] = props;
                Level[] loaded = LevelInfo.Loaded.Items;
                
                foreach (Level lvl in loaded) {
                    if (lvl.CustomBlockDefs[id] != null) continue;
                    lvl.CustomBlockProps[id] = props;
                }
            }
        }
        
        static void RemoveBlockProps(bool global, byte id, Player p) {
            // Level block reverts to using global block
            if (!global && BlockDefinition.GlobalDefs[id] != null) {
                p.level.CustomBlockProps[id] = BlockDefinition.GlobalProps[id];
            } else if (!global) {
                p.level.CustomBlockProps[id] = new BlockProps((byte)id);
            } else {
                BlockDefinition.GlobalProps[id] = new BlockProps((byte)id);
                Level[] loaded = LevelInfo.Loaded.Items;
                
                foreach (Level lvl in loaded) {
                    if (lvl.CustomBlockDefs[id] != BlockDefinition.GlobalDefs[id]) continue;
                    lvl.CustomBlockProps[id] = new BlockProps((byte)id);
                }
            }
        }
        
        
        static BlockDefinition consoleBD;
        static int consoleStep;
        
        static BlockDefinition GetBD(Player p, bool global) {
            return p == null ? consoleBD : (global ? p.gbBlock : p.lbBlock);
        }
        
        static int GetStep(Player p, bool global) {
            return p == null ? consoleStep : (global ? p.gbStep : p.lbStep);
        }
        
        static void SetBD(Player p, bool global, BlockDefinition bd) {
            if (p == null) consoleBD = bd;
            else if (global) p.gbBlock = bd;
            else p.lbBlock = bd;
        }
        
        static void SetStep(Player p, bool global, int step) {
            if (p == null) consoleStep = step;
            else if (global) p.gbStep = step;
            else p.lbStep = step;
        }
                        
        static bool ExistsInScope(BlockDefinition def, int i, bool global) {
            return def != null && (global ? true : def != BlockDefinition.GlobalDefs[i]);
        }
        
        
        static void SendStepHelp(Player p, bool global) {
            int step = GetStep(p, global);
            string[] help = stepsHelp[step];
            
            BlockDefinition bd = GetBD(p, global);
            if (step == 4 && bd.Shape == 0)
                help[0] = help[0].Replace("top texture", "texture");
            
            for (int i = 0; i < help.Length; i++)
                Player.Message(p, help[i]);
            Player.Message(p, "%f--------------------------");
        }
        
        static void SendEditHelp(Player p, int step, int offset) {
            string[] help = stepsHelp[step];
            for (int i = offset; i < help.Length; i++)
                Player.Message(p, help[i].Replace("Type", "Use"));
        }
        
        static string[][] stepsHelp = new string[][] {
            null, // step 0
            null, // step 1
            new[] { "Type the name for the block." },
            new[] { "Type '0' if the block is a cube, '1' if a sprite (e.g roses)." },
            
            new[] { "Type a number between '0' and '255' for the top texture.",
                "Textures in terrain.png are numbered from left to right, increasing downwards",
            },
            new[] { "Type a number between '0' and '255' for the sides texture.",
                "Textures in terrain.png are numbered from left to right, increasing downwards.",
            },
            new[] { "Type a number between '0' and '255' for the bottom texture.",
                "Textures in terrain.png are numbered from left to right, increasing downwards.",
            },
            
            new[] { "Enter the three minimum coordinates of the cube in units (separated by spaces). 1 block = 16 units.",
                "Minimum coordinates for a normal block are &40 &20 &10." },
            new[] { "Enter the three maximum coordinates of the cube in units (separated by spaces). 1 block = 16 units.",
                "Maximum coordinates for a normal block are &416 &216 &116." },
            
            new[] { "Type '0' if the block is walk-through.", "Type '1' if the block is swim-through.",
                "Type '2' if the block is solid.",
            },
            new[] { "Type a number between '0.25' (25% speed) and '3.96' (396% speed).",
                "This speed is used when inside or walking on the block. Default speed is 1",
            },
            new[] { "Type '1' if the block casts a shadow, '0' if it doesn't" },
            new[] { "Type a number between '0' and '9' for the sound played when walking on it and breaking.",
                "0 = None, 1 = Wood, 2 = Gravel, 3 = Grass, 4 = Stone",
                "5 = Metal, 6 = Glass, 7 = Cloth, 8 = Sand, 9 = Snow",
            },
            new[] { "Type '1' if the block is fully lit (e.g. lava), '0' if not." },
            new[] { "Enter the block's draw method.", "0 = Opaque, 1 = Transparent (Like glass)",
                "2 = Transparent (Like leaves), 3 = Translucent (Like ice), 4 = Gas (Like air)",
            },

            new[] { "Enter the fog density for the block. 0 = No fog at all",
                "1 - 255 = Increasing density (e.g. water has 12, lava 255)",
            },
            new[] { "Enter the fog color (hex color)", },
            new[] { "Enter the fallback block (Block shown to players who can't see custom blocks).",
                "You can use any block name or block ID from the normal blocks.",
            },
        };
        
        internal static void Help(Player p, string cmd) {
            // TODO: find a nicer way of doing this
            string fullCmd = cmd.Replace("lb", "levelblock")
                .Replace("gb", "globalblock");
            
            Player.Message(p, "%T{0} add/copy/edit/list/remove", fullCmd);
            Player.Message(p, "%H  {0} add [id] - begins creating a new custom block.", cmd);
            Player.Message(p, "%H  {0} copy [source id] [new id] - clones a new custom block from an existing custom block.", cmd);
            Player.Message(p, "%H  {0} edit [id] [property] [value] - edits the given property of that custom block.", cmd);
            Player.Message(p, "%H  {0} list <offset> - lists all custom blocks.", cmd);
            Player.Message(p, "%H  {0} remove [id] - removes that custom block.", cmd);
            Player.Message(p, "%H  {0} info [id] - shows info about that custom block.", cmd);
            Player.Message(p, "%HTo see the list of editable properties, type {0} edit.", cmd);
        }
    }
    
    public sealed class CmdGlobalBlock : Command {
        
        public override string name { get { return "globalblock"; } }
        public override string shortcut { get { return "gb"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        public CmdGlobalBlock() { }

        public override void Use(Player p, string message) {
            CustomBlockCommand.Execute(p, message, true, "/gb");
        }
        
        public override void Help(Player p) {
            CustomBlockCommand.Help(p, "/gb");
        }
    }
    
    public sealed class CmdLevelBlock : Command {
        
        public override string name { get { return "levelblock"; } }
        public override string shortcut { get { return "lb"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        public CmdLevelBlock() { }

        public override void Use(Player p, string message) {
            if (p == null) { MessageInGameOnly(p); return; }
            CustomBlockCommand.Execute(p, message, false, "/lb");
        }
        
        public override void Help(Player p) {
            CustomBlockCommand.Help(p, "/lb");
        }
    }
}