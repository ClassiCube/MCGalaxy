/*
    Copyright 2015 MCGalaxy
        
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
using MCGalaxy.Commands.World;
using MCGalaxy.Events.GameEvents;

namespace MCGalaxy.Games {
    
    public abstract partial class RoundsGame : IGame {
        public int RoundsLeft;
        public bool RoundInProgress;
        public DateTime RoundStart;
        public string LastMap = "";
        public LevelPicker Picker;
        
        /// <summary> Messages general info about current round and players. </summary>
        /// <remarks> e.g. who is alive, points of each team, etc. </remarks>
        public abstract void OutputStatus(Player p);
        /// <summary> The instance of this game's overall configuration object. </summary>
        public abstract RoundsGameConfig GetConfig();
        /// <summary> Updates state from the map specific configuration file. </summary>
        public abstract void UpdateMapConfig();
        
        /// <summary> Runs a single round of this game. </summary>
        protected abstract void DoRound();
        /// <summary> Gets the list of all players in this game. </summary>
        protected abstract List<Player> GetPlayers();
        /// <summary> Saves stats to the database for the given player. </summary>
        protected virtual void SaveStats(Player pl) { }
        
        public override bool HandlesChatMessage(Player p, string message) {
            if (!Running || p.level != Map) return false;
            return Picker.HandlesMessage(p, message);
        }
        
        protected abstract void StartGame();
        public virtual void Start(Player p, string map, int rounds) {
            map = GetStartMap(p, map);
            if (map == null) {
                p.Message("No maps have been setup for {0} yet", GameName); return;
            }
            if (!SetMap(map)) {
                p.Message("Failed to load initial map!"); return;
            }
            
            Chat.MessageGlobal("{0} is starting on {1}&S!", GameName, Map.ColoredName);
            Logger.Log(LogType.GameActivity, "[{0}] Game started", GameName);
            
            StartGame();
            RoundsLeft = rounds;
            Running = true;
            
            IGame.RunningGames.Add(this);
            OnStateChangedEvent.Call(this);
            HookEventHandlers();
            
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player pl in players) {
                if (pl.level == Map) PlayerJoinedGame(pl);
            }
            
            Thread t = new Thread(RunGame);
            t.Name = "MCG_" + GameName;
            t.Start();
        }

        /// <summary> Attempts to auto start this game with infinite rounds. </summary>
        public void AutoStart() {
            if (!GetConfig().StartImmediately) return;
            try {
                Start(Player.Console, "", int.MaxValue);
            } catch (Exception ex) { 
                Logger.LogError("Error auto-starting " + GameName, ex); 
            }
        }
        
        protected virtual string GetStartMap(Player p, string forcedMap) {
            if (forcedMap.Length > 0) return forcedMap;
            List<string> maps = Picker.GetCandidateMaps(this);
            
            if (maps == null || maps.Count == 0) return null;
            return LevelPicker.GetRandomMap(new Random(), maps);
        }
        
        void RunGame() {
            try {
                while (Running && RoundsLeft > 0) {
                    RoundInProgress = false;
                    if (RoundsLeft != int.MaxValue) RoundsLeft--;
                    
                    DoRound();
                    if (Running) EndRound();
                    if (Running) VoteAndMoveToNextMap();
                }
                End();
            } catch (Exception ex) {
                Logger.LogError("Error in game " + GameName, ex);
                Chat.MessageGlobal("&W" + GameName + " disabled due to an error.");
                
                try { End(); }
                catch (Exception ex2) { Logger.LogError(ex2); }
            }
            IGame.RunningGames.Remove(this);
        }
        
        protected virtual bool SetMap(string map) {
            Picker.QueuedMap = null;
            Level next = LevelInfo.FindExact(map);
            if (next == null) next = LevelActions.Load(Player.Console, map, false);
            if (next == null) return false;
            
            Map = next;
            Map.SaveChanges = false;
            
            if (GetConfig().SetMainLevel) Server.SetMainLevel(Map);
            UpdateMapConfig();
            return true;
        }
        
        protected void DoCountdown(string format, int delay, int minThreshold) {
            const CpeMessageType type = CpeMessageType.Announcement;
            for (int i = delay; i > 0 && Running; i--) {
                if (i == 1) {
                    MessageMap(type, String.Format(format, i)
                               .Replace("seconds", "second"));
                } else if (i < minThreshold || (i % 10) == 0) {
                    MessageMap(type, String.Format(format, i));
                }
                Thread.Sleep(1000);
            }
            MessageMap(type, "");
        }
        
        protected List<Player> DoRoundCountdown(int delay) {
            while (true) {
                RoundStart = DateTime.UtcNow.AddSeconds(delay);
                if (!Running) return null;

                DoCountdown("&4Starting in &f{0} &4seconds", delay, 10);
                if (!Running) return null;
                
                List<Player> players = GetPlayers();
                if (players.Count >= 2) return players;
                Map.Message("&WNeed 2 or more non-ref players to start a round.");
            }
        }
        
        protected virtual void VoteAndMoveToNextMap() {
            Picker.AddRecentMap(Map.MapName);
            if (RoundsLeft == 0) return;
            
            string map = Picker.ChooseNextLevel(this);
            if (!Running) return;
            
            if (map == null || map.CaselessEq(Map.MapName)) { 
                ContinueOnSameMap(); return; 
            }
            
            AnnounceMapChange(map);
            Level lastMap = Map; LastMap = Map.MapName;
            
            if (!SetMap(map)) {
                Map.Message("&WFailed to change map to " + map);
                ContinueOnSameMap();
            } else {
                TransferPlayers(lastMap);
                lastMap.Unload();
            }
        }
        
        protected virtual void AnnounceMapChange(string newMap) {
            Map.Message("The next map has been chosen - &c" + newMap);
            Map.Message("Please wait while you are transfered.");
        }
        
        protected virtual void ContinueOnSameMap() {
            Map.Message("Continuing " + GameName + " on the same map");
            Level old = Level.Load(Map.MapName);
            
            if (old == null) {
                Map.Message("&WCannot reset changes to map"); return;
            }
            if (old.Width != Map.Width || old.Height != Map.Height || old.Length != Map.Length) {
                Map.Message("&WCannot reset changes to map"); return;
            }
            
            // Try to reset changes made to this map, if possible
            // TODO: do this in a nicer way
            Map.blocks = old.blocks;
            Map.CustomBlocks = old.CustomBlocks;
            LevelActions.ReloadAll(Map, Player.Console, false);
            Map.Message("Reset map to latest backup");
        }
        
        void TransferPlayers(Level lastMap) {
            Random rnd = new Random();
            Player[] online = PlayerInfo.Online.Items;
            List<Player> transfers = new List<Player>(online.Length);
            
            foreach (Player pl in online) {
                pl.Game.RatedMap = false;
                pl.Game.PledgeSurvive = false;
                if (pl.level != Map && pl.level == lastMap) transfers.Add(pl);
            }
            
            while (transfers.Count > 0) {
                int i = rnd.Next(0, transfers.Count);
                Player pl = transfers[i];
                
                pl.Message("Going to the next map - &a" + Map.MapName);
                PlayerActions.ChangeMap(pl, Map);
                transfers.RemoveAt(i);
            }
        }
        
        protected abstract void EndGame();
        public override void End() {
            if (!Running) return;
            Running = false;
            IGame.RunningGames.Remove(this);
            UnhookEventHandlers();
            
            if (RoundInProgress) {
                EndRound();
                RoundInProgress = false;
            }
            
            EndGame();
            OnStateChangedEvent.Call(this);
            
            RoundStart = DateTime.MinValue;
            RoundsLeft = 0;
            RoundInProgress = false;
            
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player pl in players) {
                if (pl.level != Map) continue;
                pl.Game.RatedMap = false;
                pl.Game.PledgeSurvive = false;
                PlayerLeftGame(pl);
                
                TabList.Update(pl, true);
                ResetStatus(pl);
                pl.SetPrefix();
            }
            
            // in case players left game partway through
            foreach (Player pl in players) { SaveStats(pl); }
            
            if (Map != null) Map.Message(GameName + " &Sgame ended");
            Logger.Log(LogType.GameActivity, "[{0}] Game ended", GameName);
            if (Picker != null) Picker.Clear();
            
            LastMap = "";
            if (Map != null) Map.AutoUnload();
            Map = null;
        }
        
        protected void UpdateAllMotd() {
            List<Player> players = GetPlayers();
            foreach (Player p in players) {
                if (p.Supports(CpeExt.InstantMOTD)) p.SendMapMotd();
            }
        }
    }
}
