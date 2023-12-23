/*
    Copyright 2011 MCForge
        
    Dual-licensed under the Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    https://opensource.org/license/ecl-2-0/
    https://www.gnu.org/licenses/gpl-3.0.html
    
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
using MCGalaxy.Blocks.Physics;
using MCGalaxy.Events.EntityEvents;
using MCGalaxy.Events.LevelEvents;
using MCGalaxy.Events.PlayerEvents;
using MCGalaxy.Games;
using MCGalaxy.Maths;
using MCGalaxy.Tasks;
using BlockID = System.UInt16;

namespace MCGalaxy.Modules.Games.TW
{    
    public partial class TWGame : RoundsGame 
    {    
        void UpdateBlockHandlers() {
            Map.UpdateBlockHandlers(Block.TNT);
        }
        
        void HandleBlockHandlersUpdated(Level lvl, BlockID block) {
            if (!Running || lvl != Map || block != Block.TNT) return;
            
            lvl.PlaceHandlers[Block.TNT]   = HandleTNTPlace;
            lvl.PhysicsHandlers[Block.TNT] = HandleTNTPhysics;
        }
        
        bool CheckTNTPlace(Player p, TWData data, ushort x, ushort y, ushort z) {
            if (InZone(x, y, z, tntFreeZones)) {
                p.Message("TNT cannot be placed in this area"); return false;
            }
            if (cfg.MaxActiveTnt == 0) return true;
            
            if (data.TNTCounter == cfg.MaxActiveTnt) {
                p.Message("TNT Wars: Maximum amount of TNT placed"); return false;
            }
            if (data.TNTCounter > cfg.MaxActiveTnt) {
                p.Message("TNT Wars: You are past the maximum amount of TNT that can be placed!"); return false;
            }
            return true;
        }
        
        ChangeResult HandleTNTPlace(Player p, BlockID newBlock, ushort x, ushort y, ushort z) {
            TWData data = Get(p);
            if (!CheckTNTPlace(p, data, x, y, z)) 
                return ChangeResult.Unchanged;
            
            data.TNTCounter++;
            int delay = 1250;
            
            switch (Config.Difficulty) {
                    case TWDifficulty.Easy:   delay = 3250; break;
                    case TWDifficulty.Normal: delay = 2250; break;
            }

            AddTntCheck(Map.PosToInt(x, y, z), p);
            Server.MainScheduler.QueueOnce(AllowMoreTntTask, data,
                                           TimeSpan.FromMilliseconds(delay));
            return p.ChangeBlock(x, y, z, Block.TNT);
        }

        void AddTntCheck(int b, Player p) {
            PhysicsArgs args = default(PhysicsArgs);
            int sessionID = p.Session.ID;
            
            args.Type1  = PhysicsArgs.Custom;
            args.Value1 = (byte)sessionID;
            args.Value2 = (byte)(sessionID >> 8);
            args.Data   = (byte)(sessionID >> 16);
            Map.AddCheck(b, false, args);
        }
        
        static void AllowMoreTntTask(SchedulerTask task) {
            TWData data = (TWData)task.State;
            data.TNTCounter--;
        }
        
        void HandleTNTPhysics(Level lvl, ref PhysInfo C) {
            ushort x = C.X, y = C.Y, z = C.Z;
            Player p = GetPlayer(ref C.Data);
            if (p == null) { C.Data.Data = PhysicsArgs.RemoveFromChecks; return; }
            
            int power = 2, threshold = 3;
            switch (Config.Difficulty) {
                    case TWDifficulty.Easy: threshold = 7; break;
                    case TWDifficulty.Normal: threshold = 5; break;
                    case TWDifficulty.Extreme: power = 3; break;
            }
            
            if ((C.Data.Data >> 4) < threshold) {
                C.Data.Data += (1 << 4);
                TntPhysics.ToggleFuse(lvl, x, (ushort)(y + 1), z);
                return;
            }
            
            TWData data = Get(p);
            if (data.KillStreak >= cfg.StreakTwoAmount && cfg.Streaks) power++;
            TntPhysics.MakeExplosion(Map, x, y, z, power - 2, true, tntImmuneFilter);
            
            List<Player> inRange = new List<Player>();
            Player[] all = allPlayers.Items;
            
            foreach (Player pl in all)
            {
                if (pl == p) continue;
                if (Math.Abs(pl.Pos.BlockX - x) + Math.Abs(pl.Pos.BlockY - y) + Math.Abs(pl.Pos.BlockZ - z) < ((power * 3) + 1)) {
                    inRange.Add(pl);
                }
            }
            
            KillPlayers(p, data, inRange);
        }
        
        static Player GetPlayer(ref PhysicsArgs args) {
            if (args.Type1 != PhysicsArgs.Custom) return null;
            
            int id = args.Value1 | args.Value2 << 8 | (args.Data & 0xF) << 16;
            Player[] players = PlayerInfo.Online.Items;
            for (int i = 0; i < players.Length; i++) 
            {
                if (players[i].Session.ID == id) return players[i];
            }
            return null;
        }
        
        void KillPlayers(Player killer, TWData data, List<Player> inRange) {
            List<Player> killed = new List<Player>();
            int damage = 1, kills = 0, penalty = 0;
            TWDifficulty diff = Config.Difficulty;
            
            if (diff == TWDifficulty.Hard || diff == TWDifficulty.Extreme) {
                damage = 2;
            }
            
            foreach (Player pl in inRange) 
            {
                if (!cfg.TeamKills && TeamKill(killer, pl)) continue;                
                TWData plData = Get(pl);
                
                if (plData.Health <= damage) {
                    killed.Add(pl);
                } else {
                    plData.Health -= damage;
                    plData.HarmedBy = killer;
                    
                    killer.Message("TNT Wars: You harmed " + pl.ColoredName);
                    pl.Message("TNT Wars: You were harmed by " + killer.ColoredName);
                }
            }
            
            foreach (Player pl in killed) 
            {
                string suffix = "";
                TWData plData = Get(pl);
                
                if (plData.HarmedBy != null && plData.HarmedBy != killer) {
                    Player assistant = plData.HarmedBy;
                    suffix = " &S(with help from " + assistant.ColoredName + ")";
                    
                    if (TeamKill(assistant, pl)) {
                        assistant.Message("TNT Wars: - " + cfg.AssistScore + " points for team kill assist!");
                        ChangeScore(assistant, -cfg.AssistScore);
                    } else {
                        assistant.Message("TNT Wars: + " + cfg.AssistScore + " points for assist!");
                        ChangeScore(assistant, cfg.AssistScore);
                    }
                }
                
                if (TeamKill(killer, pl)) {
                    Map.Message("TNT Wars: " + killer.ColoredName + " &Steam killed " + pl.ColoredName + suffix);
                    penalty += cfg.ScorePerKill;
                } else {
                    Map.Message("TNT Wars: " + killer.ColoredName + " &Skilled " + pl.ColoredName + suffix);
                    kills += 1;
                }
                
                plData.HarmedBy = null;
                PlayerActions.Respawn(pl);
            }
            
            int points = 0;
            data.KillStreak += kills;
            
            if (kills > 0 && cfg.Streaks) {
                if (data.KillStreak >= cfg.StreakOneAmount && data.KillStreak < cfg.StreakTwoAmount && data.LastKillStreakAnnounced != cfg.StreakOneAmount)
                {
                    killer.Message("TNT Wars: Kill streak of " + data.KillStreak + " (Multiplier of " + cfg.StreakOneMultiplier + ")");
                    Map.Message(killer.ColoredName + " &Shas a kill streak of " + data.KillStreak);
                    data.ScoreMultiplier = cfg.StreakOneMultiplier;
                    data.LastKillStreakAnnounced = cfg.StreakOneAmount;
                }
                else if (data.KillStreak >= cfg.StreakTwoAmount && data.KillStreak < cfg.StreakThreeAmount && data.LastKillStreakAnnounced != cfg.StreakTwoAmount)
                {
                    killer.Message("TNT Wars: Kill streak of " + data.KillStreak + " (Multiplier of " + cfg.StreakTwoMultiplier + " and a bigger explosion!)");
                    Map.Message(killer.ColoredName + " &Shas a kill streak of " + data.KillStreak + " and now has a bigger explosion for their TNT!");
                    data.ScoreMultiplier = cfg.StreakTwoMultiplier;
                    data.LastKillStreakAnnounced = cfg.StreakTwoAmount;
                }
                else if (data.KillStreak >= cfg.StreakThreeAmount && data.LastKillStreakAnnounced != cfg.StreakThreeAmount)
                {
                    killer.Message("TNT Wars: Kill streak of " + data.KillStreak + " (Multiplier of " + cfg.StreakThreeMultiplier + " and you now have 1 extra health!)");
                    Map.Message(killer.ColoredName + " &Shas a kill streak of " + data.KillStreak + " and now has 1 extra health!");
                    data.ScoreMultiplier = cfg.StreakThreeMultiplier;
                    data.LastKillStreakAnnounced = cfg.StreakThreeAmount;
                    
                    if (diff == TWDifficulty.Hard || diff == TWDifficulty.Extreme) {
                        data.Health += 2;
                    } else {
                        data.Health += 1;
                    }
                } else {
                    killer.Message("TNT Wars: Kill streak of " + data.KillStreak);
                }
            }
            
            points += kills * cfg.ScorePerKill;
            if (kills > 1) points += kills * cfg.MultiKillBonus;

            if (points > 0) {
                points = (int)(points * data.ScoreMultiplier);
                ChangeScore(killer, points);
                killer.Message("TNT Wars: + " + points + " points for " + kills + " kills");
            }
            if (penalty > 0) {
                ChangeScore(killer, -penalty);
                killer.Message("TNT Wars: - " + penalty + " points for team killing!");
            }
        }
    }
}
