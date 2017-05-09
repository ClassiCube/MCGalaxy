/*
    Copyright 2011 MCForge
        
    Dual-licensed under the    Educational Community License, Version 2.0 and
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
namespace MCGalaxy.Commands.Misc {
    public sealed class CmdInvincible : Command {
        public override string name { get { return "invincible"; } }
        public override string shortcut { get { return "inv"; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("godmode") }; }
        }
        public CmdInvincible() { }

        public override void Use(Player p, string message)
        {
            Player who = message == "" ? p : PlayerInfo.FindMatches(p, message);
            if (who == null) return;

            if (p != null && who.Rank > p.Rank) {
                MessageTooHighRank(p, "toggle invinciblity", true); return;
            }
            who.invincible = !who.invincible;
            ShowPlayerMessage(p, who);
        }
        
        static void ShowPlayerMessage(Player p, Player who) {
            string msg = who.invincible ? "now invincible" : "no longer invincible";
            if (p != null && who == p)
                Player.Message(p, "You are {0}", msg);
            else
                Player.Message(p, "{0} %Sis {1}.", who.ColoredName, msg);
            
            string globalMsg = who.invincible ? Server.cheapMessageGiven : "has stopped being invincible";
            if (Server.cheapMessage && !who.hidden)
                Chat.MessageGlobal(who, who.ColoredName + " %S" + globalMsg, false);
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/invincible <name>");
            Player.Message(p, "%HTurns invincible mode on/off.");
            Player.Message(p, "%HIf <name> is given, that player's invincibility is toggled");
        }
    }
}
