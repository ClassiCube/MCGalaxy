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
    
    public class CmdSkin : Command {
        
        public override string name { get { return "skin"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public override CommandPerm[] AdditionalPerms {
            get { return new[] { new CommandPerm(LevelPermission.Operator, "+ can change the skin of others") }; }
        }

        public override void Use(Player p, string message) {
            if (CheckSuper(p, message, "player or bot name")) return;
            if (message == "") message = p.truename;
            
            Player who = p;
            PlayerBot pBot = null;
            bool isBot = message.CaselessStarts("bot ");
            string[] args = message.SplitSpaces(isBot ? 3 : 2);
            string skin = null;

            if (isBot && args.Length > 2) {
                pBot = PlayerBot.FindMatches(p, args[1]);
                if (pBot == null) return;
                skin = args[2];
            } else if (args.Length >= 2) {
                isBot = false;
                who = PlayerInfo.FindMatches(p, args[0]);
                if (who == null) return;
                skin = args.Length >= 2 ? args[1] : who.truename;
            } else {
                isBot = false;
                who = p;
                if (who == null) { SuperRequiresArgs(p, "player name"); return; }
                skin = message;
            }

            if (!ValidName(p, skin, "skin")) return;
            if (p != null && who != null && who.Rank > p.Rank) {
                MessageTooHighRank(p, "change the skin of", true); return;
            }
            if ((isBot || who != p) && !CheckExtraPerm(p)) { MessageNeedExtra(p, "can change the skin of others."); return; }
            if (skin[0] == '+') 
                skin = "http://skins.minecraft.net/MinecraftSkins/" + skin.Substring(1) + ".png";
            
            if (isBot) {
                pBot.SkinName = skin;
                pBot.GlobalDespawn();
                pBot.GlobalSpawn();
                Player.GlobalMessage("Bot " + pBot.ColoredName + "'s %Sskin was changed to &c" + skin);
                BotsFile.UpdateBot(pBot);
            } else {
                who.skinName = skin;
                Entities.GlobalDespawn(who, true);
                Entities.GlobalSpawn(who, true);
                
                if (p != who)
                    Player.GlobalMessage(who, who.ColoredName + "'s %Sskin was changed to &c" + skin);
                else
                    Player.Message(who, "Changed your own skin to &c" + skin);    
                
                if (skin == who.truename)
                    Server.skins.Remove(who.name);
                else
                    Server.skins.AddOrReplace(who.name, skin);
                Server.skins.Save();
            }
        }

        public override void Help(Player p) {
            Player.Message(p, "%T/skin [name] [skin] %H- Sets the skin of that player.");
            Player.Message(p, "%T/skin bot [name] [skin] %H- Sets the skin of that bot.");     
            Player.Message(p, "%H e.g the player \"Test\" by default has the skin \"Test\".");
            Player.Message(p, "%H If you put a + before [skin], players will retrieve [skin] " +
                           "from minecraft.net instead of classicube.net.");
        }
    }
}
