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
using System.IO;

namespace MCGalaxy.Undo {

    /// <summary> Retrieves and saves undo data in a particular format. </summary>
    /// <remarks> Note most formats only support retrieving undo data. </remarks>
    public abstract partial class UndoFormat {
        
        public static void DoUndo(string target, ref bool found, UndoFormatArgs args) {
            List<string> files = GetUndoFiles(target);
            if (files.Count == 0) return;
            found = true;
            
            foreach (string file in files) {
                using (Stream s = File.OpenRead(file)) {
                    DoUndo(s, GetFormat(file), args);
                    if (args.Stop) break;
                }
            }
        }
        
        public static void DoUndo(Stream s, UndoFormat format, UndoFormatArgs args) {
            Level lvl = args.Player == null ? null : args.Player.level;
            BufferedBlockSender buffer = new BufferedBlockSender(lvl);
            string lastMap = null;
            
            foreach (UndoFormatEntry P in format.GetEntries(s, args)) {
                if (P.LevelName != lastMap) {
                    lvl = LevelInfo.FindExact(P.LevelName);
                    buffer.Send(true);
                    buffer.level = lvl;
                }
                
                if (lvl == null || P.Time > args.End) continue;
                UndoBlock(args.Player, lvl, P, buffer);
            }
            buffer.Send(true);
        }
        
        
        public static void DoUndoArea(string target, Vec3S32 min, Vec3S32 max, 
    	                              ref bool found, UndoFormatArgs args) {
            List<string> files = GetUndoFiles(target);
            if (files.Count == 0) return;
            found = true;
            
            foreach (string file in files) {
                using (Stream s = File.OpenRead(file)) {
                    DoUndoArea(s, min, max, GetFormat(file), args);
                    if (args.Stop) break;
                }
            }
        }
        
        public static void DoUndoArea(Stream s, Vec3S32 min, Vec3S32 max,
                                      UndoFormat format, UndoFormatArgs args) {
            Level lvl = args.Player == null ? null : args.Player.level;
            BufferedBlockSender buffer = new BufferedBlockSender(lvl);
            string lastMap = null;
            
            foreach (UndoFormatEntry P in format.GetEntries(s, args)) {
                if (P.LevelName != lastMap) {
                    lvl = LevelInfo.FindExact(P.LevelName);
                    buffer.Send(true);
                    buffer.level = lvl;
                }
                
                if (lvl == null) continue;
                if (P.X < min.X || P.Y < min.Y || P.Z < min.Z) continue;
                if (P.X > max.X || P.Y > max.Y || P.Z > max.Z) continue;
                UndoBlock(args.Player, lvl, P, buffer);
            }
            buffer.Send(true);
        }
        
        
        public static void DoHighlight(string target, ref bool found, UndoFormatArgs args) {
            List<string> files = GetUndoFiles(target);
            if (files.Count == 0) return;
            found = true;
            
            foreach (string file in files) {
                using (Stream s = File.OpenRead(file)) {
                    DoHighlight(s, GetFormat(file), args);
                    if (args.Stop) break;
                }
            }
        }
        
        public static void DoHighlight(Stream s, UndoFormat format, UndoFormatArgs args) {
            BufferedBlockSender buffer = new BufferedBlockSender(args.Player);
            Level lvl = args.Player.level;
            
            foreach (UndoFormatEntry P in format.GetEntries(s, args)) {
                byte block = P.Block, newBlock = P.NewBlock;
                byte highlight = (newBlock == Block.air
                                  || Block.Convert(block) == Block.water || block == Block.waterstill
                                  || Block.Convert(block) == Block.lava || block == Block.lavastill)
                    ? Block.red : Block.green;
                
                buffer.Add(lvl.PosToInt(P.X, P.Y, P.Z), highlight, 0);
            }
            buffer.Send(true);
        }
        
        
        public static void DoRedo(string target, Action<DrawOpBlock> output,
                                  ref bool found, UndoFormatArgs args) {
            List<string> files = GetUndoFiles(target);
            if (files.Count == 0) return;
            found = true;
            
            foreach (string file in files) {
                using (Stream s = File.OpenRead(file)) {
                    DoRedo(s, output, GetFormat(file), args);
                    if (args.Stop) break;
                }
            }
        }
        
