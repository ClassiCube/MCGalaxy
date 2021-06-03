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
using MCGalaxy.Blocks.Extended;
using MCGalaxy.DB;
using MCGalaxy.Events;
using MCGalaxy.Events.PlayerEvents;
using MCGalaxy.Maths;
using MCGalaxy.Network;
using MCGalaxy.Util;
using BlockID = System.UInt16;

namespace MCGalaxy.Core {
    internal static class MiscHandlers {
        
        internal static void HandlePlayerMove(Player p, Position next, byte yaw, byte pitch) {
            if (!p.frozen) return;
            
            bool movedX = Math.Abs(next.X - p.Pos.X) > 4;  // moved more than 0.125 blocks horizontally
            bool movedY = Math.Abs(next.Y - p.Pos.Y) > 40; // moved more than 1.25 blocks vertically
            bool movedZ = Math.Abs(next.Z - p.Pos.Z) > 4;  // moved more than 0.125 blocks horizontally
            p.SetYawPitch(yaw, pitch);
            
            if (movedX || movedY || movedZ) { p.SendPos(Entities.SelfID, p.Pos, p.Rot); }
            p.cancelmove = true;
        }
        
        internal static void HandleSentMap(Player p, Level prevLevel, Level level) {
            p.AFKCooldown = DateTime.UtcNow.AddSeconds(2);
            p.prevMsg = "";
            p.showMBs = false;
            p.showPortals = false;
            p.SetModel(p.Model); // in case had been using a level-only custom block for their model

            p.ZoneIn = null;
            OnChangedZoneEvent.Call(p);
            p.SendCurrentTextures();
            p.SendCurrentBlockPermissions();
            
            // TODO: unshow old zones here??
            if (p.Supports(CpeExt.SelectionCuboid)) {
                Zone[] zones = level.Zones.Items;
                foreach (Zone zn in zones) { zn.Show(p); }
            }

            if (p.weapon != null && !level.Config.Guns) p.weapon.Disable();
            if (!level.Config.UseBlockDB) {
                p.Message("BlockDB is disabled here, &Wyou will not be able to /undo or /redo");
            }
        }
		
		internal static void HandleChangedZone(Player p) {
			if (p.Supports(CpeExt.InstantMOTD)) p.SendMapMotd();
            p.SendCurrentEnv();
            
            if (p.isFlying && !Hacks.CanUseFly(p)) {
                p.Message("You cannot use &T/Fly &Son this map.");
                p.isFlying = false;
            }
        }
        
        internal static void HandlePlayerClick(Player p, MouseButton button, MouseAction action, ushort yaw, ushort pitch,
                                               byte entity, ushort x, ushort y, ushort z, TargetBlockFace face) {
            if (action != MouseAction.Pressed) return;           
            if (entity != Entities.SelfID && ClickOnBot(p, entity)) return;
            
            // if Deletable is enabled, handle MBs/Portals in SetBlock instead
            if (p.level.Config.Deletable || !p.level.IsValidPos(x, y, z)) return;
            BlockID block = p.level.GetBlock(x, y, z);
            bool isMB     = p.level.Props[block].IsMessageBlock;
            bool isPortal = p.level.Props[block].IsPortal;

            if (isMB) { MessageBlock.Handle(p, x, y, z, true); }
            if (isPortal) { Portal.Handle(p, x, y, z); }
        }
        
        static bool ClickOnBot(Player p, byte entity) {
            PlayerBot[] bots = p.level.Bots.Items;
            for (int i = 0; i < bots.Length; i++) {
                if (bots[i].EntityID != entity) continue;
                if (bots[i].ClickedOnText == null && !p.checkingBotInfo) return false;
                
                Vec3F32 delta = p.Pos.ToVec3F32() - bots[i].Pos.ToVec3F32();
                float reachSq = p.ReachDistance * p.ReachDistance;
                if (delta.LengthSquared > (reachSq + 1)) return false;
                
                if (p.checkingBotInfo) {
                    bots[i].DisplayInfo(p);
                    p.checkingBotInfo = false;
                    return true;
                }
                string message = bots[i].ClickedOnText;
                MessageBlock.Execute(p, message, bots[i].Pos.FeetBlockCoords);
                return true;
            }
            return false;
        }       
    }
}
