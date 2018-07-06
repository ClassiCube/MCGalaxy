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
using System.Text.RegularExpressions;
using System.Threading;
using MCGalaxy.Blocks;
using MCGalaxy.Blocks.Physics;
using MCGalaxy.Commands;
using MCGalaxy.Commands.Chatting;
using MCGalaxy.DB;
using MCGalaxy.Events.PlayerEvents;
using MCGalaxy.Games;
using MCGalaxy.Maths;
using MCGalaxy.Network;
using MCGalaxy.SQL;
using MCGalaxy.Util;
using BlockID = System.UInt16;
using BlockRaw = System.Byte;

namespace MCGalaxy {
    public partial class Player : IDisposable {
        const string mustAgreeMsg = "You must read /rules then agree to them with /agree!";
        
        internal bool HasBlockChange() { return Blockchange != null; }
        internal bool DoBlockchangeCallback(ushort x, ushort y, ushort z, BlockID block) {
            lastClick.X = x; lastClick.Y = y; lastClick.Z = z;
            if (Blockchange == null) return false;
            
            Blockchange(this, x, y, z, block);
            return true;
        }

        public void ManualChange(ushort x, ushort y, ushort z, bool placing,
                                 BlockID block, bool checkPlaceDist) {
            BlockID old = level.GetBlock(x, y, z);
            if (old == Block.Invalid) return;
            
            if (jailed || frozen || !canBuild) { RevertBlock(x, y, z); return; }
            if (!agreed) {
                SendMessage(mustAgreeMsg);
                RevertBlock(x, y, z); return;
            }
            
            if (level.IsMuseum && Blockchange == null) return;
            bool deletingBlock = !painting && !placing;

            if (ServerConfig.verifyadmins && adminpen) {
                SendMessage("&cYou must first verify with %T/Pass [Password]");
                RevertBlock(x, y, z); return;
            }

            if ( LSGame.Instance.Running && LSGame.Instance.Map == level && LSGame.Instance.IsPlayerDead(this) ) {
                SendMessage("You are out of the round, and cannot build.");
                RevertBlock(x, y, z); return;
            }

            if (ClickToMark && DoBlockchangeCallback(x, y, z, block)) return;
            
            OnBlockChangeEvent.Call(this, x, y, z, block, placing);
            if (cancelBlock) { cancelBlock = false; return; }

            if (old >= Block.Air_Flood && old <= Block.Door_Air_air) {
                SendMessage("Block is active, you cannot disturb it.");
                RevertBlock(x, y, z); return;
            }
            
            if (!deletingBlock) {
                PhysicsArgs args = level.foundInfo(x, y, z);
                if (args.HasWait) return;
            }

            if (Rank == LevelPermission.Banned) return;
            if (checkPlaceDist) {
                int dx = Pos.BlockX - x, dy = Pos.BlockY - y, dz = Pos.BlockZ - z;
                int diff = (int)Math.Sqrt(dx * dx + dy * dy + dz * dz);
                
                if (diff > ReachDistance + 4) {
                    Logger.Log(LogType.Warning, "{0} attempted to build with a {1} distance offset", name, diff);
                    SendMessage("You can't build that far away.");
                    RevertBlock(x, y, z); return;
                }
            }

            BlockID held = block;
            block = BlockBindings[block];
            if (!CheckManualChange(old, block, deletingBlock)) {
                RevertBlock(x, y, z); return;
            }
            if (ModeBlock != Block.Air) block = ModeBlock;
            
            // Ignores updating blocks that are the same and revert block back only to the player
            BlockID newB = deletingBlock ? Block.Air : block;
            if (old == newB) {
                if (painting || !Block.VisuallyEquals(old, held)) RevertBlock(x, y, z);
                return;
            }
            
            if (deletingBlock) {
                bool deleted = DeleteBlock(old, x, y, z, block);
            } else {
                bool placed = PlaceBlock(old, x, y, z, block);
                // Client always assumes delete succeeds, so we need to echo back the painted over block
                // if the block was not changed visually (e.g. they paint white with door_white)
                if (!placed && painting) RevertBlock(x, y, z);
            }
        }
        
        internal bool CheckManualChange(BlockID old, BlockID block, bool deleteMode) {
            if (!group.Blocks[old] && !level.BuildIn(old) && !Block.AllowBreak(old)) {
                string action = deleteMode ? "delete" : "replace";
                BlockPerms.Find(old).MessageCannotUse(this, action);
                return false;
            }
            return CommandParser.IsBlockAllowed(this, "place", block);
        }
        
