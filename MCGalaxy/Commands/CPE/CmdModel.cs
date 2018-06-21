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
            get { return new[] { new CommandPerm(LevelPermission.Operator, "can change the model of others"),
                    new CommandPerm(LevelPermission.Operator, "can change the model of bots") }; }
        }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("XModel", "-own") }; }
        }

        public override void Use(Player p, string message) {
            if (message.IndexOf(' ') == -1) {
                message = "-own " + message;
                message = message.TrimEnd();
            }
            UseBotOrPlayer(p, message, "model");
        }
        
        protected override void SetBotData(Player p, PlayerBot bot, string model) {
            bool changedAxisScale;
            model = ParseModel(p, bot, model, out changedAxisScale);
            Entities.UpdateModel(bot, model);
            
            Player.Message(p, "You changed the model of bot " + bot.ColoredName + " %Sto a &c" + model);
            BotsFile.Save(bot.level);
        }
        
        protected override void SetPlayerData(Player p, Player who, string model) {
            bool changedAxisScale;
            model = ParseModel(p, who, model, out changedAxisScale);
            Entities.UpdateModel(who, model);
            
            if (p != who) {
                Chat.MessageFrom(who, "λNICK %Shad their model change to a &c" + model);
            } else {
                Player.Message(who, "Changed your own model to a &c" + model);
            }
            
            if (!model.CaselessEq("humanoid")) {
                Server.models.AddOrReplace(who.name, model);
            } else {
                Server.models.Remove(who.name);
            }
            Server.models.Save();
            
            if (!changedAxisScale) return;
            if (who.ScaleX != 0 || who.ScaleY != 0 || who.ScaleZ != 0) {
                Server.modelScales.AddOrReplace(who.name, who.ScaleX + " " + who.ScaleY + " " + who.ScaleZ);
            } else {
                Server.modelScales.Remove(who.name);
            }
            Server.modelScales.Save();
        }
        
        static string ParseModel(Player dst, Entity entity, string model, out bool changedAxisScale) {
            if (model.Length == 0) model = "humanoid";
            model = model.ToLower();
            model = model.Replace(':', '|'); // since many assume : is for scale instead of |.
            changedAxisScale = false;
            
            if (model.CaselessStarts("x ")) {
                changedAxisScale = true;
                return ParseModelScale(dst, entity, model, "X scale", ref entity.ScaleX);
            } else if (model.CaselessStarts("y ")) {
                changedAxisScale = true;
                return ParseModelScale(dst, entity, model, "Y scale", ref entity.ScaleY);
            } else if (model.CaselessStarts("z ")) {
                changedAxisScale = true;
                return ParseModelScale(dst, entity, model, "Z scale", ref entity.ScaleZ);
            }
            return model;
        }
        
        static string ParseModelScale(Player dst, Entity entity, string model, string argName, ref float value) {
            string[] bits = model.SplitSpaces();
            CommandParser.GetReal(dst, bits[1], argName, ref value, 0, 3);
            return entity.Model;
        }

        public override void Help(Player p) {
            Player.Message(p, "%T/Model [name] [model] %H- Sets the model of that player.");
            Player.Message(p, "%T/Model bot [name] [model] %H- Sets the model of that bot.");
            Player.Message(p, "%HType %T/Help Model models %Hfor a list of models.");
            Player.Message(p, "%HType %T/Help Model scale %Hfor how to scale a model.");
        }
        
        public override void Help(Player p, string message) {
            if (message.CaselessEq("models")) {
                Player.Message(p, "%HAvailable models: %SChibi, Chicken, Creeper, Giant, Humanoid, Pig, Sheep, Spider, Skeleton, Zombie, Head, Sit, Corpse");
                Player.Message(p, "%HTo set a block model, use a block ID for the model name.");
                Player.Message(p, "%HType %T/Help Model scale %Hfor how to scale a model.");
            } else if (message.CaselessEq("scale")) {
                Player.Message(p, "%HFor a scaled model, put \"|[scale]\" after the model name.");
                Player.Message(p, "%H  e.g. pig|0.5, chibi|3");
                Player.Message(p, "%HUse X/Y/Z [scale] for [model] to set scale on one axis.");
                Player.Message(p, "%H  e.g. to set twice as tall, use 'Y 2' for [model]");
                Player.Message(p, "%H  Use a [scale] of 0 to reset");
            } else {
                Help(p);
            }
        }
    }
}
