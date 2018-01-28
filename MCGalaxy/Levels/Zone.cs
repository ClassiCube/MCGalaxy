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
using MCGalaxy.Config;

namespace MCGalaxy {
    
    public sealed class ZoneConfig : AreaConfig {
        [ConfigString("Name", "General", "", true)]
        public string Name = "";
        
        public string Color { get { return Group.GetColor(BuildMin); } }
    }    
      
    /// <summary> Encapuslates build access permissions for a zone. </summary>
    public sealed class ZoneAccessController : AccessController {
        
        readonly Level lvl;
        readonly ZoneConfig cfg;
        
        public ZoneAccessController(Level lvl, ZoneConfig cfg) {
            this.lvl = lvl;
            this.cfg = cfg;
        }
        
        public override LevelPermission Min {
            get { return cfg.BuildMin; } set { cfg.BuildMin = value; }
        }
        
        public override LevelPermission Max {
            get { return cfg.BuildMax; } set { cfg.BuildMax = value; }
        }
        
        public override List<string> Whitelisted { get { return cfg.BuildWhitelist; } }       
        public override List<string> Blacklisted { get { return cfg.BuildBlacklist; } }
        
        protected override string ColoredName { get { return "zone " + cfg.Color + cfg.Name; } }
        protected override string Action { get { return "build in"; } }
        protected override string ActionIng { get { return "building in"; } }
        protected override string Type { get { return "build"; } }
        protected override string MaxCmd { get { return null; } }
        
        public override void OnPermissionChanged(Player p, Group grp, string type) {
            Update();
            Logger.Log(LogType.UserActivity, "{0} rank changed to {1} in zone {2}.", type, grp.Name, cfg.Name);
            Chat.MessageLevel(lvl, type + " rank changed to " + grp.ColoredName + "%S.");
            if (p != null && p.level != lvl)
                Player.Message(p, "{0} rank changed to {1} %Sin {2}%S.", type, grp.ColoredName, ColoredName);
        }
        
        public override void OnListChanged(Player p, string name, bool whitelist, bool removedFromOpposite) {
            string msg = PlayerInfo.GetColoredName(p, name);
            if (removedFromOpposite) {
                msg += " %Swas removed from the build" + (whitelist ? " blacklist" : " whitelist");
            } else {
                msg += " %Swas build" + (whitelist ? " whitelisted" : " blacklisted");
            }
            
            Update();
            Logger.Log(LogType.UserActivity, "{0} in zone {1}", msg, cfg.Name);
            Chat.MessageLevel(lvl, msg);
            if (p != null && p.level != lvl)
                Player.Message(p, "{0} in %S{1}", msg, ColoredName);
        }       
        
        void Update() { lvl.Save(true); }
    }
    
    public class Zone {
        public ushort MinX, MinY, MinZ;
        public ushort MaxX, MaxY, MaxZ;
        
        public ZoneConfig Config;
        public ZoneAccessController Access;
        public string ColoredName { get { return Config.Color + Config.Name; } }
        
        public bool Contains(int x, int y, int z) {
            return x >= MinX && x <= MaxX && y >= MinY && y <= MaxY && z >= MinZ && z <= MaxZ;
        }
        
        public bool CoversMap(Level lvl) {
            return MinX == 0 && MinY == 0 && MinZ == 0 && 
                MaxX == lvl.Width - 1 && MaxY == lvl.Height - 1 && MaxZ == lvl.Length - 1;
        }
        
        public Zone(Level lvl) {
            Config = new ZoneConfig();
            Access = new ZoneAccessController(lvl, Config);
        }
    }
}