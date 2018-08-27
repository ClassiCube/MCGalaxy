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
using BlockID = System.UInt16;
using BlockRaw = System.Byte;

namespace MCGalaxy.Commands.CPE {
    internal static class CustomBlockCommand {
        
        public static void Execute(Player p, string message, CommandData data, bool global, string cmd) {
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
                case "copyall":
                case "copyfrom":
                    CopyAllHandler(p, parts, data,global, cmd); break;
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
                    p.Message("Aborted the custom block creation process.");
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
            BlockID target;
            if (parts.Length >= 2 ) {
                string id = parts[1];
                if (!CheckBlock(p, id, out target)) return;
                BlockDefinition[] defs = global ? BlockDefinition.GlobalDefs : p.level.CustomBlockDefs;
                BlockDefinition old = defs[target];
                
                if (ExistsInScope(old, target, global)) {
                    p.Message("There is already a custom block with the id " + id +
                              ", you must either use a different id or use \"" + cmd + " remove " + id + "\"");
                    return;
                }
            } else {
                target = GetFreeBlock(global, p.IsSuper ? null : p.level);
                if (target == Block.Invalid) {
                    p.Message("There are no custom block ids left, you must " 
                	          + cmd + " remove a custom block first.");
                    return;
                }
            }
            
            SetBD(p, global, new BlockDefinition());
            BlockDefinition def = GetBD(p, global);
            def.SetBlock(target);
            
            p.Message("Use %T{0} abort %Sat anytime to abort the creation process.", cmd);
            p.Message("  Use %T{0} revert %Sto go back a step", cmd);
            p.Message("  Use %T{0} [input] %Sto provide input", cmd);
            p.Message("&f----------------------------------------------------------");
            
            SetStep(p, global, 2);
            SendStepHelp(p, global);
        }
        
        static void CopyAllHandler(Player p, string[] parts, CommandData data, bool global, string cmd) {
            if (parts.Length < 2) { Help(p, cmd); return; }
            string map = Matcher.FindMaps(p, parts[1]);
            if (map == null) return;
            
            Level lvl = null;
            LevelConfig cfg = LevelInfo.GetConfig(map, out lvl); 
            AccessController visit = new LevelAccessController(cfg, map, true);
            if (!visit.CheckDetailed(p, data.Rank)) {
                p.Message("Hence, you cannot copy custom blocks from that level"); return;
            }
            
            int copied = 0;
            BlockDefinition[] defs = BlockDefinition.Load(false, map);
            for (int i = 0; i < defs.Length; i++) {
                if (defs[i] == null) continue;
                
                BlockID b = (BlockID)i;
                if (!DoCopy(p, global, cmd, true, defs[i], b, b)) continue;
                copied++;
                
                string scope = global ? "global" : "level";
                p.Message("Copied the {0} custom block with id \"{1}\".", scope, Block.ToRaw(b));
            }
            
            string prefix = copied > 0 ? copied.ToString() : "No";
            p.Message("{0} custom blocks were copied from level {1}", 
                           prefix, cfg.Color + map);
        }
        
        static void CopyHandler(Player p, string[] parts, bool global, string cmd) {
            if (parts.Length < 2) { Help(p, cmd); return; }
            BlockID src, dst;
            if (!CheckBlock(p, parts[1], out src, true)) return;
            
            if (parts.Length > 2) {
                if (!CheckBlock(p, parts[2], out dst)) return;
            } else {
                dst = GetFreeBlock(global, p.IsSuper ? null : p.level);
                if (dst == Block.Invalid) {
                    p.Message("There are no custom block ids left, you must " 
                	          + cmd + " remove a custom block first.");
                    return;
                }
            }
            
            BlockDefinition[] defs = global ? BlockDefinition.GlobalDefs : p.level.CustomBlockDefs;
            BlockDefinition srcDef = defs[src];
            if (!DoCopy(p, global, cmd, false, srcDef, src, dst)) return;
            
            string scope = global ? "global" : "level";
            p.Message("Duplicated the {0} custom block with id \"{1}\" to \"{2}\".", 
                      scope, Block.ToRaw(src), Block.ToRaw(dst));
        }

