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
using MCGalaxy.Maths;

namespace MCGalaxy {
    public enum PlayerAction { Joker, Unjoker, AFK, UnAFK, JoinWorld, Me, Review };
    
    /// <summary> This is the player object </summary>
    public sealed partial class Player {
        
        public bool cancelcommand, cancelchat, cancelmove, cancelBlock, cancelmysql;
        public bool cancelmessage, cancellogin, cancelconnecting;
        internal bool HasBlockchange { get { return Blockchange != null; } }
        
        //Should people be able to cancel this event?
        /// <summary> Called when the MOTD is sent to the player </summary>
        public delegate void MOTDSent(Player p, byte[] motdPacket);
        /// <summary> This event is called when the server is sending the MOTD to a player </summary>
        public static event MOTDSent OnSendMOTD;
        
        //Should people be able to cancel this event?
        /// <summary> Called after the server sent the map to a player. </summary>
        public delegate void MapSent(Player p, byte[] buffer);
        /// <summary> This event is called whenever the server sends a map to a player </summary>
        public static event MapSent OnSendMap;

        /// <summary> Called when the player has joined the given level. </summary>
        public delegate void JoinedLevel(Player p, Level prevLevel, Level level);
        /// <summary> This event is called when a player has joined a level. </summary>
        public static event JoinedLevel OnJoinedLevel;
        
        /// <summary> This is called when a player goes AFK </summary>
        /// <param name="p">The player that went AFK</param>
        public delegate void OnAFK(Player p);
        /// <summary> This event is triggered when a player goes AFK </summary>
        [Obsolete("Please use the OnPlayerAFKEvent.Register()")]
        public static event OnAFK AFK;
        public static void RaiseAFK(Player p) { if (AFK != null) AFK(p); }
        /// <summary> This event is triggered when a player goes AFK </summary>
        [Obsolete("Please use the OnPlayerAFKEvent.Register()")]
        public event OnAFK ONAFK;
        public void RaiseONAFK() { if (ONAFK != null) ONAFK(this); }
        
        /// <summary> Called when a player's data is saved to the database. (even if disabled) </summary>
        public delegate void OnMySQLSave(Player p, string sqlCommand);
        /// <summary> Called a players data is saved in mysql (even if disabled) (Can be cancled) </summary>
        [Obsolete("Please use the OnMySQLSaveEvent.Register()")]
        public static event OnMySQLSave MySQLSave;
        
        /// <summary> Called when a player removes or places a block. However, this event will due normal 
        /// permission checking and normal block placing unless the event you cancel the event </summary>
        public delegate void BlockchangeEventHandler2(Player p, ushort x, ushort y, ushort z, ExtBlock block);        
        /// <summary> Called when a player removes, or places a block. </summary>
        public delegate void BlockchangeEventHandler(Player p, ushort x, ushort y, ushort z, ExtBlock block);
        /// <summary> Called when a player removes or places a block </summary>
        public event BlockchangeEventHandler Blockchange;
        /// <summary> Called when a player places a block. </summary>
        public static event BlockchangeEventHandler2 PlayerBlockChange;
        public void ClearBlockchange() { Blockchange = null; }
        public object blockchangeObject;

        /// <summary> Called when a player clicks their mouse. </summary>
        public delegate void PlayerClickHandler(Player p, MouseButton button, MouseAction action, ushort yaw, ushort pitch, byte entity, ushort x, ushort y, ushort z, TargetBlockFace face);
        /// <summary> Called when a player clicks their mouse </summary>
        public static event PlayerClickHandler OnPlayerClick;

        /// <summary> Called when a player connects to the server. </summary>
        public delegate void OnPlayerConnect(Player p);
        /// <summary> Called when a player connects to the server. </summary>
        public static event OnPlayerConnect PlayerConnect;

        /// <summary> Called when a player tries connecting to the server. </summary>
        public delegate void OnPlayerConnecting(Player p, string mppass);
        /// <summary> Called when a player tries connecting to the server. </summary>
        public static event OnPlayerConnecting PlayerConnecting;
        
        /// <summary> Called when a player disconnects. </summary>
        public delegate void OnPlayerDisconnect(Player p, string reason);        
        /// <summary> Called when a player disconnects. </summary>
        public static event OnPlayerDisconnect PlayerDisconnect;
        
        /// <summary> Called when a player does a command. However, the server will still look for 
        /// another command unless you cancel the event </summary>
        public delegate void OnPlayerCommand(string cmd, Player p, string message);
        /// <summary> Called when a player does a command (string cmd, Player p, string message) </summary>
        public static event OnPlayerCommand PlayerCommand;
        /// <summary> Called when the player does a command. </summary>
        public event OnPlayerCommand OnCommand;
        public void ClearPlayerCommand() { OnCommand = null; }
        
        /// <summary> Called when a player chats on the server. Message will be sent unless you cancel the event. </summary>
        public delegate void OnPlayerChat(Player p, string message);
        /// <summary> Called when a player receives messages from the server. (including chat) </summary>
        public delegate void OnPlayerMessageReceived(Player p, string message);
        /// <summary> Called when a player chats. </summary>
        public static event OnPlayerChat PlayerChat;
        /// <summary> Called when a player is about to recieve a chat message </summary>
        public static event OnPlayerMessageReceived MessageRecieve;
        /// <summary> Called when the player is about to recieve a chat message </summary>
        public event OnPlayerMessageReceived OnMessageRecieve;
        /// <summary> Called when the player chats. </summary>
        public event OnPlayerChat OnChat;
        public void ClearPlayerChat() { OnChat = null; }
        
        /// <summary> Called when a player dies. </summary>
        public delegate void OnPlayerDeath(Player p, ExtBlock deathblock);     
        /// <summary> Called when the player dies. </summary>
        public event OnPlayerDeath OnDeath;
        /// <summary> Called when a player dies. </summary>
        public static event OnPlayerDeath PlayerDeath;
        public void ClearPlayerDeath() { OnDeath = null; }
         
        /// <summary> Called when a player moves on the server </summary>
        public delegate void OnPlayerMove(Player p, Position next, byte yaw, byte pitch);
        /// <summary> Called when a player moves. </summary>
        public static event OnPlayerMove PlayerMove;
        /// <summary> Called when the player moves. </summary>
        public event OnPlayerMove OnMove;
        
        /// <summary> Called when the player performs an action. </summary>
        public delegate void OnPlayerAction(Player p, PlayerAction action, 
                                            string message, bool stealth);
        /// <summary> Called when a player performs an action. </summary>
        public static event OnPlayerAction DoPlayerAction;
        public static void RaisePlayerAction(Player p, PlayerAction action,
                                             string message = null, bool stealth = false) {
            OnPlayerAction change = DoPlayerAction;
            if (change != null) change(p, action, message, stealth);
        }
        
        /// <summary> Called when the player has finished providing all the marks for a selection. </summary>
        public delegate bool SelectionHandler(Player p, Vec3S32[] marks, object state, ExtBlock block);
    }
}
