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
            skin = GetSkin(skin, bot.name);
            bot.SkinName = skin;
            p.Message("You changed the skin of bot " + bot.ColoredName + " %Sto &c" + skin);
            
            bot.GlobalDespawn();
            bot.GlobalSpawn();
            BotsFile.Save(p.level);
        }
        
        protected override void SetPlayerData(Player p, Player who, string skin) {
            skin = GetSkin(skin, who.truename);
            who.SkinName = skin;
            Entities.GlobalRespawn(who);
            
            if (p != who) {
                Chat.MessageFrom(who,"λNICK %Shad their skin changed to &c" + skin);
            } else {
                who.Message("Changed your own skin to &c" + skin);
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
            
            Utils.FilterURL(ref skin);
            return skin;
        }

        public override void Help(Player p) {
            p.Message("%T/Skin [name] [skin] %H- Sets the skin of that player.");
            p.Message("%T/Skin bot [name] [skin] %H- Sets the skin of that bot.");
            p.Message("%H[skin] can be:");
            p.Message("%H - a ClassiCube player's name (e.g Hetal)");
            p.Message("%H - a Minecraft player's name, if you put a + (e.g +Hypixel)");
            p.Message("%H - a direct url to a skin");
            p.Message("%HDirect url skins also apply to non human models (e.g pig)");
        }
    }
}
