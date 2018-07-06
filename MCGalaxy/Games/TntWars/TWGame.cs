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
///////--|----------------------------------|--\\\\\\\
//////---|  TNT WARS - Coded by edh649      |---\\\\\\
/////----|                                  |----\\\\\
////-----|  Note: Double click on // to see |-----\\\\
///------|        them in the sidebar!!     |------\\\
//-------|__________________________________|-------\\
using System;
using System.Collections.Generic;
using System.Threading;
using MCGalaxy.Commands.World;
using MCGalaxy.Events;
using MCGalaxy.Events.LevelEvents;
using MCGalaxy.Events.PlayerEvents;
using MCGalaxy.Maths;
using MCGalaxy.Network;
using BlockID = System.UInt16;

namespace MCGalaxy.Games {
    
    public enum TWGameMode { FFA, TDM };
    public enum TWDifficulty {
        Easy,    // 2 Hits to die, Tnt has long delay
        Normal,  // 2 Hits to die, Tnt has normal delay
        Hard,    // 1 Hit to die, Tnt has short delay
        Extreme, // 1 Hit to die, Tnt has short delay and BIG exlosion
    }
    public sealed class PlayerAndScore { public Player p; public int Score; }
    
    internal sealed class TWData {
        public int Score, Health = 2, KillStreak, TNTCounter;
        public float ScoreMultiplier = 1f;
        public int LastKillStreakAnnounced;
        public Player HarmedBy; // For Assists
        public string OrigCol;
        
        public void Reset(TWDifficulty diff) {
            bool easyish = diff == TWDifficulty.Easy || diff == TWDifficulty.Normal;
            Score = 0;
            Health = easyish ? 2 : 1;
            KillStreak = 0;
            LastKillStreakAnnounced = 0;
            TNTCounter = 0;
            ScoreMultiplier = 1f;
            HarmedBy = null;
        }
    }
    
    public sealed partial class TWGame : RoundsGame {
        TWMapConfig cfg = new TWMapConfig();
        public static TWConfig Config = new TWConfig();
        public override string GameName { get { return "TNT Wars"; } }
        public override RoundsGameConfig GetConfig() { return Config; }
        VolatileArray<Player> allPlayers = new VolatileArray<Player>(false);
        
        sealed class TWTeam {
            public string Name, Color;
            public string ColoredName { get { return Color + Name; } }
            public int Score;
            public Vec3U16 SpawnPos;
            public VolatileArray<Player> Members = new VolatileArray<Player>();
            
            public TWTeam(string name, string color) { Name = name; Color = color; }
        }
        
        TWTeam Red  = new TWTeam("Red", Colors.red);
        TWTeam Blue = new TWTeam("Blue", Colors.blue);
        public List<TWZone> tntFreeZones = new List<TWZone>();
        public List<TWZone> tntImmuneZones = new List<TWZone>();
        
        public static TWGame Instance = new TWGame();
        public TWGame() { Picker = new LevelPicker(); }
        
        const string twExtrasKey = "MCG_TW_DATA";
        static TWData Get(Player p) {
            TWData data = TryGet(p);
            if (data != null) return data;
            data = new TWData();
            
            // TODO: Is this even thread-safe
            data.OrigCol = p.color;
            
            p.Extras.Put(twExtrasKey, data);
            return data;
        }
        
        static TWData TryGet(Player p) {
            object data; p.Extras.TryGet(twExtrasKey, out data); return (TWData)data;
        }
        
        public void UpdateMapConfig() {
            TWMapConfig cfg = new TWMapConfig();
            cfg.SetDefaults(Map);
            cfg.Load(Map.name);
            
            this.cfg = cfg;
            Red.SpawnPos = cfg.RedSpawn;
            Blue.SpawnPos = cfg.BlueSpawn;
        }
        
        protected override List<Player> GetPlayers() {
            List<Player> playing = new List<Player>();
            playing.AddRange(Red.Members.Items);
            playing.AddRange(Blue.Members.Items);
            return playing;
        }
        
        public override void OutputStatus(Player p) {
            if (Config.Mode == TWGameMode.TDM) {
                Player.Message(p, "{0} team score: &f{1}/{2} points",
                               Red.ColoredName, Red.Score, cfg.ScoreRequired);
                Player.Message(p, "{0} team score: &f{1}/{2} points",
                               Blue.ColoredName, Blue.Score, cfg.ScoreRequired);
            }
            Player.Message(p, "Your score: &f{0}/{1} %Spoints, health: &f{2} %SHP",
                           Get(p).Score, cfg.ScoreRequired, Get(p).Health);
        }

        protected override void StartGame() {
            ResetTeams();
        }
        
        protected override void EndGame() {
            if (RoundInProgress) EndRound();
            RestoreBuildPerms();
            ResetTeams();
        }
        
        void ResetTeams() {
            Blue.Members.Clear();
            Red.Members.Clear();
            Blue.Score = 0;
            Red.Score = 0;
        }
        
