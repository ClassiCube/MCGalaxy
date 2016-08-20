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

namespace MCGalaxy.Undo {

    public sealed class UndoFormatText : UndoFormat {
        
        protected override string Ext { get { return ".undo"; } }
        
        protected override void Save(List<Player.UndoPos> buffer, string path) {
            throw new NotSupportedException("Text undo files have been deprecated");
        }
        
        protected override void Save(UndoCache buffer, string path) {
            throw new NotSupportedException("Text undo files have been deprecated");
        }
        
        protected override IEnumerable<UndoFormatEntry> GetEntries(Stream s, UndoFormatArgs args) {
            UndoFormatEntry pos;
            pos.NewExtBlock = 0; pos.ExtBlock = 0;
            string[] lines = new StreamReader(s).ReadToEnd().Split(' ');
            Player p = args.Player;
            bool super = p == null || p.ircNick != null;
            DateTime start = args.Start;
            
            // because we have space to end of each entry, need to subtract one otherwise we'll start at a "".
            const int items = 7;
            for (int i = (lines.Length - 1) / items; i > 0; i--) {
                // line format: mapName x y z date oldblock newblock
                string timeRaw = lines[(i * items) - 3].Replace('&', ' ');
                pos.Time = DateTime.Parse(timeRaw, CultureInfo.InvariantCulture);
                if (pos.Time < start) { args.Stop = true; yield break; }
                
                string map = lines[(i * items) - 7];
                if (!super && !p.level.name.CaselessEq(map)) continue;
                pos.LevelName = map;
                
                pos.X = Convert.ToUInt16(lines[(i * items) - 6]);
                pos.Y = Convert.ToUInt16(lines[(i * items) - 5]);
                pos.Z = Convert.ToUInt16(lines[(i * items) - 4]);
                                
                pos.Block = Convert.ToByte(lines[(i * items) - 2]);
                pos.NewBlock = Convert.ToByte(lines[(i * items) - 1]);
                yield return pos;
            }
        }
    }
}
