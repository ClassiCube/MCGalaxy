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
using MCGalaxy.DB;
using MCGalaxy.Eco;
using MCGalaxy.Games;
using MCGalaxy.SQL;

namespace MCGalaxy.Modules.Games.LS
{    
    public sealed partial class LSGame : RoundsGame 
    {
        static void HookItems() {
            Economy.RegisterItem(itemLife);
            Economy.RegisterItem(itemSponges);
            Economy.RegisterItem(itemWater);
        }
        
        static void UnhookItems() {
            Economy.Items.Remove(itemLife);
            Economy.Items.Remove(itemSponges);
            Economy.Items.Remove(itemWater);
        }       
        
        static Item itemLife    = new LifeItem();
        static Item itemSponges = new SpongesItem();
        static Item itemWater   = new WaterItem();
                
        
        static void HookCommands() {
            Command.TryRegister(true, cmdLives);
        }
        
        static void UnhookCommands() {
            Command.Unregister(cmdLives);
        }
        
        static Command cmdLives = new CmdLives();
    }
}
