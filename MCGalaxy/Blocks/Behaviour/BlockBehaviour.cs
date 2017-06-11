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
using MCGalaxy.Blocks.Physics;

namespace MCGalaxy.Blocks {

    /// <summary> Handles the player placing a block at the given coordinates. </summary>
    /// <remarks> Use p.ChangeBlock to do a normal player block change (adds to BlockDB, updates dirt/grass beneath) </remarks>
    public delegate void HandleDelete(Player p, ExtBlock oldBlock, ushort x, ushort y, ushort z);

    /// <summary> Handles the player deleting a block at the given coordinates. </summary>
    /// <remarks> Use p.ChangeBlock to do a normal player block change (adds to BlockDB, updates dirt/grass beneath) </remarks>
    public delegate void HandlePlace(Player p, ExtBlock oldBlock, ushort x, ushort y, ushort z);

    /// <summary> Returns whether this block handles the player walking through this block at the given coordinates. </summary>
    /// <remarks> If this returns true, the usual 'death check' behaviour is skipped. </remarks>
    public delegate bool HandleWalkthrough(Player p, ExtBlock block, ushort x, ushort y, ushort z);

    /// <summary> Called to handle the physics for this particular block. </summary>
    public delegate void HandlePhysics(Level lvl, ref Check C);
    
    public static class BlockBehaviour {
        internal static HandleDelete[] deleteHandlers = new HandleDelete[Block.Count];
        internal static HandlePlace[] placeHandlers = new HandlePlace[Block.Count];
        internal static HandleWalkthrough[] walkthroughHandlers = new HandleWalkthrough[Block.Count];
        internal static HandlePhysics[] physicsHandlers = new HandlePhysics[Block.Count];
        internal static HandlePhysics[] physicsDoorsHandlers = new HandlePhysics[Block.Count];
        
        /// <summary> Initalises default deleting, placing, and walkthrough handling behaviour for the core blocks. </summary>
        internal static void SetDefaultHandlers() {
            for (int i = 0; i < Block.Count; i++) {
                SetDefaultHandler(i);
            }
        }
        
        internal static void SetDefaultHandler(int i) {
            ExtBlock block = new ExtBlock((byte)i, 0);
            deleteHandlers[i] = GetDeleteHandler(block, Block.Props);
            placeHandlers[i] = GetPlaceHandler(block, Block.Props);
            
            bool nonSolid = Block.Walkthrough(Block.Convert((byte)i));
            walkthroughHandlers[i] = GetWalkthroughHandler(block, Block.Props, nonSolid);
            
            physicsHandlers[i] = GetPhysicsHandler(block, Block.Props);
            physicsDoorsHandlers[i] = GetPhysicsDoorsHandler(block, Block.Props);
        }
        
        
        /// <summary> Retrieves the default place block handler for the given block. </summary>
        internal static HandlePlace GetPlaceHandler(ExtBlock block, BlockProps[] props) {
            switch (block.BlockID) {
                case Block.dirt: return PlaceBehaviour.Dirt;
                case Block.grass: return PlaceBehaviour.Grass;
                case Block.staircasestep: return PlaceBehaviour.Stairs;
                case Block.cobblestoneslab: return PlaceBehaviour.CobbleStairs;
                case Block.c4: return PlaceBehaviour.C4;
                case Block.c4det: return PlaceBehaviour.C4Det;
            }
            return null;
        }
        
        /// <summary> Retrieves the default delete block handler for the given block. </summary>
        internal static HandleDelete GetDeleteHandler(ExtBlock block, BlockProps[] props) {
            switch (block.BlockID) {
                case Block.rocketstart: return DeleteBehaviour.RocketStart;
                case Block.firework: return DeleteBehaviour.Firework;
                case Block.c4det: return DeleteBehaviour.C4Det;
                case Block.custom_block: return DeleteBehaviour.CustomBlock;
                case Block.door_tree_air: return DeleteBehaviour.RevertDoor;
                case Block.door_tnt_air: return DeleteBehaviour.RevertDoor;
                case Block.door_green_air: return DeleteBehaviour.RevertDoor;
            }
            
            int i = block.Index;
            if (props[i].IsMessageBlock) return DeleteBehaviour.DoMessageBlock;
            if (props[i].IsPortal) return DeleteBehaviour.DoPortal;
            
            if (props[i].IsTDoor) return DeleteBehaviour.RevertDoor;
            if (props[i].ODoorId != Block.Invalid) return DeleteBehaviour.ODoor;
            if (props[i].IsDoor) return DeleteBehaviour.Door;
            return null;
        }

