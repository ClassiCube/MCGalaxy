/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
        
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

namespace MCGalaxy.BlockPhysics {
    
    public static class C4Physics {
        
		public static void DoC4(Level lvl, Check C) {
            C4Data c4 = Find(lvl, ((Player)C.data).c4circuitNumber);
            if (c4 != null) c4.list.Add(C.b);
            C.time = 255;
        }
        
        public static void DoC4Det(Level lvl, Check C) {
            C4Data c4 = Find(lvl, ((Player)C.data).c4circuitNumber);
            if (c4 != null) c4.detIndex = C.b;
            ((Player)C.data).c4circuitNumber = -1;
            C.time = 255;
        }
        
        public static void BlowUp(ushort[] pos, Level lvl) {
            int srcIndex = lvl.PosToInt(pos[0], pos[1], pos[2]);
            try {
                foreach (C4Data c4 in lvl.C4list) {
                    if (c4.detIndex != srcIndex) continue;
                    
                    foreach (int index in c4.list) {
                        ushort x, y, z;
                        lvl.IntToPos(index, out x, out y, out z);
                        lvl.MakeExplosion(x, y, z, 0);
                    }
                    lvl.C4list.Remove(c4);
                }
            } catch { }
        }
        
        public static sbyte NextCircuit(Level lvl) {
            sbyte number = 1;
            foreach (C4Data c4 in lvl.C4list)
                number++;
            return number;
        }
        
        public static C4Data Find(Level lvl, sbyte circuitId) {
            foreach (C4Data c4 in lvl.C4list) {
                if (c4.CircuitID == circuitId) return c4;
            }
            return null;
        }
        
        public class C4Data {
            public sbyte CircuitID;
            public int detIndex = -1;
            public List<int> list = new List<int>();

            public C4Data(sbyte num) {
                CircuitID = num;
            }
        }
    }
}
