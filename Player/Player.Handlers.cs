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
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using MCGalaxy.BlockPhysics;
using MCGalaxy.Commands;
using MCGalaxy.Commands.World;
using MCGalaxy.Games;
using MCGalaxy.SQL;

namespace MCGalaxy {
    
    public sealed partial class Player : IDisposable {
        
        bool removedFromPending = false;
        void RemoveFromPending() {
            if (removedFromPending) return;
            removedFromPending = true;
            
            lock (pendingLock) {
                for (int i = 0; i < pendingNames.Count; i++) {
                    PendingItem item = pendingNames[i];
                    if (item.Name != truename) continue;
                    pendingNames.RemoveAt(i); return;
                }
            }
        }
        
        public void ManualChange(ushort x, ushort y, ushort z, byte action, byte block, byte extBlock = 0) {
            ManualChange(x, y, z, action, block, extBlock, true);
        }
        
        public void ManualChange(ushort x, ushort y, ushort z, byte action, 
                                 byte block, byte extBlock, bool checkPlaceDist) {
            byte old = level.GetTile(x, y, z);
            if (old == Block.Zero) return;
            if (jailed || !agreed || !canBuild) { RevertBlock(x, y, z); return; }
            if (level.IsMuseum && Blockchange == null) return;

            if (!deleteMode) {
                PhysicsArgs args = level.foundInfo(x, y, z);
                if (args.HasWait) return;
            }

            if (Server.verifyadmins && adminpen) {
                SendMessage("&cYou must first verify with %T/pass [Password]");
                RevertBlock(x, y, z); return;
            }

            if (Server.zombie.Running && Server.zombie.HandlesManualChange(this, x, y, z, action, block, old)) 
                return;

            if ( Server.lava.active && Server.lava.HasPlayer(this) && Server.lava.IsPlayerDead(this) ) {
                SendMessage("You are out of the round, and cannot build.");
                RevertBlock(x, y, z); return;
            }

            lastClick.X = x; lastClick.Y = y; lastClick.Z = z;
            if (Blockchange != null) {
                Blockchange(this, x, y, z, block, extBlock); return;
            }
            if (PlayerBlockChange != null)
                PlayerBlockChange(this, x, y, z, block, extBlock);
            OnBlockChangeEvent.Call(this, x, y, z, block, extBlock);
            if ( cancelBlock ) { cancelBlock = false; return; }

            if (group.Permission == LevelPermission.Banned) return;
            if (checkPlaceDist && group.Permission == LevelPermission.Guest) {
            	int dx = ((short)pos[0] / 32) - x, dy = ((short)pos[1] / 32) - y, dz = ((short)pos[2] / 32) - z;
            	int diff = (int)Math.Sqrt(dx * dx + dy * dy + dz * dz);
                if (diff > ReachDistance + 4) {
                    Server.s.Log(name + " attempted to build with a " + diff + " distance offset");
                    SendMessage("You can't build that far away.");
                    RevertBlock(x, y, z); return;
                }
            }

            if (!Block.canPlace(this, old) && !Block.BuildIn(old) && !Block.AllowBreak(old)) {
                SendMessage("Cannot build here!");
                RevertBlock(x, y, z); return;
            }

            if (!Block.canPlace(this, block)) {
            	Formatter.MessageBlock(this, "place ", block);
                RevertBlock(x, y, z); return;
            }

            if (old >= 200 && old < 220) {
                SendMessage("Block is active, you cant disturb it!");
                RevertBlock(x, y, z); return;
            }

            if (action > 1 ) { Leave("Unknown block action!", true); return; }
            byte blockRaw = block;
            if (block < Block.CpeCount) block = bindings[block];
            
            //Ignores updating blocks that are the same and send block only to the player
            byte newBlock = (painting || action == 1) ? block : (byte)0;
            if (old == newBlock && (painting || blockRaw != block)) {
                if (old != Block.custom_block || extBlock == level.GetExtTile(x, y, z)) {
                    RevertBlock(x, y, z); return;
                }
            }
            
            int index = level.PosToInt(x, y, z);
            if (!painting && action == 0) {
                if (DeleteBlock(old, x, y, z, block, extBlock))
                    level.AddToBlockDB(this, index, block, extBlock, true);
            } else {
                if (PlaceBlock(old, x, y, z, block, extBlock))
                	level.AddToBlockDB(this, index, block, extBlock, false);
            }
        }
        
        bool DeleteBlock(byte old, ushort x, ushort y, ushort z, byte block, byte extBlock) {
            if (deleteMode) { return ChangeBlock(x, y, z, Block.air, 0); }
            bool changed = true;

            Block.HandleDelete handler = Block.deleteHandlers[old];
            if (handler != null) {
                if (handler(this, old, x, y, z)) return false;
            } else {
                changed = ChangeBlock(x, y, z, Block.air, 0);
            }

            if ((level.physics == 0 || level.physics == 5) && level.GetTile(x, (ushort)(y - 1), z) == Block.dirt) 
                ChangeBlock(x, (ushort)(y - 1), z, Block.grass, 0);
            return changed;
        }

        bool PlaceBlock(byte old, ushort x, ushort y, ushort z, byte block, byte extBlock) {
            if (modeType != 0) {
                if (old == modeType) SendBlockchange(x, y, z, old);
                else ChangeBlock(x, y, z, modeType, 0);
                return true;
            }
            
            Block.HandlePlace handler = Block.placeHandlers[block];
            if (handler != null) {
                if (handler(this, old, x, y, z)) return false;
            } else {
                return ChangeBlock(x, y, z, block, extBlock);
            }
            return true;
        }
        
