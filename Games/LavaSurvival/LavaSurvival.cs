/*
	Copyright 2011 MCForge
		
	Dual-licensed under the	Educational Community License, Version 2.0 and
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

namespace MCGalaxy.Games
{
    public sealed partial class LavaSurvival
    {
        // Private variables
        private string propsPath = "properties/lavasurvival/";
        private List<string> maps, voted;
        private Dictionary<string, int> votes, deaths;
        private Random rand = new Random();
        private Timer announceTimer, voteTimer, transferTimer;
        private DateTime startTime;

        // Public variables
        public bool active = false, roundActive = false, flooded = false, voteActive = false, sendingPlayers = false;
        public Level map;
        public MapSettings mapSettings;
        public MapData mapData;

        // Settings
        public bool startOnStartup, sendAfkMain;
        public byte voteCount;
        public int lifeNum;
        public double voteTime;
        public LevelPermission setupRank, controlRank;

        // Plugin event delegates
        public delegate void GameStartHandler(Level map);
        public delegate void GameStopHandler();
        public delegate void MapChangeHandler(Level oldmap, Level newmap); // Keep in mind oldmap will be unloaded after this event finishes.
        public delegate void LavaFloodHandler(ushort x, ushort y, ushort z); // Only called on normal flood, not layer flood.
        public delegate void LayerFloodHandler(ushort x, ushort y, ushort z);
        public delegate void RoundStartHandler(Level map);
        public delegate void RoundEndHandler();
        public delegate void VoteStartHandler(string[] options);
        public delegate void VoteEndHandler(string winner);
        public delegate void PlayerDeathHandler(Player p); // Only called when the plaer is out of the round, not when they lose a life.

        // Plugin events
        public event GameStartHandler OnGameStart;
        public event GameStopHandler OnGameStop;
        public event MapChangeHandler OnMapChange;
        public event LavaFloodHandler OnLavaFlood;
        public event LayerFloodHandler OnLayerFlood;
        public event RoundStartHandler OnRoundStart;
        public event RoundEndHandler OnRoundEnd;
        public event VoteStartHandler OnVoteStart;
        public event VoteEndHandler OnVoteEnd;
        public event PlayerDeathHandler OnPlayerDeath;

        // Constructors
        public LavaSurvival()
        {
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

            startOnStartup = false;
            sendAfkMain = true;
            voteCount = 2;
            voteTime = 2;
            lifeNum = 3;
            setupRank = LevelPermission.Admin;
            controlRank = LevelPermission.Operator;
            LoadSettings();
        }

        // Public methods
        public byte Start(string mapName = "")
        {
            if (active) return 1; // Already started
            if (maps.Count < 3) return 2; // Not enough maps
            if (!String.IsNullOrEmpty(mapName) && !HasMap(mapName)) return 3; // Map doesn't exist

            deaths.Clear();
            active = true;
            Server.s.Log("[Lava Survival] Game started.");
            try { LoadMap(String.IsNullOrEmpty(mapName) ? maps[rand.Next(maps.Count)] : mapName); }
            catch (Exception e) { Server.ErrorLog(e); active = false; return 4; }
            if (OnGameStart != null)
                OnGameStart(map);
            return 0;
        }
        public byte Stop()
        {
            if (!active) return 1; // Not started

            if (OnGameStop != null)
                OnGameStop();
            active = false;
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
            map.Unload(true, false);
            map = null;
            Server.s.Log("[Lava Survival] Game stopped.");
            return 0;
        }

        public void StartRound()
        {
            if (roundActive) return;

            if (OnRoundStart != null)
                OnRoundStart(map);
            try
            {
                deaths.Clear();
                mapData.roundTimer.Elapsed += delegate { EndRound(); };
                mapData.floodTimer.Elapsed += delegate { DoFlood(); };
                mapData.roundTimer.Start();
                mapData.floodTimer.Start();
                announceTimer.Start();
                startTime = DateTime.Now;
                roundActive = true;
                Server.s.Log("[Lava Survival] Round started. Map: " + map.name);
            }
            catch (Exception e) { Server.ErrorLog(e); }
        }

        public void EndRound()
        {
            if (!roundActive) return;

            if (OnRoundEnd != null)
                OnRoundEnd();
            roundActive = false;
            flooded = false;
            try
            {
                try { mapData.Dispose(); }
                catch { }
                map.setPhysics(5);
                map.ChatLevel("The round has ended!");
                Server.s.Log("[Lava Survival] Round ended. Voting...");
                StartVote();
            }
            catch (Exception e) { Server.ErrorLog(e); }
        }

        public void DoFlood()
        {
            if (!active || !roundActive || flooded || map == null) return;
            flooded = true;

            try
            {
                announceTimer.Stop();
                map.ChatLevel("&4Look out, here comes the flood!");
                Server.s.Log("[Lava Survival] Map flooding.");
                if (mapData.layer)
                {
                    DoFloodLayer();
                    mapData.layerTimer.Elapsed += delegate
                    {
                        if (mapData.currentLayer <= mapSettings.layerCount)
                        {
                            DoFloodLayer();
                        }
                        else
                            mapData.layerTimer.Stop();
                    };
                    mapData.layerTimer.Start();
                }
                else
                {
                    map.Blockchange((ushort)mapSettings.blockFlood.X, (ushort)mapSettings.blockFlood.Y, (ushort)mapSettings.blockFlood.Z, mapData.block, true);
                    if (OnLavaFlood != null)
                        OnLavaFlood((ushort)mapSettings.blockFlood.X, (ushort)mapSettings.blockFlood.Y, (ushort)mapSettings.blockFlood.Z);
                }
            }
            catch (Exception e) { Server.ErrorLog(e); }
        }

        public void DoFloodLayer()
        {
            map.ChatLevel("&4Layer " + mapData.currentLayer + " flooding...");
            Server.s.Log("[Lava Survival] Layer " + mapData.currentLayer + " flooding.");
            map.Blockchange((ushort)mapSettings.blockLayer.X, (ushort)(mapSettings.blockLayer.Y + ((mapSettings.layerHeight * mapData.currentLayer) - 1)), (ushort)mapSettings.blockLayer.Z, mapData.block, true);
            if (OnLayerFlood != null)
                OnLayerFlood((ushort)mapSettings.blockLayer.X, (ushort)(mapSettings.blockLayer.Y + ((mapSettings.layerHeight * mapData.currentLayer) - 1)), (ushort)mapSettings.blockLayer.Z);
            mapData.currentLayer++;
        }

        public void AnnounceTimeLeft(bool flood, bool round, Player p = null, bool console = false)
        {
            if (!active || !roundActive || startTime == null || map == null) return;

            if (flood)
            {
                double floodMinutes = Math.Ceiling((startTime.AddMinutes(mapSettings.floodTime) - DateTime.Now).TotalMinutes);
                if (p == null && !console) map.ChatLevel("&3" + floodMinutes + " minute" + (floodMinutes == 1 ? "" : "s") + " %Suntil the flood.");
                else Player.Message(p, "&3" + floodMinutes + " minute" + (floodMinutes == 1 ? "" : "s") + " %Suntil the flood.");
            }
            if (round)
            {
                double roundMinutes = Math.Ceiling((startTime.AddMinutes(mapSettings.roundTime) - DateTime.Now).TotalMinutes);
                if (p == null && !console) map.ChatLevel("&3" + roundMinutes + " minute" + (roundMinutes == 1 ? "" : "s") + " %Suntil the round ends.");
                else Player.Message(p, "&3" + roundMinutes + " minute" + (roundMinutes == 1 ? "" : "s") + " %Suntil the round ends.");
            }
        }

        public void AnnounceRoundInfo(Player p = null, bool console = false)
        {
            if (p == null && !console)
            {
                if (mapData.water) map.ChatLevel("The map will be flooded with &9water %Sthis round!");
                if (mapData.layer)
                {
                    map.ChatLevel("The " + (mapData.water ? "water" : "lava") + " will &aflood in layers %Sthis round!");
                    map.ChatLevelOps("There will be " + mapSettings.layerCount + " layers, each " + mapSettings.layerHeight + " blocks high.");
                    map.ChatLevelOps("There will be another layer every " + mapSettings.layerInterval + " minutes.");
                }
                if (mapData.fast) map.ChatLevel("The lava will be &cfast %Sthis round!");
                if (mapData.killer) map.ChatLevel("The " + (mapData.water ? "water" : "lava") + " will &ckill you %Sthis round!");
                if (mapData.destroy) map.ChatLevel("The " + (mapData.water ? "water" : "lava") + " will &cdestroy plants " + (mapData.water ? "" : "and flammable blocks ") + "%Sthis round!");
            }
            else
            {
                if (mapData.water) Player.Message(p, "The map will be flooded with &9water %Sthis round!");
                if (mapData.layer) Player.Message(p, "The " + (mapData.water ? "water" : "lava") + " will &aflood in layers %Sthis round!");
                if (mapData.fast) Player.Message(p, "The lava will be &cfast %Sthis round!");
                if (mapData.killer) Player.Message(p, "The " + (mapData.water ? "water" : "lava") + " will &ckill you %Sthis round!");
                if (mapData.destroy) Player.Message(p, "The " + (mapData.water ? "water" : "lava") + " will &cdestroy plants " + (mapData.water ? "" : "and flammable blocks ") + "%Sthis round!");
            }
        }

        public void LoadMap(string name)
        {
            if (String.IsNullOrEmpty(name) || !HasMap(name)) return;

            name = name.ToLower();
            Level oldMap = null;
            if (active && map != null) oldMap = map;
            Command.all.Find("load").Use(null, name);
            map = LevelInfo.Find(name);

            if (map != null)
            {
                mapSettings = LoadMapSettings(name);
                mapData = GenerateMapData(mapSettings);

                map.setPhysics(mapData.destroy ? 2 : 1);
                map.motd = "Lava Survival: " + map.name.Capitalize() + " -hax +ophax";
                map.overload = 1000000;
                map.unload = false;
                map.loadOnGoto = false;
                Level.SaveSettings(map);
            }
            
            if (active && map != null)
            {
                sendingPlayers = true;
                try
                {
                	Player[] online = PlayerInfo.Online.Items; 
                	foreach (Player pl in online) {
                	    pl.Game.RatedMap = false;
                	    pl.Game.PledgeSurvive = false;
                        if (pl.level == oldMap)
                        {
                            if (sendAfkMain && pl.IsAfk) 
                                PlayerActions.ChangeMap(pl, Server.mainLevel.name);
                            else 
                                PlayerActions.ChangeMap(pl, map.name);
                        }
                    }
                    if (OnMapChange != null)
                        OnMapChange(oldMap, map);
                    oldMap.Unload(true, false);
                }
                catch { }
                sendingPlayers = false;

                StartRound();
            }
        }

        public void StartVote()
        {
            if (maps.Count < 3) return;

            // Make sure these are cleared or bad stuff happens!
            votes.Clear();
            voted.Clear();

            byte i = 0;
            string opt, str = "";
            while (i < Math.Min(voteCount, maps.Count - 1))
            {
                opt = maps[rand.Next(maps.Count)];
                if (!votes.ContainsKey(opt) && opt != map.name)
                {
                    votes.Add(opt, 0);
                    str += Server.DefaultColor + ", &5" + opt.Capitalize();
                    i++;
                }
            }

            if (OnVoteStart != null)
            	OnVoteStart(GetVotedLevels().ToArray());
            map.ChatLevel("Vote for the next map! The vote ends in " + voteTime + " minute" + (voteTime == 1 ? "" : "s") +".");
            map.ChatLevel("Choices: " + str.Remove(0, 4));

            voteTimer = new Timer(TimeSpan.FromMinutes(voteTime).TotalMilliseconds);
            voteTimer.AutoReset = false;
            voteTimer.Elapsed += delegate
            {
                try {
                    EndVote();
                    voteTimer.Dispose();
                }
                catch (Exception e) { Server.ErrorLog(e); }
            };
            voteTimer.Start();
            voteActive = true;
        }
        
        List<string> GetVotedLevels() {
            var keys = votes.Keys;
            List<string> names = new List<string>();
            foreach (string key in keys) 
                names.Add(key);
            return names;
        }

        public void EndVote() {
            if (!voteActive) return;

            voteActive = false;
            Server.s.Log("[Lava Survival] Vote ended.");
            KeyValuePair<string, int> most = new KeyValuePair<string, int>(String.Empty, -1);
            foreach (KeyValuePair<string, int> kvp in votes)
            {
                if (kvp.Value > most.Value) most = kvp;
                map.ChatLevelOps("&5" + kvp.Key.Capitalize() + "&f: &a" + kvp.Value);
            }
            votes.Clear();
            voted.Clear();

            if (OnVoteEnd != null)
                OnVoteEnd(most.Key);
            map.ChatLevel("The vote has ended! &5" + most.Key.Capitalize() + " %Swon with &a" + most.Value + " %Svote" + (most.Value == 1 ? "" : "s") + ".");
            map.ChatLevel("You will be transferred in 5 seconds...");
            transferTimer = new Timer(5000);
            transferTimer.AutoReset = false;
            transferTimer.Elapsed += delegate
            {
                try
                {
                    LoadMap(most.Key);
                    transferTimer.Dispose();
                }
                catch (Exception e) { Server.ErrorLog(e); }
            };
            transferTimer.Start();
        }

        public bool AddVote(Player p, string vote)
        {
            if (!voteActive || voted.Contains(p.name) || !votes.ContainsKey(vote)) return false;
            int temp = votes[vote] + 1;
            votes.Remove(vote);
            votes.Add(vote, temp);
            voted.Add(p.name);
            return true;
        }

        public bool HasVote(string vote)
        {
            return voteActive && votes.ContainsKey(vote);
        }

        public bool HasPlayer(Player p)
        {
            return p.level == map;
        }
        public void KillPlayer(Player p, bool silent = false)
        {
            if (lifeNum < 1) return;
            string name = p.name.ToLower();
            if (!deaths.ContainsKey(name))
                deaths.Add(name, 0);
            deaths[name]++;
            if (!silent && IsPlayerDead(p))
            {
                if (OnPlayerDeath != null)
                    OnPlayerDeath(p);
                Player[] online = PlayerInfo.Online.Items; 
                foreach (Player pl in online) {
                    if (pl != p && HasPlayer(pl))
                        Player.Message(pl, p.ColoredName + " &4ran out of lives, and is out of the round!");
                }
                Player.Message(p, "&4You ran out of lives, and are out of the round!");
                Player.Message(p, "&4You can still watch, but you cannot build.");
            }
        }
        public bool IsPlayerDead(Player p)
        {
            string name = p.name.ToLower();
            if (lifeNum < 1 || !deaths.ContainsKey(name))
                return false;
            return (deaths[name] >= lifeNum);
        }

        public void AddMap(string name)
        {
            if (!String.IsNullOrEmpty(name) && !maps.Contains(name.ToLower()))
            {
                maps.Add(name.ToLower());
                SaveSettings();
            }
        }
        public void RemoveMap(string name)
        {
            if (maps.Contains(name.ToLower()))
            {
                maps.Remove(name.ToLower());
                SaveSettings();
            }
        }
        public bool HasMap(string name)
        {
            return maps.Contains(name.ToLower());
        }

        public bool InSafeZone(Vec3U16 pos)
        {
            return InSafeZone(pos.X, pos.Y, pos.Z);
        }

        public bool InSafeZone(ushort x, ushort y, ushort z)
        {
            if (mapSettings == null) return false;
            return x >= mapSettings.safeZone[0].X && x <= mapSettings.safeZone[1].X && y >= mapSettings.safeZone[0].Y 
                && y <= mapSettings.safeZone[1].Y && z >= mapSettings.safeZone[0].Z && z <= mapSettings.safeZone[1].Z;
        }

        // Accessors
        public string VoteString
        {
            get
            {
                if (votes.Count > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (KeyValuePair<string, int> kvp in votes)
                        sb.AppendFormat("{0}, &5{1}", Server.DefaultColor, kvp.Key.Capitalize());
                    sb.Remove(0, 4);
                    return sb.ToString();
                }
                return String.Empty;
            }
        }

        public List<string> Maps
        {
            get
            {
                return new List<string>(maps);
            }
        }
    }
}