        static bool DoCopy(Player p, bool global, string cmd, bool keepOrder, 
                           BlockDefinition srcDef, BlockID src, BlockID dst) {
            if (srcDef == null && src < Block.CpeCount) {
                srcDef = DefaultSet.MakeCustomBlock(src);
            }
            if (srcDef == null) { MessageNoBlock(p, src, global, cmd); return false; }
            
            BlockDefinition[] defs = global ? BlockDefinition.GlobalDefs : p.level.CustomBlockDefs;
            BlockDefinition dstDef = defs[dst];
            if (ExistsInScope(dstDef, dst, global)) { MessageAlreadyBlock(p, dst, global, cmd); return false; }
            
            BlockProps props = global ? Block.Props[src] : p.level.Props[src];
            dstDef = srcDef.Copy();
            dstDef.SetBlock(dst);
            if (!keepOrder) dstDef.InventoryOrder = -1;
            
            AddBlock(p, dstDef, global, cmd, props);
            return true;
        }
        
        static void InfoHandler(Player p, string[] parts, bool global, string cmd) {
            if (parts.Length == 1) { Help(p, cmd); return; }
            BlockID block;
            if (!CheckBlock(p, parts[1], out block)) return;
            
            BlockDefinition[] defs = global ? BlockDefinition.GlobalDefs : p.level.CustomBlockDefs;
            BlockDefinition def = defs[block];
            if (def == null) { MessageNoBlock(p, block, global, cmd); return; }
            
            p.Message("About {0} ({1})", def.Name, Block.ToRaw(block));
            p.Message("  Draw type: {0}, Blocks light: {1}, collide type: {2}",
                           def.BlockDraw, def.BlocksLight, def.CollideType);
            p.Message("  Fallback ID: {0}, Sound: {1}, Speed: {2}",
                           def.FallBack, def.WalkSound, def.Speed.ToString("F2"));
            
            if (def.FogDensity == 0) {
                p.Message("  Block does not use fog");
            } else {
                p.Message("  Fog density: {0}, color: {1}",
                               def.FogDensity, Utils.Hex(def.FogR, def.FogG, def.FogB));
            }
            
            bool tinted = (def.FogR != 0 || def.FogG != 0 || def.FogB != 0) && def.Name.IndexOf('#') >= 0;
            if (tinted) {
                p.Message("  Tint color: {0}", Utils.Hex(def.FogR, def.FogG, def.FogB));
            }
            
            if (def.Shape == 0) {
                p.Message("  Block is a sprite");
                p.Message("  Texture ID: {0}", def.RightTex);
            } else {
                p.Message("  Block is a cube from ({0}, {1}, {2}) to ({3}, {4}, {5})",
                               def.MinX, def.MinZ, def.MinY, def.MaxX, def.MaxZ, def.MaxY);
                p.Message("  Texture IDs (left: {0}, right: {1}, front: {2}, back: {3}, top: {4}, bottom: {5})",
                               def.LeftTex, def.RightTex, def.FrontTex, def.BackTex, def.TopTex, def.BottomTex);
            }
            
            if (def.InventoryOrder < 0) {
                p.Message("  Order: None");
            } else if (def.InventoryOrder == 0) {
                p.Message("  Order: Hidden from inventory");
            } else {
                p.Message("  Order: " + def.InventoryOrder);
            }
        }
        
        static void ListHandler(Player p, string[] parts, bool global, string cmd) {
            string modifier = parts.Length > 1 ? parts[1] : "";
            BlockDefinition[] defs = global ? BlockDefinition.GlobalDefs : p.level.CustomBlockDefs;
            List<BlockDefinition> defsInScope = new List<BlockDefinition>();
            
            for (int i = 0; i < defs.Length; i++) {
                BlockDefinition def = defs[i];
                if (def == null) continue;
                BlockID block = def.GetBlock();
                
                if (!ExistsInScope(def, block, global)) continue;
                defsInScope.Add(def);
            }
            MultiPageOutput.Output(p, defsInScope, FormatBlock, cmd.Substring(1) + " list",
                                   "custom blocks", modifier, true);
        }
        
