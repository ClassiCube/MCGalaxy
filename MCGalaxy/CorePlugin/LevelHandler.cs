/*
    Copyright 2015 MCGalaxy
        
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

namespace MCGalaxy.Core {
    internal static class LevelHandler {
        
        internal static void HandleOnJoinedLevel(Player p, Level prevLevel, Level level) {
            p.AFKCooldown = DateTime.UtcNow.AddSeconds(2);
            p.prevMsg = "";
            p.showMBs = false;
            p.showPortals = false;
            
            if (p.HasCpeExt(CpeExt.EnvWeatherType))
                p.Send(Packet.EnvWeatherType((byte)level.Weather));
            if (p.HasCpeExt(CpeExt.EnvColors))
                p.SendCurrentEnvColors();
            p.SendCurrentMapAppearance();
            if (p.HasCpeExt(CpeExt.BlockPermissions))
                p.SendCurrentBlockPermissions();

            if (!level.guns && p.aiming) {
                p.aiming = false;
                p.ClearBlockchange();
            }
            if (!p.level.UseBlockDB) {
                Player.Message(p, "BlockDB is disabled here, &cyou will not be able to /undo or /redo");
            }
        }
    }
}
