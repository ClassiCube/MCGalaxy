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
using System.IO;
using fNbt;
using MCGalaxy.Maths;

namespace MCGalaxy.Levels.IO {
    public sealed class McLevelImporter : IMapImporter {

        public override string Extension { get { return ".mclevel"; } }
        public override string Description { get { return "Minecraft Indev map"; } }

        public override Vec3U16 ReadDimensions(Stream src) {
            throw new NotSupportedException();
        }
        
        public override Level Read(Stream src, string name, bool metadata) {
            NbtFile file = new NbtFile();
            file.LoadFromStream(src);
            
            Level lvl;
            ReadData(file.RootTag, name, out lvl);
            if (!metadata) return lvl;
            
            ReadMetadata(file.RootTag, lvl);
            return lvl;
        }
        
        void ReadData(NbtCompound root, string name, out Level lvl) {
            NbtCompound map = (NbtCompound)root["Map"];
            ushort width  = (ushort)map["Width" ].ShortValue;
            ushort height = (ushort)map["Height"].ShortValue;
            ushort length = (ushort)map["Length"].ShortValue;
            byte[] blocks = map["Blocks"].ByteArrayValue;            
            lvl = new Level(name, width, height, length, blocks);
            
            NbtList spawn = (NbtList)map["Spawn"];
            lvl.spawnx = (ushort)spawn.Tags[0].ShortValue;
            lvl.spawny = (ushort)spawn.Tags[1].ShortValue;
            lvl.spawnz = (ushort)spawn.Tags[2].ShortValue;
        }
        
        void ReadMetadata(NbtCompound root, Level lvl) {
            NbtCompound env = (NbtCompound)root["Environment"];
            // TODO: Work out sun/shadow color from Skylight and TimeOfDay
            lvl.Config.SkyColor = env["SkyColor"].IntValue.ToString("X6");
            lvl.Config.FogColor = env["FogColor"].IntValue.ToString("X6");
            lvl.Config.CloudColor = env["CloudColor"].IntValue.ToString("X6");            
            lvl.Config.CloudsHeight = env["CloudHeight"].ShortValue;
            
            // TODO: These don't seem right still. do more testing.
            lvl.Config.HorizonBlock = env["SurroundingWaterType"].ByteValue;
            lvl.Config.EdgeLevel = env["SurroundingWaterHeight"].ShortValue;
            lvl.Config.EdgeBlock = env["SurroundingGroundType"].ByteValue;
            int borderHeight = env["SurroundingGroundHeight"].ShortValue;
            lvl.Config.SidesOffset = borderHeight - lvl.Config.EdgeLevel;
        }
    }
}