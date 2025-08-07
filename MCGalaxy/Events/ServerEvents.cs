﻿/*
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
using System.Collections.Generic;
using System.Net.Sockets;
using MCGalaxy.Network;

namespace MCGalaxy.Events.ServerEvents 
{
    public delegate void OnSendingHeartbeat(Heartbeat service, ref string name);
    /// <summary> Called when a heartbeat is being sent out. </summary>
    public sealed class OnSendingHeartbeatEvent : IEvent<OnSendingHeartbeat> 
    { 
        public static void Call(Heartbeat service, ref string name) {
            IEvent<OnSendingHeartbeat>[] items = handlers.Items;
            // Can't use CallCommon because we need to pass arguments by ref
            for (int i = 0; i < items.Length; i++) 
            {
                try { items[i].method(service, ref name); } 
                catch (Exception ex) { LogHandlerException(ex, items[i]); }
            }
        }
    }
    
    public delegate void OnShuttingDown(bool restarting, string reason);
    /// <summary> Called when the server is shutting down or restarting. </summary>
    public sealed class OnShuttingDownEvent : IEvent<OnShuttingDown> 
    {        
        public static void Call(bool restarting, string reason) {
            if (handlers.Count == 0) return;
            CallCommon(pl => pl(restarting, reason));
        }
    }
    
    public delegate void OnConfigUpdated();
    /// <summary> Called when the server configuration has been updated. </summary>
    public sealed class OnConfigUpdatedEvent : IEvent<OnConfigUpdated> 
    {      
        public static void Call() {
            if (handlers.Count == 0) return;
            CallCommon(pl => pl());
        }
    }
    
    public delegate void OnConnectionReceived(Socket s, ref bool cancel, ref bool announce);
    /// <summary> Called when a new connection has been received. </summary>
    public sealed class OnConnectionReceivedEvent : IEvent<OnConnectionReceived> 
    {        
        public static void Call(Socket s, ref bool cancel, ref bool announce) {
            IEvent<OnConnectionReceived>[] items = handlers.Items;
            // Can't use CallCommon because we need to pass arguments by ref
            for (int i = 0; i < items.Length; i++) 
            {
                try { items[i].method(s, ref cancel, ref announce); } 
                catch (Exception ex) { LogHandlerException(ex, items[i]); }
            }
        }
    }
    
    public delegate void OnChatSys(ChatScope scope, ref string msg, object arg,
                                   ref ChatMessageFilter filter, bool relay);
    public sealed class OnChatSysEvent : IEvent<OnChatSys> 
    {      
        public static void Call(ChatScope scope, ref string msg, object arg, 
                                ref ChatMessageFilter filter, bool relay) {
            IEvent<OnChatSys>[] items = handlers.Items;
            for (int i = 0; i < items.Length; i++) 
            {
                try { items[i].method(scope, ref msg, arg, ref filter, relay); } 
                catch (Exception ex) { LogHandlerException(ex, items[i]); }
            }
        }
    }
    
    public delegate void OnChatFrom(ChatScope scope, Player source, ref string msg, 
                                    object arg, ref ChatMessageFilter filter, bool relay);
    public sealed class OnChatFromEvent : IEvent<OnChatFrom> 
    {        
        public static void Call(ChatScope scope,Player source, ref string msg, 
                                object arg, ref ChatMessageFilter filter, bool relay) {
            IEvent<OnChatFrom>[] items = handlers.Items;
            for (int i = 0; i < items.Length; i++) 
            {
                try { items[i].method(scope, source, ref msg, arg, ref filter, relay); } 
                catch (Exception ex) { LogHandlerException(ex, items[i]); }
            }
        }        
    }
    
    public delegate void OnChat(ChatScope scope, Player source, ref string msg, 
                                object arg, ref ChatMessageFilter filter, bool relay);
    public sealed class OnChatEvent : IEvent<OnChat> 
    {       
        public static void Call(ChatScope scope, Player source, ref string msg, 
                                object arg, ref ChatMessageFilter filter, bool relay) {
            IEvent<OnChat>[] items = handlers.Items;
            for (int i = 0; i < items.Length; i++) 
            {
                try { items[i].method(scope, source, ref msg, arg, ref filter, relay); } 
                catch (Exception ex) { LogHandlerException(ex, items[i]); }
            }
        }
    }

    public delegate void OnPluginMessageReceived(Player p, byte channel, byte[] data);
    /// <summary> Called when a player sends a PluginMessage CPE packet to the server. </summary>
    public sealed class OnPluginMessageReceivedEvent : IEvent<OnPluginMessageReceived> 
    {        
        public static void Call(Player p, byte channel, byte[] data) {
            if (handlers.Count == 0) return;
            CallCommon(pl => pl(p, channel, data));
        }
    }
    
    public delegate void OnPluginsLoaded();
    /// <summary>Called when all plugins have been auto-loaded.</summary>
    public sealed class OnPluginsLoadedEvent : IEvent<OnPluginsLoaded> {
        public static void Call() {
            if (handlers.Count == 0) return;
            CallCommon(pl => pl());
        }
    }
}
