/*
    Copyright 2011 MCForge
    
    Written by fenderrock87
    
    Dual-licensed under the    Educational Community License, Version 2.0 and
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
using System.IO;
using System.Threading;
using MCGalaxy.Commands.World;
using MCGalaxy.Maths;
using MCGalaxy.SQL;
using BlockID = System.UInt16;

namespace MCGalaxy.Games {
    internal sealed class CtfData {
        public int Captures, Tags, Points;
        public bool HasFlag, TagCooldown, TeamChatting;
        public Vec3S32 LastHeadPos;
    }

    public sealed partial class CTFGame : RoundsGame {
        CTFMapConfig cfg = new CTFMapConfig();
        public static CTFConfig Config = new CTFConfig();
        public override string GameName { get { return "CTF"; } }
        public override RoundsGameConfig GetConfig() { return Config; }
        
        sealed class CtfTeam {
            public string Name, Color;
            public string ColoredName { get { return Color + Name; } }
            public int Captures;
            public Vec3U16 FlagPos, SpawnPos;
            public BlockID FlagBlock;
            public VolatileArray<Player> Members = new VolatileArray<Player>();
            
            public CtfTeam(string name, string color) { Name = name; Color = color; }
            
            public void RespawnFlag(Level lvl) {
                Vec3U16 pos = FlagPos;
                lvl.Blockchange(pos.X, pos.Y, pos.Z, FlagBlock);
            }
        }
        
        CtfTeam Red  = new CtfTeam("Red", Colors.red);
        CtfTeam Blue = new CtfTeam("Blue", Colors.blue);
        
        public static CTFGame Instance = new CTFGame();
        public CTFGame() { Picker = new LevelPicker(); }

        const string ctfExtrasKey = "MCG_CTF_DATA";
        static CtfData Get(Player p) {
            CtfData data = TryGet(p);
            if (data != null) return data;
            data = new CtfData();
            
            // TODO: Is this even thread-safe
            CtfStats s = LoadStats(p.name);
            data.Captures = s.Captures; data.Points = s.Points; data.Tags = s.Tags;
            
            p.Extras[ctfExtrasKey] = data;
            return data;
        }
        
        static CtfData TryGet(Player p) {
            object data; p.Extras.TryGet(ctfExtrasKey, out data); return (CtfData)data;
        }
        
        public override void UpdateMapConfig() {
            CTFMapConfig cfg = new CTFMapConfig();
            cfg.SetDefaults(Map);
            cfg.Load(Map.name);
            this.cfg = cfg;
            
            Red.FlagBlock = cfg.RedFlagBlock;
            Red.FlagPos   = cfg.RedFlagPos;
            Red.SpawnPos  = cfg.RedSpawn;
            
            Blue.FlagBlock = cfg.BlueFlagBlock;
            Blue.FlagPos   = cfg.BlueFlagPos;
            Blue.SpawnPos  = cfg.BlueSpawn;
        }

        
        protected override List<Player> GetPlayers() {
            List<Player> playing = new List<Player>();
            playing.AddRange(Red.Members.Items);
            playing.AddRange(Blue.Members.Items);
            return playing;
        }
        
        // TODO: Actually make this show something
        public override void OutputStatus(Player p) {
            p.Message("{0} &Steam: {1} captures", Blue.ColoredName, Blue.Captures);
            p.Message("{0} &Steam: {1} captures", Red.ColoredName,  Red.Captures);
        }

        protected override void StartGame() {
            Blue.RespawnFlag(Map);
            Red.RespawnFlag(Map);
            ResetTeams();

            Database.CreateTable("CTF", ctfTable);
        }
        
        protected override void EndGame() {
            ResetTeams();
            ResetFlagsState();
        }
        
        void ResetTeams() {
            Blue.Members.Clear();
            Red.Members.Clear();
            Blue.Captures = 0;
            Red.Captures = 0;
        }

        void ResetFlagsState() {
            Blue.RespawnFlag(Map);
            Red.RespawnFlag(Map);
            Player[] players = PlayerInfo.Online.Items;
            
            foreach (Player p in players) {
                if (p.level != Map) continue;
                CtfData data = Get(p);
                
                if (!data.HasFlag) continue;
                data.HasFlag = false;
                ResetPlayerFlag(p, data);
            }
        }
        
        public override void PlayerJoinedGame(Player p) {
            bool announce = false;
            HandleSentMap(p, Map, Map);
            HandleJoinedLevel(p, Map, Map, ref announce);
        }

        public override void PlayerLeftGame(Player p) {
            CtfTeam team = TeamOf(p);
            if (team == null) return;
            team.Members.Remove(p);
            
            DropFlag(p, team);          
        }
        
        void AutoAssignTeam(Player p) {     
            if (Blue.Members.Count > Red.Members.Count) {
                JoinTeam(p, Red);
            } else if (Red.Members.Count > Blue.Members.Count) {
                JoinTeam(p, Blue);
            } else {
                bool red = new Random().Next(2) == 0;
                JoinTeam(p, red ? Red : Blue);
            }
        }
        
        void JoinTeam(Player p, CtfTeam team) {
            Get(p).HasFlag = false;
            team.Members.Add(p);
            Map.Message(p.ColoredName + " &Sjoined the " + team.ColoredName + " &Steam");
            p.Message("You are now on the " + team.ColoredName + " team!");
            TabList.Update(p, true);
        }
        
        bool OnOwnTeamSide(int z, CtfTeam team) {
            int baseZ = team.FlagPos.Z, zline = cfg.ZDivider;
            if (baseZ < zline && z < zline) return true;
            if (baseZ > zline && z > zline) return true;
            return false;
        }
        
        CtfTeam TeamOf(Player p) {
            if (Red.Members.Contains(p)) return Red;
            if (Blue.Members.Contains(p)) return Blue;
            return null;
        }
        
        CtfTeam Opposing(CtfTeam team) {
            return team == Red ? Blue : Red;
        }
    }
}
