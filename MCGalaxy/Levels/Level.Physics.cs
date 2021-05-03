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
using System.Threading;
using MCGalaxy.Blocks;
using MCGalaxy.Blocks.Physics;
using MCGalaxy.Events.LevelEvents;
using MCGalaxy.Games;
using MCGalaxy.Network;
using BlockID = System.UInt16;

namespace MCGalaxy {
    
    public enum PhysicsState { Stopped, Warning, Other }

    public sealed partial class Level : IDisposable {
        
        readonly object updateLock = new object();
        readonly object checkLock = new object();
        
        public void SetPhysics(int level) {
            if (physics == 0 && level != 0 && blocks != null) {
                for (int i = 0; i < blocks.Length; i++)
                    // Optimization hack, since no blocks under 183 ever need a restart
                    if (blocks[i] > 183 && Block.NeedRestart(blocks[i]))
                        AddCheck(i);
            }
            
            if (physics != level) OnPhysicsLevelChangedEvent.Call(this, level);
            if (level > 0 && physics == 0) StartPhysics();
            
            Physicsint     = level;
            Config.Physics = level;
        }
        
        public void StartPhysics() {
            lock (physThreadLock) {
                if (physThread != null && physThread.ThreadState == ThreadState.Running) return;
                if (ListCheck.Count == 0 || physThreadStarted) return;
                
                physThread = new Thread(PhysicsLoop);
                physThread.Name = "MCG_Physics_" + name;
                physThread.Start();
                physThreadStarted = true;
            }
        }

        void PhysicsLoop() {
            int wait = Config.PhysicsSpeed;
            while (true) {
                try {
                    
                    if (PhysicsPaused) {
                        if (physics == 0) break;
                        Thread.Sleep(500); continue; 
                    } 
                    
                    if (wait > 0) Thread.Sleep(wait);
                    if (physics == 0) break;
                    
                    // No block calculations in this tick
                    if (ListCheck.Count == 0) {
                        lastCheck = 0;
                        wait = Config.PhysicsSpeed;
                        continue;
                    }

                    DateTime tickStart = default(DateTime);
                    try {
                        lock (physTickLock) {
                            tickStart = DateTime.UtcNow;
                            PhysicsTick();
                        }
                    } catch (Exception ex) {
                        Logger.LogError("Error in physics tick", ex);
                    }

                    // Measure how long this physics tick took to execute
                    TimeSpan elapsed = DateTime.UtcNow - tickStart;
                    wait = Config.PhysicsSpeed - (int)elapsed.TotalMilliseconds;

                    // Check if tick took too long to execute (server is falling behind)
                    if (wait < (int)(-Config.PhysicsOverload * 0.75f)) {
                        if (wait < -Config.PhysicsOverload) {
                            if (!Server.Config.PhysicsRestart) SetPhysics(0);
                            ClearPhysics();
                            Chat.MessageGlobal("Physics shutdown on {0}", ColoredName);
                            
                            Logger.Log(LogType.Warning, "Physics shutdown on " + name);
                            OnPhysicsStateChangedEvent.Call(this, PhysicsState.Stopped);
                            wait = Config.PhysicsSpeed;
                        } else {
                            Message("Physics warning!");
                            Logger.Log(LogType.Warning, "Physics warning on " + name);
                            OnPhysicsStateChangedEvent.Call(this, PhysicsState.Warning);
                        }
                    }
                } catch {
                    wait = Config.PhysicsSpeed;
                }
            }
            
            lastCheck = 0;
            physThreadStarted = false;
        }

        public PhysicsArgs foundInfo(ushort x, ushort y, ushort z) {
            if (!listCheckExists.Get(x, y, z))
                return default(PhysicsArgs);
            
            int index = PosToInt(x, y, z);
            for (int i = 0; i < ListCheck.Count; i++) {
                Check C = ListCheck.Items[i];
                if (C.Index != index) continue;
                return C.data;
            }
            return default(PhysicsArgs);
        }

