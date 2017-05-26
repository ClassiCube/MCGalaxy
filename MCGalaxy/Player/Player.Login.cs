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
using MCGalaxy.Events;
using MCGalaxy.Games;
using MCGalaxy.Network;
using MCGalaxy.SQL;
using MCGalaxy.Tasks;
using MCGalaxy.Maths;

namespace MCGalaxy {
    public sealed partial class Player : IDisposable {
        
        void HandleLogin(byte[] packet) {
            LastAction = DateTime.UtcNow;
            if (loggedIn) return;
            byte version = packet[1];
            if (version != Server.version) { Leave(null, "Wrong version!", true); return; }
            
            name = NetUtils.ReadString(packet, 2);
            SkinName = name; DisplayName = name; truename = name;
            if (Server.ClassicubeAccountPlus) name += "+";
            
            string mppass = NetUtils.ReadString(packet, 66);
            if (PlayerConnecting != null) PlayerConnecting(this, mppass);
            OnPlayerConnectingEvent.Call(this, mppass);
            if (cancelconnecting) { cancelconnecting = false; return; }
                                   
            isDev = Server.Devs.CaselessContains(truename);
            isMod = Server.Mods.CaselessContains(truename);
            
            byte protocolType = packet[130];
            Loading = true;
            if (disconnected) return;
            
            if (protocolType == 0x42) { hasCpe = true; SendCpeExtensions(); }
            if (protocolType != 0x42) CompleteLoginProcess();
        }
        
        void SendCpeExtensions() {
            Send(Packet.ExtInfo(25), true);
            
            Send(Packet.ExtEntry(CpeExt.EnvMapAppearance, 1), true); // fix for classicube client, doesn't reply if only send EnvMapAppearance with version 2
            Send(Packet.ExtEntry(CpeExt.ClickDistance, 1), true);
            Send(Packet.ExtEntry(CpeExt.CustomBlocks, 1), true);
            
            Send(Packet.ExtEntry(CpeExt.HeldBlock, 1), true);
            Send(Packet.ExtEntry(CpeExt.TextHotkey, 1), true);
            Send(Packet.ExtEntry(CpeExt.EnvColors, 1), true);
            
            Send(Packet.ExtEntry(CpeExt.SelectionCuboid, 1), true);
            Send(Packet.ExtEntry(CpeExt.BlockPermissions, 1), true);
            Send(Packet.ExtEntry(CpeExt.ChangeModel, 1), true);
            
            Send(Packet.ExtEntry(CpeExt.EnvMapAppearance, 2), true);
            Send(Packet.ExtEntry(CpeExt.EnvWeatherType, 1), true);
            Send(Packet.ExtEntry(CpeExt.HackControl, 1), true);
            
            Send(Packet.ExtEntry(CpeExt.EmoteFix, 1), true);
            Send(Packet.ExtEntry(CpeExt.FullCP437, 1), true);
            Send(Packet.ExtEntry(CpeExt.LongerMessages, 1), true);
            
            Send(Packet.ExtEntry(CpeExt.BlockDefinitions, 1), true);
            Send(Packet.ExtEntry(CpeExt.BlockDefinitionsExt, 2), true);
            Send(Packet.ExtEntry(CpeExt.TextColors, 1), true);
            
            Send(Packet.ExtEntry(CpeExt.BulkBlockUpdate, 1), true);
            Send(Packet.ExtEntry(CpeExt.MessageTypes, 1), true);
            Send(Packet.ExtEntry(CpeExt.ExtPlayerList, 2), true);
            
            Send(Packet.ExtEntry(CpeExt.EnvMapAspect, 1), true);
            Send(Packet.ExtEntry(CpeExt.PlayerClick, 1), true);
            Send(Packet.ExtEntry(CpeExt.EntityProperty, 1), true);
            
            Send(Packet.ExtEntry(CpeExt.ExtEntityPositions, 1), true);
        }
        
