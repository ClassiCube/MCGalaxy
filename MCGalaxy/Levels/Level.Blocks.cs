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
using MCGalaxy.Blocks;
using MCGalaxy.Blocks.Physics;
using MCGalaxy.DB;
using MCGalaxy.Games;
using MCGalaxy.Maths;
using BlockID = System.UInt16;
using BlockRaw = System.Byte;

namespace MCGalaxy {

    public sealed partial class Level : IDisposable {
        
        public byte[] blocks;
        public byte[][] CustomBlocks;
        public int ChunksX, ChunksY, ChunksZ;
        
        public bool MayHaveCustomBlocks {
            get {
                if (CustomBlocks == null) return false;
                for (int i = 0; i < CustomBlocks.Length; i++)
                    if (CustomBlocks[i] != null) return true;
                return false;
            }
        }
        
        /// <summary> Gets the block at the given coordinates. </summary>
        /// <returns> Undefined behaviour if coordinates are invalid. </returns>
        public BlockID FastGetBlock(int index) {
            byte raw = blocks[index];
            return raw != Block.custom_block ? raw : (BlockID)(Block.Extended | GetExtTile(index));
        }
        
        /// <summary> Gets the block at the given coordinates. </summary>
        /// <returns> Undefined behaviour if coordinates are invalid. </returns>
        public BlockID FastGetBlock(ushort x, ushort y, ushort z) {
            byte raw = blocks[x + Width * (z + y * Length)];
            return raw != Block.custom_block ? raw : (BlockID)(Block.Extended | FastGetExtTile(x, y, z));
        }
        
        /// <summary> Gets the block at the given coordinates. </summary>
        /// <returns> Block.Invalid if coordinates outside map. </returns>
        public BlockID GetBlock(ushort x, ushort y, ushort z) {
            if (x >= Width || y >= Height || z >= Length || blocks == null) return Block.Invalid;
            byte raw = blocks[x + Width * (z + y * Length)];
            return raw != Block.custom_block ? raw : (BlockID)(Block.Extended | FastGetExtTile(x, y, z));
        }
        
        /// <summary> Gets the block at the given coordinates. </summary>
        /// <returns> Block.Invalid if coordinates outside map. </returns>
        public BlockID GetBlock(ushort x, ushort y, ushort z, out int index) {
            if (x >= Width || y >= Height || z >= Length || blocks == null) { index = -1; return Block.Invalid; }
            index = x + Width * (z + y * Length);
            byte raw = blocks[index];
            return raw != Block.custom_block ? raw : (BlockID)(Block.Extended | FastGetExtTile(x, y, z));
        }
        
        /// <summary> Gets whether the block at the given coordinates is air. </summary>
        public bool IsAirAt(ushort x, ushort y, ushort z) {
            if (x >= Width || y >= Height || z >= Length || blocks == null) return false;
            return blocks[x + Width * (z + y * Length)] == Block.Air;
        }
        
        /// <summary> Gets whether the block at the given coordinates is air. </summary>
        public bool IsAirAt(ushort x, ushort y, ushort z, out int index) {
            if (x >= Width || y >= Height || z >= Length || blocks == null) { index = -1; return false; }
            index = x + Width * (z + y * Length);
            return blocks[index] == Block.Air;
        }
        
        public byte GetExtTile(int index) {
            ushort x, y, z;
            IntToPos(index, out x, out y, out z);
            
            int cx = x >> 4, cy = y >> 4, cz = z >> 4;
            byte[] chunk = CustomBlocks[(cy * ChunksZ + cz) * ChunksX + cx];
            return chunk == null ? Block.Air : chunk[(y & 0x0F) << 8 | (z & 0x0F) << 4 | (x & 0x0F)];
        }
        
        public byte FastGetExtTile(ushort x, ushort y, ushort z) {
            int cx = x >> 4, cy = y >> 4, cz = z >> 4;
            byte[] chunk = CustomBlocks[(cy * ChunksZ + cz) * ChunksX + cx];
            return chunk == null ? Block.Air : chunk[(y & 0x0F) << 8 | (z & 0x0F) << 4 | (x & 0x0F)];
        }
        
        public byte RawFallback(BlockID b) {
            BlockDefinition def = CustomBlockDefs[b];
            if (def != null) return def.FallBack;
            return b < Block.CpeCount ? (byte)b : Block.Air;
        }
        
