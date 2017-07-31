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
    public class CmdSkin : EntityPropertyCmd {
        public override string name { get { return "Skin"; } }
        public override string type { get { return CommandTypes.Other; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public override CommandPerm[] ExtraPerms {
            get { return new[] { new CommandPerm(LevelPermission.Operator, "+ can change the skin of others"),
                    new CommandPerm(LevelPermission.Operator, "+ can change the skin of bots") }; }
        }

        public override void Use(Player p, string message) {
            if (message.IndexOf(' ') == -1) {
                message = "-own " + message;
                message = message.TrimEnd();
            }
            UseBotOrPlayer(p, message, "skin");
        }

        protected override void SetBotData(Player p, PlayerBot bot, string skin) {
            skin = GetSkin(skin, bot.name);
            bot.SkinName = skin;
            Player.Message(p, "You changed the skin of bot " + bot.ColoredName + " %Sto &c" + skin);
            
            bot.GlobalDespawn();
            bot.GlobalSpawn();
            BotsFile.UpdateBot(bot);
        }
        
        protected override void SetPlayerData(Player p, Player who, string skin) {
            skin = GetSkin(skin, who.truename);
            who.SkinName = skin;
            Entities.GlobalRespawn(who);
            
            if (p != who) {
                Player.GlobalMessage(who, who.ColoredName + "'s %Sskin was changed to &c" + skin);
            } else {
                Player.Message(who, "Changed your own skin to &c" + skin);
            }
            
            if (skin == who.truename) {
                Server.skins.Remove(who.name);
            } else {
                Server.skins.AddOrReplace(who.name, skin);
            }
            Server.skins.Save();
        }
        
        static string GetSkin(string skin, string defSkin) {
            if (skin.Length == 0) skin = defSkin;
            if (skin[0] == '+')
                skin = "http://skins.minecraft.net/MinecraftSkins/" + skin.Substring(1) + ".png";
            return skin;
        }

        public override void Help(Player p) {
            Player.Message(p, "%T/Skin [name] [skin] %H- Sets the skin of that player.");
            Player.Message(p, "%T/Skin bot [name] [skin] %H- Sets the skin of that bot.");
            Player.Message(p, "%H e.g the player \"Test\" by default has the skin \"Test\".");
            Player.Message(p, "%H If you put a + before [skin], players will retrieve [skin] " +
                           "from minecraft.net instead of classicube.net.");
        }
    }
}
