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
using System.Data;
using System.IO;
using MCGalaxy.Commands;
using MCGalaxy.DB;
using MCGalaxy.Events.PlayerEvents;
using MCGalaxy.Games;
using MCGalaxy.Maths;
using MCGalaxy.Network;
using MCGalaxy.SQL;
using MCGalaxy.Tasks;
using MCGalaxy.Util;

namespace MCGalaxy {
    public partial class Player : IDisposable {
        
        void HandleLogin(byte[] buffer, int offset) {
            LastAction = DateTime.UtcNow;
            if (loggedIn) return;
            byte version = buffer[offset + 1];
            if (version != Server.version) { Leave(null, "Wrong version!", true); return; }
            
            name = NetUtils.ReadString(buffer, offset + 2);
            SkinName = name; DisplayName = name; truename = name;
            if (ServerConfig.ClassicubeAccountPlus) name += "+";
            
            string mppass = NetUtils.ReadString(buffer, offset + 66);
            OnPlayerStartConnectingEvent.Call(this, mppass);
            if (cancelconnecting) { cancelconnecting = false; return; }
            
            hasCpe = buffer[offset + 130] == 0x42 && ServerConfig.EnableCPE;
            level = Server.mainLevel;
            Loading = true;
            if (disconnected) return;
            
            if (hasCpe) { SendCpeExtensions(); } 
            else { CompleteLoginProcess(); }
        }
        
        void SendCpeExtensions() {
            Send(Packet.ExtInfo((byte)(extensions.Length + 1)));
            // fix for classicube client, doesn't reply if only send EnvMapAppearance with version 2
            Send(Packet.ExtEntry(CpeExt.EnvMapAppearance, 1));
            
            foreach (ExtEntry ext in extensions) {
                Send(Packet.ExtEntry(ext.ExtName, ext.ServerExtVersion));
            }
        }
        
