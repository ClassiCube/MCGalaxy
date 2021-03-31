/*
Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
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
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using MCGalaxy.DB;
using MCGalaxy.Drawing;
using MCGalaxy.Events.EconomyEvents;
using MCGalaxy.Events.EntityEvents;
using MCGalaxy.Events.PlayerEvents;
using MCGalaxy.Events.PlayerDBEvents;
using MCGalaxy.Games;
using MCGalaxy.Maths;
using MCGalaxy.Network;
using MCGalaxy.SQL;
using BlockID = System.UInt16;

namespace MCGalaxy {
    sealed class ConsolePlayer : Player {
        public ConsolePlayer() : base("(console)") {
            group = Group.NobodyRank;
            color = "&S";
            SuperName = "Console";
        }
        
        public override string FullName {
            get { return "Console [&a" + Server.Config.ConsoleName + "&S]"; }
        }
        
        public override void Message(byte type, string message) {
            Logger.Log(LogType.ConsoleMessage, message);
        }
    }
    
    public partial class Player : Entity, IDisposable {

        static int sessionCounter;
        public static Player Console = new ConsolePlayer();

        //This is so that plugin devs can declare a player without needing a socket..
        //They would still have to do p.Dispose()..
        public Player(string playername) { 
            name = playername;
            truename = playername;
            DisplayName = playername;
            SessionID = Interlocked.Increment(ref sessionCounter) & SessionIDMask;
            IsSuper = true;
        }

        internal Player() {
            spamChecker = new SpamChecker(this);
            SessionID = Interlocked.Increment(ref sessionCounter) & SessionIDMask;
            for (int b = 0; b < BlockBindings.Length; b++) {
                BlockBindings[b] = (BlockID)b;
            }
        }
        
        public override byte EntityID { get { return id; } }
        public override Level Level { get { return level; } }
        public override bool RestrictsScale { get { return true; } }
        
        /// <summary> Whether this player can see the given player. </summary>
        public bool CanSee(Player target) { return CanSee(target, Rank); }
        /// <summary> Whether this player can see the given player, as if they were the given rank. </summary>
        public bool CanSee(Player target, LevelPermission plRank) {
            if (target == this || target == null) return true;
            
            bool canSee = !target.hidden || plRank >= target.hideRank;
            OnGettingCanSeeEvent.Call(this, plRank, ref canSee, target);
            return canSee;
        }
        
        public override bool CanSeeEntity(Entity target) {
            if (target == this) return true; // always see self
            
            bool canSee = CanSee(target as Player, Rank);
            OnGettingCanSeeEntityEvent.Call(this, ref canSee, target);
            return canSee;
        }        
        
        public BlockID GetHeldBlock() {
            if (ModeBlock != Block.Invalid) return ModeBlock;
            return BlockBindings[ClientHeldBlock];
        }
        
        public string GetMotd() {
            Zone zone = ZoneIn;
            string motd = zone == null ? "ignore" : zone.Config.MOTD;
            
            // fallback to level MOTD, then rank MOTD, then server MOTD            
            if (motd == "ignore") motd = level.Config.MOTD;           
            if (motd == "ignore") motd = String.IsNullOrEmpty(group.MOTD) ? Server.Config.MOTD : group.MOTD;
            
            OnGettingMotdEvent.Call(this, ref motd);
            return motd;
        }
        
        public void SetPrefix() {
            prefix = Game.Referee ? "&2[Ref] " : "";
            if (GroupPrefix.Length > 0) { prefix += GroupPrefix + color; }
            
            Team team = Game.Team;
            prefix += team != null ? "<" + team.Color + team.Name + color + "> " : "";
            
            IGame game = IGame.GameOn(level);
            if (game != null) game.AdjustPrefix(this, ref prefix);
            
            bool isDev = Server.Devs.CaselessContains(truename);
            bool devPrefix = Server.Config.SoftwareStaffPrefixes;
            
            if (devPrefix && isDev) prefix += MakeTitle("Dev", "&9");
            if (title.Length > 0)   prefix += MakeTitle(title, titlecolor);
        }
        
        /// <summary> Raises OnSettingColorEvent then sets color. </summary>
        public void SetColor(string col) {
            OnSettingColorEvent.Call(this, ref col);
            color = col;
        }
        
        /// <summary> Calls SetColor, then updates state depending on color. 
        /// (e.g. entity name, tab list name, prefix, etc) </summary>
        public void UpdateColor(string col) {
            string prevCol = color;
            SetColor(col);
            
            if (prevCol == color) return;
            Entities.GlobalRespawn(this);
            SetPrefix();
        }
        
        internal string MakeTitle(string title, string titleCol) {
             return color + "[" + titleCol + title + color + "] ";
        }
        
        public bool IsLikelyInsideBlock() {
            AABB bb = ModelBB.OffsetPosition(Pos);
            // client collision is very slightly more precise than server
            // so move slightly in to avoid false positives
            bb = bb.Expand(-1);
            return AABB.IntersectsSolidBlocks(bb, level);
        }

        public void SaveStats() {
            bool cancel = false;      	
            OnInfoSaveEvent.Call(this, ref cancel);
            if (cancel) return;

            // Player disconnected before SQL data was retrieved
            if (!gotSQLData) return;
            long blocks = PlayerData.Pack(TotalPlaced,  TotalModified);
            long drawn  = PlayerData.Pack(TotalDeleted, TotalDrawn);
            Database.UpdateRows("Players", "IP=@0, LastLogin=@1, totalLogin=@2, totalDeaths=@3, Money=@4, " +
                                "totalBlocks=@5, totalCuboided=@6, totalKicked=@7, TimeSpent=@8, Messages=@9", "WHERE Name=@10",
                                ip, LastLogin.ToString(Database.DateFormat),
                                TimesVisited, TimesDied, money, blocks,
                                drawn, TimesBeenKicked, (long)TotalTime.TotalSeconds, TotalMessagesSent, name);
        }
        
        public bool CanUse(Command cmd) { return group.Commands.Contains(cmd); }
        public bool CanUse(string cmdName) {
            Command cmd = Command.Find(cmdName);
            return cmd != null && CanUse(cmd);
        }

        public bool MarkPossessed(string marker = "") {
            if (marker.Length > 0) {
                Player controller = PlayerInfo.FindExact(marker);
                if (controller == null) return false;
                marker = " (" + controller.color + controller.name + color + ")";
            }
            
            Entities.GlobalDespawn(this, true);
            Entities.GlobalSpawn(this, true, marker);
            return true;
        }

        #region == DISCONNECTING ==
        
        /// <summary> Disconnects the player from the server, 
        /// with their default logout message shown in chat. </summary>
        public void Disconnect() { LeaveServer(PlayerDB.GetLogoutMessage(this), "disconnected", false); }
        
        /// <summary> Kicks the player from the server,
        /// with the given messages shown in chat and in the disconnect packet. </summary>
        public void Kick(string chatMsg, string discMsg, bool sync = false) {
            LeaveServer(chatMsg, discMsg, true, sync); 
        }

        /// <summary> Kicks the player from the server,
        /// with the given message shown in both chat and in the disconnect packet. </summary>
        public void Kick(string discMsg) { Kick(discMsg, false); }
        public void Kick(string discMsg, bool sync = false) {
            string chatMsg = discMsg;
            if (chatMsg.Length > 0) chatMsg = "(" + chatMsg + ")"; // old format
            LeaveServer(chatMsg, discMsg, true, sync);
        }
        
        /// <summary> Disconnects the players from the server,
        /// with the given messages shown in chat and in the disconnect packet. </summary>
        public void Leave(string chatMsg, string discMsg, bool sync = false) { 
            LeaveServer(chatMsg, discMsg, false, sync); 
        }

        /// <summary> Disconnects the players from the server,
        /// with the same message shown in chat and in the disconnect packet. </summary>        
        public void Leave(string msg) { Leave(msg, false); }       
        public void Leave(string msg, bool sync = false) {
            LeaveServer(msg, msg, false, sync);
        }
        
        [Obsolete("Use Leave() or Kick() instead", true)]
        public void leftGame(string discMsg = "") { Kick(discMsg); }

        bool leftServer = false;
        void LeaveServer(string chatMsg, string discMsg, bool isKick, bool sync = false) {
            if (leftServer || IsSuper) return;
            leftServer = true;
            CriticalTasks.Clear();
            ZoneIn = null;
            
            // Disconnected before sent handshake
            if (name == null) {
                if (Socket != null) Socket.Close();
                Logger.Log(LogType.UserActivity, "{0} disconnected.", ip);
                return;
            }
            
            Server.reviewlist.Remove(name);
            try { 
                if (Socket.Disconnected) {
                    PlayerInfo.Online.Remove(this);
                    return;
                }
                // FlyBuffer.Clear();
                LastAction = DateTime.UtcNow;
                IsAfk = false;
                isFlying = false;
                if (weapon != null) weapon.Disable();
                
                if (chatMsg != null) chatMsg = Colors.Escape(chatMsg);
                discMsg = Colors.Escape(discMsg);
                
                string kickPacketMsg = ChatTokens.Apply(discMsg, this);
                Send(Packet.Kick(kickPacketMsg, hasCP437), sync);
                Socket.Disconnected = true;
                ZoneIn = null;
                if (isKick) TimesBeenKicked++;
                
                if (!loggedIn) {
                    PlayerInfo.Online.Remove(this);
                    string user = truename + " (" + ip + ")";
                    Logger.Log(LogType.UserActivity, "{0} disconnected. ({1})", user, discMsg);
                    return;
                }

                Entities.DespawnEntities(this, false);
                ShowDisconnectInChat(chatMsg, isKick);
                SaveStats();

                PlayerInfo.Online.Remove(this);
                OnPlayerDisconnectEvent.Call(this, discMsg);
                
                level.AutoUnload();
                Dispose();
            } catch (Exception e) { 
                Logger.LogError("Error disconnecting player", e); 
            } finally {
                Socket.Close();
            }
        }
        
        void ShowDisconnectInChat(string chatMsg, bool isKick) {
            if (chatMsg == null) return;
            
            if (!isKick) {
                string leaveMsg = "&c- λFULL &S" + chatMsg;
                if (Server.Config.GuestLeavesNotify || Rank > LevelPermission.Guest) {
                    Chat.MessageFrom(ChatScope.All, this, leaveMsg, null, Chat.FilterVisible(this), !hidden);
                }
                Logger.Log(LogType.UserActivity, "{0} disconnected ({1}&S).", truename, chatMsg);
            } else {
                string leaveMsg = "&c- λFULL &Skicked &S" + chatMsg;
                Chat.MessageFrom(ChatScope.All, this, leaveMsg, null, null, true);
                Logger.Log(LogType.UserActivity, "{0} kicked ({1}&S).", truename, chatMsg);
            }
        }

        public void Dispose() {
            Extras.Clear();
            
            foreach (CopyState cState in CopySlots) { 
                if (cState != null) cState.Clear();
            }
            CopySlots.Clear();
            
            DrawOps.Clear();
            if (spamChecker != null) spamChecker.Clear();
        }

        #endregion
        #region == OTHER ==
        
        [Obsolete("Use PlayerInfo.Online.Items")]
        public static List<Player> players;

        public static bool ValidName(string name) {
            if (name.Length == 0) return false;
            const string valid = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890._+";
            foreach (char c in name) {
                if (valid.IndexOf(c) == -1) return false;
            }
            return true;
        }
        
        internal byte UserType() { return group.Blocks[Block.Bedrock] ? (byte)100 : (byte)0; }

        #endregion

        /// <summary> Returns whether the player is currently allowed to talk. </summary>
        public bool CanSpeak() { 
            return IsConsole || (!muted && !Unverified && (voice || !Server.chatmod));
        }
        
        public bool CheckCanSpeak(string action) {
            if (IsConsole) return true;
            
            if (muted) { 
                Message("Cannot {0} &Swhile muted", action); return false; 
            }
            if (Server.chatmod && !voice) { 
                Message("Cannot {0} &Swhile chat moderation is on without &T/Voice&S", action); return false; 
            }
            if (Unverified) {
                Message("&WYou must first verify with &T/Pass [Password]"); return false;
            }
            return true;
        }
        
        /// <summary> Checks if player is currently unverified, and if so, sends a message informing them </summary>
        public void CheckIsUnverified() {
            if (verifiedPass) return;
            Unverified = Server.Config.verifyadmins && Rank >= Server.Config.VerifyAdminsRank;
            if (!Unverified) return;
            
            if (!Commands.Moderation.CmdPass.HasPassword(name)) {
                Message("&WPlease set your admin verification password with &T/SetPass [password]!");
            } else {
                Message("&WPlease complete admin verification with &T/Pass [password]!");
            }
        }
        
          
        /// <summary> Formats a player name for displaying in chat. </summary>
        public string FormatNick(string name) {
            Player target = PlayerInfo.FindExact(name);
            // TODO: select color from database?
            if (target != null && CanSee(target)) return FormatNick(target);
            return Group.GroupIn(name).Color + name.RemoveLastPlus();
        }

        /// <summary> Formats a player's name for displaying in chat. </summary>        
        public string FormatNick(Player target) {
            if (Ignores.Nicks) return target.color + target.truename;
            return target.color + target.DisplayName;
        }
        
        /// <summary> Blocks calling thread until all 'new map loaded' packets have been sent. </summary>
        public void BlockUntilLoad(int sleep) {
            while (Loading) 
                Thread.Sleep(sleep);
        }
        
        /// <summary> Sends a block change packet to the user containing the current block at the given coordinates. </summary>
        /// <remarks> Vanilla client always assumes block place/delete succeeds, so this is usually used to echo back the
        /// old block. (e.g. insufficient permission to change that block, used as mark for draw operations) </remarks>
        public void RevertBlock(ushort x, ushort y, ushort z) {
            SendBlockchange(x, y, z, level.GetBlock(x, y, z));
        }
   
        public void SetMoney(int amount) {
            money = amount;
            OnMoneyChangedEvent.Call(this);
        }
        
        internal static bool CheckVote(string msg, Player p, string a, string b, ref int totalVotes) {
            if (!(msg.CaselessEq(a) || msg.CaselessEq(b))) return false;
            
            if (p.voted) {
                p.Message("&cYou have already voted!");
            } else {
                totalVotes++;
                p.Message("&aThanks for voting!");
                p.voted = true;
            }
            return true;
        }
        
        public void CheckForMessageSpam() {
            if (spamChecker != null) spamChecker.CheckChatSpam();
        }        
        
        string selTitle;
        readonly object selLock = new object();
        Vec3S32[] selMarks;
        object selState;
        SelectionHandler selCallback;
        SelectionMarkHandler selMarkCallback;
        int selIndex;

        public void MakeSelection(int marks, string title, object state, 
                                  SelectionHandler callback, SelectionMarkHandler markCallback = null) {
            lock (selLock) {
                selMarks = new Vec3S32[marks];
                selTitle = title;
                selState = state;
                selCallback = callback;
                selMarkCallback = markCallback;
                selIndex = 0;
                Blockchange = SelectionBlockChange;
                
                if (title != null) InitSelectionHUD();
                else ResetSelectionHUD();
            }
        }
        
        public void MakeSelection(int marks, object state, SelectionHandler callback) {
            MakeSelection(marks, null, state, callback);
        }
        
        public void ClearSelection() {
            lock (selLock) {
                if (selTitle != null) ResetSelectionHUD();
                selTitle = null;
                selState = null;
                selCallback = null;
                selMarkCallback = null;
                Blockchange = null;
            }
        }
        
        void SelectionBlockChange(Player p, ushort x, ushort y, ushort z, BlockID block) {
            lock (selLock) {
                Blockchange = SelectionBlockChange;
                RevertBlock(x, y, z);
                
                selMarks[selIndex] = new Vec3S32(x, y, z);
                if (selMarkCallback != null) selMarkCallback(p, selMarks, selIndex, selState, block);
                // Mark callback cancelled selection
                if (selCallback == null) return;
                
                selIndex++;
                if (selIndex == 1 && selTitle != null) {
                    SendCpeMessage(CpeMessageType.BottomRight2, "Mark #1" + FormatSelectionMark(selMarks[0]));
                } else if (selIndex == 2 && selTitle != null) {
                    SendCpeMessage(CpeMessageType.BottomRight1, "Mark #2" + FormatSelectionMark(selMarks[1]));
                }
                if (selIndex != selMarks.Length) return;
                
                string title = selTitle;
                object state = selState;
                SelectionMarkHandler markCallback = selMarkCallback;
                SelectionHandler callback = selCallback;
                ClearSelection();

                block = p.BlockBindings[block];
                bool canRepeat = callback(this, selMarks, state, block);
                
                if (canRepeat && staticCommands) {
                    MakeSelection(selIndex, title, state, callback, markCallback);
                }
            }
        }
        
        string FormatSelectionMark(Vec3S32 P) {
            return ": &S(" + P.X + ", " + P.Y + ", " + P.Z + ")";
        }
        
        void InitSelectionHUD() {
            SendCpeMessage(CpeMessageType.BottomRight3, selTitle);
            SendCpeMessage(CpeMessageType.BottomRight2, "Mark #1: &S(Not yet set)");
            string mark2Msg = selMarks.Length >= 2 ? "Mark #2: &S(Not yet set)" : "";
            SendCpeMessage(CpeMessageType.BottomRight1, mark2Msg);
        }
        
        void ResetSelectionHUD() {
            SendCpeMessage(CpeMessageType.BottomRight3, "");
            SendCpeMessage(CpeMessageType.BottomRight2, "");
            SendCpeMessage(CpeMessageType.BottomRight1, "");
        }        
    }
}
