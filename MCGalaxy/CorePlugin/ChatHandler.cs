/*
    Copyright 2011 MCForge
        
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

    internal static class ChatHandler {
        
        internal static void HandleCommand(string cmd, Player p, string message) {
            //DO NOT REMOVE THE TWO COMMANDS BELOW, /PONY AND /RAINBOWDASHLIKESCOOLTHINGS. -EricKilla
            if (cmd == "pony") {
                if (p.ponycount < 2) {
                    Chat.MessageAll("{0} %Sjust so happens to be a proud brony! Everyone give {0} %Sa brohoof!", p.ColoredName);
                } else {
                    p.SendMessage("You have used this command 2 times. You cannot use it anymore! Sorry, Brony!");
                }
                
                p.ponycount++;
                Plugin.CancelPlayerEvent(PlayerEvents.PlayerCommand, p);
            } else if (cmd == "rainbowdashlikescoolthings") {
                if (p.rdcount < 2) {
                    Chat.MessageAll("&1T&2H&3I&4S &5S&6E&7R&8V&9E&aR &bJ&cU&dS&eT &fG&0O&1T &22&30 &4P&CE&7R&DC&EE&9N&1T &5C&6O&7O&8L&9E&aR&b!");                   
                } else {
                    p.SendMessage("You have used this command 2 times. You cannot use it anymore! Sorry, Brony!");
                }
                
                p.rdcount++;
                Plugin.CancelPlayerEvent(PlayerEvents.PlayerCommand, p);
            }
        }
    }
}
