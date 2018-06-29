/*
    Copyright 2010 MCLawl Team -
    Created by Snowl (David D.) and Cazzar (Cayde D.)

    Dual-licensed under the    Educational Community License, Version 2.0 and
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
using MCGalaxy.Games;

namespace MCGalaxy.Commands.Fun {
	public sealed class CmdZombieGame : RoundsGameCmd {
		public override string name { get { return "ZombieSurvival"; } }
		public override string shortcut { get { return "ZS"; } }
		protected override RoundsGame Game { get { return Server.zombie; } }
		public override CommandAlias[] Aliases {
			get { return new[] { new CommandAlias("ZG"), new CommandAlias("RoundTime", "set roundtime") }; }
		}
		public override CommandPerm[] ExtraPerms {
			get { return new[] { new CommandPerm(LevelPermission.Operator, "can manage zombie survival") }; }
		}
		
		protected override void HandleSet(Player p, RoundsGame game, string[] args) {
			if (!CheckExtraPerm(p, 1)) return;
			if (args.Length < 2) { Help(p, "set"); return; }
			
			ZSGame zs = (ZSGame)game;
			ZSConfig cfg = ZSGame.Config;
			string prop = args[1];
			
			if (prop.CaselessEq("hitbox")) {
				if (args.Length == 2) {
					Player.Message(p, "Hitbox detection is currently &a" + ZSGame.Config.HitboxDist + " %Sunits apart."); return;
				}

				if (!CommandParser.GetInt(p, args[2], "Hitbox detection", ref cfg.HitboxDist, 0, 256)) return;
				Player.Message(p, "Hitbox detection set to &a" + cfg.HitboxDist + " %Sunits apart.");
				cfg.Save();
			} else if (prop.CaselessEq("maxmove")) {
				if (args.Length == 2) {
					Player.Message(p, "Maxmium move distance is currently &a" + ZSGame.Config.MaxMoveDist + " %Sunits apart."); return;
				}

				if (!CommandParser.GetInt(p, args[2], "Maxmimum move distance", ref cfg.MaxMoveDist, 0, 256)) return;
				Player.Message(p, "Maximum move distance set to &a" + cfg.MaxMoveDist + " %Sunits apart.");
				cfg.Save();
			} else {
				Help(p, "set");
			}
			
            if (args.Length == 0) {
                Player.Message(p, "Map authors: " + p.level.Config.Authors);
                Player.Message(p, "Pillaring allowed: " + p.level.Config.Pillaring);
                Player.Message(p, "Build type: " + p.level.Config.BuildType);
                Player.Message(p, "Min round time: " + p.level.Config.MinRoundTime + " minutes");
                Player.Message(p, "Max round time: " + p.level.Config.MaxRoundTime + " minutes");
                return;
            }
            
            if (args.Length == 1) { Player.Message(p, "You need to provide a value."); return; }
            LevelConfig lCfg = p.level.Config;
            
            if (args[0].CaselessEq("authors")) {
                p.level.Config.Authors = args[1].Replace(" ", "%S, ");
                Player.Message(p, "Sets the authors of the map to: " + args[1]);
            } else if (args[0].CaselessEq("pillaring")) {
                if (!CommandParser.GetBool(p, args[1], ref lCfg.Pillaring)) return;
                
                Player.Message(p, "Set pillaring allowed to: " + lCfg.Pillaring);
                Server.zombie.UpdateAllStatus2();
            } else if (args[0].CaselessEq("build")) {
                if (!CommandParser.GetEnum(p, args[1], "Build type", ref lCfg.BuildType)) return;
                
                p.level.UpdateBlockPermissions();
                Player.Message(p, "Set build type to: " + lCfg.BuildType);
                Server.zombie.UpdateAllStatus2();
            } else if (args[0].CaselessEq("minroundtime") || args[0].CaselessEq("minround")) {
                int time = GetRoundTime(p, args[1]);
                if (time == 0) return;
                
                if (time > p.level.Config.MaxRoundTime) {
                    Player.Message(p, "Min round time must be less than or equal to max round time"); return;
                }
                p.level.Config.MinRoundTime = time;
                Player.Message(p, "Set min round time to: " + time + " minutes");
            } else if (args[0].CaselessEq("maxroundtime") || args[0].CaselessEq("maxround")) {
                int time = GetRoundTime(p, args[1]);
                if (time == 0) return;
                
                if (time < p.level.Config.MinRoundTime) {
                    Player.Message(p, "Max round time must be greater than or equal to min round time"); return;
                }
                p.level.Config.MaxRoundTime = time;
                Player.Message(p, "Set max round time to: " + time + " minutes");
            } else if (args[0].CaselessEq("roundtime") || args[0].CaselessEq("round")) {
                int time = GetRoundTime(p, args[1]);
                if (time == 0) return;
                
                p.level.Config.MinRoundTime = time;
                p.level.Config.MaxRoundTime = time;
                Player.Message(p, "Set round time to: " + time + " minutes");
            } else {
                Player.Message(p, "Unrecognised property \"" + args[0] + "\"."); return;
            }
            Level.SaveSettings(p.level);
        }
        
        static int GetRoundTime(Player p, string arg) {
            int time = 0;
            if (!CommandParser.GetInt(p, arg, "Minutes", ref time, 1, 10)) return 0;            
            return time;
        }
		
		public override void Help(Player p, string message) {
			if (message.CaselessEq("set")) {
				Player.Message(p, "%T/Help ZS game %H- Views help for game settings");
				Player.Message(p, "%T/Help ZS map %H- Views help for per-map settings");
			} else if (message.CaselessEq("game")) {
				Player.Message(p, "%T/ZS set hitbox [distance]");
				Player.Message(p, "%HSets how far apart players need to be before they " +
				               "are considered touching. (32 units = 1 block).");
				Player.Message(p, "%T/ZS set maxmove [distance]");
				Player.Message(p, "%HSets how far apart players are allowed to move in a " +
				               "movement packet before they are considered speedhacking. (32 units = 1 block).");
			} else if (message.CaselessEq("map")) {
				Player.Message(p, "%T/ZS set pillaring [yes/no]");
				Player.Message(p, "%HSets whether players are allowed to pillar");
				Player.Message(p, "%T/ZS set build [normal/modifyonly/nomodify]");
				Player.Message(p, "%HSets build type of the map");
				Player.Message(p, "%T/ZS set minroundtime [timespan]");
				Player.Message(p, "%T/ZS set maxroundtime [timespan]");
				Player.Message(p, "%HSets duration for which a round can last");
			} else {
				base.Help(p, message);
			}
		}
		
		public override void Help(Player p) {
			Player.Message(p, "%T/ZS start <map> %H- Starts Zombie Survival");
			Player.Message(p, "%T/ZS stop %H- Stops Zombie Survival");
			Player.Message(p, "%T/ZS end %H- Ends current round of Zombie Survival");
			Player.Message(p, "%T/ZS set [property] %H- Sets a property. See %T/Help ZG set");
			Player.Message(p, "%T/ZS status %H- Outputs current status of Zombie Survival");
			Player.Message(p, "%T/ZS go %H- Moves you to the current Zombie Survival map");
		}
	}
}
