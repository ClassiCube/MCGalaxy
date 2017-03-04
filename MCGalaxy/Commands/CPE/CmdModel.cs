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
        public override string name { get { return "model"; } }
        public override string shortcut { get { return "setmodel"; } }
        public override string type { get { return CommandTypes.Other; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public override CommandPerm[] ExtraPerms {
            get { return new[] { new CommandPerm(LevelPermission.Operator, "+ can change the model of others"),
                    new CommandPerm(LevelPermission.Operator, "+ can change the model of bots") }; }
        }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("xmodel") }; }
        }

        public override void Use(Player p, string message) {
            if (message.IndexOf(' ') == -1) {
                message = "-own " + message;
                message = message.TrimEnd();
            }
            UseBotOrPlayer(p, message, "model");
        }
        
        protected override void SetBotData(Player p, PlayerBot bot, string[] args) {
            string model = GetModel(p, args, 2);
            bot.model = model;
            bot.ModelBB = AABB.ModelAABB(model, bot.level);
            Entities.UpdateModel(bot.id, model, bot.level, null);
            
            Player.SendMessage(p, "You changed the model of bot " + bot.ColoredName + " %Sto a &c" + model);
            BotsFile.UpdateBot(bot);
        }
        
        protected override void SetPlayerData(Player p, Player who, string[] args) {
            string model = GetModel(p, args, 1);
            who.model = model;
            who.ModelBB = AABB.ModelAABB(model, who.level);
            Entities.UpdateModel(who.id, model, who.level, who);
            
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
        
        static string GetModel(Player p, string[] args, int i) {
            string model = args.Length > i ? args[i] : "humanoid";
            model = model.ToLower();
            model = model.Replace(':', '|'); // since many assume : is for scale instead of |.
            return model;
        }

        public override void Help(Player p) {
            Player.Message(p, "%T/model [name] [model] %H- Sets the model of that player.");
            Player.Message(p, "%T/model bot [name] [model] %H- Sets the model of that bot.");
            Player.Message(p, "%HAvailable models: %SChibi, Chicken, Creeper, Giant, Humanoid, Pig, Sheep, Spider, Skeleton, Zombie.");
            Player.Message(p, "%HTo set a block model, use a block ID for the model name.");
            Player.Message(p, "%HFor setting scaling models, put \"|[scale]\" after the model name (not supported by all clients).");
        }
    }
}
