/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
    Dual-licensed under the    Educational Community License, Version 2.0 and
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
using System.Linq;
using MCGalaxy.SQL;

namespace MCGalaxy {

    public sealed partial class Level : IDisposable {
        
        public byte[] blocks;
        public byte[][] CustomBlocks;
        public int ChunksX, ChunksY, ChunksZ;
        
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
        
        public byte GetCustomTile(ushort x, ushort y, ushort z) {
            int index = PosToInt(x, y, z);
            if (index < 0 || blocks == null) return Block.Zero;
            
            int cx = x >> 4, cy = y >> 4, cz = z >> 4;
            byte[] chunk = CustomBlocks[(cy * ChunksZ + cz) * ChunksX + cx];
            return chunk == null ? (byte)0 :
                chunk[(y & 0x0F) << 8 | (z & 0x0F) << 4 | (x & 0x0F)];
        }
        
        public void SetTile(int b, byte type) {
            if (blocks == null || b < 0 || b >= blocks.Length) return;
            blocks[b] = type;
        }
        
        public void SetTile(ushort x, ushort y, ushort z, byte type) {
            int b = PosToInt(x, y, z);
            if (blocks == null || b < 0) return;
            blocks[b] = type;
        }
        
        public void SetCustomTile(ushort x, ushort y, ushort z, byte type) {
            int index = PosToInt(x, y, z);
            if (index < 0 || blocks == null) return;
            
            int cx = x >> 4, cy = y >> 4, cz = z >> 4;
            int cIndex = (cy * ChunksZ + cz) * ChunksX + cx;
            byte[] chunk = CustomBlocks[cIndex];
            if (chunk == null) {
                chunk = new byte[16 * 16 * 16];
                CustomBlocks[cIndex] = chunk;
            }
            chunk[(y & 0x0F) << 8 | (z & 0x0F) << 4 | (x & 0x0F)] = type;
        }
        
        public void SetTile(ushort x, ushort y, ushort z, byte type, Player p, byte extType = 0) {
            int b = PosToInt(x, y, z);
            if (blocks == null || b < 0) return;
            
            byte oldType = blocks[b];
            blocks[b] = type;
            if (p == null)
                return;
            Level.BlockPos bP;
            bP.name = p.name;
            bP.TimePerformed = DateTime.Now;
            bP.index = b;
            bP.type = type;
            bP.extType = extType;
            bP.deleted = bP.type == 0;
            blockCache.Add(bP);
            
            Player.UndoPos Pos;
            Pos.x = x; Pos.y = y; Pos.z = z;
            Pos.mapName = this.name;
            Pos.type = oldType;
            Pos.newtype = type;
            Pos.timePlaced = DateTime.Now;
            p.UndoBuffer.Add(Pos);
        }

