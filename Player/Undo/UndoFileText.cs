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
using System.Globalization;
using System.IO;
using MCGalaxy.Drawing;

namespace MCGalaxy.Util {

    public sealed class UndoFileText : UndoFile {
        
        protected override string Extension { get { return ".undo"; } }
        
        protected override void SaveUndoData(List<Player.UndoPos> buffer, string path) {
            throw new NotSupportedException("Text undo files have been deprecated");
        }
        
        protected override void SaveUndoData(UndoCache buffer, string path) {
            throw new NotSupportedException("Text undo files have been deprecated");
        }
        
        protected override void ReadUndoData(List<Player.UndoPos> buffer, string path) {
            Player.UndoPos Pos;
            Pos.extType = 0; Pos.newExtType = 0;
            string[] lines = File.ReadAllText(path).Split(' ');
            int approxEntries = (int)(lines.Length / 7);
            if (buffer.Capacity < approxEntries)
                buffer.Capacity = approxEntries;
            
            for (int i = 0; i < lines.Length; i += 7) {
                if (lines[i].Length == 0) continue;
                Pos.mapName = lines[i];
                Pos.x = ushort.Parse(lines[i + 1]);
                Pos.y = ushort.Parse(lines[i + 2]);
                Pos.z = ushort.Parse(lines[i + 3]);
                
                string time = lines[i + 4].Replace('&', ' ');
                DateTime rawTime = DateTime.Parse(time, CultureInfo.InvariantCulture);
                Pos.timeDelta = (int)rawTime.Subtract(Server.StartTimeLocal).TotalSeconds;
                Pos.type = byte.Parse(lines[i + 5]);
                Pos.newtype = byte.Parse(lines[i + 6]);
                buffer.Add(Pos);
            }
        }
        
        protected override bool UndoEntry(Player p, string path, Vec3U16[] marks,
                                          ref byte[] temp, DateTime start) {
            Player.UndoPos Pos = default(Player.UndoPos);
            int timeDelta = (int)DateTime.UtcNow.Subtract(Server.StartTime).TotalSeconds;
            string[] lines = File.ReadAllText(path).Split(' ');
            Vec3U16 min = marks[0], max = marks[1];
            bool undoArea = min.X != ushort.MaxValue;
            BufferedBlockSender buffer = new BufferedBlockSender(null);
            string last = null;
            
            // because we have space to end of each entry, need to subtract one otherwise we'll start at a "".
            for (int i = (lines.Length - 1) / 7; i >= 0; i--) {
                try {
                    // line format: mapName x y z date oldblock newblock
                    if (!InTime(lines[(i * 7) - 3], start)) return false;
                    Level lvl = LevelInfo.FindExact(lines[(i * 7) - 7]);
                    if (lvl == null || (p.level != null && !p.level.name.CaselessEq(lvl.name)))
                        continue;
                    if (!undoArea) {
                        min = new Vec3U16(0, 0, 0);
                        max = new Vec3U16((ushort)(lvl.Width - 1), (ushort)(lvl.Height - 1), (ushort)(lvl.Length - 1));
                    }
                    if (last == null || last != lvl.name) {
                        buffer.CheckIfSend(true);
                        last = lvl.name;
                    }
                    buffer.level = lvl; 
                    Pos.mapName = lvl.name;
                    
                    Pos.x = Convert.ToUInt16(lines[(i * 7) - 6]);
                    Pos.y = Convert.ToUInt16(lines[(i * 7) - 5]);
                    Pos.z = Convert.ToUInt16(lines[(i * 7) - 4]);
                    if (Pos.x < min.X || Pos.y < min.Y || Pos.z < min.Z ||
                        Pos.x > max.X || Pos.y > max.Y || Pos.z > max.Z) continue;
                    
                    Pos.newtype = Convert.ToByte(lines[(i * 7) - 1]);
                    Pos.type = Convert.ToByte(lines[(i * 7) - 2]);
                    UndoBlock(p, lvl, Pos, timeDelta, buffer);                   
                } catch {
                }
            }
            buffer.CheckIfSend(true);
            return true;
        }
        
        protected override bool HighlightEntry(Player p, string path, ref byte[] temp, DateTime start) {
            string[] lines = File.ReadAllText(path).Split(' ');
            // because we have space to end of each entry, need to subtract one otherwise we'll start at a "".
            for (int i = (lines.Length - 1) / 7; i >= 0; i--) {
                try {
                    // line format: mapName x y z date oldblock newblock
                    if (!InTime(lines[(i * 7) - 3], start)) return false;
                    Level lvl = LevelInfo.FindExact(lines[(i * 7) - 7]);
                    if (lvl == null || lvl != p.level) continue;
                    
                    ushort x = Convert.ToUInt16(lines[(i * 7) - 6]);
                    ushort y = Convert.ToUInt16(lines[(i * 7) - 5]);
                    ushort z = Convert.ToUInt16(lines[(i * 7) - 4]);
                    HighlightBlock(p, lvl, Convert.ToByte(lines[(i * 7) - 1]), x, y, z);
                } catch { }
            }
            return true;
        }
        
        static bool InTime(string line, DateTime start) {
            line = line.Replace('&', ' ');
            DateTime time = DateTime.Parse(line, CultureInfo.InvariantCulture);
            return time >= start;
        }
    }
}
