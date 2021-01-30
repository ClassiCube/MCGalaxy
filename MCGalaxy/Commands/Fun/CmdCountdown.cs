/*  Copyright 2011 MCForge
        
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
//--------------------------------------------------\\
//the whole of the game 'countdown' was made by edh649\\
//======================================================\\
using System;
using MCGalaxy.Commands.World;
using MCGalaxy.Games;
using MCGalaxy.Generator;

namespace MCGalaxy.Commands.Fun {
    
    public sealed class CmdCountdown : RoundsGameCmd {
        public override string name { get { return "CountDown"; } }
        public override string shortcut { get { return "CD"; } }
        protected override RoundsGame Game { get { return CountdownGame.Instance; } }
        public override CommandPerm[] ExtraPerms {
            get { return new[] { new CommandPerm(LevelPermission.Operator, "can manage countdown") }; }
        }
        
        public override void Use(Player p, string message, CommandData data) {
            if (message.CaselessEq("join")) {
                HandleJoin(p, CountdownGame.Instance);
            } else {
                base.Use(p, message, data);
            }
        }
        
        void HandleJoin(Player p, CountdownGame game) {
            if (!game.Running) {
                p.Message("Cannot join as countdown is not running.");
            } else if (game.RoundInProgress) {
                p.Message("Cannot join when a round is in progress. Wait until next round.");
            } else {
                game.PlayerJoinedGame(p);
            }
        }

        static string FormatPlayer(Player pl, CountdownGame game) {
            string suffix = game.Remaining.Contains(pl) ? " &a[IN]" : " &c[OUT]";
            return pl.ColoredName + suffix;
        }
        
        protected override void HandleSet(Player p, RoundsGame game_, string[] args) {
            if (args.Length < 4) { Help(p); return; }
            if (game_.Running) {
                p.Message("You must stop Countdown before replacing the map."); return;
            }
            
            ushort x = 0, y = 0, z = 0;
            if (!MapGen.GetDimensions(p, args, 1, ref x, ref y, ref z)) return;
            
            CountdownGame game = (CountdownGame)game_;
            game.GenerateMap(p, x, y, z);
        }        
        
        protected override void HandleStart(Player p, RoundsGame game_, string[] args) {
            if (game_.Running) { p.Message("{0} is already running", game_.GameName); return; }
            
            CountdownGame game = (CountdownGame)game_;
            string speed = args.Length > 1 ? args[1] : "";
            string  mode = args.Length > 2 ? args[2] : "";
            
            switch (speed) {
                case "slow":     game.Interval = 800; break;
                case "normal":   game.Interval = 650; break;
                case "fast":     game.Interval = 500; break;
                case "extreme":  game.Interval = 300; break;
                case "ultimate": game.Interval = 150; break;
                
                default:
                    p.Message("No speed specified, playing at 'normal' speed.");
                    game.Interval = 650; speed = "normal"; break;
            }
            
            game.FreezeMode = mode == "freeze" || mode == "frozen";
            game.SpeedType = speed;
            game.Start(p, "countdown", int.MaxValue);
        }       
        
        public override void Help(Player p) {
            p.Message("&T/CD set [width] [height] [length]");
            p.Message("&HRe-generates the countdown map (default is 32x32x32)");
            p.Message("&T/CD start <speed> <mode> &H- Starts Countdown");
            p.Message("&H  speed can be: slow, normal, fast, extreme or ultimate");
            p.Message("&H  mode can be: normal or freeze");
            p.Message("&T/CD stop &H- Stops Countdown"); 
            p.Message("&T/CD end &H- Ends current round of Countdown");
            p.Message("&T/CD join &H- joins the game");
            p.Message("&T/CD status &H- lists players currently playing");
        }
    }
}
