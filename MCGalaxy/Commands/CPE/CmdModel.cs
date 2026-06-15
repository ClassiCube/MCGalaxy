/*
    Copyright 2015-2024 MCGalaxy
        
    Dual-licensed under the Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    https://opensource.org/license/ecl-2-0/
    https://www.gnu.org/licenses/gpl-3.0.html
    
    Unless required by applicable law or agreed to in writing,
    software distributed under the Licenses are distributed on an "AS IS"
    BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
    or implied. See the Licenses for the specific language governing
    permissions and limitations under the Licenses.
 */
using System;
using MCGalaxy.Bots;

namespace MCGalaxy.Commands.CPE 
{
    public class CmdModel : EntityPropertyCmd 
    {
        public override string name { get { return "Model"; } }
        public override string type { get { return CommandTypes.Other; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public override CommandPerm[] ExtraPerms {
            get { return new[] { new CommandPerm(LevelPermission.Operator, "can change the model of others"),
                    new CommandPerm(LevelPermission.Operator, "can change the model of bots") }; }
        }
        public override CommandAlias[] Aliases {
            get { return new[] {
                new CommandAlias("XModel"),
                new CommandAlias("OModel", OTHER_FLAG)
            }; }
        }

        public override void Use(Player p, string message, CommandData data) {
            UseBotOrOnline(p, data, message, "model");
        }
        
        protected override void SetBotData(Player p, PlayerBot bot, string model) {
            model = PlayerOperations.ParseModel(p, bot, model);
            if (model == null) return;
            bot.UpdateModel(model);
            
            p.Message("You changed the model of bot {0} &Sto a &c{1}", bot.ColoredName, model);
            BotsFile.Save(p.level);
        }
        
        protected override void SetOnlineData(Player p, Player who, string model) {
            PlayerOperations.SetModel(p, who, model);
        }

        public override void Help(Player p) {
            p.Message("&T/Model <model> &H- Sets your own model.");
            p.Message("&T/OModel [name] <model> &H- Sets the model of other player.");
            p.Message("&T/Model bot [name] <model> &H- Sets the model of that bot.");
            p.Message("&H Leave <model> blank to reset it.");
            p.Message("&H  Use &T/Help Model models &Hfor a list of models.");
            p.Message("&H  Use &T/Help Model scale &Hfor how to scale a model.");
        }
        
        public override void Help(Player p, string message) {
            if (message.CaselessEq("models")) {
                p.Message("&HAvailable models: &SChibi, Chicken, Creeper, Giant, Humanoid, Pig, Sheep, Spider, Skeleton, Zombie, Head, Sit, Corpse");
                p.Message("&HTo set a block model, use a block ID for the model name.");
                p.Message("&HUse &T/Help Model scale &Hfor how to scale a model.");
            } else if (message.CaselessEq("scale")) {
                p.Message("&HFor a scaled model, put \"|[scale]\" after the model name.");
                p.Message("&H  e.g. pig|0.5, chibi|3");
            } else {
                Help(p);
            }
        }
    }
}
