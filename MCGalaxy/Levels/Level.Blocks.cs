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
using MCGalaxy.Blocks;
using MCGalaxy.Blocks.Physics;
using MCGalaxy.DB;
using MCGalaxy.Games;
using MCGalaxy.Maths;

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
        
        public byte GetTile(ushort x, ushort y, ushort z) {
            int index = PosToInt(x, y, z);
            if (index < 0 || blocks == null) return Block.Invalid;
            return blocks[index];
        }
        
        /// <summary> Gets the block at the given coordinates. </summary>
        /// <returns> Block.Invalid if coordinates outside map. </returns>
        public ExtBlock GetBlock(ushort x, ushort y, ushort z) {
            if (x >= Width || y >= Height || z >= Length || blocks == null) return ExtBlock.Invalid;
            ExtBlock block;
            
            block.BlockID = blocks[x + Width * (z + y * Length)];
            block.ExtID = block.BlockID == Block.custom_block ? GetExtTileNoCheck(x, y, z) : Block.Air;
            return block;
        }       
        
        /// <summary> Gets the block at the given coordinates. </summary>
        /// <returns> Block.Invalid if coordinates outside map. </returns>
        public ExtBlock GetBlock(ushort x, ushort y, ushort z, out int index) {
            if (x >= Width || y >= Height || z >= Length || blocks == null) { index = -1; return ExtBlock.Invalid; }
            ExtBlock block;
            
            index = x + Width * (z + y * Length);
            block.BlockID = blocks[index];
            block.ExtID = block.BlockID == Block.custom_block ? GetExtTileNoCheck(x, y, z) : Block.Air;
            return block;
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

        public byte GetTile(int b) {
            ushort x = 0, y = 0, z = 0;
            IntToPos(b, out x, out y, out z);
            return GetTile(x, y, z);
        }
        
        public byte GetExtTile(ushort x, ushort y, ushort z) {
            if (x >= Width || y >= Height || z >= Length || blocks == null) 
                return Block.Invalid;
            
            int cx = x >> 4, cy = y >> 4, cz = z >> 4;
            byte[] chunk = CustomBlocks[(cy * ChunksZ + cz) * ChunksX + cx];
            return chunk == null ? Block.Air :
                chunk[(y & 0x0F) << 8 | (z & 0x0F) << 4 | (x & 0x0F)];
        }
        
        public byte GetExtTile(int index) {
            ushort x, y, z;
            IntToPos(index, out x, out y, out z);
            
            int cx = x >> 4, cy = y >> 4, cz = z >> 4;
            byte[] chunk = CustomBlocks[(cy * ChunksZ + cz) * ChunksX + cx];
            return chunk == null ? Block.Air :
                chunk[(y & 0x0F) << 8 | (z & 0x0F) << 4 | (x & 0x0F)];
        }
        
        public byte GetExtTileNoCheck(ushort x, ushort y, ushort z) {
            int cx = x >> 4, cy = y >> 4, cz = z >> 4;
            byte[] chunk = CustomBlocks[(cy * ChunksZ + cz) * ChunksX + cx];
            return chunk == null ? Block.Air :
                chunk[(y & 0x0F) << 8 | (z & 0x0F) << 4 | (x & 0x0F)];
        }
        
        public byte GetFallbackExtTile(int index) {
            byte tile = GetExtTile(index);
            BlockDefinition def = CustomBlockDefs[tile];
            return def == null ? Block.Air : def.FallBack;
        }
        
        public byte RawFallback(byte raw) {
            BlockDefinition def = CustomBlockDefs[raw];
            if (def != null) return def.FallBack;
            return raw < Block.CpeCount ? raw : Block.Air;
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

        bool CheckTNTWarsChange(Player p, ushort x, ushort y, ushort z, ref byte block) {
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
        
        bool CheckZonePerms(Player p, ushort x, ushort y, ushort z, ref bool inZone) {
            bool zoneAllow = true;            
            for (int i = 0; i < ZoneList.Count; i++) {
                Zone zn = ZoneList[i];
                if (x < zn.MinX || x > zn.MaxX || y < zn.MinY || y > zn.MaxY || z < zn.MinZ || z > zn.MaxZ)
                    continue;
                
                inZone = true;
                if (zn.Owner.Length >= 3 && zn.Owner.StartsWith("grp")) {
                    Group grp = Group.Find(zn.Owner.Substring(3));
                    if (grp != null && grp.Permission <= p.Rank) return true;
                } else {
                    if (zn.Owner.CaselessEq(p.name)) return true;
                }
                zoneAllow = false;
            }
            
            if (zoneAllow) return true;
            if (p.ZoneSpam > DateTime.UtcNow) return false;
            
            Player.Message(p, FindZoneOwners(p, x, y, z));
            p.ZoneSpam = DateTime.UtcNow.AddSeconds(2);
            return false;
        }
        
        internal string FindZoneOwners(Player p, ushort x, ushort y, ushort z) {
            string owners = "";
            for (int i = 0; i < ZoneList.Count; i++) {
                Zone zn = ZoneList[i];
                if (x < zn.MinX || x > zn.MaxX || y < zn.MinY || y > zn.MaxY || z < zn.MinZ || z > zn.MaxZ)
                    continue;
                
                if (zn.Owner.Length >= 3 && zn.Owner.StartsWith("grp")) {
                    owners += ", " + Group.GetColoredName(zn.Owner.Substring(3));
                } else {
                    owners += ", " + PlayerInfo.GetColoredName(p, zn.Owner);
                }
            }
            
            if (owners.Length == 0) return "No zones affect this block";
            return "This zone belongs to " + owners.Remove(0, 2) + ".";
        }
        
        bool CheckRank(Player p) {
            if (p.ZoneSpam <= DateTime.UtcNow) {
                BuildAccess.CheckDetailed(p);
                p.ZoneSpam = DateTime.UtcNow.AddSeconds(2);
            }
            if (p.level == this) return p.AllowBuild;
            
            AccessResult access = BuildAccess.Check(p);
            return access == AccessResult.Whitelisted 
                || access == AccessResult.Allowed;
        }
        
        public bool CheckAffectPermissions(Player p, ushort x, ushort y, ushort z, ExtBlock old, ExtBlock block) {
            if (!p.group.Blocks[old.BlockID] && !Block.AllowBreak(old.BlockID) && !Block.BuildIn(old.BlockID)) return false;
            if (p.PlayingTntWars && !CheckTNTWarsChange(p, x, y, z, ref block.BlockID)) return false;
            
            bool inZone = false;
            if (ZoneList.Count > 0 && !CheckZonePerms(p, x, y, z, ref inZone)) return false;
            return inZone || CheckRank(p);
        }
        
        public void Blockchange(Player p, ushort x, ushort y, ushort z, ExtBlock block) {
            if (DoBlockchange(p, x, y, z, block) == 2)
                Player.GlobalBlockchange(this, x, y, z, block);
        }
        
        /// <summary> Returns: <br/> 
        /// 0 - block change was not performed <br/>
        /// 1 - old block was same as new block visually (e.g. white to door_white)<br/>
        /// 2 - old block was different to new block visually </summary>
        /// <remarks> The return code can be used to avoid sending redundant block changes. </remarks>
        public int DoBlockchange(Player p, ushort x, ushort y, ushort z, ExtBlock block, bool drawn = false) {
            string errorLocation = "start";
            try
            {
                if (x >= Width || y >= Height || z >= Length) return 0;
                ExtBlock old = GetBlock(x, y, z);

                errorLocation = "Permission checking";
                if (!CheckAffectPermissions(p, x, y, z, old, block)) {
                    p.RevertBlock(x, y, z); return 0;
                }
                if (old == block) return 0;

                if (old.BlockID == Block.Sponge && physics > 0 && block.BlockID != Block.Sponge)
                    OtherPhysics.DoSpongeRemoved(this, PosToInt(x, y, z), false);
                if (old.BlockID == Block.LavaSponge && physics > 0 && block.BlockID != Block.LavaSponge)
                    OtherPhysics.DoSpongeRemoved(this, PosToInt(x, y, z), true);

                p.SessionModified++;
                p.TotalModified++;
            
                if (drawn) p.TotalDrawn++;
                else if (block.BlockID == Block.Air) p.TotalDeleted++;
                else p.TotalPlaced++;
            
                errorLocation = "Setting tile";
                SetTile(x, y, z, block.BlockID);
                if (old.BlockID == Block.custom_block && block.BlockID != Block.custom_block)
                    RevertExtTileNoCheck(x, y, z);
                if (block.BlockID == Block.custom_block)
                    SetExtTileNoCheck(x, y, z, block.ExtID);

                errorLocation = "Adding physics";
                if (p.PlayingTntWars && block.BlockID == Block.TNT_Small) AddTntCheck(PosToInt(x, y, z), p);
                if (physics > 0 && ActivatesPhysics(block)) AddCheck(PosToInt(x, y, z));

                Changed = true;
                backedup = false;
                
                return old.VisuallyEquals(block) ? 1 : 2;
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
        
        public void Blockchange(int b, ExtBlock block, bool overRide = false, 
                                PhysicsArgs data = default(PhysicsArgs), bool addUndo = true) { //Block change made by physics
            if (DoPhysicsBlockchange(b, block, overRide, data, addUndo))
                Player.GlobalBlockchange(this, b, block);
        }
        
        public void Blockchange(ushort x, ushort y, ushort z, ExtBlock block, bool overRide = false, 
                                PhysicsArgs data = default(PhysicsArgs), bool addUndo = true) {
            Blockchange(PosToInt(x, y, z), block, overRide, data, addUndo); //Block change made by physics
        }
        
        public void Blockchange(ushort x, ushort y, ushort z, ExtBlock block) {
            Blockchange(PosToInt(x, y, z), block, false, default(PhysicsArgs)); //Block change made by physics
        }
        
        internal bool DoPhysicsBlockchange(int b, ExtBlock block, bool overRide = false, 
                                           PhysicsArgs data = default(PhysicsArgs), bool addUndo = true) {
            if (blocks == null || b < 0 || b >= blocks.Length) return false;
            ExtBlock old;
            old.BlockID = blocks[b];
            old.ExtID = old.BlockID == Block.custom_block ? GetExtTile(b) : Block.Air;
            
            try
            {
                if (!overRide) {
                    if (Props[old.Index].OPBlock || (Props[block.Index].OPBlock && data.Raw != 0)) 
                        return false;
                }

                if (old.BlockID == Block.Sponge && physics > 0 && block.BlockID != Block.Sponge)
                    OtherPhysics.DoSpongeRemoved(this, b, false);

                if (old.BlockID == Block.LavaSponge && physics > 0 && block.BlockID != Block.LavaSponge)
                    OtherPhysics.DoSpongeRemoved(this, b, true);

                if (addUndo) {
                    UndoPos uP = default(UndoPos);
                    uP.index = b;
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

                blocks[b] = block.BlockID;
                Changed = true;
                if (block.BlockID == Block.custom_block) {
                    ushort x, y, z;
                    IntToPos(b, out x, out y, out z);
                    SetExtTileNoCheck(x, y, z, block.ExtID);
                } else if (old.BlockID == Block.custom_block) {
                    ushort x, y, z;
                    IntToPos(b, out x, out y, out z);
                    RevertExtTileNoCheck(x, y, z);
                }                
                if (physics > 0 && (ActivatesPhysics(block) || data.Raw != 0))
                    AddCheck(b, false, data);
                
                // Save bandwidth sending identical looking blocks, like air/op_air changes.
                return !old.VisuallyEquals(block);
            } catch {
                blocks[b] = block.BlockID;
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
        
        public void UpdateBlock(Player p, ushort x, ushort y, ushort z, ExtBlock block,
                                ushort flags = BlockDBFlags.ManualPlace, bool buffered = false) {
            ExtBlock old = GetBlock(x, y, z);
            bool drawn = (flags & BlockDBFlags.ManualPlace) != 0;
            int type = DoBlockchange(p, x, y, z, block, drawn);
            if (type == 0) return; // no block change performed
            
            BlockDB.Cache.Add(p, x, y, z, flags, old, block);
            if (type == 1) return; // not different visually
            
            int index = PosToInt(x, y, z);
            if (buffered) BlockQueue.Addblock(p, index, block);
            else Player.GlobalBlockchange(this, x, y, z, block);
        }
        
        public BlockDefinition GetBlockDef(ExtBlock block) {
            if (block.BlockID == Block.custom_block) return CustomBlockDefs[block.ExtID];
            if (block.BlockID == Block.Air) return null;
            
            if (block.BlockID >= Block.CpeCount) {
                return CustomBlockDefs[Block.Convert(block.BlockID)];
            } else {
                return CustomBlockDefs[block.BlockID];
            }
        }
        
        public string BlockName(ExtBlock block) {
            if (block.IsPhysicsType) return Block.Name(block.BlockID);
            BlockDefinition def = GetBlockDef(block);
            if (def != null) return def.Name.Replace(" ", "");
            
            return block.BlockID != Block.custom_block ? Block.Name(block.BlockID) : block.ExtID.ToString();
        }
        
        public byte CollideType(ExtBlock block) {
            BlockDefinition def = GetBlockDef(block);
            byte collide = def != null ? def.CollideType : MCGalaxy.Blocks.CollideType.Solid;
            
            if (def == null && !block.IsCustomType)
                return DefaultSet.Collide(Block.Convert(block.BlockID));
            return collide;
        }
        
        public bool LightPasses(ExtBlock block) {
            BlockDefinition def = GetBlockDef(block);
            if (def != null) return !def.BlocksLight || def.BlockDraw != DrawType.Opaque || def.MinZ > 0;
            return Block.LightPass(block.BlockID);
        }
    }
}
