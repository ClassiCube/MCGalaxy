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
using MCGalaxy.Blocks;

namespace MCGalaxy {
    public sealed partial class Block {
        public static BlockProps[] Properties = new BlockProps[256];
        
        static void SetDefaultProperties() {
            for (int i = 0; i < 256; i++) {
                Properties[i] = new BlockProps((byte)i);
                // Fallback for unrecognised physics blocks
                if (i >= Block.CpeCount)
                    Properties[i].ConvertId = Block.orange;
            }
        }
    }
}
