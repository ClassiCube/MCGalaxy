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
using MCGalaxy.Drawing;

namespace MCGalaxy.Util {

    public abstract class UndoFile {
        
        protected const string undoDir = "extra/undo", prevUndoDir = "extra/undoPrevious";
        public static UndoFile TxtFormat = new UndoFileText();
        public static UndoFile BinFormat = new UndoFileBin();
        public static UndoFile NewFormat = new UndoFileCBin();
        
        protected abstract void SaveUndoData(List<Player.UndoPos> buffer, string path);
        
        protected abstract void SaveUndoData(UndoCache buffer, string path);
         
        protected abstract void ReadUndoData(List<Player.UndoPos> buffer, string path);
        
        protected abstract bool UndoEntry(Player p, string path, Vec3U16[] marks,
                                          ref byte[] temp, DateTime start);
        
        protected abstract bool HighlightEntry(Player p, string path, 
                                               ref byte[] temp, DateTime start);
        
        protected abstract string Extension { get; }
        
        public static void SaveUndo(Player p) {
            if (p == null || p.UndoBuffer.Count < 1) return;
            
            CreateDefaultDirectories();
            if (Directory.GetDirectories(undoDir).Length >= Server.totalUndo) {
                Directory.Delete(prevUndoDir, true);
                Directory.Move(undoDir, prevUndoDir);
                Directory.CreateDirectory(undoDir);
            }

            string playerDir = Path.Combine(undoDir, p.name.ToLower());
            if (!Directory.Exists(playerDir))
                Directory.CreateDirectory(playerDir);
            
            int numFiles = Directory.GetFiles(playerDir).Length;
            string path = Path.Combine(playerDir, numFiles + NewFormat.Extension);
            NewFormat.SaveUndoData(p.UndoBuffer, path);
        }
        
        public static void UndoPlayer(Player p, string target, Vec3U16[] marks, DateTime start, ref bool FoundUser) {
            FilterEntries(p, undoDir, target, marks, start, false, ref FoundUser);
            FilterEntries(p, prevUndoDir, target, marks, start, false, ref FoundUser);
        }
        
        public static void HighlightPlayer(Player p, string target, DateTime start, ref bool FoundUser) {
            FilterEntries(p, undoDir, target, null, start, true, ref FoundUser);
            FilterEntries(p, prevUndoDir, target, null, start, true, ref FoundUser);
        }
        
        static void FilterEntries(Player p, string dir, string name, Vec3U16[] marks,
                                  DateTime start, bool highlight, ref bool FoundUser) {
            string path = Path.Combine(dir, name);
            if (!Directory.Exists(path))
                return;
            string[] files = Directory.GetFiles(path);
            Array.Sort<string>(files, CompareFiles);
            byte[] temp = null;
            
            for (int i = files.Length - 1; i >= 0; i--) {
                path = files[i];
                string file = Path.GetFileName(path);
                if (file.Length == 0 || file[0] < '0' || file[0] > '9')
                    continue;
                
                UndoFile format = null;
                if (path.EndsWith(TxtFormat.Extension)) format = TxtFormat;
                if (path.EndsWith(BinFormat.Extension)) format = BinFormat;
                if (path.EndsWith(NewFormat.Extension)) format = NewFormat;
                if (format == null) continue;
                
                if (highlight) {
                    if (!format.HighlightEntry(p, path, ref temp, start)) break;
                } else {
                    if (!format.UndoEntry(p, path, marks, ref temp, start)) break;
                }
            }
            FoundUser = true;
        }
        
        static int CompareFiles(string a, string b) {
            a = Path.GetFileNameWithoutExtension(a);
            b = Path.GetFileNameWithoutExtension(b);
            
            int aNum, bNum;
            if (!int.TryParse(a, out aNum) || !int.TryParse(b, out bNum))
                return a.CompareTo(b);
            return aNum.CompareTo(bNum);
        }
        
        protected internal static void UndoBlock(Player pl, Level lvl, Player.UndoPos P, 
                                        int timeDelta, BufferedBlockSender buffer) {
            byte lvlTile = lvl.GetTile(P.x, P.y, P.z);
            if (lvlTile == P.newtype || Block.Convert(lvlTile) == Block.water
                || Block.Convert(lvlTile) == Block.lava || lvlTile == Block.grass) {
                
                byte newExtType = P.newExtType;
                P.newtype = P.type; P.newExtType = P.extType;
                P.extType = newExtType; P.type = lvlTile;
                P.timeDelta = timeDelta;
                
                if (pl != null) {
                    if (lvl.DoBlockchange(pl, P.x, P.y, P.z, P.newtype, P.newExtType)) {
                        buffer.Add(lvl.PosToInt(P.x, P.y, P.z), P.newtype, P.newExtType);
                        buffer.CheckIfSend(false);
                    }
                } else {
                	bool diffBlock = Block.Convert(lvlTile) != Block.Convert(P.newtype);
                    if (!diffBlock && lvlTile == Block.custom_block)
                    	diffBlock = lvl.GetExtTile(P.x, P.y, P.z) != P.newExtType;
                    
                	if (diffBlock) {
                        buffer.Add(lvl.PosToInt(P.x, P.y, P.z), P.newtype, P.newExtType);
                        buffer.CheckIfSend(false);
                    }                    
                    lvl.SetTile(P.x, P.y, P.z, P.newtype);
                    if (P.newtype == Block.custom_block)
                    	lvl.SetExtTile(P.x, P.y, P.z, P.newExtType);
                }              
            }
        }
        
        protected static void HighlightBlock(Player p, Level lvl, byte type, ushort x, ushort y, ushort z) {
            byte lvlTile = lvl.GetTile(x, y, z);
            if (lvlTile == type || Block.Convert(lvlTile) == Block.water || Block.Convert(lvlTile) == Block.lava) {             
                byte block = (lvlTile == Block.air || Block.Convert(lvlTile) == Block.water
                              || Block.Convert(lvlTile) == Block.lava) ? Block.red : Block.green;
                p.SendBlockchange(x, y, z, block);
            }
        }
        
        public static void CreateDefaultDirectories() {
            if (!Directory.Exists(undoDir))
                Directory.CreateDirectory(undoDir);
            if (!Directory.Exists(prevUndoDir))
                Directory.CreateDirectory(prevUndoDir);
        }
        
        public static void UpgradePlayerUndoFiles(string name) {
            UpgradeFiles(undoDir, name);
            UpgradeFiles(prevUndoDir, name);
        }
        
        static void UpgradeFiles(string dir, string name) {
            string path = Path.Combine(dir, name);
            if (!Directory.Exists(path))
                return;
            string[] files = Directory.GetFiles(path);
            List<Player.UndoPos> buffer = new List<Player.UndoPos>();
            
            for (int i = 0; i < files.Length; i++) {
                path = files[i];
                if (path.EndsWith(BinFormat.Extension)) {
                    buffer.Clear();
                    BinFormat.ReadUndoData(buffer, path);
                } else if (path.EndsWith(TxtFormat.Extension)) {
                    buffer.Clear();
                    TxtFormat.ReadUndoData(buffer, path);
                }
                
                string newPath = Path.ChangeExtension(path, NewFormat.Extension);
                NewFormat.SaveUndoData(buffer, newPath);
                File.Delete(path);
            }
        }
    }
}