        /// <summary> Updates the block at the given position, also turning the block below to dirt if the block above blocks light. </summary>
        internal bool ChangeBlock(ushort x, ushort y, ushort z, byte block, byte extBlock) {
            if (!level.DoBlockchange(this, x, y, z, block, extBlock)) return false;
            Player.GlobalBlockchange(level, x, y, z, block, extBlock);
            
            if (level.GetTile(x, (ushort)(y - 1), z) == Block.grass && level.GrassDestroy 
                && !Block.LightPass(block, extBlock, level.CustomBlockDefs)) {
                level.Blockchange(this, x, (ushort)(y - 1), z, Block.dirt);
            }
            return true;
        }
        
        byte[] HandleMessage(byte[] buffer) {
            try {
                int length = 0; byte msg = buffer[0];
                // Get the length of the message by checking the first byte
                switch (msg) {
                    //For wom
                    case (byte)'G':
                        return new byte[1];
                    case Opcode.Handshake:
                        length = 130;
                        break;
                    case Opcode.SetBlockClient:
                        if (!loggedIn)
                            goto default;
                        length = 8;
                        break;
                    case Opcode.EntityTeleport:
                        if (!loggedIn)
                            goto default;
                        length = 9;
                        break;
                    case Opcode.Message:
                        if (!loggedIn)
                            goto default;
                        length = 65;
                        break;
                    case Opcode.CpeExtInfo:
                        length = 66;
                        break;
                    case Opcode.CpeExtEntry:
                        length = 68;
                        break;
                    case Opcode.CpeCustomBlockSupportLevel:
                        length = 1;
                        break;
                    default:
                        if (!dontmindme)
                            Leave("Unhandled message id \"" + msg + "\"!", true);
                        else
                            Server.s.Log(Encoding.UTF8.GetString(buffer, 0, buffer.Length));
                        return new byte[0];
                }
                if (buffer.Length > length) {
                    byte[] message = new byte[length];
                    Buffer.BlockCopy(buffer, 1, message, 0, length);

                    byte[] tempbuffer = new byte[buffer.Length - length - 1];
                    Buffer.BlockCopy(buffer, length + 1, tempbuffer, 0, buffer.Length - length - 1);

                    buffer = tempbuffer;

                    switch (msg) {
                        case Opcode.Handshake:
                            HandleLogin(message);
                            break;
                        case Opcode.SetBlockClient:
                            if (!loggedIn) break;
                            HandleBlockchange(message);
                            break;
                        case Opcode.EntityTeleport:
                            if (!loggedIn) break;
                            HandleMovement(message);
                            break;
                        case Opcode.Message:
                            if (!loggedIn) break;
                            HandleChat(message);
                            break;
                        case Opcode.CpeExtInfo:
                            HandleExtInfo( message );
                            break;
                        case Opcode.CpeExtEntry:
                            HandleExtEntry( message );
                            break;
                        case Opcode.CpeCustomBlockSupportLevel:
                            HandleCustomBlockSupportLevel( message );
                            break;
                    }
                    //thread.Start((object)message);
                    if (buffer.Length > 0)
                        buffer = HandleMessage(buffer);
                    else
                        return new byte[0];
                }
            } catch (Exception e) {
                Server.ErrorLog(e);
            }
            return buffer;
        }
        
        #region Login
        
        void HandleLogin(byte[] message)
        {
            LastAction = DateTime.UtcNow;
            try
            {
                if (loggedIn) return;

                byte version = message[0];
                name = enc.GetString(message, 1, 64).Trim();
                if (name.Length > 16) {
                    Leave("Usernames must be 16 characters or less", true); return;
                }
                truename = name;
                skinName = name;
                
                lock (pendingLock) {
                    int altsCount = 0;
                    DateTime now = DateTime.UtcNow;
                    foreach (PendingItem item in pendingNames) {
                        if (item.Name == truename && (now - item.Connected).TotalSeconds <= 60)
                            altsCount++;
                    }
                    pendingNames.Add(new PendingItem(name));
                    
                    if (altsCount > 0) {
                        Leave("Already logged in!", true); return;
                    }
                }

                string verify = enc.GetString(message, 65, 32).Trim();
                verifiedName = false;
                if (Server.verify) {
                    byte[] hash = null;
                    lock (md5Lock)
                        hash = md5.ComputeHash(enc.GetBytes(Server.salt + truename));
                    
                    string hashHex = BitConverter.ToString(hash);
                    if (!verify.CaselessEq(hashHex.Replace("-", ""))) {
                        if (!IPInPrivateRange(ip)) {
                            Leave("Login failed! Try signing in again.", true); return;
                        }
                    } else {
                        verifiedName = true;
                    }
                }
                DisplayName = name;
                if (Server.ClassicubeAccountPlus) name += "+";
                byte type = message[129];

                isDev = Server.Devs.CaselessContains(truename);
                isMod = Server.Mods.CaselessContains(truename);

                try {
                    Server.TempBan tBan = Server.tempBans.Find(tB => tB.name.ToLower() == name.ToLower());
                    if (tBan.expiryTime < DateTime.UtcNow) {
                        Server.tempBans.Remove(tBan);
                    } else {
                        string reason = String.IsNullOrEmpty(tBan.reason) ? "" :
                            " (" + tBan.reason + ")";
                        Kick("You're still temp banned!" + reason, true);
                    }
                } catch { }

                if (!CheckWhitelist()) { Leave("This is a private server!", true); return; }
                Group foundGrp = Group.findPlayerGroup(name);
                
                // ban check
                if (Server.bannedIP.Contains(ip) && (!Server.useWhitelist || !onWhitelist)) {
                    Kick(Server.defaultBanMessage, true);  return;
                }
                
                if (foundGrp.Permission == LevelPermission.Banned) {
                    string[] data = Ban.GetBanData(name);
                    if (data != null) {
                        Kick(Ban.FormatBan(data[0], data[1]), true);
                    } else {
                        Kick(Server.defaultBanMessage, true);
                    }
                    return;
                }

                // maxplayer check
                if (!CheckPlayersCount(foundGrp)) return;
                if (version != Server.version) { Leave("Wrong version!", true); return; }
                
                Player[] players = PlayerInfo.Online.Items;
                foreach (Player p in players) {
                    if (p.name == name)  {
                        if (Server.verify) {
                            p.Leave("Someone logged in as you!"); break;
                        } else { 
                            Leave("Already logged in!", true); return;
                        }
                    }
                }
                
                LoadIgnores();
                if (type == 0x42) {
                    hasCpe = true;
                    SendCpeExtensions();
                }
                
                try { left.Remove(name.ToLower()); }
                catch { }

                group = foundGrp;
                Loading = true;
                if (disconnected) return;
                id = NextFreeId();
                
                if (type != 0x42)
                     CompleteLoginProcess();
            } catch (Exception e) {
                Server.ErrorLog(e);
                Player.GlobalMessage("An error occurred: " + e.Message);
            }
        }
        