        bool DeleteBlock(BlockID old, ushort x, ushort y, ushort z, BlockID block) {
            if (deleteMode) { return ChangeBlock(x, y, z, Block.Air) == 2; }

            HandleDelete handler = level.deleteHandlers[old];
            if (handler != null) {
                handler(this, old, x, y, z);
                return true;
            }
            return ChangeBlock(x, y, z, Block.Air) == 2;
        }

        bool PlaceBlock(BlockID old, ushort x, ushort y, ushort z, BlockID block) {
            HandlePlace handler = level.placeHandlers[block];
            if (handler != null) {
                handler(this, block, x, y, z);
                return true;
            }
            return ChangeBlock(x, y, z, block) == 2;
        }
        
        /// <summary> Updates the block at the given position, mainly intended for manual changes by the player. </summary>
        /// <remarks> Adds to the BlockDB. Also turns block below to grass/dirt depending on light. </remarks>
        /// <returns> Return code from DoBlockchange </returns>
        public int ChangeBlock(ushort x, ushort y, ushort z, BlockID block) {
            BlockID old = level.GetBlock(x, y, z);
            int type = level.DoBlockchange(this, x, y, z, block);
            if (type == 0) return type;                                     // no change performed
            if (type == 2) Player.GlobalBlockchange(level, x, y, z, block); // different visually
            
            ushort flags = BlockDBFlags.ManualPlace;
            if (painting && CollideType.IsSolid(level.CollideType(old))) {
                flags = BlockDBFlags.Painted;
            }
            level.BlockDB.Cache.Add(this, x, y, z, flags, old, block);
            
            bool autoGrass = level.Config.GrassGrow && (level.physics == 0 || level.physics == 5);
            if (!autoGrass) return type;           
            BlockID below = level.GetBlock(x, (ushort)(y - 1), z);
            
            BlockID grass = level.Props[below].GrassBlock;
            if (grass != Block.Invalid && block == Block.Air) {
                level.Blockchange(this, x, (ushort)(y - 1), z, grass);
            }
            
            BlockID dirt = level.Props[below].DirtBlock;
            if (dirt != Block.Invalid && !level.LightPasses(block)) {
                level.Blockchange(this, x, (ushort)(y - 1), z, dirt);
            }
            return type;
        }
        
        internal int ProcessReceived(byte[] buffer, int bufferLen) {
            int processedLen = 0;
            try {
                while (processedLen < bufferLen) {
                    int packetLen = PacketSize(buffer[processedLen]);
                    if (packetLen == -1) return -1;
                    
                    // Partial packet data received
                    if (processedLen + packetLen > bufferLen) return processedLen;
                    HandlePacket(buffer, processedLen);
                    processedLen += packetLen;
                }
            } catch (Exception ex) {
                Logger.LogError(ex);
            }
            return processedLen;
        }
        
        int PacketSize(byte opcode) {
            switch (opcode) {
                case (byte)'G': return -1; // HTTP GET, ignore it
                case Opcode.Handshake:      return 1 + 1 + 64 + 64 + 1;
                case Opcode.SetBlockClient: return 1 + 6 + 1 + (hasExtBlocks ? 2 : 1);
                case Opcode.EntityTeleport: return 1 + 6 + 2 + (hasExtPositions ? 6 : 0) + (hasExtBlocks ? 2 : 1);
                case Opcode.Message:        return 1 + 1 + 64;
                case Opcode.CpeExtInfo:     return 1 + 64 + 2;
                case Opcode.CpeExtEntry:    return 1 + 64 + 4;
                case Opcode.CpeCustomBlockSupportLevel: return 1 + 1;
                case Opcode.CpePlayerClick: return 1 + 1 + 1 + 2 + 2 + 1 + 2 + 2 + 2 + 1;
                case Opcode.Ping:           return 1;
                case Opcode.CpeTwoWayPing:  return 1 + 1 + 2;

                default:
                    if (!nonPlayerClient) {
                        string msg = "Unhandled message id \"" + opcode + "\"!";
                        Leave(msg, msg, true);
                    }
                    return -1;
            }
        }
        
