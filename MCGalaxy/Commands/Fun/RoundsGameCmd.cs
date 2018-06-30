/*
    Copyright 2015 MCGalaxy

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
using MCGalaxy.Games;

namespace MCGalaxy.Commands.Fun {
    public abstract class RoundsGameCmd : Command {
        public override string type { get { return CommandTypes.Games; } }
        public override bool museumUsable { get { return false; } }
        public override bool SuperUseable { get { return false; } }
        protected abstract RoundsGame Game { get; }
        
        public override void Use(Player p, string message) {
            RoundsGame game = Game;
            if (message.CaselessEq("go")) {
                HandleGo(p, game);
            } else if (IsInfoCommand(message)) {
                HandleStatus(p, game);
            } else if (message.CaselessEq("start") || message.CaselessStarts("start ")) {
                if (!CheckExtraPerm(p, 1)) return;
                HandleStart(p, game, message.SplitSpaces());
            } else if (message.CaselessEq("end")) {
                if (!CheckExtraPerm(p, 1)) return;
                HandleEnd(p, game);
            } else if (message.CaselessEq("stop")) {
                if (!CheckExtraPerm(p, 1)) return;
                HandleStop(p, game);
            } else if (message.CaselessStarts("set ") || message.CaselessStarts("setup ")) {
                if (!CheckExtraPerm(p, 1)) return;
                HandleSet(p, game, message.SplitSpaces());
            } else {
                Help(p);
            }
        }

        protected void HandleGo(Player p, RoundsGame game) {
            if (!game.Running) {
                Player.Message(p, "{0} is not running", game.GameName);
            } else {
                PlayerActions.ChangeMap(p, game.Map);
            }
        }
        
        protected virtual void HandleStart(Player p, RoundsGame game, string[] args) {
            if (game.Running) { Player.Message(p, "{0} is already running", game.GameName); return; }

            string map = args.Length > 1 ? args[1] : "";
            game.Start(p, map, int.MaxValue);
        }
        
        protected void HandleEnd(Player p, RoundsGame game) {
            if (game.RoundInProgress) {
                game.EndRound();
            } else {
                Player.Message(p, "No round is currently in progress");
            }
        }
        
        protected void HandleStop(Player p, RoundsGame game) {
            if (!game.Running) {
                Player.Message(p, "{0} is not running", game.GameName);
            } else {
                game.End();
                Chat.MessageGlobal(game.GameName + " has ended! We hope you had fun!");
            }
        }

        protected void HandleStatus(Player p, RoundsGame game) {
            if (!game.Running) {
                Player.Message(p, "{0} is not running", game.GameName);
            } else {
                Player.Message(p, "Running on map: " + game.Map.ColoredName);
                game.OutputStatus(p);
            }
        }
        
        protected virtual void HandleSet(Player p, RoundsGame game, string[] args) {
            if (args.Length < 2) { Help(p, "set"); return; }
            string prop = args[1];
            
            if (prop.CaselessEq("add")) {
                RoundsGameConfig.AddMap(p, p.level.name, p.level.Config, game);
            } else if (IsDeleteCommand(prop)) {
                RoundsGameConfig.RemoveMap(p, p.level.name, p.level.Config, game);
            } else {
                HandleSetCore(p, game, args);
            }
        }
        
        protected abstract void HandleSetCore(Player p, RoundsGame game, string[] args);
    }
}