        void PhysicsTick() {
            lastCheck = ListCheck.Count;
            const uint mask = PhysicsArgs.TypeMask;
            
            HandlePhysics[] handlers = PhysicsHandlers;
            ExtraInfoHandler extraHandler = ExtraInfoPhysics.normalHandler;
            if (physics == 5) {
                handlers = physicsDoorsHandlers;
                extraHandler = ExtraInfoPhysics.doorsHandler;
            }
            
            PhysInfo C;
            for (int i = 0; i < ListCheck.Count; i++) {
                Check chk = ListCheck.Items[i];
                IntToPos(chk.Index, out C.X, out C.Y, out C.Z);
                C.Index = chk.Index; C.Data = chk.data;
                
                try {
                    if (OnPhysicsUpdateEvent.handlers.Count > 0)
                        OnPhysicsUpdateEvent.Call(C.X, C.Y, C.Z, C.Data, this);
                    
                    C.Block = blocks[chk.Index];
                    #if TEN_BIT_BLOCKS
                    BlockID extended = Block.ExtendedBase[C.Block];
                    if (extended > 0) {
                        C.Block = (BlockID)(extended | FastGetExtTile(C.X, C.Y, C.Z));
                    }
                    #else
                    if (C.Block == Block.custom_block) {
                        C.Block = (BlockID)(Block.Extended | FastGetExtTile(C.X, C.Y, C.Z));
                    }
                    #endif
                    
                    if ((C.Data.Raw & mask) == 0 || C.Data.Type1 == PhysicsArgs.Custom || extraHandler(this, ref C)) {
                        HandlePhysics handler = handlers[C.Block];
                        if (handler != null) {
                            handler(this, ref C);
                        } else if ((C.Data.Raw & mask) == 0 || !C.Data.HasWait) {
                            C.Data.Data = PhysicsArgs.RemoveFromChecks;
                        }
                    }

                    ListCheck.Items[i].data = C.Data;
                } catch {
                    listCheckExists.Set(C.X, C.Y, C.Z, false);
                    ListCheck.RemoveAt(i);
                }
            }
            RemoveExpiredChecks();
            
            lastUpdate = ListUpdate.Count;
            if (ListUpdate.Count > 0 && bulkSender == null)
                bulkSender = new BufferedBlockSender(this);
                        
            for (int i = 0; i < ListUpdate.Count; i++) {
                Update U = ListUpdate.Items[i];
                try {
                    BlockID block = U.data.Data;
                    U.data.Data = 0;
                    
                    // Is the Ext flag just an indicator for the block update?
                    byte extBits = U.data.ExtBlock;
                    if (extBits != 0 && (U.data.Raw & PhysicsArgs.TypeMask) == 0) {
                        block |= (BlockID)(extBits << Block.ExtendedShift);
                        U.data.Raw &= ~PhysicsArgs.ExtBits;
                    }
                    
                    if (DoPhysicsBlockchange(U.Index, block, false, U.data, true))
                        bulkSender.Add(U.Index, block);
                } catch {
                    Logger.Log(LogType.Warning, "Phys update issue");
                }
            }
            
            if (bulkSender != null) bulkSender.Flush();
            ListUpdate.Clear(); listUpdateExists.Clear();
        }
        
        public void AddCheck(int index, bool overRide = false) {
            AddCheck(index, overRide, default(PhysicsArgs));
        }
        
        public void AddCheck(int index, bool overRide, PhysicsArgs data) {
            try {
                int x = index % Width;
                int y = (index / Width) / Length;
                int z = (index / Width) % Length;
                if (x >= Width || y >= Height || z >= Length) return;
                
                if (listCheckExists.TrySetOn(x, y, z)) {
                    Check check; check.Index = index; check.data = data;
                    ListCheck.Add(check); // Adds block to list to be updated
                } else if (overRide) {
                    Check[] items = ListCheck.Items;
                    int count = ListCheck.Count;
                    for (int i = 0; i < count; i++) {
                        if (items[i].Index != index) continue;
                        items[i].data = data; return;
                    }
                    //Dont need to check physics here because if the list is active, then physics is active :)
                }
                
                if (!physThreadStarted && physics > 0)
                    StartPhysics();
            } catch {
                //s.Log("Warning-PhysicsCheck");
                //ListCheck.Add(new Check(b));    //Lousy back up plan
            }
        }

        internal bool AddUpdate(int index, BlockID block, bool overRide = false) {
            PhysicsArgs args = default(PhysicsArgs);
            args.Raw |= (uint)(PhysicsArgs.ExtBit * (block >> Block.ExtendedShift));
            return AddUpdate(index, block, args, overRide);
        }
        
        internal bool AddUpdate(int index, BlockID block, PhysicsArgs data, bool overRide = false) {
            try {
                int x = index % Width;
                int y = (index / Width) / Length;
                int z = (index / Width) % Length;
                if (x >= Width || y >= Height || z >= Length) return false;
                
                if (overRide) {
                    // Is the Ext flag just an indicator for the block update?
                    if (data.ExtBlock != 0 && (data.Raw & PhysicsArgs.TypeMask) == 0) {
                        data.Raw &= ~PhysicsArgs.ExtBits;
                    }
                    AddCheck(index, true, data); //Dont need to check physics here....AddCheck will do that
                    Blockchange((ushort)x, (ushort)y, (ushort)z, block, true, data);
                    return true;
                }
                
                if (listUpdateExists.TrySetOn(x, y, z)) {
                } else if (block == Block.Sand || block == Block.Gravel)  {
                    RemoveUpdatesAtPos(index);
                } else {
                    return false;
                }
                
                data.Data = (byte)block;
                Update update; update.Index = index; update.data = data;
                ListUpdate.Add(update);
                
                if (!physThreadStarted && physics > 0)
                    StartPhysics();
                return true;
            } catch {
                //s.Log("Warning-PhysicsUpdate");
                //ListUpdate.Add(new Update(b, (byte)type));    //Lousy back up plan
                return false;
            }
        }
        
