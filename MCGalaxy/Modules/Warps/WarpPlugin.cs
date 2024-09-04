/*
    Copyright 2015-2024 MCGalaxy
        
    Dual-licensed under the Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    https://opensource.org/license/ecl-2-0/
    https://www.gnu.org/licenses/gpl-3.0.html
    
    Unless required by applicable law or agreed to in writing,
    software distributed under the Licenses are distributed on an "AS IS"
    BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
    or implied. See the Licenses for the specific language governing
    permissions and limitations under the Licenses.
*/
using System;
using MCGalaxy.Events.ServerEvents;

namespace MCGalaxy.Modules.Warps 
{
    public sealed class WarpsPlugin : Plugin 
    {
        public override string name { get { return "Warps"; } }

        Command cmdWarps     = new CmdWarp();
        Command cmdWaypoints = new CmdWaypoint();

        public override void Load(bool startup) {
            Server.EnsureDirectoryExists(Paths.WAYPOINTS_DIR);
            OnConfigUpdatedEvent.Register(OnConfigUpdated, Priority.Low);

            Command.Register(cmdWarps);
            Command.Register(cmdWaypoints);
        }

        public override void Unload(bool shutdown) {
            OnConfigUpdatedEvent.Unregister(OnConfigUpdated);
            Command.Unregister(cmdWarps, cmdWaypoints);
        }
        
        static void OnConfigUpdated() { 
            if (WarpList.Global == null) return;
            WarpList.Global.Load();
        }
    }
}
