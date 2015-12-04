/*
    Copyright 2015 MCGalaxy
        
    Dual-licensed under the    Educational Community License, Version 2.0 and
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

namespace MCGalaxy.Drawing {

    public sealed class CopyState {
        
        public ushort[] MinCoords;
        public ushort[] MaxCoords;
        
        public byte[] Blocks;
        int width, height, length;
        
        public CopyState(int width, int height, int length) {
            this.width = width;
            this.height = height;
            this.length = length;
            Blocks = new byte[width * height * length];
        }
        
        public void SetBounds(ushort minX, ushort minY, ushort minZ, 
                              ushort maxX, ushort maxY, ushort maxZ) {
            MinCoords = new [] { minX, minY, minZ };
            MaxCoords = new [] { maxX, maxY, maxZ };
        }
    }
}
