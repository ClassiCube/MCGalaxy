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
using MCGalaxy.Blocks;
using MCGalaxy.Blocks.Physics;
using MCGalaxy.DB;
using MCGalaxy.Commands;
using MCGalaxy.Commands.Chatting;
using MCGalaxy.Events;
using MCGalaxy.Games;
using MCGalaxy.Network;
using MCGalaxy.SQL;
using MCGalaxy.Maths;
using MCGalaxy.Util;

namespace MCGalaxy {
    public partial class Player : IDisposable {
        
        bool removedFromPending = false;
        internal void RemoveFromPending() {
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
        
        public void ManualChange(ushort x, ushort y, ushort z, byte action, ExtBlock block) {
            ManualChange(x, y, z, action, block, true);
        }
        
        public void ManualChange(ushort x, ushort y, ushort z, byte action,
                                 ExtBlock block, bool checkPlaceDist) {
            ExtBlock old = level.GetBlock(x, y, z);
            if (old.IsInvalid) return;
            
            if (jailed || !agreed || !canBuild) { RevertBlock(x, y, z); return; }
            if (level.IsMuseum && Blockchange == null) return;
            
            if (action > 1) {
                const string msg = "Unknown block action!";
                Leave(msg, msg, true); return;
            }
            bool doDelete = !painting && action == 0;

            if (ServerConfig.verifyadmins && adminpen) {
                SendMessage("&cYou must first verify with %T/pass [Password]");
                RevertBlock(x, y, z); return;
            }

            if (Server.zombie.Running && Server.zombie.HandlesManualChange(this, x, y, z, action, block.BlockID, old.BlockID))
                return;

            if ( Server.lava.active && Server.lava.HasPlayer(this) && Server.lava.IsPlayerDead(this) ) {
                SendMessage("You are out of the round, and cannot build.");
                RevertBlock(x, y, z); return;
            }

            lastClick.X = x; lastClick.Y = y; lastClick.Z = z;
            if (Blockchange != null) {
                Blockchange(this, x, y, z, block); return;
            }
            if (PlayerBlockChange != null)
                PlayerBlockChange(this, x, y, z, block);
            OnBlockChangeEvent.Call(this, x, y, z, block);
            if (cancelBlock) { cancelBlock = false; return; }

            if (old.BlockID >= Block.air_flood && old.BlockID <= Block.air_door_air) {
                SendMessage("Block is active, you cannot disturb it.");
                RevertBlock(x, y, z); return;
            }
            
            if (!deleteMode) {
                PhysicsArgs args = level.foundInfo(x, y, z);
                if (args.HasWait) return;
            }

            if (group.Permission == LevelPermission.Banned) return;
            if (checkPlaceDist && group.Permission == LevelPermission.Guest) {
                int dx = Pos.BlockX - x, dy = Pos.BlockY - y, dz = Pos.BlockZ - z;
                int diff = (int)Math.Sqrt(dx * dx + dy * dy + dz * dz);
                
                if (diff > ReachDistance + 4) {
                    Logger.Log(LogType.Warning, "{0} attempted to build with a {1} distance offset", name, diff);
                    SendMessage("You can't build that far away.");
                    RevertBlock(x, y, z); return;
                }
            }

            ExtBlock held = block;
            block = BlockBindings[block.RawID];
            if (!CheckManualChange(old, block, doDelete)) {
                RevertBlock(x, y, z); return;
            }
            if (!ModeBlock.IsAir) block = ModeBlock;
            
            //Ignores updating blocks that are the same and revert block back only to the player
            ExtBlock newB = (painting || action == 1) ? block : ExtBlock.Air;
            if (old == newB) {
                if (painting || !old.VisuallyEquals(held)) RevertBlock(x, y, z);
                return;
            }
            
            if (doDelete) {
                bool deleted = DeleteBlock(old, x, y, z, block);
            } else {
                bool placed = PlaceBlock(old, x, y, z, block);
                // Client always assumes delete succeeds, so we need to echo back the painted over block
                // if the block was not changed visually (e.g. they paint white with door_white)
                if (!placed && painting) RevertBlock(x, y, z);
            }
        }
        
        internal bool CheckManualChange(ExtBlock old, ExtBlock block, bool replaceMode) {
            if (!BlockPerms.CanModify(this, old.BlockID) && !Block.BuildIn(old.BlockID) && !Block.AllowBreak(old.BlockID)) {
                Formatter.MessageBlock(this, replaceMode ? "replace" : "delete", old.BlockID);
                return false;
            }
            return CommandParser.IsBlockAllowed(this, "place", block);
        }
        
        bool DeleteBlock(ExtBlock old, ushort x, ushort y, ushort z, ExtBlock block) {
            if (deleteMode) { return ChangeBlock(x, y, z, ExtBlock.Air) == 2; }

            HandleDelete handler = level.deleteHandlers[old.Index];
            if (handler != null) {
                handler(this, old, x, y, z);
                return true;
            }
            return ChangeBlock(x, y, z, ExtBlock.Air) == 2;
        }

        bool PlaceBlock(ExtBlock old, ushort x, ushort y, ushort z, ExtBlock block) {
            HandlePlace handler = level.placeHandlers[block.Index];
            if (handler != null) {
                handler(this, old, x, y, z);
                return true;
            }
            return ChangeBlock(x, y, z, block) == 2;
        }
        
        /// <summary> Updates the block at the given position, mainly intended for manual changes by the player. </summary>
        /// <remarks> Adds to the BlockDB. Also turns block below to grass/dirt depending on light. </remarks>
        /// <returns> Return code from DoBlockchange </returns>
        public int ChangeBlock(ushort x, ushort y, ushort z, ExtBlock block) {
            ExtBlock old = level.GetBlock(x, y, z);
            int type = level.DoBlockchange(this, x, y, z, block);
            if (type == 0) return type;                                     // no change performed
            if (type == 2) Player.GlobalBlockchange(level, x, y, z, block); // different visually
            
            ushort flags = BlockDBFlags.ManualPlace;
            if (painting && Replacable(old.BlockID)) flags = BlockDBFlags.Painted;
            level.BlockDB.Cache.Add(this, x, y, z, flags, old, block);
            
            bool autoGrass = level.Config.GrassGrow && (level.physics == 0 || level.physics == 5);
            if (!autoGrass) return type;
            
            byte below = level.GetTile(x, (ushort)(y - 1), z);
            if (below == Block.dirt && block.BlockID == Block.air) {
                level.Blockchange(this, x, (ushort)(y - 1), z, (ExtBlock)Block.grass);
            }
            if (below == Block.grass && !level.LightPasses(block)) {
                level.Blockchange(this, x, (ushort)(y - 1), z, (ExtBlock)Block.dirt);
            }
            return type;
        }
        
        
        static bool Replacable(byte block) {
            block = Block.Convert(block);
            return block == Block.air || (block >= Block.water && block <= Block.lavastill);
        }
        
        internal byte[] ProcessReceived(byte[] buffer) {
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
                Logger.LogError(ex);
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
                    return 10 + (hasExtPositions ? 6 : 0);
                case Opcode.Message:
                    if (!loggedIn) goto default;
                    return 66;
                    case Opcode.CpeExtInfo: return 67;
                    case Opcode.CpeExtEntry: return 69;
                    case Opcode.CpeCustomBlockSupportLevel: return 2;
                    case Opcode.CpePlayerClick: return 15;
                    case Opcode.Ping: return 1;
                    case Opcode.CpeTwoWayPing: return 4;

                default:
                    if (!dontmindme) {
                        string msg = "Unhandled message id \"" + buffer[0] + "\"!";
                        Leave(msg, msg, true);
                    }
                    return -1;
            }
        }
        
