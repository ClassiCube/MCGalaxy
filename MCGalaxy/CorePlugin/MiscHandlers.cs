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
using MCGalaxy.Maths;
using MCGalaxy.Network;
using MCGalaxy.Util;

namespace MCGalaxy.Core {
    internal static class MiscHandlers {
        
        internal static void HandlePlayerMove(Player p, Position next, byte yaw, byte pitch) {
            if (!p.frozen) return;
            
            bool movedX = Math.Abs(next.X - p.Pos.X) > 4;  // moved more than 0.125 blocks horizontally
            bool movedY = Math.Abs(next.Y - p.Pos.Y) > 40; // moved more than 1.25 blocks vertically
            bool movedZ = Math.Abs(next.Z - p.Pos.Z) > 4;  // moved more than 0.125 blocks horizontally
            p.SetYawPitch(yaw, pitch);
            
            if (movedX || movedY || movedZ) { p.SendPos(Entities.SelfID, p.Pos, p.Rot); }
            Plugin.CancelPlayerEvent(PlayerEvents.PlayerMove, p);
        }
        
        internal static void HandleOnJoinedLevel(Player p, Level prevLevel, Level level) {
            p.AFKCooldown = DateTime.UtcNow.AddSeconds(2);
            p.prevMsg = "";
            p.showMBs = false;
            p.showPortals = false;
            p.ModelBB = AABB.ModelAABB(p.Model, level); // in case had been using a level-only custom block for their model
            
            if (!Hacks.CanUseHacks(p, level) && p.isFlying) {
                Player.Message(p, "You cannot use /fly on this map.");
                p.isFlying = false;
            }
            
            if (p.HasCpeExt(CpeExt.EnvWeatherType))
                p.Send(Packet.EnvWeatherType((byte)level.Config.Weather));
            if (p.HasCpeExt(CpeExt.EnvColors))
                p.SendCurrentEnvColors();
            p.SendCurrentMapAppearance();
            p.SendCurrentBlockPermissions();

            if (!level.Config.Guns && p.aiming) {
                p.aiming = false;
                p.ClearBlockchange();
            }
            if (!p.level.Config.UseBlockDB) {
                Player.Message(p, "BlockDB is disabled here, &cyou will not be able to /undo or /redo");
            }
            ShowWelcome(p);
        }
        
        static void ShowWelcome(Player p) {
            if (p.showedWelcome) return;
            p.showedWelcome = true;
            p.LastAction = DateTime.UtcNow;
            TextFile welcomeFile = TextFile.Files["Welcome"];
            
            try {
                welcomeFile.EnsureExists();
                string[] welcome = welcomeFile.GetText();
                Player.MessageLines(p, welcome);
            } catch (Exception ex) {
                Logger.LogError(ex);
            }
        }
        
        internal static void HandlePlayerClick(Player p, MouseButton button, MouseAction action, ushort yaw, ushort pitch,
                                               byte entity, ushort x, ushort y, ushort z, TargetBlockFace face) {
            if (p.level.Config.Deletable || action != MouseAction.Pressed || !p.level.IsValidPos(x, y, z)) return;
            
            ExtBlock block = p.level.GetBlock(x, y, z);
            bool isMB = p.level.BlockProps[block.Index].IsMessageBlock;
            bool isPortal = p.level.BlockProps[block.Index].IsPortal;

            if (isMB) { MessageBlock.Handle(p, x, y, z, true); }
            if (isPortal) { Portal.Handle(p, x, y, z); }
        }
        
        // Update rank colors and rank prefixes for online players
        internal static void HandleGroupLoad() {
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player p in players) {
                p.group = Group.Find(p.group.Permission);
                if (p.group == null) p.group = Group.standard;                
                p.SetPrefix();
                
                string dbCol = PlayerData.FindDBColor(p);
                if (dbCol == "" && p.color != p.group.Color) {
                    p.color = p.group.Color;
                    Entities.GlobalRespawn(p);
                }                
            }
        }
    }
}
