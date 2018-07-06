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
using System.Threading;
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

        public override void Use(Player p, string message) {
            if (message.CaselessEq("rules")) {
                HandleRules(p);
            } else if (message.CaselessEq("scores")) {
                HandleScores(p);
            } else {
                base.Use(p, message);
            }
        }

        
        void HandleRules(Player p) {
            Player.Message(p, "TNT Wars Rules:");
            Player.Message(p, "The aim of the game is to blow up people using TNT!");
            Player.Message(p, "To place tnt simply place a TNT block and after a short delay it shall explode!");
            Player.Message(p, "During the game the amount of TNT placable at one time may be limited!");
            Player.Message(p, "You are not allowed to use hacks of any sort during the game!");
        }
        
        void HandleScores(Player p) {
            TWGame game = (TWGame)Game;
            if (!game.RoundInProgress) {
                Player.Message(p, "Round is not in progress!"); return;
            }
            
            PlayerAndScore[] top = game.SortedByScore();
            int count = Math.Min(top.Length, 5);
            
            Player.Message(p, "Top {0} scores:", count);
            for (int i = 0; i < count; i++) {
                Player.Message(p, game.FormatTopScore(top, i));
            }
        }
        
        protected override void HandleSetCore(Player p, RoundsGame game_, string[] args) {
            TWGame game = (TWGame)game_;
            TWMapConfig cfg = RetrieveConfig(p);
            TWConfig gameCfg = TWGame.Config;
            
            if (args.Length == 1) { Help(p, "set"); return; }
            if (args.Length == 2) { OutputStatus(p, gameCfg, cfg); return; }

            string prop = args[1], value = args[2];
            if (prop.CaselessEq("spawn")) {
                if (gameCfg.Mode == TntWarsGameMode.FFA) {
                    Player.Message(p, "&cCannot set spawns in Free For All mode"); return;
                }
                
                if (value.CaselessEq("red")) {
                    cfg.RedSpawn = (Vec3U16)p.Pos.FeetBlockCoords;
                    Player.Message(p, "TNT Wars: Set &cRed %Sspawn");
                } else if (value.CaselessEq("blue")) {
                    cfg.BlueSpawn = (Vec3U16)p.Pos.FeetBlockCoords;
                    Player.Message(p, "TNT Wars: Set &9Blue %Sspawn");
                } else {
                    Help(p, "team"); return;
                }
            } else if (prop.CaselessEq("tnt")) {
                int amount = 1;
                if (!CommandParser.GetInt(p, value, "TNT at a time", ref amount, 0)) return;
                cfg.MaxPlayerActiveTnt = amount;
                
                Player.Message(p, "TNT Wars: Number of TNTs placeable by a player at a time is now {0}",
                               amount == 0 ? "unlimited" : value);
            } else if (prop.CaselessEq("graceperiod")) {
                SetBool(p, ref cfg.InitialGracePeriod, value, "Grace period");
            } else if (prop.CaselessEq("gracetime")) {
                int time = 1;
                if (!CommandParser.GetInt(p, value, "Grace time", ref time, 10, 300)) return;
                cfg.GracePeriodSeconds = time;
                
                Player.Message(p, "TNT Wars: Grace period is now {0} seconds", value);
            } else if (prop.CaselessEq("gamemode")) {
                if (value.CaselessEq("tdm")) {
                    if (gameCfg.Mode == TntWarsGameMode.FFA) {
                        if (p.level != game.Map) { Player.Message(p, "Changed gamemode to Team Deathmatch"); }
                        game.ModeTDM();
                    } else {
                        Player.Message(p, "&cGamemode is already Team Deathmatch"); return;
                    }
                } else if (value.CaselessEq("ffa")) {
                    if (gameCfg.Mode == TntWarsGameMode.TDM) {
                        if (p.level != game.Map) { Player.Message(p, "Changed gamemode to Free For All"); }
                        game.ModeFFA();
                    } else {
                        Player.Message(p, "&cGamemode is already Free For All"); return;
                    }
                } else {
                    Help(p, "other"); return;
                }
            } else if (prop.CaselessEq("difficulty")) {
                TntWarsDifficulty diff = TntWarsDifficulty.Easy;
                if (!CommandParser.GetEnum(p, value, "Difficulty", ref diff)) return;
                SetDifficulty(game, diff, p);
            } else if (prop.CaselessEq("score")) {
                if (args.Length < 4) { Help(p, "score"); return; }
                if (!HandleSetScore(p, cfg, args)) return;
            } else if (prop.CaselessEq("balanceteams")) {
                SetBool(p, ref cfg.BalanceTeams, value, "Team balancing");
            } else if (prop.CaselessEq("teamkill")) {
                SetBool(p, ref cfg.TeamKills, value, "Team killing");
            } else if (prop.CaselessEq("zones")) {
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
            UpdateConfig(p, cfg);
        }
        
        static void OutputStatus(Player p, TWConfig gameCfg, TWMapConfig cfg) {
            Player.Message(p, "Gamemode: &a{0} %Sat difficulty &a{1}",
                           gameCfg.Mode, gameCfg.Difficulty);
            Player.Message(p, "TNT per player at a time: &a{0}",
                           cfg.MaxPlayerActiveTnt == 0 ? "unlimited" : cfg.MaxPlayerActiveTnt.ToString());
            Player.Message(p, "Grace period: {0} %S(for {1} seconds)",
                           GetBool(cfg.InitialGracePeriod), cfg.GracePeriodSeconds);
            Player.Message(p, "Team balancing: {0}%S, Team killing: {1}",
                           GetBool(cfg.BalanceTeams), GetBool(cfg.TeamKills));
            Player.Message(p, "Score: &a{0} %Spoints needed to win, &a{1} %Spoints per kill",
                           cfg.ScoreRequired, cfg.ScorePerKill);
            Player.Message(p, "Streaks: {0}%S, Multikill bonus: {1}",
                           GetBool(cfg.Streaks), GetBool(cfg.MultiKillBonus != 0));
            Player.Message(p, "Assists: {0} %S(at {1} points)",
                           GetBool(cfg.AssistScore > 0), cfg.AssistScore);
        }
        
        static string GetBool(bool value) { return value ? "&aEnabled" : "&cDisabled"; }
        
        bool HandleSetScore(Player p, TWMapConfig cfg, string[] args) {
            string opt = args[2], value = args[3];            
            if (opt.CaselessEq("required")) {
                int score = 1;
                if (!CommandParser.GetInt(p, value, "Points", ref score, 0)) return false;
                cfg.ScoreRequired = score;
                
                Player.Message(p, "TNT Wars: Score required to win is now &a{0} %Spoints!", score);
            } else if (opt.CaselessEq("streaks")) {
                SetBool(p, ref cfg.Streaks, value, "Streaks");
            } else if (opt.CaselessEq("multi")) {
                int multi = 1;
                if (!CommandParser.GetInt(p, value, "Points", ref multi, 0)) return false;
                cfg.MultiKillBonus = multi;
                
                if (multi == 0) {
                    Player.Message(p, "Multikill bonus is now &cdisabled");
                } else {
                    Player.Message(p, "Scores per extra kill is now &a{0} %Spoints", multi);
                }
            } else if (opt.CaselessEq("kill")) {
                int kill = 1;
                if (!CommandParser.GetInt(p, value, "Points", ref kill, 0)) return false;
                cfg.ScorePerKill = kill;
                
                Player.Message(p, "Score per kill is now &a{0} %Spoints", kill);
            } else if (opt.CaselessEq("assist")) {
                int assist = 1;
                if (!CommandParser.GetInt(p, value, "Points", ref assist, 0)) return false;
                cfg.AssistScore = assist;
                
                if (assist == 0) {
                    Player.Message(p, "Scores per assist is now &cdisabled");
                } else {
                    Player.Message(p, "TScore per assist is now &a{0} %Spoints", assist);
                }
            } else {
                return false;
            }
            return true;
        }
        
        bool HandleZone(Player p, TWGame game, bool noTntZone, string[] args) {
            string msg = noTntZone ? "no TNT" : "no blocks deleted on explosions";
            List<TWGame.TWZone> zones = noTntZone ? game.tntFreeZones : game.tntImmuneZones;
            string opt = args[3];
            
            if (IsCreateCommand(opt)) {
                Player.Message(p, "TNT Wars: Place 2 blocks to create the zone for {0}!", msg);
                p.MakeSelection(2, zones, AddZoneCallback);
            } else if (IsDeleteCommand(opt)) {
                if (args.Length > 4 && args[4].CaselessEq("all")) {
                    Player.Message(p, "TNT Wars: Place a block to delete the zone for {0}!", msg);
                    p.MakeSelection(1, zones, DeleteZoneCallback);
                } else {
                    zones.Clear();
                    Player.Message(p, "TNT Wars: Deleted all zones for {0}!", msg);
                }
            } else if (opt.CaselessEq("check")) {
                Player.Message(p, "TNT Wars: Place a block to check for {0}!", msg);
                p.MakeSelection(1, zones, CheckZoneCallback);
            } else {
                return false;
            }
            return true;
        }
        
        static bool AddZoneCallback(Player p, Vec3S32[] marks, object state, BlockID block) {
            Vec3U16 p1 = (Vec3U16)marks[0], p2 = (Vec3U16)marks[1];
            TWGame.TWZone zn = new TWGame.TWZone();
            
            zn.MinX = Math.Min(p1.X, p2.X);
            zn.MinY = Math.Min(p1.Y, p2.Y);
            zn.MinZ = Math.Min(p1.Z, p2.Z);
            zn.MaxX = Math.Max(p1.X, p2.X);
            zn.MaxY = Math.Max(p1.Y, p2.Y);
            zn.MaxZ = Math.Max(p1.Z, p2.Z);

            List<TWGame.TWZone> zones = (List<TWGame.TWZone>)state;
            zones.Add(zn);
            Player.Message(p, "TNT Wars: Zone added!");
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
                    Player.Message(p, "TNT Wars: Zone deleted!");
                    any = true;
                }
            }
            
            if (!any) Player.Message(p, "TNT Wars Error: You weren't in any zone");
            return false;
        }
        
        static bool CheckZoneCallback(Player p, Vec3S32[] marks, object state, BlockID block) {
            ushort x = (ushort)marks[0].X, y = (ushort)marks[0].Y, z = (ushort)marks[0].Z;
            
            List<TWGame.TWZone> zones = (List<TWGame.TWZone>)state;
            TWGame game = TWGame.Instance;
            if (zones == game.tntFreeZones) {
                if (game.InZone(x, y, z, zones)) {
                    Player.Message(p, "TNT Wars: You are currently in a no TNT zone!");
                } else {
                    Player.Message(p, "TNT Wars: You are not currently in a no TNT zone!");
                }
            } else if (zones == game.tntImmuneZones) {
                if (game.InZone(x, y, z, zones)) {
                    Player.Message(p, "TNT Wars: You are currently in a no TNT block explosion zone (explosions won't destroy blocks)!");
                } else {
                    Player.Message(p, "TNT Wars: You are currently in a TNT block explosion zone (explosions will destroy blocks)!");
                }
            }
            return false;
        }
        
        static void SetDifficulty(TWGame game, TntWarsDifficulty diff, Player p) {
            if (p.level != game.Map)
                Player.Message(p, "TNT Wars: Changed difficulty to {0}", diff);
            game.SetDifficulty(diff);
        }
        
        static void SetBool(Player p, ref bool target, string opt, string name) {
            if (!CommandParser.GetBool(p, opt, ref target)) return;
            Player.Message(p, "TNT Wars: {0} is now {1}", name, GetBool(target));
        }
        
        static TWMapConfig RetrieveConfig(Player p) {
            TWMapConfig cfg = new TWMapConfig();
            cfg.SetDefaults(p.level);
            cfg.Load(p.level.name);
            return cfg;
        }
        
        static void UpdateConfig(Player p, TWMapConfig cfg) {
            cfg.Save(p.level.name);
            if (p.level == TWGame.Instance.Map) TWGame.Instance.UpdateMapConfig();
        }
        
        
        public override void Help(Player p, string message) {
            if (message.CaselessEq("set")) {
                Player.Message(p, "%T/Help TW team %H- Views help for team settings");
                Player.Message(p, "%T/Help TW score %H- Views help for score settings");
                Player.Message(p, "%T/Help TW zone %H- Views help for zone settings");
                Player.Message(p, "%T/Help TW other %H- Views help for other settings");
                Player.Message(p, "%T/TW set status %H- view status of settings");
            } else if (message.CaselessEq("team")) {
                Player.Message(p, "%T/TW set spawn [red/blue] %H- set the spawns for red/blue team");
                Player.Message(p, "%T/TW set balanceteams [on/off] %H- enable/disable balancing teams");
                Player.Message(p, "%T/TW set teamkill [on/off] %H- enable/disable teamkills");
            } else if (message.CaselessEq("score")) {
                Player.Message(p, "%T/TW set score required [points] %H- Sets score required to win the round");
                Player.Message(p, "%T/TW set score streaks [on/off] %H- enable/disable kill streaks");
                Player.Message(p, "%T/TW set score multi [points] %H- set the score bonus per multikill");
                Player.Message(p, "%T/TW set score kill [points] %H- set the score per kill");
                Player.Message(p, "%T/TW set score assist [points] %H- set the score per assist");
            } else if (message.CaselessEq("zone")) {
                // TODO: improve this massively
                Player.Message(p, "%T/TW set zone notnt/nodestroy add %H- Creates a new zone");
                Player.Message(p, "%T/TW set zone notnt/nodestroy delete %H- Deletes all zones affecting a block");
                Player.Message(p, "%T/TW set zone notnt/nodestroy delete all %H- Deletes all zones");
                Player.Message(p, "%T/TW set zone notnt/nodestroy check %H- Lists zones affecting a block");
                Player.Message(p, "(No TNT zones or zones where explosions do not delete blocks");
            } else if (message.CaselessEq("other")) {
                Player.Message(p, "%T/TW set tnt [amount] %H- sets the amount of tnt per player at a time");
                Player.Message(p, "%T/TW set graceperiod [on/off] %H- enable/disable the grace period");
                Player.Message(p, "%T/TW set gracetime [amount] %H- set the grace period time (in seconds)");
                Player.Message(p, "%T/TW set gamemode [tdm/ffa] %H- set team or free for all gamemode");
                Player.Message(p, "%T/TW set difficulty [easy/normal/hard/extreme] %H- set difficulty");
            } else {
                base.Help(p, message);
            }
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/TW start <map> %H- Starts TNT wars");
            Player.Message(p, "%T/TW stop %H- Stops TNT wars");
            Player.Message(p, "%T/TW end %H- Ends current round of TNT wars");
            Player.Message(p, "%T/TW set add/remove %H- Adds/removes current map from map list");
            Player.Message(p, "%T/TW set [property] %H- Sets a property. See %T/Help TW set");
            Player.Message(p, "%T/TW status %H- View your current score and health");
            Player.Message(p, "%T/TW go %H- Moves you to the current TNT wars map");
            Player.Message(p, "%T/TW rules %H- view the rules of TNT wars");
            Player.Message(p, "%T/TW scores %H- lists players with highest scores");
        }
    }
}
