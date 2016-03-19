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
using System.Linq;
using System.Threading;
using MCGalaxy.BlockPhysics;
using MCGalaxy.Games;

namespace MCGalaxy {
    
    public enum PhysicsState { Stopped, Warning, Other }

    public sealed partial class Level : IDisposable {
        
        public void setPhysics(int newValue) {
            if (physics == 0 && newValue != 0 && blocks != null) {
                for (int i = 0; i < blocks.Length; i++)
                    // Optimization hack, since no blocks under 183 ever need a restart
                    if (blocks[i] > 183)
                        if (Block.NeedRestart(blocks[i]))
                            AddCheck(i);
            }
            physics = newValue;
            //StartPhysics(); This isnt needed, the physics will start when we set the new value above
        }
        
        public void StartPhysics() {
            lock (physThreadLock) {
                if (physThread != null) {
                    if (physThread.ThreadState == System.Threading.ThreadState.Running) return;
                }
                if (ListCheck.Count == 0 || physicssate) return;
                
                physThread = new Thread(PhysicsLoop);
                physThread.Name = "MCG_Physics";
                PhysicsEnabled = true;
                physThread.Start();
                physicssate = true;
            }
        }

        /// <summary> Gets or sets a value indicating whether physics are enabled. </summary>
        public bool PhysicsEnabled;

        void PhysicsLoop() {
            int wait = speedPhysics;
            while (true) {
                if (!PhysicsEnabled) { Thread.Sleep(500); continue; }

                try {
                    if (wait > 0) Thread.Sleep(wait);
                    if (physics == 0 || ListCheck.Count == 0)
                    {
                        lastCheck = 0;
                        wait = speedPhysics;
                        if (physics == 0) break;
                        continue;
                    }

                    DateTime start = DateTime.UtcNow;
                    if (physics > 0) CalcPhysics();

                    TimeSpan delta = DateTime.UtcNow - start;
                    wait = speedPhysics - (int)delta.TotalMilliseconds;

                    if (wait < (int)(-overload * 0.75f))
                    {
                        if (wait < -overload)
                        {
                            if (!Server.physicsRestart)
                                setPhysics(0);
                            ClearPhysics();

                            Player.GlobalMessage("Physics shutdown on &b" + name);
                            Server.s.Log("Physics shutdown on " + name);
                            if (PhysicsStateChanged != null)
                                PhysicsStateChanged(this, PhysicsState.Stopped);

                            wait = speedPhysics;
                        } else {
                        	Player[] online = PlayerInfo.Online.Items;
                            foreach (Player p in online) {
                        	    if (p.level != this) continue;
                                Player.SendMessage(p, "Physics warning!");
                            }
                            Server.s.Log("Physics warning on " + name);

                            if (PhysicsStateChanged != null)
                                PhysicsStateChanged(this, PhysicsState.Warning);
                        }
                    }
                } catch {
                    wait = speedPhysics;
                }
            }
            physicssate = false;
            physThread.Abort();
        }

        public string foundInfo(ushort x, ushort y, ushort z) {
            int index = PosToInt(x, y, z);
            for (int i = 0; i < ListCheck.Count; i++) {
                Check C = ListCheck.Items[i];
                if (C.b != index) continue;
                return (C.data is string) ? (string)C.data : "";
            }
            return "";
        }

