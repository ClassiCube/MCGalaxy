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
        
        public void setPhysics(int newValue) {
            if (physics == 0 && newValue != 0 && blocks != null) {
                for (int i = 0; i < blocks.Length; i++)
                    // Optimization hack, since no blocks under 183 ever need a restart
                    if (blocks[i] > 183 && Block.NeedRestart(blocks[i]))
                        AddCheck(i);
            }
            physics = newValue;
            //StartPhysics(); This isnt needed, the physics will start when we set the new value above
        }
        
        public void StartPhysics() {
            lock (physThreadLock) {
                if (physThread != null && physThread.ThreadState == ThreadState.Running) return;
                if (ListCheck.Count == 0 || physThreadStarted) return;
                
                physThread = new Thread(PhysicsLoop);
                physThread.Name = "MCG_Physics_" + name;
                PhysicsEnabled = true;
                physThread.Start();
                physThreadStarted = true;
            }
        }

        /// <summary> Gets or sets a value indicating whether physics are enabled. </summary>
        public bool PhysicsEnabled;

        void PhysicsLoop() {
            int wait = Config.PhysicsSpeed;
            while (true) {
                if (!PhysicsEnabled) { Thread.Sleep(500); continue; }

                try {
                    if (wait > 0) Thread.Sleep(wait);
                    if (physics == 0 || ListCheck.Count == 0)
                    {
                        lastCheck = 0;
                        wait = Config.PhysicsSpeed;
                        if (physics == 0) break;
                        continue;
                    }

                    DateTime start = DateTime.UtcNow;
                    if (physics > 0) {
                        try {
                            lock (physStepLock)
                                CalcPhysics();
                        } catch (Exception ex) {
                            Logger.Log(LogType.Warning, "Level physics error");
                            Logger.LogError(ex);
                        }
                    }

                    TimeSpan delta = DateTime.UtcNow - start;
                    wait = Config.PhysicsSpeed - (int)delta.TotalMilliseconds;

                    if (wait < (int)(-Config.PhysicsOverload * 0.75f)) {
                        if (wait < -Config.PhysicsOverload) {
                            if (!ServerConfig.PhysicsRestart)
                                setPhysics(0);
                            ClearPhysics();

                            Chat.MessageGlobal("Physics shutdown on {0}", ColoredName);
                            Logger.Log(LogType.Warning, "Physics shutdown on " + name);
                            OnPhysicsStateChangedEvent.Call(this, PhysicsState.Stopped);
                            wait = Config.PhysicsSpeed;
                        } else {
                            Player[] online = PlayerInfo.Online.Items;
                            foreach (Player p in online) {
                                if (p.level != this) continue;
                                Player.Message(p, "Physics warning!");
                            }
                            Logger.Log(LogType.Warning, "Physics warning on " + name);
                            OnPhysicsStateChangedEvent.Call(this, PhysicsState.Warning);
                        }
                    }
                } catch {
                    wait = Config.PhysicsSpeed;
                }
            }
            physThreadStarted = false;
        }

        public PhysicsArgs foundInfo(ushort x, ushort y, ushort z) {
            if (!listCheckExists.Get(x, y, z))
                return default(PhysicsArgs);
            
            int index = PosToInt(x, y, z);
            for (int i = 0; i < ListCheck.Count; i++) {
                Check C = ListCheck.Items[i];
                if (C.b != index) continue;
                return C.data;
            }
            return default(PhysicsArgs);
        }

        public void CalcPhysics() {
            ushort x, y, z;
            lastCheck = ListCheck.Count;
            const uint mask = PhysicsArgs.TypeMask;
            
            HandlePhysics[] handlers = physicsHandlers;
            ExtraInfoHandler extraHandler = ExtraInfoPhysics.DoNormal;
            if (physics == 5) {
                handlers = physicsDoorsHandlers;
                extraHandler = ExtraInfoPhysics.DoDoorsOnly;
            }
            
            for (int i = 0; i < ListCheck.Count; i++) {
                Check C = ListCheck.Items[i];
                IntToPos(C.b, out x, out y, out z);
                try {
                    if (OnPhysicsUpdateEvent.handlers.Count > 0)
                        OnPhysicsUpdateEvent.Call(x, y, z, C.data, this);
                    
                    if ((C.data.Raw & mask) == 0 || extraHandler(this, ref C)) {
                        int idx = blocks[C.b];
                        if (idx == Block.custom_block) idx = Block.Count + GetExtTileNoCheck(x, y, z);
                        
                        HandlePhysics handler = handlers[idx];
                        if (handler != null) {
                            handler(this, ref C);
                        } else if ((C.data.Raw & mask) == 0 || !C.data.HasWait) {
                            C.data.Data = PhysicsArgs.RemoveFromChecks;
                        }
                    }
                    ListCheck.Items[i] = C;
                } catch {
                    listCheckExists.Set(x, y, z, false);
                    ListCheck.RemoveAt(i);
                }
            }
            RemoveExpiredChecks();
            
            lastUpdate = ListUpdate.Count;
            if (ListUpdate.Count > 0 && bulkSender == null)
                bulkSender = new BufferedBlockSender(this);
            
            BlockID block;
            for (int i = 0; i < ListUpdate.Count; i++) {
                Update C = ListUpdate.Items[i];
                try {
                    block = C.data.Data;
                    C.data.Data = 0;
                    
                    // Is the Ext flag just an indicator for the block update?
                    if (C.data.ExtBlock && (C.data.Raw & PhysicsArgs.TypeMask) == 0) {
                        block |= Block.Extended;
                        C.data.ExtBlock = false;
                    }
                    
                    if (DoPhysicsBlockchange(C.b, block, false, C.data, true))
                        bulkSender.Add(C.b, block);
                } catch {
                    Logger.Log(LogType.Warning, "Phys update issue");
                }
            }
            
            if (bulkSender != null)
                bulkSender.Send(true);
            ListUpdate.Clear(); listUpdateExists.Clear();
        }
        
        public void AddCheck(int b, bool overRide = false) {
            AddCheck(b, overRide, default(PhysicsArgs));
        }
        
        public void AddCheck(int b, bool overRide, PhysicsArgs data) {
            try {
                int x = b % Width;
                int y = (b / Width) / Length;
                int z = (b / Width) % Length;
                if (x >= Width || y >= Height || z >= Length) return;
                
                if (!listCheckExists.Get(x, y, z)) {
                    ListCheck.Add(new Check(b, data)); //Adds block to list to be updated
                    listCheckExists.Set(x, y, z, true);
                } else if (overRide) {
                    Check[] items = ListCheck.Items;
                    int count = ListCheck.Count;
                    for (int i = 0; i < count; i++) {
                        if (items[i].b != b) continue;
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

        internal bool AddUpdate(int b, BlockID block, bool overRide = false) {
            PhysicsArgs args = default(PhysicsArgs);
            args.ExtBlock = block >= Block.Extended;
            return AddUpdate(b, block, args, overRide);
        }
        
        internal bool AddUpdate(int b, BlockID block, PhysicsArgs data, bool overRide = false) {
            try {
                int x = b % Width;
                int y = (b / Width) / Length;
                int z = (b / Width) % Length;
                if (x >= Width || y >= Height || z >= Length) return false;
                
                if (overRide) {
                    // Is the Ext flag just an indicator for the block update?
                    if (data.ExtBlock && (data.Raw & PhysicsArgs.TypeMask) == 0) {
                        data.ExtBlock = false;
                    }
                    AddCheck(b, true, data); //Dont need to check physics here....AddCheck will do that
                    Blockchange((ushort)x, (ushort)y, (ushort)z, block, true, data);
                    return true;
                }

                if (!listUpdateExists.Get(x, y, z)) {
                    listUpdateExists.Set(x, y, z, true);
                } else if (block == Block.Sand || block == Block.Gravel)  {
                    RemoveUpdatesAtPos(b);
                } else {
                    return false;
                }
                ListUpdate.Add(new Update(b, (byte)block, data));
                
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
                    IntToPos(items[i].b, out x, out y, out z);
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
                if (items[j].b == b) continue;
                items[j] = items[i]; j++;
            }
            ListUpdate.Items = items;
            ListUpdate.Count = j;
        }
        
        
        public void ClearPhysics() {
            for (int i = 0; i < ListCheck.Count; i++ )
                RevertPhysics(ListCheck.Items[i]);
            ListCheck.Clear(); listCheckExists.Clear();
            ListUpdate.Clear(); listUpdateExists.Clear();
        }
        
        void RevertPhysics(Check C) {
            //attemps on shutdown to change blocks back into normal selves that are active, hopefully without needing to send into to clients.
            switch (blocks[C.b]) {
                case Block.Air_Flood:
                case Block.Air_FloodLayer:
                case Block.Air_FloodDown:
                case Block.Air_FloodUp:
                    blocks[C.b] = 0; break;
            }

            try {
                PhysicsArgs args = C.data;
                // Copy paste here because it's worthwhile inlining
                if (args.Type1 == PhysicsArgs.Revert) {
                    ushort x, y, z;
                    IntToPos(C.b, out x, out y, out z);
                    
                    BlockID block = Block.FromRaw(args.Value1, args.ExtBlock);
                    Blockchange(C.b, block, true, default(PhysicsArgs));
                } else if (args.Type2 == PhysicsArgs.Revert) {
                    ushort x, y, z;
                    IntToPos(C.b, out x, out y, out z);
                    
                    BlockID block = Block.FromRaw(args.Value2, args.ExtBlock);
                    Blockchange(C.b, block, true, default(PhysicsArgs));
                }
            } catch (Exception e) {
                Logger.LogError(e);
            }
        }
        
        
        internal bool ActivatesPhysics(BlockID block) {
            if (Props[block].IsMessageBlock || Props[block].IsPortal) return false;
            if (Props[block].IsDoor || Props[block].IsTDoor) return false;
            if (Props[block].OPBlock) return false;
            return physicsHandlers[block] != null;
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

        public void MakeExplosion(ushort x, ushort y, ushort z, int size, bool force = false, TntWarsGame CheckForExplosionZone = null) {
            TntPhysics.MakeExplosion(this, x, y, z, size, force, CheckForExplosionZone);
        }
    }
    
    public struct Check {
        public int b;
        public PhysicsArgs data;

        public Check(int b, PhysicsArgs data = default(PhysicsArgs)) {
            this.b = b;
            this.data = data;
        }
    }

    public struct Update {
        public int b;
        public PhysicsArgs data;

        public Update(int b, byte type, PhysicsArgs data = default(PhysicsArgs)) {
            this.b = b;
            this.data = data;
            this.data.Data = type;
        }
    }
}