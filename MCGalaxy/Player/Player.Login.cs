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
using MCGalaxy.Commands.World;
using MCGalaxy.Games;
using MCGalaxy.SQL;

namespace MCGalaxy {
    public sealed partial class Player : IDisposable {
        
        void HandleLogin(byte[] packet) {
            LastAction = DateTime.UtcNow;
            if (loggedIn) return;
            byte version = packet[1];
            if (version != Server.version) { Leave(null, "Wrong version!", true); return; }
            
            name = NetUtils.ReadString(packet, 2);
            skinName = name; DisplayName = name; truename = name;
            if (Server.ClassicubeAccountPlus) name += "+";
            
            string mppass = NetUtils.ReadString(packet, 66);
            if (PlayerConnecting != null) PlayerConnecting(this, mppass);
            OnPlayerConnectingEvent.Call(this, mppass);
            if (cancelconnecting) { cancelconnecting = false; return; }
                                   
            isDev = Server.Devs.CaselessContains(truename);
            isMod = Server.Mods.CaselessContains(truename);
            
            byte type = packet[130];
            Loading = true;
            if (disconnected) return;
            id = NextFreeId();
            
            if (type == 0x42) { hasCpe = true; SendCpeExtensions(); }
            if (type != 0x42) CompleteLoginProcess();
        }
        
        void SendCpeExtensions() {
            Send(Packet.ExtInfo(22), true);
            
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
        }
        
        void CompleteLoginProcess() {
            LevelPermission adminChatRank = CommandOtherPerms.FindPerm("adminchat", LevelPermission.Admin);
            
            SendUserMOTD();
            SendMap(null);
            if (disconnected) return;
            loggedIn = true;

            PlayerInfo.Online.Add(this);
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
                Player[] players = PlayerInfo.Online.Items;
                foreach (Player pl in players) {
                    if (Entities.CanSee(pl, this)) Player.Message(pl, joinm);
                }
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

            Server.s.Log(name + " [" + ip + "] has joined the server.");
            Game.InfectMessages = PlayerDB.GetInfectMessages(this);
            Server.zombie.PlayerJoinedServer(this);
            
            ushort x = (ushort)(level.spawnx * 32 + 16);
            ushort y = (ushort)(level.spawny * 32 + 32);
            ushort z = (ushort)(level.spawnz * 32 + 16);
            pos = new ushort[3] { x, y, z };
            rot = new byte[2] { level.rotx, level.roty };
            
            Entities.SpawnEntities(this, x, y, z, rot[0], rot[1]);
            PlayerActions.CheckGamesJoin(this, null);
            Loading = false;
        }
        
        void LoadCpeData() {
            string line = Server.skins.Find(name);
            if (line != null) {
                int sep = line.IndexOf(' ');
                if (sep >= 0) skinName = line.Substring(sep + 1);
            }
            
            line = Server.models.Find(name);
            if (line != null) {
                int sep = line.IndexOf(' ');
                if (sep >= 0) model = line.Substring(sep + 1);
            }
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
                Chat.MessageAll("{0} &cis still muted from previously.", DisplayName);
            }
            if (Server.frozen.Contains(name)) {
                frozen = true;
                Chat.MessageAll("{0} &cis still frozen from previously.", DisplayName);
            }
        }
        
        static void ShowAltsTask(SchedulerTask task) {
            string name = (string)task.State;
            Player p = PlayerInfo.FindExact(name);
            if (p == null || p.ip == "127.0.0.1" || p.disconnected) return;
            
            List<string> alts = PlayerInfo.FindAccounts(p.ip);
            // Remove online accounts from the list of accounts on the IP
            for (int i = alts.Count - 1; i >= 0; i--) {
                if (PlayerInfo.FindExact(alts[i]) == null) continue;
                alts.RemoveAt(i);
            }
            if (alts.Count == 0) return;
            
            LevelPermission adminChatRank = CommandOtherPerms.FindPerm("adminchat", LevelPermission.Admin);
            string altsMsg = p.ColoredName + " %Sis lately known as: " + alts.Join();
            if (p.group.Permission < adminChatRank || !Server.adminsjoinsilent) {
                Chat.MessageOps(altsMsg);
                //IRCBot.Say(temp, true); //Tells people in op channel on IRC
            }
            Server.s.Log(altsMsg);
        }
    }
}
