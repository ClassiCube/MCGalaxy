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
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using MCGalaxy.BlockPhysics;
using MCGalaxy.Commands;
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
            if (action > 1) { Leave("Unknown block action!", true); return; }
            bool doDelete = !painting && action == 0;

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
            if (cancelBlock) { cancelBlock = false; return; }            

            if (old >= Block.air_flood && old <= Block.air_door_air) {
                SendMessage("Block is active, you cannot disturb it.");
                RevertBlock(x, y, z); return;
            }
            
            if (!deleteMode) {
                PhysicsArgs args = level.foundInfo(x, y, z);
                if (args.HasWait) return;
            }

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
                Formatter.MessageBlock(this, doDelete ? "delete " : "replace ", old);
                RevertBlock(x, y, z); return;
            }

            if (!Block.canPlace(this, block)) {
                Formatter.MessageBlock(this, "place ", block);
                RevertBlock(x, y, z); return;
            }

            byte blockRaw = block;
            if (block < Block.CpeCount) block = bindings[block];
            
            //Ignores updating blocks that are the same and send block only to the player
            byte newBlock = (painting || action == 1) ? block : (byte)0;
            if (old == newBlock && (painting || blockRaw != block)) {
                if (old != Block.custom_block || extBlock == level.GetExtTile(x, y, z)) {
                    RevertBlock(x, y, z); return;
                }
            }
            
            byte heldExt = 0;
            byte heldBlock = GetActualHeldBlock(out heldExt);
            int index = level.PosToInt(x, y, z);
            if (doDelete) {
                if (DeleteBlock(old, x, y, z, block, extBlock))
                    level.AddToBlockDB(this, index, heldBlock, heldExt, true);
            } else {
                if (PlaceBlock(old, x, y, z, block, extBlock))
                    level.AddToBlockDB(this, index, heldBlock, heldExt, false);
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

            bool autoDelete = level.GrassGrow && (level.physics == 0 || level.physics == 5);
            if (autoDelete && level.GetTile(x, (ushort)(y - 1), z) == Block.dirt)
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
            
            if (level.GrassGrow && level.GetTile(x, (ushort)(y - 1), z) == Block.grass
                && !Block.LightPass(block, extBlock, level.CustomBlockDefs)) {
                level.Blockchange(this, x, (ushort)(y - 1), z, Block.dirt);
            }
            return true;
        }
        
        byte[] ProcessReceived(byte[] buffer) {
            try {
                int size = PacketSize(buffer);
                if (size == -2) return new byte[1]; // WoM get request
                if (size == -1) return new byte[0]; // invalid packet
                
                if (buffer.Length < size) return buffer;
                HandlePacket(buffer);
                if (buffer.Length == size) return new byte[0];
                
                byte[] remaining = new byte[buffer.Length - size];
                Buffer.BlockCopy(buffer, size, remaining, 0, remaining.Length);
                return ProcessReceived(remaining);
            } catch (Exception ex) {
                Server.ErrorLog(ex);
            }
            return buffer;
        }
        
        int PacketSize(byte[] buffer) {
            switch (buffer[0]) {
                case (byte)'G': return -2; //For wom
                case Opcode.Handshake: return 131;
                case Opcode.SetBlockClient:
                    if (!loggedIn) goto default;
                    return 9;
                case Opcode.EntityTeleport:
                    if (!loggedIn) goto default;
                    return 10;
                case Opcode.Message:
                    if (!loggedIn) goto default;
                    return 66;
                case Opcode.CpeExtInfo: return 67;
                case Opcode.CpeExtEntry: return 69;
                case Opcode.CpeCustomBlockSupportLevel: return 2;
                default:
                    if (!dontmindme) {
                        Leave("Unhandled message id \"" + buffer[0] + "\"!", true);
                    }
                    return -1;
            }
        }
        
        void HandlePacket(byte[] buffer) {
            switch (buffer[0]) {
                case Opcode.Handshake:
                    HandleLogin(buffer); break;
                case Opcode.SetBlockClient:
                    if (!loggedIn) break;
                    HandleBlockchange(buffer); break;
                case Opcode.EntityTeleport:
                    if (!loggedIn) break;
                    HandleMovement(buffer); break;
                case Opcode.Message:
                    if (!loggedIn) break;
                    HandleChat(buffer); break;
                case Opcode.CpeExtInfo:
                    HandleExtInfo(buffer); break;
                case Opcode.CpeExtEntry:
                    HandleExtEntry(buffer); break;
                case Opcode.CpeCustomBlockSupportLevel:
                    customBlockSupportLevel = buffer[1]; break;
            }
        }

        void HandleBlockchange(byte[] packet) {
            try {
                if (!loggedIn || spamChecker.CheckBlockSpam()) return;
                ushort x = NetUtils.ReadU16(packet, 1);
                ushort y = NetUtils.ReadU16(packet, 3);
                ushort z = NetUtils.ReadU16(packet, 5);
                byte action = packet[7], block = packet[8];
                byte extBlock = block;
                RawHeldBlock = block;
                
                if ((action == 0 || block == 0) && !level.Deletable) {
                    SendMessage("Deleting blocks is disabled in this level.");
                    RevertBlock(x, y, z); return;
                } else if (action == 1 && !level.Buildable) {
                    SendMessage("Placing blocks is disabled in this level.");
                    RevertBlock(x, y, z); return;
                }
                
                if (block >= Block.CpeCount) {
                    if (!hasBlockDefs || level.CustomBlockDefs[block] == null) {
                        SendMessage("Invalid block type: " + block);
                        RevertBlock(x, y, z); return;
                    }
                    extBlock = block;
                    block = Block.custom_block;
                }
                ManualChange(x, y, z, action, block, extBlock);
            } catch ( Exception e ) {
                // Don't ya just love it when the server tattles?
                Chat.MessageOps(DisplayName + " has triggered a block change error");
                Chat.MessageOps(e.GetType().ToString() + ": " + e.Message);
                Server.ErrorLog(e);
            }
        }
        
        void HandleMovement(byte[] packet) {
            if (!loggedIn || trainGrab || following != "" || frozen) return;
            /*if (CheckIfInsideBlock())
{
this.SendPos(0xFF, (ushort)(clippos[0] - 18), (ushort)(clippos[1] - 18), (ushort)(clippos[2] - 18), cliprot[0], cliprot[1]);
return;
}*/
            byte heldBlock = packet[1];
            if (HasCpeExt(CpeExt.HeldBlock))
                RawHeldBlock = heldBlock;
            
            ushort x = NetUtils.ReadU16(packet, 2);
            ushort y = NetUtils.ReadU16(packet, 4);
            ushort z = NetUtils.ReadU16(packet, 6);
            byte rotx = packet[8], roty = packet[9];

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
            if (!Moved() || Loading) return;
            
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

            onTrain = false; trainInvincible = false; trainGrab = false;
            ushort x = (ushort)(pos[0] / 32), y = (ushort)(pos[1] / 32), z = (ushort)(pos[2] / 32);
            string deathMsg = Block.Props[b].DeathMessage;
            if (deathMsg != null) Chat.GlobalChatLevel(this, String.Format(deathMsg, ColoredName), false);
            
            if (b == Block.rockethead) level.MakeExplosion(x, y, z, 0);
            if (b == Block.creeper) level.MakeExplosion(x, y, z, 1);
            if (b == Block.rock || b == Block.stone) {
                if (explode) level.MakeExplosion(x, y, z, 1);
                if (b == Block.rock) {
                    SendChatFrom(this, ColoredName + "%S" + customMessage, false);
                } else {
                    Chat.GlobalChatLevel(this, ColoredName + "%S" + customMessage, false);
                }
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

        void HandleChat(byte[] packet) {
            if (!loggedIn) return;
            byte continued = packet[1];
            string text = GetString(packet, 2);
            LastAction = DateTime.UtcNow;
            if (FilterChat(ref text, continued)) return;

            if (text != "/afk" && IsAfk)
                CmdAfk.ToggleAfk(this, "");
            
            // Typing //Command appears in chat as /command
            // Suggested by McMrCat
            if (text.StartsWith("//")) {
                text = text.Remove(0, 1);
            } else if (DoCommand(text)) {
                return;
            }

            // People who are muted can't speak or vote
            if (muted) { SendMessage("You are muted."); return; } //Muted: Only allow commands

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
            if (Server.chatmod && !voice) { SendMessage("Chat moderation is on, you cannot speak."); return; }

            if (ChatModes.Handle(this, text)) return;

            if (text[0] == ':' && PlayingTntWars) {
                string newtext = text.Remove(0, 1).Trim();
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

            text = HandleJoker(text);
            if (Chatroom != null) { Chat.ChatRoom(this, text, true, Chatroom); return; }

            if (!level.worldChat) {
                Server.s.Log("<" + name + ">[level] " + text);
                Chat.GlobalChatLevel(this, text, true);
            } else {
                Server.s.Log("<" + name + "> " + text);
                if (OnChat != null) OnChat(this, text);
                if (PlayerChat != null) PlayerChat(this, text);
                OnPlayerChatEvent.Call(this, text);
                
                if (cancelchat) { cancelchat = false; return; }
                if (Server.worldChat) {
                    SendChatFrom(this, text);
                } else {
                    Chat.GlobalChatLevel(this, text, true);
                }
            }
            CheckForMessageSpam();
        }
        
        bool FilterChat(ref string text, byte continued) {
            // handles the /womid client message, which displays the WoM vrersion
            if (text.Truncate(6) == "/womid") {
                string version = (text.Length <= 21 ? text.Substring(text.IndexOf(' ') + 1) : text.Substring(7, 15));
                Server.s.Log(Colors.red + "[INFO] " + ColoredName + "%f is using wom client");
                Server.s.Log(Colors.red + "[INFO] %fVersion: " + version);
                UsingWom = true;
                return true;
            }
            
            if (HasCpeExt(CpeExt.LongerMessages) && continued != 0) {
                if (text.Length < 64) storedMessage = storedMessage + text + " ";
                else storedMessage = storedMessage + text;
                return true;
            }

            if (text.ToLower().Contains("^detail.user=")) {
                SendMessage("&cYou cannot use WoM detail strings in a chat message.");
                text = text.Replace("^detail.user=", "");
            }

            if (storedMessage != "" && !text.EndsWith(">") && !text.EndsWith("<")) {
                text = storedMessage.Replace("|>|", " ").Replace("|<|", "") + text;
                storedMessage = "";
            }

            if (text.EndsWith(">")) {
                storedMessage += text.Substring(0, text.Length - 1) + "|>|";
                SendMessage(Colors.teal + "Partial message: &f" + storedMessage.Replace("|>|", " ").Replace("|<|", ""));
                return true;
            } else if (text.EndsWith("<")) {
                storedMessage += text.Substring(0, text.Length - 1) + "|<|";
                SendMessage(Colors.teal + "Partial message: &f" + storedMessage.Replace("|<|", "").Replace("|>|", " "));
                return true;
            }

            text = Regex.Replace(text, "  +", " ");
            if (text.IndexOf('&') >= 0) {
                Leave("Illegal character in chat message!", true); return true;
            }
            return text.Length == 0;
        }
        
        bool DoCommand(string text) {
            // Typing / will act as /repeat
            if (text == "/") {
                HandleCommand("repeat", ""); return true;
            } else if (text[0] == '/' || text[0] == '!') {
                text = text.Remove(0, 1);
                int sep = text.IndexOf(' ');
                if (sep == -1) {
                    HandleCommand(text.ToLower(), "");
                } else {
                    string cmd = text.Substring(0, sep).ToLower();
                    string msg = text.Substring(sep + 1);
                    HandleCommand(cmd, msg);
                }
                return true;
            }
            return false;
        }
        
        string HandleJoker(string text) {
            if (!joker) return text;
            if (!File.Exists("text/joker.txt")) {
                File.Create("text/joker.txt").Dispose(); return text;
            }
            Server.s.Log("<JOKER>: " + name + ": " + text);
            Chat.MessageOps("%S<&aJ&bO&cK&5E&9R%S>: " + ColoredName + ":&f " + text);

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
                    voteKickChoice = VoteKickChoice.Yes;
                    SendMessage("Thanks for voting!");
                    return true;
                } else if (text.ToLower() == "n") {
                    voteKickChoice = VoteKickChoice.No;
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
            
            TimeSpan delta = cmdUnblocked - DateTime.UtcNow;
            if (delta.TotalSeconds > 0) {
                int secs = (int)Math.Ceiling(delta.TotalSeconds);
                SendMessage("Blocked from using commands for " +
                            "another " + secs + " seconds"); return false;
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
                lastCMD = message == "" ? cmd : cmd + " " + message;
                lastCmdTime = DateTime.UtcNow;
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
            if (spamChecker.CheckCommandSpam()) return false;
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
