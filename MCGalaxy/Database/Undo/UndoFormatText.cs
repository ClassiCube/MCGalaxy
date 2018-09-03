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

namespace MCGalaxy.Undo {

    /// <summary> Reads undo entries in the old MCForge undo text format. </summary>
    public sealed class UndoFormatText : UndoFormat {
        
        protected override string Ext { get { return ".undo"; } }
        
        public override void EnumerateEntries(Stream s, UndoFormatArgs args) {
            UndoFormatEntry pos = default(UndoFormatEntry);
            string[] lines = new StreamReader(s).ReadToEnd().SplitSpaces();
            DateTime time;
            
            // because we have space to end of each entry, need to subtract one otherwise we'll start at a "".
            const int items = 7;
            for (int i = (lines.Length - 1) / items; i > 0; i--) {
                // line format: mapName x y z date oldblock newblock
                string timeRaw = lines[(i * items) - 3].Replace('&', ' ');
                time = DateTime.Parse(timeRaw, CultureInfo.InvariantCulture);
                if (time < args.Start) { args.Finished = true; return; }
                if (time > args.End) continue;
                
                string map = lines[(i * items) - 7];
                if (!args.Map.CaselessEq(map)) continue;
                
                pos.X = ushort.Parse(lines[(i * items) - 6]);
                pos.Y = ushort.Parse(lines[(i * items) - 5]);
                pos.Z = ushort.Parse(lines[(i * items) - 4]);
                                
                pos.Block    = byte.Parse(lines[(i * items) - 2]);
                pos.NewBlock = byte.Parse(lines[(i * items) - 1]);
                args.Output(pos);
            }
        }
    }
}
