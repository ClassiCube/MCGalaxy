/*
	Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
	
	Dual-licensed under the	Educational Community License, Version 2.0 and
	the GNU General Public License, Version 3 (the "Licenses"); you may
	not use this file except in compliance with the Licenses. You may
	obtain a copy of the Licenses at
	
	http://www.osedu.org/licenses/ECL-2.0
	http://www.gnu.org/licenses/gpl-3.0.html
	
	Unless required by applicable law or agreed to in writing,
	software distributed under the Licenses are distributed on an "AS IS"
	BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
	or implied. See the Licenses for the specific language governing
	permissions and limitations under the Licenses.
*/
namespace MCGalaxy.Commands
{
    public sealed class CmdTp : Command
    {
        public override string name { get { return "tp"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return "other"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Builder; } }
        public CmdTp() { }

        public override void Use(Player p, string message)
        {
            if (message == "")
            {
                Command.all.Find("spawn");
                return;
            }
            int number = message.Split(' ').Length;
            if (number > 2) { Help(p); return; }
            if (number == 2)
            {
                if (!p.group.CanExecute(Command.all.Find("P2P")))
                { 
                   Player.SendMessage(p, "You cannot teleport others!"); 
                   return; 
                }
                Command.all.Find("P2P").Use(p, message);
            }
            if (number == 1)
            {
                Player who = Player.Find(message);
                if (who == null || (who.hidden && p.group.Permission < LevelPermission.Admin)) { Player.SendMessage(p, "There is no player \"" + message + "\"!"); return; }
                if (p.level != who.level)
                {
                    if (who.level.name.Contains("cMuseum"))
                    {
                        Player.SendMessage(p, "Player \"" + message + "\" is in a museum!");
                        return;
                    }
                    else
                    {
                        if (Server.higherranktp == false)
                        {
                            if (p.group.Permission < who.group.Permission)
                            {
                                Player.SendMessage(p, "You cannot teleport to a player of higher rank!");
                                return;
                            }
                        }
                        p.beforeTeleportMap = p.level.name;
                        p.beforeTeleportPos = p.pos;
                        Command.all.Find("goto").Use(p, who.level.name);
                        if (who.Loading)
                        {
                            Player.SendMessage(p, "Waiting for " + who.color + who.name + Server.DefaultColor + " to spawn...");
                            while (who.Loading) { }
                        }
                        while (p.Loading) { }  //Wait for player to spawn in new map
                        unchecked { p.SendPos((byte)-1, who.pos[0], who.pos[1], who.pos[2], who.rot[0], 0); }
                    }
                    return;
                }
                if (p.level == who.level)
                {
                    if (Server.higherranktp == false)
                    {
                        if (p.group.Permission < who.group.Permission)
                        {
                            Player.SendMessage(p, "You cannot teleport to a player of higher rank!");
                            return;
                        }
                    }
                    p.beforeTeleportMap = p.level.name;
                    p.beforeTeleportPos = p.pos;
                    if (who.Loading)
                    {
                        Player.SendMessage(p, "Waiting for " + who.color + who.name + Server.DefaultColor + " to spawn...");
                        while (who.Loading) { }
                    }
                    while (p.Loading) { }  //Wait for player to spawn in new map
                    unchecked { p.SendPos((byte)-1, who.pos[0], who.pos[1], who.pos[2], who.rot[0], 0); }
                }
            }
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/tp <player1> [player2] - Teleports yourself to a player.");
            Player.SendMessage(p, "[player2] is optional but if present will act like /p2p.");
            Player.SendMessage(p, "If <player1> is blank, /spawn is used.");
        }
    }
}