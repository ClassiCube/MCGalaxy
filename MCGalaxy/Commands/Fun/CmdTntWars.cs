/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
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
using MCGalaxy.Games;
using MCGalaxy.Maths;
using BlockID = System.UInt16;

namespace MCGalaxy.Commands.Fun {
    public sealed class CmdTntWars : RoundsGameCmd {
        public override string name { get { return "TntWars"; } }
        public override string shortcut { get { return "tw"; } }
        protected override RoundsGame Game { get { return TWGame.Instance; } }
        public override CommandPerm[] ExtraPerms {
            get { return new[] { new CommandPerm(LevelPermission.Operator, "can manage TNT wars") }; }
        }

        public override void Use(Player p, string message, CommandData data) {
            if (message.CaselessEq("scores")) {
                HandleScores(p);
            } else {
                base.Use(p, message, data);
            }
        }
        
        void HandleScores(Player p) {
            TWGame game = (TWGame)Game;
            if (!game.RoundInProgress) {
                p.Message("Round is not in progress!"); return;
            }
            
            PlayerAndScore[] top = game.SortedByScore();
            int count = Math.Min(top.Length, 5);
            
            p.Message("Top {0} scores:", count);
            for (int i = 0; i < count; i++) {
                p.Message(game.FormatTopScore(top, i));
            }
        }
        
        protected override void HandleSet(Player p, RoundsGame game_, string[] args) {
            TWGame game = (TWGame)game_;
            TWMapConfig cfg  = new TWMapConfig();
            TWConfig gameCfg = TWGame.Config;
            
            LoadMapConfig(p, cfg);
            if (args.Length == 1) { Help(p, "set"); return; }
            if (args.Length == 2) { OutputStatus(p, gameCfg, cfg); return; }

            string prop = args[1], value = args[2];
            if (prop.CaselessEq("spawn")) {
                if (gameCfg.Mode == TWGameMode.FFA) {
                    p.Message("&WCannot set spawns in Free For All mode"); return;
                }
                
                if (value.CaselessEq("red")) {
                    cfg.RedSpawn = (Vec3U16)p.Pos.FeetBlockCoords;
                    p.Message("Set &cRed &Sspawn");
                } else if (value.CaselessEq("blue")) {
                    cfg.BlueSpawn = (Vec3U16)p.Pos.FeetBlockCoords;
                    p.Message("Set &9Blue &Sspawn");
                } else {
                    Help(p, "team"); return;
                }
            } else if (prop.CaselessEq("tnt")) {
                int amount = 1;
                if (!CommandParser.GetInt(p, value, "TNT at a time", ref amount, 0)) return;
                cfg.MaxActiveTnt = amount;
                
                p.Message("Number of TNTs placeable by a player at a time is now {0}",
                               amount == 0 ? "unlimited" : value);
            } else if (prop.CaselessEq("graceperiod")) {
                SetBool(p, ref cfg.GracePeriod, value, "Grace period");
            } else if (prop.CaselessEq("gracetime")) {
                TimeSpan time = default(TimeSpan);
                if (!CommandParser.GetTimespan(p, value, ref time, "set grace time to", "s")) return;
                cfg.GracePeriodTime = time;
                
                p.Message("Grace period is now {0}", time.Shorten(true, true));
            } else if (prop.CaselessEq("gamemode")) {
                if (value.CaselessEq("tdm")) {
                    if (gameCfg.Mode == TWGameMode.FFA) {
                        if (p.level != game.Map) { p.Message("Changed gamemode to Team Deathmatch"); }
                        game.ModeTDM();
                    } else {
                        p.Message("&cGamemode is already Team Deathmatch"); return;
                    }
                } else if (value.CaselessEq("ffa")) {
                    if (gameCfg.Mode == TWGameMode.TDM) {
                        if (p.level != game.Map) { p.Message("Changed gamemode to Free For All"); }
                        game.ModeFFA();
                    } else {
                        p.Message("&cGamemode is already Free For All"); return;
                    }
                } else {
                    Help(p, "other"); return;
                }
            } else if (prop.CaselessEq("difficulty")) {
                TWDifficulty diff = TWDifficulty.Easy;
                if (!CommandParser.GetEnum(p, value, "Difficulty", ref diff)) return;
                SetDifficulty(game, diff, p);
            } else if (prop.CaselessEq("score")) {
                if (args.Length < 4) { Help(p, "score"); return; }
                if (!HandleSetScore(p, cfg, args)) return;
            } else if (prop.CaselessEq("balanceteams")) {
                SetBool(p, ref cfg.BalanceTeams, value, "Team balancing");
            } else if (prop.CaselessEq("teamkill")) {
                SetBool(p, ref cfg.TeamKills, value, "Team killing");
            } else if (prop.CaselessEq("zone")) {
                if (args.Length < 4) { Help(p, "zone"); return; }
                
                if (value.CaselessEq("notnt")) {
                    if (!HandleZone(p, game, true, args)) return;
                } else if (value.CaselessEq("nodestroy")) {
                    if (!HandleZone(p, game, false, args)) return;
                } else {
                    Help(p, "zone"); return;
                }
            } else {
                OutputStatus(p, gameCfg, cfg); return;
            }
            SaveMapConfig(p, cfg);
        }
        
