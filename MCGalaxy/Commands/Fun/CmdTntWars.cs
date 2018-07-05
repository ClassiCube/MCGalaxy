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
            TntWarsGame1 it = TntWarsGame1.GameIn(p);
            if (it.GameStatus != TntWarsGame1.TntWarsStatus.InProgress) {
                Player.Message(p, "TNT Wars Error: Can't display scores - game not in progress!");
                return;
            }
            
            List<TntWarsGame1.player> top = it.SortedByScore();
            int count = Math.Min(it.PlayingPlayers(), 5);
            
            Player.Message(p, "Top {0} scores:", count);
            for (int i = 0; i < count; i++) {
                Player.Message(p, "{0}) {1} - {2} points", (i + 1), top[i].p.name, top[i].Score);
                Thread.Sleep(500); //Maybe, not sure (250??)
            }
        }
        
        protected override void HandleSetCore(Player p, RoundsGame game, string[] args) {
            throw new NotImplementedException();
        }
        
        void DoSetup(Player p, string message, RoundsGame game_) {
            string[] text = new string[] { "", "", "", "", "" };
            string[] parts = message.ToLower().SplitSpaces();
            for (int i = 0; i < Math.Min(text.Length, parts.Length); i++)
                text[i] = parts[i];
            
            if (!CheckExtraPerm(p, 1)) return;
            TWGame game = (TWGame)game_;
            
            TWMapConfig cfg = RetrieveConfig(p);
            TWConfig gameCfg = TWGame.Config;
            
            /*
                case "reset":
                    it.GameStatus = TntWarsGame1.TntWarsStatus.WaitingForPlayers;
                    Command.Find("Restore").Use(null, it.BackupNumber + it.lvl.name);
                    it.RedScore = 0;
                    it.BlueScore = 0;
                    
                    foreach (TntWarsGame1.player pl in it.Players) {
                        pl.Score = 0;
                        pl.spec = false;
                        pl.p.TntWarsKillStreak = 0;
                        pl.p.TNTWarsLastKillStreakAnnounced = 0;
                        pl.p.CurrentAmountOfTnt = 0;
                        pl.p.CurrentTntGameNumber = it.GameNumber;
                        pl.p.PlayingTntWars = false;
                        pl.p.canBuild = true;
                        pl.p.TntWarsHealth = 2;
                        pl.p.TntWarsScoreMultiplier = 1f;
                        pl.p.inTNTwarsMap = true;
                        pl.p.HarmedBy = null;
                        TntWarsGame1.SetTitlesAndColor(pl, true);
                    }
                    Player.Message(p, "TNT Wars: Reset TNT Wars");
                    break;
             */

            switch (text[0]) {
                case "spawn":
                    if (gameCfg.Mode == TntWarsGameMode.FFA) { 
            		    Player.Message(p, "&cCannot set spawns in Free For All mode"); return; 
            		}
                    switch (text[2])
                    {
                        case "red":
                            cfg.RedSpawn = (Vec3U16)p.Pos.FeetBlockCoords;
                            Player.Message(p, "TNT Wars: Set &cRed %Sspawn");
                            break;
                        case "blue":
                            cfg.BlueSpawn = (Vec3U16)p.Pos.FeetBlockCoords;
                            Player.Message(p, "TNT Wars: Set &9Blue %Sspawn");
                            break;
                    }
                    break;

                case "tntatatime":
                case "tnt":
                    int amount = 1;
                    if (!CommandParser.GetInt(p, text[2], "TNT at a time", ref amount, 0)) return;
                    cfg.MaxPlayerActiveTnt = amount;
                    
                    Player.Message(p, "TNT Wars: Number of TNTs placeable by a player at a time is now {0}",
                                   amount == 0 ? "unlimited" : amount.ToString());
                    break;
                    
                case "graceperiod":
                    SetBool(p, ref cfg.InitialGracePeriod, text[2], "Grace period");
                    break;

                case "gracetime":
                    int time = 1;
                    if (!CommandParser.GetInt(p, text[2], "Grace time", ref time, 10, 300)) return;
                    cfg.GracePeriodSeconds = time;
                    
                    Player.Message(p, "TNT Wars: Grace period is now {0} seconds long", time);
                    break;

                case "mode":
                case "gamemode":
                    if (text[2] == "tdm") {
                        if (gameCfg.Mode == TntWarsGameMode.FFA) {
                            if (p.level != game.Map) {
                                Player.Message(p, "TNT Wars: Changed gamemode to Team Deathmatch");
                            }
                            game.ModeTDM();
                        } else {
                            Player.Message(p, "&cGamemode is already Team Deathmatch"); return;
                        }
                    } else if (text[2] == "ffa") {
                        if (gameCfg.Mode == TntWarsGameMode.TDM) {
                            if (p.level != game.Map) {
                                Player.Message(p, "TNT Wars: Changed gamemode to Free For All");
                            }
                            game.ModeFFA();
                        } else {
                            Player.Message(p, "&cGamemode is already Free For All"); return;
                        }
                    }
                    break;

                case "difficulty":
                    switch (text[2])
                    {
                        case "easy":
                        case "1":
                    	    SetDifficulty(game, TntWarsDifficulty.Easy, p); break;

                        case "normal":
                        case "2":
                        default:
                            SetDifficulty(game, TntWarsDifficulty.Normal, p); break;
                            
                        case "hard":
                        case "3":
                            SetDifficulty(game, TntWarsDifficulty.Hard, p); break;

                        case "extreme":
                        case "4":
                            SetDifficulty(game, TntWarsDifficulty.Extreme, p); break;
                    }
                    break;

                case "score":
                    switch (text[2])
                    {
                        case "required":
                            int score = 1;
                            if (!CommandParser.GetInt(p, text[3], "Points", ref score, 0)) return;
                            cfg.ScoreRequired = score;
                            
                            Player.Message(p, "TNT Wars: Score required to win is now &a{0} %Spoints!", score);
                            break;

                        case "streaks":
                            SetBool(p, ref cfg.Streaks, text[3], "Streaks");
                            break;

                        case "multi":
                            int multi = 1;
                            if (!CommandParser.GetInt(p, text[3], "Points", ref multi, 0)) return;
                            cfg.MultiKillBonus = multi;
                            
                            if (multi == 0) {
                                Player.Message(p, "TNT Wars: Multikill bonus is now &cdisabled");
                            } else {
                                Player.Message(p, "TNT Wars: Scores per extra kill is now &a{0} %Spoints", multi);
                            }
                            break;

                        case "kill":
                            int kill = 1;
                            if (!CommandParser.GetInt(p, text[3], "Points", ref kill, 0)) return;
                            cfg.ScorePerKill = kill;
                            
                            Player.Message(p, "TNT Wars: Score per kill is now &a{0} %Spoints", kill);
                            break;
                            
                        case "assist":
                            int assist = 1;
                            if (!CommandParser.GetInt(p, text[3], "Points", ref assist, 0)) return;
                            cfg.AssistScore = assist;
                            
                            if (assist == 0) {
                                Player.Message(p, "TNT Wars: Scores per assist is now &cdisabled");
                            } else {
                                Player.Message(p, "TNT Wars: Score per assist is now &a{0} %Spoints", assist);
                            }
                            break;
                    }
                    break;

                case "balanceteams":
                    SetBool(p, ref cfg.BalanceTeams, text[2], "Team balancing");
                    break;

                case "teamkill":
                    SetBool(p, ref cfg.TeamKills, text[2], "Team killing");
                    break;

                case "zone":
                case "zones":
                case "z":
                    switch (text[2])
                    {
                        case "notnt":
                        case "nt":
                            HandleZone(p, game, true, text);
                            break;

                        case "noexplosion":
                        case "nodeleteblocks":
                        case "neb":
                            HandleZone(p, game, false, text);
                            break;
                    }
                    break;

                default:
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
                    break;
            }
            UpdateConfig(p, cfg);
        }
        
        static string GetBool(bool value) { return value ? "&aEnabled" : "&cDisabled"; }
        
        void HandleZone(Player p, TWGame game, bool noTntZone, string[] text) {
            string msg = noTntZone ? "no TNT" : "no blocks deleted on explosions";
            List<TntWarsGame1.Zone> zones = noTntZone ? game.NoTNTplacableZones : game.NoBlockDeathZones;
            
            if (IsCreateCommand(text[3])) {
                Player.Message(p, "TNT Wars: Place 2 blocks to create the zone for {0}!", msg);
                p.MakeSelection(2, zones, AddZoneCallback);
            } else if (IsDeleteCommand(text[3])) {
                if (!text[4].CaselessEq("all")) {
                    Player.Message(p, "TNT Wars: Place a block to delete the zone for {0}!", msg);
                    p.MakeSelection(1, zones, DeleteZoneCallback);
                } else {
                    zones.Clear();
                    Player.Message(p, "TNT Wars: Deleted all zones for {0}!", msg);
                }
            } else if (text[3].CaselessEq("check")) {
                Player.Message(p, "TNT Wars: Place a block to check for {0}!", msg);
                p.MakeSelection(1, zones, CheckZoneCallback);
            }
        }
        
        static bool AddZoneCallback(Player p, Vec3S32[] marks, object state, BlockID block) {
            Vec3U16 p1 = (Vec3U16)marks[0], p2 = (Vec3U16)marks[1];
            TntWarsGame1.Zone zn = new TntWarsGame1.Zone();
            
            zn.MinX = Math.Min(p1.X, p2.X);
            zn.MinY = Math.Min(p1.Y, p2.Y);
            zn.MinZ = Math.Min(p1.Z, p2.Z);
            zn.MaxX = Math.Max(p1.X, p2.X);
            zn.MaxY = Math.Max(p1.Y, p2.Y);
            zn.MaxZ = Math.Max(p1.Z, p2.Z);

            List<TntWarsGame1.Zone> zones = (List<TntWarsGame1.Zone>)state;
            zones.Add(zn);
            Player.Message(p, "TNT Wars: Zone added!");
            return false;
        }
        
        static bool DeleteZoneCallback(Player p, Vec3S32[] marks, object state, BlockID block) {
            ushort x = (ushort)marks[0].X, y = (ushort)marks[0].Y, z = (ushort)marks[0].Z;
            List<TntWarsGame1.Zone> zones = (List<TntWarsGame1.Zone>)state;
            
            foreach (TntWarsGame1.Zone zn in zones) {
                if (x >= zn.MinX && x <= zn.MaxX && y >= zn.MinY && y <= zn.MaxY && z >= zn.MinZ && z <= zn.MaxZ) {
                    zones.Remove(zn);
                    Player.Message(p, "TNT Wars: Zone deleted!");
                    return false;
                }
            }
            
            Player.Message(p, "TNT Wars Error: You weren't in any zone");
            return false;
        }
        
        static bool CheckZoneCallback(Player p, Vec3S32[] marks, object state, BlockID block) {
            ushort x = (ushort)marks[0].X, y = (ushort)marks[0].Y, z = (ushort)marks[0].Z;
            
            List<TntWarsGame1.Zone> zones = (List<TntWarsGame1.Zone>)state;
            TWGame game = TWGame.Instance;
            if (zones == game.NoTNTplacableZones) {
                if (game.InZone(x, y, z, zones)) {
                    Player.Message(p, "TNT Wars: You are currently in a no TNT zone!");
                } else {
                    Player.Message(p, "TNT Wars: You are not currently in a no TNT zone!");
                }
            } else if (zones == game.NoBlockDeathZones) {
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
                Player.Message(p, "/tw s zone [notnt {nt}/noexplodeblocks {neb}] [add {a}/delete {d} <all>/check {c}]- create zones (No TNT zones or zones where explosions do not delete blocks");
            } else if (message.CaselessEq("other")) {
                Player.Message(p, "%T/TW set tnt [amount] %H- sets the amount of tnt per player at a time");
                Player.Message(p, "%T/TW set graceperiod [on/off] %H- enable/disable the grace period");
                Player.Message(p, "%T/TW set gracetime [amount] %H- set the grace period time (in seconds)");
                // TODO: game section
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
