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
using MCGalaxy.Events.PlayerEvents;
using MCGalaxy.Maths;
using MCGalaxy.Network;

namespace MCGalaxy {
    
    public sealed class ZoneConfig : AreaConfig {
        [ConfigString("Name", "General", "", true)]
        public string Name = "";
        [ConfigString("ShowColor", "General", "000000", true)]
        public string ShowColor = "000000";
        [ConfigInt("ShowAlpha", "General", 0, 0, 255)]
        public int ShowAlpha = 0;
        
        public string Color { get { return Group.GetColor(BuildMin); } }
    }
    
    /// <summary> Encapuslates build access permissions for a zone. </summary>
    public sealed class ZoneAccessController : AccessController {        
        readonly ZoneConfig cfg;
        
        public ZoneAccessController(ZoneConfig cfg) {
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

        
        protected override void ApplyChanges(Player p, Level lvl, string msg) {
            lvl.Save(true);
            msg += " &Sin " + ColoredName;
            Logger.Log(LogType.UserActivity, "{0} &Son {1}", msg, lvl.name);
            
            lvl.Message(msg);           
            if (p.level != lvl) p.Message("{0} &Son {1}", msg, lvl.ColoredName);
        }
    }
    
    public class Zone {
        public ushort MinX, MinY, MinZ;
        public ushort MaxX, MaxY, MaxZ;
        public byte ID;
        
        public ZoneConfig Config;
        public ZoneAccessController Access;
        public string ColoredName { get { return Config.Color + Config.Name; } }
        
        public Zone() {
            Config = new ZoneConfig();
            Access = new ZoneAccessController(Config);
        }
        
        
        public bool Contains(int x, int y, int z) {
            return x >= MinX && x <= MaxX && y >= MinY && y <= MaxY && z >= MinZ && z <= MaxZ;
        }
        
        public bool CoversMap(Level lvl) {
            return MinX == 0 && MinY == 0 && MinZ == 0 &&
                MaxX == lvl.Width - 1 && MaxY == lvl.Height - 1 && MaxZ == lvl.Length - 1;
        }
        
        public bool Shows { get { return Config.ShowAlpha != 0 && Config.ShowColor.Length > 0; } }
        public void Show(Player p) {
            if (!p.Supports(CpeExt.SelectionCuboid) || !Shows) return;
            
            ColorDesc col; Colors.TryParseHex(Config.ShowColor, out col);
            Vec3U16 min = new Vec3U16(MinX, MinY, MinZ);
            Vec3U16 max = new Vec3U16((ushort)(MaxX + 1), (ushort)(MaxY + 1), (ushort)(MaxZ + 1));
            p.Send(Packet.MakeSelection(ID, Config.Name, min, max,
                col.R, col.G, col.B, (byte)Config.ShowAlpha, p.hasCP437));
        }
        
        public void ShowAll(Level lvl) {
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player p in players) {
                if (p.level == lvl) Show(p);
            }
        }
        
        public void Unshow(Player p) {
            if (!p.Supports(CpeExt.SelectionCuboid) || !Shows) return;
            p.Send(Packet.DeleteSelection(ID));
        }
        
        public void UnshowAll(Level lvl) {
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player p in players) {
                if (p.level == lvl) Unshow(p);
            }
        }
        
        public void AddTo(Level level) {
            lock (level.Zones.locker) {
                ID = NextFreeZoneId(level);
                level.Zones.Add(this);
            }
        }
        
        public void RemoveFrom(Level level) {
            lock (level.Zones.locker) {
                UnshowAll(level);
                level.Zones.Remove(this);
            }
            
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player pl in players) {
                if (pl.ZoneIn != this) continue;
                pl.ZoneIn = null;
                OnChangedZoneEvent.Call(pl);
            }
        }
        
        unsafe byte NextFreeZoneId(Level level) {
            byte* used = stackalloc byte[256];
            for (int i = 0; i < 256; i++) used[i] = 0;

            Zone[] zones = level.Zones.Items;
            for (int i = 0; i < zones.Length; i++) {
                byte id = zones[i].ID;
                used[id] = 1;
            }
            
            for (byte i = 0; i < 255; i++ ) {
                if (used[i] == 0) return i;
            }
            return 255;
        }
    }
}