        public unsafe void CalcPhysics() {
            if (physics == 0) return;
            try {
                ushort x, y, z;
                lastCheck = ListCheck.Count;
                if (physics == 5) {
                    for (int i = 0; i < ListCheck.Count; i++) {
                        Check C = ListCheck.Items[i];
                        IntToPos(C.b, out x, out y, out z);
                        try {
                            string info = C.data as string;
                            if (info == null) info = "";
                            
                            if (PhysicsUpdate != null)
                                PhysicsUpdate(x, y, z, C.time, info, this);
                            if (info == "" || ExtraInfoPhysics.DoDoorsOnly(this, C, null))
                                DoorPhysics.Do(this, C);
                        } catch {
                            listCheckExists.Set(x, y, z, false);
                            ListCheck.Remove(C);
                        }
                    }
                } else {
                    Random rand = new Random();
                    for (int i = 0; i < ListCheck.Count; i++) {
                        Check C = ListCheck.Items[i];
                        IntToPos(C.b, out x, out y, out z);
                        try {
                            string info = C.data as string;
                            if (info == null) info = "";
                            
                            if (PhysicsUpdate != null)
                                PhysicsUpdate(x, y, z, C.time, info, this);
                            if (OnPhysicsUpdateEvent.events.Count > 0)
                                OnPhysicsUpdateEvent.Call(x, y, z, C.time, info, this);
                            if (info == "" || ExtraInfoPhysics.DoComplex(this, C, rand))
                                DoNormalPhysics(x, y, z, rand, C);
                        } catch {
                            listCheckExists.Set(x, y, z, false);
                            ListCheck.Remove(C);
                        }
                    }
                }
                RemoveExpiredChecks();
                
                lastUpdate = ListUpdate.Count;
                if (ListUpdate.Count > 0 && bulkSender == null)
                	bulkSender = new BufferedBlockSender(this);
                
                for (int i = 0; i < ListUpdate.Count; i++) {
                    Update C = ListUpdate.Items[i];
                    try {
                        string info = C.data as string;
                        if (info == null) info = "";
                        if (DoPhysicsBlockchange(C.b, C.type, false, info, 0, true))
                        	bulkSender.Add(C.b, C.type, 0);
                    } catch {
                        Server.s.Log("Phys update issue");
                    }
                    bulkSender.CheckIfSend(false);
                }
                if (bulkSender != null)
                    bulkSender.CheckIfSend(true);
                ListUpdate.Clear(); listUpdateExists.Clear();
            } catch (Exception e) {
                Server.s.Log("Level physics error");
                Server.ErrorLog(e);
            }
        }
        