        /// <summary> Retrieves the default walkthrough block handler for the given block. </summary>
        internal static HandleWalkthrough GetWalkthroughHandler(ExtBlock block, BlockProps[] props, bool nonSolid) {
            switch (block.BlockID) {
                case Block.checkpoint: return WalkthroughBehaviour.Checkpoint;
                case Block.air_switch: return WalkthroughBehaviour.Door;
                case Block.air_door: return WalkthroughBehaviour.Door;
                case Block.water_door: return WalkthroughBehaviour.Door;
                case Block.lava_door: return WalkthroughBehaviour.Door;
                case Block.train: return WalkthroughBehaviour.Train;
                case Block.custom_block: return WalkthroughBehaviour.CustomBlock;
            }
            
            int i = block.Index;
            if (props[i].IsMessageBlock && nonSolid) return WalkthroughBehaviour.DoMessageBlock;
            if (props[i].IsPortal && nonSolid) return WalkthroughBehaviour.DoPortal;
            return null;
        }

        
        /// <summary> Retrieves the default physics block handler for the given block. </summary>
        internal static HandlePhysics GetPhysicsHandler(ExtBlock block, BlockProps[] props) {
            switch (block.BlockID) {
                case Block.snaketail: return SnakePhysics.DoTail;
                case Block.snake: return SnakePhysics.Do;
                case Block.rockethead: return RocketPhysics.Do;
                case Block.firework: return FireworkPhysics.Do;
                case Block.zombiebody: return ZombiePhysics.Do;
                case Block.zombiehead: return ZombiePhysics.DoHead;
                case Block.creeper: return ZombiePhysics.Do;
                    
                case Block.water: return SimpleLiquidPhysics.DoWater;
                case Block.activedeathwater: return SimpleLiquidPhysics.DoWater;
                case Block.lava: return SimpleLiquidPhysics.DoLava;
                case Block.activedeathlava: return SimpleLiquidPhysics.DoLava;
                case Block.WaterDown: return ExtLiquidPhysics.DoWaterfall;
                case Block.LavaDown: return ExtLiquidPhysics.DoLavafall;
                
                case Block.WaterFaucet: return (Level lvl, ref Check C) =>
                    ExtLiquidPhysics.DoFaucet(lvl, ref C, Block.WaterDown);
                case Block.LavaFaucet: return (Level lvl, ref Check C) =>
                    ExtLiquidPhysics.DoFaucet(lvl, ref C, Block.LavaDown);
                    
                case Block.finiteWater: return FinitePhysics.DoWaterOrLava;
                case Block.finiteLava: return FinitePhysics.DoWaterOrLava;
                case Block.finiteFaucet: return FinitePhysics.DoFaucet;
                case Block.magma: return ExtLiquidPhysics.DoMagma;
                case Block.geyser: return ExtLiquidPhysics.DoGeyser;
                case Block.lava_fast: return SimpleLiquidPhysics.DoFastLava;
                case Block.fastdeathlava: return SimpleLiquidPhysics.DoFastLava;
                    
                case Block.air: return AirPhysics.DoAir;
                case Block.dirt: return OtherPhysics.DoDirt;
                case Block.leaf: return LeafPhysics.DoLeaf;
                    case Block.shrub: return OtherPhysics.DoShrub;
                case Block.fire: return FirePhysics.Do;
                case Block.lava_fire: return FirePhysics.Do;
                case Block.sand: return OtherPhysics.DoFalling;
                case Block.gravel: return OtherPhysics.DoFalling;
                case Block.cobblestoneslab: return OtherPhysics.DoStairs;
                case Block.staircasestep: return OtherPhysics.DoStairs;
                case Block.wood_float: return OtherPhysics.DoFloatwood;

                case Block.sponge: return (Level lvl, ref Check C) => 
                    OtherPhysics.DoSponge(lvl, ref C, false);
                case Block.lava_sponge: return (Level lvl, ref Check C) => 
                    OtherPhysics.DoSponge(lvl, ref C, true);

                // Special blocks that are not saved
                case Block.air_flood: return (Level lvl, ref Check C) =>
                    AirPhysics.DoFlood(lvl, ref C, AirFlood.Full, Block.air_flood);
                case Block.air_flood_layer: return (Level lvl, ref Check C) =>
                    AirPhysics.DoFlood(lvl, ref C, AirFlood.Layer, Block.air_flood_layer);
                case Block.air_flood_down: return (Level lvl, ref Check C) =>
                    AirPhysics.DoFlood(lvl, ref C, AirFlood.Down, Block.air_flood_down);
                case Block.air_flood_up: return (Level lvl, ref Check C) =>
                    AirPhysics.DoFlood(lvl, ref C, AirFlood.Up, Block.air_flood_up);
                    
                case Block.smalltnt: return TntPhysics.DoSmallTnt;
                case Block.bigtnt: return (Level lvl, ref Check C) =>
                    TntPhysics.DoLargeTnt(lvl, ref C, 1);
                case Block.nuketnt: return (Level lvl, ref Check C) => 
                    TntPhysics.DoLargeTnt(lvl, ref C, 4);
                case Block.tntexplosion: return TntPhysics.DoTntExplosion;
                case Block.train: return TrainPhysics.Do;
            }

            int i = block.Index;
            HandlePhysics animalAI = AnimalAIHandler(props[i].AnimalAI);
            if (animalAI != null) return animalAI;
            if (props[i].ODoorId != Block.Invalid) return DoorPhysics.oDoor;
            
            i = block.BlockID; // TODO: should this be checking WaterKills/LavaKills
            // Adv physics updating anything placed next to water or lava
            if ((i >= Block.red && i <= Block.redmushroom) || i == Block.wood || i == Block.trunk || i == Block.bookcase) {
                return OtherPhysics.DoOther;
            }
            return null;
        }
        