        public void SetTile(int index, byte block) {
            if (blocks == null || index < 0 || index >= blocks.Length) return;
            blocks[index] = block;
            Changed = true;
        }
        
        public void SetTile(ushort x, ushort y, ushort z, byte block) {
            int index = PosToInt(x, y, z);
            if (blocks == null || index < 0) return;
            blocks[index] = block;
            Changed = true;
        }
        
        public void SetExtTile(ushort x, ushort y, ushort z, byte extBlock) {
            int index = PosToInt(x, y, z);
            if (index < 0 || blocks == null) return;
            FastSetExtTile(x, y, z, extBlock);
        }
        
        public void FastSetExtTile(ushort x, ushort y, ushort z, byte extBlock) {
            int cx = x >> 4, cy = y >> 4, cz = z >> 4;
            int cIndex = (cy * ChunksZ + cz) * ChunksX + cx;
            byte[] chunk = CustomBlocks[cIndex];
            
            if (chunk == null) {
                chunk = new byte[16 * 16 * 16];
                CustomBlocks[cIndex] = chunk;
            }
            chunk[(y & 0x0F) << 8 | (z & 0x0F) << 4 | (x & 0x0F)] = extBlock;
        }
        
        public void FastRevertExtTile(ushort x, ushort y, ushort z) {
            int cx = x >> 4, cy = y >> 4, cz = z >> 4;
            int cIndex = (cy * ChunksZ + cz) * ChunksX + cx;
            byte[] chunk = CustomBlocks[cIndex];
            
            if (chunk == null) return;
            chunk[(y & 0x0F) << 8 | (z & 0x0F) << 4 | (x & 0x0F)] = 0;
        }

        bool CheckTNTWarsChange(Player p, ushort x, ushort y, ushort z, ref BlockID block) {
            if (!(block == Block.TNT || block == Block.TNT_Big || block == Block.TNT_Nuke || block == Block.TNT_Small))
                return true;
            
            TntWarsGame game = TntWarsGame.GameIn(p);
            if (game.InZone(x, y, z, true))
                return false;
            
            if (p.CurrentAmountOfTnt == game.Config.MaxPlayerActiveTnt) {
                Player.Message(p, "TNT Wars: Maximum amount of TNT placed"); return false;
            }
            if (p.CurrentAmountOfTnt > game.Config.MaxPlayerActiveTnt) {
                Player.Message(p, "TNT Wars: You have passed the maximum amount of TNT that can be placed!"); return false;
            }
            p.TntAtATime();
            block = Block.TNT_Small;
            return true;
        }
        
        bool CheckRank(Player p) {
            if (p.ZoneSpam <= DateTime.UtcNow) {
                BuildAccess.CheckDetailed(p);
                p.ZoneSpam = DateTime.UtcNow.AddSeconds(2);
            }
            if (p.level == this) return p.AllowBuild;
            
            AccessResult access = BuildAccess.Check(p);
            return access == AccessResult.Whitelisted || access == AccessResult.Allowed;
        }
        
        internal bool BuildIn(BlockID block) {
            if (block == Block.Op_Water || block == Block.Op_Lava || Props[block].IsPortal || Props[block].IsMessageBlock) return false;
            block = Block.Convert(block);
            return block >= Block.Water && block <= Block.StillLava;
        }
        
        public bool CheckAffectPermissions(Player p, ushort x, ushort y, ushort z, BlockID old, BlockID block) {
            if (!p.group.Blocks[old] && !Block.AllowBreak(old) && !BuildIn(old)) return false;
            if (p.PlayingTntWars && !CheckTNTWarsChange(p, x, y, z, ref block)) return false;
            Zone[] zones = Zones.Items;
            if (zones.Length == 0) return CheckRank(p);
            
            // Check zones specifically allowed in
            for (int i = 0; i < zones.Length; i++) {
                Zone zn = zones[i];
                if (x < zn.MinX || x > zn.MaxX || y < zn.MinY || y > zn.MaxY || z < zn.MinZ || z > zn.MaxZ) continue;
                
                ZoneConfig cfg = zn.Config;
                if (cfg.BuildBlacklist.Count > 0 && cfg.BuildBlacklist.CaselessContains(p.name)) break;
                
                if (p.group.Permission >= cfg.BuildMin) return true;
                if (cfg.BuildWhitelist.Count > 0 && cfg.BuildWhitelist.CaselessContains(p.name)) return true;
            }
            
            // Check zones denied from
            for (int i = 0; i < zones.Length; i++) {
                Zone zn = zones[i];
                if (x < zn.MinX || x > zn.MaxX || y < zn.MinY || y > zn.MaxY || z < zn.MinZ || z > zn.MaxZ) continue;
                AccessResult access = zn.Access.Check(p);
                if (access == AccessResult.Allowed || access == AccessResult.Whitelisted) continue;

                if (p.ZoneSpam > DateTime.UtcNow) return false;
                zn.Access.CheckDetailed(p);
                p.ZoneSpam = DateTime.UtcNow.AddSeconds(2);
                return false;
            }
            return CheckRank(p);
        }
        
