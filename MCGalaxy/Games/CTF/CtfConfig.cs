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
    
    public sealed class CTFConfig : RoundsGameConfig {
        public override bool AllowAutoload { get { return false; } }
        protected override string GameName { get { return "CTF"; } }
        protected override string PropsPath { get { return "properties/ctf.properties"; } }
    }
    
    public sealed class CTFMapConfig {
        [ConfigVec3("red-spawn", null)] public Vec3U16 RedSpawn;
        [ConfigVec3("red-pos", null)] public Vec3U16 RedFlagPos;
        [ConfigBlock("red-block", null, Block.Air)]
        public BlockID RedFlagBlock;
        
        [ConfigVec3("blue-spawn", null)] public Vec3U16 BlueSpawn;
        [ConfigVec3("blue-pos", null)] public Vec3U16 BlueFlagPos;
        [ConfigBlock("blue-block", null, Block.Air)]
        public BlockID BlueFlagBlock;        
        
        [ConfigInt("map.line.z", null, 0)]
        public int ZDivider;
        [ConfigInt("game.maxpoints", null, 3)]
        public int RoundPoints = 3;
        [ConfigInt("game.tag.points-gain", null, 5)]
        public int Tag_PointsGained = 5;
        [ConfigInt("game.tag.points-lose", null, 5)]
        public int Tag_PointsLost = 5;
        [ConfigInt("game.capture.points-gain", null, 10)]
        public int Capture_PointsGained = 10;
        [ConfigInt("game.capture.points-lose", null, 10)]
        public int Capture_PointsLost = 10;

        
        const string propsDir = "properties/CTF/";
        static string Path(string map) { return propsDir + map + ".properties"; }
        static ConfigElement[] cfg;
        
        public void Load(string map) {
            if (cfg == null) cfg = ConfigElement.GetAll(typeof(CTFMapConfig));
            ConfigElement.ParseFile(cfg, Path(map), this);
        }
        
        public void Save(string map) {
            if (!Directory.Exists(propsDir)) Directory.CreateDirectory(propsDir);
            
            if (cfg == null) cfg = ConfigElement.GetAll(typeof(CTFMapConfig));
            ConfigElement.SerialiseSimple(cfg, Path(map), this);
        }
        
        public void SetDefaults(Level lvl) {
            ZDivider = lvl.Length / 2;
            RedFlagBlock  = Block.Red;
            BlueFlagBlock = Block.Blue;
            ushort midX = (ushort)(lvl.Width / 2), maxZ = (ushort)(lvl.Length - 1);
            
            RedFlagPos  = new Vec3U16(midX, 6, 0);
            RedSpawn    = new Vec3U16(midX, 4, 0);
            BlueFlagPos = new Vec3U16(midX, 6, maxZ);
            BlueSpawn   = new Vec3U16(midX, 4, maxZ);
        }
    }
}
