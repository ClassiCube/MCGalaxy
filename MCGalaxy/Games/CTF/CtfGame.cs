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
using MCGalaxy.Events;
using MCGalaxy.Maths;
using MCGalaxy.SQL;

namespace MCGalaxy.Games {
    
    internal sealed class Data {
        public Player p;
        public int cap, Tags, points;
        public bool hasflag, tagging, chatting;
        public Data(Player p) { this.p = p; }
    }

    public sealed partial class CTFGame {
        public System.Timers.Timer tagging = new System.Timers.Timer(500);
        public bool voting = false;
        int vote1 = 0;
        int vote2 = 0;
        int vote3 = 0;
        string map1 = "";
        string map2 = "";
        string map3 = "";
        public bool started = false;
        
        CtfTeam2 red;
        CtfTeam2 blue;
        public Level map;
        List<string> maps = new List<string>();
        List<Data> cache = new List<Data>();
        
        public CTFConfig Config = new CTFConfig();
        
        /// <summary> Create a new CTF object </summary>
        public CTFGame() {
            red = new CtfTeam2("Red", Colors.red);
            blue = new CtfTeam2("Blue", Colors.blue);
            
            tagging.Elapsed += CheckTagging;
            tagging.Start();
            HookEvents();
        }
        
        /// <summary> Stop the CTF game (if its running) </summary>
        public void Stop() {
            tagging.Stop();
            tagging.Dispose();
            
            map = null;
            started = false;
            if (LevelInfo.FindExact("ctf") != null)
                Command.all.Find("unload").Use(null, "ctf");
        }
        
        void HookEvents() {
            Player.PlayerDeath += HandlePlayerDeath;
            Player.PlayerChat += HandlePlayerChat;
            Player.PlayerCommand += HandlePlayerCommand;
            Player.PlayerBlockChange += HandlePlayerBlockChange;
            Player.PlayerDisconnect += HandlePlayerDisconnect;
            Level.LevelUnload += HandleLevelUnload;
        }
        
        
        /// <summary> Load a map into CTF </summary>
        public void LoadMap(string mapName) {
            Command.all.Find("unload").Use(null, "ctf");
            if (File.Exists("levels/ctf.lvl"))
                File.Delete("levels/ctf.lvl");
            
            File.Copy("CTF/maps/" + mapName + ".lvl", "levels/ctf.lvl");
            CmdLoad.LoadLevel(null, "ctf");
            map = LevelInfo.FindExact("ctf");
            UpdateConfig();
        }
        
        public void UpdateConfig() {
            Config.SetDefaults(map);
            Config.Retrieve(map.name);
            CTFConfig cfg = Config;
            
            red.FlagBlock = ExtBlock.FromRaw(cfg.RedFlagBlock);
            red.FlagPos = new Vec3U16((ushort)cfg.RedFlagX, (ushort)cfg.RedFlagY, (ushort)cfg.RedFlagZ);
            red.SpawnPos = new Position(cfg.RedSpawnX, cfg.RedSpawnY, cfg.RedSpawnZ);
            
            blue.FlagBlock = ExtBlock.FromRaw(cfg.BlueFlagBlock);
            blue.FlagPos = new Vec3U16((ushort)cfg.BlueFlagX, (ushort)cfg.BlueFlagY, (ushort)cfg.BlueFlagZ);
            blue.SpawnPos = new Position(cfg.BlueSpawnX, cfg.BlueSpawnY, cfg.BlueSpawnZ);
        }
        