        public override void PlayerJoinedGame(Player p) {
            bool announce = false;
            HandleJoinedLevel(p, Map, Map, ref announce);
        }
        
        public override void PlayerLeftGame(Player p) {
            allPlayers.Remove(p);
            TWTeam team = TeamOf(p);
            
            if (team == null) return;
            team.Members.Remove(p);
            RestoreColor(p);
        }
        
        void RestoreColor(Player p) {
            TWData data = TryGet(p);
            if (data == null) return;
            
            p.color = data.OrigCol;
            p.SetPrefix();
            TabList.Update(p, true);
        }
        
        void JoinTeam(Player p, TWTeam team) {
            team.Members.Add(p);
            Map.Message(p.ColoredName + " %Sjoined the " + team.ColoredName + " %Steam");
            
            p.color = team.Color;
            Player.Message(p, "You are now on the " + team.ColoredName + " team!");
            TabList.Update(p, true);
        }
        
        TWTeam TeamOf(Player p) {
            if (Red.Members.Contains(p)) return Red;
            if (Blue.Members.Contains(p)) return Blue;
            return null;
        }
        
        
        public void ModeTDM() {
            Config.Mode = TWGameMode.TDM;
            MessageMap(CpeMessageType.Announcement,
                       "&4Gamemode changed to &fTeam Deathmatch");
            Player[] players = allPlayers.Items;
            
            foreach (Player pl in players) {
                string msg = pl.ColoredName + " %Sis now on the ";
                AutoAssignTeam(pl);
                
                // assigning team changed colour of player
                msg += TeamOf(pl).ColoredName + " team";
                Map.Message(msg);
            }
            Config.Save();
        }
        
        public void ModeFFA() {
            Config.Mode = TWGameMode.FFA;
            MessageMap(CpeMessageType.Announcement,
                       "&4Gamemode changed to &fFree For All");
            ResetTeams();
            
            Player[] players = allPlayers.Items;
            foreach (Player pl in players) {
                RestoreColor(pl);
            }
            Config.Save();
        }
        
        public void SetDifficulty(TWDifficulty diff) {
            Config.Difficulty = diff;
            MessageMap(CpeMessageType.Announcement,
                       "&4Difficulty changed to &f" + diff);
            Config.Save();
            
            bool teamKill = diff >= TWDifficulty.Hard;
            if (cfg.TeamKills == teamKill) return;
            
            cfg.TeamKills = teamKill;
            cfg.Save(Map.name);
        }
        
        public class TWZone { public ushort MinX, MinY, MinZ, MaxX, MaxY, MaxZ; }       
        public bool InZone(ushort x, ushort y, ushort z, List<TWZone> zones) {
            foreach (TWZone Zn in zones) {
                if (x >= Zn.MinX && y >= Zn.MinY && z >= Zn.MinZ
                    && x <= Zn.MaxX && y <= Zn.MaxY && z <= Zn.MaxZ) return true;
            }
            return false;
        }
        
        void AutoAssignTeam(Player p) {
            if (Blue.Members.Count > Red.Members.Count) {
                JoinTeam(p, Red);
            } else if (Red.Members.Count > Blue.Members.Count) {
                JoinTeam(p, Blue);
            } else if (Red.Score > Blue.Score) {
                JoinTeam(p, Blue);
            } else if (Blue.Score > Red.Score) {
                JoinTeam(p, Blue);
            } else {
                bool red = new Random().Next(2) == 0;
                JoinTeam(p, red ? Red : Blue);
            }
        }
        
        public PlayerAndScore[] SortedByScore() {
            Player[] all = allPlayers.Items;
            PlayerAndScore[] sorted = new PlayerAndScore[all.Length];
            
            for (int i = 0; i < all.Length; i++) {
                PlayerAndScore entry = new PlayerAndScore();
                entry.p = all[i];
                entry.Score = Get(entry.p).Score;
                sorted[i] = entry;
            }
            
            Array.Sort(sorted, (a, b) => b.Score.CompareTo(a.Score));
            return sorted;
        }
        
        public string FormatTopScore(PlayerAndScore[] top, int i) {
            string col = "&f";
            PlayerAndScore p = top[i];
            
            if (i == 0) col = "&6";
            if (i == 1) col = "&7";
            if (i == 2) col = "&4";
            
            return String.Format("{0}) {2} - {1}{3} points", i + 1, col,
                                 p.p.ColoredName, p.Score);
        }
        
        public void ChangeScore(Player p, int amount) {
            Get(p).Score += amount;
            if (Config.Mode != TWGameMode.TDM) return;
            
            TWTeam team = TeamOf(p);
            if (team != null) team.Score += amount;
        }
        
        public bool TeamKill(Player p1, Player p2) {
            return Config.Mode == TWGameMode.TDM && TeamOf(p1) == TeamOf(p2);
        }
    }
}
