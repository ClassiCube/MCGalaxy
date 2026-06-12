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
    public class CmdModelScale : EntityPropertyCmd 
    {
        public override string name { get { return "ModelScale"; } }
        public override string type { get { return CommandTypes.Other; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public override CommandPerm[] ExtraPerms {
            get { return new[] { new CommandPerm(LevelPermission.Operator, "can change the model scale of others"),
                    new CommandPerm(LevelPermission.Operator, "can change the model scale of bots") }; }
        }
        public override CommandAlias[] Aliases {
            get { return new[] {
                new CommandAlias("XModelScale"),
                new CommandAlias("OModelScale", OTHER_FLAG)
            }; }
        }

        public override void Use(Player p, string message, CommandData data) {
            UseBotOrOnline(p, data, message, "model scale");
        }
        
        protected override void SetBotData(Player p, PlayerBot bot, string args) {
            string axis, res;
            if (!ParseArgs(p, bot, args, out axis, out res)) return;
            bot.UpdateModel(bot.Model);
            
            p.Message("You changed the {0} scale of bot {1} &Sto {2}", axis, bot.ColoredName, res);
            BotsFile.Save(p.level);
        }
        
        protected override void SetOnlineData(Player p, Player who, string args) {
            string axis, res;
            if (!ParseArgs(p, who, args, out axis, out res)) return;
            who.UpdateModel(who.Model);

            if (p != who) {
                PlayerOperations.MessageAction(p, who.name, who, "λACTOR &Schanged λTARGET " + axis + " scale to &c" + res);
            } else {
                who.Message("Changed your own {0} scale to {1}", axis, res);
            }
            
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
        
        bool ParseArgs(Player dst, Entity e, string args, out string axis, out string res) {
            string[] bits = args.SplitSpaces(2);
            res = "";
            if (bits.Length < 2) { Help(dst); axis = null; return false; }
            
            axis = bits[0].ToUpper();
            string scale = bits[1];
            
            if (axis == "X") return ParseScale(dst, e, axis, scale, ref e.ScaleX, out res);
            if (axis == "Y") return ParseScale(dst, e, axis, scale, ref e.ScaleY, out res);
            if (axis == "Z") return ParseScale(dst, e, axis, scale, ref e.ScaleZ, out res);
            res = "";
            return false;
        }
        
        static bool ParseScale(Player dst, Entity e, string axis, string scale, ref float value, out string res) {
            float max = ModelInfo.MaxScale(e, e.Model);
            bool gotEm = CommandParser.GetReal(dst, scale, axis + " scale", ref value, 0, max);
            if (value == 1) {
                dst.Message("&WTIP:");
                dst.Message("&H  To reset scale, use &T0 &Hfor the value.");
            }
            res = value == 0 ? "default" : value.ToString();
            return gotEm;
        }

        public override void Help(Player p) {
            p.Message("&T/ModelScale X/Y/Z [scale] &H- Sets your own scale");
            p.Message("&T/OModelScale [name] X/Y/Z [scale] &H- Sets scale for another player");
            p.Message("&T/ModelScale bot [name] X/Y/Z [scale] &H- Sets scale for a bot");
            p.Message("&HSets the scale of the given entity's model on one axis ");
            p.Message("&H  e.g. &T/ModelScale Y 2 &Hmakes yourself twice as tall");
            p.Message("&H  Use a [scale] of 0 to reset scale on that axis");
        }
    }
}