        bool LoadConfig() {
            //Load some configs
            if (!Directory.Exists("CTF")) Directory.CreateDirectory("CTF");
            if (!File.Exists("CTF/maps.config")) return false;
            
            string[] lines = File.ReadAllLines("CTF/maps.config");
            maps = new List<string>(lines);
            return maps.Count > 0;
        }
        
        
        void CheckTagging(object sender, System.Timers.ElapsedEventArgs e) {
            Player[] online = PlayerInfo.Online.Items;
            foreach (Player p in online) {
                if (p.level != map) continue;
                
                CtfTeam2 team = TeamOf(p);
                if (team == null || DataOf(p).tagging) continue;
                if (!OnOwnTeamSide(p.Pos.BlockZ, team)) continue;
                CtfTeam2 opposing = Opposing(team);
                
                Player[] opponents = opposing.Members.Items;
                foreach (Player other in opponents) {
                    if (!MovementCheck.InRange(p, other, 2 * 32)) continue;

                    DataOf(other).tagging = true;
                    Player.Message(other, p.ColoredName + " %Stagged you!");
                    team.SendToSpawn(other);
                    Thread.Sleep(300);
                    
                    if (DataOf(other).hasflag) DropFlag(p, opposing);
                    DataOf(p).points += Config.Tag_PointsGained;
                    DataOf(other).points -= Config.Tag_PointsLost;
                    DataOf(p).Tags++;
                    DataOf(other).tagging = false;
                }
            }
        }

        void HandlePlayerDisconnect(Player p, string reason) {
            if (p.level != map) return;
            CtfTeam2 team = TeamOf(p);
            if (team == null) return;
            
            DropFlag(p, team);
            team.Remove(p);
            Chat.MessageLevel(map, team.Color + p.DisplayName + " %Sleft the ctf game");
        }

        void HandleLevelUnload(Level l) {
            if (started && l == map) {
                Logger.Log(LogType.GameActivity, "Unload Failed!, A ctf game is currently going on!");
                Plugin.CancelLevelEvent(LevelEvents.LevelUnload, l);
            }
        }

        static ColumnDesc[] createSyntax = new ColumnDesc[] {
            new ColumnDesc("ID", ColumnType.Integer, priKey: true, autoInc: true, notNull: true),
            new ColumnDesc("Name", ColumnType.VarChar, 20),
            new ColumnDesc("Points", ColumnType.UInt24),
            new ColumnDesc("Captures", ColumnType.UInt24),
            new ColumnDesc("tags", ColumnType.UInt24),
        };
        
        /// <summary> Start the CTF game </summary>
        public bool Start(Player p) {
            if (LevelInfo.FindExact("ctf") != null) {
                Command.all.Find("unload").Use(null, "ctf");
                Thread.Sleep(1000);
            }
            
            if (started) {
                Player.Message(p, "CTF game already running."); return false;
            }
            if (!LoadConfig()) {
                Player.Message(p, "No CTF maps were found."); return false;
            }
            
            blue = new CtfTeam2("blue", Colors.blue);
            red = new CtfTeam2("red", Colors.red);
            LoadMap(maps[new Random().Next(maps.Count)]);
            
            Logger.Log(LogType.GameActivity, "[CTF] Running...");
            started = true;
            Database.Backend.CreateTable("CTF", createSyntax);
            return true;
        }
        
        internal void SpawnPlayer(Player p) {
            if (p.level != map) return;
            CtfTeam2 team = TeamOf(p);
            if (team != null) team.SendToSpawn(p);
        }
        
        string Vote() {
            started = false;
            vote1 = 0;
            vote2 = 0;
            vote3 = 0;
            Random rand = new Random();
            List<string> maps1 = maps;
            map1 = maps1[rand.Next(maps1.Count)];
            maps1.Remove(map1);
            map2 = maps1[rand.Next(maps1.Count)];
            maps1.Remove(map2);
            map3 = maps1[rand.Next(maps1.Count)];
            Chat.MessageLevel(map, "%2VOTE:");
            Chat.MessageLevel(map, "1. " + map1 + " 2. " + map2 + " 3. " + map3);
            voting = true;
            int seconds = rand.Next(15, 61);
            Chat.MessageLevel(map, "You have " + seconds + " seconds to vote!");
            Thread.Sleep(seconds * 1000);
            voting = false;
            Chat.MessageLevel(map, "VOTING ENDED!");
            Thread.Sleep(rand.Next(1, 10) * 1000);
            if (vote1 > vote2 && vote1 > vote3)
            {
                Chat.MessageLevel(map, map1 + " WON!");
                return map1;
            }
            if (vote2 > vote1 && vote2 > vote3)
            {
                Chat.MessageLevel(map, map2 + " WON!");
                return map2;
            }
            if (vote3 > vote2 && vote3 > vote1)
            {
                Chat.MessageLevel(map, map3 + " WON!");
                return map3;
            }
            else
            {
                Chat.MessageLevel(map, "There was a tie!");
                Chat.MessageLevel(map, "I'll choose!");
                return maps[rand.Next(maps.Count)];
            }
        }
        
