/*
	Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
	
	Dual-licensed under the	Educational Community License, Version 2.0 and
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
using System.Threading;
namespace MCGalaxy.Commands
{
    public sealed class CmdSummon : Command
    {
        public override string name { get { return "summon"; } }
        public override string shortcut { get { return "s"; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("fetch") }; }
        }
        public CmdSummon() { }

        public override void Use(Player p, string message)
        {
            if (message == "") { Help(p); return; }
            if (p == null) { MessageInGameOnly(p); return; }
            if (message.ToLower() == "all")
            {
                Player[] players = PlayerInfo.Online.Items;
                foreach (Player pl in players) {
                    if (pl.level == p.level && pl != p && p.group.Permission > pl.group.Permission) {
                        pl.SendOwnPos(p.pos[0], p.pos[1], p.pos[2], p.rot[0], 0);
                        pl.SendMessage("You were summoned by " + p.ColoredName + "%S.");
                    }
                }
                Player.GlobalMessage(p.ColoredName + " %Ssummoned everyone!");
                return;
            }

            Player who = PlayerInfo.FindOrShowMatches(p, message);
            if (who == null) return;
            if (p.group.Permission < who.group.Permission) {
                MessageTooHighRank(p, "summon", true); return;
            }
            
            if (p.level != who.level)
            {
                Player.Message(p, who.ColoredName + " %Sis in a different Level. Forcefetching has started!");
                Command.all.Find("goto").Use(who, p.level.name);
                Thread.Sleep(1000);
                // Sleep for a bit while they load
            }

            who.SendOwnPos(p.pos[0], p.pos[1], p.pos[2], p.rot[0], 0);
            who.SendMessage("You were summoned by " + p.ColoredName + "%S.");
        }
        public override void Help(Player p)
        {
            Player.Message(p, "/summon <player> - Summons a player to your position.");
            Player.Message(p, "/summon all - Summons all players in the map");
        }
    }
}
