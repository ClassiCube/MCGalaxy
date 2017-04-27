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
using MCGalaxy.Games;
using MCGalaxy.Network;
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
            byte oldB = level.GetTile(x, y, z);
            if (oldB == Block.Invalid) return;
            
            if (jailed || !agreed || !canBuild) { RevertBlock(x, y, z); return; }
            if (level.IsMuseum && Blockchange == null) return;
            
            if (action > 1) {
                const string msg = "Unknown block action!";
                Leave(msg, msg, true); return;
            }
            bool doDelete = !painting && action == 0;

            if (Server.verifyadmins && adminpen) {
                SendMessage("&cYou must first verify with %T/pass [Password]");
                RevertBlock(x, y, z); return;
            }

            if (Server.zombie.Running && Server.zombie.HandlesManualChange(this, x, y, z, action, block, oldB))
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

            if (oldB >= Block.air_flood && oldB <= Block.air_door_air) {
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
                    Server.s.Log(name + " attempted to build with a " + diff + " distance offset");
                    SendMessage("You can't build that far away.");
                    RevertBlock(x, y, z); return;
                }
            }

            if (!CheckManualChange(oldB, block, doDelete)) {
                RevertBlock(x, y, z); return;
            }

            byte blockRaw = block;
            if (block < Block.CpeCount) block = bindings[block];
            
            //Ignores updating blocks that are the same and send block only to the player
            byte newB = (painting || action == 1) ? block : Block.air;
            if (oldB == newB && (painting || blockRaw != block)) {
                if (oldB != Block.custom_block || extBlock == level.GetExtTile(x, y, z)) {
                    RevertBlock(x, y, z); return;
                }
            }
            
            if (modeType != 0) block = modeType;
            if (doDelete) {
                bool deleted = DeleteBlock(oldB, x, y, z, block, extBlock);
            } else {
                bool placed = PlaceBlock(oldB, x, y, z, block, extBlock);
                // Client always assumes delete succeeds, so we need to echo back the painted over block
                // if the block was not changed visually (e.g. they paint white with door_white)
                if (!placed && painting) RevertBlock(x, y, z);
            }
        }
        
        internal bool CheckManualChange(byte old, byte block, bool replaceMode) {
            if (!BlockPerms.CanModify(this, old) && !Block.BuildIn(old) && !Block.AllowBreak(old)) {
                Formatter.MessageBlock(this, replaceMode ? "replace " : "delete ", old);
                return false;
            }
            return CommandParser.IsBlockAllowed(this, "place ", block);
        }
        
        bool DeleteBlock(byte old, ushort x, ushort y, ushort z, byte block, byte extBlock) {
            if (deleteMode) { return ChangeBlock(x, y, z, Block.air, 0) == 2; }

            HandleDelete handler = BlockBehaviour.deleteHandlers[old];
            if (handler != null) {
                handler(this, old, x, y, z);
                return true;
            }
            return ChangeBlock(x, y, z, Block.air, 0) == 2;
        }

        bool PlaceBlock(byte old, ushort x, ushort y, ushort z, byte block, byte extBlock) {
            HandlePlace handler = BlockBehaviour.placeHandlers[block];
            if (handler != null) {
                handler(this, old, x, y, z);
                return true;
            }
            return ChangeBlock(x, y, z, block, extBlock) == 2;
        }
        
        /// <summary> Updates the block at the given position, mainly intended for manual changes by the player. </summary>
        /// <remarks> Adds to the BlockDB. Also turns block below to grass/dirt depending on light. </remarks>
        /// <returns> Return code from DoBlockchange </returns>
        public int ChangeBlock(ushort x, ushort y, ushort z, byte block, byte extBlock) {
            byte old = level.GetTile(x, y, z), extOld = 0;
            if (old == Block.custom_block) extOld = level.GetExtTile(x, y, z);
            
            int type = level.DoBlockchange(this, x, y, z, block, extBlock);
            if (type == 0) return type;                                               // no change performed
            if (type == 2) Player.GlobalBlockchange(level, x, y, z, block, extBlock); // different visually
            
            ushort flags = BlockDBFlags.ManualPlace;
            if (painting && Replacable(old)) flags = BlockDBFlags.Painted;
            level.BlockDB.Cache.Add(this, x, y, z, flags, old, extOld, block, extBlock);           
            
            bool autoGrass = level.GrassGrow && (level.physics == 0 || level.physics == 5);
            if (!autoGrass) return type;
            
            byte below = level.GetTile(x, (ushort)(y - 1), z);
            if (below == Block.dirt && block == Block.air) {
                level.Blockchange(this, x, (ushort)(y - 1), z, Block.grass);
            }
            if (below == Block.grass && !level.LightPasses(block, extBlock)) {
                level.Blockchange(this, x, (ushort)(y - 1), z, Block.dirt);
            }
            return type;
        }
        
        
        static bool Replacable(byte block) {
            block = Block.Convert(block);
            return block == Block.air || (block >= Block.water && block <= Block.lavastill);
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
                    return 10 + (hasExtPositions ? 6 : 0);
                case Opcode.Message:
                    if (!loggedIn) goto default;
                    return 66;
                    case Opcode.CpeExtInfo: return 67;
                    case Opcode.CpeExtEntry: return 69;
                    case Opcode.CpeCustomBlockSupportLevel: return 2;
                    case Opcode.CpePlayerClick: return 15;
                    case Opcode.Ping: return 1;

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
            }
        }

        void HandleBlockchange(byte[] packet) {
            try {
                if (!loggedIn || spamChecker.CheckBlockSpam()) return;
                ushort x = NetUtils.ReadU16(packet, 1);
                ushort y = NetUtils.ReadU16(packet, 3);
                ushort z = NetUtils.ReadU16(packet, 5);
                if (frozen) { RevertBlock(x, y, z); return; }
                
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
            if (!loggedIn || trainGrab || following != "") return;
            byte heldBlock = packet[1];
            if (HasCpeExt(CpeExt.HeldBlock))
                RawHeldBlock = heldBlock;
            
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

            if (Server.Countdown.HandlesMovement(this, next, yaw, pitch))
                return;
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

        internal void CheckSurvival(ushort x, ushort y, ushort z) {
            byte bFeet = GetSurvivalBlock(x, (ushort)(y - 2), z);
            byte bBody = GetSurvivalBlock(x, (ushort)(y - 1), z);
            byte bHead = GetSurvivalBlock(x, y, z);
            
            if (level.PosToInt(x, y, z) != oldIndex || y != oldFallY) {
                bFeet = Block.Convert(bFeet);
                
                if (bFeet == Block.air) {
                    if (y < oldFallY)
                        fallCount++;
                    else if (y > oldFallY) // flying up, for example
                        fallCount = 0;
                    oldFallY = y;
                    drownCount = 0;
                    return;
                } else if (!(bFeet == Block.water || bFeet == Block.waterstill ||
                             bFeet == Block.lava || bFeet == Block.lavastill)) {
                    if (fallCount > level.fall)
                        HandleDeath(Block.air, 0, null, false, true);
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
                    
                    // level drown is in 10ths of a second, and there are 100 ticks/second
                    if (drownCount > level.drown * 10) {
                        HandleDeath(Block.water, 0);
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

        internal void CheckBlock() {
            AABB bb = ModelBB.OffsetPosition(Pos);
            Vec3S32 min = bb.BlockMin, max = bb.BlockMax;
            bool hitWalkthrough = false;
            
            for (int y = min.Y; y <= max.Y; y++)
                for (int z = min.Z; z <= max.Z; z++)
                    for (int x = min.X; x <= max.X; x++)
            {
                ushort xP = (ushort)x, yP = (ushort)y, zP = (ushort)z;
                byte block = level.GetTile(xP, yP, zP), extBlock = 0;
                if (block == Block.Invalid) continue;
                if (block == Block.custom_block)
                    extBlock = level.GetExtTileNoCheck(xP, yP, zP);
                
                AABB blockBB = Block.BlockAABB(block, extBlock, level)
                    .Offset(x * 32, y * 32, z * 32);
                if (!bb.Intersects(blockBB)) continue;
                
                // We can activate only one walkthrough block per movement
                if (!hitWalkthrough) {
                    HandleWalkthrough handler = BlockBehaviour.walkthroughHandlers[block];
                    if (handler != null && handler(this, block, xP, yP, zP)) {
                        lastWalkthrough = level.PosToInt(xP, yP, zP);
                        hitWalkthrough = true;
                    }
                }
                
                // Some blocks will cause death of players
                if (block != Block.custom_block && !Block.Props[block].KillerBlock) continue;
                if (block == Block.custom_block && !level.CustomBlockProps[extBlock].KillerBlock) continue;
                
                if (block == Block.tntexplosion && PlayingTntWars) continue; // TODO: hardcoded behaviour is icky
                if (block == Block.train && trainInvincible) continue;
                HandleDeath(block, extBlock);
            }
            
            if (!hitWalkthrough) lastWalkthrough = -1;
        }

        [Obsolete("Use HandleDeath with ExtBlock attribute")]
        public void HandleDeath(byte b, string customMessage = "", bool explode = false, bool immediate = false) {
            HandleDeath(b, 0, customMessage, explode, immediate);
        }
        
        public void HandleDeath(byte block, byte extBlock, string customMessage = "",
                                bool explode = false, bool immediate = false) {
            if (OnDeath != null) OnDeath(this, block);
            if (PlayerDeath != null) PlayerDeath(this, block);
            OnPlayerDeathEvent.Call(this, block);
            
            if (Server.lava.active && Server.lava.HasPlayer(this) && Server.lava.IsPlayerDead(this)) return;
            if (!immediate && lastDeath.AddSeconds(2) > DateTime.UtcNow) return;
            if (!level.Killer || invincible || hidden) return;

            onTrain = false; trainInvincible = false; trainGrab = false;
            ushort x = (ushort)Pos.BlockX, y = (ushort)Pos.BlockY, z = (ushort)Pos.BlockZ;
            
            string deathMsg = null;
            if (block != Block.custom_block) {
                deathMsg = Block.Props[block].DeathMessage;
            } else {
                deathMsg = level.CustomBlockProps[extBlock].DeathMessage;
            }
            if (deathMsg != null) {
                Chat.MessageLevel(this, deathMsg.Replace("@p", ColoredName), false, level);
            }
            
            if (block == Block.rockethead) level.MakeExplosion(x, y, z, 0);
            if (block == Block.creeper) level.MakeExplosion(x, y, z, 1);
            if (block == Block.rock || block == Block.stone) {
                if (explode) level.MakeExplosion(x, y, z, 1);
                if (block == Block.rock) {
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
            if (Chatroom != null) { Chat.MessageChatRoom(this, text, true, Chatroom); return; }

            if (!level.worldChat) {
                Server.s.Log("<" + name + ">[level] " + text);
                Chat.MessageLevel(this, text, true, level);
            } else {
                Server.s.Log("<" + name + "> " + text);
                if (OnChat != null) OnChat(this, text);
                if (PlayerChat != null) PlayerChat(this, text);
                OnPlayerChatEvent.Call(this, text);
                if (cancelchat) { cancelchat = false; return; }
                
                if (Server.worldChat) {
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
                const string msg = "Illegal character in chat message!";
                Leave(msg, msg, true); return true;
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
            if (byte.TryParse(cmd, out bindIndex) && bindIndex < cmdBind.Length) {
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
                if (Block.Byte(cmd) != Block.Invalid) {
                    cmdArgs = cmd.ToLower(); cmd = "mode";
                    command = Command.all.Find("mode");
                } else {
                    Server.s.CommandUsed(name + " tried to use unknown command: /" + cmd + " " + cmdArgs);
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
                    Database.Backend.AddRow("Opstats", "Time, Name, Cmd, Cmdmsg",
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