        void CompleteLoginProcess() {
            lock (PlayerInfo.Online.locker) {
                id = NextFreeId();
                PlayerInfo.Online.Add(this);
            }

            SendMap(null);
            if (disconnected) return;
            loggedIn = true;
            connections.Remove(this);
            RemoveFromPending();
            Server.s.PlayerListUpdate();
            
            timeLogged = DateTime.Now;
            lastLogin = DateTime.Now;
            time = new TimeSpan(0, 0, 0, 1);
            DataTable playerDb = Database.Backend.GetRows("Players", "*", "WHERE Name=@0", name);
            
            if (playerDb.Rows.Count == 0) {
                InitPlayerStats(playerDb);
            } else {
                LoadPlayerStats(playerDb);
            }
            
            Server.Background.QueueOnce(ShowAltsTask, name, TimeSpan.Zero);
            CheckState();
            ZombieStats stats = Server.zombie.LoadZombieStats(name);
            Game.MaxInfected = stats.MaxInfected; Game.TotalInfected = stats.TotalInfected;
            Game.MaxRoundsSurvived = stats.MaxRounds; Game.TotalRoundsSurvived = stats.TotalRounds;
            
            if (!Directory.Exists("players"))
                Directory.CreateDirectory("players");
            PlayerDB.Load(this);
            Game.Team = Team.FindTeam(this);
            SetPrefix();
            playerDb.Dispose();
            LoadCpeData();
            
            if (Server.verifyadmins && group.Permission >= Server.verifyadminsrank)
                adminpen = true;
            if (Server.noEmotes.Contains(name))
                parseEmotes = !Server.parseSmiley;

            LevelPermission adminChatRank = CommandExtraPerms.MinPerm("adminchat", LevelPermission.Admin);
            hidden = group.CanExecute("hide") && Server.hidden.Contains(name);
            if (hidden) SendMessage("&8Reminder: You are still hidden.");
            if (group.Permission >= adminChatRank && Server.adminsjoinsilent) {
                hidden = true; adminchat = true;
            }
            
            if (PlayerConnect != null) PlayerConnect(this);
            OnPlayerConnectEvent.Call(this);
            if (cancellogin) { cancellogin = false; return; }
            
            
            string joinm = "&a+ " + FullName + " %S" + PlayerDB.GetLoginMessage(this);
            if (hidden) joinm = "&8(hidden)" + joinm;
            
            const LevelPermission perm = LevelPermission.Guest;
            if (group.Permission > perm || (Server.guestJoinNotify && group.Permission <= perm)) {
                Chat.MessageGlobal(this, joinm, false, true);
            }

            if (Server.agreetorulesonentry && group.Permission == LevelPermission.Guest && !Server.agreed.Contains(name)) {
                SendMessage("&9You must read the &c/rules&9 and &c/agree&9 to them before you can build and use commands!");
                agreed = false;
            }

            if (Server.verifyadmins && group.Permission >= Server.verifyadminsrank) {
                if (!Directory.Exists("extra/passwords") || !File.Exists("extra/passwords/" + name + ".dat"))
                    SendMessage("&cPlease set your admin verification password with &a/setpass [Password]!");
                else
                    SendMessage("&cPlease complete admin verification with &a/pass [Password]!");
            }
            
           try {
                if (group.commands.Contains("inbox") && Database.TableExists("Inbox" + name) ) {
                    using (DataTable table = Database.Backend.GetRows("Inbox" + name, "*")) {
                        if (table.Rows.Count > 0)
                            SendMessage("You have &a" + table.Rows.Count + " %Smessages in /inbox");
                    }
                }
            } catch {
            }
            
            if (Server.updateTimer.Interval > 1000)
                SendMessage("Lowlag mode is currently &aON.");

            if (String.IsNullOrEmpty(appName)) {
                Server.s.Log(name + " [" + ip + "] connected.");
            } else {
                Server.s.Log(name + " [" + ip + "] connected using " + appName + ".");
            }
            Game.InfectMessages = PlayerDB.GetInfectMessages(this);
            Server.zombie.PlayerJoinedServer(this);
            Server.lava.PlayerJoinedServer(this);
            
            Pos = level.SpawnPos;
            SetYawPitch(level.rotx, level.roty);
            
            Entities.SpawnEntities(this, true);
            PlayerActions.CheckGamesJoin(this, null);
            Loading = false;
        }
        
        void LoadCpeData() {
            string skin = Server.skins.FindData(name);
            if (skin != null) SkinName = skin;
            
            string model = Server.models.FindData(name);
            if (model != null) Model = model;
            ModelBB = AABB.ModelAABB(Model, level);
            
            string rotations = Server.rotations.FindData(name);
            if (rotations == null) return;
            string[] rotParts = rotations.SplitSpaces(2);
            if (rotParts.Length != 2) return;
            
            Orientation rot = Rot;
            byte.TryParse(rotParts[0], out rot.RotX);
            byte.TryParse(rotParts[1], out rot.RotZ);
            Rot = rot;
        }
        
        void InitPlayerStats(DataTable playerDb) {
            SendMessage("Welcome " + DisplayName + "! This is your first visit.");
            PlayerData.Create(this);
        }
        
        void LoadPlayerStats(DataTable playerDb) {
            PlayerData.Load(playerDb, this);
            SendMessage("Welcome back " + FullName + "%S! You've been here " 
                        + totalLogins + " times!");
        }
        
        void CheckState() {
            if (Server.muted.Contains(name)) {
                muted = true;
                Chat.MessageGlobal(this, DisplayName + " &cis still muted from previously.", false);
            }
            
            if (Server.frozen.Contains(name)) {
                frozen = true;
                Chat.MessageGlobal(this, DisplayName + " &cis still frozen from previously.", false);
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
            
            LevelPermission adminChatRank = CommandExtraPerms.MinPerm("adminchat", LevelPermission.Admin);
            string altsMsg = p.ColoredName + " %Sis lately known as: " + alts.Join();
            if (p.group.Permission < adminChatRank || !Server.adminsjoinsilent) {
                Chat.MessageOps(altsMsg);
                //IRCBot.Say(temp, true); //Tells people in op channel on IRC
            }
            Server.s.Log(altsMsg);
        }
    }
}
