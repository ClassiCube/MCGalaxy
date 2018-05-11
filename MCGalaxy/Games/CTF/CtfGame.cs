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
        public Player p;
        public int Captures, Tags, Points;
        public bool hasflag, tagging, TeamChatting;
        public CtfData(Player p) { this.p = p; }
    }
    
    public sealed class CtfTeam2 {
        public string Name, Color;
        public string ColoredName { get { return Color + Name; } }
        public int Points;
        public Vec3U16 FlagPos;
        public Position SpawnPos;
        public BlockID FlagBlock;
        public VolatileArray<Player> Members = new VolatileArray<Player>();
                
        public CtfTeam2(string name, string color) { Name = name; Color = color; }      
        public bool Remove(Player p) { return Members.Remove(p); }
    }

    public sealed partial class CTFGame : RoundsGame {
        bool running = false;
        public override bool Running { get { return running; } }
        public override string GameName { get { return "CTF"; } }
        
        public CtfTeam2 Red  = new CtfTeam2("Red", Colors.red);
        public CtfTeam2 Blue = new CtfTeam2("Blue", Colors.blue);
        public CTFConfig Config = new CTFConfig();
        public CTFGame() { Picker = new CTFLevelPicker(); }

        List<CtfData> cache = new List<CtfData>();
        internal CtfData Get(Player p) {
            foreach (CtfData d in cache) {
                if (d.p == p) return d;
            }
            return null;
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
            Blue.Members.Clear();
            Red.Members.Clear();
            Blue.Points = 0;
            Red.Points = 0;
            
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
            
            RoundsLeft = 0;
            RoundInProgress = false;
            
            Blue.Members.Clear();
            Red.Members.Clear();
            Blue.Points = 0;
            Red.Points = 0;
            
            LastMap = "";          
            Picker.Clear();
            Map = null;
        }
        

        public void JoinTeam(Player p, CtfTeam2 team) {
            if (Get(p) == null) {
                cache.Add(new CtfData(p));
            } else {
                Get(p).hasflag = false;
            }
            
            team.Members.Add(p);
            Chat.MessageLevel(Map, p.ColoredName + " %Sjoined the " + team.ColoredName + " %Steam");
            Player.Message(p, "You are now on the " + team.ColoredName + " team!");
        }
        
        bool OnOwnTeamSide(int z, CtfTeam2 team) {
            int baseZ = team.FlagPos.Z, zline = Config.ZDivider;
            if (baseZ < zline && z < zline) return true;
            if (baseZ > zline && z > zline) return true;
            return false;
        }
        
        public CtfTeam2 TeamOf(Player p) {
            if (Red.Members.Contains(p)) return Red;
            if (Blue.Members.Contains(p)) return Blue;
            return null;
        }
        
        public CtfTeam2 Opposing(CtfTeam2 team) {
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
