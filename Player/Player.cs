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
using System.Net;
using System.Net.Sockets;
using System.Threading;
using MCGalaxy.Games;
using MCGalaxy.SQL;
using MCGalaxy.Util;

namespace MCGalaxy {
    
    public sealed partial class Player : IDisposable {

        public void IncrementBlockStats(byte block, bool drawn) {
            loginBlocks++;
            overallBlocks++;
            
            if (drawn) TotalDrawn++;
            else if (block == 0) TotalDeleted++;
            else TotalPlaced++;
        }
		
        public static string CheckPlayerStatus(Player p) {
            if ( p.hidden ) return "hidden";
            if ( p.IsAfk ) return "afk";
            return "active";
        }
        
        public void SetPrefix() {
            prefix = Game.Referee ? "&2[Ref] " : "";
            if (group.prefix != "") prefix += "&f" + group.prefix + color;
            Team team = Game.Team;
            prefix += team != null ? "<" + team.Color + team.Name + color + "> " : "";
            
            IGame game = level == null ? null : level.CurrentGame();
            if (game != null) game.AdjustPrefix(this, ref prefix);
            
            bool isOwner = Server.server_owner.CaselessEq(name);
            string viptitle = isDev ? string.Format("{0}[&9Dev{0}] ", color) : 
                isOwner ? string.Format("{0}[&cOwner{0}] ", color) : "";
            prefix = prefix + viptitle;
            prefix = (title == "") ? prefix : prefix + color + "[" + titlecolor + title + color + "] ";
        }
        
        public bool CheckIfInsideBlock() {
            ushort x = (ushort)(pos[0] / 32), y = (ushort)(pos[1] / 32), z = (ushort)(pos[2] / 32);
            byte head = level.GetTile(x, y, z);
            byte feet = level.GetTile(x, (ushort)(y - 1), z);

            return !(Block.Walkthrough(Block.Convert(head)) || head == Block.Zero)
                && !(Block.Walkthrough(Block.Convert(feet)) || feet == Block.Zero);
        }
        static int sessionCounter;

        //This is so that plugin devs can declare a player without needing a socket..
        //They would still have to do p.Dispose()..
        public Player(string playername) { 
            name = playername;
            truename = playername;
            DisplayName = playername;
            SessionID = Interlocked.Increment(ref sessionCounter) & SessionIDMask;
        }

        public Player(Socket s) {
            try {
                socket = s;
                ip = socket.RemoteEndPoint.ToString().Split(':')[0];
                SessionID = Interlocked.Increment(ref sessionCounter) & SessionIDMask;
                Server.s.Log(ip + " connected to the server.");

                for (byte i = 0; i < Block.CpeCount; i++) bindings[i] = i;

                socket.BeginReceive(tempbuffer, 0, tempbuffer.Length, SocketFlags.None, new AsyncCallback(Receive), this);
                InitTimers();
                connections.Add(this);
            }
            catch ( Exception e ) { Leave("Login failed!"); Server.ErrorLog(e); }
        }


        public void save() {
            const string query = "UPDATE Players SET IP=@0, LastLogin=@1, totalLogin=@2, totalDeaths=@3, " +
                "Money=@4, totalBlocks=@5, totalCuboided=@6, totalKicked=@7, TimeSpent=@8 WHERE Name=@9";
            
            if (MySQLSave != null) MySQLSave(this, query);
            OnMySQLSaveEvent.Call(this, query);
            if (cancelmysql) { cancelmysql = false; return; }

            long blocks = PlayerData.BlocksPacked(TotalPlaced, overallBlocks);
            long cuboided = PlayerData.CuboidPacked(TotalDeleted, TotalDrawn);            
            Database.Execute(query, ip, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), 
                             totalLogins, overallDeath, money, blocks, 
                             cuboided, totalKicked, time.ToDBTime(), name);
            
            if (Economy.Enabled && loginMoney != money) {
                Economy.EcoStats ecos = Economy.RetrieveEcoStats(name);
                ecos.money = money;
                Economy.UpdateEcoStats(ecos);
            }
            Server.zombie.SaveZombieStats(this);

            try {
                SaveUndo(this);
            } catch (Exception e) {
                Server.s.Log("Error saving undo data.");
                Server.ErrorLog(e);
            }
        }

        #region == GLOBAL MESSAGES ==
        
        public static void GlobalBlockchange(Level level, int b, byte block, byte extBlock) {
            ushort x, y, z;
            level.IntToPos(b, out x, out y, out z);
            GlobalBlockchange(level, x, y, z, block, extBlock);
        }
        
