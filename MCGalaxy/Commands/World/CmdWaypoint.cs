/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
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

namespace MCGalaxy.Commands.Misc {
    public sealed class CmdWaypoint : CmdWarp {
        public override string name { get { return "Waypoint"; } }
        public override string shortcut { get { return "wp"; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Builder; } }
        public override CommandPerm[] ExtraPerms { get { return null; } }
                
        public override void Use(Player p, string message, CommandData data) {
            UseCore(p, message, data, p.Waypoints, "Waypoint");
        }

        public override void Help(Player p) {
            p.Message("&HWaypoints are warps only usable by you.");
            p.Message("&T/Waypoint create [name] &H- Create a new waypoint");
            p.Message("&T/Waypoint update [name] &H- Update a waypoint");
            p.Message("&T/Waypoint remove [name] &H- Remove a waypoint");
            p.Message("&T/Waypoint list &H- Shows a list of waypoints");
            p.Message("&T/Waypoint [name] &H- Goto a waypoint");
        }
    }
}
