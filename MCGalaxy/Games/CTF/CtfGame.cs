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
    
    public sealed class CtfTeam {
        public string Name, Color;
        public string ColoredName { get { return Color + Name; } }
        public int Points;
        public Vec3U16 FlagPos;
        public Position SpawnPos;
        public BlockID FlagBlock;
        public VolatileArray<Player> Members = new VolatileArray<Player>();
                
        public CtfTeam(string name, string color) { Name = name; Color = color; }      
        public bool Remove(Player p) { return Members.Remove(p); }
        
        public void RespawnFlag(Level lvl) {
            Vec3U16 pos = FlagPos;
            lvl.Blockchange(pos.X, pos.Y, pos.Z, FlagBlock);
        }
    }

    public sealed partial class CTFGame : RoundsGame {
        bool running = false;
        public override bool Running { get { return running; } }
        public override string GameName { get { return "CTF"; } }
        
        public CtfTeam Red  = new CtfTeam("Red", Colors.red);
        public CtfTeam Blue = new CtfTeam("Blue", Colors.blue);
        public CTFConfig Config = new CTFConfig();
        public CTFGame() { Picker = new CTFLevelPicker(); }

        CtfData Get(Player p) {
            object data;
            if (!p.Extras.TryGet("MCG_CTF_DATA", out data)) {
                data = new CtfData();
                p.Extras.Put("MCG_CTF_DATA", data);
            }
            return (CtfData)data;
        }

        protected override bool SetMap(string map) {
            bool success = base.SetMap(map);
            if (success) UpdateConfig();
            return success;
        }
        
        public void UpdateConfig() {
            Config.SetDefaults(Map);
            Config.Retrieve(Map.name);
            CTFConfig cfg = Config;
            
            Red.FlagBlock = cfg.RedFlagBlock;
            Red.FlagPos = new Vec3U16((ushort)cfg.RedFlagX, (ushort)cfg.RedFlagY, (ushort)cfg.RedFlagZ);
            Red.SpawnPos = new Position(cfg.RedSpawnX, cfg.RedSpawnY, cfg.RedSpawnZ);
            
            Blue.FlagBlock = cfg.BlueFlagBlock;
            Blue.FlagPos = new Vec3U16((ushort)cfg.BlueFlagX, (ushort)cfg.BlueFlagY, (ushort)cfg.BlueFlagZ);
            Blue.SpawnPos = new Position(cfg.BlueSpawnX, cfg.BlueSpawnY, cfg.BlueSpawnZ);
        }


        static ColumnDesc[] createSyntax = new ColumnDesc[] {
            new ColumnDesc("ID", ColumnType.Integer, priKey: true, autoInc: true, notNull: true),
            new ColumnDesc("Name", ColumnType.VarChar, 20),
            new ColumnDesc("Points", ColumnType.UInt24),
            new ColumnDesc("Captures", ColumnType.UInt24),
            new ColumnDesc("tags", ColumnType.UInt24),
        };

        public bool Start(Player p, int rounds) {
            if (running) {
                Player.Message(p, "CTF game already running."); return false;
            } 
            
            List<string> maps = Picker.GetCandidateMaps();           
            if (maps == null || maps.Count == 0) {
                Player.Message(p, "No maps have been setup for CTF yet"); return false;
            }
            
            if (!SetMap(LevelPicker.GetRandomMap(new Random(), maps))) {
                Player.Message(p, "Failed to load initial map!"); return false;
            }       
            
            RoundsLeft = rounds;
            Blue.RespawnFlag(Map);
            Red.RespawnFlag(Map);
            ResetTeams();
            
            Logger.Log(LogType.GameActivity, "[CTF] Running...");
            running = true;
            Database.Backend.CreateTable("CTF", createSyntax);
            HookEventHandlers();
            
            Thread t = new Thread(RunGame);
            t.Name = "MCG_CTFGame";
            t.Start();
            return true;
        }
        
        public override void End() {
            if (!running) return;
            running = false;
            UnhookEventHandlers();
            
            ResetTeams();
            ResetFlagsState();
            EndCommon();
        }
        
        void ResetTeams() {
            Blue.Members.Clear();
            Red.Members.Clear();
            Blue.Points = 0;
            Red.Points = 0;
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

        public override void PlayerLeftGame(Player p) {
            CtfTeam team = TeamOf(p);
            if (team == null) return;
            
            DropFlag(p, team);
            team.Remove(p);
            Chat.MessageLevel(Map, team.Color + p.DisplayName + " %Sleft CTF");
        }
        
        void JoinTeam(Player p, CtfTeam team) {
            Get(p).HasFlag = false;
            team.Members.Add(p);
            Chat.MessageLevel(Map, p.ColoredName + " %Sjoined the " + team.ColoredName + " %Steam");
            Player.Message(p, "You are now on the " + team.ColoredName + " team!");
        }
        
        bool OnOwnTeamSide(int z, CtfTeam team) {
            int baseZ = team.FlagPos.Z, zline = Config.ZDivider;
            if (baseZ < zline && z < zline) return true;
            if (baseZ > zline && z > zline) return true;
            return false;
        }
        
        public CtfTeam TeamOf(Player p) {
            if (Red.Members.Contains(p)) return Red;
            if (Blue.Members.Contains(p)) return Blue;
            return null;
        }
        
        public CtfTeam Opposing(CtfTeam team) {
            return team == Red ? Blue : Red;
        }
    }
    
    internal class CTFLevelPicker : LevelPicker {
        
        public override List<string> GetCandidateMaps() {
            List<string> maps = null;
            if (!Directory.Exists("CTF")) Directory.CreateDirectory("CTF");           
            if (File.Exists("CTF/maps.config")) {                
                string[] lines = File.ReadAllLines("CTF/maps.config");
                maps = new List<string>(lines);
            }
            
            if (maps == null || maps.Count == 0) {
                Logger.Log(LogType.Warning, "You must have at least 1 level configured to play CTF");
                return null;
            }
            return maps;
        }
    }
}
