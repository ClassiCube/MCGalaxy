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
    
    public enum TntWarsGameMode { FFA, TDM };
    public enum TntWarsDifficulty {
        Easy,    // 2 Hits to die, Tnt has long delay
        Normal,  // 2 Hits to die, Tnt has normal delay
        Hard,    // 1 Hit to die, Tnt has short delay
        Extreme, // 1 Hit to die, Tnt has short delay and BIG exlosion
    }
    
    internal sealed class TntWarsData {
        public int Score, Health = 2, KillStreak;
        public float ScoreMultiplier = 1f;
        public int LastKillStreakAnnounced;
        public string HarmedBy; // For Assists
        public string OrigCol;
    }
    
    public sealed partial class TWGame : RoundsGame {
        TWMapConfig cfg = new TWMapConfig();
        public static TWConfig Config = new TWConfig();
        public override string GameName { get { return "TNT Wars"; } }
        public override RoundsGameConfig GetConfig() { return Config; }
        VolatileArray<Player> allPlayers = new VolatileArray<Player>(false);
        
        sealed class TntWarsTeam {
            public string Name, Color;
            public string ColoredName { get { return Color + Name; } }
            public int Score;
            public Vec3U16 SpawnPos;
            public VolatileArray<Player> Members = new VolatileArray<Player>();
            
            public TntWarsTeam(string name, string color) { Name = name; Color = color; }
        }
        
        TntWarsTeam Red  = new TntWarsTeam("Red", Colors.red);
        TntWarsTeam Blue = new TntWarsTeam("Blue", Colors.blue);
        public List<TntWarsGame1.Zone> NoTNTplacableZones = new List<TntWarsGame1.Zone>();
        public List<TntWarsGame1.Zone> NoBlockDeathZones = new List<TntWarsGame1.Zone>();
        
        public static TWGame Instance = new TWGame();
        public TWGame() { Picker = new LevelPicker(); }
        
        const string twExtrasKey = "MCG_TW_DATA";
        static TntWarsData Get(Player p) {
            TntWarsData data = TryGet(p);
            if (data != null) return data;
            data = new TntWarsData();
            
            // TODO: Is this even thread-safe
            data.OrigCol = p.color;
            
            p.Extras.Put(twExtrasKey, data);
            return data;
        }
        
        static TntWarsData TryGet(Player p) {
            object data; p.Extras.TryGet(twExtrasKey, out data); return (TntWarsData)data;
        }
        
        public void UpdateMapConfig() {
            TWMapConfig cfg = new TWMapConfig();
            cfg.SetDefaults(Map);
            cfg.Load(Map.name);
            this.cfg = cfg;
        }
        
        protected override List<Player> GetPlayers() {
            List<Player> playing = new List<Player>();
            playing.AddRange(Red.Members.Items);
            playing.AddRange(Blue.Members.Items);
            return playing;
        }
        
        public override void OutputStatus(Player p) {
            if (Config.Mode == TntWarsGameMode.TDM) {
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
            TntWarsTeam team = TeamOf(p);
                    
            if (team == null) return;
            team.Members.Remove(p);
            RestoreColor(p);
        }
        
        void RestoreColor(Player p) {
            TntWarsData data = TryGet(p);
            if (data == null) return;
            
            p.color = data.OrigCol;
            p.SetPrefix();
        }
        
        void JoinTeam(Player p, TntWarsTeam team) {
            team.Members.Add(p);
            Map.Message(p.ColoredName + " %Sjoined the " + team.ColoredName + " %Steam");
            
            p.color = team.Color;
            Player.Message(p, "You are now on the " + team.ColoredName + " team!");
            TabList.Update(p, true);
        }
        
        TntWarsTeam TeamOf(Player p) {
            if (Red.Members.Contains(p)) return Red;
            if (Blue.Members.Contains(p)) return Blue;
            return null;
        }
        
        
        public void ModeTDM() {
            Config.Mode = TntWarsGameMode.TDM;
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
            Config.Mode = TntWarsGameMode.FFA;
            MessageMap(CpeMessageType.Announcement,
                       "&4Gamemode changed to &fFree For All");           
            ResetTeams();
            
            Player[] players = allPlayers.Items;
            foreach (Player pl in players) {
                RestoreColor(pl);
            }
            Config.Save();
        }
        
        public void SetDifficulty(TntWarsDifficulty diff) {
            Config.Difficulty = diff;
            MessageMap(CpeMessageType.Announcement,
                       "&4Difficulty changed to &f" + diff);
            
            bool teamKill = diff >= TntWarsDifficulty.Hard;
            if (cfg.TeamKills == teamKill) return;
            
            cfg.TeamKills = teamKill;
            cfg.Save(Map.name);
        }
        
        public bool InZone(ushort x, ushort y, ushort z, List<TntWarsGame1.Zone> zones) {
            foreach (TntWarsGame1.Zone Zn in zones) {
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
    }
}
