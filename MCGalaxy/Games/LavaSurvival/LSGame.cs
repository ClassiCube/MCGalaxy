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
        public override List<string> GetCandidateMaps() { 
            return new List<string>(LSGame.Config.Maps); 
        }
    }
    
    public sealed partial class LSGame : RoundsGame {
        const string propsDir = "properties/lavasurvival/";
        Random rand = new Random();
        MapData data;
        MapSettings mapSettings;
        
        public override string GameName { get { return "Lava survival"; } }
        public bool Flooded;
        
        public LSGame() { Picker = new LSLevelPicker(); }
        
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
        
        protected override void StartGame() {
            ResetPlayerDeaths();
        }
        
        protected override void EndGame() {
            Flooded = false;
            ResetPlayerDeaths();
        }
        
        public bool IsPlayerDead(Player p) {
            return Config.MaxLives > 0 && Get(p).TimesDied >= Config.MaxLives;
        }
        
        void ResetPlayerDeaths() {
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player p in players) {
                if (p.level == Map) Get(p).TimesDied = 0;
            }
        }

        public void AddMap(string name) {
            if (!String.IsNullOrEmpty(name) && !HasMap(name)) {
                Config.Maps.Add(name);
                Config.Save();
            }
        }
        
        public void RemoveMap(string name) {
            if (Config.Maps.CaselessRemove(name)) Config.Save();
        }
        
        public bool HasMap(string name) {
            return Config.Maps.CaselessContains(name);
        }

        public bool InSafeZone(ushort x, ushort y, ushort z) {
            if (mapSettings == null) return false;
            return x >= mapSettings.safeZone[0].X && x <= mapSettings.safeZone[1].X && y >= mapSettings.safeZone[0].Y
                && y <= mapSettings.safeZone[1].Y && z >= mapSettings.safeZone[0].Z && z <= mapSettings.safeZone[1].Z;
        }
        
        public override void PlayerJoinedGame(Player p) {
            bool announce = false;
            HandleJoinedLevel(p, Map, Map, ref announce);
        }
        
        public override bool HandlesChatMessage(Player p, string message) {
            if (!Running || p.level != Map) return false;
            return Picker.HandlesMessage(p, message);
        }
    }
}
