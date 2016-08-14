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
using MCGalaxy.BlockPhysics;
using MCGalaxy.Games;
using MCGalaxy.SQL;

namespace MCGalaxy {

    public sealed partial class Level : IDisposable {
        
        public byte[] blocks;
        public byte[][] CustomBlocks;
        public int ChunksX, ChunksY, ChunksZ;
        
        public bool HasCustomBlocks {
            get {
                if (CustomBlocks == null) return false;
                for (int i = 0; i < CustomBlocks.Length; i++)
                    if (CustomBlocks[i] != null) return true;
                return false;
            }
        }
        
        public byte GetTile(ushort x, ushort y, ushort z) {
            int index = PosToInt(x, y, z);
            if (index < 0 || blocks == null) return Block.Zero;
            return blocks[index];
        }

        public byte GetTile(int b) {
            ushort x = 0, y = 0, z = 0;
            IntToPos(b, out x, out y, out z);
            return GetTile(x, y, z);
        }
        
        public byte GetExtTile(ushort x, ushort y, ushort z) {
            int index = PosToInt(x, y, z);
            if (index < 0 || blocks == null) return Block.Zero;
            
            int cx = x >> 4, cy = y >> 4, cz = z >> 4;
            byte[] chunk = CustomBlocks[(cy * ChunksZ + cz) * ChunksX + cx];
            return chunk == null ? (byte)0 :
                chunk[(y & 0x0F) << 8 | (z & 0x0F) << 4 | (x & 0x0F)];
        }
        
        public byte GetExtTile(int index) {
            ushort x, y, z;
            IntToPos(index, out x, out y, out z);
            
            int cx = x >> 4, cy = y >> 4, cz = z >> 4;
            byte[] chunk = CustomBlocks[(cy * ChunksZ + cz) * ChunksX + cx];
            return chunk == null ? (byte)0 :
                chunk[(y & 0x0F) << 8 | (z & 0x0F) << 4 | (x & 0x0F)];
        }
        
        public byte GetFallbackExtTile(ushort x, ushort y, ushort z) {
            byte tile = GetExtTile(x, y, z);
            BlockDefinition def = CustomBlockDefs[tile];
            return def == null ? Block.air : def.FallBack;
        }
        
        public byte GetFallbackExtTile(int index) {
            byte tile = GetExtTile(index);
            BlockDefinition def = CustomBlockDefs[tile];
            return def == null ? Block.air : def.FallBack;
        }
        
        public byte GetFallback(byte extType) {
            BlockDefinition def = CustomBlockDefs[extType];
            return def == null ? Block.air : def.FallBack;
        }
        
        public void SetTile(int index, byte block) {
            if (blocks == null || index < 0 || index >= blocks.Length) return;
            blocks[index] = block;
            changed = true;
        }
        
        public void SetTile(ushort x, ushort y, ushort z, byte block) {
            int index = PosToInt(x, y, z);
            if (blocks == null || index < 0) return;
            blocks[index] = block;
            changed = true;
        }
        
        public void SetExtTile(ushort x, ushort y, ushort z, byte extBlock) {
            int index = PosToInt(x, y, z);
            if (index < 0 || blocks == null) return;
            SetExtTileNoCheck(x, y, z, extBlock);
        }
        
        public void SetExtTileNoCheck(ushort x, ushort y, ushort z, byte extBlock) {
            int cx = x >> 4, cy = y >> 4, cz = z >> 4;
            int cIndex = (cy * ChunksZ + cz) * ChunksX + cx;
            byte[] chunk = CustomBlocks[cIndex];
            
            if (chunk == null) {
                chunk = new byte[16 * 16 * 16];
                CustomBlocks[cIndex] = chunk;
            }
            chunk[(y & 0x0F) << 8 | (z & 0x0F) << 4 | (x & 0x0F)] = extBlock;
        }
        
        public void RevertExtTileNoCheck(ushort x, ushort y, ushort z) {
            int cx = x >> 4, cy = y >> 4, cz = z >> 4;
            int cIndex = (cy * ChunksZ + cz) * ChunksX + cx;        
            byte[] chunk = CustomBlocks[cIndex];
            
            if (chunk == null) return;
            chunk[(y & 0x0F) << 8 | (z & 0x0F) << 4 | (x & 0x0F)] = 0;
        }
        
