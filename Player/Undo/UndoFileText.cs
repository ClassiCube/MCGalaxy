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

namespace MCGalaxy.Util {

    public sealed class UndoFileText : UndoFile {
        
        protected override string Ext { get { return ".undo"; } }
        
        protected override void SaveUndoData(List<Player.UndoPos> buffer, string path) {
            throw new NotSupportedException("Text undo files have been deprecated");
        }
        
        protected override void SaveUndoData(UndoCache buffer, string path) {
            throw new NotSupportedException("Text undo files have been deprecated");
        }
        
        protected override IEnumerable<Player.UndoPos> GetEntries(Stream s, UndoEntriesArgs args) {
            Player.UndoPos pos;
            pos.newExtType = 0; pos.extType = 0;
            string[] lines = new StreamReader(s).ReadToEnd().Split(' ');
            Player p = args.Player;
            bool super = p == null || p.ircNick != null;
            DateTime start = args.StartRange;
            
            // because we have space to end of each entry, need to subtract one otherwise we'll start at a "".
            for (int i = (lines.Length - 1) / 7; i >= 0; i--) {
                // line format: mapName x y z date oldblock newblock
                string timeRaw = lines[(i * 7) - 3].Replace('&', ' ');
                DateTime time = DateTime.Parse(timeRaw, CultureInfo.InvariantCulture);
                if (time < start) { args.Stop = true; yield break; }
                pos.timeDelta = (int)time.Subtract(Server.StartTimeLocal).TotalSeconds;
                
                string map = lines[(i * 7) - 7];
                if (!super && !p.level.name.CaselessEq(map)) continue;
                pos.mapName = map;
                
                pos.x = Convert.ToUInt16(lines[(i * 7) - 6]);
                pos.y = Convert.ToUInt16(lines[(i * 7) - 5]);
                pos.z = Convert.ToUInt16(lines[(i * 7) - 4]);
                
                pos.newtype = Convert.ToByte(lines[(i * 7) - 1]);
                pos.type = Convert.ToByte(lines[(i * 7) - 2]);
                yield return pos;
            }
        }
    }
}
