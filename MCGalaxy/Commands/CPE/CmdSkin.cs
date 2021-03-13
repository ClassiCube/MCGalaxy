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
using MCGalaxy.Network;

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
            skin = ParseSkin(p, skin, bot.name);
            if (skin == null) return;
            
            bot.SkinName = skin;
            p.Message("You changed the skin of bot " + bot.ColoredName + " &Sto &c" + skin);
            
            bot.GlobalDespawn();
            bot.GlobalSpawn();
            BotsFile.Save(p.level);
        }
        
        protected override void SetPlayerData(Player p, string target, string skin) {
            string rawName = target.RemoveLastPlus();
            skin = ParseSkin(p, skin, rawName);    
            if (skin == null) return;
            
            Player who = PlayerInfo.FindExact(target);
            if (p == who) {
                p.Message("Changed your own skin to &c" + skin);
            } else {
                MessageFrom(target, who, "had their skin changed to &c" + skin);
            }
            
            if (who != null) who.SkinName = skin;
            if (who != null) Entities.GlobalRespawn(who);
            
            if (skin == rawName) {
                Server.skins.Remove(target);
            } else {
                Server.skins.Update(target, skin);
            }
            Server.skins.Save();
        }
        
        static string ParseSkin(Player p, string skin, string defSkin) {
            if (skin.Length == 0) skin = defSkin;
            if (skin[0] == '+')
                skin = "https://minotar.net/skin/" + skin.Substring(1) + ".png";
            
            if (skin.CaselessStarts("http://") || skin.CaselessStarts("https")) {
                HttpUtil.FilterURL(ref skin);
            }
            
            if (skin.Length > NetUtils.StringSize) {
                p.Message("&WThe skin must be " + NetUtils.StringSize + " characters or less."); 
                return null;
            }
            return skin;
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
