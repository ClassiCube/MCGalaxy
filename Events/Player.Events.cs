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
namespace MCGalaxy {
    public enum PlayerAction { Joker, Unjoker, AFK, UnAFK, JoinWorld, Me };
    
    /// <summary> This is the player object </summary>
    public sealed partial class Player {
        
        internal bool cancelcommand = false;
        internal bool cancelchat = false;
        internal bool cancelmove = false;
        internal bool cancelBlock = false;
        internal bool cancelmysql = false;
        internal bool cancelmessage = false;
        
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
        
        /// <summary> This is called when a player goes AFK </summary>
        /// <param name="p">The player that went AFK</param>
        public delegate void OnAFK(Player p);
        /// <summary> This event is triggered when a player goes AFK </summary>
        [Obsolete("Please use the OnPlayerAFKEvent.Register()")]
        public static event OnAFK AFK;
        /// <summary> This event is triggered when a player goes AFK </summary>
        [Obsolete("Please use the OnPlayerAFKEvent.Register()")]
        public event OnAFK ONAFK;
        
        /// <summary> Called when a player's data is saved to the database. (even if disabled) </summary>
        public delegate void OnMySQLSave(Player p, string sqlCommand);
        /// <summary> Called a players data is saved in mysql (even if disabled) (Can be cancled) </summary>
        [Obsolete("Please use the OnMySQLSaveEvent.Register()")]
        public static event OnMySQLSave MySQLSave;
        
        /// <summary> Called when a player removes or places a block. However, this event will due normal 
        /// permission checking and normal block placing unless the event you cancel the event </summary>
        public delegate void BlockchangeEventHandler2(Player p, ushort x, ushort y, ushort z, byte type, byte extType);        
        /// <summary> Called when a player removes, or places a block. </summary>
        public delegate void BlockchangeEventHandler(Player p, ushort x, ushort y, ushort z, byte type, byte extType);
        /// <summary> Called when a player removes or places a block </summary>
        [Obsolete("Please use OnBlockChangeEvent.Register()")]
        public event BlockchangeEventHandler Blockchange = null;
        /// <summary> Called when a player places a block. </summary>
        [Obsolete("Please use OnBlockChangeEvent.Register()")]
        public static event BlockchangeEventHandler2 PlayerBlockChange = null;
        public void ClearBlockchange() { Blockchange = null; }
        public object blockchangeObject = null;
        
        /// <summary> Called when a player connects to the server. </summary>
        public delegate void OnPlayerConnect(Player p);
        /// <summary> Called when a player connects to the server. </summary>
        [Obsolete("Please use OnPlayerConnectEvent.Register()")]
        public static event OnPlayerConnect PlayerConnect = null;
        
        /// <summary> Called when a player disconnects. </summary>
        public delegate void OnPlayerDisconnect(Player p, string reason);        
        /// <summary> Called when a player disconnects. </summary>
        public static event OnPlayerDisconnect PlayerDisconnect = null;
        
        /// <summary> Called when a player does a command. However, the server will still look for 
        /// another command unless you cancel the event </summary>
        public delegate void OnPlayerCommand(string cmd, Player p, string message);
        /// <summary> Called when a player does a command (string cmd, Player p, string message) </summary>
        [Obsolete("Please use OnPlayerCommandEvent.Register()")]
        public static event OnPlayerCommand PlayerCommand = null;
        /// <summary> Called when the player does a command. </summary>
        [Obsolete("Please use OnPlayerCommandEvent.Register()")]
        public event OnPlayerCommand OnCommand = null;
        public void ClearPlayerCommand() { OnCommand = null; }
        
        /// <summary> Called when a player chats on the server. Message will be sent unless you cancel the event. </summary>
        public delegate void OnPlayerChat(Player p, string message);
        /// <summary> Called when a player receives messages from the server. (including chat) </summary>
        public delegate void OnPlayerMessageReceived(Player p, string message);
        /// <summary> Called when a player chats. </summary>
        [Obsolete("Please use OnPlayerChatEvent.Register()")]
        public static event OnPlayerChat PlayerChat = null;
        /// <summary> Called when a player is about to recieve a chat message </summary>
        [Obsolete("Please use OnMessageRecieveEvent.Register()")]
        public static event OnPlayerMessageReceived MessageRecieve = null;
        /// <summary> Called when the player is about to recieve a chat message </summary>
        [Obsolete("Please use OnMessageRecieveEvent.Register()")]
        public event OnPlayerMessageReceived OnMessageRecieve = null;
        /// <summary> Called when the player chats. </summary>
        [Obsolete("Please use OnPlayerChatEvent.Register()")]
        public event OnPlayerChat OnChat = null;
        public void ClearPlayerChat() { OnChat = null; }
        
        /// <summary> Called when a player dies. </summary>
        public delegate void OnPlayerDeath(Player p, byte deathblock);     
        /// <summary> Called when the player dies. </summary>
        [Obsolete("Please use OnPlayerDeathEvent.Register()")]
        public event OnPlayerDeath OnDeath = null;
        /// <summary> Called when a player dies. </summary>
        [Obsolete("Please use OnPlayerDeathEvent.Register()")]
        public static event OnPlayerDeath PlayerDeath = null;
        public void ClearPlayerDeath() { OnDeath = null; }
         
        /// <summary> TCalled when a player moves on the server </summary>
        public delegate void OnPlayerMove(Player p, ushort x, ushort y, ushort z);
        /// <summary> Called when a player moves. </summary>
        public static event OnPlayerMove PlayerMove = null;
        /// <summary> Called when the player moves. </summary>
        public event OnPlayerMove OnMove = null;
        
        /// <summary> Called when a player rotates on the server. 
        /// (rot[0] is yaw, rot[1] is pitch). </summary>
        public delegate void OnPlayerRotate(Player p, byte[] rot);
        /// <summary> Called when a player rotates. </summary>
        public static event OnPlayerRotate PlayerRotate = null;
        /// <summary> Called when the player rotates. </summary>
        public event OnPlayerRotate OnRotate = null;
        
        /// <summary> Called when the player performs an action. </summary>
        public delegate void OnPlayerAction(Player p, PlayerAction action, 
                                            string message, bool stealth);
        /// <summary> Called when a player performs an action. </summary>
        public static event OnPlayerAction DoPlayerAction = null;      
        public static void RaisePlayerAction(Player p, PlayerAction action,
                                             string message = null, bool stealth = false) {
            OnPlayerAction change = DoPlayerAction;
            if (change != null) change(p, action, message, stealth);
        }
    }
}
