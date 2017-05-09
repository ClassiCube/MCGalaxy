/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
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

namespace MCGalaxy.Commands.Building {    
    public sealed class CmdMeasure : Command {
        
        public override string name { get { return "measure"; } }
        public override string shortcut { get { return "ms"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }

        public override void Use(Player p, string message)
        {
            if (message.IndexOf(' ') != -1) { Help(p); return; }
            byte toIgnore = Block.air;
            if (message != "") {
                toIgnore = Block.Byte(message);
                if (toIgnore == Block.Invalid) {
                    Player.Message(p, "Could not find block specified"); return;
                }
            }
            
            Player.Message(p, "Place or break two blocks to determine the edges.");
            p.MakeSelection(2, toIgnore, DoMeasure);
        }
        
        bool DoMeasure(Player p, Vec3S32[] m, object state, byte type, byte extType) {
            byte toIgnore = (byte)state;
            int minX = Math.Min(m[0].X, m[1].X), maxX = Math.Max(m[0].X, m[1].X);
            int minY = Math.Min(m[0].Y, m[1].Y), maxY = Math.Max(m[0].Y, m[1].Y);
            int minZ = Math.Min(m[0].Z, m[1].Z), maxZ = Math.Max(m[0].Z, m[1].Z);
            int found = 0;

            for (int y = minY; y <= maxY; y++)
                for (int z = minZ; z <= maxZ; z++)
                    for (int x = minX; x <= maxX; x++)
            {
                if (p.level.GetTile((ushort)x, (ushort)y, (ushort)z) != toIgnore)
                    found++;
            }

            int width = maxX - minX + 1, height = maxY - minY + 1, length = maxZ - minZ + 1;
            Player.Message(p, "Measuring from &a({0}, {1}, {2}) %Sto &a({3}, {4}, {5})", 
                           minX, minY, minZ, maxX, maxY, maxZ);
            Player.Message(p, "Area is {0} wide, {1} high, {2} long. Volume is {3} blocks.", 
                           width, height, length, width * height * length);
            Player.Message(p, "There are {0} {1} blocks in the area.", found, "non-" + Block.Name(toIgnore));
            return true;
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/measure [ignore]");
            Player.Message(p, "%HMeasures all the blocks between two points");
            Player.Message(p, "%H [ignore] is optional, and specifies the block which is not counted.");
        }
    }
}
