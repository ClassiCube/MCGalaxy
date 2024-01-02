/*
    Copyright 2015 MCGalaxy
        
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
using MCGalaxy.Config;
using MCGalaxy.Events;

namespace MCGalaxy.Modules.Relay.Discord 
{
    public delegate void OnSendingWhoEmbed(DiscordBot bot, RelayUser user, ref ChannelSendEmbed embed);
    /// <summary> Called when sending an embed response to a .who message from Discord </summary>
    public sealed class OnSendingWhoEmbedEvent : IEvent<OnSendingWhoEmbed> 
    { 
        public static void Call(DiscordBot bot, RelayUser user, ref ChannelSendEmbed embed) {
            IEvent<OnSendingWhoEmbed>[] items = handlers.Items;
            for (int i = 0; i < items.Length; i++) 
            {
                try {
                    items[i].method(bot, user, ref embed);
                } catch (Exception ex) {
                    LogHandlerException(ex, items[i]);
                }
            }
        }
    }
    
    public delegate void OnGatewayEventReceived(DiscordBot bot, string eventName, JsonObject data);
    /// <summary> Called when a gateway event has been received from Discord </summary>
    public sealed class OnGatewayEventReceivedEvent : IEvent<OnGatewayEventReceived> 
    { 
        public static void Call(DiscordBot bot, string eventName, JsonObject data) {
            IEvent<OnGatewayEventReceived>[] items = handlers.Items;
            for (int i = 0; i < items.Length; i++) 
            {
                try {
                    items[i].method(bot, eventName, data);
                } catch (Exception ex) {
                    LogHandlerException(ex, items[i]);
                }
            }
        }
    }
}
