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
    public sealed class CmdZombieSurvival : RoundsGameCmd {
        public override string name { get { return "ZombieSurvival"; } }
        public override string shortcut { get { return "ZS"; } }
        protected override RoundsGame Game { get { return ZSGame.Instance; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("ZG"), new CommandAlias("RoundTime", "set round") }; }
        }
        public override CommandPerm[] ExtraPerms {
            get { return new[] { new CommandPerm(LevelPermission.Operator, "can manage zombie survival") }; }
        }
        
        protected override void HandleSet(Player p, RoundsGame game, string[] args) {
            ZSConfig cfg = ZSGame.Config;
            string prop = args[1];
            LevelConfig lCfg = p.level.Config;
            
            if (prop.CaselessEq("map")) {
                Player.Message(p, "Pillaring allowed: &b" + lCfg.Pillaring);
                Player.Message(p, "Build type: &b" + lCfg.BuildType);
                Player.Message(p, "Round time: Min &b{0}, %S Max &b{1}",
                               lCfg.MinRoundTime.Shorten(true), lCfg.MaxRoundTime.Shorten(true));
                return;
            }
            if (args.Length < 3) { Help(p, "set"); return; }  
            
            if (prop.CaselessEq("hitbox")) {
                if (!CommandParser.GetReal(p, args[2], "Hitbox detection", ref cfg.HitboxDist, 0, 4)) return;
                Player.Message(p, "Set hitbox detection to &a" + cfg.HitboxDist + " %Sblocks apart");
                
                cfg.Save(); return;
            } else if (prop.CaselessEq("maxmove")) {
                if (!CommandParser.GetReal(p, args[2], "Max move distance", ref cfg.MaxMoveDist, 0, 4)) return;
                Player.Message(p, "Set max move distance to &a" + cfg.MaxMoveDist + " %Sblocks apart");
                
                cfg.Save(); return;
            } else if (prop.CaselessEq("pillaring")) {
                if (!CommandParser.GetBool(p, args[1], ref lCfg.Pillaring)) return;
                
                Player.Message(p, "Set pillaring allowed to &b" + lCfg.Pillaring);
                game.UpdateAllStatus2();
            } else if (prop.CaselessEq("build")) {
                if (!CommandParser.GetEnum(p, args[1], "Build type", ref lCfg.BuildType)) return;
                p.level.UpdateBlockPermissions();
                
                Player.Message(p, "Set build type to &b" + lCfg.BuildType);
                game.UpdateAllStatus2();
            } else if (prop.CaselessEq("minround")) {
                if (!ParseTimespan(p, "min round time", args, ref lCfg.MinRoundTime)) return;
            } else if (prop.CaselessEq("maxround")) {
                if (!ParseTimespan(p, "max round time", args, ref lCfg.MaxRoundTime)) return;
            } else if (prop.CaselessEq("round")) {
                if (!ParseTimespan(p, "round time", args, ref lCfg.MinRoundTime)) return;
                lCfg.MaxRoundTime = lCfg.MinRoundTime;
            } else {
                Help(p, "set"); return;
            }
            Level.SaveSettings(p.level);
        }
        
        static bool ParseTimespan(Player p, string arg, string[] args, ref TimeSpan span) {
            if (!CommandParser.GetTimespan(p, args[2], ref span, "set " + arg + " to", "m")) return false;
            Player.Message(p, "Set {0} to &b{1}", arg, span.Shorten(true));
            return true;
        }
        
        public override void Help(Player p, string message) {
            if (message.CaselessEq("set")) {
                Player.Message(p, "%T/Help ZS game %H- Views help for game settings");
                Player.Message(p, "%T/Help ZS map %H- Views help for per-map settings");
            } else if (message.CaselessEq("game")) {
                Player.Message(p, "%T/ZS set hitbox [distance]");
                Player.Message(p, "%HSets furthest apart players can be before they are considered touching.");
                Player.Message(p, "%T/ZS set maxmove [distance]");
                Player.Message(p, "%HSets largest distance players can move in a tick" +
                               "before they are considered speedhacking.");
            } else if (message.CaselessEq("map")) {
                Player.Message(p, "%T/ZS set map %H-Views map settings");
                Player.Message(p, "%T/ZS set pillaring [yes/no]");
                Player.Message(p, "%HSets whether players are allowed to pillar");
                Player.Message(p, "%T/ZS set build [normal/modifyonly/nomodify]");
                Player.Message(p, "%HSets build type of the map");
                Player.Message(p, "%T/ZS set minround/maxround [timespan]");
                Player.Message(p, "%HSets duration for which a round can last");
            } else {
                base.Help(p, message);
            }
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/ZS start <map> %H- Starts Zombie Survival");
            Player.Message(p, "%T/ZS stop %H- Stops Zombie Survival");
            Player.Message(p, "%T/ZS end %H- Ends current round of Zombie Survival");
            Player.Message(p, "%T/ZS add/remove %H- Adds/removes current map from map list");
            Player.Message(p, "%T/ZS set [property] %H- Sets a property. See %T/Help ZS set");
            Player.Message(p, "%T/ZS status %H- Outputs current status of Zombie Survival");
            Player.Message(p, "%T/ZS go %H- Moves you to the current Zombie Survival map");
        }
    }
}