        public static void GlobalBlockchange(Level level, ushort x, ushort y, ushort z, 
                                             byte block, byte extBlock) {
            Player[] players = PlayerInfo.Online.Items; 
            foreach (Player p in players) { 
                if (p.level == level) p.SendBlockchange(x, y, z, block, extBlock);
            }
        }

        [Obsolete("Use SendChatFrom() instead.")]
        public static void GlobalChat(Player from, string message) { SendChatFrom(from, message, true); }
        [Obsolete("Use SendChatFrom() instead.")]
        public static void GlobalChat(Player from, string message, bool showname) { SendChatFrom(from, message, showname); }
        
        public static void SendChatFrom(Player from, string message) { SendChatFrom(from, message, true); }
        public static void SendChatFrom(Player from, string message, bool showname) {
            if (from == null) return;            
            if (Last50Chat.Count == 50) Last50Chat.RemoveAt(0);
            var chatmessage = new ChatMessage();
            chatmessage.text = message;
            chatmessage.username = from.color + from.name;
            chatmessage.time = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss");
            Last50Chat.Add(chatmessage);
            
            string msg_NT = message, msg_NN = message, msg_NNNT = message;
            if (showname) {
                string msg = ": &f" + message;
                string pre = from.color + from.prefix;     
                message = pre + from.DisplayName + msg; // Titles + Nickname
                msg_NN = pre + from.truename + msg; // Titles + Account name
                
                pre = from.group.prefix == "" ? "" : "&f" + from.group.prefix;
                msg_NT = pre + from.color + from.DisplayName + msg; // Nickname
                msg_NNNT = pre + from.color + from.truename + msg; // Account name
            }
            
            Player[] players = PlayerInfo.Online.Items; 
            foreach (Player p in players) {
                if (p.Chatroom != null || p.listignored.Contains(from.name)) continue;
                if (!p.level.worldChat || (p.ignoreAll && p != from)) continue;
                
                if (p.ignoreNicks && p.ignoreTitles) Player.Message(p, msg_NNNT);
                else if (p.ignoreNicks) Player.Message(p, msg_NN);
                else if (p.ignoreTitles) Player.Message(p, msg_NT);
                else Player.Message(p, message);
            }
        }

        public static List<ChatMessage> Last50Chat = new List<ChatMessage>();
        public static void GlobalMessage(string message) { GlobalMessage(message, false); }
        public static void GlobalMessage(string message, bool global) {
            message = Colors.EscapeColors(message);           
            Player[] players = PlayerInfo.Online.Items; 
            foreach (Player p in players) {
                if (p.ignoreAll || (global && p.ignoreGlobal)) continue;
                
                if (p.level.worldChat && p.Chatroom == null)
                    p.SendMessage(message, !global);
            }
        }
        
        public static void GlobalIRCMessage(string message) {
            message = Colors.EscapeColors(message);            
            Player[] players = PlayerInfo.Online.Items; 
            foreach (Player p in players) {
                if (p.ignoreAll || p.ignoreIRC) continue;
                
                if (p.level.worldChat && p.Chatroom == null)
                    p.SendMessage(message);
            }
        }
        
        public static void GlobalMessage(Player from, string message) {
            if (from == null) GlobalMessage(message, false);
            else SendChatFrom(from, message, false);
        }
        
        
        public static void GlobalSpawn(Player p, bool self, string possession = "") {
           Entities.GlobalSpawn(p, self, possession);
        }
        
        public static void GlobalSpawn(Player p, ushort x, ushort y, ushort z, 
                                       byte rotx, byte roty, bool self, string possession = "") {
            Entities.GlobalSpawn(p, x, y, z, rotx, roty, self, possession);
        }
        
        internal void SpawnEntity(Player p, byte id, ushort x, ushort y, ushort z, 
                                       byte rotx, byte roty, string possession = "") {
            Entities.Spawn(this, p, id, x, y, z, rotx, roty, possession);
        }
        
        internal void DespawnEntity(byte id) { Entities.Despawn(this, id); }
        
        public static void GlobalDespawn(Player p, bool self) { Entities.GlobalDespawn(p, self); }

        public bool MarkPossessed(string marker = "") {
            if (marker != "") {
                Player controller = PlayerInfo.FindExact(marker);
                if (controller == null) return false;
                marker = " (" + controller.color + controller.name + color + ")";
            }
            
            Entities.GlobalDespawn(this, true);
            Entities.GlobalSpawn(this, true, marker);
            return true;
        }

        #endregion
        #region == DISCONNECTING ==
        