        public void Blockchange(Player p, ushort x, ushort y, ushort z, byte type, byte extType = 0)
        {
            string errorLocation = "start";
        retry:
            try
            {
                if (x < 0 || y < 0 || z < 0) return;
                if (x >= Width || y >= Height || z >= Length) return;

                byte b = GetTile(x, y, z);

                errorLocation = "Block rank checking";
                if (!Block.AllowBreak(b))
                {
                    if (!Block.canPlace(p, b) && !Block.BuildIn(b))
                    {
                        p.RevertBlock(x, y, z); return;
                    }
                }
                errorLocation = "Allowed to place tnt there (TNT Wars)";
                if (type == Block.tnt || type == Block.smalltnt || type == Block.bigtnt || type == Block.nuketnt)
                {
                    if (p.PlayingTntWars)
                    {
                        if (TntWarsGame.GetTntWarsGame(p).InZone(x, y, z, true))
                        {
                            p.RevertBlock(x, y, z); return;
                        }
                    }
                }
                errorLocation = "Max tnt for TNT Wars checking";
                if (type == Block.tnt || type == Block.smalltnt || type == Block.bigtnt || type == Block.nuketnt)
                {
                    if (p.PlayingTntWars)
                    {
                        if (p.CurrentAmountOfTnt == TntWarsGame.GetTntWarsGame(p).TntPerPlayerAtATime)
                        {
                            Player.SendMessage(p, "TNT Wars: Maximum amount of TNT placed");
                            p.RevertBlock(x, y, z); return;
                        }
                        if (p.CurrentAmountOfTnt > TntWarsGame.GetTntWarsGame(p).TntPerPlayerAtATime)
                        {
                            Player.SendMessage(p, "TNT Wars: You have passed the maximum amount of TNT that can be placed!");
                            p.RevertBlock(x, y, z); return;
                        }
                        else
                        {
                            p.TntAtATime();
                        }
                    }
                }

                errorLocation = "TNT Wars switch TNT block to smalltnt";
                if ((type == Block.tnt || type == Block.bigtnt || type == Block.nuketnt || type == Block.smalltnt) && p.PlayingTntWars)
                {
                    type = Block.smalltnt;
                }

                errorLocation = "Zone checking";

                #region zones

                bool AllowBuild = true, foundDel = false, inZone = false;
                string Owners = "";
                var toDel = new List<Zone>();
                if ((p.group.Permission < LevelPermission.Admin || p.ZoneCheck || p.zoneDel) && !Block.AllowBreak(b))
                {
                    if (ZoneList.Count == 0) AllowBuild = true;
                    else
                    {
                        for (int index = 0; index < ZoneList.Count; index++)
                        {
                            Zone Zn = ZoneList[index];
                            if (Zn.smallX <= x && x <= Zn.bigX && Zn.smallY <= y && y <= Zn.bigY && Zn.smallZ <= z &&
                                z <= Zn.bigZ)
                            {
                                inZone = true;
                                if (p.zoneDel)
                                {
                                    //DB
                                    Database.executeQuery("DELETE FROM `Zone" + p.level.name + "` WHERE Owner='" +
                                                          Zn.Owner + "' AND SmallX='" + Zn.smallX + "' AND SMALLY='" +
                                                          Zn.smallY + "' AND SMALLZ='" + Zn.smallZ + "' AND BIGX='" +
                                                          Zn.bigX + "' AND BIGY='" + Zn.bigY + "' AND BIGZ='" + Zn.bigZ +
                                                          "'");
                                    toDel.Add(Zn);

                                    p.RevertBlock(x, y, z);
                                    Player.SendMessage(p, "Zone deleted for &b" + Zn.Owner);
                                    foundDel = true;
                                }
                                else
                                {
                                    if (Zn.Owner.Substring(0, 3) == "grp")
                                    {
                                        if (Group.Find(Zn.Owner.Substring(3)).Permission <= p.group.Permission &&
                                            !p.ZoneCheck)
                                        {
                                            AllowBuild = true;
                                            break;
                                        }
                                        AllowBuild = false;
                                        Owners += ", " + Zn.Owner.Substring(3);
                                    }
                                    else
                                    {
                                        if (Zn.Owner.ToLower() == p.name.ToLower() && !p.ZoneCheck)
                                        {
                                            AllowBuild = true;
                                            break;
                                        }
                                        AllowBuild = false;
                                        Owners += ", " + Zn.Owner;
                                    }
                                }
                            }
                        }
                    }

                    if (p.zoneDel)
                    {
                        if (!foundDel) Player.SendMessage(p, "No zones found to delete.");
                        else
                        {
                            foreach (Zone Zn in toDel)
                            {
                                ZoneList.Remove(Zn);
                            }
                        }
                        p.zoneDel = false;
                        return;
                    }

                    if (!AllowBuild || p.ZoneCheck)
                    {
                        if (Owners != "") Player.SendMessage(p, "This zone belongs to &b" + Owners.Remove(0, 2) + ".");
                        else Player.SendMessage(p, "This zone belongs to no one.");

                        p.ZoneSpam = DateTime.Now;
                        p.SendBlockchange(x, y, z, b);

                        if (p.ZoneCheck) if (!p.staticCommands) p.ZoneCheck = false;
                        return;
                    }
                }

                #endregion

                errorLocation = "Map rank checking";
                if (Owners == "")
                {
                    if (p.group.Permission < permissionbuild && (!inZone || !AllowBuild))
                    {
                        Player.SendMessage(p, "Must be at least " + PermissionToName(permissionbuild) + " to build here");
                        p.RevertBlock(x, y, z); return;
                    }
                }

                errorLocation = "Map Max Rank Checking";
                if (Owners == "")
                {
                    if (p.group.Permission > perbuildmax && (!inZone || !AllowBuild))
                    {
                        if (!p.group.CanExecute(Command.all.Find("perbuildmax")))
                        {
                            Player.SendMessage(p, "Your rank must be " + perbuildmax + " or lower to build here!");
                            p.RevertBlock(x, y, z); return;
                        }
                    }
                }

                errorLocation = "Block sending";
                if (Block.Convert(b) != Block.Convert(type) && !Instant)
                    Player.GlobalBlockchange(this, x, y, z, type);

                if (b == Block.sponge && physics > 0 && type != Block.sponge) PhysSpongeRemoved(PosToInt(x, y, z));
                if (b == Block.lava_sponge && physics > 0 && type != Block.lava_sponge)
                    PhysSpongeRemoved(PosToInt(x, y, z), true);

                errorLocation = "Undo buffer filling";
                Player.UndoPos Pos;
                Pos.x = x;
                Pos.y = y;
                Pos.z = z;
                Pos.mapName = name;
                Pos.type = b;
                Pos.newtype = type;
                Pos.timePlaced = DateTime.Now;
                p.UndoBuffer.Add(Pos);

                errorLocation = "Setting tile";
                p.loginBlocks++;
                p.overallBlocks++;
                SetTile(x, y, z, type); //Updates server level blocks

                errorLocation = "Growing grass";
                if (GetTile(x, (ushort)(y - 1), z) == Block.grass && GrassDestroy && !Block.LightPass(type))
                {
                    Blockchange(p, x, (ushort)(y - 1), z, Block.dirt);
                }

                errorLocation = "Adding physics";
                if (p.PlayingTntWars && type == Block.smalltnt) AddCheck(PosToInt(x, y, z), "", false, p);
                if (physics > 0) if (Block.Physics(type)) AddCheck(PosToInt(x, y, z), "", false, p);

                changed = true;
                backedup = false;
            }
            catch (OutOfMemoryException)
            {
                Player.SendMessage(p, "Undo buffer too big! Cleared!");
                p.UndoBuffer.Clear();
                goto retry;
            }
            catch (Exception e)
            {
                Server.ErrorLog(e);
                Chat.GlobalMessageOps(p.name + " triggered a non-fatal error on " + name);
                Chat.GlobalMessageOps("Error location: " + errorLocation);
                Server.s.Log(p.name + " triggered a non-fatal error on " + name);
                Server.s.Log("Error location: " + errorLocation);
            }
        }
        
