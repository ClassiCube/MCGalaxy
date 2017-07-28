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

namespace MCGalaxy.Events.PlayerEvents {
    public enum PlayerAction { Joker, Unjoker, AFK, UnAFK, JoinWorld, Me, Review, Referee, UnReferee };
    
    public delegate void OnPlayerChat(Player p, string message);
    /// <summary> Called whenever a player chats on the server. </summary>
    /// <remarks> You must cancel this event to prevent the message being sent to the user (and others). </remarks>
    public sealed class OnPlayerChatEvent : IEvent<OnPlayerChat> {
        
        public static void Call(Player p, string message) {
            if (handlers.Count == 0) return;
            CallCommon(pl => pl(p, message));
        }
    }

    public delegate void OnPlayerMove(Player p, Position next, byte yaw, byte pitch);
    /// <summary> Called whenever a player moves. </summary>
    public sealed class OnPlayerMoveEvent : IEvent<OnPlayerMove> {
        
        public static void Call(Player p, Position next, byte yaw, byte pitch) {
            IEvent<OnPlayerMove>[] items = handlers.Items;
            // Don't use CallCommon, because this event is called very frequently
            // and we don't want lots of pointless temp mem allocations
            for (int i = 0; i < items.Length; i++) {
                IEvent<OnPlayerMove> handler = items[i];
                
                try {
                    handler.method(p, next, yaw, pitch);
                } catch (Exception ex) {
                    LogHandlerException(ex, handler);
                }
            }
        }
    }
    
    public delegate void OnSQLSave(Player p);
    /// <summary> This event is called whenever the server saves player's data to MySQL or SQLite </summary>
    public sealed class OnSQLSaveEvent : IEvent<OnSQLSave> {
        
        public static void Call(Player p) {
            if (handlers.Count == 0) return;
            CallCommon(pl => pl(p));
        }
    }
    
    public delegate void OnPlayerCommand(Player p, string cmd, string args);
    /// <summary> Called whenever a player uses a command. </summary>
    /// <remarks> You must cancel this event to prevent "Unknown command!" being shown. </remarks>
    public sealed class OnPlayerCommandEvent : IEvent<OnPlayerCommand> {
        
        public static void Call(Player p, string cmd, string args) {
            if (handlers.Count == 0) return;
            CallCommon(pl => pl(p, cmd, args));
        }
    }
    
    public delegate void OnPlayerConnect(Player p);
    /// <summary> Called whenever a player connects to the server </summary>
    public sealed class OnPlayerConnectEvent: IEvent<OnPlayerConnect> {
        
        public static void Call(Player p) {
            if (handlers.Count == 0) return;
            CallCommon(pl => pl(p));
        }
    }

    public delegate void OnPlayerConnecting(Player p, string mppass);
    /// <summary> Called whenever a player tries connecting to the server </summary>
    public sealed class OnPlayerConnectingEvent: IEvent<OnPlayerConnecting> {
        
        public static void Call(Player p, string mppass) {
            if (handlers.Count == 0) return;
            CallCommon(pl => pl(p, mppass));
        }
    }

    public delegate void OnPlayerDeath(Player p, ExtBlock cause);
    /// <summary> Called whenever a player dies in-game </summary>
    public sealed class OnPlayerDeathEvent : IEvent<OnPlayerDeath> {
        
        public static void Call(Player p, ExtBlock block) {
            if (handlers.Count == 0) return;
            CallCommon(pl => pl(p, block));
        }
    }
    
    public delegate void OnPlayerDisconnect(Player p, string reason);
    /// <summary> Called whenever a player disconnects from the server. </summary>
    public sealed class OnPlayerDisconnectEvent : IEvent<OnPlayerDisconnect> {
        
        public static void Call(Player p, string reason) {
            if (handlers.Count == 0) return;
            CallCommon(pl => pl(p, reason));
        }
    }

