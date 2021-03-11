/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
    Dual-licensed under the    Educational Community License, Version 2.0 and
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
using System.IO;
using MCGalaxy.Bots;
using MCGalaxy.Maths;
using MCGalaxy.Network;
using MCGalaxy.Commands;

namespace MCGalaxy {
    
    public sealed class PlayerBot : Entity {

        public bool hunt = false, kill = false;

        public string AIName = "", color;
        public string name, DisplayName;
        public string ClickedOnText;
        public string DeathMessage;
        public string Owner;
        public string ColoredName { get { return color + DisplayName; } }
        
        public byte id;
        public Level level;
        public int cur = 0;
        public int countdown = 0;
        public bool nodUp = false;
        public List<InstructionData> Instructions = new List<InstructionData>();
        
        public Position TargetPos;
        public bool movement = false;
        public int movementSpeed = 3;
        internal int curJump = 0;
        
        public PlayerBot(string n, Level lvl) {
            name = n; DisplayName = n; SkinName = n;
            color = "&1";      
            level = lvl;
            SetModel(Model);
            hasExtPositions = true;
            BotsScheduler.Activate();
        }
        
        public override bool CanSeeEntity(Entity other) { return true; }
        public override byte EntityID { get { return id; } }
        public override Level Level { get { return level; } }
        public override bool RestrictsScale { get { return false; } }
        
        public bool EditableBy(Player p, string attemptedAction = "modify") {
            if (CanEditAny(p)) { return true; }
            if (Owner == p.name) { return true; }
            
            p.Message("&WYou are not allowed to {0} bots that you did not create.", attemptedAction);
            return false;
        }
        
        public static bool CanEditAny(Player p) {
            if (LevelInfo.IsRealmOwner(p.level, p.name)) { return true; }
            ItemPerms perms = CommandExtraPerms.Find("Bot", 1) ?? new ItemPerms(LevelPermission.Operator);
            if (perms.UsableBy(p.Rank)) { return true; }
            return false;
        }
        
        public static void Add(PlayerBot bot, bool save = true) {
            // Lock to ensure that no two bots can end up with the same playerid
            lock (bot.level.Bots.locker) {
                bot.id = NextFreeId(bot);
                bot.level.Bots.Add(bot);
            }
            bot.GlobalSpawn();
            if (save) BotsFile.Save(bot.level);
        }

        public static void Remove(PlayerBot bot, bool save = true) {
            bot.level.Bots.Remove(bot);
            bot.GlobalDespawn();
            bot.curJump = 0;
            if (save) BotsFile.Save(bot.level);
        }
        
        internal static int RemoveLoadedBots(Level lvl, bool save) {
            PlayerBot[] bots = lvl.Bots.Items;
            for (int i = 0; i < bots.Length; i++) {
                Remove(bots[i], save);
            }
            return bots.Length;
        }
        
        internal static int RemoveBotsOwnedBy(Player p, string ownerName, Level lvl, bool save) {
            PlayerBot[] bots = lvl.Bots.Items;
            int removedCount = 0;
            for (int i = 0; i < bots.Length; i++) {
                if (ownerName.CaselessEq(bots[i].Owner)) {
                    Remove(bots[i], save);
                    removedCount++;
                }
            }
            return removedCount;
        }
        

        public void GlobalSpawn() {
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player p in players) {
                if (p.level == level) Entities.Spawn(p, this);
            }
        }