        public void SetTile(ushort x, ushort y, ushort z, byte block, Player p, byte extBlock = 0) {
            int index = PosToInt(x, y, z);
            if (blocks == null || index < 0) return;
            
            byte oldBlock = blocks[index], oldExtBlock = 0;
            blocks[index] = block;
            changed = true;
            
            if (oldBlock == Block.custom_block) {
                oldExtBlock = GetExtTile(x, y, z);
                if (block != Block.custom_block)
                    RevertExtTileNoCheck(x, y, z);
            }
            if (block == Block.custom_block)
                SetExtTileNoCheck(x, y, z, extBlock);
            if (p == null) return;    
            
            AddToBlockDB(p, index, block, extBlock, block == 0);
            Player.UndoPos Pos;
            Pos.x = x; Pos.y = y; Pos.z = z;
            Pos.mapName = this.name;
            Pos.type = oldBlock; Pos.extType = oldExtBlock;
            Pos.newtype = block; Pos.newExtType = extBlock;
            Pos.timeDelta = (int)DateTime.UtcNow.Subtract(Server.StartTime).TotalSeconds;
            p.UndoBuffer.Add(this, Pos);
        }

        bool CheckTNTWarsChange(Player p, ushort x, ushort y, ushort z, ref byte block) {
            if (!(block == Block.tnt || block == Block.bigtnt || block == Block.nuketnt || block == Block.smalltnt))
                return true;
            
            TntWarsGame game = TntWarsGame.GetTntWarsGame(p);
            if (game.InZone(x, y, z, true))
                return false;
            
            if (p.CurrentAmountOfTnt == game.TntPerPlayerAtATime) {
                Player.Message(p, "TNT Wars: Maximum amount of TNT placed"); return false;
            }
            if (p.CurrentAmountOfTnt > game.TntPerPlayerAtATime) {
                Player.Message(p, "TNT Wars: You have passed the maximum amount of TNT that can be placed!"); return false;
            }
            p.TntAtATime();
            block = Block.smalltnt;
            return true;
        }
        
        bool CheckZonePerms(Player p, ushort x, ushort y, ushort z,
                        ref bool AllowBuild, ref bool inZone, ref string Owners) {
            if (p.Rank < LevelPermission.Admin) {
            	bool foundDel = FindZones(p, x, y, z, ref inZone, ref AllowBuild, ref Owners);
                if (!AllowBuild) {
                    if (p.ZoneSpam <= DateTime.UtcNow) {
                        if (Owners != "")
                            Player.Message(p, "This zone belongs to &b" + Owners.Remove(0, 2) + ".");
                        else
                            Player.Message(p, "This zone belongs to no one.");
                        p.ZoneSpam = DateTime.UtcNow.AddSeconds(2);
                    }
                    return false;
                }
            }
            return true;
        }
        
        bool FindZones(Player p, ushort x, ushort y, ushort z, 
                             ref bool inZone, ref bool AllowBuild, ref string Owners) {
            if (ZoneList.Count == 0) { AllowBuild = true; return false; }
            bool foundDel = false;
            
            for (int i = 0; i < ZoneList.Count; i++) {
                Zone zn = ZoneList[i];
                if (x < zn.smallX || x > zn.bigX || y < zn.smallY || y > zn.bigY || z < zn.smallZ || z > zn.bigZ)
                    continue;
                
                inZone = true;
                if (zn.Owner.Length >= 3 && zn.Owner.StartsWith("grp")) {
                    string grpName = zn.Owner.Substring(3);
                    if (Group.Find(grpName).Permission <= p.Rank) {
                        AllowBuild = true; break;
                    }
                    AllowBuild = false;
                    Owners += ", " + grpName;
                } else {
                	if (zn.Owner.CaselessEq(p.name)) {
                        AllowBuild = true; break;
                    }
                    AllowBuild = false;
                    Owners += ", " + zn.Owner;
                }
            }
            return foundDel;
        }
        
        bool CheckRank(Player p, bool AllowBuild, bool inZone) {
            if (p.Rank < permissionbuild && (!inZone || !AllowBuild)) {
                if (p.ZoneSpam <= DateTime.UtcNow) {
                    Player.Message(p, "Must be at least " + PermissionToName(permissionbuild) + " to build here");
                    p.ZoneSpam = DateTime.UtcNow.AddSeconds(2);
                }
                return false;
            }
            
            if (p.Rank > perbuildmax && (!inZone || !AllowBuild) && !p.group.CanExecute("perbuildmax")) {
                if (p.ZoneSpam <= DateTime.UtcNow) {
                    Player.Message(p, "Your rank must be " + perbuildmax + " or lower to build here!");
                    p.ZoneSpam = DateTime.UtcNow.AddSeconds(2);
                }
                return false;
            }
            return true;
        }
        
