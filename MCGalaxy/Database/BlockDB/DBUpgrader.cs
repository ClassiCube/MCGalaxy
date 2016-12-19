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
using System.Collections.Generic;
using MCGalaxy.SQL;

namespace MCGalaxy.DB {
    
    public static class DBUpgrader {
        
        public static void SetupState() {
            Server.s.Log("Kicking players and unloading levels..");
            Player.PlayerConnecting += ConnectingHandler;
            
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player p in players) {
                p.Leave("Upgrading BlockDB. Check back later!");
            }
            
            Level[] levels = LevelInfo.Loaded.Items;
            foreach (Level lvl in levels) {
                lvl.Unload();
            }
            Server.s.Log("Kicked all players and unloaded levels.");
        }
        
        public static void ResetState() {
            Player.PlayerConnecting -= ConnectingHandler;
        }
        
        static void ConnectingHandler(Player p, string mppass) {
            string progress = current + " / " + count;
            p.Leave("Upgrading BlockDB (" + progress + "). Check back later!");
            Plugin.CancelPlayerEvent(PlayerEvents.PlayerConnecting, p);
        }
        
		
        static int current, count;
        public static void Upgrade() {
            List<string> tables = Database.Backend.AllTables();
            List<string> blockDBTables = new List<string>(tables.Count);
            current = 0; count = 0;
            
            foreach (string table in tables) {
                if (!table.CaselessStarts("block")) continue;
                blockDBTables.Add(table);
            }
            
            current = 0;
            count = blockDBTables.Count;
            Server.s.Log("Upgrading " + count + " tables. this may take several hours..");
            
            // TODO: do the upgrading here
        }
    }
}
