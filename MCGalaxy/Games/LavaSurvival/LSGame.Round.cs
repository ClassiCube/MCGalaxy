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

        public void StartRound() {
            if (roundActive) return;

            try {
                deaths.Clear();
                mapData.roundTimer.Elapsed += delegate { EndRound(); };
                mapData.floodTimer.Elapsed += delegate { DoFlood(); };
                mapData.roundTimer.Start();
                mapData.floodTimer.Start();
                announceTimer.Start();
                startTime = DateTime.UtcNow;
                roundActive = true;
                Logger.Log(LogType.GameActivity, "[Lava Survival] Round started. Map: " + Map.ColoredName);
            }
            catch (Exception e) { Logger.LogError(e); }
        }
		
		protected override void DoRound() {
			if (!running) return;
		}
		
		// RTound format
		// announce / round countdown
		// flood / flood layers
		// vote for next

        public override void EndRound() {
            if (!roundActive) return;

            roundActive = false;
            flooded = false;
            
            try {
                try { mapData.Dispose(); }
                catch { }
                Map.SetPhysics(5);
                Map.ChatLevel("The round has ended!");
                Logger.Log(LogType.GameActivity, "[Lava Survival] Round ended. Voting...");
                StartVote();
            }
            catch (Exception e) { Logger.LogError(e); }
        }

        public void DoFlood()
        {
            if (!running || !roundActive || flooded || Map == null) return;
            flooded = true;

            try
            {
                announceTimer.Stop();
                Map.ChatLevel("&4Look out, here comes the flood!");
                Logger.Log(LogType.GameActivity, "[Lava Survival] Map flooding.");
                if (mapData.layer)
                {
                    DoFloodLayer();
                    mapData.layerTimer.Elapsed += delegate
                    {
                        if (mapData.currentLayer <= mapSettings.LayerCount)
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
                    Map.Blockchange(mapSettings.FloodPos.X, mapSettings.FloodPos.Y, mapSettings.FloodPos.Z, mapData.block, true);
                }
            }
            catch (Exception e) { Logger.LogError(e); }
        }

        void DoFloodLayer()  {
            Logger.Log(LogType.GameActivity, "[Lava Survival] Layer " + mapData.currentLayer + " flooding.");
            Map.Blockchange(mapSettings.LayerPos.X, (ushort)(mapSettings.LayerPos.Y + ((mapSettings.LayerHeight * mapData.currentLayer) - 1)), mapSettings.LayerPos.Z, mapData.block, true);
            mapData.currentLayer++;
        }

        public void AnnounceTimeLeft(bool flood, bool round, Player p = null, bool console = false) {
            if (!running || !roundActive || startTime == null || Map == null) return;

            if (flood) {
                double floodMinutes = Math.Ceiling((startTime.AddMinutes(mapSettings.floodTime) - DateTime.UtcNow).TotalMinutes);
                if (p == null && !console) Map.ChatLevel("&3" + floodMinutes + " minute" + (floodMinutes == 1 ? "" : "s") + " %Suntil the flood.");
                else Player.Message(p, "&3" + floodMinutes + " minute" + (floodMinutes == 1 ? "" : "s") + " %Suntil the flood.");
            }
            if (round) {
                double roundMinutes = Math.Ceiling((startTime.AddMinutes(mapSettings.roundTime) - DateTime.UtcNow).TotalMinutes);
                if (p == null && !console) Map.ChatLevel("&3" + roundMinutes + " minute" + (roundMinutes == 1 ? "" : "s") + " %Suntil the round ends.");
                else Player.Message(p, "&3" + roundMinutes + " minute" + (roundMinutes == 1 ? "" : "s") + " %Suntil the round ends.");
            }
        }

        public void AnnounceRoundInfo(Player p = null, bool console = false)  {
            if (p == null && !console) {
                if (mapData.water) Map.ChatLevel("The map will be flooded with &9water %Sthis round!");
                if (mapData.layer)
                {
                    Map.ChatLevel("The " + (mapData.water ? "water" : "lava") + " will &aflood in layers %Sthis round!");
                    Map.ChatLevelOps("There will be " + mapSettings.LayerCount + " layers, each " + mapSettings.LayerHeight + " blocks high.");
                    Map.ChatLevelOps("There will be another layer every " + mapSettings.layerInterval + " minutes.");
                }
                if (mapData.fast) Map.ChatLevel("The lava will be &cfast %Sthis round!");
                if (mapData.killer) Map.ChatLevel("The " + (mapData.water ? "water" : "lava") + " will &ckill you %Sthis round!");
                if (mapData.destroy) Map.ChatLevel("The " + (mapData.water ? "water" : "lava") + " will &cdestroy plants " + (mapData.water ? "" : "and flammable blocks ") + "%Sthis round!");
            } else {
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
            if (running && Map != null) oldMap = Map;
            CmdLoad.LoadLevel(null, name);
            Map = LevelInfo.FindExact(name);

            if (Map != null) {
                mapSettings = LoadMapSettings(name);
                mapData = GenerateMapData(mapSettings);
                Map.SaveChanges = false;
                
                Map.SetPhysics(mapData.destroy ? 2 : 1);
                Map.Config.PhysicsOverload = 1000000;
                Map.Config.LoadOnGoto = false;
                Level.SaveSettings(Map);
            }
            
            if (running && Map != null)
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
                                PlayerActions.ChangeMap(pl, Server.mainLevel);
                            else 
                                PlayerActions.ChangeMap(pl, Map);
                        }
                    }
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
                if (!votes.ContainsKey(opt) && opt != Map.name)
                {
                    votes.Add(opt, 0);
                    str += "%S, &5" + opt.Capitalize();
                    i++;
                }
            }

            Map.ChatLevel("Vote for the next map! The vote ends in " + voteTime + " minute" + (voteTime == 1 ? "" : "s") +".");
            Map.ChatLevel("Choices: " + str.Remove(0, 4));

            voteTimer = new Timer(TimeSpan.FromMinutes(voteTime).TotalMilliseconds);
            voteTimer.AutoReset = false;
            voteTimer.Elapsed += delegate
            {
                try {
                    EndVote();
                    voteTimer.Dispose();
                }
                catch (Exception e) { Logger.LogError(e); }
            };
            voteTimer.Start();
            voteActive = true;
        }

        public void EndVote() {
            if (!voteActive) return;

            voteActive = false;
            Logger.Log(LogType.GameActivity, "[Lava Survival] Vote ended.");
            KeyValuePair<string, int> most = new KeyValuePair<string, int>(String.Empty, -1);
            foreach (KeyValuePair<string, int> kvp in votes)
            {
                if (kvp.Value > most.Value) most = kvp;
                Map.ChatLevelOps("&5" + kvp.Key.Capitalize() + "&f: &a" + kvp.Value);
            }
            votes.Clear();
            voted.Clear();

            Map.ChatLevel("The vote has ended! &5" + most.Key.Capitalize() + " %Swon with &a" + most.Value + " %Svote" + (most.Value == 1 ? "" : "s") + ".");
            Map.ChatLevel("You will be transferred in 5 seconds...");
            transferTimer = new Timer(5000);
            transferTimer.AutoReset = false;
            transferTimer.Elapsed += delegate
            {
                try
                {
                    LoadMap(most.Key);
                    transferTimer.Dispose();
                }
                catch (Exception e) { Logger.LogError(e); }
            };
            transferTimer.Start();
        }

        public bool AddVote(Player p, string vote) {
            if (!voteActive || voted.Contains(p.name) || !votes.ContainsKey(vote)) return false;
            int temp = votes[vote] + 1;
            votes.Remove(vote);
            votes.Add(vote, temp);
            voted.Add(p.name);
            return true;
        }

        public bool HasVote(string vote) {
            return voteActive && votes.ContainsKey(vote);
        }
        
        void KillPlayer(Player p) {
            if (lifeNum < 1) return;
            string name = p.name.ToLower();
            if (!deaths.ContainsKey(name)) deaths.Add(name, 0);
            deaths[name]++;
            if (!IsPlayerDead(p)) return;
            
            Player[] online = PlayerInfo.Online.Items; 
            foreach (Player pl in online) {
            	if (pl != p && pl.level == Map) {
                    Player.Message(pl, p.ColoredName + " &4ran out of lives, and is out of the round!");
            	}
            }
            
            Player.Message(p, "&4You ran out of lives, and are out of the round!");
            Player.Message(p, "&4You can still watch, but you cannot build.");
        }
    }
}
