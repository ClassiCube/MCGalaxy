/*
    Copyright 2011 MCForge
        
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
using System.Text;
using System.Threading;
using MCGalaxy.Commands.World;
using MCGalaxy.Maths;

namespace MCGalaxy.Games {
    public sealed partial class LSGame : RoundsGame {
        const string propsDir = "properties/lavasurvival/";
        List<string> maps;
        Dictionary<string, int> deaths = new Dictionary<string, int>();
        Random rand = new Random();
        DateTime startTime;
        MapData data;
        MapSettings mapSettings;
        
        public override string GameName { get { return "Lava survival"; } }
        public override bool Running { get { return running; } }        
        public bool running, Flooded, StartOnStartup;
        public int MaxLives = 3;
        
        public LSGame() {
            Picker = new LSLevelPicker();
            maps = new List<string>();
            ((LSLevelPicker)Picker).maps = maps;
            LoadSettings();
        }

        public bool Start(Player p, string mapName, int rounds) {
            if (running) {
                Player.Message(p, "Lava survival game already running."); return false;
            }
            
            List<string> maps = Picker.GetCandidateMaps();
            if (maps == null || maps.Count == 0) {
                Player.Message(p, "No maps have been setup for lava survival yet"); return false;
            }         
            if (mapName.Length > 0 && !HasMap(mapName)) {
                Player.Message(p, "Given map has not been setup for lava survival"); return false;
            }
            
            mapName = mapName.Length == 0 ? maps[rand.Next(maps.Count)] : mapName;
            if (!SetMap(mapName)) {
                Player.Message(p, "Failed to load initial map!"); return false;
            }
            
            RoundsLeft = rounds;
            deaths.Clear();
                     
            Logger.Log(LogType.GameActivity, "[Lava Survival] Game started.");
            running = true;
            HookEventHandlers();
            
            Thread t = new Thread(RunGame);
            t.Name = "MCG_LSGame";
            t.Start();
            return true;
        }
        
        public override void End() {
            if (!running) return;
            running = false;
            UnhookEventHandlers();
            
            Flooded = false;
            deaths.Clear();
            Map.Unload(true, false);
            
            ResetState();
            Logger.Log(LogType.GameActivity, "[Lava Survival] Game stopped.");
        }
        
        public bool IsPlayerDead(Player p) {
            string name = p.name.ToLower();
            if (MaxLives < 1 || !deaths.ContainsKey(name))
                return false;
            return (deaths[name] >= MaxLives);
        }

        public void AddMap(string name) {
            if (!String.IsNullOrEmpty(name) && !HasMap(name)) {
                maps.Add(name.ToLower());
                SaveSettings();
            }
        }
        
        public void RemoveMap(string name) {
            if (maps.CaselessRemove(name)) {
                SaveSettings();
            }
        }
        
        public bool HasMap(string name) {
            return maps.CaselessContains(name);
        }

        public bool InSafeZone(ushort x, ushort y, ushort z) {
            if (mapSettings == null) return false;
            return x >= mapSettings.safeZone[0].X && x <= mapSettings.safeZone[1].X && y >= mapSettings.safeZone[0].Y
                && y <= mapSettings.safeZone[1].Y && z >= mapSettings.safeZone[0].Z && z <= mapSettings.safeZone[1].Z;
        }

        public List<string> Maps { get { return new List<string>(maps); } }
        
        public override bool HandlesChatMessage(Player p, string message) {
            if (!running || p.level != Map) return false;
            return Picker.HandlesMessage(p, message);
        }
    }
    
    internal class LSLevelPicker : LevelPicker {
        public List<string> maps;
        public override List<string> GetCandidateMaps() { return new List<string>(maps); }
    }
}
