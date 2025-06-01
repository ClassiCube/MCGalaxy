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
using MCGalaxy.Network;

namespace MCGalaxy.Commands.CPE
{
    public class CmdSkin : EntityPropertyCmd 
    {
        public override string name { get { return "Skin"; } }
        public override string type { get { return CommandTypes.Other; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public override CommandPerm[] ExtraPerms {
            get { return new[] { new CommandPerm(LevelPermission.Operator, "can change the skin of others"),
                    new CommandPerm(LevelPermission.Operator, "can change the skin of bots") }; }
        }

        public override void Use(Player p, string message, CommandData data) {
            if (message.IndexOf(' ') == -1) {
                message = "-own " + message;
                message = message.TrimEnd();
            }
            UseBotOrPlayer(p, data, message, "skin");
        }

        protected override void SetBotData(Player p, PlayerBot bot, string skin) {
            skin = HttpUtil.FilterSkin(p, skin, bot.name);
            if (skin == null) return;
            
            bot.SkinName = skin;
            p.Message("You changed the skin of bot " + bot.ColoredName + " &Sto &c" + skin);
            
            bot.GlobalDespawn();
            bot.GlobalSpawn();
            BotsFile.Save(p.level);
        }
        
        protected override void SetPlayerData(Player p, string target, string skin) {
            PlayerOperations.SetSkin(p, target, skin);
        }

        public override void Help(Player p) {
            p.Message("&T/Skin [name] [skin] &H- Sets the skin of that player.");
            p.Message("&T/Skin bot [name] [skin] &H- Sets the skin of that bot.");
            p.Message("&H[skin] can be:");
            p.Message("&H - a ClassiCube player's name (e.g Hetal)");
            p.Message("&H - a Minecraft player's name, if you put a + (e.g +Hypixel)");
            p.Message("&H - a direct url to a skin");
            p.Message("&HDirect url skins also apply to non human models (e.g pig)");
        }
    }
}
