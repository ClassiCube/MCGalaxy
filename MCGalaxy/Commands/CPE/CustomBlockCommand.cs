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
            
            if (message.Length == 0) {
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
            ushort target;
            if (parts.Length >= 2 ) {
                string id = parts[1];
                if (!CheckBlock(p, id, out target)) return;
                BlockDefinition[] defs = global ? BlockDefinition.GlobalDefs : p.level.CustomBlockDefs;
                BlockDefinition def = defs[target.RawID];
                
                if (ExistsInScope(def, target, global)) {
                    Player.Message(p, "There is already a custom block with the id " + id +
                                   ", you must either use a different id or use \"" + cmd + " remove " + id + "\"");
                    return;
                }
            } else {
                target = GetFreeBlock(global, p == null ? null : p.level);
                if (target == Block.Invalid) {
                    Player.Message(p, "There are no custom block ids left, " +
                                   "you must " + cmd +" remove a custom block first.");
                    return;
                }
            }
            
            SetBD(p, global, new BlockDefinition());
            GetBD(p, global).Version2 = true;
            GetBD(p, global).BlockID = target.RawID;
            Player.Message(p, "Use %T{0} abort %Sat anytime to abort the creation process.", cmd);
            Player.Message(p, "  Use %T{0} revert %Sto go back a step", cmd);
            Player.Message(p, "  Use %T{0} [input] %Sto provide input", cmd);
            Player.Message(p, "%f----------------------------------------------------------");
            
            SetStep(p, global, 2);
            SendStepHelp(p, global);
        }
        
        static void CopyHandler(Player p, string[] parts, bool global, string cmd) {
            if (parts.Length <= 2) { Help(p, cmd); return; }
            ushort src, dst;
            if (!CheckBlock(p, parts[1], out src, true)) return;
            if (!CheckBlock(p, parts[2], out dst)) return;
            BlockDefinition[] defs = global ? BlockDefinition.GlobalDefs : p.level.CustomBlockDefs;
            
            BlockDefinition srcDef = defs[src.RawID], dstDef = defs[dst.RawID];
            if (srcDef == null && src.BlockID < Block.CpeCount) {
                srcDef = DefaultSet.MakeCustomBlock(src.BlockID);
            }
            if (srcDef == null) { MessageNoBlock(p, src, global, cmd); return; }
            if (ExistsInScope(dstDef, dst, global)) { MessageAlreadyBlock(p, dst, global, cmd); return; }
            
            BlockProps props = global ? BlockDefinition.GlobalProps[src.RawID] : p.level.Props[src];
            dstDef = srcDef.Copy();
            dstDef.BlockID = (byte)dst.RawID;
            dstDef.InventoryOrder = -1;
            
            AddBlock(p, dstDef, global, cmd, props);
            string scope = global ? "global" : "level";
            Player.Message(p, "Duplicated the {0} custom block with id \"{1}\" to \"{2}\".", scope, src.RawID, dst.RawID);
        }
        
        static void InfoHandler(Player p, string[] parts, bool global, string cmd) {
            if (parts.Length == 1) { Help(p, cmd); return; }
            ushort block;
            if (!CheckBlock(p, parts[1], out block)) return;
            
            BlockDefinition[] defs = global ? BlockDefinition.GlobalDefs : p.level.CustomBlockDefs;
            BlockDefinition def = defs[block.RawID];
            if (!ExistsInScope(def, block, global)) { MessageNoBlock(p, block, global, cmd); return; }
            
            Player.Message(p, "About {0} ({1})", def.Name, def.BlockID);
            Player.Message(p, "  Draw type: {0}, Blocks light: {1}, collide type: {2}",
                           def.BlockDraw, def.BlocksLight, def.CollideType);
            Player.Message(p, "  Fallback ID: {0}, Sound: {1}, Speed: {2}",
                           def.FallBack, def.WalkSound, def.Speed.ToString("F2"));
            
            if (def.FogDensity == 0) {
                Player.Message(p, "  Block does not use fog");
            } else {
                Player.Message(p, "  Fog density: {0}, color: {1}",
                               def.FogDensity, Utils.Hex(def.FogR, def.FogG, def.FogB));
            }
            
            bool tinted = (def.FogR != 0 || def.FogG != 0 || def.FogB != 0) && def.Name.IndexOf('#') >= 0;
            if (tinted) {
                Player.Message(p, "  Tint color: {0}", Utils.Hex(def.FogR, def.FogG, def.FogB));
            }
            
            if (def.Shape == 0) {
                Player.Message(p, "  Block is a sprite");
                Player.Message(p, "  Texture ID: {0}", def.SideTex);
            } else {
                Player.Message(p, "  Block is a cube from ({0}, {1}, {2}) to ({3}, {4}, {5})",
                               def.MinX, def.MinZ, def.MinY, def.MaxX, def.MaxZ, def.MaxY);
                Player.Message(p, "  Texture IDs (left: {0}, right: {1}, front: {2}, back: {3}, top: {4}, bottom: {5})",
                               def.LeftTex, def.RightTex, def.FrontTex, def.BackTex, def.TopTex, def.BottomTex);
            }
        }
        
        static void ListHandler(Player p, string[] parts, bool global, string cmd) {
            string modifier = parts.Length > 1 ? parts[1] : "";
            BlockDefinition[] defs = global ? BlockDefinition.GlobalDefs : p.level.CustomBlockDefs;
            List<BlockDefinition> defsInScope = new List<BlockDefinition>();
            
            for (int i = 0; i < Block.Count; i++) {
                BlockDefinition def = defs[i];
                ushort block = Block.FromRaw((byte)i);
                
                if (!ExistsInScope(def, block, global)) continue;
                defsInScope.Add(def);
            }
            MultiPageOutput.Output(p, defsInScope, FormatBlock, cmd.Substring(1) + " list",
                                   "custom blocks", modifier, true);
        }
        
        static string FormatBlock(BlockDefinition def) {
            return "Custom block %T" + def.BlockID + " %Shas name %T" + def.Name;
        }
        
        static void RemoveHandler(Player p, string[] parts, bool global, string cmd) {
            if (parts.Length <= 1) { Help(p, cmd); return; }
            ushort block;
            if (!CheckBlock(p, parts[1], out block)) return;
            
            BlockDefinition[] defs = global ? BlockDefinition.GlobalDefs : p.level.CustomBlockDefs;
            BlockDefinition def = defs[block.RawID];
            if (!ExistsInScope(def, block, global)) { MessageNoBlock(p, block, global, cmd); return; }
            
            RemoveBlockProps(global, block, p);
            BlockDefinition.Remove(def, defs, p == null ? null : p.level);
            
            string scope = global ? "global" : "level";
            Player.Message(p, "Removed " + scope + " custom block " + def.Name + "(" + def.BlockID + ")");
            
            BlockDefinition globalDef = BlockDefinition.GlobalDefs[block.RawID];
            if (!global && globalDef != null)
                BlockDefinition.Add(globalDef, defs, p.level);
        }
        
        static void DefineBlockStep(Player p, string value, bool global, string cmd) {
            string opt = value.ToLower();
            int step = GetStep(p, global);
            BlockDefinition bd = GetBD(p, global);
            bool temp = false;
            
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
                if (CommandParser.GetBool(p, value, ref temp)) {
                    bd.Shape = temp ? (byte)0 : (byte)16;
                    step++;
                }
            } else if (step == 4) {
                if (CommandParser.GetByte(p, value, "Texture ID", ref bd.TopTex)) {
                    step += (bd.Shape == 0 ? 5 : 1); // skip other texture steps for sprites
                    if (bd.Shape == 0) bd.SetAllTex(bd.TopTex);
                }
            } else if (step == 5) {
                if (CommandParser.GetByte(p, value, "Texture ID", ref bd.SideTex)) {
                    bd.SetSideTex(bd.SideTex);
                    step++;
                }
            } else if (step == 6) {
                if (CommandParser.GetByte(p, value, "Texture ID", ref bd.BottomTex))
                    step++;
            } else if (step == 7) {
                if (ParseCoords(p, value, ref bd.MinX, ref bd.MinY, ref bd.MinZ))
                    step++;
            } else if (step == 8) {
                if (ParseCoords(p, value, ref bd.MaxX, ref bd.MaxY, ref bd.MaxZ))
                    step++;
                bd.Shape = bd.MaxY;
            } else if (step == 9) {
                if (CommandParser.GetByte(p, value, "Collide type", ref bd.CollideType, 0, 6))
                    step++;
            } else if (step == 10) {
                if (Utils.TryParseDecimal(value, out bd.Speed) && bd.Speed >= 0.25f && bd.Speed <= 3.96f)
                    step++;
            } else if (step == 11) {
                if (CommandParser.GetBool(p, value, ref temp)) {
                    bd.BlocksLight = temp;
                    step++;
                }
            } else if (step == 12) {
                if (CommandParser.GetByte(p, value, "Walk sound", ref bd.WalkSound, 0, 11))
                    step++;
            } else if (step == 13) {
                if (CommandParser.GetBool(p, value, ref bd.FullBright))
                    step++;
            } else if (step == 14) {
                if (CommandParser.GetByte(p, value, "Block draw", ref bd.BlockDraw, 0, 4))
                    step++;
            } else if (step == 15) {
                if (CommandParser.GetByte(p, value, "Fog density", ref bd.FogDensity)) {
                    step += (bd.FogDensity == 0 ? 2 : 1);
                }
            } else if (step == 16) {
                ColorDesc rgb = default(ColorDesc);
                if (CommandParser.GetHex(p, value, ref rgb)) {
                    bd.FogR = rgb.R; bd.FogG = rgb.G; bd.FogB = rgb.B;
                    step++;
                }
            } else if (step == 17) {
                byte fallback = GetFallback(p, value);
                if (fallback == Block.Invalid) { SendStepHelp(p, global); return; }
                bd.FallBack = fallback;
                
                ushort block = Block.FromRaw(bd.BlockID);
                BlockProps props = BlockDefinition.DefaultProps(block);
                if (!AddBlock(p, bd, global, cmd, props)) return;
                
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
                    Player.Message(p, "Valid properties: " + helpSections.Keys.Join());
                } else if (parts.Length == 3) {
                    Help(p, cmd, "edit " + parts[2]);
                } else {
                    Help(p, cmd);
                }
                return;
            }
            
            ushort block;
            if (!CheckBlock(p, parts[1], out block)) return;
            BlockDefinition[] defs = global ? BlockDefinition.GlobalDefs : p.level.CustomBlockDefs;
            BlockDefinition def = defs[block.RawID], globalDef = BlockDefinition.GlobalDefs[block.RawID];
            
            if (def == null && block.BlockID < Block.CpeCount) {
                def = DefaultSet.MakeCustomBlock(block.BlockID);
                AddBlock(p, def, global, cmd, BlockDefinition.DefaultProps(block));
            }
            if (def != null && !global && def == globalDef) {
                def = globalDef.Copy();
                AddBlock(p, def, global, cmd, BlockDefinition.DefaultProps(block));
            }
            if (!ExistsInScope(def, block, global)) { MessageNoBlock(p, block, global, cmd); return; }
            
            string value = parts[3], blockName = def.Name;
            float fTemp;
            bool temp = false, changedFallback = false;
            Level level = p == null ? null : p.level;
            
            string arg = MapPropertyName(parts[2].ToLower());
            switch (arg) {
                case "name":
                    def.Name = value; break;
                case "collide":
                    if (!EditByte(p, value, "Collide type", ref def.CollideType, arg)) return;
                    break;
                case "speed":
                    if (!Utils.TryParseDecimal(value, out fTemp) || fTemp < 0.25f || fTemp > 3.96f) {
                        SendEditHelp(p, arg); return;
                    }
                    def.Speed = fTemp; break;
                    
                case "toptex":
                    if (!EditByte(p, value, "Top texture", ref def.TopTex, arg)) return;
                    break;
                case "alltex":
                    if (!EditByte(p, value, "All textures", ref def.SideTex, arg)) return;
                    def.SetAllTex(def.SideTex);
                    break;
                case "sidetex":
                    if (!EditByte(p, value, "Side texture", ref def.SideTex, arg)) return;
                    def.SetSideTex(def.SideTex);
                    break;
                case "lefttex":
                    if (!EditByte(p, value, "Left texture", ref def.LeftTex, arg)) return;
                    break;
                case "righttex":
                    if (!EditByte(p, value, "Right texture", ref def.RightTex, arg)) return;
                    break;
                case "fronttex":
                    if (!EditByte(p, value, "Front texture", ref def.FrontTex, arg)) return;
                    break;
                case "backtex":
                    if (!EditByte(p, value, "Back texture", ref def.BackTex, arg)) return;
                    break;
                case "bottomtex":
                    if (!EditByte(p, value, "Bottom texture", ref def.BottomTex, arg)) return;
                    break;
                    
                case "blockslight":
                    if (!CommandParser.GetBool(p, value, ref temp)) {
                        SendEditHelp(p, arg); return;
                    }
                    def.BlocksLight = temp;
                    break;
                case "sound":
                    if (!EditByte(p, value, "Walk sound", ref def.WalkSound, arg)) return;
                    break;
                case "fullbright":
                    if (!CommandParser.GetBool(p, value, ref temp)) {
                        SendEditHelp(p, arg); return;
                    }
                    def.FullBright = temp;
                    break;
                    
                case "shape":
                    if (!CommandParser.GetBool(p, value, ref temp)) {
                        SendEditHelp(p, arg); return;
                    }
                    def.Shape = temp ? (byte)0 : def.MaxZ;
                    break;
                case "blockdraw":
                    if (!EditByte(p, value, "Block draw", ref def.BlockDraw, arg)) return;
                    break;
                case "min":
                    if (!ParseCoords(p, value, ref def.MinX, ref def.MinY, ref def.MinZ)) {
                        SendEditHelp(p, arg); return;
                    } break;
                case "max":
                    if (!ParseCoords(p, value, ref def.MaxX, ref def.MaxY, ref def.MaxZ)) {
                        SendEditHelp(p, arg); return;
                    } break;
                    
                case "fogdensity":
                    if (!EditByte(p, value, "Fog density", ref def.FogDensity, arg)) return;
                    break;
                case "fogcolor":
                    ColorDesc rgb = default(ColorDesc);
                    if (!CommandParser.GetHex(p, value, ref rgb)) return;
                    def.FogR = rgb.R; def.FogG = rgb.G; def.FogB = rgb.B;
                    break;
                case "fallback":
                    byte fallback = GetFallback(p, value);
                    if (fallback == Block.Invalid) return;
                    changedFallback = true;
                    
                    value = Block.Name(fallback);
                    def.FallBack = fallback; break;
                    
                case "order":
                    int order = 0;
                    if (!CommandParser.GetInt(p, value, "Inventory order", ref order, 1, 255)) {
                        SendEditHelp(p, arg); return;
                    }
                    
                    def.InventoryOrder = order == def.BlockID ? -1 : order;
                    BlockDefinition.UpdateOrder(def, global, level);
                    BlockDefinition.Save(global, level);
                    Player.Message(p, "Set inventory order for {0} to {1}", blockName, 
                                   order == def.BlockID ? "default" : order.ToString());
                    return;
                default:
                    Player.Message(p, "Unrecognised property: " + arg); return;
            }
            
            Player.Message(p, "Set {0} for {1} to {2}", arg, blockName, value);
            BlockDefinition.Add(def, defs, level);
            if (changedFallback) {
                BlockDefinition.UpdateFallback(global, def.BlockID, level);
            }
        }
        
        
        static bool AddBlock(Player p, BlockDefinition def, bool global, string cmd, BlockProps props) {
            BlockDefinition[] defs = global ? BlockDefinition.GlobalDefs : p.level.CustomBlockDefs;
            BlockDefinition old = defs[def.BlockID];
            if (!global && old == BlockDefinition.GlobalDefs[def.BlockID]) old = null;
            ushort block;
            
            // in case the list is modified before we finish the command.
            if (old != null) {
                block = GetFreeBlock(global, p == null ? null : p.level);
                if (block == Block.Invalid) {
                    Player.Message(p, "There are no custom block ids left, " +
                                   "you must " + cmd + " remove a custom block first.");
                    if (!global) {
                        Player.Message(p, "You may also manually specify the same existing id of a global custom block.");
                    }
                    return false;
                }
                def.BlockID = block.RawID;
            }
            
            string scope = global ? "global" : "level";
            Player.Message(p, "Created a new " + scope + " custom block " + def.Name + "(" + def.BlockID + ")");
            
            block = Block.FromRaw(def.BlockID);
            BlockDefinition.Add(def, defs, p == null ? null : p.level);
            UpdateBlockProps(global, p, block, props);
            return true;
        }
        
        static byte GetFallback(Player p, string value) {
            ushort block;
            if (!CommandParser.GetBlock(p, value, out block)) return Block.Invalid;
            
            if (block.BlockID == Block.custom_block) {
                Player.Message(p, "&cCustom blocks cannot be used as fallback blocks.");
                return Block.Invalid;
            }
            if (block.IsPhysicsType) {
                Player.Message(p, "&cPhysics block cannot be used as fallback blocks.");
                return Block.Invalid;
            }
            return block.BlockID;
        }
        
        
        static ushort GetFreeBlock(bool global, Level lvl) {
            // Start from opposite ends to avoid overlap.
            if (global) {
                BlockDefinition[] defs = BlockDefinition.GlobalDefs;
                for (int i = Block.CpeCount; i < Block.Invalid; i++) {
                    if (defs[i] == null) return Block.FromRaw((byte)i);
                }
            } else {
                BlockDefinition[] defs = lvl.CustomBlockDefs;
                for (int i = Block.Invalid - 1; i >= Block.CpeCount; i--) {
                    if (defs[i] == null) return Block.FromRaw((byte)i);
                }
            }
            return Block.Invalid;
        }
        
        static void MessageNoBlock(Player p, ushort block, bool global, string cmd) {
            string scope = global ? "global" : "level";
            Player.Message(p, "&cThere is no {1} custom block with the id \"{0}\".", block.RawID, scope);
            Player.Message(p, "Type \"%T{0} list\" %Sto see a list of {1} custom blocks.", cmd, scope);
        }
        
        static void MessageAlreadyBlock(Player p, ushort block, bool global, string cmd) {
            string scope = global ? "global" : "level";
            Player.Message(p, "&cThere is already a {1} custom block with the id \"{0}\".", block.RawID, scope);
            Player.Message(p, "Type \"%T{0} list\" %Sto see a list of {1} custom blocks.", cmd, scope);
        }
        
        static bool EditByte(Player p, string value, string propName, ref byte target, string help) {
            byte temp = 0;
            if (!CommandParser.GetByte(p, value, propName, ref temp, 0, 255)) {
                SendEditHelp(p, help);
                return false;
            }
            target = temp; return true;
        }
        
        static bool ParseCoords(Player p, string parts, ref byte x, ref byte y, ref byte z) {
            string[] coords = parts.SplitSpaces();
            if (coords.Length != 3) return false;
            
            byte tx = 0, ty = 0, tz = 0;
            if (!CommandParser.GetByte(p, coords[0], "X", ref tx, 0, 16)) return false;
            if (!CommandParser.GetByte(p, coords[1], "Y", ref ty, 0, 16)) return false;
            if (!CommandParser.GetByte(p, coords[2], "Z", ref tz, 0, 16)) return false;
            
            x = tx; z = ty; y = tz; // blockdef files have z being height, we use y being height
            return true;
        }
        
        static bool CheckBlock(Player p, string arg, out ushort block, bool allowAir = false) {
            block = Block.Invalid;
            int raw = 0;
            int min = allowAir ? 0 : 1;
            bool success = CommandParser.GetInt(p, arg, "Block ID", ref raw, min, 255);
            
            block = Block.FromRaw((byte)raw);
            return success;
        }
        
        
        static void UpdateBlockProps(bool global, Player p, ushort block, BlockProps props) {
            if (!global) {
                p.level.Props[block] = props;
                p.level.UpdateBlockHandler(block);
                return;
            }
            
            BlockDefinition.GlobalProps[block.RawID] = props;
            Level[] loaded = LevelInfo.Loaded.Items;
            byte raw = block.RawID;
            
            foreach (Level lvl in loaded) {
                if (lvl.CustomBlockDefs[raw] != BlockDefinition.GlobalDefs[raw]) continue;
                lvl.Props[block] = props;
                lvl.UpdateBlockHandler(block);
            }
        }
        
        static void RemoveBlockProps(bool global, ushort block, Player p) {
            // Level block reverts to using global block
            if (!global) {
                p.level.Props[block] = BlockDefinition.DefaultProps(block);
                p.level.UpdateBlockHandler(block);
                return;
            }
            
            BlockProps props = BlockProps.MakeDefault();
            if (!block.IsCustomType) props = Block.Props[block.RawID];
            
            BlockDefinition.GlobalProps[block.RawID] = props;
            Level[] loaded = LevelInfo.Loaded.Items;
            byte raw = block.RawID;
            
            foreach (Level lvl in loaded) {
                if (lvl.CustomBlockDefs[raw] != BlockDefinition.GlobalDefs[raw]) continue;
                lvl.Props[block] = BlockDefinition.GlobalProps[block.RawID];
                lvl.UpdateBlockHandler(block);
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
        
        static bool ExistsInScope(BlockDefinition def, ushort block, bool global) {
            return def != null && (global ? true : def != BlockDefinition.GlobalDefs[block.RawID]);
        }
        
        
        static void SendStepHelp(Player p, bool global) {
            int step = GetStep(p, global);
            string[] help = helpSections[stepsHelp[step]];
            
            BlockDefinition bd = GetBD(p, global);
            if (step == 4 && bd.Shape == 0)
                help[0] = help[0].Replace("top texture", "texture");
            
            for (int i = 0; i < help.Length; i++)
                Player.Message(p, help[i]);
            Player.Message(p, "%f--------------------------");
        }
        
        static void SendEditHelp(Player p, string section) {
            string[] help = helpSections[section];
            for (int i = 0; i < help.Length; i++)
                Player.Message(p, help[i].Replace("Type", "Use"));
        }
        
        static string MapPropertyName(string prop) {
            if (prop == "side" || prop == "all" || prop == "top" || prop == "bottom"
                || prop == "left" || prop == "right" || prop == "front" || prop == "back") return prop + "tex";
            
            if (prop == "sides" || prop == "sidestex") return "sidetex";
            if (prop == "light") return "blockslight";
            if (prop == "bright") return "fullbright";
            if (prop == "walksound") return "sound";
            if (prop == "draw") return "blockdraw";
            if (prop == "mincoords") return "min";
            if (prop == "maxcoords") return "max";
            if (prop == "density") return "fogdensity";
            if (prop == "col" || prop == "fogcol")  return "fogcolor";
            if (prop == "fallbackid" || prop == "fallbackblock") return "fallback";
            
            return prop;
        }
        
        
        static string[] stepsHelp = new string[] {
            null, null, "name", "shape", "toptex", "sidetex", "bottomtex", "min", "max", "collide",
            "speed", "blockslight", "sound", "fullbright", "blockdraw", "fogdensity", "fogcolor", "fallback" };
        
        const string texLine = "Press F10 to see the numbers for each texture in terrain.png";
        static Dictionary<string, string[]> helpSections = new Dictionary<string, string[]>() {
            { "name", new string[] { "Type the name for the block." } },
            { "shape", new string[] { "Type '0' if the block is a cube, '1' if a sprite (e.g roses)." } },
            { "blockslight", new string[] { "Type 'yes' if the block casts a shadow, 'no' if it doesn't." } },
            { "fullbright", new string[] { "Type 'yes' if the block is fully lit (e.g. lava), 'no' if not." } },
            
            { "alltex", new string[] { "Type a number between '0' and '255' for all textures.", texLine } },
            { "sidetex", new string[] { "Type a number between '0' and '255' for sides texture.", texLine } },
            { "lefttex", new string[] { "Type a number between '0' and '255' for the left side texture.", texLine } },
            { "righttex", new string[] { "Type a number between '0' and '255' for the right side texture.", texLine } },
            { "fronttex", new string[] { "Type a number between '0' and '255' for the front side texture.", texLine } },
            { "backtex", new string[] { "Type a number between '0' and '255' for the back side texture.", texLine } },
            { "toptex", new string[] { "Type a number between '0' and '255' for the top texture.", texLine } },
            { "bottomtex", new string[] { "Type a number between '0' and '255' for the bottom texture.", texLine } },
            
            { "min", new string[] { "Enter the three minimum coordinates of the cube in units (separated by spaces). 1 block = 16 units.",
                    "Minimum coordinates for a normal block are &40 &20 &10." } },
            { "max", new string[] { "Enter the three maximum coordinates of the cube in units (separated by spaces). 1 block = 16 units.",
                    "Maximum coordinates for a normal block are &416 &216 &116." } },
            { "collide", new string[] { "Type a number between '0' and '6' for collision type.",
                    "0 - block is walk-through (e.g. air).", "1 - block is swim-through/climbable (e.g. rope).",
                    "2 - block is solid (e.g. dirt).", "3 - block is solid, but slippery like ice",
                    "4 - block is solid, but even slipperier than ice", "5 - block is swim-through like water",
                    "6 - block is swim-through like lava" } },
            { "speed", new string[] { "Type a number between '0.25' (25% speed) and '3.96' (396% speed).",
                    "This speed is used when inside or walking on the block. Default speed is 1" }
            },
            { "sound", new string[] { "Type a number between '0' and '9' for the sound played when walking on it and breaking.",
                    "0 = None, 1 = Wood, 2 = Gravel, 3 = Grass, 4 = Stone",
                    "5 = Metal, 6 = Glass, 7 = Cloth, 8 = Sand, 9 = Snow" }
            },
            { "blockdraw", new string[] { "Enter the block's draw method.", "0 = Opaque, 1 = Transparent (Like glass)",
                    "2 = Transparent (Like leaves), 3 = Translucent (Like ice), 4 = Gas (Like air)" }
            },
            { "fogdensity", new string[] { "Enter the fog density for the block. 0 = No fog at all",
                    "1 - 255 = Increasing density (e.g. water has 12, lava 255)" }
            },
            { "fogcolor", new string[] { "Enter the fog color (hex color)" } },
            { "fallback", new string[] { "Enter the fallback block (Block shown to players who can't see custom blocks).",
                    "You can use any block name or block ID from the normal blocks." }
            },
            { "order", new string[] { "Enter the position/order of this block in the inventory.",
                    "The default position of a block is its ID.",
                    "A position of 255 hides the block from the inventory." }
            },
        };
        
        
        internal static void Help(Player p, string cmd) {
            Player.Message(p, "%T{0} add [id] %H- begins creating a new custom block.", cmd);
            Player.Message(p, "%T{0} copy [source id] [new id] %H- clones a new custom block from an existing custom block.", cmd);
            Player.Message(p, "%T{0} edit [id] [property] [value] %H- edits the given property of that custom block.", cmd);
            Player.Message(p, "%T{0} list <offset> %H- lists all custom blocks.", cmd);
            Player.Message(p, "%T{0} remove [id] %H- removes that custom block.", cmd);
            Player.Message(p, "%T{0} info [id] %H- shows info about that custom block.", cmd);
            Player.Message(p, "%HTo see the list of editable properties, type {0} edit.", cmd);
        }
        
        internal static void Help(Player p, string cmd, string args) {
            if (!args.CaselessStarts("edit ")) { Help(p, cmd); return; }            
            string prop = args.Substring(args.IndexOf(' ') + 1);
            prop = MapPropertyName(prop.ToLower());

            if (!helpSections.ContainsKey(prop)) {
                Player.Message(p, "Valid properties: " + helpSections.Keys.Join());
            } else {
                SendEditHelp(p, prop);
            }
        }
    }
    
    public sealed class CmdGlobalBlock : Command {
        public override string name { get { return "GlobalBlock"; } }
        public override string shortcut { get { return "gb"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }

        public override void Use(Player p, string message) {
            CustomBlockCommand.Execute(p, message, true, "/gb");
        }
        
        public override void Help(Player p) {
            CustomBlockCommand.Help(p, "/gb");
        }
        
        public override void Help(Player p, string message) {
            CustomBlockCommand.Help(p, "/gb", message);
        }
    }
    
    public sealed class CmdLevelBlock : Command {
        public override string name { get { return "LevelBlock"; } }
        public override string shortcut { get { return "lb"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        public override bool SuperUseable { get { return false; } }

        public override void Use(Player p, string message) {
            CustomBlockCommand.Execute(p, message, false, "/lb");
        }
        
        public override void Help(Player p) {
            CustomBlockCommand.Help(p, "/lb");
        }
        
        public override void Help(Player p, string message) {
            CustomBlockCommand.Help(p, "/lb", message);
        }
    }
}