        static void OutputStatus(Player p, TWConfig gameCfg, TWMapConfig cfg) {
            p.Message("Gamemode: &a{0} &Sat difficulty &a{1}",
                           gameCfg.Mode, gameCfg.Difficulty);
            p.Message("TNT per player at a time: &a{0}",
                           cfg.MaxActiveTnt == 0 ? "unlimited" : cfg.MaxActiveTnt.ToString());
            p.Message("Grace period: {0} &S(for {1} seconds)",
                           GetBool(cfg.GracePeriod), cfg.GracePeriodTime);
            p.Message("Team balancing: {0}&S, Team killing: {1}",
                           GetBool(cfg.BalanceTeams), GetBool(cfg.TeamKills));
            p.Message("Score: &a{0} &Spoints needed to win, &a{1} &Spoints per kill",
                           cfg.ScoreRequired, cfg.ScorePerKill);
            p.Message("Streaks: {0}&S, Multikill bonus: {1}",
                           GetBool(cfg.Streaks), GetBool(cfg.MultiKillBonus != 0));
            p.Message("Assists: {0} &S(at {1} points)",
                           GetBool(cfg.AssistScore > 0), cfg.AssistScore);
        }
        
        static string GetBool(bool value) { return value ? "&aEnabled" : "&cDisabled"; }
        
        bool HandleSetScore(Player p, TWMapConfig cfg, string[] args) {
            string opt = args[2], value = args[3];            
            if (opt.CaselessEq("required")) {
                int score = 1;
                if (!CommandParser.GetInt(p, value, "Points", ref score, 0)) return false;
                cfg.ScoreRequired = score;
                
                p.Message("Score required to win is now &a{0} &Spoints!", score);
            } else if (opt.CaselessEq("streaks")) {
                SetBool(p, ref cfg.Streaks, value, "Streaks");
            } else if (opt.CaselessEq("multi")) {
                int multi = 1;
                if (!CommandParser.GetInt(p, value, "Points", ref multi, 0)) return false;
                cfg.MultiKillBonus = multi;
                
                if (multi == 0) {
                    p.Message("Multikill bonus is now &cdisabled");
                } else {
                    p.Message("Scores per extra kill is now &a{0} &Spoints", multi);
                }
            } else if (opt.CaselessEq("kill")) {
                int kill = 1;
                if (!CommandParser.GetInt(p, value, "Points", ref kill, 0)) return false;
                cfg.ScorePerKill = kill;
                
                p.Message("Score per kill is now &a{0} &Spoints", kill);
            } else if (opt.CaselessEq("assist")) {
                int assist = 1;
                if (!CommandParser.GetInt(p, value, "Points", ref assist, 0)) return false;
                cfg.AssistScore = assist;
                
                if (assist == 0) {
                    p.Message("Scores per assist is now &cdisabled");
                } else {
                    p.Message("Score per assist is now &a{0} &Spoints", assist);
                }
            } else {
                return false;
            }
            return true;
        }
        
        bool HandleZone(Player p, TWGame game, bool noTntZone, string[] args) {
            string type = noTntZone ? "no TNT" : "no blocks deleted on explosions";
            List<TWGame.TWZone> zones = noTntZone ? game.tntFreeZones : game.tntImmuneZones;
            string opt = args[3];
            
            if (IsCreateCommand(opt)) {
                p.Message("Place 2 blocks to create a {0} zone", type);
                p.MakeSelection(2, zones, AddZoneCallback);
            } else if (IsDeleteCommand(opt)) {
                if (args.Length > 4 && args[4].CaselessEq("all")) {
                    zones.Clear();
                    p.Message("Deleted all {0} zones", type);
            	} else {
                    p.Message("Place a block to delete a {0} zone", type);
                    p.MakeSelection(1, zones, DeleteZoneCallback);
                }
            } else if (opt.CaselessEq("check")) {
                p.Message("Place a block to check for {0} zones", type);
                p.MakeSelection(1, zones, CheckZoneCallback);
            } else {
                return false;
            }
            return true;
        }
        
        static bool AddZoneCallback(Player p, Vec3S32[] marks, object state, BlockID block) {
            Vec3U16 p1 = (Vec3U16)marks[0], p2 = (Vec3U16)marks[1];
            TWGame.TWZone zn = new TWGame.TWZone(p1, p2);

            List<TWGame.TWZone> zones = (List<TWGame.TWZone>)state;
            zones.Add(zn);
            p.Message("TNT Wars: Zone added!");
            return false;
        }
        
