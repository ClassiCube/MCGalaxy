/*
    Copyright 2011 MCForge
        
    Dual-licensed under the    Educational Community License, Version 2.0 and
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

namespace MCGalaxy {
    
    /// <summary> This event is called whenever a player chats on the server </summary>
    public sealed class OnPlayerChatEvent : IPluginEvent<Player.OnPlayerChat> {
        
        internal OnPlayerChatEvent(Player.OnPlayerChat method, Priority priority, Plugin plugin)
            : base(method, priority, plugin) { }
        
        public static void Call(Player p, string message) {      	
            events.ForEach(
                pl => {
                    try {
                        pl.method(p, message);
                    } catch (Exception e) {
                        Server.s.Log("Plugin " + pl.plugin.name + " errored when calling PlayerChat Event."); Server.ErrorLog(e);
                    }
                });
        }
    }
    
    /// <summary> This event is called whenever a player moves </summary>
    public sealed class PlayerMoveEvent : IPluginEvent<Player.OnPlayerMove>
    {
        internal PlayerMoveEvent(Player.OnPlayerMove method, Priority priority, Plugin plugin)
            : base(method, priority, plugin) { }
        
        public static void Call(Player p, ushort x, ushort y,  ushort z) {
            events.ForEach(
                pl => {
                    try {
                        pl.method(p, x, y, z);
                    } catch (Exception e) {
                        Server.s.Log("Plugin " + pl.plugin.name + " errored when calling PlayerMove Event."); Server.ErrorLog(e);
                    }
                });
        }
    }
    
    /// <summary> This event is called whenever a player rotates </summary>
    public sealed class PlayerRotateEvent : IPluginEvent<Player.OnPlayerRotate> {
        
        internal PlayerRotateEvent(Player.OnPlayerRotate method, Priority priority, Plugin plugin)
            : base(method, priority, plugin) { }
        
        public static void Call(Player p, byte[] rot) {      	
            events.ForEach(
                pl => {
                    try {
                        pl.method(p, rot);
                    } catch (Exception e) {
                        Server.s.Log("Plugin " + pl.plugin.name + " errored when calling PlayerRotate Event."); Server.ErrorLog(e);
                    }
                });
        }
    }
    
    /// <summary> This event is called whenever the player goes AFK </summary>
    public sealed class OnPlayerAFKEvent: IPluginEvent<Player.OnAFK > {

        internal OnPlayerAFKEvent(Player.OnAFK method, Priority priority, Plugin plugin)
            : base(method, priority, plugin) { }
        
        public static void Call(Player p) {
            events.ForEach(
                pl => {
                    try {
                        pl.method(p);
                    } catch (Exception e) {
                        Server.s.Log("Plugin " + pl.plugin.name + " errored when calling OnAFK Event."); Server.ErrorLog(e);
                    }
                });
        }
    }
    
    /// <summary> This event is called whenever the server saves data to MySQL or SQLite </summary>
    public sealed class OnMySQLSaveEvent : IPluginEvent<Player.OnMySQLSave> {

        internal OnMySQLSaveEvent(Player.OnMySQLSave method, Priority priority, Plugin plugin)
            : base(method, priority, plugin) { }
        
        public static void Call(Player p, string mysqlcommand) {
            events.ForEach(
                pl => {
                    try {
                        pl.method(p, mysqlcommand);
                    } catch (Exception e) {
                        Server.s.Log("Plugin " + pl.plugin.name + " errored when calling MySQLSave Event."); Server.ErrorLog(e);
                    }
                });
        }
    }
    
    /// <summary> This event is called whenever a player uses a command </summary>
    public sealed class OnPlayerCommandEvent : IPluginEvent<Player.OnPlayerCommand> {
        
        internal OnPlayerCommandEvent(Player.OnPlayerCommand method, Priority priority, Plugin plugin)
            : base(method, priority, plugin) { }
        
        public static void Call(string cmd, Player p, string message) {
            events.ForEach(
                pl => {
                    try {
                        pl.method(cmd, p, message);
                    } catch (Exception e) {
                        Server.s.Log("Plugin " + pl.plugin.name + " errored when calling PlayerCommand Event."); Server.ErrorLog(e);
                    }
                });
        }
    }
    
    /// <summary> This event is called whenever a player connects to the server </summary>
    public sealed class OnPlayerConnectEvent: IPluginEvent<Player.OnPlayerConnect> {
        
        internal OnPlayerConnectEvent(Player.OnPlayerConnect method, Priority priority, Plugin plugin)
            : base(method, priority, plugin) { }
        
        public static void Call(Player p) {
            events.ForEach(
                pl => {
                    try {
                        pl.method(p);
                    } catch (Exception e) {
                        Server.s.Log("Plugin " + pl.plugin.name + " errored when calling PlayerConnect Event."); Server.ErrorLog(e);
                    }
                });
        }
    }
    
    /// <summary> This event is called whenever a player dies in-game </summary>
    public sealed class OnPlayerDeathEvent : IPluginEvent<Player.OnPlayerDeath> {
        
        internal OnPlayerDeathEvent(Player.OnPlayerDeath method, Priority priority, Plugin plugin)
            : base(method, priority, plugin) { }
        
        public static void Call(Player p, byte type) {
            events.ForEach(
                pl => {
                    try {
                        pl.method(p, type);
                    } catch (Exception e) {
                        Server.s.Log("Plugin " + pl.plugin.name + " errored when calling PlayerDeath Event."); Server.ErrorLog(e);
                    }
                });
        }
    }
    
    /// <summary> This event is called whenever a player connects to the server </summary>
    public sealed class OnPlayerDisconnectEvent : IPluginEvent<Player.OnPlayerDisconnect> {
        
        internal OnPlayerDisconnectEvent(Player.OnPlayerDisconnect method, Priority priority, Plugin plugin)
            : base(method, priority, plugin) { }
        
        public static void Call(Player p, string reason) {
            events.ForEach(
                pl => {
                    try {
                        pl.method(p,reason);
                    } catch (Exception e) {
                        Server.s.Log("Plugin " + pl.plugin.name + " errored when calling PlayerDisconnect Event."); Server.ErrorLog(e);
                    }
                });
        }
    }
    
    /// <summary> This event is called whenever a player places or deletes a block </summary>
    public sealed class OnBlockChangeEvent : IPluginEvent<Player.BlockchangeEventHandler> {
        
        internal OnBlockChangeEvent(Player.BlockchangeEventHandler method, Priority priority, Plugin plugin)
            : base(method, priority, plugin) { }
        
        public static void Call(Player p, ushort x, ushort y, ushort z, byte block, byte extBlock) {
            events.ForEach(
                pl => {
                    try {
                        pl.method(p, x, y, z, block, extBlock);
                    } catch (Exception e) {
                        Server.s.Log("Plugin " + pl.plugin.name + " errored calling BlockChange Event."); Server.ErrorLog(e);
                    }
                });
        }
    }
    
    /// <summary> This event is called whenever a player recieves a message from the server or from another player </summary>
    public sealed class OnMessageRecieveEvent : IPluginEvent<Player.OnPlayerMessageReceived> {

        internal OnMessageRecieveEvent(Player.OnPlayerMessageReceived method, Priority priority, Plugin plugin)
            : base(method, priority, plugin) { }
        
        public static void Call(Player p, string message) {
            events.ForEach(
                pl => {
                    try {
                        pl.method(p, message);
                    } catch (Exception e) {
                        Server.s.Log("Plugin " + pl.plugin.name + " errored when calling MessageRecieve Event."); Server.ErrorLog(e);
                    }
                });
        }
    }
}
