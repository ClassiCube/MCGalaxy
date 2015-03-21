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
namespace MCGalaxy.Commands
{
    public sealed class CmdBotRemove : Command
    {
        public override string name { get { return "botremove"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return "mod"; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        public string[,] botlist;
        public CmdBotRemove() { }

        public override void Use(Player p, string message)
        {
            if (message == "") { Help(p); return; }
            if (p == null)
            {
                Player.SendMessage(p, "This command can only be used in-game!");
                return;
            }
                try
                {
                    if (message.ToLower() == "all")
                    {
                        for (int i = 0; i < PlayerBot.playerbots.Count; i++)
                        {
                            if (PlayerBot.playerbots[i].level == p.level)
                            {
                                //   PlayerBot.playerbots.Remove(PlayerBot.playerbots[i]);
                                PlayerBot Pb = PlayerBot.playerbots[i];
                                Pb.removeBot();
                                i--;
                            }
                        }
                    }
                    else
                    {
                        PlayerBot who = PlayerBot.Find(message);
                        if (who == null) { Player.SendMessage(p, "There is no bot " + who + "!"); return; }
                        if (p.level != who.level) { Player.SendMessage(p, who.name + " is in a different level."); return; }
                        who.removeBot();
                        Player.SendMessage(p, "Removed bot.");
                    }
                }
                catch (Exception e) { Server.ErrorLog(e); Player.SendMessage(p, "Error caught"); }
            }
        
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/botremove <name> - Remove a bot on the same level as you");
            //   Player.SendMessage(p, "If All is used, all bots on the current level are removed");
        }
    }
}