        static string FormatBlock(BlockDefinition def) {
            return "Custom block %T" + def.RawID + " %Shas name %T" + def.Name;
        }
        
        static void RemoveHandler(Player p, string[] parts, bool global, string cmd) {
            if (parts.Length <= 1) { Help(p, cmd); return; }
            BlockID block;
            if (!CheckBlock(p, parts[1], out block)) return;
            
            BlockDefinition[] defs = global ? BlockDefinition.GlobalDefs : p.level.CustomBlockDefs;
            BlockDefinition def = defs[block];
            if (!ExistsInScope(def, block, global)) { MessageNoBlock(p, block, global, cmd); return; }
            
            RemoveBlockProps(global, block, p);
            BlockDefinition.Remove(def, defs, p.IsSuper ? null : p.level);
            
            string scope = global ? "global" : "level";
            p.Message("Removed " + scope + " custom block " + def.Name + "(" + def.RawID + ")");
            
            BlockDefinition globalDef = BlockDefinition.GlobalDefs[block];
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
                if (CommandParser.GetUShort(p, value, "Texture ID", ref bd.TopTex, 0, 255)) {
                    step += (bd.Shape == 0 ? 5 : 1); // skip other texture steps for sprites
                    if (bd.Shape == 0) bd.SetAllTex(bd.TopTex);
                }
            } else if (step == 5) {
                if (CommandParser.GetUShort(p, value, "Texture ID", ref bd.RightTex, 0, 255)) {
                    bd.SetSideTex(bd.RightTex);
                    step++;
                }
            } else if (step == 6) {
                if (CommandParser.GetUShort(p, value, "Texture ID", ref bd.BottomTex, 0, 255))
                    step++;
            } else if (step == 7) {
                if (ParseCoords(p, value, ref bd.MinX, ref bd.MinY, ref bd.MinZ))
                    step++;
            } else if (step == 8) {
                if (ParseCoords(p, value, ref bd.MaxX, ref bd.MaxY, ref bd.MaxZ))
                    step++;
                bd.Shape = bd.MaxY;
            } else if (step == 9) {
                if (CommandParser.GetByte(p, value, "Collide type", ref bd.CollideType, 0, 7))
                    step++;
            } else if (step == 10) {
                if (CommandParser.GetReal(p, value, "Movement speed", ref bd.Speed, 0.25f, 3.96f))
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
                
                BlockID block = bd.GetBlock();
                if (!AddBlock(p, bd, global, cmd, Block.Props[block])) return;
                
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
                    p.Message("Valid properties: " + helpSections.Keys.Join());
                } else if (parts.Length == 3) {
                    Help(p, cmd, "edit " + parts[2]);
                } else {
                    Help(p, cmd);
                }
                return;
            }
            
            BlockID block;
            if (!CheckBlock(p, parts[1], out block)) return;
            BlockDefinition[] defs = global ? BlockDefinition.GlobalDefs : p.level.CustomBlockDefs;
            BlockDefinition def = defs[block], globalDef = BlockDefinition.GlobalDefs[block];
            
            if (def == null && block < Block.CpeCount) {
                def = DefaultSet.MakeCustomBlock(block);
                AddBlock(p, def, global, cmd, Block.Props[block]);
            }
            if (def != null && !global && def == globalDef) {
                def = globalDef.Copy();
                AddBlock(p, def, global, cmd, Block.Props[block]);
            }
            if (!ExistsInScope(def, block, global)) { MessageNoBlock(p, block, global, cmd); return; }
            
            string value = parts[3], blockName = def.Name;
            bool temp = false, changedFallback = false;
            Level level = p.IsSuper ? null : p.level;
            