    public delegate void SelectionBlockChange(Player p, ushort x, ushort y, ushort z, ExtBlock block);
    public delegate void OnBlockChange(Player p, ushort x, ushort y, ushort z, ExtBlock block, bool placing);
    /// <summary> Called whenever a player places or deletes a block. </summary>
    public sealed class OnBlockChangeEvent : IEvent<OnBlockChange> {
        
        public static void Call(Player p, ushort x, ushort y, ushort z, ExtBlock block, bool placing) {
            if (handlers.Count == 0) return;
            CallCommon(pl => pl(p, x, y, z, block, placing));
        }
    }

    public delegate void OnPlayerClick(Player p, MouseButton button, MouseAction action,
                                       ushort yaw, ushort pitch, byte entity,
                                       ushort x, ushort y, ushort z, TargetBlockFace face);
    /// <summary> Called whenever a player clicks their mouse </summary>
    public sealed class OnPlayerClickEvent : IEvent<OnPlayerClick> {
        
        public static void Call(Player p, MouseButton button, MouseAction action,
                                ushort yaw, ushort pitch, byte entity, 
                                ushort x, ushort y, ushort z, TargetBlockFace face) {
            if (handlers.Count == 0) return;
            CallCommon(pl => pl(p, button, action, yaw,
                              pitch, entity, x, y, z, face));
        }
    }

    public delegate void OnMessageReceived(Player p, string message);
    /// <summary> Called whenever a player recieves a message from the server or from another player. </summary>
    public sealed class OnMessageRecievedEvent : IEvent<OnMessageReceived> {
        
        public static void Call(Player p, string message) {
            if (handlers.Count == 0) return;
            CallCommon(pl => pl(p, message));
        }
    }

    public delegate void OnJoinedLevel(Player p, Level prevLevel, Level level);
    /// <summary> Called when a player has joined a level. </summary>
    public sealed class OnJoinedLevelEvent : IEvent<OnJoinedLevel> {
        
        public static void Call(Player p, Level prevLevl, Level level) {
            if (handlers.Count == 0) return;
            CallCommon(pl => pl(p, prevLevl, level));
        }
    }

    public delegate void OnPlayerAction(Player p, PlayerAction action, 
                                        string message, bool stealth);
    /// <summary> Called when a player performs an action. </summary>
    public sealed class OnPlayerActionEvent : IEvent<OnPlayerAction> {
        
        public static void Call(Player p, PlayerAction action, 
                                string message = null, bool stealth = false) {
            if (handlers.Count == 0) return;
            CallCommon(pl => pl(p, action, message, stealth));
        }
    }

    public delegate void OnSendingMotd(Player p, byte[] packet);
    /// <summary> Called when MOTD is being send to the user. </summary>
    public sealed class OnSendingMotdEvent : IEvent<OnSendingMotd> {
        
        public static void Call(Player p, byte[] packet) {
            if (handlers.Count == 0) return;
            CallCommon(pl => pl(p, packet));
        }
    }
    

    public delegate void OnPlayerSpawning(Player p, ref Position pos, ref byte yaw, ref byte pitch, bool respawning);
    /// <summary> Called when a player is being initially spawned in a map, 
    /// or is respawning (e.g. died from a killer block). </summary>
    public sealed class OnPlayerSpawningEvent : IEvent<OnPlayerSpawning> {
        
        public static void Call(Player p, ref Position pos, ref byte yaw, ref byte pitch, bool respawning) {
            IEvent<OnPlayerSpawning>[] items = handlers.Items;
            // Can't use CallCommon because we need to pass arguments by ref
            for (int i = 0; i < items.Length; i++) {
                IEvent<OnPlayerSpawning> handler = items[i];
                
                try {
                    handler.method(p, ref pos, ref yaw, ref pitch, respawning);
                } catch (Exception ex) {
                    LogHandlerException(ex, handler);
                }
            }
        }
    }
}
