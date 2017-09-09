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

        /// <summary> Retrieves the default place block handler for the given block. </summary>
        internal static HandlePlace GetPlaceHandler(ExtBlock block, BlockProps[] props) {
            switch (block.BlockID) {
                case Block.Dirt: return PlaceBehaviour.Dirt;
                case Block.Grass: return PlaceBehaviour.Grass;
                case Block.C4: return PlaceBehaviour.C4;
                case Block.C4Detonator: return PlaceBehaviour.C4Det;
            }
            
            if (props[block.Index].StackId != Block.Air) return PlaceBehaviour.Stack(block);
            return null;
        }
        
        /// <summary> Retrieves the default delete block handler for the given block. </summary>
        internal static HandleDelete GetDeleteHandler(ExtBlock block, BlockProps[] props) {
            switch (block.BlockID) {
                case Block.RocketStart: return DeleteBehaviour.RocketStart;
                case Block.Fireworks: return DeleteBehaviour.Firework;
                case Block.C4Detonator: return DeleteBehaviour.C4Det;
                case Block.Door_Log_air: return DeleteBehaviour.RevertDoor;
                case Block.Door_TNT_air: return DeleteBehaviour.RevertDoor;
                case Block.Door_Green_air: return DeleteBehaviour.RevertDoor;
            }
            
            int i = block.Index;
            if (props[i].IsMessageBlock) return DeleteBehaviour.DoMessageBlock;
            if (props[i].IsPortal) return DeleteBehaviour.DoPortal;
            
            if (props[i].IsTDoor) return DeleteBehaviour.RevertDoor;
            if (props[i].oDoorIndex != Block.Invalid) return DeleteBehaviour.oDoor;
            if (props[i].IsDoor) return DeleteBehaviour.Door;
            return null;
        }

        /// <summary> Retrieves the default walkthrough block handler for the given block. </summary>
        internal static HandleWalkthrough GetWalkthroughHandler(ExtBlock block, BlockProps[] props, bool nonSolid) {
            switch (block.BlockID) {
                case Block.Checkpoint: return WalkthroughBehaviour.Checkpoint;
                case Block.Door_AirActivatable: return WalkthroughBehaviour.Door;
                case Block.Door_Water: return WalkthroughBehaviour.Door;
                case Block.Door_Lava: return WalkthroughBehaviour.Door;
                case Block.Train: return WalkthroughBehaviour.Train;
            }
            
            int i = block.Index;
            if (props[i].IsMessageBlock && nonSolid) return WalkthroughBehaviour.DoMessageBlock;
            if (props[i].IsPortal && nonSolid) return WalkthroughBehaviour.DoPortal;
            return null;
        }

        
        /// <summary> Retrieves the default physics block handler for the given block. </summary>
        internal static HandlePhysics GetPhysicsHandler(ExtBlock block, BlockProps[] props) {
            switch (block.BlockID) {
                case Block.SnakeTail: return SnakePhysics.DoTail;
                case Block.Snake: return SnakePhysics.Do;
                case Block.RocketHead: return RocketPhysics.Do;
                case Block.Fireworks: return FireworkPhysics.Do;
                case Block.ZombieBody: return ZombiePhysics.Do;
                case Block.ZombieHead: return ZombiePhysics.DoHead;
                case Block.Creeper: return ZombiePhysics.Do;
                    
                case Block.Water: return SimpleLiquidPhysics.DoWater;
                case Block.Deadly_ActiveWater: return SimpleLiquidPhysics.DoWater;
                case Block.Lava: return SimpleLiquidPhysics.DoLava;
                case Block.Deadly_ActiveLava: return SimpleLiquidPhysics.DoLava;
                case Block.WaterDown: return ExtLiquidPhysics.DoWaterfall;
                case Block.LavaDown: return ExtLiquidPhysics.DoLavafall;
                
                case Block.WaterFaucet: return (Level lvl, ref Check C) =>
                    ExtLiquidPhysics.DoFaucet(lvl, ref C, Block.WaterDown);
                case Block.LavaFaucet: return (Level lvl, ref Check C) =>
                    ExtLiquidPhysics.DoFaucet(lvl, ref C, Block.LavaDown);
                    
                case Block.FiniteWater: return FinitePhysics.DoWaterOrLava;
                case Block.FiniteLava: return FinitePhysics.DoWaterOrLava;
                case Block.FiniteFaucet: return FinitePhysics.DoFaucet;
                case Block.Magma: return ExtLiquidPhysics.DoMagma;
                case Block.Geyser: return ExtLiquidPhysics.DoGeyser;
                case Block.FastLava: return SimpleLiquidPhysics.DoFastLava;
                case Block.Deadly_FastLava: return SimpleLiquidPhysics.DoFastLava;
                    
                case Block.Air: return AirPhysics.DoAir;
                case Block.Dirt: return OtherPhysics.DoDirt;
                case Block.Grass: return OtherPhysics.DoGrass;
                case Block.Leaves: return LeafPhysics.DoLeaf;
                case Block.Sapling: return OtherPhysics.DoShrub;
                case Block.Fire: return FirePhysics.Do;
                case Block.LavaFire: return FirePhysics.Do;
                case Block.Sand: return OtherPhysics.DoFalling;
                case Block.Gravel: return OtherPhysics.DoFalling;
                case Block.FloatWood: return OtherPhysics.DoFloatwood;

                case Block.Sponge: return (Level lvl, ref Check C) => 
                    OtherPhysics.DoSponge(lvl, ref C, false);
                case Block.LavaSponge: return (Level lvl, ref Check C) => 
                    OtherPhysics.DoSponge(lvl, ref C, true);

                // Special blocks that are not saved
                case Block.Air_Flood: return (Level lvl, ref Check C) =>
                    AirPhysics.DoFlood(lvl, ref C, AirFlood.Full, Block.Air_Flood);
                case Block.Air_FloodLayer: return (Level lvl, ref Check C) =>
                    AirPhysics.DoFlood(lvl, ref C, AirFlood.Layer, Block.Air_FloodLayer);
                case Block.Air_FloodDown: return (Level lvl, ref Check C) =>
                    AirPhysics.DoFlood(lvl, ref C, AirFlood.Down, Block.Air_FloodDown);
                case Block.Air_FloodUp: return (Level lvl, ref Check C) =>
                    AirPhysics.DoFlood(lvl, ref C, AirFlood.Up, Block.Air_FloodUp);
                    
                case Block.TNT_Small: return TntPhysics.DoSmallTnt;
                case Block.TNT_Big: return (Level lvl, ref Check C) =>
                    TntPhysics.DoLargeTnt(lvl, ref C, 1);
                case Block.TNT_Nuke: return (Level lvl, ref Check C) => 
                    TntPhysics.DoLargeTnt(lvl, ref C, 4);
                case Block.TNT_Explosion: return TntPhysics.DoTntExplosion;
                case Block.Train: return TrainPhysics.Do;
            }

            int i = block.Index;
            HandlePhysics animalAI = AnimalAIHandler(props[i].AnimalAI);
            if (animalAI != null) return animalAI;
            if (props[i].oDoorIndex != Block.Invalid) return DoorPhysics.oDoor;
            
            i = block.BlockID; // TODO: should this be checking WaterKills/LavaKills
            // Adv physics updating anything placed next to water or lava
            if ((i >= Block.Red && i <= Block.RedMushroom) || i == Block.Wood || i == Block.Log || i == Block.Bookshelf) {
                return OtherPhysics.DoOther;
            }
            return null;
        }
        
        /// <summary> Retrieves the default physics block handler for the given block. </summary>
        internal static HandlePhysics GetPhysicsDoorsHandler(ExtBlock block, BlockProps[] props) {
            if (props[block.Index].oDoorIndex != Block.Invalid) return DoorPhysics.oDoor;
            return null;
        }
        
        static HandlePhysics AnimalAIHandler(AnimalAI ai) {
            if (ai == AnimalAI.Fly) return BirdPhysics.Do;

            if (ai == AnimalAI.FleeAir) {
                return (Level lvl, ref Check C) => HunterPhysics.DoFlee(lvl, ref C, Block.Air);
            } else if (ai == AnimalAI.FleeWater) {
                return (Level lvl, ref Check C) => HunterPhysics.DoFlee(lvl, ref C, Block.Water);
            } else if (ai == AnimalAI.FleeLava) {
                return (Level lvl, ref Check C) => HunterPhysics.DoFlee(lvl, ref C, Block.Lava);
            }
            
            if (ai == AnimalAI.KillerAir) {
                return (Level lvl, ref Check C) => HunterPhysics.DoKiller(lvl, ref C, Block.Air);
            } else if (ai == AnimalAI.KillerWater) {
                return (Level lvl, ref Check C) => HunterPhysics.DoKiller(lvl, ref C, Block.Water);
            } else if (ai == AnimalAI.KillerLava) {
                return (Level lvl, ref Check C) => HunterPhysics.DoKiller(lvl, ref C, Block.Lava);
            }
            return null;
        }
    }
}
