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
using System.Threading;

namespace MCGalaxy.Games {
    internal sealed class LSData {
        public int TimesDied;
    }
	
    class LSLevelPicker : LevelPicker {
        public List<string> maps;
        public override List<string> GetCandidateMaps() { return new List<string>(maps); }
    }
	
    public sealed partial class LSGame : RoundsGame {
        const string propsDir = "properties/lavasurvival/";
        List<string> maps;
        Random rand = new Random();
        MapData data;
        MapSettings mapSettings;
        
        public override string GameName { get { return "Lava survival"; } }
        public bool Flooded, StartOnStartup;
        public int MaxLives = 3;
        
        public LSGame() {
            Picker = new LSLevelPicker();
            maps = new List<string>();
            ((LSLevelPicker)Picker).maps = maps;
            LoadSettings();
        }
        
        LSData Get(Player p) {
            object data;
            if (!p.Extras.TryGet("MCG_LS_DATA", out data)) {
                data = new LSData();
                p.Extras.Put("MCG_LS_DATA", data);
            }
            return (LSData)data;
        }
                
        protected override List<Player> GetPlayers() {
            return Map.getPlayers();
        }
        
        public override void Start(Player p, string map, int rounds) {
            map = GetStartMap(map);
            if (map == null) {
                Player.Message(p, "No maps have been setup for lava survival yet"); return;
            }         
            if (!HasMap(map)) {
                Player.Message(p, "Given map has not been setup for lava survival"); return;
            }
            if (!SetMap(map)) {
                Player.Message(p, "Failed to load initial map!"); return;
            }
            
            RoundsLeft = rounds;
            ResetPlayerDeaths();
                     
            Logger.Log(LogType.GameActivity, "[Lava Survival] Game started.");
            HookEventHandlers();
            
            Thread t = new Thread(RunGame);
            t.Name = "MCG_LSGame";
            t.Start();
        }
        
        public override void End() {
            if (!Running) return;
            Running = false;
            UnhookEventHandlers();
            
            Flooded = false;
            ResetPlayerDeaths();
            Map.Unload(true, false);
            
            EndCommon();
        }
        
        public bool IsPlayerDead(Player p) {
            return MaxLives > 0 && Get(p).TimesDied >= MaxLives;
        }
        
        void ResetPlayerDeaths() {
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player p in players) {
                if (p.level == Map) Get(p).TimesDied = 0;
            }
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
            if (!Running || p.level != Map) return false;
            return Picker.HandlesMessage(p, message);
        }
    }
}
