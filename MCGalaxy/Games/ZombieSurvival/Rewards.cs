/*
    Copyright 2010 MCLawl Team -
    Created by Snowl (David D.) and Cazzar (Cayde D.)

    Dual-licensed under the Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    http://www.osedu.org/licenses/ECL-2.0
    http://www.gnu.org/licenses/gpl-3.0.html
    
    Unless required by applicable law or agreed to in writing,
    software distributed under the Licenses are distributed on an "AS IS"
    BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
    or implied. See the Licenses for the specific language governing
    permissions and limitations under the Licenses.
 */
using System;
using System.Collections.Generic;

namespace MCGalaxy.Games.ZS {
    
    internal static class Rewards {

        public static void HandOut(ZSGame game) {
            Player[] alive = game.Alive.Items, dead = game.Infected.Items;
            game.Map.ChatLevel("&aThe game has ended!");
            
            if (alive.Length == 0) game.Map.ChatLevel("&4Zombies have won this round.");
            else if (alive.Length == 1) game.Map.ChatLevel("&2Congratulations to the sole survivor:");
            else game.Map.ChatLevel("&2Congratulations to the survivors:");
            AnnounceWinners(game, alive, dead);
            
            game.Map.Config.RoundsPlayed++;
            if (alive.Length > 0) {
                game.Map.Config.RoundsHumanWon++;
                foreach (Player p in alive)
                    IncreaseAliveStats(p, game);
            }
            
            GiveMoney(game, alive);
        }

        static void AnnounceWinners(ZSGame game, Player[] alive, Player[] dead) {
            if (alive.Length > 0) {
                string winners = alive.Join(p => p.ColoredName);
                game.Map.ChatLevel(winners);
                return;
            }
            
            int maxKills = 0, count = 0;
            for (int i = 0; i < dead.Length; i++) {
                maxKills = Math.Max(maxKills, dead[i].Game.CurrentInfected);
            }
            for (int i = 0; i < dead.Length; i++) {
                if (dead[i].Game.CurrentInfected == maxKills) count++;
            }
            
            string group = count == 1 ? " zombie " : " zombies ";
            string suffix = maxKills == 1 ? " %Skill" : " %Skills";
            StringFormatter<Player> formatter = p => p.Game.CurrentInfected == maxKills ? p.ColoredName : null;
            
            game.Map.ChatLevel("&8Best" + group + "%S(&b" + maxKills
                                    + suffix + "%S)&8: " + dead.Join(formatter));
        }

        static void IncreaseAliveStats(Player p, ZSGame game) {
            if (p.Game.PledgeSurvive) {
                Player.Message(p, "You received &a5 %3" + ServerConfig.Currency +
                              " %Sfor successfully pledging that you would survive.");
                p.SetMoney(p.money + 5);
            }
            
            p.Game.CurrentRoundsSurvived++;
            p.Game.TotalRoundsSurvived++;
            p.Game.MaxRoundsSurvived = Math.Max(p.Game.CurrentRoundsSurvived, p.Game.MaxRoundsSurvived);
            p.SetPrefix(); // stars before name
        }

        static void GiveMoney(ZSGame game, Player[] alive) {
            Player[] online = PlayerInfo.Online.Items;
            Random rand = new Random();
            
            foreach (Player pl in online) {
                if (!pl.level.name.CaselessEq(game.MapName)) continue;
                pl.Game.ResetInvisibility();
                int reward = GetMoneyReward(pl, alive, rand);
                
                if (reward == -1) {
                    pl.SendMessage("You may not hide inside a block! No " + ServerConfig.Currency + " for you."); reward = 0;
                } else if (reward > 0) {
                    pl.SendMessage(Colors.gold + "You gained " + reward + " " + ServerConfig.Currency);
                }
                
                pl.SetMoney(pl.money + reward);
                pl.Game.ResetZombieState();
                if (pl.Game.Referee) {
                    pl.SendMessage("You gained one " + ServerConfig.Currency + " because you're a ref. Would you like a medal as well?");
                    pl.SetMoney(pl.money + 1);
                }
                
                Entities.GlobalDespawn(pl, false);
                Entities.GlobalSpawn(pl, false);
                TabList.Add(pl, pl, Entities.SelfID);
                HUD.UpdateTertiary(pl);
            }
        }

        static int GetMoneyReward(Player pl, Player[] alive, Random rand) {
            if (pl.CheckIfInsideBlock()) return -1;
            
            if (alive.Length == 0) {
                return rand.Next(1 + pl.Game.CurrentInfected, 5 + pl.Game.CurrentInfected);
            } else if (alive.Length == 1 && !pl.Game.Infected) {
                return rand.Next(5, 10);
            } else if (alive.Length > 1 && !pl.Game.Infected) {
                return rand.Next(2, 6);
            }
            return 0;
        }
    }
}
