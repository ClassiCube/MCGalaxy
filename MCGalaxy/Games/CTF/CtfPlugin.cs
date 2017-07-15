/*
    Copyright 2011 MCForge
    
    Written by fenderrock87
        
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
using MCGalaxy.Events.EntityEvents;
using MCGalaxy.Events.LevelEvents;
using MCGalaxy.Events.PlayerEvents;
using MCGalaxy.Maths;

namespace MCGalaxy.Games {
    public sealed class CtfPlugin : Plugin_Simple {
        public override string creator { get { return Server.SoftwareName + " team"; } }
        public override string MCGalaxy_Version { get { return Server.VersionString; } }
        public override string name { get { return "Core_CTFPlugin"; } }
        public CTFGame Game;

        public override void Load(bool startup) {
            OnPlayerDeathEvent.Register(HandlePlayerDeath, Priority.High);
            OnPlayerChatEvent.Register(HandlePlayerChat, Priority.High);
            OnPlayerCommandEvent.Register(HandlePlayerCommand, Priority.High);
            OnBlockChangeEvent.Register(HandleBlockChange, Priority.High);
            OnPlayerDisconnectEvent.Register(HandleDisconnect, Priority.High);
            OnLevelUnloadEvent.Register(HandleLevelUnload, Priority.High);
            OnPlayerSpawningEvent.Register(HandlePlayerSpawning, Priority.High);
            OnTabListEntryAddedEvent.Register(HandleTabListEntryAdded, Priority.High);
        }
        
        public override void Unload(bool shutdown) {
            OnPlayerDeathEvent.Unregister(HandlePlayerDeath);
            OnPlayerChatEvent.Unregister(HandlePlayerChat);
            OnPlayerCommandEvent.Unregister(HandlePlayerCommand);
            OnBlockChangeEvent.Unregister(HandleBlockChange);
            OnPlayerDisconnectEvent.Unregister(HandleDisconnect);
            OnLevelUnloadEvent.Unregister(HandleLevelUnload);
            OnPlayerSpawningEvent.Unregister(HandlePlayerSpawning);
            OnTabListEntryAddedEvent.Unregister(HandleTabListEntryAdded);
        }
        
        
        void HandlePlayerDeath(Player p, ExtBlock deathblock) {
            if (!Game.started || p.level != Game.Map) return;
            if (!Game.Get(p).hasflag) return;
            
            CtfTeam2 team = Game.TeamOf(p);
            if (team != null) Game.DropFlag(p, team);
        }
        
        void HandlePlayerChat(Player p, string message) {
            if (Game.voting) {
                if (message == "1" || message.CaselessEq(Game.map1)) {
                    Player.Message(p, "Thanks for voting :D");
                    Game.vote1++;
                    p.cancelchat = true;
                } else if (message == "2" || message.CaselessEq(Game.map2)) {
                    Player.Message(p, "Thanks for voting :D");
                    Game.vote2++;
                    p.cancelchat = true;
                } else if (message == "3" || message.CaselessEq(Game.map3)) {
                    Player.Message(p, "Thanks for voting :D");
                    Game.vote3++;
                    p.cancelchat = true;
                } else {
                    Player.Message(p, "%2VOTE:");
                    Player.Message(p, "1. " + Game.map1 + " 2. " + Game.map2 + " 3. " + Game.map3);
                    p.cancelchat = true;
                }
            }
            
            if (!Game.started || p.level != Game.Map) return;
            if (!Game.Get(p).TeamChatting) return;
            
            CtfTeam2 team = Game.TeamOf(p);
            if (team == null) return;
            Player[] members = team.Members.Items;
            
            foreach (Player pl in members) {
                Player.Message(pl, "({0}) {1}: &f{2}", team.Name, p.ColoredName, message);
            }
            p.cancelchat = true;
        }
        
        void HandleBlockChange(Player p, ushort x, ushort y, ushort z, ExtBlock block) {
            if (!Game.started || p.level != Game.Map) return;
            CtfTeam2 team = Game.TeamOf(p);
            if (team == null) {
                p.RevertBlock(x, y, z);
                Player.Message(p, "You are not on a team!");
                p.cancelBlock = true;
                return;
            }
            
            Vec3U16 pos = new Vec3U16(x, y, z);
            if (pos == Game.Opposing(team).FlagPos && !Game.Map.IsAirAt(x, y, z)) {
                Game.TakeFlag(p, team);
            }
            if (pos == team.FlagPos && !Game.Map.IsAirAt(x, y, z)) {
                Game.ReturnFlag(p, team);
            }
        }
        
        void HandleDisconnect(Player p, string reason) {
            if (p.level != Game.Map) return;
            CtfTeam2 team = Game.TeamOf(p);
            if (team == null) return;
            
            Game.DropFlag(p, team);
            team.Remove(p);
            Chat.MessageLevel(Game.Map, team.Color + p.DisplayName + " %Sleft the ctf game");
        }

        void HandleLevelUnload(Level lvl) {
            if (Game.started && lvl == Game.Map) {
                Logger.Log(LogType.GameActivity, "Unload Failed!, A ctf game is currently going on!");
                lvl.cancelunload = true;
            }
        }        
                
        void HandlePlayerSpawning(Player p, ref Position pos, ref byte yaw, ref byte pitch, bool respawning) {
            if (!Game.started || p.level != Game.Map) return;
            
            CtfTeam2 team = Game.TeamOf(p);
            if (team != null) pos = team.SpawnPos;
            if (team != null && respawning) Game.DropFlag(p, team);
        }        
        
        void HandleTabListEntryAdded(Entity entity, ref string tabName, ref string tabGroup, Player dst) {
            Player p = entity as Player;
            if (p == null || !Game.started || p.level != Game.Map) return;
            CtfTeam2 team = Game.TeamOf(p);
            
            if (p.Game.Referee) {
                tabGroup = "&2Referees";
            } else if (team != null) {
                tabGroup = team.ColoredName + " &fteam";
            } else {
                tabGroup = "&7Spectators";
            }
        }
        
        void HandlePlayerCommand(Player p, string cmd, string args) {
            if (!Game.started) return;
            
            if (cmd == "teamchat" && p.level == Game.Map) {
                CtfData data = Game.Get(p);
                if (data != null) {
                    if (data.TeamChatting) {
                        Player.Message(data.p, "You are no longer chatting with your team!");
                    } else {
                        Player.Message(data.p, "You are now chatting with your team!");
                    }
                    data.TeamChatting = !data.TeamChatting;
                    p.cancelcommand = true;
                }
            }
            
            if (cmd != "goto") return;
            if (args == "ctf" && p.level != Game.Map) {
                if (Game.Blue.Members.Count > Game.Red.Members.Count) {
                    Game.JoinTeam(p, Game.Red);
                } else if (Game.Red.Members.Count > Game.Blue.Members.Count) {
                    Game.JoinTeam(p, Game.Blue);
                } else if (new Random().Next(2) == 0) {
                    Game.JoinTeam(p, Game.Red);
                } else {
                    Game.JoinTeam(p, Game.Blue);
                }
            } else if (args != "ctf" && p.level == Game.Map) {
                CtfTeam2 team = Game.TeamOf(p);
                if (team == null) return;
                
                Game.DropFlag(p, team);
                team.Remove(p);
                Chat.MessageLevel(Game.Map, team.Color + p.DisplayName + " %Sleft the ctf game");
            }
        }
    }
}
