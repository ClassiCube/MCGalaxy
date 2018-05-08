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
using System.Timers;
using MCGalaxy.Commands.World;
using MCGalaxy.Maths;

namespace MCGalaxy.Games {
    public sealed partial class LSGame : RoundsGame {
        // Private variables
        const string propsDir = "properties/lavasurvival/";
        List<string> maps, voted;
        Dictionary<string, int> votes, deaths;
        Random rand = new Random();
        Timer announceTimer, voteTimer, transferTimer;
        DateTime startTime;

        // Public variables
        public bool running = false, roundActive = false, flooded = false, voteActive = false, sendingPlayers = false;
        public MapSettings mapSettings;
        public MapData mapData;
        
        public override string GameName { get { return "Lava survival"; } }
        public override bool Running { get { return running; } }

        // Settings
        public bool startOnStartup, sendAfkMain = true;
        public byte voteCount = 2;
        public int lifeNum = 3;
        public double voteTime = 2;

        // Constructors
        public LSGame() {
            maps = new List<string>();
            voted = new List<string>();
            votes = new Dictionary<string, int>();
            deaths = new Dictionary<string, int>();
            announceTimer = new Timer(60000);
            announceTimer.AutoReset = true;
            announceTimer.Elapsed += delegate
            {
                if (!flooded) AnnounceTimeLeft(true, false);
            };
            LoadSettings();
        }

        // Public methods
        public byte Start(string mapName = "")
        {
            if (running) return 1; // Already started
            if (maps.Count < 3) return 2; // Not enough maps
            if (!String.IsNullOrEmpty(mapName) && !HasMap(mapName)) return 3; // Map doesn't exist

            deaths.Clear();
            running = true;
            Logger.Log(LogType.GameActivity, "[Lava Survival] Game started.");
            
            try { 
                LoadMap(String.IsNullOrEmpty(mapName) ? maps[rand.Next(maps.Count)] : mapName); 
                HookEventHandlers();
            } catch (Exception e) { 
                Logger.LogError(e); running = false; return 4; 
            }
            return 0;
        }
        
        public override void End() {
            if (!running) return;
            UnhookEventHandlers();

            running = false;
            roundActive = false;
            voteActive = false;
            flooded = false;
            deaths.Clear();
            if (announceTimer.Enabled) announceTimer.Stop();
            try { mapData.Dispose(); }
            catch { }
            try { voteTimer.Dispose(); }
            catch { }
            try { transferTimer.Dispose(); }
            catch { }
            Map.Unload(true, false);
            Map = null;
            Logger.Log(LogType.GameActivity, "[Lava Survival] Game stopped.");
        }
        
        public bool IsPlayerDead(Player p) {
            string name = p.name.ToLower();
            if (lifeNum < 1 || !deaths.ContainsKey(name))
                return false;
            return (deaths[name] >= lifeNum);
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
            message = message.ToLower();
            if (!HasVote(message)) return false;
            
            if (AddVote(p, message)) {
                Player.Message(p, "Your vote for &5" + message.Capitalize() + " %Shas been placed. Thanks!");
                Map.ChatLevelOps(p.name + " voted for &5" + message.Capitalize() + "%S.");
                return true;
            } else {
                Player.Message(p, "&cYou already voted!");
                return true;
            }
        }
        
        public override void PlayerJoinedLevel(Player p, Level lvl, Level oldLevl) {
            if (running && !sendingPlayers && Map == lvl) {
                if (roundActive) {
                    AnnounceRoundInfo(p);
                    AnnounceTimeLeft(!flooded, true, p);
                } else {
                    Player.Message(p, "Vote for the next map!");
                    Player.Message(p, "Choices: &5" + votes.Keys.Join("%S, &5"));
                }
            }
        }
    }
}