        void HandlePacket(byte[] buffer) {
            switch (buffer[0]) {
                    case Opcode.Ping: break;
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
                case Opcode.CpePlayerClick:
                    HandlePlayerClicked(buffer); break;
                case Opcode.CpeTwoWayPing:
                    HandleTwoWayPing(buffer); break;
            }
        }

        void HandleBlockchange(byte[] packet) {
            try {
                if (!loggedIn || spamChecker.CheckBlockSpam()) return;
                ushort x = NetUtils.ReadU16(packet, 1);
                ushort y = NetUtils.ReadU16(packet, 3);
                ushort z = NetUtils.ReadU16(packet, 5);
                if (frozen) { RevertBlock(x, y, z); return; }
                
                byte action = packet[7];
                ExtBlock held = ExtBlock.FromRaw(packet[8]);
                RawHeldBlock = held;
                
                if ((action == 0 || held.BlockID == Block.air) && !level.Config.Deletable) {
                    SendMessage("Deleting blocks is disabled in this level.");
                    RevertBlock(x, y, z); return;
                } else if (action == 1 && !level.Config.Buildable) {
                    SendMessage("Placing blocks is disabled in this level.");
                    RevertBlock(x, y, z); return;
                }
                
                if (held.BlockID == Block.custom_block) {
                    if (!hasBlockDefs || level.CustomBlockDefs[held.ExtID] == null) {
                        SendMessage("Invalid block type: " + held.ExtID);
                        RevertBlock(x, y, z); return;
                    }
                }
                ManualChange(x, y, z, action, held);
            } catch ( Exception e ) {
                // Don't ya just love it when the server tattles?
                Chat.MessageOps(DisplayName + " has triggered a block change error");
                Chat.MessageOps(e.GetType().ToString() + ": " + e.Message);
                Logger.LogError(e);
            }
        }
        