        public void Blockchange(int b, byte type, bool overRide = false, string extraInfo = "") { //Block change made by physics
            if (b < 0 || b >= blocks.Length || blocks == null) return;
            if (b >= blocks.Length) return;
            byte oldBlock = blocks[b];

            try
            {
                if (!overRide)
                    if (Block.OPBlocks(oldBlock) || (Block.OPBlocks(type) && extraInfo != "")) return;

                if (Block.Convert(oldBlock) != Block.Convert(type))
                    //Should save bandwidth sending identical looking blocks, like air/op_air changes.
                    Player.GlobalBlockchange(this, b, type);

                if (b == Block.sponge && physics > 0 && type != Block.sponge)
                    PhysSpongeRemoved(b);

                if (b == Block.lava_sponge && physics > 0 && type != Block.lava_sponge)
                    PhysSpongeRemoved(b, true);

                UndoPos uP;
                uP.location = b;
                uP.newType = type;
                uP.oldType = oldBlock;
                uP.timePerformed = DateTime.Now;

                if (currentUndo > Server.physUndo) {
                    currentUndo = 0;
                    UndoBuffer[currentUndo] = uP;
                } else if (UndoBuffer.Count < Server.physUndo) {
                    currentUndo++;
                    UndoBuffer.Add(uP);
                } else {
                    currentUndo++;
                    UndoBuffer[currentUndo] = uP;
                }

                blocks[b] = type;
                if (physics > 0 && ((Block.Physics(type) || extraInfo != "")))
                    AddCheck(b, extraInfo);
            } catch {
                blocks[b] = type;
            }
        }
        
        public void Blockchange(ushort x, ushort y, ushort z, byte type, bool overRide = false, string extraInfo = "") {
            Blockchange(PosToInt(x, y, z), type, overRide, extraInfo); //Block change made by physics
        }

        public int PosToInt(ushort x, ushort y, ushort z) {
            if (x < 0 || x >= Width || y < 0 || y >= Height || z < 0 || z >= Length)
                return -1;
            return x + (z * Width) + (y * Width * Length);
            //alternate method: (h * widthY + y) * widthX + x;
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
    }
}