        void HandlePacket(byte[] buffer, int offset) {
            switch (buffer[offset]) {
                case Opcode.Ping: break;
                case Opcode.Handshake:
                    HandleLogin(buffer, offset); break;
                case Opcode.SetBlockClient:
                    if (!loggedIn) break;
                    HandleBlockchange(buffer, offset); break;
                case Opcode.EntityTeleport:
                    if (!loggedIn) break;
                    HandleMovement(buffer, offset); break;
                case Opcode.Message:
                    if (!loggedIn) break;
                    HandleChat(buffer, offset); break;
                case Opcode.CpeExtInfo:
                    HandleExtInfo(buffer, offset); break;
                case Opcode.CpeExtEntry:
                    HandleExtEntry(buffer, offset); break;
                case Opcode.CpeCustomBlockSupportLevel:
                    customBlockSupportLevel = buffer[offset + 1]; break;
                case Opcode.CpePlayerClick:
                    HandlePlayerClicked(buffer, offset); break;
                case Opcode.CpeTwoWayPing:
                    HandleTwoWayPing(buffer, offset); break;
            }
        }
        
        #if TEN_BIT_BLOCKS
        BlockID ReadBlock(byte[] buffer, int offset) {
            BlockID block;
            if (hasExtBlocks) {
                block = NetUtils.ReadU16(buffer, offset);
            } else {
                block = buffer[offset];
            }
            
            if (block > Block.MaxRaw) block = Block.MaxRaw;
            return Block.FromRaw(block);
        }
        #else
        BlockID ReadBlock(byte[] buffer, int offset) { return Block.FromRaw(buffer[offset]); }
        #endif

        void HandleBlockchange(byte[] buffer, int offset) {
            try {
                if (!loggedIn || spamChecker.CheckBlockSpam()) return;
                ushort x = NetUtils.ReadU16(buffer, offset + 1);
                ushort y = NetUtils.ReadU16(buffer, offset + 3);
                ushort z = NetUtils.ReadU16(buffer, offset + 5);
                
                byte action = buffer[offset + 7];
                if (action > 1) {
                    const string msg = "Unknown block action!";
                    Leave(msg, msg, true); return;
                }
                
                LastAction = DateTime.UtcNow;
                if (IsAfk) CmdAfk.ToggleAfk(this, "");
                
                BlockID held = ReadBlock(buffer, offset + 8);
                RawHeldBlock = held;
                
                if ((action == 0 || held == Block.Air) && !level.Config.Deletable) {
                    SendMessage("Deleting blocks is disabled in this level.");
                    RevertBlock(x, y, z); return;
                } else if (action == 1 && !level.Config.Buildable) {
                    SendMessage("Placing blocks is disabled in this level.");
                    RevertBlock(x, y, z); return;
                }
                
                if (held >= Block.Extended) {
                    if (!hasBlockDefs || level.CustomBlockDefs[held] == null) {
                        SendMessage("Invalid block type: " + Block.ToRaw(held));
                        RevertBlock(x, y, z); return;
                    }
                }
                ManualChange(x, y, z, action != 0, held, true);
            } catch ( Exception e ) {
                // Don't ya just love it when the server tattles?
                Chat.MessageOps(DisplayName + " has triggered a block change error");
                Chat.MessageOps(e.GetType().ToString() + ": " + e.Message);
                Logger.LogError(e);
            }
        }
        
        void HandleMovement(byte[] buffer, int offset) {
            if (!loggedIn || trainGrab || following.Length > 0) { CheckBlocks(Pos); return; }
            if (Supports(CpeExt.HeldBlock)) {
                RawHeldBlock = ReadBlock(buffer, offset + 1);
                if (hasExtBlocks) offset++; // corret offset for position later
            }
            
            int x, y, z;
            if (hasExtPositions) {
                x = NetUtils.ReadI32(buffer, offset + 2);
                y = NetUtils.ReadI32(buffer, offset + 6);
                z = NetUtils.ReadI32(buffer, offset + 10);
                offset += 6; // for yaw/pitch offset below
            } else {
                x = NetUtils.ReadI16(buffer, offset + 2);
                y = NetUtils.ReadI16(buffer, offset + 4);
                z = NetUtils.ReadI16(buffer, offset + 6);
            }
            
            byte yaw = buffer[offset + 8], pitch = buffer[offset + 9];
            Position next = new Position(x, y, z);
            CheckBlocks(next);

            OnPlayerMoveEvent.Call(this, next, yaw, pitch);
            if (cancelmove) { cancelmove = false; return; }
            
            Pos = next;
            SetYawPitch(yaw, pitch);
            CheckZones(next);
            
            if (!Moved() || Loading) return;
            if (DateTime.UtcNow < AFKCooldown) return;
            
            LastAction = DateTime.UtcNow;
            if (IsAfk) CmdAfk.ToggleAfk(this, "");
        }
        