            string arg = MapPropertyName(parts[2].ToLower());
            switch (arg) {
                case "name":
                    def.Name = value; break;
                case "collide":
                    if (!EditByte(p, value, "Collide type", ref def.CollideType, arg)) return;
                    break;
                case "speed":
                    if (!CommandParser.GetReal(p, value, "Movement speed", ref def.Speed, 0.25f, 3.96f)) {
                        SendEditHelp(p, arg); return;
                    } break;                  
                case "toptex":
                    if (!EditUShort(p, value, "Top texture", ref def.TopTex, arg)) return;
                    break;
                case "alltex":
                    if (!EditUShort(p, value, "All textures", ref def.RightTex, arg)) return;
                    def.SetAllTex(def.RightTex);
                    break;
                case "sidetex":
                    if (!EditUShort(p, value, "Side texture", ref def.RightTex, arg)) return;
                    def.SetSideTex(def.RightTex);
                    break;
                case "lefttex":
                    if (!EditUShort(p, value, "Left texture", ref def.LeftTex, arg)) return;
                    break;
                case "righttex":
                    if (!EditUShort(p, value, "Right texture", ref def.RightTex, arg)) return;
                    break;
                case "fronttex":
                    if (!EditUShort(p, value, "Front texture", ref def.FrontTex, arg)) return;
                    break;
                case "backtex":
                    if (!EditUShort(p, value, "Back texture", ref def.BackTex, arg)) return;
                    break;
                case "bottomtex":
                    if (!EditUShort(p, value, "Bottom texture", ref def.BottomTex, arg)) return;
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
                    
                    value = Block.GetName(p, fallback);
                    def.FallBack = fallback; break;
                    
                case "order":
                    int order = 0;
                    if (!CommandParser.GetInt(p, value, "Inventory order", ref order, 0, Block.MaxRaw)) {
                        SendEditHelp(p, arg); return;
                    }
                    
                    // Don't let multiple blocks be assigned to same order
                    if (order != def.RawID && order != 0) {
                        for (int i = 0; i < defs.Length; i++) {
                            if (defs[i] == null || defs[i].InventoryOrder != order) continue;
                            p.Message("Block {0} already had order {1}", defs[i].Name, order);
                            return;
                        }
                    }
                    
                    def.InventoryOrder = order == def.RawID ? -1 : order;
                    BlockDefinition.UpdateOrder(def, global, level);
                    BlockDefinition.Save(global, level);
                    p.Message("Set inventory order for {0} to {1}", blockName,
                                   order == def.RawID ? "default" : order.ToString());
                    return;
                default:
                    p.Message("Unrecognised property: " + arg); return;
            }
            
