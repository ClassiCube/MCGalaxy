﻿/*
Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCForge)
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
using System.Collections.Generic;
using System.Net;
using MCGalaxy.Authentication;
using MCGalaxy.DB;
using MCGalaxy.Events.PlayerEvents;
using MCGalaxy.Games;
using MCGalaxy.SQL;
using MCGalaxy.Tasks;
using MCGalaxy.Util;

namespace MCGalaxy 
{
    public partial class Player : IDisposable 
    { 
        public bool ProcessLogin(string user, string mppass) {
            LastAction = DateTime.UtcNow;
            name     = user; truename    = user;
            SkinName = user; DisplayName = user;

            // TODO move to ClassicProtocol (SetRawName)
            if (Session.ProtocolVersion > Server.VERSION_0030) {
                Leave(null, "Unsupported protocol version " + Session.ProtocolVersion, true); return false;
            }
            if (user.Length < 1 || user.Length > 16) {
                Leave(null, "Usernames must be between 1 and 16 characters", true); return false;
            }
            if (!user.ContainsAllIn(USERNAME_ALPHABET)) {
                Leave(null, "Invalid player name", true); return false;
            }
            
            if (Server.Config.ClassicubeAccountPlus) name += "+";
            OnPlayerStartConnectingEvent.Call(this, mppass);
            if (cancelconnecting) { cancelconnecting = false; return false; }
                
            // mppass can be used as /pass when it is not used for name authentication            
            if (!verifiedName && NeedsVerification())
                ExtraAuthenticator.Current.AutoVerify(this, mppass);
            
            level   = Server.mainLevel;
            Loading = true;
            // returns false if disconnected during login
            return !Socket.Disconnected;
        }
        
        public void CompleteLoginProcess() {
            Player clone = null;
            OnPlayerFinishConnectingEvent.Call(this);
            if (cancelconnecting) { cancelconnecting = false; return; }
            
            SessionStartTime = DateTime.UtcNow;
            LastLogin = DateTime.Now;
            TotalTime = TimeSpan.FromSeconds(1);
            
            lock (PlayerInfo.Online.locker) {
                // Check if any players online have same name
                clone = FindClone(truename);
                
                // Remove clone from list (hold lock for as short time as possible)
                //  NOTE: check 'Server.Config.VerifyNames' too for LAN/localhost IPs
                if (clone != null && (verifiedName || Server.Config.VerifyNames)) 
                    PlayerInfo.Online.Remove(clone);

                PlayerInfo.Online.Add(this);
            }
            
            if (clone != null && (verifiedName || Server.Config.VerifyNames)) {
                string reason = ip == clone.ip ? "(Reconnecting)" : "(Reconnecting from a different IP)";
                clone.Leave(reason);
            } else if (clone != null) {
                Leave(null, "Already logged in!", true); return;
            }
            
            deathCooldown = DateTime.UtcNow.AddSeconds(2);
            LoadCpeData();

            SendRawMap(null, level);
            if (Socket.Disconnected) return;
            loggedIn = true;

            GetPlayerStats();
            ShowWelcome();
            CheckState();
            
            string nick = PlayerDB.LoadNick(name);
            if (nick != null) DisplayName = nick;
            Game.Team   = Team.TeamIn(this);
            SetPrefix();

            if (Server.noEmotes.Contains(name)) { parseEmotes = !Server.Config.ParseEmotes; }

            hideRank = Rank;
            hidden   = CanUse("Hide") && Server.hidden.Contains(name);
            if (hidden) Message("&8Reminder: You are still hidden.");
            
            if (Chat.AdminchatPerms.UsableBy(this) && Server.Config.AdminsJoinSilently) {
                hidden = true; adminchat = true;                
            }

            OnPlayerConnectEvent.Call(this);
            if (cancellogin) { cancellogin = false; return; }

            Server.Background.QueueOnce(ShowAltsTask, name, TimeSpan.Zero);

            string joinMsg = "&a+ λFULL &S" + PlayerInfo.GetLoginMessage(this);
            if (hidden) joinMsg = "&8(hidden)" + joinMsg;
            
            if (Server.Config.GuestJoinsNotify || Rank > LevelPermission.Guest) {
                Chat.MessageFrom(ChatScope.All, this, joinMsg, null, Chat.FilterVisible(this), !hidden);
            }

            if (Server.Config.AgreeToRulesOnEntry && Rank == LevelPermission.Guest && !Server.agreed.Contains(name)) {
                Message("&9You must read the &c/Rules &9and &c/Agree &9to them before you can build and use commands!");
                agreed = false;
            }
            
            CheckIsUnverified();
            
            if (CanUse("Inbox") && Database.TableExists("Inbox" + name)) {
                int count = Database.CountRows("Inbox" + name);
                if (count > 0) {
                    Message("You have &a" + count + " &Smessages in &T/Inbox");
                }
            }
            
            if (Server.Config.PositionUpdateInterval > 1000)
                Message("Lowlag mode is currently &aON.");

            Logger.Log(LogType.UserActivity, "{0} [{1}] connected using {2}.", truename, IP, Session.ClientName());
            PlayerActions.PostSentMap(this, null, level, false);
            Loading = false;
        }

        static Player FindClone(string name) {
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player pl in players)
            {
                if (pl.truename.CaselessEq(name)) return pl;
            }
            return null;
        }
        
        void ShowWelcome() {
            LastAction = DateTime.UtcNow;
            TextFile welcomeFile = TextFile.Files["Welcome"];
            
            try {
                welcomeFile.EnsureExists();
                string[] welcome = welcomeFile.GetText();
                MessageLines(welcome);
            } catch (Exception ex) {
                Logger.LogError("Error loading welcome text", ex);
            }
        }
        
        void LoadCpeData() {
            string skin = Server.skins.Get(name);
            if (!string.IsNullOrEmpty(skin)) SkinName = skin;
            
            string model = Server.models.Get(name);
            if (!string.IsNullOrEmpty(model)) Model = model;

            string modelScales = Server.modelScales.Get(name);
            if (!string.IsNullOrEmpty(modelScales)) {
                string[] bits = modelScales.SplitSpaces(3);
                NumberUtils.TryParseSingle(bits[0], out ScaleX);
                NumberUtils.TryParseSingle(bits[1], out ScaleY);
                NumberUtils.TryParseSingle(bits[2], out ScaleZ);
            }            

            string rotations = Server.rotations.Get(name);
            if (!string.IsNullOrEmpty(rotations)) {
                string[] bits = rotations.SplitSpaces(2);
                Orientation rot = Rot;
                byte.TryParse(bits[0], out rot.RotX);
                byte.TryParse(bits[1], out rot.RotZ);
                Rot = rot;
            }            
            SetModel(Model);
        }
        
        void GetPlayerStats() {
            PlayerData data = null;
            
            if (verifiedName || Server.Config.VerifyNames) {
                // Existing servers may have multiple records for the same name with differing case
                // So for backwards compatibility, do a case sensitive name lookup
                //   to avoid retrieving player stats from the wrong row
                Database.ReadRows("Players", "*",
                                    record => data = PlayerData.Parse(record),
                                    "WHERE Name=@0", name);
            } else {
                data = PlayerDB.FindData(name);
            }
            
            if (data == null) {
                PlayerData.Create(this);
                Chat.MessageFrom(this, "λNICK &Shas connected for the first time!");
                Message("Welcome " + ColoredName + "&S! This is your first visit.");
            } else {
                data.ApplyTo(this);
                Message("Welcome back " + FullName + "&S! You've been here " + TimesVisited + " times!");
            }
            gotSQLData = true;
        }
        
        void CheckState() {
            if (Server.muted.Contains(name)) {
                muted = true;
                Chat.MessageFrom(this, "λNICK &Wis still muted from previously.");
            }
            
            if (Server.frozen.Contains(name)) {
                frozen = true;
                Chat.MessageFrom(this, "λNICK &Wis still frozen from previously.");
            }
        }
        
        static void ShowAltsTask(SchedulerTask task) {
            string name = (string)task.State;
            Player p    = PlayerInfo.FindExact(name);
            if (p == null || p.Socket.Disconnected) return;
            
            // Server host is exempt from alt listing
            if (IPAddress.IsLoopback(p.IP)) return;
            
            List<string> alts = PlayerInfo.FindAccounts(p.ip);
            // in older versions it was possible for your name to appear multiple times in DB
            while (alts.CaselessRemove(p.name)) { }
            if (alts.Count == 0) return;
            
            ItemPerms opchat = Chat.OpchatPerms;
            string altsMsg = "λNICK &Sis lately known as: " + alts.Join();

            Chat.MessageFrom(p, altsMsg,
                             (pl, obj) => pl.CanSee(p) && opchat.UsableBy(pl));
                         
            //IRCBot.Say(temp, true); //Tells people in op channel on IRC
            altsMsg = altsMsg.Replace("λNICK", name);
            Logger.Log(LogType.UserActivity, altsMsg);
        }
    }
}