        bool CheckPlayersCount(Group foundGrp) {
        	if (Server.vip.Contains(name)) return true;
            
            Player[] online = PlayerInfo.Online.Items;
            if (online.Length >= Server.players && !IPInPrivateRange(ip)) { Leave("Server full!", true); return false; }
            if (foundGrp.Permission > LevelPermission.Guest) return true;
            
            online = PlayerInfo.Online.Items;
            int guests = 0;
            foreach (Player p in online) {
                if (p.Rank <= LevelPermission.Guest) guests++;
            }
            if (guests < Server.maxGuests) return true;
            
            if (Server.guestLimitNotify) Chat.GlobalMessageOps("Guest " + DisplayName + " couldn't log in - too many guests.");
            Server.s.Log("Guest " + name + " couldn't log in - too many guests.");
            Leave("Server has reached max number of guests", true);
            return false;
        }
        
        void SendCpeExtensions() {
            SendExtInfo(21);
            SendExtEntry(CpeExt.ClickDistance, 1);
            SendExtEntry(CpeExt.CustomBlocks, 1);
            SendExtEntry(CpeExt.HeldBlock, 1);
            
            SendExtEntry(CpeExt.TextHotkey, 1);
            SendExtEntry(CpeExt.EnvColors, 1);
            SendExtEntry(CpeExt.SelectionCuboid, 1);
            
            SendExtEntry(CpeExt.BlockPermissions, 1);
            SendExtEntry(CpeExt.ChangeModel, 1);
            SendExtEntry(CpeExt.EnvMapAppearance, 2);
            
            SendExtEntry(CpeExt.EnvWeatherType, 1);
            SendExtEntry(CpeExt.HackControl, 1);
            SendExtEntry(CpeExt.EmoteFix, 1);
            
            SendExtEntry(CpeExt.FullCP437, 1);
            SendExtEntry(CpeExt.LongerMessages, 1);
            SendExtEntry(CpeExt.BlockDefinitions, 1);
            
            SendExtEntry(CpeExt.BlockDefinitionsExt, 2);
            SendExtEntry(CpeExt.TextColors, 1);
            SendExtEntry(CpeExt.BulkBlockUpdate, 1);
            
            SendExtEntry(CpeExt.MessageTypes, 1);
            SendExtEntry(CpeExt.ExtPlayerList, 2);
            SendExtEntry(CpeExt.EnvMapAspect, 1);
        }
        
        bool CheckWhitelist() {
            if (!Server.useWhitelist) return true;            
            if (Server.verify) return Server.whiteList.Contains(name);
            
            // Verify names is off, check if the player is on the same IP.
            return Server.whiteList.Contains(name) && PlayerInfo.FindAccounts(ip).Contains(name);
        }
        
        void CompleteLoginProcess() {
            try {
                SendMotd();
                SendMap(null);
                if (disconnected) return;
                loggedIn = true;

                PlayerInfo.Online.Add(this);
                connections.Remove(this);
                RemoveFromPending();

                Server.s.PlayerListUpdate();

                //Test code to show when people come back with different accounts on the same IP
                string alts = name + " is lately known as:";
                bool found = false;
                if (!ip.StartsWith("127.0.0.")) {
                    foreach (KeyValuePair<string, string> prev in left) {
                        if (prev.Value == ip)
                        {
                            found = true;
                            alts += " " + prev.Key;
                        }
                    }
                    if (found) {
                        if (group.Permission < Server.adminchatperm || !Server.adminsjoinsilent) {
                            Chat.GlobalMessageOps(alts);
                            //IRCBot.Say(temp, true); //Tells people in op channel on IRC
                        }
                        Server.s.Log(alts);
                    }
                }
                CheckOutdatedClient();
            } catch (Exception e) {
                Server.ErrorLog(e);
                Player.GlobalMessage("An error occurred: " + e.Message);
            }
            
            //OpenClassic Client Check
            SendBlockchange(0, 0, 0, 0);            
            timeLogged = DateTime.Now;
            lastLogin = DateTime.Now;
            time = new TimeSpan(0, 0, 0, 1);
            DataTable playerDb = Database.Fill("SELECT * FROM Players WHERE Name=@0", name);
            	
            if (playerDb.Rows.Count == 0)
                InitPlayerStats(playerDb);
            else
                LoadPlayerStats(playerDb);
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
            parseEmotes = !Server.noEmotes.Contains(name);

            hidden = group.CanExecute("hide") && Server.hidden.Contains(name);
            if (hidden) SendMessage("&8Reminder: You are still hidden.");
            if (group.Permission >= Server.adminchatperm && Server.adminsjoinsilent) {
                hidden = true; adminchat = true;
            }
            
            string joinm = "&a+ " + FullName + " %S" + PlayerDB.GetLoginMessage(this);
            if (hidden) joinm = "&8(hidden)" + joinm;
            const LevelPermission perm = LevelPermission.Guest;
            if (group.Permission > perm || (Server.guestJoinNotify && group.Permission <= perm)) {
                Player[] players = PlayerInfo.Online.Items;
                foreach (Player pl in players) {
                    if (Entities.CanSee(pl, this)) Player.Message(pl, joinm);
                }
            }
            
            if (PlayerConnect != null)
                PlayerConnect(this);
            OnPlayerConnectEvent.Call(this);
            
            CheckLoginJailed();
            CheckReviewList();
            if (Server.agreetorulesonentry && group.Permission == LevelPermission.Guest && !Server.agreed.Contains(name)) {
                SendMessage("&9You must read the &c/rules&9 and &c/agree&9 to them before you can build and use commands!");
                agreed = false;
            }

            if (Server.verifyadmins && group.Permission >= Server.verifyadminsrank) {
                if (!Directory.Exists("extra/passwords") || !File.Exists("extra/passwords/" + this.name + ".dat"))
                    SendMessage("&cPlease set your admin verification password with &a/setpass [Password]!");
                else
                    SendMessage("&cPlease complete admin verification with &a/pass [Password]!");
            }
            
            try {
                Waypoints.Load(this);
            } catch (Exception ex) {
                SendMessage("Error loading waypoints!");
                Server.ErrorLog(ex);
            }

            Server.s.Log(name + " [" + ip + "] has joined the server.");
            Game.InfectMessages = PlayerDB.GetInfectMessages(this);
            Server.zombie.PlayerJoinedServer(this);
            try {
                ushort x = (ushort)((0.5 + level.spawnx) * 32);
                ushort y = (ushort)((1 + level.spawny) * 32);
                ushort z = (ushort)((0.5 + level.spawnz) * 32);
                pos = new ushort[3] { x, y, z }; rot = new byte[2] { level.rotx, level.roty };
                Entities.SpawnEntities(this, x, y, z, rot[0], rot[1]);
            } catch (Exception e) {
                Server.ErrorLog(e);
                Server.s.Log("Error spawning player \"" + name + "\"");
            }
            CmdGoto.CheckGamesJoin(this, null);
            Loading = false;
        }
        
