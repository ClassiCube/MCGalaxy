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
using MCGalaxy.Network;
using BlockID = System.UInt16;

namespace MCGalaxy.Games {
    
    public sealed partial class TWGame : RoundsGame {
        
        protected override void DoRound() {
            Player[] all = allPlayers.Items;
            foreach (Player p in all) {
                Get(p).Reset(Config.Difficulty);
                PlayerActions.Respawn(p);
            }
            
            Red.Score = 0; Blue.Score = 0;
            UpdateAllStatus1();
            UpdateAllStatus2();
            
            //Announcing Etc.
            // TODO: tidy up
            string Gamemode = "Free For All";
            if (Config.Mode == TWGameMode.TDM) Gamemode = "Team Deathmatch";
            string difficulty = "Normal";
            string HitsToDie = "2";
            string explosiontime = "medium";
            string explosionsize = "normal";
            switch (Config.Difficulty)
            {
                case TWDifficulty.Easy:
                    difficulty = "Easy";
                    explosiontime = "long";
                    break;

                case TWDifficulty.Normal:
                    difficulty = "Normal";
                    break;

                case TWDifficulty.Hard:
                    HitsToDie = "1";
                    difficulty = "Hard";
                    break;

                case TWDifficulty.Extreme:
                    HitsToDie = "1";
                    explosiontime = "short";
                    explosionsize = "big";
                    difficulty = "Extreme";
                    break;
            }
            
            string teamkillling = "Disabled";
            if (cfg.TeamKills) teamkillling = "Enabled";
            Chat.MessageGlobal("&cTNT Wars %Son " + Map.ColoredName + " %Shas started &3" + Gamemode + " %Swith a difficulty of &3" +
                               difficulty + " %S(&3" + HitsToDie + " %Shits to die, a &3" + explosiontime +
                               " %Sexplosion delay and with a &3" + explosionsize + " %Sexplosion size)" +
                               ", team killing is &3" + teamkillling + " %Sand you can place &3" + cfg.MaxActiveTnt
                               + " %STNT at a time and there is a score limit of &3" + cfg.ScoreRequired + "%S!!");
            
            if (Config.Mode == TWGameMode.TDM) {
                Map.Message("Start your message with ':' to send it to team only!");
            }

            GracePeriod();
            RoundInProgress = true;
            MessageMap(CpeMessageType.Announcement, "&4TNT Wars has started!");
            
            bool won = false;
            while (Running && !won) {
                if (Config.Mode == TWGameMode.TDM) {
                    won = Red.Score >= cfg.ScoreRequired || Blue.Score >= cfg.ScoreRequired;
                } else {
                    all = allPlayers.Items;
                    foreach (Player p in all) {
                        if (Get(p).Score >= cfg.ScoreRequired) won = true;
                    }
                }
                Thread.Sleep(250);
            }
        }
        
        void GracePeriod() {
            if (!cfg.GracePeriod) return;
            int duration = (int)cfg.GracePeriodTime.SecondsLong();
            
            Map.Message("Grace period of &a" + duration + " %Sseconds");
            Map.Message("Building is disabled during this time!");
            if (!Running) return;
            
            Map.Config.Buildable = false;
            Map.Config.Deletable = false;
            Map.UpdateBlockPermissions();
            DoCountdown("&b{0} %Sseconds left", duration, 15);
            
            if (!Running) return;
            Map.Message("Grace period is over!");
            Map.Message("You can now &aplace &cTNT!");
            RestoreBuildPerms();
        }
        
        protected override bool SetMap(string map) {
            if (!base.SetMap(map)) return false;
            
            buildable = Map.Config.Buildable;
            deletable = Map.Config.Deletable;
            Map.SetPhysics(3);
            
            // TODO: handle when these are externally changed (event)
            Map.placeHandlers[Block.TNT]   = HandleTNTPlace;
            Map.physicsHandlers[Block.TNT] = HandleTNTPhysics;
            Map.Props[Block.TNT_Explosion].KillerBlock = false;
            return true;
        }
        
        public override void EndRound() {
            if (!RoundInProgress) return;
            RoundInProgress = false;
            RestoreBuildPerms();
            
            Player[] all = allPlayers.Items;
            foreach (Player p in all) {
                PlayerActions.Respawn(p);
            }
            
            if (Config.Mode == TWGameMode.TDM) {
                if (Red.Score > Blue.Score) {
                    int amount = Red.Score - Blue.Score;
                    Map.Message(Red.ColoredName + " %Swon &cTNT Wars %Sby &f" + amount + " %Spoints!");
                } else if (Blue.Score > Red.Score) {
                    int amount = Blue.Score - Red.Score;
                    Map.Message(Blue.ColoredName + " %Swon &cTNT Wars %Sby &f" + amount + " %Spoints!");
                } else {
                    Map.Message("The round ended in a tie!");
                }
            }
            
            Map.Message("&aTop player scores:");
            PlayerAndScore[] top = SortedByScore();
            int count = Math.Min(top.Length, 3);
            
            for (int i = 0; i < count; i++) {
                Map.Message(FormatTopScore(top, i));
            }
            
            foreach (Player p in all) {
                p.Message("TNT Wars: You scored &f" + Get(p).Score + " points");
            }
        }
        
        bool buildable = true, deletable = true;
        void RestoreBuildPerms() {
            if (Map.Config.Buildable == buildable &&
                Map.Config.Deletable == deletable) return;
            
            Map.Config.Buildable = buildable;
            Map.Config.Deletable = deletable;
            Map.UpdateBlockPermissions();
        }
    }
}
