/*
    Copyright 2011 MCForge
        
    Dual-licensed under the    Educational Community License, Version 2.0 and
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
using MCGalaxy.Events;

namespace MCGalaxy.Modules.Relay 
{
    public delegate void OnDirectMessage(RelayBot bot, string channel, RelayUser user, string message, ref bool cancel);
    /// <summary> Called when an external communication service user sends a message directly to the relay bot </summary>
    public sealed class OnDirectMessageEvent : IEvent<OnDirectMessage> 
    {
        public static void Call(RelayBot bot, string channel, RelayUser user, string message, ref bool cancel) {
            IEvent<OnDirectMessage>[] items = handlers.Items;
            for (int i = 0; i < items.Length; i++) 
            {
                try {
                    items[i].method(bot, channel, user, message, ref cancel);
                } catch (Exception ex) {
                    LogHandlerException(ex, items[i]);
                }
            }
        }
    }
    
    public delegate void OnChannelMessage(RelayBot bot, string channel, RelayUser user, string message, ref bool cancel);
    /// <summary> Called when an external communication service user sends a message to the given channel </summary>
    public sealed class OnChannelMessageEvent : IEvent<OnChannelMessage> 
    { 
        public static void Call(RelayBot bot, string channel, RelayUser user, string message, ref bool cancel) {
            IEvent<OnChannelMessage>[] items = handlers.Items;
            for (int i = 0; i < items.Length; i++) 
            {
                try {
                    items[i].method(bot, channel, user, message, ref cancel);
                } catch (Exception ex) {
                    LogHandlerException(ex, items[i]);
                }
            }
        }
    }
}