        void CompleteLoginProcess() {
            Player clone = null;
            OnPlayerFinishConnectingEvent.Call(this);
            if (cancelconnecting) { cancelconnecting = false; return; }
            
            lock (PlayerInfo.Online.locker) {
                // Check if any players online have same name
                Player[] players = PlayerInfo.Online.Items;
                foreach (Player pl in players) {
                    if (pl.truename == truename) { clone = pl; break; }
                }
                
                // Remove clone from list (hold lock for as short time as possible)
                if (clone != null && ServerConfig.VerifyNames) PlayerInfo.Online.Remove(clone);
                id = NextFreeId();
                PlayerInfo.Online.Add(this);
            }
            
            if (clone != null && ServerConfig.VerifyNames) {
                string reason = ip == clone.ip ? "(Reconnecting)" : "(Reconnecting from a different IP)";
                clone.Leave(reason);
            } else if (clone != null) {
                Leave(null, "Already logged in!", true); return;
            }

            SendMap(null);
            if (disconnected) return;
            loggedIn = true;
            pending.Remove(this);

            SessionStartTime = DateTime.UtcNow;
            LastLogin = DateTime.Now;
            TotalTime = TimeSpan.FromSeconds(1);
            GetPlayerStats();
            ShowWelcome();
            
            Server.Background.QueueOnce(ShowAltsTask, name, TimeSpan.Zero);
            CheckState();
            
            if (!Directory.Exists("players"))
                Directory.CreateDirectory("players");
            PlayerDB.Load(this);
            Game.Team = Team.TeamIn(this);
            SetPrefix();
            LoadCpeData();
            
            if (ServerConfig.verifyadmins && Rank >= ServerConfig.VerifyAdminsRank) adminpen = true;
            if (Server.noEmotes.Contains(name)) { parseEmotes = !ServerConfig.ParseEmotes; }

            hidden = group.CanExecute("Hide") && Server.hidden.Contains(name);
            if (hidden) SendMessage("&8Reminder: You are still hidden.");
            
            if (Chat.AdminchatPerms.UsableBy(Rank) && ServerConfig.AdminsJoinSilently) {
                hidden = true; adminchat = true;
            }
            
            OnPlayerConnectEvent.Call(this);
            if (cancellogin) { cancellogin = false; return; }
            
            string joinm = "&a+ λFULL %S" + PlayerDB.GetLoginMessage(this);
            if (hidden) joinm = "&8(hidden)" + joinm;
            
            if (ServerConfig.GuestJoinsNotify || Rank > LevelPermission.Guest) {
                Chat.MessageFrom(this, joinm, Chat.FilterVisible(this), !hidden);
            }

            if (ServerConfig.AgreeToRulesOnEntry && Rank == LevelPermission.Guest && !Server.agreed.Contains(name)) {
                SendMessage("&9You must read the &c/Rules &9and &c/Agree &9to them before you can build and use commands!");
                agreed = false;
            }

            if (ServerConfig.verifyadmins && Rank >= ServerConfig.VerifyAdminsRank) {
                if (!Directory.Exists("extra/passwords") || !File.Exists("extra/passwords/" + name + ".dat"))
                    SendMessage("%WPlease set your admin verification password with %T/SetPass [Password]!");
                else
                    SendMessage("%Wcomplete admin verification with %T/Pass [Password]!");
            }
            
            if (group.CanExecute("Inbox") && Database.TableExists("Inbox" + name)) {
                int count = Database.CountRows("Inbox" + name);
                if (count > 0) {
                    SendMessage("You have &a" + count + " %Smessages in %T/Inbox");
                }
            }
            
            if (ServerConfig.PositionUpdateInterval > 1000)
                SendMessage("Lowlag mode is currently &aON.");

            if (String.IsNullOrEmpty(appName)) {
                Logger.Log(LogType.UserActivity, "{0} [{1}] connected.", name, ip);
            } else {
                Logger.Log(LogType.UserActivity, "{0} [{1}] connected using {2}.", name, ip, appName);
            }
            
            PlayerActions.PostSentMap(this, null, level, false);
            Loading = false;
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
        
        unsafe static byte NextFreeId() {
            byte* used = stackalloc byte[256];
            for (int i = 0; i < 256; i++) used[i] = 0;

            Player[] players = PlayerInfo.Online.Items;
            for (int i = 0; i < players.Length; i++) {
                byte id = players[i].id;
                used[id] = 1;
            }
            
            for (byte i = 0; i < 255; i++ ) {
                if (used[i] == 0) return i;
            }
            return 1;
        }
        
        void LoadCpeData() {
            string skin = Server.skins.FindData(name);
            if (skin != null) SkinName = skin;           
            string model = Server.models.FindData(name);
            if (model != null) Model = model;

            string modelScales = Server.modelScales.FindData(name);
            if (modelScales != null) {
                string[] bits = modelScales.SplitSpaces(3);
                Utils.TryParseDecimal(bits[0], out ScaleX);
                Utils.TryParseDecimal(bits[1], out ScaleY);
                Utils.TryParseDecimal(bits[2], out ScaleZ);
            }            

            string rotations = Server.rotations.FindData(name);
            if (rotations != null) {
                string[] bits = rotations.SplitSpaces(2);
                Orientation rot = Rot;
                byte.TryParse(bits[0], out rot.RotX);
                byte.TryParse(bits[1], out rot.RotZ);
                Rot = rot;
            }            
            SetModel(Model, level);
        }
        
        void GetPlayerStats() {
            object raw = Database.Backend.ReadRows("Players", "*",
                                                   null, PlayerData.Read,
                                                   "WHERE Name=@0", name);
            if (raw == null) {
                PlayerData.Create(this);
                Chat.MessageFrom(this, "λNICK %Shas connected for the first time!");
                SendMessage("Welcome " + ColoredName + "%S! This is your first visit.");
            } else {
                PlayerData.Apply((PlayerData)raw, this);
                SendMessage("Welcome back " + FullName + "%S! You've been here " + TimesVisited + " times!");
            }
            gotSQLData = true;
        }
        
        void CheckState() {
            if (Server.muted.Contains(name)) {
                muted = true;
                Chat.MessageFrom(this, "λNICK %Wis still muted from previously.");
            }
            
            if (Server.frozen.Contains(name)) {
                frozen = true;
                Chat.MessageFrom(this, "λNICK %Wis still frozen from previously.");
            }
        }
        
        static void ShowAltsTask(SchedulerTask task) {
            string name = (string)task.State;
            Player p = PlayerInfo.FindExact(name);
            if (p == null || p.ip == "127.0.0.1" || p.disconnected) return;
            
            List<string> alts = PlayerInfo.FindAccounts(p.ip);
            // in older versions it was possible for your name to appear multiple times in DB
            while (alts.CaselessRemove(p.name)) { }
            if (alts.Count == 0) return;
            
            ItemPerms opchat = Chat.OpchatPerms;
            string altsMsg = "λNICK %Sis lately known as: " + alts.Join();

            Chat.MessageFrom(p, altsMsg,
                             (pl, obj) => Entities.CanSee(pl, p) && opchat.UsableBy(pl.Rank));
                         
            //IRCBot.Say(temp, true); //Tells people in op channel on IRC
            altsMsg = altsMsg.Replace("λNICK", name);
            Logger.Log(LogType.UserActivity, altsMsg);
        }
    }
}
