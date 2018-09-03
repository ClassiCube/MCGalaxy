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
            get { return new[] { new CommandAlias("ZG"), new CommandAlias("RoundTime", "set roundtime") }; }
        }
        public override CommandPerm[] ExtraPerms {
            get { return new[] { new CommandPerm(LevelPermission.Operator, "can manage zombie survival") }; }
        }
        
        protected override void HandleSet(Player p, RoundsGame game, string[] args) {
            ZSConfig cfg = ZSGame.Config;
            string prop = args[1];
            LevelConfig lCfg = p.level.Config;
            
            if (prop.CaselessEq("map")) {
                p.Message("Pillaring allowed: &b" + lCfg.Pillaring);
                p.Message("Build type: &b" + lCfg.BuildType);
                p.Message("Round time: &b{0}" + lCfg.RoundTime.Shorten(true, true));
                return;
            }
            if (args.Length < 3) { Help(p, "set"); return; }  
            
            if (prop.CaselessEq("hitbox")) {
                if (!CommandParser.GetReal(p, args[2], "Hitbox detection", ref cfg.HitboxDist, 0, 4)) return;
                p.Message("Set hitbox detection to &a" + cfg.HitboxDist + " %Sblocks apart");
                
                cfg.Save(); return;
            } else if (prop.CaselessEq("maxmove")) {
                if (!CommandParser.GetReal(p, args[2], "Max move distance", ref cfg.MaxMoveDist, 0, 4)) return;
                p.Message("Set max move distance to &a" + cfg.MaxMoveDist + " %Sblocks apart");
                
                cfg.Save(); return;
            } else if (prop.CaselessEq("pillaring")) {
                if (!CommandParser.GetBool(p, args[2], ref lCfg.Pillaring)) return;
                
                p.Message("Set pillaring allowed to &b" + lCfg.Pillaring);
                game.UpdateAllStatus2();
            } else if (prop.CaselessEq("build")) {
                if (!CommandParser.GetEnum(p, args[2], "Build type", ref lCfg.BuildType)) return;
                p.level.UpdateBlockPermissions();
                
                p.Message("Set build type to &b" + lCfg.BuildType);
                game.UpdateAllStatus2();
            } else if (prop.CaselessEq("roundtime")) {
                if (!ParseTimespan(p, "round time", args, ref lCfg.RoundTime)) return;
            } else {
                Help(p, "set"); return;
            }
            Level.SaveSettings(p.level);
        }
        
        static bool ParseTimespan(Player p, string arg, string[] args, ref TimeSpan span) {
            if (!CommandParser.GetTimespan(p, args[2], ref span, "set " + arg + " to", "m")) return false;
            p.Message("Set {0} to &b{1}", arg, span.Shorten(true));
            return true;
        }
        
        public override void Help(Player p, string message) {
            if (message.CaselessEq("set")) {
                p.Message("%T/Help ZS game %H- Views help for game settings");
                p.Message("%T/Help ZS map %H- Views help for per-map settings");
            } else if (message.CaselessEq("game")) {
                p.Message("%T/ZS set hitbox [distance]");
                p.Message("%HSets furthest apart players can be before they are considered touching.");
                p.Message("%T/ZS set maxmove [distance]");
                p.Message("%HSets largest distance players can move in a tick" +
                               "before they are considered speedhacking.");
            } else if (message.CaselessEq("map")) {
                p.Message("%T/ZS set map %H-Views map settings");
                p.Message("%T/ZS set pillaring [yes/no]");
                p.Message("%HSets whether players are allowed to pillar");
                p.Message("%T/ZS set build [normal/modifyonly/nomodify]");
                p.Message("%HSets build type of the map");
                p.Message("%T/ZS set roundtime [timespan]");
                p.Message("%HSets how long a round is");
            } else {
                base.Help(p, message);
            }
        }
        
        public override void Help(Player p) {
            p.Message("%T/ZS start <map> %H- Starts Zombie Survival");
            p.Message("%T/ZS stop %H- Stops Zombie Survival");
            p.Message("%T/ZS end %H- Ends current round of Zombie Survival");
            p.Message("%T/ZS add/remove %H- Adds/removes current map from map list");
            p.Message("%T/ZS set [property] %H- Sets a property. See %T/Help ZS set");
            p.Message("%T/ZS status %H- Outputs current status of Zombie Survival");
            p.Message("%T/ZS go %H- Moves you to the current Zombie Survival map");
        }
    }
}
