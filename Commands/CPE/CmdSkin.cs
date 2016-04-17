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

namespace MCGalaxy.Commands {
    
    public class CmdSkin : Command {
        
        public override string name { get { return "skin"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        static char[] trimChars = { ' ' };

        public override void Use(Player p, string message) {
            if (message == "") {
                if (p == null) {
                    Player.SendMessage(p, "Console must provide a player or bot name."); return;
                }
                message = p.name;
            }
            
            Player who = p;
            PlayerBot pBot = null;
            bool isBot = message.CaselessStarts("bot ");
            string[] args = message.Split(trimChars, isBot ? 3 : 2);
            string skin = null;

            if (isBot && args.Length > 2) {
                pBot = PlayerBot.Find(args[1]);
                if (pBot == null) { Player.SendMessage(p, "There is no bot with that name."); return; }
                skin = args[2];
            } else if (args.Length > 1) {
                isBot = false;
                who = PlayerInfo.FindOrShowMatches(p, args[0]);
                if (who == null) return;
                skin = args.Length >= 2 ? args[1] : who.truename;
            } else {
                isBot = false;
                who = p;
                if (who == null) { Player.SendMessage(p, "Console must provide a player name."); return; }
                skin = message;
            }

            if (!Player.ValidName(skin)) {
                Player.SendMessage(p, "\"" + skin + "\" is not a valid skin name."); return;
            }
            if (isBot) {
                pBot.skinName = skin;
                pBot.GlobalDespawn();
                pBot.GlobalSpawn();
                Player.GlobalMessage("Bot " + pBot.name + "'s %Sskin was changed to &c" + skin);
            } else {
                who.skinName = skin;
                Player.GlobalDespawn(who, true);
                Player.GlobalSpawn(who, true);
                Player.GlobalMessage(who.color + who.DisplayName + "'s %Sskin was changed to &c" + skin);
            }
        }

        public override void Help(Player p) {
            Player.SendMessage(p, "/skin [name] [skin] - Sets the skin of that player.");
            Player.SendMessage(p, "/skin bot [name] [model] - Sets the model of that bot.");
            Player.SendMessage(p, "For example, the player \"Test\" by default has the skin \"Test\".");
        }
    }
}
