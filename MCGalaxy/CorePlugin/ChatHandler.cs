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
                    Chat.MessageGlobal("{0} %Sjust so happens to be a proud brony! Everyone give {0} %Sa brohoof!", p.ColoredName);
                } else {
                    p.SendMessage("You have used this command 2 times. You cannot use it anymore! Sorry, Brony!");
                }
                
                p.ponycount++;
                Plugin.CancelPlayerEvent(PlayerEvents.PlayerCommand, p);
            } else if (cmd == "rainbowdashlikescoolthings") {
                if (p.rdcount < 2) {
                    Chat.MessageGlobal("&4T&6H&eI&aS&3 S&9E&1R&4V&6E&eR &aJ&3U&9S&1T &4G&6O&eT &a2&30 &9P&1E&4R&6C&eE&aN&3T &9C&1O&4O&6L&eE&aR&3!");                   
                } else {
                    p.SendMessage("You have used this command 2 times. You cannot use it anymore! Sorry, Brony!");
                }
                
                p.rdcount++;
                Plugin.CancelPlayerEvent(PlayerEvents.PlayerCommand, p);
            }
        }
    }
}
