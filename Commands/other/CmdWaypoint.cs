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
    public sealed class CmdWaypoint : Command
    {
        public override string name { get { return "waypoint"; } }
        public override string shortcut { get { return "wp"; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Builder; } }
        public CmdWaypoint() { }

        public override void Use(Player p, string message)
        {
            if (p == null) { Player.SendMessage(p, "This command can only be used in-game"); return; }
            string[] command = message.ToLower().Split(' ');
            string par0 = String.Empty;
            string par1 = String.Empty;
            try
            {
                par0 = command[0];
                par1 = command[1];
            }
            catch { }
            if (par0.ToLower() == "create" || par0.ToLower() == "new" || par0.ToLower() == "add")
            {
                if (!WaypointList.Exists(par1, p))
                {
                    WaypointList.Create(par1, p);
                    Player.SendMessage(p, "Created waypoint");
                    return;
                }
                else { Player.SendMessage(p, "That waypoint already exists"); return; }
            }
            else if (par0.ToLower() == "goto")
            {
                if (WaypointList.Exists(par1, p))
                {
                    WaypointList.Goto(par1, p);
                    return;
                }
                else { Player.SendMessage(p, "That waypoint doesn't exist"); return; }
            }
            else if (par0.ToLower() == "replace" || par0.ToLower() == "update" || par0.ToLower() == "edit")
            {
                if (WaypointList.Exists(par1, p))
                {
                    WaypointList.Update(par1, p);
                    Player.SendMessage(p, "Updated waypoint");
                    return;
                }
                else { Player.SendMessage(p, "That waypoint doesn't exist"); return; }
            }
            else if (par0.ToLower() == "delete" || par0.ToLower() == "remove")
            {
                if (WaypointList.Exists(par1, p))
                {
                    WaypointList.Remove(par1, p);
                    Player.SendMessage(p, "Deleted waypoint");
                    return;
                }
                else { Player.SendMessage(p, "That waypoint doesn't exist"); return; }
            }
            else if (par0.ToLower() == "list")
            {
                Player.SendMessage(p, "Waypoints:");
                foreach (Waypoint wp in p.Waypoints)
                {
                    if (Level.Find(wp.lvlname) != null)
                    {
                        Player.SendMessage(p, wp.name + ":" + wp.lvlname);
                    }
                }
                return;
            }
            else
            {
                if (WaypointList.Exists(par0, p))
                {
                    WaypointList.Goto(par0, p);
                    Player.SendMessage(p, "Sent you to waypoint");
                    return;
                }
                else { Player.SendMessage(p, "That waypoint or command doesn't exist"); return; }
            }
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/waypoint [create] [name] - Create a new waypoint");
            Player.SendMessage(p, "/waypoint [update] [name] - Update a waypoint");
            Player.SendMessage(p, "/waypoint [remove] [name] - Remove a waypoint");
            Player.SendMessage(p, "/waypoint [list] - Shows a list of waypoints");
            Player.SendMessage(p, "/waypoint [goto] [name] - Goto a waypoint");
            Player.SendMessage(p, "/waypoint [name] - Goto a waypoint");
        }
    }
}