            p.Message("Set {0} for {1} to {2}", arg, blockName, value);
            BlockDefinition.Add(def, defs, level);
            if (changedFallback) {
                BlockDefinition.UpdateFallback(global, def.GetBlock(), level);
            }
        }
        
        
        static bool AddBlock(Player p, BlockDefinition def, bool global, string cmd, BlockProps props) {
            BlockDefinition[] defs = global ? BlockDefinition.GlobalDefs : p.level.CustomBlockDefs;
            BlockID block = def.GetBlock();
            BlockDefinition old = defs[block];
            if (!global && old == BlockDefinition.GlobalDefs[block]) old = null;
            
            // in case the list is modified before we finish the command.
            if (old != null) {
                block = GetFreeBlock(global, p.IsSuper ? null : p.level);
                if (block == Block.Invalid) {
                    p.Message("There are no custom block ids left, " +
                                   "you must " + cmd + " remove a custom block first.");
                    if (!global) {
                        p.Message("You may also manually specify the same existing id of a global custom block.");
                    }
                    return false;
                }
                def.SetBlock(block);
            }
            
            string scope = global ? "global" : "level";
            p.Message("Created a new " + scope + " custom block " + def.Name + "(" + def.RawID + ")");
            
            block = def.GetBlock();
            BlockDefinition.Add(def, defs, p.IsSuper ? null : p.level);
            UpdateBlockProps(global, p, block, props);
            return true;
        }
        
        static BlockRaw GetFallback(Player p, string value) {
            BlockID block;
            if (!CommandParser.GetBlock(p, value, out block)) return Block.Invalid;
            
            if (block >= Block.Extended) {
                p.Message("%WCustom blocks cannot be used as fallback blocks.");
                return Block.Invalid;
            }
            if (Block.IsPhysicsType(block)) {
                p.Message("%WPhysics block cannot be used as fallback blocks.");
                return Block.Invalid;
            }
            return (BlockRaw)block;
        }
        
        
        static BlockID GetFreeBlock(bool global, Level lvl) {
            // Start from opposite ends to avoid overlap.
            if (global) {
                BlockDefinition[] defs = BlockDefinition.GlobalDefs;
                for (BlockID b = Block.CpeCount; b <= Block.MaxRaw; b++) {
                    BlockID block = Block.FromRaw(b);
                    if (defs[block] == null) return block;
                }
            } else {
                BlockDefinition[] defs = lvl.CustomBlockDefs;
                for (BlockID b = Block.MaxRaw; b >= Block.CpeCount; b--) {
                    BlockID block = Block.FromRaw(b);
                    if (defs[block] == null) return block;
                }
            }
            return Block.Invalid;
        }
        
        static void MessageNoBlock(Player p, BlockID block, bool global, string cmd) {
            string scope = global ? "global" : "level";
            p.Message("%WThere is no {1} custom block with the id \"{0}\".", Block.ToRaw(block), scope);
            p.Message("Type %T{0} list %Sto see a list of {1} custom blocks.", cmd, scope);
        }
        
        static void MessageAlreadyBlock(Player p, BlockID block, bool global, string cmd) {
            string scope = global ? "global" : "level";
            p.Message("%WThere is already a {1} custom block with the id \"{0}\".", Block.ToRaw(block), scope);
            p.Message("Type %T{0} list %Sto see a list of {1} custom blocks.", cmd, scope);
        }
        
        static bool EditByte(Player p, string value, string propName, ref byte target, string help) {
            if (!CommandParser.GetByte(p, value, propName, ref target)) {
                SendEditHelp(p, help); return false;
            }
            return true;
        }
         
        static bool EditUShort(Player p, string value, string propName, ref ushort target, string help) {
            if (!CommandParser.GetUShort(p, value, propName, ref target)) {
                SendEditHelp(p, help); return false;
            }
            return true;
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
        
        static bool CheckBlock(Player p, string arg, out BlockID block, bool allowAir = false) {
            block = Block.Invalid;
            BlockID raw = 0;
            BlockID min = (BlockID)(allowAir ? 0 : 1);
            BlockID max = Block.MaxRaw;
            bool success = CommandParser.GetUShort(p, arg, "Block ID", ref raw, min, max);
            
            block = Block.FromRaw(raw);
            return success;
        }
        
        
        static void UpdateBlockProps(bool global, Player p, BlockID block, BlockProps props) {
            if (!global) {
                p.level.Props[block] = props;
                p.level.UpdateBlockHandler(block);
            } else {
                Block.ChangeGlobalProps(block, props);
            }
        }
        
        static void RemoveBlockProps(bool global, BlockID block, Player p) {
            if (!global) {
                p.level.Props[block] = Block.Props[block];
                p.level.UpdateBlockHandler(block);
            } else {
                BlockProps[] defProps = new BlockProps[Block.ExtendedCount];
                Block.MakeDefaultProps(defProps);
                Block.ChangeGlobalProps(block, defProps[block]);
            }
        }
        
        
        static BlockDefinition GetBD(Player p, bool global) { return global ? p.gbBlock : p.lbBlock; }        
        static int GetStep(Player p, bool global) { return global ? p.gbStep : p.lbStep; }
        
        static void SetBD(Player p, bool global, BlockDefinition bd) {
            if (global) p.gbBlock = bd;
            else p.lbBlock = bd;
        }
        
        static void SetStep(Player p, bool global, int step) {
            if (global) p.gbStep = step;
            else p.lbStep = step;
        }
        
        static bool ExistsInScope(BlockDefinition def, BlockID block, bool global) {
            return def != null && (global ? true : def != BlockDefinition.GlobalDefs[block]);
        }
        
        
        static void SendStepHelp(Player p, bool global) {
            int step = GetStep(p, global);
            string[] help = helpSections[stepsHelp[step]];
            
            BlockDefinition bd = GetBD(p, global);
            if (step == 4 && bd.Shape == 0)
                help[0] = help[0].Replace("top texture", "texture");
            
            for (int i = 0; i < help.Length; i++)
                p.Message(help[i]);
            p.Message("&f--------------------------");
        }
        
        static void SendEditHelp(Player p, string section) {
            string[] help = helpSections[section];
            for (int i = 0; i < help.Length; i++)
                p.Message(help[i].Replace("Type", "Use"));
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
            { "collide", new string[] { "Type a number between '0' and '7' for collision type.",
                    "0 - block is walk-through (e.g. air).", "1 - block is swim-through/climbable (e.g. rope).",
                    "2 - block is solid (e.g. dirt).", "3 - block is solid, but slippery like ice",
                    "4 - block is solid, but even slipperier than ice", "5 - block is swim-through like water",
                    "6 - block is swim-through like lava", "7 - block is climbable like rope" } },
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
                    "A position of 0 hides the block from the inventory." }
            },
        };
        
        
        internal static void Help(Player p, string cmd) {
            p.Message("%T{0} add [id] %H- begins creating a new custom block", cmd);
            p.Message("%T{0} copyall [map] %H- clones all custom blocks in [map]", cmd);            
            p.Message("%T{0} copy [id] [new id] %H- clones an existing custom block", cmd);
            p.Message("%T{0} edit [id] [property] [value] %H- edits that custom block", cmd);
            p.Message("%T{0} list <offset> %H- lists all custom blocks", cmd);
            p.Message("%T{0} remove [id] %H- removes that custom block", cmd);
            p.Message("%T{0} info [id] %H- shows info about that custom block", cmd);
            p.Message("%HTo see the list of editable properties, type {0} edit", cmd);
        }
        
        internal static void Help(Player p, string cmd, string args) {
            if (!args.CaselessStarts("edit ")) { Help(p, cmd); return; }
            string prop = args.Substring(args.IndexOf(' ') + 1);
            prop = MapPropertyName(prop.ToLower());

            if (!helpSections.ContainsKey(prop)) {
                p.Message("Valid properties: " + helpSections.Keys.Join());
            } else {
                SendEditHelp(p, prop);
            }
        }
    }
    
    public sealed class CmdGlobalBlock : Command2 {
        public override string name { get { return "GlobalBlock"; } }
        public override string shortcut { get { return "gb"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }

        public override void Use(Player p, string message, CommandData data) {
            CustomBlockCommand.Execute(p, message, data, true, "/gb");
        }
        
        public override void Help(Player p) {
            CustomBlockCommand.Help(p, "/gb");
        }
        
        public override void Help(Player p, string message) {
            CustomBlockCommand.Help(p, "/gb", message);
        }
    }
    
    public sealed class CmdLevelBlock : Command2 {
        public override string name { get { return "LevelBlock"; } }
        public override string shortcut { get { return "lb"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        public override bool SuperUseable { get { return false; } }

        public override void Use(Player p, string message, CommandData data) {
            CustomBlockCommand.Execute(p, message, data, false, "/lb");
        }
        
        public override void Help(Player p) {
            CustomBlockCommand.Help(p, "/lb");
        }
        
        public override void Help(Player p, string message) {
            CustomBlockCommand.Help(p, "/lb", message);
        }
    }
}