        public void GlobalDespawn() {
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player p in players) {
                if (p.level == level) Entities.Despawn(p, this);
            }
        }
        
        unsafe static byte NextFreeId(PlayerBot bot) {
            byte* used = stackalloc byte[256];
            for (int i = 0; i < 256; i++) used[i] = 0;

            PlayerBot[] bots = bot.level.Bots.Items;
            for (int i = 0; i < bots.Length; i++) {
                byte id = bots[i].id;
                used[id] = 1;
            }
            
            for (byte i = 127; i >= 64; i--) {
                if (used[i] == 0) return i;
            }
            // NOTE: For some clients these IDs mean self ID
            for (byte i = 254; i > 127; i--) {
                if (used[i] == 0) return i;
            }
            // NOTE: These IDs may conflict with player IDs, so use as a last resort
            for (byte i = 63; i > 0; i--) {
                if (used[i] == 0) return i;
            }
            return Entities.SelfID;
        }

        
        public void AdvanceRotation() {
            if (!movement && Instructions.Count > 0) {
                Orientation rot = Rot;
                if (rot.RotY < 245) rot.RotY += 8;
                else rot.RotY = 0;

                if (rot.HeadX > 32 && rot.HeadX < 64) rot.HeadX = 224;
                else if (rot.HeadX > 250) rot.HeadX = 0;
                else rot.HeadX += 4;
                Rot = rot;
            }
        }
        
        public void NextInstruction() {
            cur++;
            if (cur == Instructions.Count) cur = 0;
        }

        
        public static void GlobalUpdatePosition() {
            Level[] levels = LevelInfo.Loaded.Items;
            for (int i = 0; i < levels.Length; i++) {
                UpdatePositions(levels[i]);
            }
        }
        
        unsafe static void UpdatePositions(Level lvl) {
            byte* src = stackalloc byte[16 * 256]; // 16 = size of absolute update, with extended positions
            byte* ext = stackalloc byte[16 * 256];
            byte* ptrSrc = src, ptrExt = ext;
            
            PlayerBot[] bots = lvl.Bots.Items;
            for (int i = 0; i < bots.Length; i++) {
                PlayerBot bot = bots[i];
                if (bot.movement) bot.PerformMovement();
                Position pos = bot.Pos; Orientation rot = bot.Rot;
                
                Entities.GetPositionPacket(ref ptrSrc, bot.id, true, false,
                                           pos, bot.lastPos, rot, bot.lastRot);
                Entities.GetPositionPacket(ref ptrExt, bot.id, true, true,
                                           pos, bot.lastPos, rot, bot.lastRot);
                bot.lastPos = pos; bot.lastRot = rot;
            }
            
            int countSrc = (int)(ptrSrc - src);
            if (countSrc == 0) return;
            int countExt = (int)(ptrExt - ext);
            
            byte[] srcPacket = new byte[countSrc];
            byte[] extPacket = new byte[countExt];
            for (int i = 0; i < srcPacket.Length; i++) { srcPacket[i] = src[i]; }
            for (int i = 0; i < extPacket.Length; i++) { extPacket[i] = ext[i]; }

            Player[] players = PlayerInfo.Online.Items;
            foreach (Player p in players) {
                if (p.level != lvl) continue;
                byte[] packet = p.hasExtPositions ? extPacket : srcPacket;
                p.Send(packet);
            }
        }
        
        static AABB[] downs = new AABB[16], ups = new AABB[16];
        static int downsCount, upsCount;
        
        void RecalcDownExtent(ref AABB bb, int steps, int dx, int dz) {
            AABB downExtent = bb.Adjust(dx * steps, -32, dz * steps);
            downsCount = AABB.FindIntersectingSolids(downExtent, level, ref downs);
        }
        
        void RecalcUpExtent(ref AABB bb, int steps, int dx, int dz) {
            AABB upExtent = bb.Adjust(dx * steps, 32, dz * steps);
            upsCount = AABB.FindIntersectingSolids(upExtent, level, ref ups);
        }
        
        void PerformMovement() {
            double scale = Math.Ceiling(Server.Config.PositionUpdateInterval / 25.0);
            int steps = movementSpeed * (int)scale;
            
            downsCount = -1;
            for (int i = 0; i < steps; i++) DoMove(steps);
        }
        
        void DoMove(int steps) {
            Position pos = Pos;
            AABB bb = ModelBB.OffsetPosition(pos);
            int dx = Math.Sign(TargetPos.X - pos.X);
            int dz = Math.Sign(TargetPos.Z - pos.Z);
            
            if (downsCount == -1) {
                RecalcDownExtent(ref bb, steps, dx, dz);
                RecalcUpExtent(ref bb, steps, dx, dz);
            }
            
            // Advance the AABB to the bot's next position
            bb = bb.Offset(dx, 0, dz);
            AABB bbCopy = bb;
            
            // Attempt to drop the bot down up to 1 block
            int hitY = -32;
            for (int dy = 0; dy >= -32; dy--) {
                bool intersectsAny = false;
                for (int i = 0; i < downsCount; i++) {
                    if (AABB.Intersects(ref bb, ref downs[i])) { intersectsAny = true; break; }
                }
                
                if (intersectsAny) { hitY = dy + 1; break; }
                bb.Min.Y--; bb.Max.Y--;
            }
            
            // Does the bot fall down a block
            if (hitY < 0) {
                pos.X += dx; pos.Y += hitY; pos.Z += dz; Pos = pos;
                RecalcDownExtent(ref bb, steps, dx, dz);
                RecalcUpExtent(ref bb, steps, dx, dz);
                return;
            }
            
            // Attempt to move the bot up to 1 block
            bb = bbCopy;
            
            for (int dy = 0; dy <= 32; dy++) {
                bool intersectsAny = false;
                for (int i = 0; i < upsCount; i++) {
                    if (AABB.Intersects(ref bb, ref ups[i])) { intersectsAny = true; break; }
                }
                
                if (!intersectsAny) {
                    pos.X += dx; pos.Y += dy; pos.Z += dz; Pos = pos;
                    
                    if (dy != 0) {
                        RecalcDownExtent(ref bb, steps, dx, dz);
                        RecalcUpExtent(ref bb, steps, dx, dz);
                    }
                    return;
                }
                bb.Min.Y++; bb.Max.Y++;
            }
        }
        
        public void DisplayInfo(Player p) {
            p.Message("Bot {0} &S({1}) has:", ColoredName, name);
            p.Message("  Owner: &f{0}", string.IsNullOrEmpty(Owner) ? "no one" : p.FormatNick(Owner));
            if (!String.IsNullOrEmpty(AIName)) { p.Message("  AI: &f{0}", AIName); }
            if (hunt || kill)                  { p.Message("  Hunt: &f{0}&S, Kill: %f{1}", hunt, kill); }
            if (SkinName != name)              { p.Message("  Skin: &f{0}", SkinName); }
            if (Model != "humanoid")           { p.Message("  Model: &f{0}", Model); }
            if (!(ScaleX == 0 && ScaleY == 0 && ScaleZ == 0)) {
                p.Message("  X scale: &a{0}&S, Y scale: &a{1}&S, Z scale: &a{2}",
                         ScaleX == 0 ? "none" : ScaleX.ToString(),
                         ScaleY == 0 ? "none" : ScaleY.ToString(),
                         ScaleZ == 0 ? "none" : ScaleZ.ToString()
                        );
            }
            
            if (String.IsNullOrEmpty(ClickedOnText)) return;
            ItemPerms perms = CommandExtraPerms.Find("About", 1) ?? new ItemPerms(LevelPermission.AdvBuilder);
            if (!perms.UsableBy(p.Rank)) return; //don't show bot's ClickedOnText if player isn't allowed to see message block contents
            p.Message("  Clicked-on text: {0}", ClickedOnText);
        }
        
        
        /*
         * Old water/lava swimming code - TODO: need to fix.
         * 
                if ((ushort)(foundPos[1] / 32) > y) {
                    if (b1 == Block.water || b1 == Block.waterstill || b1 == Block.lava || b1 == Block.lavastill) {
                        if (Block.Walkthrough(b2)) {
                            pos[1] = (ushort)(pos[1] + (Math.Sign(foundPos[1] - pos[1])));
                        }
                    } else if (b2 == Block.water || b2 == Block.waterstill || b2 == Block.lava || b2 == Block.lavastill) {
                        pos[1] = (ushort)(pos[1] + (Math.Sign(foundPos[1] - pos[1])));
                    }
                } else if ((ushort)(foundPos[1] / 32) < y) {
                    if (b3 == Block.water || b3 == Block.waterstill || b3 == Block.lava || b3 == Block.lavastill) {
                        pos[1] = (ushort)(pos[1] + (Math.Sign(foundPos[1] - pos[1])));
                    }
                }*/
    }
}