        void CheckZones(Position pos) {
            Vec3S32 P = pos.BlockCoords;
            Zone zone = ZoneIn;
            
            // player hasn't moved from current zone
            if (zone != null && zone.Contains(P.X, P.Y, P.Z)) return;
            Zone[] zones = level.Zones.Items;
            if (zones.Length == 0) return;
            
            for (int i = 0; i < zones.Length; i++) {
                if (!zones[i].Contains(P.X, P.Y, P.Z)) continue;
                
                ZoneIn = zones[i];
                OnChangedZone();
                return;
            }
            
            ZoneIn = null;
            if (zone != null) OnChangedZone();
        }
        
        public void OnChangedZone() {
            if (Supports(CpeExt.InstantMOTD)) SendMapMotd();
            Zone zone = ZoneIn;
            
            for (int i = 0; i <= 4; i++) {
                string col = level.Config.GetColor(i);
                if (zone != null && zone.Config.GetColor(i) != "") {
                    col = zone.Config.GetColor(i);
                }
                if (Supports(CpeExt.EnvColors)) SendEnvColor((byte)i, col);
            }
            
            for (EnvProp i = 0; i < EnvProp.Max; i++) {
                int value = level.Config.GetEnvProp(i);
                if (i == EnvProp.SidesBlock || i == EnvProp.EdgeBlock) {
                    if (zone != null && zone.Config.GetEnvProp(i) != Block.Invalid) {
                        value = zone.Config.GetEnvProp(i);
                    }
                    
                    BlockID raw = Block.ToRaw((BlockID)value);
                    if (raw > MaxRawBlock) raw = level.RawFallback((BlockID)value);
                    value = raw;
                } else {
                    if (zone != null && zone.Config.GetEnvProp(i) != -1) {
                        value = zone.Config.GetEnvProp(i);
                    }
                }
                if (Supports(CpeExt.EnvMapAspect)) Send(Packet.EnvMapProperty(i, value));
            }
            
            if (Supports(CpeExt.EnvWeatherType)) {
                int weather = level.Config.Weather;
                if (zone != null && zone.Config.Weather != -1) weather = zone.Config.Weather;
                Send(Packet.EnvWeatherType((byte)weather));
            }
        }
        
        void CheckBlocks(Position pos) {
            try {
                Vec3U16 P = (Vec3U16)pos.BlockCoords;
                AABB bb = ModelBB.OffsetPosition(Pos);
                int index = level.PosToInt(P.X, P.Y, P.Z);
                    
                if (level.Config.SurvivalDeath) {
                    PlayerPhysics.Drown(this, bb);
                    PlayerPhysics.Fall(this, bb);
                }
                lastFallY = bb.Min.Y;
                
                PlayerPhysics.Walkthrough(this, bb);
                oldIndex = index;
            } catch (Exception ex) {
                Logger.LogError(ex);
            }
        }
        
        bool Moved() { return lastRot.RotY != Rot.RotY || lastRot.HeadX != Rot.HeadX; }
        
        public bool HandleDeath(BlockID block, string customMsg = "", bool explode = false, bool immediate = false) {
            if (!immediate && lastDeath.AddSeconds(2) > DateTime.UtcNow) return false;
            if (invincible || hidden) return false;
            
            cancelDeath = false;
            OnPlayerDeathEvent.Call(this, block);
            if (cancelDeath) { cancelDeath = false; return false; }

            onTrain = false; trainInvincible = false; trainGrab = false;
            ushort x = (ushort)Pos.BlockX, y = (ushort)Pos.BlockY, z = (ushort)Pos.BlockZ;
            
            string deathMsg = level.Props[block].DeathMessage;
            if (deathMsg != null) {
                Chat.MessageFromLevel(this, deathMsg.Replace("@p", "λNICK"));
            }
            
            if (block == Block.RocketHead) level.MakeExplosion(x, y, z, 0);
            if (block == Block.Creeper) level.MakeExplosion(x, y, z, 1);
            
            if (block == Block.Stone || block == Block.Cobblestone) {
                if (explode) level.MakeExplosion(x, y, z, 1);
                if (block == Block.Stone) {
                    Chat.MessageFrom(this, customMsg.Replace("@p", "λNICK"));
                } else {
                    Chat.MessageFromLevel(this, customMsg.Replace("@p", "λNICK"));
                }
            }
            
            PlayerActions.Respawn(this);
            TimesDied++;
            // NOTE: If deaths column is ever increased past 16 bits, remove this clamp
            if (TimesDied > short.MaxValue) TimesDied = short.MaxValue;

            if (ServerConfig.AnnounceDeathCount && (TimesDied > 0 && TimesDied % 10 == 0)) {
                Chat.MessageFromLevel(this, "λNICK %Shas died &3" + TimesDied + " times");
            }
            lastDeath = DateTime.UtcNow;
            return true;
        }

