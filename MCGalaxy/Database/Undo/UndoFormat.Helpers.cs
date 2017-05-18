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
using MCGalaxy.Maths;

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
        
        static void DoUndo(Stream s, UndoFormat format, UndoFormatArgs args) {
            Level lvl = args.Player == null ? null : args.Player.level;
            string lastMap = null;
            Vec3S32 min = args.Min, max = args.Max;
            DrawOpBlock block;
            
            foreach (UndoFormatEntry P in format.GetEntries(s, args)) {
                if (P.LevelName != lastMap) lvl = LevelInfo.FindExact(P.LevelName);
                if (lvl == null || P.Time > args.End) continue;
                if (P.X < min.X || P.Y < min.Y || P.Z < min.Z) continue;
                if (P.X > max.X || P.Y > max.Y || P.Z > max.Z) continue;
                
                byte lvlBlock = lvl.GetTile(P.X, P.Y, P.Z);
                if (lvlBlock == P.NewBlock.BlockID || Block.Convert(lvlBlock) == Block.water
                    || Block.Convert(lvlBlock) == Block.lava || lvlBlock == Block.grass) {
                    
                    block.X = P.X; block.Y = P.Y; block.Z = P.Z;
                    block.Block = P.Block;
                    args.Output(block);
                }
            }
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
        
        static void DoHighlight(Stream s, UndoFormat format, UndoFormatArgs args) {
            Level lvl = args.Player.level;
            Vec3S32 min = args.Min, max = args.Max;
            DrawOpBlock block;
            
            foreach (UndoFormatEntry P in format.GetEntries(s, args)) {
                ExtBlock old = P.Block, newBlock = P.NewBlock;
                if (P.X < min.X || P.Y < min.Y || P.Z < min.Z) continue;
                if (P.X > max.X || P.Y > max.Y || P.Z > max.Z) continue;
                
                block.Block = (newBlock.BlockID == Block.air
                                       || Block.Convert(old.BlockID) == Block.water || old.BlockID == Block.waterstill
                                       || Block.Convert(old.BlockID) == Block.lava || old.BlockID == Block.lavastill)
                    ? args.DeleteHighlight : args.PlaceHighlight;
                
                block.X = P.X; block.Y = P.Y; block.Z = P.Z;
                args.Output(block);
            }
        }
    }
}