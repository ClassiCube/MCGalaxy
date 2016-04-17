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
using System.Text;

namespace MCGalaxy.Commands {
    
    public sealed class CmdPClients : Command {
        
        public override string name { get { return "pclients"; } }
        public override string shortcut { get { return "clients"; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public CmdPClients() { }

        public override void Use(Player p, string message) {
            Dictionary<string, List<Player>> clients = new Dictionary<string, List<Player>>();
            Player[] online = PlayerInfo.Online.Items;
            foreach (Player pl in online) {
            	if (!Entities.CanSee(p, pl)) continue;
                string appName = pl.appName;
                if (String.IsNullOrEmpty(appName))
                    appName = "(unknown)";
                    
               List<Player> usingClient;
               if (!clients.TryGetValue(appName, out usingClient)) {
                    usingClient = new List<Player>();
                    clients[appName] = usingClient;
                }
                usingClient.Add(pl);
            }
            
            Player.SendMessage(p, "Players using:");
            foreach (var kvp in clients) {
                StringBuilder builder = new StringBuilder();
                List<Player> players = kvp.Value;
                for (int i = 0; i < players.Count; i++) {
                    string name = Colors.StripColours(players[i].DisplayName);
                    builder.Append(name);
                    if (i < players.Count - 1)
                        builder.Append(", ");
                }
                
                string msg = String.Format("  {0}: &f{1}", kvp.Key, builder.ToString());
                Player.SendMessage(p, msg);
            }
        }

        public override void Help(Player p) {
            Player.SendMessage(p, "/pclients - Lists the clients players are using, and who uses which client.");
        }
    }
}
