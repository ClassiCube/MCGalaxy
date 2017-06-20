﻿/*
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
    
    public sealed class UndoDrawOpEntry {
        public string DrawOpName;
        public string LevelName;
        public DateTime Start, End;
    }

    /// <summary> Retrieves and saves undo data in a particular format. </summary>
    /// <remarks> Undo files are obsolete, so only reading them is supported. </remarks>
    public abstract partial class UndoFormat {
        
        protected const string undoDir = "extra/undo", prevUndoDir = "extra/undoPrevious";
        public static UndoFormat TxtFormat = new UndoFormatText();
        public static UndoFormat BinFormat = new UndoFormatBin();
        public static UndoFormat NewFormat = new UndoFormatCBin();
        
        /// <summary> Enumerates through all the entries in the undo file. </summary>
        public abstract void EnumerateEntries(Stream s, UndoFormatArgs args);
        
        /// <summary> File extension of undo files in this format. </summary>
        protected abstract string Ext { get; }
        
        
        /// <summary> Gets a list of all undo file names for the given player. </summary>
        /// <remarks> This list is sorted, such that the first element is the
        /// most recent undo file, and the last element is the oldest.</remarks>
        public static List<string> GetUndoFiles(string name) {
            List<string> entries = new List<string>();
            string[] cur = GetFiles(undoDir, name);
            string[] prev = GetFiles(prevUndoDir, name);
            
            // Start from the last entry, because when undoing we want to start
            // with the most recent file first
            for (int i = cur.Length - 1; i >= 0; i--) {
                if (cur[i] == null) continue;
                entries.Add(cur[i]);
            }
            for (int i = prev.Length - 1; i >= 0; i--) {
                if (prev[i] == null) continue;
                entries.Add(prev[i]);
            }
            return entries;
        }
        
        static string[] GetFiles(string dir, string name) {
            string path = Path.Combine(dir, name);
            if (!Directory.Exists(path)) return new string[0];
            string[] files = Directory.GetFiles(path);
            Array.Sort<string>(files, CompareFiles);
            
            for (int i = 0; i < files.Length; i++) {
                name = Path.GetFileName(files[i]);
                if (name.Length == 0 || name[0] < '0' || name[0] > '9')
                    files[i] = null;
                if (files[i] != null && GetFormat(name) == null)
                    files[i] = null;
            }
            return files;
        }
        
        public static UndoFormat GetFormat(string file) {
            if (file.EndsWith(TxtFormat.Ext)) return TxtFormat;
            if (file.EndsWith(BinFormat.Ext)) return BinFormat;
            if (file.EndsWith(NewFormat.Ext)) return NewFormat;
            return null;
        }
        
        static int CompareFiles(string a, string b) {
            a = Path.GetFileNameWithoutExtension(a);
            b = Path.GetFileNameWithoutExtension(b);
            
            int aNum, bNum;
            if (!int.TryParse(a, out aNum) || !int.TryParse(b, out bNum))
                return a.CompareTo(b);
            return aNum.CompareTo(bNum);
        }
    }
    
    /// <summary> Arguments provided to an UndoFormat for retrieving undo data. </summary>
    public class UndoFormatArgs {
        
        /// <summary> Level to retrieve undo data on. </summary>
        internal readonly string LevelName;

        /// <summary> Small work buffer, used to avoid memory allocations. </summary>
        internal byte[] Temp;

        /// <summary> Whether the format has finished retrieving undo data,
        /// due to finding an entry before the start range. </summary>
        public bool Stop;        

        /// <summary> First instance in time that undo data should be retrieved back to. </summary>
        internal readonly DateTime Start;      

        /// <summary> Last instance in time that undo data should be retrieved up to. </summary>
        internal readonly DateTime End;
        
        /// <summary> Performs action on the given undo format entry. </summary>
        public Action<UndoFormatEntry> Output;

        public UndoFormatArgs(string lvlName, DateTime start, DateTime end,
                              Action<UndoFormatEntry> output) {
            LevelName = lvlName; Start = start; End = end; Output = output;
        }
    }

    public struct UndoFormatEntry {
        public ushort X, Y, Z;
        public ExtBlock Block;
        public ExtBlock NewBlock;
    }
}