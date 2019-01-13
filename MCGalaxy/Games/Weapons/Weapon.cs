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
	/// <remarks> Activated by clicking through either PlayerClick or a glass box around the player. </remarks>
	public abstract class Weapon {

		public abstract string Name { get; }
		static bool hookedEvents;
		
		protected Player p;
		AimBox aimer;
		
		public void Enable(Player p) {
			if (!hookedEvents) {
				OnPlayerClickEvent.Register(PlayerClickCallback, Priority.Low);
				hookedEvents = true;
			}
			this.p = p;
			p.ClearBlockchange();
			
			if (p.Supports(CpeExt.PlayerClick)) {
				p.Message(Name + " engaged, click to fire at will");
			} else {
				p.Blockchange += BlockClickCallback;
				p.Message(Name + " engaged, fire at will");
				aimer = new AimBox();
				aimer.Hook(p);
			}
		}

		public void Disable() {
			p.aiming = false;
			p.ClearBlockchange();
			p.Message(Name + " disabled");
		}
		
		protected abstract void OnActivated(byte yaw, byte pitch, BlockID block);

		
		static void BlockClickCallback(Player p, ushort x, ushort y, ushort z, BlockID block) {
			Weapon weapon = p.weapon;
			if (weapon == null) return;
			
			p.RevertBlock(x, y, z);
			// defer to player click handler
			if (weapon.aimer == null) return;
			
			if (!p.level.Config.Guns) { weapon.Disable(); return; }
			if (!CommandParser.IsBlockAllowed(p, "use", block)) return;

			weapon.OnActivated(p.Rot.RotY, p.Rot.HeadX, block);
		}
		
		static void PlayerClickCallback(Player p, MouseButton btn, MouseAction action,
		                                ushort yaw, ushort pitch, byte entity,
		                                ushort x, ushort y, ushort z, TargetBlockFace face) {
			Weapon weapon = p.weapon;
			if (weapon == null || action != MouseAction.Pressed) return;
			
			if (!(btn == MouseButton.Left || btn == MouseButton.Right)) return;
			if (!p.level.Config.Guns) { weapon.Disable(); return; }
			
			BlockID held = p.RawHeldBlock;
			if (!CommandParser.IsBlockAllowed(p, "use", held)) return;
			weapon.OnActivated((byte)(yaw >> 8), (byte)(pitch >> 8), held);
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
	}
	
	public class AmmunitionData {
		public BlockID block;
		public Vec3U16 pos, start;
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
