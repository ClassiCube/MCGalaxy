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
using System.Globalization;
using System.IO;
using System.Linq;

namespace MCGalaxy.Util {

    public static class UndoFile {
        
        const string undoDir = "extra/undo", prevUndoDir = "extra/undoPrevious";        
        public static void SaveUndo(Player p) {
            if( p == null || p.UndoBuffer == null || p.UndoBuffer.Count < 1) return;
            
            CreateDefaultDirectories();
            
            DirectoryInfo di = new DirectoryInfo(undoDir);
            if (di.GetDirectories("*").Length >= Server.totalUndo) {
                Directory.Delete(prevUndoDir, true);
                Directory.Move(undoDir, prevUndoDir);
                Directory.CreateDirectory(undoDir);
            }

            string playerDir = Path.Combine(undoDir, p.name.ToLower());
            if (!Directory.Exists(playerDir))
                Directory.CreateDirectory(playerDir);
            
            di = new DirectoryInfo(playerDir);
            int number = di.GetFiles("*.undo").Length;
            string file = number + ".undo";
            
            using (StreamWriter w = File.CreateText(Path.Combine(playerDir, file))) {
                foreach (Player.UndoPos uP in p.UndoBuffer) {
                    w.Write(
                        uP.mapName + " " + uP.x + " " + uP.y + " " + uP.z + " " +
                        uP.timePlaced.ToString(CultureInfo.InvariantCulture).Replace(' ', '&') + " " +
                        uP.type + " " + uP.newtype + " ");
                }
            }
        }
        
        public static void UndoPlayer(Player p, string targetName, long seconds, ref bool FoundUser) {
            if (p != null)
                p.RedoBuffer.Clear();
            FilterEntries(p, undoDir, targetName, seconds, false, ref FoundUser);
            FilterEntries(p, prevUndoDir, targetName, seconds, false, ref FoundUser);
        }
        
        public static void HighlightPlayer(Player p, string targetName, long seconds, ref bool FoundUser) {
            FilterEntries(p, undoDir, targetName, seconds, true, ref FoundUser);
            FilterEntries(p, prevUndoDir, targetName, seconds, true, ref FoundUser);
        }
        
        static void FilterEntries(Player p, string dir, string name, long seconds, bool highlight, ref bool FoundUser) {
            string path = Path.Combine(dir, name);
            if (!Directory.Exists(path)) return;
            DirectoryInfo di = new DirectoryInfo(path);
            int numFiles = di.GetFiles("*.undo").Length;
            
            for (int i = numFiles - 1; i >= 0; i--) {
                string undoPath = Path.Combine(path, i + ".undo");
                string[] lines = File.ReadAllText(undoPath).Split(' ');
                
                if (highlight) {
                    if (!HighlightEntry(p, lines, seconds)) break;
                } else {
                    if (!UndoEntry(p, lines, seconds)) break;
                }
            }
            FoundUser = true;
        }
        
        static bool UndoEntry(Player p, string[] lines, long seconds) {
            Player.UndoPos Pos;
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
        
        static bool HighlightEntry(Player p, string[] lines, long seconds) {
            Player.UndoPos Pos;
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
            DateTime time = DateTime.Parse(line, CultureInfo.InvariantCulture);
            
            time = time.AddSeconds(seconds);
            return time >= DateTime.Now;
        }
        
        public static void CreateDefaultDirectories() {
            if (!Directory.Exists(undoDir))
                Directory.CreateDirectory(undoDir);
            if (!Directory.Exists(prevUndoDir))
                Directory.CreateDirectory(prevUndoDir);
        }
    }
}