        void HandleChat(byte[] buffer, int offset) {
            if (!loggedIn) return;
            byte continued = buffer[offset + 1];
            string text = NetUtils.ReadString(buffer, offset + 2);
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

            if (Server.voting) {
                if (CheckVote(text, this, "y", "yes", ref Server.YesVotes) ||
                    CheckVote(text, this, "n", "no", ref Server.NoVotes)) return;
            }

            if (LSGame.Instance.HandlesChatMessage(this, text)) return;
            if (ZSGame.Instance.HandlesChatMessage(this, text)) return;
            
            // Put this after vote collection so that people can vote even when chat is moderated
            if (Server.chatmod && !voice) { SendMessage("Chat moderation is on, you cannot speak."); return; }
            
            // Filter out bad words
            if (ServerConfig.ProfanityFiltering) text = ProfanityFilter.Parse(text);

            if (ChatModes.Handle(this, text)) return;
            text = HandleJoker(text);

            OnPlayerChatEvent.Call(this, text);
            if (cancelchat) { cancelchat = false; return; }

            if (Chatroom != null) { 
                string roomPrefix = "<ChatRoom: " + Chatroom + "> λNICK: &f";
                Chat.MessageChat(ChatScope.Chatroom, this, roomPrefix + text, Chatroom, null);
            } else {
                Chat.MessageChat(this, "λFULL: &f" + text, null, true);
            }
        }
        
        bool FilterChat(ref string text, byte continued) {
            // handles the /womid client message, which displays the WoM vrersion
            if (text.StartsWith("/womid")) {
                string version = (text.Length <= 21 ? text.Substring(text.IndexOf(' ') + 1) : text.Substring(7, 15));
                UsingWom = true;
                return true;
            }
            
            if (Supports(CpeExt.LongerMessages) && continued != 0) {
                partialMessage += text;
                if (text.Length < NetUtils.StringSize) partialMessage += " ";
                return true;
            }

            if (text.CaselessContains("^detail.user=")) {
                SendMessage("&cYou cannot use WoM detail strings in a chat message.");
                return true;
            }

            if (IsPartialSpaced(text)) {
                partialMessage += text.Substring(0, text.Length - 2) + " ";
                SendMessage("&3Partial message: &f" + partialMessage);
                return true;
            } else if (IsPartialJoined(text)) {
                partialMessage += text.Substring(0, text.Length - 2);
                SendMessage("&3Partial message: &f" + partialMessage);
                return true;
            } else if (partialMessage.Length > 0) {
                text = partialMessage + text;
                partialMessage = "";
            }

            text = Regex.Replace(text, "  +", " ");
            if (text.IndexOf('&') >= 0) {
                const string msg = "Illegal character in chat message!";
                Leave(msg, msg, true); return true;
            }
            return text.Length == 0;
        }
        
        static bool IsPartialSpaced(string text) {
            return text.EndsWith(" >") || text.EndsWith(" /");
        }
        
        static bool IsPartialJoined(string text) {
            return text.EndsWith(" <") || text.EndsWith(" \\");
        }
        