        void RemoveExpiredChecks() {
            Check[] items = ListCheck.Items;
            int j = 0, count = ListCheck.Count;
            ushort x, y, z;
            
            for (int i = 0; i < count; i++) {
                if (items[i].data.Data == PhysicsArgs.RemoveFromChecks) {
                    IntToPos(items[i].Index, out x, out y, out z);
                    listCheckExists.Set(x, y, z, false);
                    continue;
                }
                items[j] = items[i]; j++;
            }
            ListCheck.Items = items;
            ListCheck.Count = j;
        }
        
        void RemoveUpdatesAtPos(int b) {
            Update[] items = ListUpdate.Items;
            int j = 0, count = ListUpdate.Count;
            
            for (int i = 0; i < count; i++) {
                if (items[j].Index == b) continue;
                items[j] = items[i]; j++;
            }
            ListUpdate.Items = items;
            ListUpdate.Count = j;
        }
        
        
        void ClearPhysicsLists() {
            ListCheck.Count  = 0; listCheckExists.Clear();
            ListUpdate.Count = 0; listUpdateExists.Clear();
        }
        
        public void ClearPhysics() {
            for (int i = 0; i < ListCheck.Count; i++ )
                RevertPhysics(ListCheck.Items[i]);
            ClearPhysicsLists();
        }
        
        void RevertPhysics(Check C) {
            //attemps on shutdown to change blocks back into normal selves that are active, hopefully without needing to send into to clients.
            switch (blocks[C.Index]) {
                case Block.Air_Flood:
                case Block.Air_FloodLayer:
                case Block.Air_FloodDown:
                case Block.Air_FloodUp:
                    blocks[C.Index] = Block.Air; break;
            }

            try {
                PhysicsArgs args = C.data;
                // Copy paste here because it's worthwhile inlining
                if (args.Type1 == PhysicsArgs.Revert) {
                    BlockID block = (BlockID)(args.Value1 | (args.ExtBlock << Block.ExtendedShift));
                    Blockchange(C.Index, block, true, default(PhysicsArgs));
                } else if (args.Type2 == PhysicsArgs.Revert) {
                    BlockID block = (BlockID)(args.Value2 | (args.ExtBlock << Block.ExtendedShift));
                    Blockchange(C.Index, block, true, default(PhysicsArgs));
                }
            } catch (Exception e) {
                Logger.LogError(e);
            }
        }
        
        
        internal bool ActivatesPhysics(BlockID block) {
            if (Props[block].IsMessageBlock || Props[block].IsPortal) return false;
            if (Props[block].IsDoor || Props[block].IsTDoor) return false;
            if (Props[block].OPBlock) return false;
            return PhysicsHandlers[block] != null;
        }
        
        internal bool CheckSpongeWater(ushort x, ushort y, ushort z) {
            for (int yy = y - 2; yy <= y + 2; ++yy) {
                if (yy < 0 || yy >= Height) continue;
                for (int zz = z - 2; zz <= z + 2; ++zz) {
                    if (zz < 0 || zz >= Length) continue;
                    for (int xx = x - 2; xx <= x + 2; ++xx) {
                        if (xx < 0 || xx >= Width) continue;
                        if (blocks[xx + Width * (zz + yy * Length)] == Block.Sponge)
                            return true;
                    }
                }
            }
            return false;
        }
        
        internal bool CheckSpongeLava(ushort x, ushort y, ushort z) {
            for (int yy = y - 2; yy <= y + 2; ++yy) {
                if (yy < 0 || yy >= Height) continue;
                for (int zz = z - 2; zz <= z + 2; ++zz) {
                    if (zz < 0 || zz >= Length) continue;
                    for (int xx = x - 2; xx <= x + 2; ++xx) {
                        if (xx < 0 || xx >= Width) continue;
                        if (blocks[xx + Width * (zz + yy * Length)] == Block.LavaSponge)
                            return true;
                    }
                }
            }
            return false;
        }

        public void MakeExplosion(ushort x, ushort y, ushort z, int size, bool force = false) {
            TntPhysics.MakeExplosion(this, x, y, z, size, force, null);
        }
    }
    
    public struct PhysInfo {
        public ushort X, Y, Z;
        public BlockID Block;
        public int Index;
        public PhysicsArgs Data;
    }
    
    public struct Check {
        public int Index;
        public PhysicsArgs data;
    }

    public struct Update {
        public int Index;
        public PhysicsArgs data;
    }
}