        void DoNormalPhysics(ushort x, ushort y, ushort z, Random rand, Check C) {
            switch (blocks[C.b])
            {
                case Block.air: //Placed air
                    AirPhysics.DoAir(this, C, rand);
                    break;
                case Block.dirt: //Dirt
                    if (!GrassGrow) { C.time = 255; break; }

                    if (C.time > 20) {
                        byte type = GetTile(x, (ushort)(y + 1), z), extType = 0;
                        if (type == Block.custom_block)
                            extType = GetExtTile(x, (ushort)(y + 1), z);
                        
                        if (Block.LightPass(type, extType, CustomBlockDefs))
                            AddUpdate(C.b, Block.grass);
                        C.time = 255;
                    } else {
                        C.time++;
                    }
                    break;

                case Block.leaf:
                    if (physics > 1)
                        //Adv physics kills flowers and mushroos in water/lava
                    {
                        PhysAir(PosToInt((ushort)(x + 1), y, z));
                        PhysAir(PosToInt((ushort)(x - 1), y, z));
                        PhysAir(PosToInt(x, y, (ushort)(z + 1)));
                        PhysAir(PosToInt(x, y, (ushort)(z - 1)));
                        PhysAir(PosToInt(x, (ushort)(y + 1), z));
                        //Check block above
                    }

                    if (!leafDecay) {
                        C.time = 255;
                        leaves.Clear();
                        break;
                    }
                    if (C.time < 5)
                    {
                        if (rand.Next(10) == 0) C.time++;
                        break;
                    }
                    if (StandardPhysics.DoLeafDecay(this, C))
                        AddUpdate(C.b, Block.air);
                    C.time = 255;
                    break;

                case Block.shrub:
                    if (physics > 1)
                        //Adv physics kills flowers and mushroos in water/lava
                    {
                        PhysAir(PosToInt((ushort)(x + 1), y, z));
                        PhysAir(PosToInt((ushort)(x - 1), y, z));
                        PhysAir(PosToInt(x, y, (ushort)(z + 1)));
                        PhysAir(PosToInt(x, y, (ushort)(z - 1)));
                        PhysAir(PosToInt(x, (ushort)(y + 1), z));
                        //Check block above
                    }

                    if (!growTrees)
                    {
                        C.time = 255;
                        break;
                    }
                    if (C.time < 20)
                    {
                        if (rand.Next(20) == 0) C.time++;
                        break;
                    }
                    TreeGen.AddTree(this, x, y, z, rand, true, false);
                    C.time = 255;
                    break;

                case Block.water:
                case Block.activedeathwater:
                    LiquidPhysics.DoWater(this, C, rand); break;
                case Block.WaterDown:
                    ExtLiquidPhysics.DoWaterfall(this, C, rand); break;
                case Block.LavaDown:
                    ExtLiquidPhysics.DoLavafall(this, C, rand); break;
                case Block.WaterFaucet:
                    ExtLiquidPhysics.DoFaucet(this, C, rand, Block.WaterDown); break;
                case Block.LavaFaucet:
                    ExtLiquidPhysics.DoFaucet(this, C, rand, Block.LavaDown); break;
                case Block.lava:
                case Block.activedeathlava:
                    LiquidPhysics.DoLava(this, C, rand); break;
                case Block.fire:
                    FirePhysics.Do(this, C, rand); break;
                case Block.finiteWater:
                case Block.finiteLava:
                    FinitePhysics.DoWaterOrLava(this, C, rand); break;
                case Block.finiteFaucet:
                    FinitePhysics.DoFaucet(this, C, rand); break;
                case Block.sand:
                case Block.gravel:
                    if (PhysSand(C.b, blocks[C.b]))
                    {
                        PhysAir(PosToInt((ushort)(x + 1), y, z));
                        PhysAir(PosToInt((ushort)(x - 1), y, z));
                        PhysAir(PosToInt(x, y, (ushort)(z + 1)));
                        PhysAir(PosToInt(x, y, (ushort)(z - 1)));
                        PhysAir(PosToInt(x, (ushort)(y + 1), z));
                    }
                    C.time = 255;
                    break;
                case Block.sponge: //SPONGE
                    PhysSponge(C.b);
                    C.time = 255;
                    break;
                case Block.lava_sponge: //SPONGE
                    PhysSponge(C.b, true);
                    C.time = 255;
                    break;
                    //Adv physics updating anything placed next to water or lava
                case Block.wood: //Wood to die in lava
                case Block.trunk: //Wood to die in lava
                case Block.yellowflower:
                case Block.redflower:
                case Block.mushroom:
                case Block.redmushroom:
                case Block.bookcase: //bookcase
                case Block.red: //Shitload of cloth
                case Block.orange:
                case Block.yellow:
                case Block.lightgreen:
                case Block.green:
                case Block.aquagreen:
                case Block.cyan:
                case Block.lightblue:
                case Block.blue:
                case Block.purple:
                case Block.lightpurple:
                case Block.pink:
                case Block.darkpink:
                case Block.darkgrey:
                case Block.lightgrey:
                case Block.white:
                    if (physics > 1)
                        //Adv physics kills flowers and mushroos in water/lava
                    {
                        PhysAir(PosToInt((ushort)(x + 1), y, z));
                        PhysAir(PosToInt((ushort)(x - 1), y, z));
                        PhysAir(PosToInt(x, y, (ushort)(z + 1)));
                        PhysAir(PosToInt(x, y, (ushort)(z - 1)));
                        PhysAir(PosToInt(x, (ushort)(y + 1), z));
                        //Check block above
                    }
                    C.time = 255;
                    break;

                case Block.staircasestep:
                case Block.cobblestoneslab:
                    PhysStair(C.b);
                    C.time = 255;
                    break;
                case Block.wood_float: //wood_float
                    PhysFloatwood(C.b);
                    C.time = 255;
                    break;
                case Block.lava_fast: //lava_fast
                case Block.fastdeathlava:
                    LiquidPhysics.DoFastLava(this, C, rand);
                    break;

                    //Special blocks that are not saved
                case Block.air_flood:
                    AirPhysics.DoFlood(this, C, rand, AirFlood.Full, Block.air_flood); break;
                case Block.air_flood_layer:
                    AirPhysics.DoFlood(this, C, rand, AirFlood.Layer, Block.air_flood_layer); break;
                case Block.air_flood_down:
                    AirPhysics.DoFlood(this, C, rand, AirFlood.Down, Block.air_flood_down); break;
                case Block.air_flood_up:
                    AirPhysics.DoFlood(this, C, rand, AirFlood.Up, Block.air_flood_up); break;
                case Block.smalltnt:
                    TntPhysics.DoSmallTnt(this, C, rand); break;
                case Block.bigtnt:
                    TntPhysics.DoLargeTnt(this, C, rand, 1);
                    break;
                case Block.nuketnt:
                    TntPhysics.DoLargeTnt(this, C, rand, 4);
                    break;
                case Block.tntexplosion:
                    if (rand.Next(1, 11) <= 7)
                        AddUpdate(C.b, Block.air);
                    break;
                case Block.train:
                    TrainPhysics.Do(this, C, rand); break;
                case Block.magma:
                    ExtLiquidPhysics.DoMagma(this, C, rand); break;
                case Block.geyser:
                    ExtLiquidPhysics.DoGeyser(this, C, rand); break;
                case Block.birdblack:
                case Block.birdwhite:
                case Block.birdlava:
                case Block.birdwater:
                    BirdPhysics.Do(this, C, rand); break;
                case Block.snaketail:
                    if (GetTile(IntOffset(C.b, -1, 0, 0)) != Block.snake ||
                        GetTile(IntOffset(C.b, 1, 0, 0)) != Block.snake ||
                        GetTile(IntOffset(C.b, 0, 0, 1)) != Block.snake ||
                        GetTile(IntOffset(C.b, 0, 0, -1)) != Block.snake)
                        C.data = "revert 0";
                    break;
                case Block.snake:
                    SnakePhysics.Do(this, C, rand); break;
                case Block.birdred:
                case Block.birdblue:
                case Block.birdkill:
                    HunterPhysics.DoKiller(this, C, rand, Block.air); break;
                case Block.fishbetta:
                case Block.fishshark:
                    HunterPhysics.DoKiller(this, C, rand, Block.water); break;
                case Block.fishgold:
                case Block.fishsalmon:
                case Block.fishsponge:
                    HunterPhysics.DoFlee(this, C, rand, Block.water); break;
                case Block.fishlavashark:
                    HunterPhysics.DoKiller(this, C, rand, Block.lava); break;
                case Block.rockethead:
                    RocketPhysics.Do(this, C, rand); break;
                case Block.firework:
                    FireworkPhysics.Do(this, C, rand); break;
                case Block.zombiehead:
                    if (GetTile(IntOffset(C.b, 0, -1, 0)) != Block.zombiebody &&
                        GetTile(IntOffset(C.b, 0, -1, 0)) != Block.creeper)
                        C.data = "revert 0";
                    break;
                case Block.zombiebody:
                case Block.creeper:
                    ZombiePhysics.Do(this, C, rand); break;

                case Block.c4:
                    C4.C4s c4 = C4.Find(this, ((Player)C.data).c4circuitNumber);
                    if (c4 != null) {
                        FillPos pos; pos.X = x; pos.Y = y; pos.Z = z;
                        c4.list.Add(pos);
                    }
                    C.time = 255;
                    break;

                case Block.c4det:
                    C4.C4s c = C4.Find(this, ((Player)C.data).c4circuitNumber);
                    if (c != null) {
                        c.detenator[0] = x;
                        c.detenator[1] = y;
                        c.detenator[2] = z;
                    }
                    ((Player)C.data).c4circuitNumber = -1;
                    C.time = 255;
                    break;

                default:
                    DoorPhysics.Do(this, C); break;
            }
        }
        