        public void Disconnect() { LeaveServer("Disconnected", PlayerDB.GetLogoutMessage(this)); }
        public void Kick(string kickString) { LeaveServer(kickString, null); }
        public void Kick(string kickString, bool sync = false) { LeaveServer(kickString, null, sync); }
        public void Leave(string discMsg, bool sync = false) { LeaveServer(discMsg, discMsg, sync); }
        
        [Obsolete("Use Leave() or Kick() instead")]
        public void leftGame(string kickMsg = "") { LeaveServer(kickMsg, null); }

        bool leftServer = false;
        void LeaveServer(string kickMsg, string discMsg, bool sync = false) {
            if (discMsg != null) discMsg = Colors.EscapeColors(discMsg);
            if (kickMsg != null) kickMsg = Colors.EscapeColors(kickMsg);
            if (leftServer) return;
            leftServer = true;
            
            OnPlayerDisconnectEvent.Call(this, discMsg ?? kickMsg);
            //Umm...fixed?
            if (name == "") {
                if (socket != null) CloseSocket();
                connections.Remove(this);
                SaveUndo(this);
                disconnected = true;
                return;
            }
            
            Server.reviewlist.Remove(name);
            try { 
                if (disconnected) {
                    CloseSocket();
                    connections.Remove(this);
                    return;
                }
                // FlyBuffer.Clear();
                SaveIgnores();
                DisposeTimers();
                LastAction = DateTime.UtcNow;
                IsAfk = false;
                isFlying = false;
                aiming = false;
                
                SendKick(kickMsg, sync);
                disconnected = true;
                if (!loggedIn) {
                    connections.Remove(this);
                    RemoveFromPending();
                    Server.s.Log(ip + " disconnected.");
                    return;
                }

                Server.zombie.PlayerLeftServer(this);
                if ( Game.team != null ) Game.team.RemoveMember(this);
                Server.Countdown.PlayerLeftServer(this);
                TntWarsGame tntwarsgame = TntWarsGame.GetTntWarsGame(this);
                if ( tntwarsgame != null ) {
                    tntwarsgame.Players.Remove(tntwarsgame.FindPlayer(this));
                    tntwarsgame.SendAllPlayersMessage("TNT Wars: " + ColoredName + " %Shas left TNT Wars!");
                }

                Entities.DespawnEntities(this, false);
                if (discMsg != null) {
                    string leavem = "&c- " + FullName + " %S" + discMsg;
                    const LevelPermission perm = LevelPermission.Guest;
                    if (group.Permission > perm || (Server.guestLeaveNotify && group.Permission <= perm)) {
                        Player[] players = PlayerInfo.Online.Items;
                        foreach (Player pl in players) {
                            if (Entities.CanSee(pl, this)) Player.Message(pl, leavem); 
                        }
                    }
                    Server.s.Log(name + " disconnected (" + discMsg + ").");
                } else {
                    totalKicked++;
                    SendChatFrom(this, "&c- " + FullName + " %Skicked (" + kickMsg + "%S).", false);
                    Server.s.Log(name + " kicked (" + kickMsg + ").");
                }

                try { save(); }
                catch ( Exception e ) { Server.ErrorLog(e); }

                PlayerInfo.Online.Remove(this);
                Server.s.PlayerListUpdate();
                if (name != null)
                    left[name.ToLower()] = ip;
                if (PlayerDisconnect != null)
                    PlayerDisconnect(this, discMsg ?? kickMsg);
                if (Server.AutoLoad && level.unload && !level.IsMuseum && IsAloneOnCurrentLevel())
                    level.Unload(true);
                Dispose();
            } catch ( Exception e ) { 
                Server.ErrorLog(e); 
            } finally {
                CloseSocket();
            }
        }
        
        public static void SaveUndo(Player p) {
            try {
                UndoFile.SaveUndo(p);
            } catch (Exception e) { 
                Server.s.Log("Error saving undo data for " + p.name + "!"); Server.ErrorLog(e); 
            }
        }

        public void Dispose() {
            connections.Remove(this);
            RemoveFromPending();
            Extras.Clear();
            if (CopyBuffer != null)
                CopyBuffer.Clear();
            DrawOps.Clear();
            UndoBuffer.Clear();
            spamBlockLog.Clear();
            //spamChatLog.Clear();
            spyChatRooms.Clear();
        }

        public bool IsAloneOnCurrentLevel() {
            lock (PlayerInfo.Online.locker) {
                Player[] players = PlayerInfo.Online.Items;
                foreach (Player p in players) {
                    if (p != this && p.level == level) return false;
                }
                return true;
            }
        }