        public static void DoRedo(Stream s, Action<DrawOpBlock> output,
                                  UndoFormat format, UndoFormatArgs args) {
            DrawOpBlock block;           
            foreach (UndoFormatEntry P in format.GetEntries(s, args)) {
                if (P.Time > args.End) continue;
                
                block.X = P.X; block.Y = P.Y; block.Z = P.Z;
                block.Block = P.Block; block.ExtBlock = P.ExtBlock;
                output(block);
            }
        }
        
        
        public static void UpgradePlayerUndoFiles(string name) {
            UpgradeFiles(undoDir, name);
            UpgradeFiles(prevUndoDir, name);
        }
        
        static void UpgradeFiles(string dir, string name) {
            string path = Path.Combine(dir, name);
            if (!Directory.Exists(path)) return;
            string[] files = Directory.GetFiles(path);
            List<Player.UndoPos> buffer = new List<Player.UndoPos>();
            UndoFormatArgs args = new UndoFormatArgs(null, DateTime.MinValue, DateTime.MaxValue);
            
            for (int i = 0; i < files.Length; i++) {
                path = files[i];
                if (!path.EndsWith(BinFormat.Ext) && !path.EndsWith(TxtFormat.Ext)) continue;
                IEnumerable<UndoFormatEntry> data = null;
                Player.UndoPos pos;
                
                using (FileStream s = File.OpenRead(path)) {
                    data = path.EndsWith(BinFormat.Ext)
                        ? BinFormat.GetEntries(s, args) : TxtFormat.GetEntries(s, args);

                    foreach (UndoFormatEntry P in data) {
                        pos.x = P.X; pos.y = P.Y; pos.z = P.Z;
                        pos.type = P.Block; pos.extType = P.ExtBlock;
                        pos.newtype = P.NewBlock; pos.newExtType = P.NewExtBlock;
                        
                        pos.timeDelta = (int)P.Time.Subtract(Server.StartTimeLocal).TotalSeconds;
                        pos.mapName = P.LevelName;
                        buffer.Add(pos);
                    }

                    buffer.Reverse();
                    string newPath = Path.ChangeExtension(path, NewFormat.Ext);
                    NewFormat.Save(buffer, newPath);
                }
                File.Delete(path);
            }
        }
        
        static void UndoBlock(Player pl, Level lvl, UndoFormatEntry P,
                              BufferedBlockSender buffer) {
            byte lvlBlock = lvl.GetTile(P.X, P.Y, P.Z);
            if (lvlBlock == P.NewBlock || Block.Convert(lvlBlock) == Block.water
                || Block.Convert(lvlBlock) == Block.lava || lvlBlock == Block.grass) {
                lvl.changed = true;
                
                if (pl != null) {
                    if (lvl.DoBlockchange(pl, P.X, P.Y, P.Z, P.Block, P.ExtBlock, true))
                        buffer.Add(lvl.PosToInt(P.X, P.Y, P.Z), P.Block, P.ExtBlock);
                } else {
                    bool diff = Block.Convert(lvlBlock) != Block.Convert(P.Block);
                    if (!diff && lvlBlock == Block.custom_block)
                        diff = lvl.GetExtTile(P.X, P.Y, P.Z) != P.ExtBlock;
                    if (diff)
                        buffer.Add(lvl.PosToInt(P.X, P.Y, P.Z), P.Block, P.ExtBlock);
                    
                    lvl.SetTile(P.X, P.Y, P.Z, P.Block);
                    if (P.ExtBlock == Block.custom_block)
                        lvl.SetExtTile(P.X, P.Y, P.Z, P.ExtBlock);
                }
            }
        }
    }
}