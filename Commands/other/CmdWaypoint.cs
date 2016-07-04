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
            if (p == null) { MessageInGameOnly(p); return; }
            string[] command = message.ToLower().Split(' ');
            string cmd = String.Empty;
            string par1 = String.Empty;
            try
            {
            	cmd = command[0].ToLower();
                par1 = command[1];
            }
            catch { }
            if (cmd == "create" || cmd == "new" || cmd == "add")
            {
                if (!p.Waypoints.Exists(par1))
                {
                    p.Waypoints.Create(par1, p);
                    Player.Message(p, "Created waypoint");
                    return;
                }
                else { Player.Message(p, "That waypoint already exists"); return; }
            }
            else if (cmd == "goto")
            {
                if (p.Waypoints.Exists(par1))
                {
                    p.Waypoints.Goto(par1, p);
                    return;
                }
                else { Player.Message(p, "That waypoint doesn't exist"); return; }
            }
            else if (cmd == "replace" || cmd == "update" || cmd == "edit")
            {
                if (p.Waypoints.Exists(par1))
                {
                    p.Waypoints.Update(par1, p);
                    Player.Message(p, "Updated waypoint");
                    return;
                }
                else { Player.Message(p, "That waypoint doesn't exist"); return; }
            }
            else if (cmd == "delete" || cmd == "remove")
            {
                if (p.Waypoints.Exists(par1))
                {
                    p.Waypoints.Remove(par1, p);
                    Player.Message(p, "Deleted waypoint");
                    return;
                }
                else { Player.Message(p, "That waypoint doesn't exist"); return; }
            }
            else if (cmd == "list")
            {
                Player.Message(p, "Waypoints:");
                foreach (Warp wp in p.Waypoints.Items)
                {
                    if (LevelInfo.FindExact(wp.lvlname) != null)
                    {
                        Player.Message(p, wp.name + ":" + wp.lvlname);
                    }
                }
                return;
            }
            else
            {
                if (p.Waypoints.Exists(cmd))
                {
                    p.Waypoints.Goto(cmd, p);
                    return;
                }
                else { Player.Message(p, "That waypoint or command doesn't exist"); return; }
            }
        }
        public override void Help(Player p)
        {
            Player.Message(p, "/waypoint [create] [name] - Create a new waypoint");
            Player.Message(p, "/waypoint [update] [name] - Update a waypoint");
            Player.Message(p, "/waypoint [remove] [name] - Remove a waypoint");
            Player.Message(p, "/waypoint [list] - Shows a list of waypoints");
            Player.Message(p, "/waypoint [goto] [name] - Goto a waypoint");
            Player.Message(p, "/waypoint [name] - Goto a waypoint");
        }
    }
}
