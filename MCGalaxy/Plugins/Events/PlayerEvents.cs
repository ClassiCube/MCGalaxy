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

namespace MCGalaxy.Events {
    
    /// <summary> This event is called whenever a player chats on the server </summary>
    public sealed class OnPlayerChatEvent : IPluginEvent<Player.OnPlayerChat> {        
        internal OnPlayerChatEvent(Player.OnPlayerChat method, Priority priority, Plugin plugin)
            : base(method, priority, plugin) { }
        
        public static void Call(Player p, string message) {
            if (handlers.Count == 0) return;
            CallImpl(pl => pl(p, message));
        }
    }
    
    /// <summary> This event is called whenever a player moves </summary>
    public sealed class OnPlayerMoveEvent : IPluginEvent<Player.OnPlayerMove> {
        internal OnPlayerMoveEvent(Player.OnPlayerMove method, Priority priority, Plugin plugin)
            : base(method, priority, plugin) { }
        
        public static void Call(Player p, Position next, byte yaw, byte pitch) {
            if (handlers.Count == 0) return;
            CallImpl(pl => pl(p, next, yaw, pitch));
        }
    }
    
    /// <summary> This event is called whenever the player goes AFK </summary>
    public sealed class OnPlayerAFKEvent: IPluginEvent<Player.OnAFK > {
        internal OnPlayerAFKEvent(Player.OnAFK method, Priority priority, Plugin plugin)
            : base(method, priority, plugin) { }
        
        public static void Call(Player p) {
            if (handlers.Count == 0) return;
            CallImpl(pl => pl(p));
        }
    }
    
    /// <summary> This event is called whenever the server saves data to MySQL or SQLite </summary>
    public sealed class OnMySQLSaveEvent : IPluginEvent<Player.OnMySQLSave> {
        internal OnMySQLSaveEvent(Player.OnMySQLSave method, Priority priority, Plugin plugin)
            : base(method, priority, plugin) { }
        
        public static void Call(Player p, string mysqlcommand) {
            if (handlers.Count == 0) return;
            CallImpl(pl => pl(p, mysqlcommand));
        }
    }
    
    /// <summary> This event is called whenever a player uses a command </summary>
    public sealed class OnPlayerCommandEvent : IPluginEvent<Player.OnPlayerCommand> {        
        internal OnPlayerCommandEvent(Player.OnPlayerCommand method, Priority priority, Plugin plugin)
            : base(method, priority, plugin) { }
        
        public static void Call(string cmd, Player p, string message) {
            if (handlers.Count == 0) return;
            CallImpl(pl => pl(cmd, p, message));
        }
    }
    
    /// <summary> This event is called whenever a player connects to the server </summary>
    public sealed class OnPlayerConnectEvent: IPluginEvent<Player.OnPlayerConnect> {       
        internal OnPlayerConnectEvent(Player.OnPlayerConnect method, Priority priority, Plugin plugin)
            : base(method, priority, plugin) { }
        
        public static void Call(Player p) {
            if (handlers.Count == 0) return;
            CallImpl(pl => pl(p));
        }
    }

    /// <summary> This event is called whenever a player tries connecting to the server </summary>
    public sealed class OnPlayerConnectingEvent: IPluginEvent<Player.OnPlayerConnecting> {       
        internal OnPlayerConnectingEvent(Player.OnPlayerConnecting method, Priority priority, Plugin plugin)
            : base(method, priority, plugin) { }
        
        public static void Call(Player p, string mppass) {
            if (handlers.Count == 0) return;
            CallImpl(pl => pl(p, mppass));
        }
    }
    
    /// <summary> This event is called whenever a player dies in-game </summary>
    public sealed class OnPlayerDeathEvent : IPluginEvent<Player.OnPlayerDeath> {        
        internal OnPlayerDeathEvent(Player.OnPlayerDeath method, Priority priority, Plugin plugin)
            : base(method, priority, plugin) { }
        
        public static void Call(Player p, ExtBlock block) {
            if (handlers.Count == 0) return;
            CallImpl(pl => pl(p, block));
        }
    }
    
    /// <summary> This event is called whenever a player connects to the server </summary>
    public sealed class OnPlayerDisconnectEvent : IPluginEvent<Player.OnPlayerDisconnect> {        
        internal OnPlayerDisconnectEvent(Player.OnPlayerDisconnect method, Priority priority, Plugin plugin)
            : base(method, priority, plugin) { }
        
        public static void Call(Player p, string reason) {
            if (handlers.Count == 0) return;
            CallImpl(pl => pl(p, reason));
        }
    }
    
    /// <summary> This event is called whenever a player places or deletes a block </summary>
    public sealed class OnBlockChangeEvent : IPluginEvent<Player.BlockchangeEventHandler> {        
        internal OnBlockChangeEvent(Player.BlockchangeEventHandler method, Priority priority, Plugin plugin)
            : base(method, priority, plugin) { }
        
        public static void Call(Player p, ushort x, ushort y, ushort z, ExtBlock block) {
            if (handlers.Count == 0) return;
            CallImpl(pl => pl(p, x, y, z, block));
        }
    }

    /// <summary> This event is called whenever a player clicks their mouse </summary>
    public sealed class OnPlayerClickEvent : IPluginEvent<Player.PlayerClickHandler>
    {
        internal OnPlayerClickEvent(Player.PlayerClickHandler method, Priority priority, Plugin plugin)
            : base(method, priority, plugin) { }

        public static void Call(Player p, MouseButton button, MouseAction action, ushort yaw, ushort pitch, byte entity, ushort x, ushort y, ushort z, TargetBlockFace face)
        {
            if (handlers.Count == 0) return;
            CallImpl(pl => pl(p, button, action, yaw,
                pitch, entity, x, y, z, face));
        }
    }

    /// <summary> This event is called whenever a player recieves a message from the server or from another player </summary>
    public sealed class OnMessageRecieveEvent : IPluginEvent<Player.OnPlayerMessageReceived> {
        internal OnMessageRecieveEvent(Player.OnPlayerMessageReceived method, Priority priority, Plugin plugin)
            : base(method, priority, plugin) { }
        
        public static void Call(Player p, string message) {
            if (handlers.Count == 0) return;
            CallImpl(pl => pl(p, message));
        }
    }
    

    /// <summary> This event is called when a player has joined a level. </summary>
    public sealed class OnJoinedLevelEvent : IPluginEvent<Player.JoinedLevel> {
        internal OnJoinedLevelEvent(Player.JoinedLevel method, Priority priority, Plugin plugin)
            : base(method, priority, plugin) { }
        
        public static void Call(Player p, Level prevLevl, Level level) {
            if (handlers.Count == 0) return;
            CallImpl(pl => pl(p, prevLevl, level));
        }
    }
}
