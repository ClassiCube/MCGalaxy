/*
	Copyright 2011 MCForge
		
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
using System.Collections.Generic;
using System.Text;
namespace MCGalaxy.Commands
{
    public sealed class CmdVIP : Command
    {
        public override string name { get { return "vip"; } }
        public override string shortcut { get { return ""; } }
       public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        public CmdVIP() { }

        public override void Use(Player p, string message)
        {
            if (message == "") { Help(p); return; }
            string[] split = message.Split(' ');
            if (split[0] == "add")
            {
                if (split.Length < 2) { Help(p); return; }
                Player pl = PlayerInfo.Find(split[1]);
                if (pl != null) split[1] = pl.name;
                if (VIP.Find(split[1])) Player.Message(p, (pl == null ? "" : pl.color) + split[1] + " is already a VIP!");
                else
                {
                    VIP.Add(split[1]);
                    Player.Message(p, (pl == null ? "" : pl.color) + split[1] + " is now a VIP.");
                    if (pl != null) Player.Message(pl, "You are now a VIP!");
                }
            }
            else if (split[0] == "remove")
            {
                if (split.Length < 2) { Help(p); return; }
                Player pl = PlayerInfo.Find(split[1]);
                if (pl != null) split[1] = pl.name;
                if (!VIP.Find(split[1])) Player.Message(p, (pl == null ? "" : pl.color) + split[1] + " is not a VIP!");
                else
                {
                    VIP.Remove(split[1]);
                    Player.Message(p, (pl == null ? "" : pl.color) + split[1] + " is no longer a VIP.");
                    if (pl != null) Player.Message(pl, "You are no longer a VIP!");
                }
            }
            else if (split[0] == "list")
            {
                List<string> list = VIP.GetAll();
                if (list.Count < 1) Player.Message(p, "There are no VIPs.");
                else {
                    StringBuilder sb = new StringBuilder();
                    foreach (string name in list)
                        sb.Append(name).Append(", ");
                    
                    string count = list.Count > 1 ? "is 1" : "are " + list.Count;
                    Player.Message(p, "There " + count + " VIPs:");
                    Player.Message(p, sb.Remove(sb.Length - 2, 2).ToString());
                }
            }
            else Help(p);
        }
        public override void Help(Player p)
        {
            Player.Message(p, "VIPs are players who can join regardless of the player limit.");
            Player.Message(p, "/vip add <name> - Add a VIP.");
            Player.Message(p, "/vip remove <name> - Remove a VIP.");
            Player.Message(p, "/vip list - List all VIPs.");
        }
    }
}