        #endregion
        #region == OTHER ==
        
        [Obsolete("Use PlayerInfo.Online.Items")]
        public static List<Player> players;
        
        [Obsolete("Use PlayerInfo.Find(name)")]
        public static Player Find(string name) { return PlayerInfo.Find(name); }
        
        [Obsolete("Use PlayerInfo.FindExact(name)")]
        public static Player FindExact(string name) { return PlayerInfo.FindExact(name); }
        
        [Obsolete("Use PlayerInfo.FindNick(name)")]
        public static Player FindNick(string name) { return PlayerInfo.FindNick(null, name); }
        
        unsafe static byte NextFreeId() {
            byte* used = stackalloc byte[256];
            for (int i = 0; i < 256; i++)
                used[i] = 0;

            // Lock to ensure that no two players can end up with the same playerid
            lock (PlayerInfo.Online.locker) {
                Player[] players = PlayerInfo.Online.Items;
                for (int i = 0; i < players.Length; i++) {
                    byte id = players[i].id;
                    used[id] = 1;
                }
            }
            
            for (byte i = 0; i < 255; i++ ) {
                if (used[i] == 0) return i;
            }
            return 1;
        }

        public static bool ValidName(string name) {
            const string valid = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890._+";
            foreach (char c in name) {
                if (valid.IndexOf(c) == -1) return false;
            }
            return true;
        }

        public static int GetBannedCount() {
            Group group = Group.findPerm(LevelPermission.Banned);
            return group == null ? 0 : group.playerList.Count;
        }
        #endregion

        public void BlockUntilLoad(int sleep) {
            while (Loading) 
                Thread.Sleep(sleep);
        }
        public void RevertBlock(ushort x, ushort y, ushort z) {
            byte b = level.GetTile(x, y, z);
            SendBlockchange(x, y, z, b);
        }
        
        bool CheckBlockSpam() {
            if ( spamBlockLog.Count >= spamBlockCount ) {
                DateTime oldestTime = spamBlockLog.Dequeue();
                double spamTimer = DateTime.UtcNow.Subtract(oldestTime).TotalSeconds;
                if ( spamTimer < spamBlockTimer && !ignoreGrief ) {
                    Kick("You were kicked by antigrief system. Slow down.");
                    SendMessage(Colors.red + DisplayName + " was kicked for suspected griefing.");
                    Server.s.Log(name + " was kicked for block spam (" + spamBlockCount + " blocks in " + spamTimer + " seconds)");
                    return true;
                }
            }
            spamBlockLog.Enqueue(DateTime.UtcNow);
            return false;
        }

        public static bool IPInPrivateRange(string ip) {
            //range of 172.16.0.0 - 172.31.255.255
            if (ip.StartsWith("172.") && (int.Parse(ip.Split('.')[1]) >= 16 && int.Parse(ip.Split('.')[1]) <= 31))
                return true;
            return IPAddress.IsLoopback(IPAddress.Parse(ip)) || ip.StartsWith("192.168.") || ip.StartsWith("10.");
            //return IsLocalIpAddress(ip);
        }