        public bool CheckAffectPermissions(Player p, ushort x, ushort y, ushort z, 
                                           byte b, byte type, byte extType = 0) {
            if (!Block.AllowBreak(b) && !Block.canPlace(p, b) && !Block.BuildIn(b)) return false;
            if (p.PlayingTntWars && !CheckTNTWarsChange(p, x, y, z, ref type)) return false;
            
            string Owners = "";
            bool AllowBuild = true, inZone = false;
            if (!CheckZonePerms(p, x, y, z, ref AllowBuild, ref inZone, ref Owners))
                return false;
            if (Owners.Length == 0 && !CheckRank(p, AllowBuild, inZone))
                return false;
            return true;
        }
        
        public void Blockchange(Player p, ushort x, ushort y, ushort z, 
                                byte block, byte extBlock = 0) {
            if (DoBlockchange(p, x, y, z, block, extBlock))
                Player.GlobalBlockchange(this, x, y, z, block, extBlock);
        }
        
        public bool DoBlockchange(Player p, ushort x, ushort y, ushort z, 
                                  byte block, byte extBlock = 0, bool drawn = false) {
            string errorLocation = "start";
        retry:
            try
            {
                //if (x < 0 || y < 0 || z < 0) return;
                if (x >= Width || y >= Height || z >= Length) return false;
                byte old = GetTile(x, y, z), extOld = 0;
                if (old == Block.custom_block) extOld = GetExtTile(x, y, z);

                errorLocation = "Permission checking";
                if (!CheckAffectPermissions(p, x, y, z, old, block, extBlock)) {
                    p.RevertBlock(x, y, z); return false;
                }

                if (old == Block.sponge && physics > 0 && block != Block.sponge)
                    OtherPhysics.DoSpongeRemoved(this, PosToInt(x, y, z));
                if (old == Block.lava_sponge && physics > 0 && block != Block.lava_sponge)
                    OtherPhysics.DoSpongeRemoved(this, PosToInt(x, y, z), true);

                errorLocation = "Undo buffer filling";
                Player.UndoPos Pos;
                Pos.x = x; Pos.y = y; Pos.z = z;
                Pos.mapName = name;
                Pos.type = old; Pos.extType = extOld;
                Pos.newtype = block; Pos.newExtType = extBlock;
                Pos.timeDelta = (int)DateTime.UtcNow.Subtract(Server.StartTime).TotalSeconds;
                p.UndoBuffer.Add(this, Pos);

                errorLocation = "Setting tile";
                p.IncrementBlockStats(block, drawn);
                
                SetTile(x, y, z, block);
                if (old == Block.custom_block && block != Block.custom_block)
                    RevertExtTileNoCheck(x, y, z);
                if (block == Block.custom_block)
                    SetExtTileNoCheck(x, y, z, extBlock);

                errorLocation = "Adding physics";
                if (p.PlayingTntWars && block == Block.smalltnt) AddTntCheck(PosToInt(x, y, z), p);
                if (physics > 0 && Block.Physics(block)) AddCheck(PosToInt(x, y, z));

                changed = true;
                backedup = false;
                bool diffBlock = old == Block.custom_block ? extOld != extBlock :
                    Block.Convert(old) != Block.Convert(block);
                return diffBlock;
            } catch (OutOfMemoryException) {
                Player.Message(p, "Undo buffer too big! Cleared!");
                p.UndoBuffer.Clear();
                p.RemoveInvalidUndos();
                goto retry;
            } catch (Exception e) {
                Server.ErrorLog(e);
                Chat.MessageOps(p.name + " triggered a non-fatal error on " + name);
                Chat.MessageOps("Error location: " + errorLocation);
                Server.s.Log(p.name + " triggered a non-fatal error on " + name);
                Server.s.Log("Error location: " + errorLocation);
                return false;
            }
        }
        
        void AddTntCheck(int b, Player p) {
            PhysicsArgs args = default(PhysicsArgs);
            args.Type1 = PhysicsArgs.TntWars;
            args.Value1 = (byte)p.SessionID;
            args.Value2 = (byte)(p.SessionID >> 8);
            args.Data = (byte)(p.SessionID >> 16);
            AddCheck(b, false, args);
        }
        
        public void Blockchange(int b, byte block, bool overRide = false, 
                                PhysicsArgs data = default(PhysicsArgs),
                                byte extBlock = 0, bool addUndo = true) { //Block change made by physics
            if (DoPhysicsBlockchange(b, block, overRide, data, extBlock, addUndo))
                Player.GlobalBlockchange(this, b, block, extBlock);
        }
        
        public void Blockchange(ushort x, ushort y, ushort z, byte block, bool overRide = false, 
                                PhysicsArgs data = default(PhysicsArgs),
                                byte extBlock = 0, bool addUndo = true) {
            Blockchange(PosToInt(x, y, z), block, overRide, data, extBlock, addUndo); //Block change made by physics
        }
        
