/*
    Copyright 2015 MCGalaxy
    
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
using MCGalaxy.BlockBehaviour;
using MCGalaxy.BlockPhysics;

namespace MCGalaxy {
    
    public sealed partial class Block {
        
        /// <summary> Returns whether this block handles the player placing a block at the given coordinates. </summary>
        /// <remarks>If this returns true, the usual 'dirt/grass below' behaviour and 'adding to the BlockDB' is skipped. </remarks>
        public delegate bool HandleDelete(Player p, byte oldBlock, ushort x, ushort y, ushort z);
        internal static HandleDelete[] deleteHandlers = new HandleDelete[256];
        
        /// <summary> Returns whether this block handles the player deleting a block at the given coordinates. </summary>
        /// <remarks>If this returns true, the usual 'checking dirt/grass below' and 'adding to the BlockDB' is skipped. </remarks>
        public delegate bool HandlePlace(Player p, byte oldBlock, ushort x, ushort y, ushort z);
        internal static HandlePlace[] placeHandlers = new Block.HandlePlace[256];
        
        /// <summary> Returns whether this block handles the player walking through this block at the given coordinates. </summary>
        /// <remarks>If this returns true, the usual 'death check' behaviour is skipped. </remarks>
        public delegate bool HandleWalkthrough(Player p, byte block, ushort x, ushort y, ushort z);
        internal static HandleWalkthrough[] walkthroughHandlers = new Block.HandleWalkthrough[256];
        
        /// <summary> Called to handle the physics for this particular block. </summary>
        /// <remarks>If this returns true, the usual 'death check' behaviour is skipped. </remarks>
        public delegate void HandlePhysics(Level lvl, ref Check C);
        internal static HandlePhysics[] physicsHandlers = new Block.HandlePhysics[256];
        internal static HandlePhysics[] physicsDoorsHandlers = new Block.HandlePhysics[256];
        
        static void SetupCoreHandlers() {
            deleteHandlers[Block.rocketstart] = DeleteBehaviour.RocketStart;
            deleteHandlers[Block.firework] = DeleteBehaviour.Firework;           
            deleteHandlers[Block.c4det] = DeleteBehaviour.C4Det;
            
            placeHandlers[Block.dirt] = PlaceBehaviour.Dirt;
            placeHandlers[Block.grass] = PlaceBehaviour.Grass;
            placeHandlers[Block.staircasestep] = PlaceBehaviour.Stairs;
            placeHandlers[Block.c4] = PlaceBehaviour.C4;
            placeHandlers[Block.c4det] = PlaceBehaviour.C4Det;
            
            walkthroughHandlers[Block.checkpoint] = WalkthroughBehaviour.Checkpoint;
            walkthroughHandlers[Block.air_switch] = WalkthroughBehaviour.Door;
            walkthroughHandlers[Block.water_door] = WalkthroughBehaviour.Door;
            walkthroughHandlers[Block.lava_door] = WalkthroughBehaviour.Door;
            walkthroughHandlers[Block.train] = WalkthroughBehaviour.Train;
            
            for (int i = 0; i < 256; i++) {
                if (Props[i].IsMessageBlock) {
                    walkthroughHandlers[i] = (p, block, x, y, z) => 
                        WalkthroughBehaviour.MessageBlock(p, block, x, y, z, true);
                    deleteHandlers[i] = (p, block, x, y, z) => 
                        WalkthroughBehaviour.MessageBlock(p, block, x, y, z, false);
                } else if (Props[i].IsPortal) {
                    walkthroughHandlers[i] = (p, block, x, y, z) => 
                        WalkthroughBehaviour.Portal(p, block, x, y, z, true);
                    deleteHandlers[i] = (p, block, x, y, z) => 
                        WalkthroughBehaviour.Portal(p, block, x, y, z, false);
                }
                
                if (Block.Props[i].IsTDoor) {
                    deleteHandlers[i] = DeleteBehaviour.RevertDoor;
                } else if (Props[i].ODoorId != Block.Zero) {
                    deleteHandlers[i] = DeleteBehaviour.ODoor;
                } else if (Block.Props[i].IsDoor) {
                    deleteHandlers[i] = DeleteBehaviour.Door;
                }
            }
            
            deleteHandlers[Block.door_tree_air] = DeleteBehaviour.RevertDoor;
            deleteHandlers[Block.door_tnt_air] = DeleteBehaviour.RevertDoor;
            deleteHandlers[Block.door_green_air] = DeleteBehaviour.RevertDoor;
            SetupCorePhysicsHandlers();
        }
        
        static void SetupCorePhysicsHandlers() {
            physicsHandlers[Block.birdblack] = BirdPhysics.Do;
            physicsHandlers[Block.birdwhite] = BirdPhysics.Do;
            physicsHandlers[Block.birdlava] = BirdPhysics.Do;
            physicsHandlers[Block.birdwater] = BirdPhysics.Do;
            physicsHandlers[Block.birdred] = (Level lvl, ref Check C) => HunterPhysics.DoKiller(lvl, ref C, Block.air);
            physicsHandlers[Block.birdblue] = (Level lvl, ref Check C) => HunterPhysics.DoKiller(lvl, ref C, Block.air);
            physicsHandlers[Block.birdkill] = (Level lvl, ref Check C) => HunterPhysics.DoKiller(lvl, ref C, Block.air);
            
            physicsHandlers[Block.snaketail] = SnakePhysics.DoTail;
            physicsHandlers[Block.snake] = SnakePhysics.Do;
            physicsHandlers[Block.rockethead] = RocketPhysics.Do;
            physicsHandlers[Block.firework] = FireworkPhysics.Do;
            physicsHandlers[Block.zombiebody] = ZombiePhysics.Do;
            physicsHandlers[Block.zombiehead] = ZombiePhysics.DoHead;
            physicsHandlers[Block.creeper] = ZombiePhysics.Do;

            physicsHandlers[Block.fishbetta] = (Level lvl, ref Check C) => HunterPhysics.DoKiller(lvl, ref C, Block.water);
            physicsHandlers[Block.fishshark] = (Level lvl, ref Check C) => HunterPhysics.DoKiller(lvl, ref C, Block.water);
            physicsHandlers[Block.fishlavashark] = (Level lvl, ref Check C) => HunterPhysics.DoKiller(lvl, ref C, Block.lava);
            physicsHandlers[Block.fishgold] = (Level lvl, ref Check C) => HunterPhysics.DoFlee(lvl, ref C, Block.water);
            physicsHandlers[Block.fishsalmon] = (Level lvl, ref Check C) => HunterPhysics.DoFlee(lvl, ref C, Block.water);
            physicsHandlers[Block.fishsponge] = (Level lvl, ref Check C) => HunterPhysics.DoFlee(lvl, ref C, Block.water);
            
            physicsHandlers[Block.water] = SimpleLiquidPhysics.DoWater;
            physicsHandlers[Block.activedeathwater] = SimpleLiquidPhysics.DoWater;
            physicsHandlers[Block.lava] = SimpleLiquidPhysics.DoLava;
            physicsHandlers[Block.activedeathlava] = SimpleLiquidPhysics.DoLava;
            physicsHandlers[Block.WaterDown] = ExtLiquidPhysics.DoWaterfall;
            physicsHandlers[Block.LavaDown] = ExtLiquidPhysics.DoLavafall;
            physicsHandlers[Block.WaterFaucet] = (Level lvl, ref Check C) => 
                ExtLiquidPhysics.DoFaucet(lvl, ref C, Block.WaterDown);
            physicsHandlers[Block.LavaFaucet] = (Level lvl, ref Check C) => 
                ExtLiquidPhysics.DoFaucet(lvl, ref C, Block.LavaDown);
            physicsHandlers[Block.finiteWater] = FinitePhysics.DoWaterOrLava;
            physicsHandlers[Block.finiteLava] = FinitePhysics.DoWaterOrLava;
            physicsHandlers[Block.finiteFaucet] = FinitePhysics.DoFaucet;
            physicsHandlers[Block.magma] = ExtLiquidPhysics.DoMagma;
            physicsHandlers[Block.geyser] = ExtLiquidPhysics.DoGeyser;
            physicsHandlers[Block.lava_fast] = SimpleLiquidPhysics.DoFastLava;
            physicsHandlers[Block.fastdeathlava] = SimpleLiquidPhysics.DoFastLava;
            
            physicsHandlers[Block.air] = AirPhysics.DoAir;
            physicsHandlers[Block.dirt] = OtherPhysics.DoDirt;
            physicsHandlers[Block.leaf] = LeafPhysics.DoLeaf;
            physicsHandlers[Block.shrub] = OtherPhysics.DoShrub;
            physicsHandlers[Block.fire] = FirePhysics.Do;
            physicsHandlers[Block.lava_fire] = FirePhysics.Do;
            physicsHandlers[Block.sand] = OtherPhysics.DoFalling;
            physicsHandlers[Block.gravel] = OtherPhysics.DoFalling;
            physicsHandlers[Block.cobblestoneslab] = OtherPhysics.DoStairs;
            physicsHandlers[Block.staircasestep] = OtherPhysics.DoStairs;
            physicsHandlers[Block.wood_float] = OtherPhysics.DoFloatwood;

            physicsHandlers[Block.sponge] = (Level lvl, ref Check C) => OtherPhysics.DoSponge(lvl, ref C, false);
            physicsHandlers[Block.lava_sponge] = (Level lvl, ref Check C) => OtherPhysics.DoSponge(lvl, ref C, true);

            //Special blocks that are not saved
            physicsHandlers[Block.air_flood] = (Level lvl, ref Check C) => 
                AirPhysics.DoFlood(lvl, ref C, AirFlood.Full, Block.air_flood);
            physicsHandlers[Block.air_flood_layer] = (Level lvl, ref Check C) => 
                AirPhysics.DoFlood(lvl, ref C, AirFlood.Layer, Block.air_flood_layer);
            physicsHandlers[Block.air_flood_down] = (Level lvl, ref Check C) => 
                AirPhysics.DoFlood(lvl, ref C, AirFlood.Down, Block.air_flood_down);
            physicsHandlers[Block.air_flood_up] = (Level lvl, ref Check C) => 
                AirPhysics.DoFlood(lvl, ref C, AirFlood.Up, Block.air_flood_up);
            
            physicsHandlers[Block.smalltnt] = TntPhysics.DoSmallTnt;
            physicsHandlers[Block.bigtnt] = (Level lvl, ref Check C) => TntPhysics.DoLargeTnt(lvl, ref C, 1);
            physicsHandlers[Block.nuketnt] = (Level lvl, ref Check C) => TntPhysics.DoLargeTnt(lvl, ref C, 4);
            physicsHandlers[Block.tntexplosion] = TntPhysics.DoTntExplosion;
            physicsHandlers[Block.train] = TrainPhysics.Do;
            
            for (int i = 0; i < 256; i++) {
                //Adv physics updating anything placed next to water or lava
                if ((i >= Block.red && i <= Block.redmushroom) || i == Block.wood ||
                    i == Block.trunk || i == Block.bookcase) {
                    physicsHandlers[i] = OtherPhysics.DoOther;
                    continue;
                }
                
                if (Block.odoor((byte)i) != Block.Zero) {
                    physicsHandlers[i] = DoorPhysics.oDoor;
                    physicsDoorsHandlers[i] = DoorPhysics.oDoor;
                }
            }
        }
    }
}