        public static bool IsLocalIpAddress(string host) {
            try { // get host IP addresses
                IPAddress[] hostIPs = Dns.GetHostAddresses(host);
                // get local IP addresses
                IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());

                // test if any host IP equals to any local IP or to localhost
                foreach ( IPAddress hostIP in hostIPs ) {
                    // is localhost
                    if ( IPAddress.IsLoopback(hostIP) ) return true;
                    // is local address
                    foreach ( IPAddress localIP in localIPs ) {
                        if ( hostIP.Equals(localIP) ) return true;
                    }
                }
            }
            catch { }
            return false;
        }

        public bool EnoughMoney(int amount) {
            return money >= amount;
        }
        
        public void OnMoneyChanged() {
            if (Server.zombie.Running) Server.zombie.PlayerMoneyChanged(this);
            if (Server.lava.active) Server.lava.PlayerMoneyChanged(this);
        }

        public void TntAtATime() {
            CurrentAmountOfTnt++;
            int delay = 0;

            switch (TntWarsGame.GetTntWarsGame(this).GameDifficulty) {
                case TntWarsGame.TntWarsDifficulty.Easy:
                    delay = 3250; break;
                case TntWarsGame.TntWarsDifficulty.Normal:
                    delay = 2250; break;
                case TntWarsGame.TntWarsDifficulty.Hard:
                case TntWarsGame.TntWarsDifficulty.Extreme:
                    delay = 1250; break;
            }
            Server.MainScheduler.QueueOnce(AllowMoreTntTask, null, 
                                           TimeSpan.FromMilliseconds(delay));
        }
        
        void AllowMoreTntTask(SchedulerTask task) {
            CurrentAmountOfTnt--;
        }
        
        public static bool BlacklistCheck(string name, string foundLevel) {
            string path = "levels/blacklists/" + foundLevel + ".txt";
            if (!File.Exists(path)) { return false; }
            if (File.ReadAllText(path).Contains(name)) { return true; }
            return false;
        }
        
        internal static bool CheckVote(string message, Player p, string a, string b, ref int totalVotes) {
            if (!p.voted && (message == a || message == b)) {
                totalVotes++;
                p.SendMessage(Colors.red + "Thanks for voting!");
                p.voted = true;
                return true;
            }
            return false;
        }
        
        void SaveIgnores() {
            string path = "ranks/ignore/" + name + ".txt";
            if (!File.Exists(path)) return;
            
            try {
                using (StreamWriter w = new StreamWriter(path)) {
                    if (ignoreAll) w.WriteLine("&all");
                    if (ignoreGlobal) w.WriteLine("&global");
                    if (ignoreIRC) w.WriteLine("&irc");
                    if (ignoreTitles) w.WriteLine("&titles");
                    if (ignoreNicks) w.WriteLine("&nicks");
                    
                    foreach (string line in listignored)
                        w.WriteLine(line);
                }
            } catch (Exception ex) {
                Server.ErrorLog(ex);
                Server.s.Log("Failed to save ignored list for player: " + this.name);
            }
        }
        
        void LoadIgnores() {
            string path = "ranks/ignore/" + name + ".txt";
            if (!File.Exists(path)) return;
            
            try {
                string[] lines = File.ReadAllLines(path);
                foreach (string line in lines) {
                    if (line == "&global") ignoreGlobal = true;
                    else if (line == "&all") ignoreAll = true;
                    else if (line == "&irc") ignoreIRC = true;
                    else if (line == "&titles") ignoreTitles = true;
                    else if (line == "&nicks") ignoreNicks = true;
                    else listignored.Add(line);
                }
            } catch (Exception ex) {
                Server.ErrorLog(ex);
                Server.s.Log("Failed to load ignore list for: " + name);
            }
            
            if (ignoreAll || ignoreGlobal || ignoreIRC 
                || ignoreTitles || ignoreNicks || listignored.Count > 0)
                SendMessage("&cType &a/ignore list &cto see who you are still ignoring");
        }
        
        internal void RemoveInvalidUndos() {
            UndoDrawOpEntry[] items = DrawOps.Items;
            for (int i = 0; i < items.Length; i++) {
                if (items[i].End < UndoBuffer.LastClear)
                    DrawOps.Remove(items[i]);
            }
        }
        
        internal static void AddNote(string target, Player who, string type) {
             if (!Server.LogNotes) return;
             string src = who == null ? "(console)" : who.name;
             
             string time = DateTime.UtcNow.ToString("dd/mm/yyyy");
             Server.Notes.Append(target + " " + type + " " + src + " " + time);
        }
        
        internal static void AddNote(string target, Player who, string type, string reason) {
             if (!Server.LogNotes) return;
             string src = who == null ? "(console)" : who.name;
             
             string time = DateTime.UtcNow.ToString("dd/mm/yyyy");
             reason = reason.Replace(" ", "%20");
             Server.Notes.Append(target + " " + type + " " + src + " " + time + " " + reason);
        }
        
        readonly object selLock = new object();
        Vec3S32[] selMarks;
        object selState;
        SelectionHandler selCallback;
        int selIndex;

        public void MakeSelection(int marks, object state, SelectionHandler callback) {
            lock (selLock) {
                selMarks = new Vec3S32[marks];
                selState = state;
                selCallback = callback;
                selIndex = 0;
                Blockchange = SelectionBlockChange;
            }
        }
        
        void SelectionBlockChange(Player p, ushort x, ushort y, ushort z, byte block, byte extBlock) {
            lock (selLock) {
                Blockchange = SelectionBlockChange;
                RevertBlock(x, y, z);
                
                selMarks[selIndex] = new Vec3S32(x, y, z);
                selIndex++;
                if (selIndex != selMarks.Length) return;
                
                Blockchange = null;
                block = block < Block.CpeCount ? p.bindings[block] : block;
                bool canRepeat = selCallback(this, selMarks, selState, block, extBlock);
                if (canRepeat && staticCommands)
                    MakeSelection(selIndex, selState, selCallback);
            }
        }
    }
}