        public void Blockchange(Player p, ushort x, ushort y, ushort z, BlockID block) {
            if (DoBlockchange(p, x, y, z, block) == 2) {
                Player.GlobalBlockchange(this, x, y, z, block);
            }
        }
        
        /// <summary> Returns: <br/>
        /// 0 - block change was not performed <br/>
        /// 1 - old block was same as new block visually (e.g. white to door_white)<br/>
        /// 2 - old block was different to new block visually </summary>
        /// <remarks> The return code can be used to avoid sending redundant block changes. </remarks>
        public int DoBlockchange(Player p, ushort x, ushort y, ushort z, BlockID block, bool drawn = false) {
            string errorLocation = "start";
            try
            {
                if (x >= Width || y >= Height || z >= Length) return 0;
                BlockID old = GetBlock(x, y, z);

                errorLocation = "Permission checking";
                if (!CheckAffectPermissions(p, x, y, z, old, block)) {
                    p.RevertBlock(x, y, z); return 0;
                }
                if (old == block) return 0;

                if (old == Block.Sponge && physics > 0 && block != Block.Sponge) {
                    OtherPhysics.DoSpongeRemoved(this, PosToInt(x, y, z), false);
                }
                if (old == Block.LavaSponge && physics > 0 && block != Block.LavaSponge) {
                    OtherPhysics.DoSpongeRemoved(this, PosToInt(x, y, z), true);
                }

                p.SessionModified++;
                p.TotalModified++;
                
                if (drawn) p.TotalDrawn++;
                else if (block == Block.Air) p.TotalDeleted++;
                else p.TotalPlaced++;
                
                errorLocation = "Setting tile";
                if (block >= Block.Extended) {
                    SetTile(x, y, z, Block.custom_block);   
                    FastSetExtTile(x, y, z, (BlockRaw)block);
                } else {
                    SetTile(x, y, z, (BlockRaw)block);   
                    if (old >= Block.Extended) {
                        FastRevertExtTile(x, y, z);
                    }
                }

                errorLocation = "Adding physics";
                if (p.PlayingTntWars && block == Block.TNT_Small) AddTntCheck(PosToInt(x, y, z), p);
                if (physics > 0 && ActivatesPhysics(block)) AddCheck(PosToInt(x, y, z));

                Changed = true;
                backedup = false;
                
                return Block.VisuallyEquals(old, block) ? 1 : 2;
            } catch (Exception e) {
                Logger.LogError(e);
                Chat.MessageOps(p.name + " triggered a non-fatal error on " + ColoredName + ", %Sat location: " + errorLocation);
                Logger.Log(LogType.Warning, "{0} triggered a non-fatal error on {1}, %Sat location: {2}",
                           p.name, ColoredName, errorLocation);
                return 0;
            }
        }
        
        void AddTntCheck(int b, Player p) {
            PhysicsArgs args = default(PhysicsArgs);
            args.Type1 = PhysicsArgs.Custom;
            args.Value1 = (byte)p.SessionID;
            args.Value2 = (byte)(p.SessionID >> 8);
            args.Data = (byte)(p.SessionID >> 16);
            AddCheck(b, false, args);
        }
        
        public void Blockchange(int b, BlockID block, bool overRide = false,
                                PhysicsArgs data = default(PhysicsArgs), bool addUndo = true) { //Block change made by physics
            if (DoPhysicsBlockchange(b, block, overRide, data, addUndo))
                Player.GlobalBlockchange(this, b, block);
        }
        