        void CheckReviewList() {
            Command cmd = Command.all.Find("review");
            int perm = CommandOtherPerms.GetPerm(cmd, 1);
            
            if ((int)group.Permission < perm || !group.commands.Contains(cmd)) return;
            int count = Server.reviewlist.Count;
            if (count == 0) return;
            
            string suffix = count == 1 ? " player is " : " players are ";
            SendMessage(count + suffix + "waiting for a review. Type %T/review view");
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
        
        void CheckOutdatedClient() {
            if (appName == null || !appName.StartsWith("ClassicalSharp ")) return;
            int spaceIndex = appName.IndexOf(' ');
            string version = appName.Substring(spaceIndex, appName.Length - spaceIndex);
            Version ver;
            try {
                ver = Version.Parse(version);
            } catch {
                return;
            }
            
            if (ver < new Version("0.98.6")) {
                SendMessage("%aYou are using an outdated version of ClassicalSharp.");
                SendMessage("%aYou can click %eCheck for updates %ain the launcher to update. " +
                            "(make sure to close the client first)");
                outdatedClient = true;
            }
        }
        
        void InitPlayerStats(DataTable playerDb) {
            SendMessage("Welcome " + DisplayName + "! This is your first visit.");
            PlayerData.Create(this);
        }
        
        void LoadPlayerStats(DataTable playerDb) {
            PlayerData.Load(playerDb, this);
            SendMessage("Welcome back " + color + prefix + DisplayName + "%S! " +
                        "You've been here " + totalLogins + " times!");
        }
        
        void CheckState() {
            if (Server.muted.Contains(name)) {
                muted = true;
                GlobalMessage(DisplayName + " is still muted from the last time they went offline.");
                Player.Message(this, "!%cYou are still %8muted%c since your last login.");
            }           
            if (Server.frozen.Contains(name)) {
                frozen = true;
                GlobalMessage(DisplayName + " is still frozen from the last time they went offline.");
                Player.Message(this, "!%cYou are still %8frozen%c since your last login.");
            }            
        }
        
        void CheckLoginJailed() {
            string line = Server.jailed.Find(name);
            if (line == null) return;
            int space = line.IndexOf(' ');
            if (space == -1) return;
            string level = line.Substring(space + 1);
            
            try {
                PlayerActions.ChangeMap(this, level);
                Command.all.Find("jail").Use(null, name);
            } catch (Exception ex) {
                Leave("Error occured", true);
                Server.ErrorLog(ex);
            }
        }

        #endregion

        void HandleBlockchange(byte[] message) {
            try {
                if ( !loggedIn ) return;
                if ( CheckBlockSpam() ) return;
                
                ushort x = NetUtils.ReadU16(message, 0);
                ushort y = NetUtils.ReadU16(message, 2);
                ushort z = NetUtils.ReadU16(message, 4);
                byte action = message[6];
                byte type = message[7];
                byte extType = type;
                
                if ((action == 0 || type == 0) && !level.Deletable) {
                    SendMessage("You cannot currently delete blocks in this level.");
                    RevertBlock(x, y, z); return;
                } else if (action == 1 && !level.Buildable) {
                    SendMessage("You cannot currently place blocks in this level.");
                    RevertBlock(x, y, z); return;
                }
                
                if (type >= Block.CpeCount) {
                    if (!hasBlockDefs || level.CustomBlockDefs[type] == null) {
                        SendMessage("Invalid block type: " + type); 
                        RevertBlock(x, y, z); return;
                    }
                    extType = type;
                    type = Block.custom_block;
                }
                ManualChange(x, y, z, action, type, extType);
            } catch ( Exception e ) {
                // Don't ya just love it when the server tattles?
                Chat.GlobalMessageOps(DisplayName + " has triggered a block change error");
                Chat.GlobalMessageOps(e.GetType().ToString() + ": " + e.Message);
                Server.ErrorLog(e);
            }
        }
        
        void HandleMovement(byte[] message) {
            if ( !loggedIn || trainGrab || following != "" || frozen )
                return;
            /*if (CheckIfInsideBlock())
{
this.SendPos(0xFF, (ushort)(clippos[0] - 18), (ushort)(clippos[1] - 18), (ushort)(clippos[2] - 18), cliprot[0], cliprot[1]);
return;
}*/
            byte thisid = message[0];
            ushort x = NetUtils.ReadU16(message, 1);
            ushort y = NetUtils.ReadU16(message, 3);
            ushort z = NetUtils.ReadU16(message, 5);
            byte rotx = message[7], roty = message[8];

            if (Server.Countdown.HandlesMovement(this, x, y, z, rotx, roty))
                return;
            if (Server.zombie.Running && Server.zombie.HandlesMovement(this, x, y, z, rotx, roty))
                return;
            
            if (OnMove != null) OnMove(this, x, y, z);
            if (PlayerMove != null) PlayerMove(this, x, y, z);
            PlayerMoveEvent.Call(this, x, y, z);

            if (OnRotate != null) OnRotate(this, rot);
            if (PlayerRotate != null) PlayerRotate(this, rot);
            PlayerRotateEvent.Call(this, rot);
            
            if (cancelmove) {
                SendPos(0xFF, pos[0], pos[1], pos[2], rot[0], rot[1]); return;
            } 
            
            pos = new ushort[3] { x, y, z };
            rot = new byte[2] { rotx, roty };
            if (!Moved()) return;
            
            LastAction = DateTime.UtcNow;
            if (IsAfk) CmdAfk.ToggleAfk(this, "");
            /*if (!CheckIfInsideBlock()) { clippos = pos; cliprot = rot; }*/
        }

        internal void CheckSurvival(ushort x, ushort y, ushort z) {
            byte bFeet = GetSurvivalBlock(x, (ushort)(y - 2), z);
            byte bHead = GetSurvivalBlock(x, y, z);
            if (level.PosToInt(x, y, z) != oldIndex || y != oldFallY) {
                byte conv = Block.Convert(bFeet);
                if (conv == Block.air) {
                    if (y < oldFallY)
                        fallCount++;
                    else if (y > oldFallY) // flying up, for example
                        fallCount = 0;
                    oldFallY = y;
                    drownCount = 0;
                    return;
                } else if (!(conv == Block.water || conv == Block.waterstill ||
                             conv == Block.lava || conv == Block.lavastill)) {
                    if (fallCount > level.fall)
                        HandleDeath(Block.air, null, false, true);
                    fallCount = 0;
                    drownCount = 0;
                    return;
                }
            }

            switch (Block.Convert(bHead)) {
                case Block.water:
                case Block.waterstill:
                case Block.lava:
                case Block.lavastill:
                    fallCount = 0;
                    drownCount++;
                    if (drownCount > level.drown * (100/3)) {
                        HandleDeath(Block.water);
                        drownCount = 0;
                    }
                    break;
                case Block.air:
                    drownCount = 0;
                    break;
                default:
                    fallCount = 0;
                    drownCount = 0;
                    break;
            }
        }
        
        byte GetSurvivalBlock(ushort x, ushort y, ushort z) {
            if (y >= ushort.MaxValue - 512) return Block.blackrock;
            if (y >= level.Height) return Block.air;
            return level.GetTile(x, y, z);
        }

        internal void CheckBlock(ushort x, ushort y, ushort z) {
            byte bHead = level.GetTile(x, y, z);
            byte bFeet = level.GetTile(x, (ushort)(y - 1), z);

            Block.HandleWalkthrough handler = Block.walkthroughHandlers[bHead];
            if (handler != null && handler(this, bHead, x, y, z)) {
                lastWalkthrough = level.PosToInt(x, y, z); return;
            }
            handler = Block.walkthroughHandlers[bFeet];
            if (handler != null && handler(this, bFeet, x, (ushort)(y - 1), z)) {
                lastWalkthrough = level.PosToInt(x, (ushort)(y - 1), z); return;
            }
            
            lastWalkthrough = level.PosToInt(x, y, z);
            if ( ( bHead == Block.tntexplosion || bFeet == Block.tntexplosion ) && PlayingTntWars ) { }
            else if ( Block.Death(bHead) ) HandleDeath(bHead); 
            else if ( Block.Death(bFeet) ) HandleDeath(bFeet);
        }

        public void HandleDeath(byte b, string customMessage = "", bool explode = false, bool immediate = false) {
            if (OnDeath != null) OnDeath(this, b);
            if (PlayerDeath != null) PlayerDeath(this, b);
            OnPlayerDeathEvent.Call(this, b);
            
            if (Server.lava.active && Server.lava.HasPlayer(this) && Server.lava.IsPlayerDead(this)) return;
            if (!immediate && lastDeath.AddSeconds(2) > DateTime.UtcNow) return;
            if (!level.Killer || invincible || hidden) return;

            onTrain = false; trainGrab = false;
            ushort x = (ushort)(pos[0] / 32), y = (ushort)(pos[1] / 32), z = (ushort)(pos[2] / 32);            
            string deathMsg = Block.Props[b].DeathMessage;
            if (deathMsg != null) Chat.GlobalChatLevel(this, String.Format(deathMsg, ColoredName), false);
            
            if (b == Block.rockethead) level.MakeExplosion(x, y, z, 0);
            if (b == Block.creeper) level.MakeExplosion(x, y, z, 1);
            if (b == Block.rock || b == Block.stone) {
                if (explode) level.MakeExplosion(x, y, z, 1);
                SendChatFrom(this, ColoredName + "%S" + customMessage, false);
            }
            
            if ( Game.team != null && this.level.ctfmode ) {
                //if (carryingFlag)
                //{
                // level.ctfgame.DropFlag(this, hasflag);
                //}
                Game.team.SpawnPlayer(this);
                //this.health = 100;
            } else if ( Server.Countdown.playersleftlist.Contains(this) ) {
                Server.Countdown.Death(this);
                Command.all.Find("spawn").Use(this, "");
            } else if ( PlayingTntWars ) {
                TntWarsKillStreak = 0;
                TntWarsScoreMultiplier = 1f;
            } else if ( Server.lava.active && Server.lava.HasPlayer(this) ) {
                if (!Server.lava.IsPlayerDead(this)) {
                    Server.lava.KillPlayer(this);
                    Command.all.Find("spawn").Use(this, "");
                }
            } else {
                Command.all.Find("spawn").Use(this, "");
                overallDeath++;
            }

            if (Server.deathcount && (overallDeath > 0 && overallDeath % 10 == 0))
                Chat.GlobalChatLevel(this, ColoredName + " %Shas died &3" + overallDeath + " times", false);
            lastDeath = DateTime.UtcNow;
        }

        /* void HandleFly(Player p, ushort x, ushort y, ushort z) {
FlyPos pos;

ushort xx; ushort yy; ushort zz;

TempFly.Clear();

if (!flyGlass) y = (ushort)(y + 1);

for (yy = y; yy >= (ushort)(y - 1); --yy)
for (xx = (ushort)(x - 2); xx <= (ushort)(x + 2); ++xx)
for (zz = (ushort)(z - 2); zz <= (ushort)(z + 2); ++zz)
if (p.level.GetTile(xx, yy, zz) == Block.air) {
pos.x = xx; pos.y = yy; pos.z = zz;
TempFly.Add(pos);
}

FlyBuffer.ForEach(delegate(FlyPos pos2) {
try { if (!TempFly.Contains(pos2)) SendBlockchange(pos2.x, pos2.y, pos2.z, Block.air); } catch { }
});

FlyBuffer.Clear();

TempFly.ForEach(delegate(FlyPos pos3){
FlyBuffer.Add(pos3);
});

if (flyGlass) {
FlyBuffer.ForEach(delegate(FlyPos pos1) {
try { SendBlockchange(pos1.x, pos1.y, pos1.z, Block.glass); } catch { }
});
} else {
FlyBuffer.ForEach(delegate(FlyPos pos1) {
try { SendBlockchange(pos1.x, pos1.y, pos1.z, Block.waterstill); } catch { }
});
}
} */
        DateTime lastSpamReset;
        void HandleChat(byte[] message) {
            try {
                if ( !loggedIn ) return;
                if ((DateTime.UtcNow - lastSpamReset).TotalSeconds > Server.spamcountreset) {
                    lastSpamReset = DateTime.UtcNow;
                    consecutivemessages = 0;
                }
                byte continued = message[0];
                string text = GetString(message, 1);

                // handles the /womid client message, which displays the WoM vrersion
                if ( text.Truncate(6) == "/womid" ) {
                    string version = (text.Length <= 21 ? text.Substring(text.IndexOf(' ') + 1) : text.Substring(7, 15));
                    Server.s.Log(Colors.red + "[INFO] " + color + DisplayName + "%f is using wom client");
                    Server.s.Log(Colors.red + "[INFO] %fVersion: " + version);
                    UsingWom = true;
                    return;
                }
                
                if( HasCpeExt(CpeExt.LongerMessages) && continued != 0 ) {
                    if (text.Length < 64) storedMessage = storedMessage + text + " ";
                    else storedMessage = storedMessage + text;
                    return;
                }

                if (text.ToLower().Contains("^detail.user="))
                {
                    SendMessage("&cYou cannot use WoM detail strings in a chat message.");
                    text = text.Replace("^detail.user=", "");
                }

                if ( storedMessage != "" ) {
                    if ( !text.EndsWith(">") && !text.EndsWith("<") ) {
                        text = storedMessage.Replace("|>|", " ").Replace("|<|", "") + text;
                        storedMessage = "";
                    }
                }
                //if (text.StartsWith(">") || text.StartsWith("<")) return;
                if (text.EndsWith(">")) {
                    storedMessage += text.Substring(0, text.Length - 1) + "|>|";
                    SendMessage(Colors.teal + "Partial message: " + Colors.white + storedMessage.Replace("|>|", " ").Replace("|<|", ""));
                    return;
                }
                if (text.EndsWith("<")) {
                    storedMessage += text.Substring(0, text.Length - 1) + "|<|";
                    SendMessage(Colors.teal + "Partial message: " + Colors.white + storedMessage.Replace("|<|", "").Replace("|>|", " "));
                    return;
                }

                text = Regex.Replace(text, "  +", " ");
                if (text.IndexOf('&') >= 0) {
                    Leave("Illegal character in chat message!", true); return;
                }
                if (text.Length == 0) return;
                LastAction = DateTime.UtcNow;

                if ( text != "/afk" && IsAfk )
                    CmdAfk.ToggleAfk(this, "");
                // Typing //Command appears in chat as /command
                // Suggested by McMrCat
                if ( text.StartsWith("//") ) {
                    text = text.Remove(0, 1);
                    goto hello;
                }
                // Typing / will act as /repeat
                if ( text == "/" ) {
                    HandleCommand("repeat", "");
                    return;
                }
                if ( text[0] == '/' || text[0] == '!' ) {
                    text = text.Remove(0, 1);

                    int pos = text.IndexOf(' ');
                    if ( pos == -1 ) {
                        HandleCommand(text.ToLower(), "");
                        return;
                    }
                    string cmd = text.Substring(0, pos).ToLower();
                    string msg = text.Substring(pos + 1);
                    HandleCommand(cmd, msg);
                    return;
                }
            hello:
                // People who are muted can't speak or vote
                if ( muted ) { this.SendMessage("You are muted."); return; } //Muted: Only allow commands

                // Lava Survival map vote recorder
                if ( Server.lava.HasPlayer(this) && Server.lava.HasVote(text.ToLower()) ) {
                    if ( Server.lava.AddVote(this, text.ToLower()) ) {
                        SendMessage("Your vote for &5" + text.ToLower().Capitalize() + " %Shas been placed. Thanks!");
                        Server.lava.map.ChatLevelOps(name + " voted for &5" + text.ToLower().Capitalize() + "%S.");
                        return;
                    }
                    else {
                        SendMessage("&cYou already voted!");
                        return;
                    }
                }
                // Filter out bad words
                if (Server.profanityFilter) text = ProfanityFilter.Parse(text);
                
                if (IsHandledMessage(text)) return;
                
                // Put this after vote collection so that people can vote even when chat is moderated
                if ( Server.chatmod && !voice ) { this.SendMessage("Chat moderation is on, you cannot speak."); return; }

                if (Server.checkspam) {
                    if (Player.lastMSG == name) {
                        consecutivemessages++;
                    } else {
                        consecutivemessages--;
                    }

                    if (consecutivemessages >= Server.spamcounter) {
                        muteCooldown = Server.mutespamtime;
                        Command.all.Find("mute").Use(null, name);
                        Player.GlobalMessage(color + DisplayName + " %Shas been &0muted &efor spamming!");
                        muteTimer.Elapsed += MuteTimerElapsed;
                        muteTimer.Start();
                        return;
                    }
                }
                Player.lastMSG = this.name;

                if( Chat.HandleModes(this, text) )
                    return;

                if ( text[0] == ':' ) {
                    if ( PlayingTntWars ) {
                        string newtext = text;
                        if ( text[0] == ':' ) newtext = text.Remove(0, 1).Trim();
                        TntWarsGame it = TntWarsGame.GetTntWarsGame(this);
                        if ( it.GameMode == TntWarsGame.TntWarsGameMode.TDM ) {
                            TntWarsGame.player pl = it.FindPlayer(this);
                            foreach ( TntWarsGame.player p in it.Players ) {
                                if ( pl.Red && p.Red ) SendMessage(p.p, "To Team " + Colors.red + "-" + color + name + Colors.red + "- " + Server.DefaultColor + newtext);
                                if ( pl.Blue && p.Blue ) SendMessage(p.p, "To Team " + Colors.blue + "-" + color + name + Colors.blue + "- " + Server.DefaultColor + newtext);
                            }
                            Server.s.Log("[TNT Wars] [TeamChat (" + ( pl.Red ? "Red" : "Blue" ) + ") " + name + " " + newtext);
                            return;
                        }
                    }
                }

                /*if (this.teamchat)
{
if (team == null)
{
Player.Message(this, "You are not on a team.");
return;
}
foreach (Player p in team.players)
{
Player.Message(p, "(" + team.teamstring + ") " + this.color + this.name + ":&f " + text);
}
return;
}*/
                text = HandleJoker(text);
                //chatroom stuff
                if ( this.Chatroom != null ) {
                    Chat.ChatRoom(this, text, true, this.Chatroom);
                    return;
                }

                if ( !level.worldChat ) {
                    Server.s.Log("<" + name + ">[level] " + text);
                    Chat.GlobalChatLevel(this, text, true);
                    return;
                }

                if ( text[0] == '%' ) {
                    string newtext = text;
                    if (!Server.worldChat) {
                        newtext = text.Remove(0, 1).Trim();
                        Chat.GlobalChatLevel(this, newtext, true);
                    } else {
                       SendChatFrom(this, newtext);
                    }
                    Server.s.Log("<" + name + "> " + newtext);
                    //IRCBot.Say("<" + name + "> " + newtext);
                    if (OnChat != null) OnChat(this, text);
                    if (PlayerChat != null) PlayerChat(this, text);
                    OnPlayerChatEvent.Call(this, text);
                    return;
                }
                Server.s.Log("<" + name + "> " + text);
                if (OnChat != null) OnChat(this, text);
                if (PlayerChat != null) PlayerChat(this, text);
                OnPlayerChatEvent.Call(this, text);
                
                if (cancelchat) {
                    cancelchat = false; return;
                }
                if (Server.worldChat) {
                    SendChatFrom(this, text);
                } else {
                    Chat.GlobalChatLevel(this, text, true);
                }

                //IRCBot.Say(name + ": " + text);
            }
            catch ( Exception e ) { Server.ErrorLog(e); Player.GlobalMessage("An error occurred: " + e.Message); }
        }
        
        string HandleJoker(string text) {
            if (!joker) return text;
            if (!File.Exists("text/joker.txt")) {
                File.Create("text/joker.txt").Dispose(); return text;
            }
            Server.s.Log("<JOKER>: " + name + ": " + text);
            Chat.GlobalMessageOps("%S<&aJ&bO&cK&5E&9R%S>: " + ColoredName + ":&f " + text);

            List<string> lines = new List<string>();
            using (StreamReader r = new StreamReader("text/joker.txt")) {
                string line = null;
                while ((line = r.ReadLine()) != null)
                    lines.Add(line);
            }
            Random rnd = new Random();
            return lines.Count > 0 ? lines[rnd.Next(lines.Count)] : text;
        }
        
        bool IsHandledMessage(string text) {
            if (Server.voteKickInProgress && text.Length == 1) {
                if (text.ToLower() == "y") {
                    this.voteKickChoice = VoteKickChoice.Yes;
                    SendMessage("Thanks for voting!");
                    return true;
                } else if (text.ToLower() == "n") {
                    this.voteKickChoice = VoteKickChoice.No;
                    SendMessage("Thanks for voting!");
                    return true;
                }
            }

            if (Server.voting) {
                string test = text.ToLower();
                if (CheckVote(test, this, "y", "yes", ref Server.YesVotes) ||
                    CheckVote(test, this, "n", "no", ref Server.NoVotes)) return true;
                
                if (!voice && (test == "y" || test == "n" || test == "yes" || test == "no")) {
                    SendMessage("Chat moderation is on while voting is on!"); return true;
                }
            }

            if (Server.lava.HandlesChatMessage(this, text)) return true;
            if (Server.zombie.HandlesChatMessage(this, text)) return true;
            return false;
        }
        
        public void HandleCommand(string cmd, string message) {
            cmd = cmd.ToLower();
            try {
                if (!CheckCommand(cmd)) return;
                Command command = GetCommand(ref cmd, ref message);
                if (command == null) return;
                
                Thread thread = new Thread(() => UseCommand(command, message));
                thread.Name = "MCG_Command";
                thread.IsBackground = true;
                thread.Start();
            } catch (Exception e) { 
                Server.ErrorLog(e); SendMessage("Command failed."); 
            }
        }
        
        public void HandleCommands(List<string> cmds) {
            List<string> messages = new List<string>(cmds.Count);
            List<Command> commands = new List<Command>(cmds.Count);
            try {
                foreach (string raw in cmds) {
                    string[] parts = raw.SplitSpaces(2);
                    string cmd = parts[0].ToLower();
                    string message = parts.Length > 1 ? parts[1] : "";
                    
                     if (!CheckCommand(cmd)) return;
                     Command command = GetCommand(ref cmd, ref message);
                     if (command == null) return;
                     
                     messages.Add(message); commands.Add(command);
                }

                Thread thread = new Thread(() => UseCommands(commands, messages));
                thread.Name = "MCG_Command";
                thread.IsBackground = true;
                thread.Start();
            } catch (Exception e) { 
                Server.ErrorLog(e); SendMessage("Command failed."); 
            }
        }
        
        bool CheckCommand(string cmd) {
            if (cmd == "") { SendMessage("No command entered."); return false; }
            if (Server.agreetorulesonentry && !agreed && !(cmd == "agree" || cmd == "rules" || cmd == "disagree")) {
                SendMessage("You must read /rules then agree to them with /agree!"); return false;
            }
            if (jailed) {
                SendMessage("You cannot use any commands while jailed."); return false;
            }
            if (Server.verifyadmins && adminpen && !(cmd == "pass" || cmd == "setpass")) {
                SendMessage("&cYou must verify first with %T/pass [Password]"); return false;
            }

            //DO NOT REMOVE THE TWO COMMANDS BELOW, /PONY AND /RAINBOWDASHLIKESCOOLTHINGS. -EricKilla
            if (cmd == "pony") {
                if (ponycount < 2) {
                    GlobalMessage(color + DisplayName + " %Sjust so happens to be a proud brony! Everyone give " + color + DisplayName + " %Sa brohoof!");
                    ponycount++;
                } else {
                    SendMessage("You have used this command 2 times. You cannot use it anymore! Sorry, Brony!");
                }
                return false;
            } else if (cmd == "rainbowdashlikescoolthings") {
                if (rdcount < 2) {
                    GlobalMessage("&1T&2H&3I&4S &5S&6E&7R&8V&9E&aR &bJ&cU&dS&eT &fG&0O&1T &22&30 &4P&CE&7R&DC&EE&9N&1T &5C&6O&7O&8L&9E&aR&b!");
                    rdcount++;
                } else {
                    SendMessage("You have used this command 2 times. You cannot use it anymore! Sorry, Brony!");
                }
                return false;
            }
            return true;
        }
        
        Command GetCommand(ref string cmd, ref string cmdArgs) {
            Command.Search(ref cmd, ref cmdArgs);
            
            byte bindIndex;
            if (byte.TryParse(cmd, out bindIndex) && bindIndex < 10) {
                if (messageBind[bindIndex] == null) { SendMessage("No command is bound to: /" + cmd); return null; }
                cmd = cmdBind[bindIndex];
                cmdArgs = messageBind[bindIndex] + " " + cmdArgs;
                cmdArgs = cmdArgs.TrimEnd(' ');
            }
            
            if (OnCommand != null) OnCommand(cmd, this, cmdArgs);
            if (PlayerCommand != null) PlayerCommand(cmd, this, cmdArgs);
            OnPlayerCommandEvent.Call(cmd, this, cmdArgs);
            if (cancelcommand) { cancelcommand = false; return null; }
            
            Command command = Command.all.Find(cmd);
            if (command == null) {
                if (Block.Byte(cmd) != Block.Zero) {
                    cmdArgs = cmd.ToLower(); cmd = "mode";
                    command = Command.all.Find("mode");
                } else {
                    SendMessage("Unknown command \"" + cmd + "\"."); return null;
                }
            }

            if (!group.CanExecute(command)) { command.MessageCannotUse(this); return null; }
            string reason = Command.GetDisabledReason(command.Enabled);
            if (reason != null) {
                SendMessage("Command is disabled as " + reason); return null;
            }           
            if (level.IsMuseum && !command.museumUsable ) {
                SendMessage("Cannot use this command while in a museum."); return null;
            }
            return command;
        }
        
        bool UseCommand(Command command, string message) {
            string cmd = command.name;
            if (cmd != "repeat" && cmd != "pass") {
                lastCMD = cmd + " " + message;
                lastCmdTime = DateTime.Now;
            }            
            if (cmd != "pass") Server.s.CommandUsed(name + " used /" + cmd + " " + message);

            try { //opstats patch (since 5.5.11)
                if (Server.opstats.Contains(cmd) || (cmd == "review" && message.ToLower() == "next" && Server.reviewlist.Count > 0)) {
                    Database.Execute("INSERT INTO Opstats (Time, Name, Cmd, Cmdmsg) VALUES (@0, @1, @2, @3)",
                                     DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), name, cmd, message);
                }
            } catch { }
            
            try {
                command.Use(this, message);
            } catch (Exception e) {
                Server.ErrorLog(e);
                Player.Message(this, "An error occured when using the command!");
                Player.Message(this, e.GetType() + ": " + e.Message);
                return false;
            }
            return true;
        }
        
        bool UseCommands(List<Command> commands, List<string> messages) {
            for (int i = 0; i < messages.Count; i++) {
                if (!UseCommand(commands[i], messages[i])) return false;
            }
            return true;
        }
    }
}