        void HandleMovement(byte[] packet) {
            if (!loggedIn || trainGrab || following != "") { CheckBlocks(Pos); return; }
            if (HasCpeExt(CpeExt.HeldBlock)) {
                RawHeldBlock = ExtBlock.FromRaw(packet[1]);
            }
            
            int x, y, z;
            if (hasExtPositions) {
                x = NetUtils.ReadI32(packet, 2);
                y = NetUtils.ReadI32(packet, 6);
                z = NetUtils.ReadI32(packet, 10);
            } else {
                x = NetUtils.ReadI16(packet, 2);
                y = NetUtils.ReadI16(packet, 4);
                z = NetUtils.ReadI16(packet, 6);
            }
            
            int offset = 8 + (hasExtPositions ? 6 : 0);
            byte yaw = packet[offset + 0], pitch = packet[offset + 1];
            Position next = new Position(x, y, z);
            CheckBlocks(next);

            if (Server.zombie.Running && Server.zombie.HandlesMovement(this, next, yaw, pitch))
                return;
            
            if (OnMove != null) OnMove(this, next, yaw, pitch);
            if (PlayerMove != null) PlayerMove(this, next, yaw, pitch);
            OnPlayerMoveEvent.Call(this, next, yaw, pitch);
            if (cancelmove) { cancelmove = false; return; }
            
            Pos = next;
            SetYawPitch(yaw, pitch);
            if (!Moved() || Loading) return;
            if (DateTime.UtcNow < AFKCooldown) return;
            
            LastAction = DateTime.UtcNow;
            if (IsAfk) CmdAfk.ToggleAfk(this, "");
        }
        
        void CheckBlocks(Position pos) {
            try {
                Vec3U16 P = (Vec3U16)pos.BlockCoords;
                AABB bb = ModelBB.OffsetPosition(Pos);
                int index = level.PosToInt(P.X, P.Y, P.Z);
                	
                if (level.Config.SurvivalDeath) {
                    PlayerPhysics.Fall(this, bb);
                    PlayerPhysics.Drown(this, bb);
                }
                
                PlayerPhysics.Walkthrough(this, bb);
                oldIndex = index;
            } catch (Exception ex) {
                Logger.LogError(ex);
            }
        }
        
        bool Moved() { return lastRot.RotY != Rot.RotY || lastRot.HeadX != Rot.HeadX; }

        [Obsolete("Use HandleDeath with ExtBlock attribute")]
        public void HandleDeath(byte b, string customMessage = "", bool explode = false, bool immediate = false) {
            HandleDeath((ExtBlock)b, customMessage, explode, immediate);
        }
        
