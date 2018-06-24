/*
    Copyright 2011 MCForge
    
    Written by fenderrock87
    
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
using System.IO;
using MCGalaxy.Config;
using MCGalaxy.Maths;
using BlockID = System.UInt16;

namespace MCGalaxy.Games {
    
    public sealed class CtfMapConfig {

        [ConfigVec3("base.red.pos", null)]
        public Vec3U16 RedFlagPos;
        [ConfigBlock("base.red.block", null, Block.Air)]
        public BlockID RedFlagBlock;
        [ConfigVec3("base.red.spawn", null)]
        public Vec3U16 RedSpawn;
        
        [ConfigVec3("base.blue.pos", null)]
        public Vec3U16 BlueFlagPos;
        [ConfigBlock("base.blue.block", null, Block.Air)]
        public BlockID BlueFlagBlock;
        [ConfigVec3("base.blue.spawn", null)]
        public Vec3U16 BlueSpawn;
        
        [ConfigInt("map.line.z", null, 0)]
        public int ZDivider;
        [ConfigInt("game.maxpoints", null, 0)]
        public int RoundPoints;
        [ConfigInt("game.tag.points-gain", null, 0)]
        public int Tag_PointsGained;
        [ConfigInt("game.tag.points-lose", null, 0)]
        public int Tag_PointsLost;
        [ConfigInt("game.capture.points-gain", null, 0)]
        public int Capture_PointsGained;
        [ConfigInt("game.capture.points-lose", null, 0)]
        public int Capture_PointsLost;
        
        
        public void SetDefaults(Level lvl) {
            ZDivider = lvl.Length / 2;
            RedFlagBlock  = Block.Red;
            BlueFlagBlock = Block.Blue;
            ushort midX = (ushort)(lvl.Width / 2), maxZ = (ushort)(lvl.Length - 1);
            
            RedFlagPos  = new Vec3U16(midX, 6, 0);
            RedSpawn    = new Vec3U16(midX, 4, 0);
            BlueFlagPos = new Vec3U16(midX, 6, maxZ);
            BlueSpawn   = new Vec3U16(midX, 4, maxZ);
            
            RoundPoints = 3;
            Tag_PointsGained = 5;
            Tag_PointsLost = 5;
            Capture_PointsGained = 10;
            Capture_PointsLost = 10;
        }
        
        
        const string propsDir = "CTF/";
        static string Path(string map) { return propsDir + map + ".config"; }
        static ConfigElement[] cfg;
        
        public void Load(string map) {
            if (cfg == null) cfg = ConfigElement.GetAll(typeof(CtfMapConfig));
            ConfigElement.ParseFile(cfg, "CTF map", Path(map), this);
        }
        
        public void Save(string map) {
            if (!Directory.Exists(propsDir)) Directory.CreateDirectory(propsDir);
            
            if (cfg == null) cfg = ConfigElement.GetAll(typeof(CtfMapConfig));
            ConfigElement.SerialiseSimple(cfg, Path(map), this);
        }
    }
}