        void End() {
            started = false;
            string nextmap = "";
            if (blue.Points >= Config.MaxPoints || blue.Points > red.Points) {
                Chat.MessageLevel(map, blue.ColoredName + " %Swon this round of CTF!");
            } else if (red.Points >= Config.MaxPoints || red.Points > blue.Points) {
                Chat.MessageLevel(map, red.ColoredName + " %Swon this round of CTF!");
            } else {
                Chat.MessageLevel(map, "The round ended in a tie!");
            }
            
            Thread.Sleep(4000);
            //MYSQL!
            cache.ForEach(delegate(Data d) {
                              d.hasflag = false;
                              Database.Backend.UpdateRows("CTF", "Points=@1, Captures=@2, tags=@3",
                                                          "WHERE Name = @0", d.p.name, d.points, d.cap, d.Tags);
                          });
            nextmap = Vote();
            Chat.MessageLevel(map, "Starting a new game!");
            blue.Members.Clear();
            red.Members.Clear();
            Thread.Sleep(2000);
            LoadMap(nextmap);
        }
        
        void HandlePlayerBlockChange(Player p, ushort x, ushort y, ushort z, ExtBlock block) {
            if (!started || p.level != map) return;
            CtfTeam2 team = TeamOf(p);
            if (team == null) {
                p.RevertBlock(x, y, z);
                Player.Message(p, "You are not on a team!");
                Plugin.CancelPlayerEvent(PlayerEvents.BlockChange, p);
            }
            
            Vec3U16 pos = new Vec3U16(x, y, z);
            if (pos == Opposing(team).FlagPos && !map.IsAirAt(x, y, z)) TakeFlag(p, team);
            if (pos == team.FlagPos && !map.IsAirAt(x, y, z)) ReturnFlag(p, team);
        }
        
        void TakeFlag(Player p, CtfTeam2 team) {
            CtfTeam2 opposing = Opposing(team);
            Chat.MessageLevel(map, team.Color + p.DisplayName + " took the " + blue.ColoredName + " %Steam's FLAG");
            DataOf(p).hasflag = true;
        }
        
        void ReturnFlag(Player p, CtfTeam2 team) {
            Vec3U16 flagPos = team.FlagPos;
            p.RevertBlock(flagPos.X, flagPos.Y, flagPos.Z);
            Plugin.CancelPlayerEvent(PlayerEvents.BlockChange, p);
            
            if (DataOf(p).hasflag) {
                Chat.MessageLevel(map, team.Color + p.DisplayName + " RETURNED THE FLAG!");
                DataOf(p).hasflag = false;
                DataOf(p).points += Config.Capture_PointsGained;
                DataOf(p).cap++;
                
                CtfTeam2 opposing = Opposing(team);
                team.Points++;
                flagPos = opposing.FlagPos;
                map.Blockchange(flagPos.X, flagPos.Y, flagPos.Z, opposing.FlagBlock);
                
                if (team.Points >= Config.MaxPoints) { End(); return; }
            } else {
                Player.Message(p, "You cannot take your own flag!");
            }
        }
        
        void DropFlag(Player p, CtfTeam2 team) {
            if (!DataOf(p).hasflag) return;
            DataOf(p).hasflag = false;
            Chat.MessageLevel(map, team.Color + p.DisplayName + " DROPPED THE FLAG!");
            DataOf(p).points -= Config.Capture_PointsLost;
            
            CtfTeam2 opposing = Opposing(team);
            Vec3U16 pos = opposing.FlagPos;
            map.Blockchange(pos.X, pos.Y, pos.Z, opposing.FlagBlock);
        }
        
