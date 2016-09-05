/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
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
using System.IO;
using MCGalaxy.Games;

namespace MCGalaxy.Commands.World {
    public sealed class CmdGoto : Command {
        public override string name { get { return "goto"; } }
        public override string shortcut { get { return "g"; } }
        public override string type { get { return CommandTypes.World; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("j"), new CommandAlias("join") }; }
        }
        public CmdGoto() { }

        public override void Use(Player p, string message) {
            if (p == null) { MessageInGameOnly(p); return; }
            if (message == "") { Help(p); return; }            
            if (p.usingGoto) { Player.Message(p, "Cannot use /goto, already loading a map."); return; }
            
            Level oldLevel = p.level;
            p.usingGoto = true;
            bool didJoin = false;
            try {
                didJoin = HandleGoto(p, message);
            } finally {
                p.usingGoto = false;
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            
            if (!didJoin) return;
            bool unloadOld = true;
            if (oldLevel.unload && !oldLevel.IsMuseum) {
                Player[] players = PlayerInfo.Online.Items; 
                foreach (Player pl in players) 
                    if (pl.level == oldLevel) { unloadOld = false; break; }
                if (unloadOld && Server.AutoLoad) oldLevel.Unload(true);
            }
        }
        
        bool HandleGoto(Player p, string message) {
            Level lvl = LevelInfo.FindExact(message);
            if (lvl != null) {
                return GoToLevel(p, lvl, message);
            } else if (Server.AutoLoad) {
            	// First try exactly matching unloaded levels
            	if (LevelInfo.ExistsOffline(message))
            		return GotoOfflineLevel(p, message);
                lvl = LevelInfo.Find(message);
                if (lvl != null) return GoToLevel(p, lvl, message);
                
                string map = LevelInfo.FindMapMatches(p, message);
                if (map == null) return false;
                return GotoOfflineLevel(p, map);
            } else {
                lvl = LevelInfo.Find(message);
                if (lvl == null) {
                    Player.Message(p, "There is no level \"{0}\" loaded. Did you mean..", message);
                    Command.all.Find("search").Use(p, "levels " + message);
                    return false;
                }
                return GoToLevel(p, lvl, message);
            }
        }
        
        static bool GotoOfflineLevel(Player p, string message) {
            if (Level.CheckLoadOnGoto(message)) {
        		CmdLoad.LoadLevel(p, message, "0", true);
                Level lvl = LevelInfo.Find(message);
                if (lvl != null) {
                    return GoToLevel(p, lvl, message);
                } else {
                    Player.Message(p, "Level \"{0}\" failed to be auto-loaded.", message);
                    return false;
                }
            }
            Player.Message(p, "Level \"{0}\" cannot be loaded using /goto.", message);
            return false;
        }
        
        static bool GoToLevel(Player p, Level lvl, string message) {
            if (p.level == lvl) { Player.Message(p, "You are already in \"" + lvl.name + "\"."); return false; }
            if (!lvl.CanJoin(p)) return false;
            if (!Server.zombie.PlayerCanJoinLevel(p, lvl, p.level)) return false;

            p.Loading = true;
            Entities.DespawnEntities(p);
            Level oldLevel = p.level;
            p.level = lvl; p.SendUserMOTD(); p.SendMap(oldLevel);

            ushort x = (ushort)(lvl.spawnx * 32 + 16);
            ushort y = (ushort)(lvl.spawny * 32 + 32);
            ushort z = (ushort)(lvl.spawnz * 32 + 16);
            Entities.SpawnEntities(p, x, y, z, lvl.rotx, lvl.roty);
            p.Loading = false;
            CheckGamesJoin(p, oldLevel);
            p.prevMsg = "";
            
            if (!p.hidden && p.level.ShouldShowJoinMessage(oldLevel)) {
                Player.SendChatFrom(p, p.color + "*" + p.DisplayName + " %Swent to &b" + lvl.name, false);
                Player.RaisePlayerAction(p, PlayerAction.JoinWorld, lvl.name);
            }
            return true;
        }
        
        internal static void CheckGamesJoin(Player p, Level oldLvl) {
            Server.lava.PlayerJoinedLevel(p, p.level, oldLvl);
            Server.zombie.PlayerJoinedLevel(p, p.level, oldLvl);
 
            if (p.inTNTwarsMap) p.canBuild = true;
            TntWarsGame game = TntWarsGame.Find(p.level);
            if (game == null) return;
            
            if (game.GameStatus != TntWarsGame.TntWarsGameStatus.Finished &&
                game.GameStatus != TntWarsGame.TntWarsGameStatus.WaitingForPlayers) {
                p.canBuild = false;
                Player.Message(p, "TNT Wars: Disabled your building because you are in a TNT Wars map!");
            }
            p.inTNTwarsMap = true;
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/goto <mapname>");
            Player.Message(p, "%HTeleports yourself to a different level.");
        }
    }
}