        static bool DeleteZoneCallback(Player p, Vec3S32[] marks, object state, BlockID block) {
            ushort x = (ushort)marks[0].X, y = (ushort)marks[0].Y, z = (ushort)marks[0].Z;
            List<TWGame.TWZone> zones = (List<TWGame.TWZone>)state;
            bool any = false;
            
            for (int i = zones.Count - 1; i >= 0; i--) {
                TWGame.TWZone zn = zones[i];
                if (x >= zn.MinX && x <= zn.MaxX && y >= zn.MinY && y <= zn.MaxY && z >= zn.MinZ && z <= zn.MaxZ) {
                    zones.RemoveAt(i);
                    p.Message("TNT Wars: Zone deleted!");
                    any = true;
                }
            }
            
            if (!any) p.Message("TNT Wars Error: You weren't in any zone");
            return false;
        }
        
        static bool CheckZoneCallback(Player p, Vec3S32[] marks, object state, BlockID block) {
            ushort x = (ushort)marks[0].X, y = (ushort)marks[0].Y, z = (ushort)marks[0].Z;
            
            List<TWGame.TWZone> zones = (List<TWGame.TWZone>)state;
            TWGame game = TWGame.Instance;
            if (zones == game.tntFreeZones) {
                if (game.InZone(x, y, z, zones)) {
                    p.Message("TNT Wars: You are currently in a no TNT zone!");
                } else {
                    p.Message("TNT Wars: You are not currently in a no TNT zone!");
                }
            } else if (zones == game.tntImmuneZones) {
                if (game.InZone(x, y, z, zones)) {
                    p.Message("TNT Wars: You are currently in a no TNT block explosion zone (explosions won't destroy blocks)!");
                } else {
                    p.Message("TNT Wars: You are currently in a TNT block explosion zone (explosions will destroy blocks)!");
                }
            }
            return false;
        }
        
        static void SetDifficulty(TWGame game, TWDifficulty diff, Player p) {
            if (p.level != game.Map)
                p.Message("Changed TNT wars difficulty to {0}", diff);
            game.SetDifficulty(diff);
        }
        
        static void SetBool(Player p, ref bool target, string opt, string name) {
            if (!CommandParser.GetBool(p, opt, ref target)) return;
            p.Message("{0} is now {1}", name, GetBool(target));
        }
        
        
        public override void Help(Player p, string message) {
            if (message.CaselessEq("set")) {
                p.Message("&T/Help TW team &H- Views help for team settings");
                p.Message("&T/Help TW score &H- Views help for score settings");
                p.Message("&T/Help TW zone &H- Views help for zone settings");
                p.Message("&T/Help TW other &H- Views help for other settings");
                p.Message("&T/TW set status &H- view status of settings");
            } else if (message.CaselessEq("team")) {
                p.Message("&T/TW set spawn [red/blue] &H- set the spawns for red/blue team");
                p.Message("&T/TW set balanceteams [on/off] &H- enable/disable balancing teams");
                p.Message("&T/TW set teamkill [on/off] &H- enable/disable teamkills");
            } else if (message.CaselessEq("score")) {
                p.Message("&T/TW set score required [points] &H- Sets score required to win the round");
                p.Message("&T/TW set score streaks [on/off] &H- enable/disable kill streaks");
                p.Message("&T/TW set score multi [points] &H- set the score bonus per multikill");
                p.Message("&T/TW set score kill [points] &H- set the score per kill");
                p.Message("&T/TW set score assist [points] &H- set the score per assist");
            } else if (message.CaselessEq("zone")) {
                // TODO: improve this massively
                p.Message("&T/TW set zone notnt/nodestroy add &H- Creates a new zone");
                p.Message("&T/TW set zone notnt/nodestroy delete &H- Deletes all zones affecting a block");
                p.Message("&T/TW set zone notnt/nodestroy delete all &H- Deletes all zones");
                p.Message("&T/TW set zone notnt/nodestroy check &H- Lists zones affecting a block");
                p.Message("(No TNT zones or zones where explosions do not delete blocks");
            } else if (message.CaselessEq("other")) {
                p.Message("&T/TW set tnt [amount] &H- sets the amount of tnt per player at a time");
                p.Message("&T/TW set graceperiod [on/off] &H- enable/disable the grace period");
                p.Message("&T/TW set gracetime [amount] &H- set the grace period time (in seconds)");
                p.Message("&T/TW set gamemode [tdm/ffa] &H- set team or free for all gamemode");
                p.Message("&T/TW set difficulty [easy/normal/hard/extreme] &H- set difficulty");
            } else {
                base.Help(p, message);
            }
        }
        
        public override void Help(Player p) {
            p.Message("&T/TW start <map> &H- Starts TNT wars");
            p.Message("&T/TW stop &H- Stops TNT wars");
            p.Message("&T/TW end &H- Ends current round of TNT wars");
            p.Message("&T/TW add/remove &H- Adds/removes current map from map list");
            p.Message("&T/TW set [property] &H- Sets a property. See &T/Help TW set");
            p.Message("&T/TW status &H- View your current score and health");
            p.Message("&T/TW go &H- Moves you to the current TNT wars map");
            p.Message("&T/TW scores &H- lists players with highest scores");
        }
    }
}