        bool DoCommand(string text) {
            // Typing / repeats last command executed
            if (text == "/") {
                if (lastCMD.Length == 0) {
                    Player.Message(this, "Cannot repeat command - no commands used yet.");
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
                HandleCommand(text, "");
            } else {
                string cmd = text.Substring(0, sep);
                string args = text.Substring(sep + 1);
                HandleCommand(cmd, args);
            }
            return true;
        }
        
        string HandleJoker(string text) {
            if (!joker) return text;
            Logger.Log(LogType.PlayerChat, "<JOKER>: {0}: {1}", name, text);
            Chat.MessageFromOps(this, "%S<&aJ&bO&cK&5E&9R%S>: λNICK:&f " + text);

            TextFile jokerFile = TextFile.Files["Joker"];
            jokerFile.EnsureExists();
            
            string[] lines = jokerFile.GetText();
            Random rnd = new Random();
            return lines.Length > 0 ? lines[rnd.Next(lines.Length)] : text;
        }
        
        public void HandleCommand(string cmd, string args) {
            cmd = cmd.ToLower();
            try {
                Command command = GetCommand(ref cmd, ref args);
                if (command == null) return;
                
                Thread thread = new Thread(() => UseCommand(command, args));
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
                    string args = parts.Length > 1 ? parts[1] : "";
                    
                    Command command = GetCommand(ref cmd, ref args);
                    if (command == null) return;
                    
                    messages.Add(args); commands.Add(command);
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
            if (cmd.Length == 0) { SendMessage("No command entered."); return false; }
            if (ServerConfig.AgreeToRulesOnEntry && !agreed && !(cmd == "agree" || cmd == "rules" || cmd == "disagree" || cmd == "pass" || cmd == "setpass")) {
                SendMessage(mustAgreeMsg); return false;
            }
            if (jailed) {
                SendMessage("You cannot use any commands while jailed."); return false;
            }
            if (ServerConfig.verifyadmins && adminpen && !(cmd == "pass" || cmd == "setpass")) {
                SendMessage("&cYou must verify first with %T/Pass [Password]"); return false;
            }
            
            TimeSpan delta = cmdUnblocked - DateTime.UtcNow;
            if (delta.TotalSeconds > 0) {
                int secs = (int)Math.Ceiling(delta.TotalSeconds);
                SendMessage("Blocked from using commands for " +
                            "another " + secs + " seconds"); return false;
            }
            return true;
        }
        
        Command GetCommand(ref string cmdName, ref string cmdArgs) {
            if (!CheckCommand(cmdName)) return null;
            Command.Search(ref cmdName, ref cmdArgs);
            
            byte bindIndex;
            if (byte.TryParse(cmdName, out bindIndex) && bindIndex < CmdBindings.Length) {
                if (CmdArgsBindings[bindIndex] == null) { 
                    SendMessage("No command is bound to: %T/" + cmdName); return null; 
                }
                
                cmdName = CmdBindings[bindIndex];
                cmdArgs = CmdArgsBindings[bindIndex] + " " + cmdArgs;
                cmdArgs = cmdArgs.TrimEnd(' ');
            }
            
            OnPlayerCommandEvent.Call(this, cmdName, cmdArgs);
            if (cancelcommand) { cancelcommand = false; return null; }
            
            Command command = Command.Find(cmdName);
            if (command == null) {
                if (Block.Parse(this, cmdName) != Block.Invalid) {
                    cmdArgs = cmdName; cmdName = "mode";
                    command = Command.Find("Mode");
                } else {
                    Logger.Log(LogType.CommandUsage, "{0} tried to use unknown command: /{1} {2}", name, cmdName, cmdArgs);
                    SendMessage("Unknown command \"" + cmdName + "\"."); return null;
                }
            }

            if (!group.CanExecute(command)) {
                CommandPerms.Find(command.name).MessageCannotUse(this);
                return null; 
            }
            
            string reason = Command.GetDisabledReason(command.Enabled);
            if (reason != null) {
                SendMessage("Command is disabled as " + reason); return null;
            }
            if (level.IsMuseum && !command.museumUsable) {
                SendMessage("Cannot use this command while in a museum."); return null;
            }
            if (frozen && !command.UseableWhenFrozen) {
                SendMessage("Cannot use this command while frozen."); return null;
            }
            return command;
        }
        
        bool UseCommand(Command command, string message) {
            string cmd = command.name;
            if (!cmd.CaselessEq("pass") && !cmd.CaselessEq("lastcmd")) {
                lastCMD = message.Length == 0 ? cmd : cmd + " " + message;
                lastCmdTime = DateTime.UtcNow;
                Logger.Log(LogType.CommandUsage, "{0} used /{1} {2}", name, cmd, message);
            }

            try { //opstats patch (since 5.5.11)
                if (Server.Opstats.CaselessContains(cmd) || (cmd.CaselessEq("review") && message.CaselessEq("next") && Server.reviewlist.Count > 0)) {
                    Database.Backend.AddRow("Opstats", "Time, Name, Cmd, Cmdmsg",
                                            DateTime.Now.ToString(Database.DateFormat), name, cmd, message);
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