        public void Blockchange(ushort x, ushort y, ushort z, BlockID block, bool overRide = false,
                                PhysicsArgs data = default(PhysicsArgs), bool addUndo = true) {
            Blockchange(PosToInt(x, y, z), block, overRide, data, addUndo); //Block change made by physics
        }
        
        public void Blockchange(ushort x, ushort y, ushort z, BlockID block) {
            Blockchange(PosToInt(x, y, z), block, false, default(PhysicsArgs)); //Block change made by physics
        }
        
        internal bool DoPhysicsBlockchange(int b, BlockID block, bool overRide = false,
                                           PhysicsArgs data = default(PhysicsArgs), bool addUndo = true) {
            if (blocks == null || b < 0 || b >= blocks.Length) return false;
            BlockID old = blocks[b];
            old = old != Block.custom_block ? old : (BlockID)(Block.Extended | GetExtTile(b));
            
            try
            {
                if (!overRide) {
                    if (Props[old].OPBlock || (Props[block].OPBlock && data.Raw != 0)) return false;
                }

                if (old == Block.Sponge && physics > 0 && block != Block.Sponge) {
                    OtherPhysics.DoSpongeRemoved(this, b, false);
                }
                if (old == Block.LavaSponge && physics > 0 && block != Block.LavaSponge) {
                    OtherPhysics.DoSpongeRemoved(this, b, true);
                }

                if (addUndo) {
                    UndoPos uP = default(UndoPos);
                    uP.Index = b;
                    uP.SetData(old, block);

                    if (UndoBuffer.Count < ServerConfig.PhysicsUndo) {
                        UndoBuffer.Add(uP);
                    } else {
                        if (currentUndo >= ServerConfig.PhysicsUndo)
                            currentUndo = 0;
                        UndoBuffer[currentUndo] = uP;
                    }
                    currentUndo++;
                }
                
                Changed = true;
                if (block >= Block.Extended) {
                    blocks[b] = Block.custom_block;
                    ushort x, y, z;
                    IntToPos(b, out x, out y, out z);
                    FastSetExtTile(x, y, z, (BlockRaw)block);
                } else {
                    blocks[b] = (BlockRaw)block;
                    if (old >= Block.Extended) {               
                        ushort x, y, z;
                        IntToPos(b, out x, out y, out z);
                        FastRevertExtTile(x, y, z);
                    }
                }
                if (physics > 0 && (ActivatesPhysics(block) || data.Raw != 0)) {
                    AddCheck(b, false, data);
                }
                
                // Save bandwidth sending identical looking blocks, like air/op_air changes.
                return !Block.VisuallyEquals(old, block);
            } catch {
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
        
        public bool IsValidPos(int x, int y, int z) {
            return x >= 0 && y >= 0 && z >= 0 && x < Width && y < Height && z < Length;
        }
        
        public void UpdateBlock(Player p, ushort x, ushort y, ushort z, BlockID block,
                                ushort flags = BlockDBFlags.ManualPlace, bool buffered = false) {
            BlockID old = GetBlock(x, y, z);
            bool drawn = (flags & BlockDBFlags.ManualPlace) != 0;
            int type = DoBlockchange(p, x, y, z, block, drawn);
            if (type == 0) return; // no block change performed
            
            BlockDB.Cache.Add(p, x, y, z, flags, old, block);
            if (type == 1) return; // not different visually
            
            int index = PosToInt(x, y, z);
            if (buffered) BlockQueue.Add(p, index, block);
            else Player.GlobalBlockchange(this, x, y, z, block);
        }
        
        public BlockDefinition GetBlockDef(BlockID block) {
            if (block == Block.Air) return null;           
            if (Block.IsPhysicsType(block)) {
                return CustomBlockDefs[Block.Convert(block)];
            } else {
                return CustomBlockDefs[block];
            }
        }
        
        public byte CollideType(BlockID block) {
            BlockDefinition def = GetBlockDef(block);
            byte collide = def != null ? def.CollideType : MCGalaxy.Blocks.CollideType.Solid;
            
            if (def == null && block < Block.Extended)
                return DefaultSet.Collide(Block.Convert(block));
            return collide;
        }
        
        public bool LightPasses(BlockID block) {
            BlockDefinition def = GetBlockDef(block);
            if (def != null) return !def.BlocksLight || def.BlockDraw == DrawType.TransparentThick || def.MinZ > 0;
            return Block.LightPass(block);
        }
    }
}
