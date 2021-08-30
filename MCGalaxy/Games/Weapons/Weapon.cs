/*
    Copyright 2011 MCForge
        
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
using MCGalaxy.Commands;
using MCGalaxy.Events.PlayerEvents;
using MCGalaxy.Maths;
using MCGalaxy.Tasks;
using BlockID = System.UInt16;

namespace MCGalaxy.Games {

    /// <summary> Represents a weapon which can interact with blocks or players until it dies. </summary>
    public abstract class Weapon {

        public abstract string Name { get; }
        static bool hookedEvents;
        
        protected Player p;
        AimBox aimer;
        
        /// <summary> Applies this weapon to the given player, and sets up necessary state. </summary>
        public virtual void Enable(Player p) {
            if (!hookedEvents) {
                OnPlayerClickEvent.Register(PlayerClickCallback, Priority.Low);
                OnBlockChangingEvent.Register(BlockChangingCallback, Priority.Low);
                hookedEvents = true;
            }
            
            this.p   = p;
            p.ClearBlockchange();
            p.weapon = this;
            
            if (p.Supports(CpeExt.PlayerClick)) {
                p.Message(Name + " engaged, click to fire at will");
            } else {                
                p.Message(Name + " engaged, fire at will");
                p.aiming = true;
                aimer = new AimBox();
                aimer.Hook(p);
            }
        }

        public virtual void Disable() {
            p.aiming = false;
            p.Message(Name + " disabled");
            p.weapon = null;
        }
        
        /// <summary> Called when the player fires this weapon. </summary>
        /// <remarks> Activated by clicking through either PlayerClick or on a glass box around the player. </remarks>
        protected abstract void OnActivated(Vec3F32 dir, BlockID block);

        
        static void BlockChangingCallback(Player p, ushort x, ushort y, ushort z, BlockID block, bool placing, ref bool cancel) {
            Weapon weapon = p.weapon;
            if (weapon == null) return;
            
            // Revert block back since client assumes changes always succeeds
            p.RevertBlock(x, y, z);
            cancel = true;
            
            // Defer to player click handler if PlayerClick supported
            if (weapon.aimer == null) return;
            
            if (!p.level.Config.Guns) { weapon.Disable(); return; }
            if (!CommandParser.IsBlockAllowed(p, "use", block)) return;

            Vec3F32 dir = DirUtils.GetDirVector(p.Rot.RotY, p.Rot.HeadX);
            weapon.OnActivated(dir, block);
        }
        
        static void PlayerClickCallback(Player p, MouseButton btn, MouseAction action,
                                        ushort yaw, ushort pitch, byte entity,
                                        ushort x, ushort y, ushort z, TargetBlockFace face) {
            Weapon weapon = p.weapon;
            if (weapon == null || action != MouseAction.Pressed) return;
            
            if (!(btn == MouseButton.Left || btn == MouseButton.Right)) return;
            if (!p.level.Config.Guns) { weapon.Disable(); return; }
            
            BlockID held = p.ClientHeldBlock;
            if (!CommandParser.IsBlockAllowed(p, "use", held)) return;
            
            Vec3F32 dir = DirUtils.GetDirVectorExt(yaw, pitch);
            weapon.OnActivated(dir, held);
        }
        
        protected static Player PlayerAt(Player p, Vec3U16 pos, bool skipSelf) {
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player pl in players) {
                if (pl.level != p.level) continue;
                if (p == pl && skipSelf) continue;
                
                if (Math.Abs(pl.Pos.BlockX - pos.X)    <= 1
                    && Math.Abs(pl.Pos.BlockY - pos.Y) <= 1
                    && Math.Abs(pl.Pos.BlockZ - pos.Z) <= 1)
                {
                    return pl;
                }
            }
            return null;
        }
        
        public static WeaponType ParseType(string type) {
            if (type.Length == 0) return WeaponType.Normal;
            if (type.CaselessEq("destroy")) return WeaponType.Destroy;
            if (type.CaselessEq("tp") || type.CaselessEq("teleport")) return WeaponType.Teleport;
            if (type.CaselessEq("explode")) return WeaponType.Explode;
            if (type.CaselessEq("laser"))   return WeaponType.Laser;
            return WeaponType.Invalid;
        }
    }
    
    public enum WeaponType { Invalid, Normal, Destroy, Teleport, Explode, Laser };
    
    public class AmmunitionData {
        public BlockID block;
        public Vec3U16 start;
        public Vec3F32 dir;
        public bool moving = true;
        
        // positions of all currently visible "trailing" blocks
        public List<Vec3U16> visible = new List<Vec3U16>();
        // position of all blocks this ammunition has touched/gone through
        public List<Vec3U16> all = new List<Vec3U16>();
        public int iterations;
        
        public Vec3U16 PosAt(int i) {
            Vec3U16 target;
            target.X = (ushort)Math.Round(start.X + (double)(dir.X * i));
            target.Y = (ushort)Math.Round(start.Y + (double)(dir.Y * i));
            target.Z = (ushort)Math.Round(start.Z + (double)(dir.Z * i));
            return target;
        }
        
        public void DoTeleport(Player p) {
            int i = visible.Count - 3;
            if (i >= 0 && i < visible.Count) {
                Vec3U16 coords = visible[i];
                Position pos   = new Position(coords.X * 32, coords.Y * 32 + 32, coords.Z * 32);
                p.SendPosition(pos, p.Rot);
            }
        }
    }
    
    /// <summary> Manages the glass box around the player. Adjusts based on where player is looking. </summary>
    internal sealed class AimBox {
        
        Player player;
        List<Vec3U16> lastGlass = new List<Vec3U16>();
        List<Vec3U16> curGlass  = new List<Vec3U16>();
        
        public void Hook(Player p) {
            player = p;
            SchedulerTask task = new SchedulerTask(AimCallback, null, TimeSpan.Zero, true);
            p.CriticalTasks.Add(task);
        }
        
        void AimCallback(SchedulerTask task) {
            Player p = player;
            if (p.aiming) { Update(); return; }
            
            foreach (Vec3U16 pos in lastGlass) {
                if (!p.level.IsValidPos(pos)) continue;
                p.RevertBlock(pos.X, pos.Y, pos.Z);
            }
            task.Repeating = false;
        }
        
        void Update() {
            Player p = player;
            Vec3F32 dir = DirUtils.GetDirVector(p.Rot.RotY, p.Rot.HeadX);
            ushort x = (ushort)Math.Round(p.Pos.BlockX + dir.X * 3);
            ushort y = (ushort)Math.Round(p.Pos.BlockY + dir.Y * 3);
            ushort z = (ushort)Math.Round(p.Pos.BlockZ + dir.Z * 3);

            int dx = Math.Sign(dir.X) >= 0 ? 1 : -1, dz = Math.Sign(dir.Z) >= 0 ? 1 : -1;
            Check(p.level, x,      y, z     );
            Check(p.level, x + dx, y, z     );
            Check(p.level, x,      y, z + dz);
            Check(p.level, x + dx, y, z + dz);

            // Revert all glass blocks now not in the ray from the player's direction
            for (int i = 0; i < lastGlass.Count; i++) {
                Vec3U16 pos = lastGlass[i];
                if (curGlass.Contains(pos)) continue;
                
                if (p.level.IsValidPos(pos))
                    p.RevertBlock(pos.X, pos.Y, pos.Z);
                lastGlass.RemoveAt(i); i--;
            }

            // Place the new glass blocks that are in the ray from the player's direction
            foreach (Vec3U16 pos in curGlass) {
                if (lastGlass.Contains(pos)) continue;
                lastGlass.Add(pos);
                p.SendBlockchange(pos.X, pos.Y, pos.Z, Block.Glass);
            }
            curGlass.Clear();
        }
        
        void Check(Level lvl, int x, int y, int z) {
            Vec3U16 pos = new Vec3U16((ushort)x, (ushort)(y - 1), (ushort)z);
            if (lvl.IsAirAt(pos.X, pos.Y, pos.Z)) curGlass.Add(pos);
            
            pos.Y++;
            if (lvl.IsAirAt(pos.X, pos.Y, pos.Z)) curGlass.Add(pos);
        }
    }
}
