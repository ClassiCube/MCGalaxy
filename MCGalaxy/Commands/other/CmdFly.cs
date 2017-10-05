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
using MCGalaxy.Maths;
using MCGalaxy.Tasks;

namespace MCGalaxy.Commands.Misc {
    public sealed class CmdFly : Command {
        public override string name { get { return "Fly"; } }
        public override string type { get { return CommandTypes.Other; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public override bool SuperUseable { get { return false; } }

        public override void Use(Player p, string message) {
            if (!Hacks.CanUseHacks(p, p.level)) {
                Player.Message(p, "You cannot use %T/Fly %Son this map.");
                p.isFlying = false; return;
            }
            
            p.isFlying = !p.isFlying;
            if (!p.isFlying) return;
            
            Player.Message(p, "You are now flying. &cJump!");
            
            FlyState state = new FlyState();
            state.player = p;
            SchedulerTask task = new SchedulerTask(FlyCallback, state, TimeSpan.Zero, true);
            p.CriticalTasks.Add(task);
        }
        
        class FlyState {
            public Player player;
            public Position oldPos = default(Position);
            public List<Vec3U16> lastGlass = new List<Vec3U16>();
            public List<Vec3U16> glassCoords = new List<Vec3U16>();
        }
        
        static void FlyCallback(SchedulerTask task) {
            FlyState state = (FlyState)task.State;
            Player p = state.player;
            if (state.player.isFlying) { DoFly(state); return; }
            
            foreach (Vec3U16 pos in state.lastGlass) {
                p.SendBlockchange(pos.X, pos.Y, pos.Z, ExtBlock.Air);
            }            
            Player.Message(p, "Stopped flying");
            task.Repeating = false;
        }

        static void DoFly(FlyState state) {
            Player p = state.player;
            if (p.Pos == state.oldPos) return;

            int x = p.Pos.BlockX, z = p.Pos.BlockZ;
            int y = (p.Pos.Y - 60) / 32;
            ExtBlock glass = (ExtBlock)Block.Glass;

            for (int yy = y - 1; yy <= y; yy++)
                for (int zz = z - 2; zz <= z + 2; zz++)
                    for (int xx = x - 2; xx <= x + 2; xx++)
            {
                Vec3U16 pos;
                pos.X = (ushort)xx; pos.Y = (ushort)yy; pos.Z = (ushort)zz;
                if (p.level.IsAirAt(pos.X, pos.Y, pos.Z)) state.glassCoords.Add(pos);
            }
            
            foreach (Vec3U16 P in state.glassCoords) {
                if (state.lastGlass.Contains(P)) continue;
                state.lastGlass.Add(P);
                p.SendBlockchange(P.X, P.Y, P.Z, glass);
            }
            
            for (int i = 0; i < state.lastGlass.Count; i++) {
                Vec3U16 P = state.lastGlass[i];
                if (state.glassCoords.Contains(P)) continue;
                
                p.RevertBlock(P.X, P.Y, P.Z);
                state.lastGlass.RemoveAt(i); i--;
            }
            
            state.glassCoords.Clear();
            state.oldPos = p.Pos;
        }
        
        public override void Help(Player p) {
            string name = Group.GetColoredName(LevelPermission.Operator);
            Player.Message(p, "%T/Fly");
            Player.Message(p, "%HCreates a grass platform underneath you that moves with you.");
            Player.Message(p, "%H  May not work if you have high latency.");
            Player.Message(p, "%H  Cannot be used on maps which have -hax in their motd. " +
                           "(unless you are {0}%H+ and the motd has +ophax)", name);
        }
    }
}