        public void HandleDeath(ExtBlock block, string customMessage = "", bool explode = false, bool immediate = false) {
            if (OnDeath != null) OnDeath(this, block);
            if (PlayerDeath != null) PlayerDeath(this, block);
            OnPlayerDeathEvent.Call(this, block);
            
            if (Server.lava.active && Server.lava.HasPlayer(this) && Server.lava.IsPlayerDead(this)) return;
            if (!immediate && lastDeath.AddSeconds(2) > DateTime.UtcNow) return;
            if (!level.Config.KillerBlocks || invincible || hidden) return;

            onTrain = false; trainInvincible = false; trainGrab = false;
            ushort x = (ushort)Pos.BlockX, y = (ushort)Pos.BlockY, z = (ushort)Pos.BlockZ;
            
            string deathMsg = level.BlockProps[block.Index].DeathMessage;
            if (deathMsg != null) {
                Chat.MessageLevel(this, deathMsg.Replace("@p", ColoredName), false, level);
            }
            
            if (block.BlockID == Block.rockethead) level.MakeExplosion(x, y, z, 0);
            if (block.BlockID == Block.creeper) level.MakeExplosion(x, y, z, 1);
            if (block.BlockID == Block.rock || block.BlockID == Block.stone) {
                if (explode) level.MakeExplosion(x, y, z, 1);
                if (block.BlockID == Block.rock) {
                    Chat.MessageGlobal(this, ColoredName + "%S" + customMessage, false);
                } else {
                    Chat.MessageLevel(this, ColoredName + "%S" + customMessage, false, level);
                }
            }
            
            if ( Game.team != null && this.level.ctfmode ) {
                //if (carryingFlag)
                //{
                // level.ctfgame.DropFlag(this, hasflag);
                //}
                Game.team.SpawnPlayer(this);
                //this.health = 100;
            } else if ( Server.Countdown.Remaining.Contains(this) ) {
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
                // NOTE: If deaths column is ever increased past 16 bits, remove this clamp
                if (overallDeath > short.MaxValue) overallDeath = short.MaxValue;
            }

            if (ServerConfig.AnnounceDeathCount && (overallDeath > 0 && overallDeath % 10 == 0))
                Chat.MessageLevel(this, ColoredName + " %Shas died &3" + overallDeath + " times", false, level);
            lastDeath = DateTime.UtcNow;
        }

