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

namespace MCGalaxy.Util {

    public class UndoDrawOpEntry {
        
        public string DrawOpName;
        public string LevelName;
        public UndoCacheNode StartNode;
        public int StartIndex;
        public UndoCacheNode EndNode;
        public int EndIndex;
        
        public void SetStart(Player p) {
            StartNode = p.UndoBuffer.Tail;
            StartIndex = StartNode.Items.Count;
        }
        
        public void SetEnd(Player p) {
            EndNode = p.UndoBuffer.Tail;
            EndIndex = EndNode.Items.Count;
        }
        
        public bool IsValid(Player p) {
            UndoCache buffer = p.UndoBuffer;
            if (p.UndoBuffer.Tail == null) return false;
            
            UndoCacheNode node = p.UndoBuffer.Tail;
            while ((node = node.Prev) != null) {
                if (node == EndNode) return true;
            }
            return false;
        }
    }
}