        public void AddCheck(int b, bool overRide = false) { AddCheck(b, overRide, ""); }
        
        public void AddCheck(int b, bool overRide, object data) {
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
                if (!physicssate && physics > 0)
                    StartPhysics();
            } catch {
                //s.Log("Warning-PhysicsCheck");
                //ListCheck.Add(new Check(b));    //Lousy back up plan
            }
        }

        internal bool AddUpdate(int b, int type, bool overRide = false) { return AddUpdate(b, type, overRide, ""); }
        
        internal bool AddUpdate(int b, int type, bool overRide, object data) {
            try {
                int x = b % Width;
                int y = (b / Width) / Length;
                int z = (b / Width) % Length;
                if (x >= Width || y >= Height || z >= Length) return false;
                
                if (overRide) {
                    AddCheck(b, true, data); //Dont need to check physics here....AddCheck will do that
                    string info = data as string;
                    if (info == null) info = "";
                    Blockchange((ushort)x, (ushort)y, (ushort)z, (byte)type, true, info);
                    return true;
                }

                if (!listUpdateExists.Get(x, y, z)) {
                    listUpdateExists.Set(x, y, z, true);
                } else if (type == Block.sand || type == Block.gravel)  {
                    RemoveUpdatesAtPos(b);
                } else {
                    return false;
                }
                
                ListUpdate.Add(new Update(b, (byte)type, data));
                if (!physicssate && physics > 0)
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
                if (items[i].time == 255) {
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
            ushort x, y, z;
            IntToPos(C.b, out x, out y, out z);
            //attemps on shutdown to change blocks back into normal selves that are active, hopefully without needing to send into to clients.
            switch (blocks[C.b]) {
                case Block.air_flood:
                case Block.air_flood_layer:
                case Block.air_flood_down:
                case Block.air_flood_up:
                    blocks[C.b] = 0; break;
                case Block.door_tree__air:
                    //blocks[C.b] = 111;
                    Blockchange(x, y, z, Block.door_tree); break;
                case Block.door_obsidian_air:
                    //blocks[C.b] = 113;
                    Blockchange(x, y, z, Block.door_obsidian); break;
                case Block.door_glass_air:
                    //blocks[C.b] = 114;
                    Blockchange(x, y, z, Block.door_glass); break;
                case Block.door_stone_air:
                    //blocks[C.b] = 115;
                    Blockchange(x, y, z, Block.door_stone); break;
            }

            try {
                string info = C.data as string;
                if (info != null && info.Contains("revert")) {
                    string[] parts = info.Split(' ');
                    for (int i = 0; i < parts.Length; i++) {
                        if (parts[i] == "revert") {
                            Blockchange(x, y, z, Byte.Parse(parts[i + 1]), true); break;
                        }
                    }
                }
            } catch (Exception e) {
                Server.ErrorLog(e);
            }
        }

        internal void PhysAir(int b) { AirPhysics.PhysAir(this, b); }

        internal void PhysWater(ushort x, ushort y, ushort z, byte type) {
            if (x >= Width || y >= Height || z >= Length) return;
            if (Server.lava.active && Server.lava.map == this && Server.lava.InSafeZone(x, y, z))
                return;
            
            int b = x + (z * Width) + (y * Width * Length);
            switch (blocks[b]) {
                case Block.air:
                    if (!CheckSpongeWater(x, y, z)) AddUpdate(b, type);
                    break;

                case Block.lava:
                case Block.lava_fast:
                case Block.activedeathlava:
                    if (!CheckSpongeWater(x, y, z)) AddUpdate(b, Block.rock);
                    break;

                case Block.shrub:
                case Block.yellowflower:
                case Block.redflower:
                case Block.mushroom:
                case Block.redmushroom:
                    if (physics > 1 && physics != 5 && !CheckSpongeWater(x, y, z))
                        AddUpdate(b, Block.air); //Adv physics kills flowers and mushrooms in water
                    break;

                case Block.sand:
                case Block.gravel:
                case Block.wood_float:
                    AddCheck(b); break;
                default:
                    break;
            }
        }

        internal void PhysLava(ushort x, ushort y, ushort z, byte type) {
            if (x >= Width || y >= Height || z >= Length) return;
            if (Server.lava.active && Server.lava.map == this && Server.lava.InSafeZone(x, y, z))
                return;

            int b = x + Width * (z + y * Length);
            if (physics > 1 && physics != 5 && !CheckSpongeLava(x, y, z) && blocks[b] >= 21 && blocks[b] <= 36) {
                AddUpdate(b, Block.air); return;
            } // Adv physics destroys cloth
            
            switch (blocks[b]) {
                case Block.air:
                    if (!CheckSpongeLava(x, y, z)) AddUpdate(b, type);
                    break;

                case Block.water:
                case Block.activedeathwater:
                    if (!CheckSpongeLava(x, y, z)) AddUpdate(b, Block.rock); break;

                case Block.sand:
                    if (physics > 1) { //Adv physics changes sand to glass next to lava
                        if (physics != 5) AddUpdate(b, Block.glass);
                    } else {
                        AddCheck(b);
                    } break;

                case Block.gravel:
                    AddCheck(b); break;

                case Block.wood:
                case Block.shrub:
                case Block.trunk:
                case Block.leaf:
                case Block.yellowflower:
                case Block.redflower:
                case Block.mushroom:
                case Block.redmushroom:
                    if (physics > 1 && physics != 5) //Adv physics kills flowers and mushrooms plus wood in lava
                        if (!CheckSpongeLava(x, y, z)) AddUpdate(b, Block.air);
                    break;
                default:
                    break;
            }
        }

        bool PhysSand(int b, byte type) { //also does gravel
            if (b == -1 || physics == 0 || physics == 5) return false;

            int tempb = b;
            bool blocked = false, moved = false;

            do
            {
                tempb = IntOffset(tempb, 0, -1, 0); //Get block below each loop
                if (GetTile(tempb) != Block.Zero)
                {
                    switch (blocks[tempb])
                    {
                        case 0: //air lava water
                        case 8:
                        case 10:
                            moved = true;
                            break;

                        case 6:
                        case 37:
                        case 38:
                        case 39:
                        case 40:
                            if (physics > 1 && physics != 5) //Adv physics crushes plants with sand
                            {
                                moved = true;
                            }
                            else
                            {
                                blocked = true;
                            }
                            break;

                        default:
                            blocked = true;
                            break;
                    }
                    if (physics > 1)
                    {
                        if (physics != 5)
                        {
                            blocked = true;
                        }
                    }
                }
                else
                {
                    blocked = true;
                }
            } while (!blocked);

            if (moved)
            {
                AddUpdate(b, 0);
                if (physics > 1)
                {
                    AddUpdate(tempb, type);
                }
                else
                {
                    AddUpdate(IntOffset(tempb, 0, 1, 0), type);
                }
            }

            return moved;
        }

        void PhysStair(int b) {
            int bBelow = IntOffset(b, 0, -1, 0);
            byte tile = GetTile(bBelow);
            
            if (tile == Block.staircasestep) {
                AddUpdate(b, Block.air);
                AddUpdate(bBelow, Block.staircasefull);
            } else if (tile == Block.cobblestoneslab) {
                AddUpdate(b, Block.air);
                AddUpdate(bBelow, Block.stone);
            }
        }
        
        internal bool CheckSpongeWater(ushort x, ushort y, ushort z) {
            for (int yy = y - 2; yy <= y + 2; ++yy) {
                if (yy < 0 || yy >= Height) continue;
                for (int zz = z - 2; zz <= z + 2; ++zz) {
                    if (zz < 0 || zz >= Length) continue;
                    for (int xx = x - 2; xx <= x + 2; ++xx) {
                        if (xx < 0 || xx >= Width) continue;
                        if (blocks[xx + Width * (zz + yy * Length)] == Block.sponge)
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
                        if (blocks[xx + Width * (zz + yy * Length)] == Block.lava_sponge)
                            return true;
                    }
                }
            }
            return false;
        }

        void PhysSponge(int b, bool lava = false) {
            for (int y = -2; y <= +2; ++y)
                for (int z = -2; z <= +2; ++z)
                    for (int x = -2; x <= +2; ++x)
            {
                int index = IntOffset(b, x, y, z);
                byte block = GetTile(index);
                if (block == Block.Zero) continue;
                
                if ((!lava && Block.Convert(block) == Block.water) || (lava && Block.Convert(block) == Block.lava))
                    AddUpdate(index, Block.air);
            }
        }
        
        void PhysSpongeRemoved(int b, bool lava = false) {
            for (int y = -3; y <= +3; ++y)
                for (int z = -3; z <= +3; ++z)
                    for (int x = -3; x <= +3; ++x)
            {
                if (Math.Abs(x) == 3 || Math.Abs(y) == 3 || Math.Abs(z) == 3) //Calc only edge
                {
                    int index = IntOffset(b, x, y, z);
                    byte block = GetTile(index);
                    if (block == Block.Zero) continue;
                    
                    if ((!lava && Block.Convert(block) == Block.water) || (lava && Block.Convert(block) == Block.lava))
                        AddCheck(index);
                }
            }
        }
        
        void PhysFloatwood(int b) {
            int tempb = IntOffset(b, 0, -1, 0); //Get block below
            if (GetTile(tempb) != Block.Zero)
            {
                if (GetTile(tempb) == Block.air)
                {
                    AddUpdate(b, Block.air);
                    AddUpdate(tempb, Block.wood_float);
                    return;
                }
            }

            tempb = IntOffset(b, 0, 1, 0); //Get block above
            if (GetTile(tempb) != Block.Zero)
            {
                if (Block.Convert(GetTile(tempb)) == Block.water)
                {
                    AddUpdate(b, Block.water);
                    AddUpdate(tempb, Block.wood_float);
                    return;
                }
            }
        }

        public void MakeExplosion(ushort x, ushort y, ushort z, int size, bool force = false, TntWarsGame CheckForExplosionZone = null) {
            TntPhysics.MakeExplosion(this, x, y, z, size, force, CheckForExplosionZone);
        }

        public static class C4 {
            
            public static void BlowUp(ushort[] detenator, Level lvl) {
                try {
                    foreach (C4s c4 in lvl.C4list) {
                        if (c4.detenator[0] == detenator[0] && c4.detenator[1] == detenator[1] && c4.detenator[2] == detenator[2]) {
                            foreach (FillPos c in c4.list)
                                lvl.MakeExplosion(c.X, c.Y, c.Z, 0);
                            lvl.C4list.Remove(c4);
                        }
                    }
                } catch { }
            }
            
            public static sbyte NextCircuit(Level lvl) {
                sbyte number = 1;
                foreach (C4s c4 in lvl.C4list)
                    number++;
                return number;
            }
            
            public static C4s Find(Level lvl, sbyte CircuitNumber) {
                foreach (C4s c4 in lvl.C4list) {
                    if (c4.CircuitNumb == CircuitNumber)
                        return c4;
                }
                return null;
            }
            
            public class C4s {
                public sbyte CircuitNumb;
                public ushort[] detenator;
                public List<FillPos> list;

                public C4s(sbyte num) {
                    CircuitNumb = num;
                    list = new List<FillPos>();
                    detenator = new ushort[3];
                }
            }
        }
    }
    
    public class Check {
        public int b;
        public byte time;
        public object data;

        public Check(int b, object data) {
            this.b = b;
            this.data = data;
        }
        
        public Check(int b, string extraInfo = "") {
            this.b = b;
            this.data = extraInfo;
        }
    }

    public class Update {
        public int b;
        public object data;
        public byte type;

        public Update(int b, byte type) {
            this.b = b;
            this.type = type;
            this.data = "";
        }
        
        public Update(int b, byte type, object data) {
            this.b = b;
            this.type = type;
            this.data = data;
        }
    }
}