﻿/*
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

namespace MCGalaxy.Commands.Info 
{  
    public sealed class CmdPClients : Command2 {        
        public override string name { get { return "PClients"; } }
        public override string shortcut { get { return "Clients"; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool UseableWhenFrozen { get { return true; } }
        
        public override void Use(Player p, string message, CommandData data) 
        {
            Dictionary<string, List<Player>> clients = new Dictionary<string, List<Player>>();
            Player[] online = PlayerInfo.Online.Items;
            
            foreach (Player pl in online) 
            {
                if (!p.CanSee(pl, data.Rank)) continue;
                string appName = pl.appName;
                if (String.IsNullOrEmpty(appName)) 
                { 
                    switch(pl.ProtocolVersion)
                    {
                        case 3:
                            appName = "(unknown ?-0.0.16a_02 client)";
                            break;
                        case 4:
                            appName = "(unknown 0.0.17a-0.0.18a_02 client)";
                            break;
                        case 5:
                            appName = "(unknown 0.0.19a-0.0.19a_06 client)";
                            break;
                        case 6:
                            appName = "(unknown 0.0.20a-0.0.23a_01 client)";
                            break;
                        case 7:
                            appName = "(unknown 0.28-0.30 client)";
                            break;
                    }
                } 
                    
               List<Player> usingClient;
               if (!clients.TryGetValue(appName, out usingClient)) 
               {
                    usingClient = new List<Player>();
                    clients[appName] = usingClient;
               }
               usingClient.Add(pl);
            }
            
            p.Message("Players using:");
            foreach (var kvp in clients) 
            {
                StringBuilder builder = new StringBuilder();
                List<Player> players  = kvp.Value;
                
                for (int i = 0; i < players.Count; i++) 
                {
                    string nick = Colors.StripUsed(p.FormatNick(players[i]));
                    builder.Append(nick);
                    if (i < players.Count - 1) builder.Append(", ");
                }
                p.Message("  {0}: &f{1}", kvp.Key, builder.ToString());
            }
        }

        public override void Help(Player p) 
        {
            p.Message("&T/PClients");
            p.Message("&HLists the clients players are using, and who uses which client.");
        }
    }
}
