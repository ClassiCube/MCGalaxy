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
using System.Linq;

namespace MCGalaxy.Util {

    public sealed class UndoFileText : UndoFile {
        
        protected override string Extension { get { return ".undo"; } }
        
        protected override void SaveUndoData(List<Player.UndoPos> buffer, string path) {
            using (StreamWriter w = File.CreateText(path)) {
                foreach (Player.UndoPos uP in buffer) {
                    w.Write(
                        uP.mapName + " " + uP.x + " " + uP.y + " " + uP.z + " " +
                        uP.timePlaced.ToString(CultureInfo.InvariantCulture).Replace(' ', '&') + " " +
                        uP.type + " " + uP.newtype + " ");
                }
            }
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
                Pos.timePlaced = DateTime.Parse(time, CultureInfo.InvariantCulture);
                Pos.type = byte.Parse(lines[i + 5]);
                Pos.newtype = byte.Parse(lines[i + 6]);
                buffer.Add(Pos);
            }
        }
        
        protected override bool UndoEntry(Player p, string path, long seconds) {
            Player.UndoPos Pos;
            Pos.extType = 0; Pos.newExtType = 0;
            string[] lines = File.ReadAllText(path).Split(' ');
            // because we have space to end of each entry, need to subtract one otherwise we'll start at a "".
            for (int i = (lines.Length - 1) / 7; i >= 0; i--) {
                try {
                    // line format: mapName x y z date oldblock newblock
                    if (!InTime(lines[(i * 7) - 3], seconds)) return false;
                    Level foundLevel = Level.FindExact(lines[(i * 7) - 7]);
                    if (foundLevel == null) continue;
                    
                    Pos.mapName = foundLevel.name;
                    Pos.x = Convert.ToUInt16(lines[(i * 7) - 6]);
                    Pos.y = Convert.ToUInt16(lines[(i * 7) - 5]);
                    Pos.z = Convert.ToUInt16(lines[(i * 7) - 4]);
                    Pos.type = foundLevel.GetTile(Pos.x, Pos.y, Pos.z);

                    if (Pos.type == Convert.ToByte(lines[(i * 7) - 1]) ||
                        Block.Convert(Pos.type) == Block.water || Block.Convert(Pos.type) == Block.lava ||
                        Pos.type == Block.grass) {
                        
                        Pos.newtype = Convert.ToByte(lines[(i * 7) - 2]);
                        Pos.timePlaced = DateTime.Now;

                        foundLevel.Blockchange(Pos.x, Pos.y, Pos.z, Pos.newtype, true);
                        if (p != null)
                            p.RedoBuffer.Add(Pos);
                    }
                } catch {
                }
            }
            return true;
        }
        
        protected override bool HighlightEntry(Player p, string path, long seconds) {
            Player.UndoPos Pos;
            Pos.extType = 0; Pos.newExtType = 0;
            string[] lines = File.ReadAllText(path).Split(' ');
            // because we have space to end of each entry, need to subtract one otherwise we'll start at a "".
            for (int i = (lines.Length - 1) / 7; i >= 0; i--) {
                try {
                    // line format: mapName x y z date oldblock newblock
                    if (!InTime(lines[(i * 7) - 3], seconds)) return false;
                    Level foundLevel = Level.FindExact(lines[(i * 7) - 7]);
                    if (foundLevel == null || foundLevel != p.level) continue;
                    
                    Pos.mapName = foundLevel.name;
                    Pos.x = Convert.ToUInt16(lines[(i * 7) - 6]);
                    Pos.y = Convert.ToUInt16(lines[(i * 7) - 5]);
                    Pos.z = Convert.ToUInt16(lines[(i * 7) - 4]);
                    Pos.type = foundLevel.GetTile(Pos.x, Pos.y, Pos.z);

                    if (Pos.type == Convert.ToByte(lines[(i * 7) - 1]) ||
                        Block.Convert(Pos.type) == Block.water || Block.Convert(Pos.type) == Block.lava) {
                        
                        if (Pos.type == Block.air || Block.Convert(Pos.type) == Block.water || Block.Convert(Pos.type) == Block.lava)
                            p.SendBlockchange(Pos.x, Pos.y, Pos.z, Block.red);
                        else
                            p.SendBlockchange(Pos.x, Pos.y, Pos.z, Block.green);
                    }
                } catch { }
            }
            return true;
        }
        
        static bool InTime(string line, long seconds) {
            line = line.Replace('&', ' ');
            DateTime time = DateTime.Parse(line, CultureInfo.InvariantCulture)
                .AddSeconds(seconds);
            return time >= DateTime.Now;
        }
    }
}
