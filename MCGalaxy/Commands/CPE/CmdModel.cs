/*
    Copyright 2015 MCGalaxy
        
    Dual-licensed under the Educational Community License, Version 2.0 and
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
using MCGalaxy.Bots;

namespace MCGalaxy.Commands.CPE {
    public class CmdModel : EntityPropertyCmd {
        public override string name { get { return "Model"; } }
        public override string shortcut { get { return "SetModel"; } }
        public override string type { get { return CommandTypes.Other; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public override CommandPerm[] ExtraPerms {
            get { return new[] { new CommandPerm(LevelPermission.Operator, "+ can change the model of others"),
                    new CommandPerm(LevelPermission.Operator, "+ can change the model of bots") }; }
        }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("XModel") }; }
        }

        public override void Use(Player p, string message) {
            if (message.IndexOf(' ') == -1) {
                message = "-own " + message;
                message = message.TrimEnd();
            }
            UseBotOrPlayer(p, message, "model");
        }
        
        protected override void SetBotData(Player p, PlayerBot bot, string model) {
            model = GetModel(model);
            Entities.UpdateModel(bot, model);
            
            Player.Message(p, "You changed the model of bot " + bot.ColoredName + " %Sto a &c" + model);
            BotsFile.UpdateBot(bot);
        }
        
        protected override void SetPlayerData(Player p, Player who, string model) {
            model = GetModel(model);
            Entities.UpdateModel(who, model);
            
            if (p != who) {
                Player.GlobalMessage(who, who.ColoredName + "'s %Smodel was changed to a &c" + model);
            } else {
                Player.Message(who, "Changed your own model to a &c" + model);
            }
            
            if (model != "humanoid") {
                Server.models.AddOrReplace(who.name, model);
            } else {
                Server.models.Remove(who.name);
            }
            Server.models.Save();
        }
        
        static string GetModel(string model) {
            if (model.Length == 0) model = "humanoid";
            model = model.ToLower();
            model = model.Replace(':', '|'); // since many assume : is for scale instead of |.
            return model;
        }

        public override void Help(Player p) {
            Player.Message(p, "%T/Model [name] [model] %H- Sets the model of that player.");
            Player.Message(p, "%T/Model bot [name] [model] %H- Sets the model of that bot.");
            Player.Message(p, "%HType %T/Help Model models %Hfor a list of models.");
        }
        
        public override void Help(Player p, string message) {
            if (message.CaselessEq("models")) {
                Player.Message(p, "%HAvailable models: %SChibi, Chicken, Creeper, Giant, Humanoid, Pig, Sheep, Spider, Skeleton, Zombie, Head, Sitting");
                Player.Message(p, "%HTo set a block model, use a block ID for the model name.");
                Player.Message(p, "%HFor setting a scaled model, put \"|[scale]\" after the model name. (e.g. pig|0.5)");
            } else {
                Help(p);
            }
        }
    }
}
