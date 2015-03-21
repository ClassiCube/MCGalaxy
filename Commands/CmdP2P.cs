/*
 * Written by Jack1312
 * 
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
namespace MCGalaxy.Commands
{
    public sealed class CmdP2P : Command
    {
        public override string name { get { return "p2p"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return "mod"; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdP2P() { }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/p2p [Player1] [Player2] - Teleports player 1 to player 2.");
        }

        public override void Use(Player p, string message)
        {
            if (message == "") { Help(p); return; }
            int number = message.Split(' ').Length;
            if (number > 2) { Help(p); return; }
            if (number == 2)
            {
                int pos = message.IndexOf(' ');
                string t = message.Substring(0, pos).ToLower();
                string s = message.Substring(pos + 1).ToLower();
                Player who = Player.Find(t);
                Player who2 = Player.Find(s);
                if (who == null)
                {
                    if (who2 == null)
                    {
                        Player.SendMessage(p, "Neither of the players you specified, can be found or exist!");
                        return;
                    }
                    Player.SendMessage(p, "Player 1 is not online or does not exist!");
                    return;
                }
                if (who2 == null)
                {
                    Player.SendMessage(p, "Player 2 is not online or does not exist!");
                    return;
                }
                if (who == p)
                {
                    if (who2 == p)
                    {
                        Player.SendMessage(p, "Why are you trying to teleport yourself to yourself? =S");
                        return;
                    }
                    Player.SendMessage(p, "Why not, just use /tp " + who2.name + "!");
                }
                if (who2 == p)
                {
                    Player.SendMessage(p, "Why not, just use /summon " + who.name + "!");
                }
                if (p.group.Permission < who.group.Permission)
                {
                    Player.SendMessage(p, "You cannot force a player of higher rank to tp to another player!");
                    return;
                }
                if (s == "")
                {
                    Player.SendMessage(p, "You did not specify player 2!");
                    return;
                }
                Command.all.Find("tp").Use(who, who2.name);
                Player.SendMessage(p, who.name + " has been successfully teleported to " + who2.name + "!");
            }

            if (number == 1)
            {
                Player.SendMessage(p, "You did not specify player 2!");
                return;
            }
        }
    }
}