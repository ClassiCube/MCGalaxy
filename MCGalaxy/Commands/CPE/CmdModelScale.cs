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
    public class CmdModelScale : EntityPropertyCmd {
        public override string name { get { return "ModelScale"; } }
        public override string type { get { return CommandTypes.Other; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public override CommandPerm[] ExtraPerms {
            get { return new[] { new CommandPerm(LevelPermission.Operator, "can change the model scale of others"),
                    new CommandPerm(LevelPermission.Operator, "can change the model scale of bots") }; }
        }

        public override void Use(Player p, string message, CommandData data) {
            if (message.SplitSpaces().Length < 3) { Help(p); return; }
            UseBotOrOnline(p, data, message, "model scale");
        }
        
        protected override void SetBotData(Player p, PlayerBot bot, string args) {
            if (!ParseArgs(p, bot, args)) return;
            bot.UpdateModel(bot.Model);
            
            BotsFile.Save(p.level);
        }
        
        protected override void SetOnlineData(Player p, Player who, string args) {
            if (!ParseArgs(p, who, args)) return;
            who.UpdateModel(who.Model);
            
            UpdateSavedScale(who);
            Server.modelScales.Save();
        }
        
        internal static void UpdateSavedScale(Player p) {
            if (p.ScaleX != 0 || p.ScaleY != 0 || p.ScaleZ != 0) {
                Server.modelScales.Update(p.name, p.ScaleX + " " + p.ScaleY + " " + p.ScaleZ);
            } else {
                Server.modelScales.Remove(p.name);
            }
            Server.modelScales.Save();
        }
        
        static bool ParseArgs(Player dst, Entity e, string args) {
            if (args.CaselessStarts("x ")) {
                return ParseScale(dst, e, args, "X scale", ref e.ScaleX);
            } else if (args.CaselessStarts("y ")) {
                return ParseScale(dst, e, args, "Y scale", ref e.ScaleY);
            } else if (args.CaselessStarts("z ")) {
                return ParseScale(dst, e, args, "Z scale", ref e.ScaleZ);
            }
            return false;
        }
        
        static bool ParseScale(Player dst, Entity e, string args, string axis, ref float value) {
            string[] bits = args.SplitSpaces();
            float max     = ModelInfo.MaxScale(e, e.Model);
            return CommandParser.GetReal(dst, bits[1], axis, ref value, 0, max);
        }

        public override void Help(Player p) {
            p.Message("%T/ModelScale [name] X/Y/Z [scale] %H- Sets scale for a player");
            p.Message("%T/ModelScale bot [name] X/Y/Z [scale] %H- Sets scale for a bot");
            p.Message("%HSets the scale of the given entity's model on one axis ");
            p.Message("%H  e.g. %T/ModelScale -own Y 2 %Hmakes yourself twice as tall");
            p.Message("%H  Use a [scale] of 0 to reset scale on that axis");
        }
    }
}