        public void Blockchange(ushort x, ushort y, ushort z, byte block, byte extBlock) {
            Blockchange(PosToInt(x, y, z), block, false, default(PhysicsArgs), extBlock); //Block change made by physics
        }
        
        internal bool DoPhysicsBlockchange(int b, byte block, bool overRide = false, 
                                           PhysicsArgs data = default(PhysicsArgs), 
                                           byte extBlock = 0, bool addUndo = true) {
            if (b < 0 || b >= blocks.Length || blocks == null) return false;
            byte old = blocks[b];
            byte oldExt = old == Block.custom_block ? GetExtTile(b) : (byte)0;
            try
            {
                if (!overRide)
                    if (Block.Props[old].OPBlock || (Block.Props[block].OPBlock && data.Raw != 0)) 
                        return false;

                if (b == Block.sponge && physics > 0 && block != Block.sponge)
                    OtherPhysics.DoSpongeRemoved(this, b);

                if (b == Block.lava_sponge && physics > 0 && block != Block.lava_sponge)
                    OtherPhysics.DoSpongeRemoved(this, b, true);

                if (addUndo) {
                    UndoPos uP = default(UndoPos);
                    uP.index = b;
                    uP.SetData(old, oldExt, block, extBlock);

                    if (UndoBuffer.Count < Server.physUndo) {
                        UndoBuffer.Add(uP);
                    } else {
                        if (currentUndo >= Server.physUndo)
                            currentUndo = 0;
                        UndoBuffer[currentUndo] = uP;
                    }
                    currentUndo++;
                }

                blocks[b] = block;
                if (block == Block.custom_block) {
                    ushort x, y, z;
                    IntToPos(b, out x, out y, out z);
                    SetExtTileNoCheck(x, y, z, extBlock);
                } else if (old == Block.custom_block) {
                    ushort x, y, z;
                    IntToPos(b, out x, out y, out z);
                    RevertExtTileNoCheck(x, y, z);
                }                
                if (physics > 0 && (Block.Physics(block) || data.Raw != 0))
                    AddCheck(b, false, data);
                
                // Save bandwidth sending identical looking blocks, like air/op_air changes.
                bool diffBlock = Block.Convert(old) != Block.Convert(block);
                if (!diffBlock && old == Block.custom_block)
                    diffBlock = oldExt != extBlock;
                return diffBlock;
            } catch {
                blocks[b] = block;
                return false;
            }
        }

        public int PosToInt(ushort x, ushort y, ushort z) {
            //if (x < 0 || x >= Width || y < 0 || y >= Height || z < 0 || z >= Length)
            //    return -1;
            if (x >= Width || y >= Height || z >= Length) return -1;
            return x + Width * (z + y * Length);
        }

        public void IntToPos(int pos, out ushort x, out ushort y, out ushort z) {
            y = (ushort)(pos / Width / Length);
            pos -= y * Width * Length;
            z = (ushort)(pos / Width);
            pos -= z * Width;
            x = (ushort)pos;
        }

        public int IntOffset(int pos, int x, int y, int z)  {
            return pos + x + z * Width + y * Width * Length;
        }
        
        public bool IsValidPos(Vec3U16 pos) {
            return pos.X < Width && pos.Y < Height && pos.Z < Length;
        }
        
        public void AddToBlockDB(Player p, int index, byte block, byte extBlock, bool delete) {
            if (!UseBlockDB) return;
            BlockPos bP = default(BlockPos);
            bP.name = p.name;
            bP.index = index;
            
            bP.SetData(block, extBlock, delete);
            lock (blockCacheLock)
                blockCache.Add(bP);
        }
        
        public void UpdateBlock(Player p, ushort x, ushort y, ushort z, 
                                byte block, byte extBlock, bool drawn = false) {
            if (!DoBlockchange(p, x, y, z, block, extBlock, drawn)) return;
            int index = PosToInt(x, y, z);
            AddToBlockDB(p, index, block, extBlock, block == 0);
            
            if (bufferblocks) 
                BlockQueue.Addblock(p, index, block, extBlock);
            else 
                Player.GlobalBlockchange(this, x, y, z, block, extBlock);
        }
        
        public string BlockName(byte block, byte extBlock) {
            if (block == Block.custom_block) {
                BlockDefinition[] defs = CustomBlockDefs;
                for (int i = 1; i < 255; i++) {
                    BlockDefinition def = defs[i];
                    if (def == null) continue;
                    if (def.BlockID == extBlock) return def.Name.Replace(" ", "");
                }
            }
            return Block.Name(block);
        }
    }
}
