/*
	Copyright 2011 MCGalaxy
		
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
    public sealed class CmdJoker : Command
    {
        public override string name { get { return "joker"; } }
        public override string shortcut { get { return ""; } }
       public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public static string keywords { get { return ""; } }
        public CmdJoker() { }

        public override void Use(Player p, string message)
        {
            if (message == "") { Help(p); return; }
            bool stealth = false;
            if (message[0] == '#')
            {
                message = message.Remove(0, 1).Trim();
                stealth = true;
                Server.s.Log("Stealth joker attempted");
            }

            Player who = PlayerInfo.Find(message);
            if (who == null)
            {
                Player.SendMessage(p, "Could not find player.");
                return;
            }
            if (p != null && who.group.Permission > p.group.Permission) { Player.SendMessage(p, "Cannot joker someone of equal or greater rank."); return; }

            if (!who.joker)
            {
                who.joker = true;
                if (stealth) { Chat.GlobalMessageOps(who.color + who.DisplayName + Server.DefaultColor + " is now STEALTH joker'd. "); return; }
                Player.SendChatFrom(who, who.color + who.DisplayName + Server.DefaultColor + " is now a &aJ&bo&ck&5e&9r" + Server.DefaultColor + ".", false);
            }
            else
            {
                who.joker = false;
                if (stealth) { Chat.GlobalMessageOps(who.color + who.DisplayName + Server.DefaultColor + " is now STEALTH Unjoker'd. "); return; }
                Player.SendChatFrom(who, who.color + who.DisplayName + Server.DefaultColor + " is no longer a &aJ&bo&ck&5e&9r" + Server.DefaultColor + ".", false);
            }
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/joker <name> - Causes a player to become a joker!");
            Player.SendMessage(p, "/joker # <name> - Makes the player a joker silently");
            return;
        }
    }
}
