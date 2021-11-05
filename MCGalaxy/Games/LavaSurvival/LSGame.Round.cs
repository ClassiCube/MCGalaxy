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
using System.Threading;

namespace MCGalaxy.Games {
    public sealed partial class LSGame : RoundsGame {

        public bool floodInProgress;

        public void AwardWinners()
        {
            List<Player> winners = Alive.list;
            List<Player> onlineUsers = PlayerInfo.Online.list;
            List<Player> announceWinners = new List<Player>();

            // Calculate the awards and distribute
            foreach(Player winner in winners)
            {
                // Only award the users that are still online
                if (onlineUsers.Contains(winner))
                {
                    // Calculate the award based on times died.
                    int awardAmount = LSGame.Config.MaxAward / (LSGame.Config.AwardReducer * winner.TimesDied);
                    winner.SetMoney(winner.money + awardAmount);
                    winner.Message($"%SCongratulations on surviving! You've been awarded %a{awardAmount} {Server.Config.Currency}%S.");
                    announceWinners.Add(winner);
                }
            }

            string winnerString = string.Format(winners.ToArray().ToString());

            Chat.MessageChat(ChatScope.Global, Player.Console, "%a-------------------------------", null, null, false);
            Chat.MessageChat(ChatScope.Global, Player.Console, "%a--  This round's survivors:  --", null, null, false);
            Chat.MessageChat(ChatScope.Global, Player.Console, winnerString, null, null, false);
            Chat.MessageChat(ChatScope.Global, Player.Console, "%a-------------------------------", null, null, false);
        }

        protected override void DoRound() {
            if (!Running) return;

            ResetPlayerDeaths();
            RoundStart = DateTime.UtcNow;
            RoundInProgress = true;
            Logger.Log(LogType.GameActivity, "[Lava Survival] Round started. Map: " + Map.ColoredName);
            
            Map.SetPhysics(destroyMode ? 2 : 1);           
            int secs = 0, layerSecs = 0;
            
            while (RoundInProgress && secs < roundTotalSecs) {
                if (!Running) return;
                if ((secs % 60) == 0 && !flooded) { Map.Message(FloodTimeLeftMessage(DateTime.UtcNow)); }
                
                if (secs >= floodDelaySecs) {
                    if (layerMode && (layerSecs % layerIntervalSecs) == 0 && curLayer <= cfg.LayerCount) {
                        ushort y = (ushort)(cfg.LayerPos.Y + ((cfg.LayerHeight * curLayer) - 1));
                        Map.Blockchange(cfg.LayerPos.X, y, cfg.LayerPos.Z, floodBlock, true);
                        curLayer++;
                    } else if (!layerMode && secs == floodDelaySecs) {
                        Map.Message("&4Look out, here comes the flood!");
                        Logger.Log(LogType.GameActivity, "[Lava Survival] Starting map flood.");
                        Map.Blockchange(cfg.FloodPos.X, cfg.FloodPos.Y, cfg.FloodPos.Z, floodBlock, true);
                        floodInProgress = true;
                    }
                    
                    layerSecs++;
                    flooded = true;
                }
                
                secs++; Thread.Sleep(1000);
            }
        }

        public override void EndRound() {
            if (!RoundInProgress) return;

            RoundInProgress = false;
            flooded = false;
            floodInProgress = false;
            
            Map.SetPhysics(5);
            Map.Message("The round has ended!");
            ResetPlayers();
        }

        internal string FloodTimeLeftMessage(DateTime now) {
            TimeSpan left = RoundStart.Add(cfg.FloodTime) - now;
            string time = left.Shorten(true);
            if (time.Contains("-"))
                time = "0";
            return "&3" + time + " &Suntil the flood.";
        }
        
        internal string RoundTimeLeftMessage(DateTime now) {
            TimeSpan left = RoundStart.Add(cfg.RoundTime) - now;
            string time = left.Shorten(true);
            if (time.Contains("-"))
                time = "0";
            return "&3" + time + " &Suntil the round ends.";
        }

        /// <summary>
        /// MakeSpectator function
        /// Description: Helper function that handles putting a player into spectator mode.
        /// </summary>
        /// <param name="spectator"></param>
        public void MakeSpectator(Player spectator)
        {
            // We don't want to do anything with the prior referees (staff members)
            if (!spectator.Game.Referee)
            {
                Command.Find("xhide").Use(null, spectator.name);
                spectator.hidden = true;
                spectator.Game.Referee = true;
                Spectator.Add(spectator);
            }
        }

        public override void OutputStatus(Player p) {
            string block = waterMode ? "water" : "lava";
            
            // TODO: send these messages if player is op
            //if (data.layer) {
            //    Map.ChatLevelOps("There will be " + mapSettings.LayerCount + " layers, each " + mapSettings.LayerHeight + " blocks high.");
            //    Map.ChatLevelOps("There will be another layer every " + mapSettings.layerInterval + " minutes.");
            //}
            
            if (waterMode) p.Message("The map will be flooded with &9water &Sthis round!");
            if (layerMode) p.Message("The " + block + " will &aflood in layers &Sthis round!");
            
            if (fastMode) p.Message("The lava will be &cfast &Sthis round!");
            if (killerMode) p.Message("The " + block + " will &ckill you &Sthis round!");
            if (destroyMode) p.Message("The " + block + " will &cdestroy plants " + (waterMode ? "" : "and flammable blocks ") + "&Sthis round!");
            
            if (!flooded) p.Message(FloodTimeLeftMessage(DateTime.UtcNow));
            p.Message(RoundTimeLeftMessage(DateTime.UtcNow));
        }

        /// <summary>
        /// ResetPlayers function
        /// Description: Loops through all dead players / spectators and resets their
        /// status to Alive for the next round of Lava Survival.
        /// </summary>
        public void ResetPlayers()
        {
            // Grab an isolated version of this list to reset all users.
            List<Player> spectators = Spectator.list;
            List<Player> onlineUsers = PlayerInfo.Online.list;
            Alive.Clear();

            // Clear out the spectators
            foreach(Player spectator in spectators)
            {
                // Drop the player completely if they're no longer online
                if (onlineUsers.Contains(spectator))
                {
                    spectator.Game.Referee = false;
                    Alive.Add(spectator);
                    spectator.hidden = false;
                    Command.Find("xhide").Use(null, spectator.name);
                }
                Spectator.Remove(spectator);
            }
        }

        protected override bool SetMap(string map) {
            if (!base.SetMap(map)) return false;
            Map.Config.PhysicsOverload = 1000000;
            return true;
        }

        void KillPlayer(Player p) {
            if (Config.MaxLives <= 0) return;
            Get(p).TimesDied++;
            if (!IsPlayerDead(p)) return;
            
            Chat.MessageFromLevel(p, "λNICK &cran out of lives, and is out of the round!");
            p.Message("&cYou can still watch, but you cannot build.");
            Alive.Remove(p);
            MakeSpectator(p);
        }
    }
}
