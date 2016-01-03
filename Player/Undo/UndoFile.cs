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
using System.Linq;

namespace MCGalaxy.Util {

    public abstract class UndoFile {
        
        protected const string undoDir = "extra/undo", prevUndoDir = "extra/undoPrevious";
        public static UndoFile Instance = new UndoFileText();
        
        protected abstract void SaveUndoData(List<Player.UndoPos> buffer, string path);
        
        protected abstract bool UndoEntry(Player p, string path, long seconds);
        
        protected abstract bool HighlightEntry(Player p, string path, long seconds);
        
        protected abstract string Extension { get; }
        
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
            string ext = Instance.Extension;
            int numFiles = di.GetFiles("*" + ext).Length;
            string path = Path.Combine(playerDir, numFiles + ext);
            Instance.SaveUndoData(p.UndoBuffer, path);
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
            if (!Directory.Exists(path))
                return;
            DirectoryInfo di = new DirectoryInfo(path);
            string ext = Instance.Extension;
            int numFiles = di.GetFiles("*" + ext).Length;
            
            for (int i = numFiles - 1; i >= 0; i--) {
                string undoPath = Path.Combine(path, i + ext);
                if (highlight) {
                    if (!Instance.HighlightEntry(p, undoPath, seconds)) break;
                } else {
                    if (!Instance.UndoEntry(p, undoPath, seconds)) break;
                }
            }
            FoundUser = true;
        }
        
        public static void CreateDefaultDirectories() {
            if (!Directory.Exists(undoDir))
                Directory.CreateDirectory(undoDir);
            if (!Directory.Exists(prevUndoDir))
                Directory.CreateDirectory(prevUndoDir);
        }
    }
}