        void HandleChat(byte[] packet) {
            if (!loggedIn) return;
            byte continued = packet[1];
            string text = NetUtils.ReadString(packet, 2);
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
                } else {
                    SendMessage("&cYou already voted!");
                    return;
                }
            }
            // Filter out bad words
            if (ServerConfig.ProfanityFiltering) text = ProfanityFilter.Parse(text);
            
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
                        if ( pl.Red && p.Red ) SendMessage(p.p, "To Team " + Colors.red + "-" + color + name + Colors.red + "- %S" + newtext);
                        if ( pl.Blue && p.Blue ) SendMessage(p.p, "To Team " + Colors.blue + "-" + color + name + Colors.blue + "- %S" + newtext);
                    }
                    
                    Logger.Log(LogType.GameActivity, "[TNT Wars] [TeamChat (" + ( pl.Red ? "Red" : "Blue" ) + ") " + name + " " + newtext);
                    return;
                }
            }

            text = HandleJoker(text);
            if (Chatroom != null) { Chat.MessageChatRoom(this, text, true, Chatroom); return; }

            if (!level.Config.ServerWideChat) {
                Logger.Log(LogType.PlayerChat, "<{0}>[level] {1}", name, text);
                Chat.MessageLevel(this, text, true, level);
            } else {
                Logger.Log(LogType.PlayerChat, "<{0}> {1}", name, text);
                if (OnChat != null) OnChat(this, text);
                if (PlayerChat != null) PlayerChat(this, text);
                OnPlayerChatEvent.Call(this, text);
                if (cancelchat) { cancelchat = false; return; }
                
                if (ServerConfig.ServerWideChat) {
                    SendChatFrom(this, text);
                } else {
                    Chat.MessageLevel(this, text, true, level);
                }
            }
            CheckForMessageSpam();
        }
        
        bool FilterChat(ref string text, byte continued) {
            // handles the /womid client message, which displays the WoM vrersion
            if (text.Truncate(6) == "/womid") {
                string version = (text.Length <= 21 ? text.Substring(text.IndexOf(' ') + 1) : text.Substring(7, 15));
                UsingWom = true;
                return true;
            }
            
            if (HasCpeExt(CpeExt.LongerMessages) && continued != 0) {
                if (text.Length < NetUtils.StringSize) storedMessage = storedMessage + text + " ";
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
                const string msg = "Illegal character in chat message!";
                Leave(msg, msg, true); return true;
            }
            return text.Length == 0;
        }
        
        bool DoCommand(string text) {
            // Typing / repeats last command executed
            if (text == "/") {
                if (lastCMD == "") {
                    Player.Message(this, "Cannot repeat command: You haven't used any commands yet.");
                    return true;
                }
                text = lastCMD;
                Player.Message(this, "Repeating %T/" + lastCMD);
            } else if (text[0] == '/' || text[0] == '!') {
                text = text.Remove(0, 1);
            } else {
                return false;
            }
            
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
        
        string HandleJoker(string text) {
            if (!joker) return text;
            Logger.Log(LogType.PlayerChat, "<JOKER>: {0}: {1}", name, text);
            Chat.MessageOps("%S<&aJ&bO&cK&5E&9R%S>: " + ColoredName + ":&f " + text);

            TextFile jokerFile = TextFile.Files["Joker"];
            jokerFile.EnsureExists();
            
            string[] lines = jokerFile.GetText();
            Random rnd = new Random();
            return lines.Length > 0 ? lines[rnd.Next(lines.Length)] : text;
        }
        
        bool IsHandledMessage(string text) {
            if (Server.voteKickInProgress && text.Length == 1) {
                if (text.CaselessEq("y")) {
                    voteKickChoice = VoteKickChoice.Yes;
                    SendMessage("Thanks for voting!");
                    return true;
                } else if (text.CaselessEq("n")) {
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
                Logger.LogError(e); SendMessage("Command failed.");
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
                Logger.LogError(e); SendMessage("Command failed.");
            }
        }
        
        bool CheckCommand(string cmd) {
            if (cmd == "") { SendMessage("No command entered."); return false; }
            if (ServerConfig.AgreeToRulesOnEntry && !agreed && !(cmd == "agree" || cmd == "rules" || cmd == "disagree")) {
                SendMessage("You must read /rules then agree to them with /agree!"); return false;
            }
            if (jailed) {
                SendMessage("You cannot use any commands while jailed."); return false;
            }
            if (ServerConfig.verifyadmins && adminpen && !(cmd == "pass" || cmd == "setpass")) {
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
            if (byte.TryParse(cmd, out bindIndex) && bindIndex < CmdBindings.Length) {
                if (CmdArgsBindings[bindIndex] == null) { SendMessage("No command is bound to: /" + cmd); return null; }
                cmd = CmdBindings[bindIndex];
                cmdArgs = CmdArgsBindings[bindIndex] + " " + cmdArgs;
                cmdArgs = cmdArgs.TrimEnd(' ');
            }
            
            if (OnCommand != null) OnCommand(cmd, this, cmdArgs);
            if (PlayerCommand != null) PlayerCommand(cmd, this, cmdArgs);
            OnPlayerCommandEvent.Call(cmd, this, cmdArgs);
            if (cancelcommand) { cancelcommand = false; return null; }
            
            Command command = Command.all.Find(cmd);
            if (command == null) {
                if (Block.Byte(cmd) != Block.Invalid) {
                    cmdArgs = cmd.ToLower(); cmd = "mode";
                    command = Command.all.Find("mode");
                } else {
                    Logger.Log(LogType.CommandUsage, "{0} tried to use unknown command: /{1} {2}", name, cmd, cmdArgs);
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
            if (cmd != "pass") Logger.Log(LogType.CommandUsage, "{0} used /{1} {2}", name, cmd, message);

            try { //opstats patch (since 5.5.11)
                if (Server.Opstats.Contains(cmd) || (cmd == "review" && message.ToLower() == "next" && Server.reviewlist.Count > 0)) {
                    Database.Backend.AddRow("Opstats", "Time, Name, Cmd, Cmdmsg",
                                            DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), name, cmd, message);
                }
            } catch { }
            
            try {
                command.Use(this, message);
            } catch (Exception e) {
                Logger.LogError(e);
                Player.Message(this, "An error occured when using the command!");
                Player.Message(this, e.GetType() + ": " + e.Message);
                return false;
            }
            if (spamChecker != null && spamChecker.CheckCommandSpam()) return false;
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