        internal Data DataOf(Player p) {
            foreach (Data d in cache) {
                if (d.p == p) return d;
            }
            return null;
        }
        
        
        void HandlePlayerCommand(string cmd, Player p, string message) {
            if (!started) return;
            
            if (cmd == "teamchat" && p.level == map) {
                if (DataOf(p) != null) {
                    Data d = DataOf(p);
                    if (d.chatting) {
                        Player.Message(d.p, "You are no longer chatting with your team!");
                        d.chatting = !d.chatting;
                    } else {
                        Player.Message(d.p, "You are now chatting with your team!");
                        d.chatting = !d.chatting;
                    }
                    Plugin.CancelPlayerEvent(PlayerEvents.PlayerCommand, p);
                }
            }
            
            if (cmd != "goto") return;
            if (message == "ctf" && p.level != map) {
                if (blue.Members.Count > red.Members.Count) {
                    JoinTeam(p, red);
                } else if (red.Members.Count > blue.Members.Count) {
                    JoinTeam(p, blue);
                } else if (new Random().Next(2) == 0) {
                    JoinTeam(p, red);
                } else {
                    JoinTeam(p, blue);
                }
            } else if (message != "ctf" && p.level == map) {
                CtfTeam2 team = TeamOf(p);
                if (team == null) return;
                
                DropFlag(p, team);
                team.Remove(p);
                Chat.MessageLevel(map, team.Color + p.DisplayName + " %Sleft the ctf game");
            }
        }
        
        void JoinTeam(Player p, CtfTeam2 team) {
            if (DataOf(p) == null) {
                cache.Add(new Data(p));
            } else {
                DataOf(p).hasflag = false;
            }
            
            team.Members.Add(p);
            Chat.MessageLevel(map, p.ColoredName + " joined the " + team.ColoredName + " %Steam");
            Player.Message(p, team.Color + "You are now on the " + team.Name + " team!");
        }
        
        void HandlePlayerChat(Player p, string message) {
            if (voting) {
                if (message == "1" || message.CaselessEq(map1)) {
                    Player.Message(p, "Thanks for voting :D");
                    vote1++;
                    Plugin.CancelPlayerEvent(PlayerEvents.PlayerChat, p);
                } else if (message == "2" || message.CaselessEq(map2)) {
                    Player.Message(p, "Thanks for voting :D");
                    vote2++;
                    Plugin.CancelPlayerEvent(PlayerEvents.PlayerChat, p);
                } else if (message == "3" || message.CaselessEq(map3)) {
                    Player.Message(p, "Thanks for voting :D");
                    vote3++;
                    Plugin.CancelPlayerEvent(PlayerEvents.PlayerChat, p);
                } else {
                    Player.Message(p, "%2VOTE:");
                    Player.Message(p, "1. " + map1 + " 2. " + map2 + " 3. " + map3);
                    Plugin.CancelPlayerEvent(PlayerEvents.PlayerChat, p);
                }
            }
            
            if (!started || p.level != map) return;
            if (!DataOf(p).chatting) return;
            
            CtfTeam2 team = TeamOf(p);
            if (team == null) return;
            Player[] members = team.Members.Items;
            
            foreach (Player pl in members) {
                Player.Message(pl, "({0}) {1}: &f{2}", team.Name, p.ColoredName, message);
            }
            Plugin.CancelPlayerEvent(PlayerEvents.PlayerChat, p);
        }
        
        void HandlePlayerDeath(Player p, ExtBlock deathblock) {
            if (!started || p.level != map) return;
            if (!DataOf(p).hasflag) return;
            
            CtfTeam2 team = TeamOf(p);
            if (team != null) DropFlag(p, team);
        }
        
        bool OnOwnTeamSide(int z, CtfTeam2 team) {
            int baseZ = team.FlagPos.Z, zline = Config.ZDivider;
            if (baseZ < zline && z < zline) return true;
            if (baseZ > zline && z > zline) return true;
            return false;
        }
        
        CtfTeam2 TeamOf(Player p) {
            if (red.Members.Contains(p)) return red;
            if (blue.Members.Contains(p)) return blue;
            return null;
        }
        
        CtfTeam2 Opposing(CtfTeam2 team) { 
            return team == red ? blue : red; 
        }
    }
}