        /// <summary> Retrieves the default physics block handler for the given block. </summary>
        internal static HandlePhysics GetPhysicsDoorsHandler(ExtBlock block, BlockProps[] props) {
            if (props[block.Index].ODoorId != Block.Invalid) return DoorPhysics.oDoor;
            return null;
        }
        
        static HandlePhysics AnimalAIHandler(AnimalAI ai) {
            if (ai == AnimalAI.Fly) return BirdPhysics.Do;

            if (ai == AnimalAI.FleeAir) {
                return (Level lvl, ref Check C) => HunterPhysics.DoFlee(lvl, ref C, Block.air);
            } else if (ai == AnimalAI.FleeWater) {
                return (Level lvl, ref Check C) => HunterPhysics.DoFlee(lvl, ref C, Block.water);
            } else if (ai == AnimalAI.FleeLava) {
                return (Level lvl, ref Check C) => HunterPhysics.DoFlee(lvl, ref C, Block.lava);
            }
            
            if (ai == AnimalAI.KillerAir) {
                return (Level lvl, ref Check C) => HunterPhysics.DoKiller(lvl, ref C, Block.air);
            } else if (ai == AnimalAI.KillerWater) {
                return (Level lvl, ref Check C) => HunterPhysics.DoKiller(lvl, ref C, Block.water);
            } else if (ai == AnimalAI.KillerLava) {
                return (Level lvl, ref Check C) => HunterPhysics.DoKiller(lvl, ref C, Block.lava);
            }
            return null;
        }
    }
}
