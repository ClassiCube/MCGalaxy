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
    public class CmdEntityRot : EntityPropertyCmd {
        public override string name { get { return "EntityRot"; } }
        public override string shortcut { get { return "EntRot"; } }
        public override string type { get { return CommandTypes.Other; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public override CommandPerm[] ExtraPerms {
            get { return new[] { new CommandPerm(LevelPermission.Operator, "can change the rotation of others"),
                    new CommandPerm(LevelPermission.Operator, "can change the rotation of bots") }; }
        }

        public override void Use(Player p, string message, CommandData data) {
            if (message.IndexOf(' ') == -1) {
                message = "-own " + message;
                message = message.TrimEnd();
            }
            UseBotOrOnline(p, data, message, "rotation");
        }
        
        protected override void SetBotData(Player p, PlayerBot bot, string args) {
            if (!ParseArgs(p, args, bot)) return;
            BotsFile.Save(p.level);
        }
        
        protected override void SetOnlineData(Player p, Player who, string args) {
            if (!ParseArgs(p, args, who)) return;
            Server.rotations.Update(who.name, who.Rot.RotX + " " + who.Rot.RotZ);
            Server.rotations.Save();
        }
        
        static bool ParseArgs(Player p, string args, Entity entity) {
            if (args.Length == 0) {
                Entities.UpdateEntityProp(entity, EntityProp.RotX, 0);
                Entities.UpdateEntityProp(entity, EntityProp.RotZ, 0);
                return true;
            }
            
            string[] bits = args.SplitSpaces();
            if (bits.Length != 2) {
                p.Message("You need to provide an axis name and angle."); return false;
            }
            int angle = 0;
            if (!CommandParser.GetInt(p, bits[1], "Angle", ref angle, -360, 360)) return false;
            
            if (bits[0].CaselessEq("x")) {
                Entities.UpdateEntityProp(entity, EntityProp.RotX, angle);
            } else if (bits[0].CaselessEq("z")) {
                Entities.UpdateEntityProp(entity, EntityProp.RotZ, angle);
            } else {
                p.Message("Axis name must be X or Z."); return false;
            }
            return true;
        }

        public override void Help(Player p) {
            p.Message("&T/EntityRot [name] x/z [angle].");
            p.Message("&HSets X or Z axis rotation (in degrees) of that player.");
            p.Message("&T/EntityRot bot [name] x/z [angle]");
            p.Message("&HSets the X or Z axis rotation (in degrees) of that bot.");
        }
    }
}