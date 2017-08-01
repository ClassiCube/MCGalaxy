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
using MCGalaxy.Bots;
using MCGalaxy.Maths;
using MCGalaxy.Network;

namespace MCGalaxy {
    
    public sealed class PlayerBot : Entity {

        public bool hunt = false, kill = false;

        public string AIName = "", color;
        public string name, DisplayName;
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
        internal bool jumping = false;
        internal int currentjump = 0;
        
        public PlayerBot(string n, Level lvl) { Init(n, lvl); }
        
        public PlayerBot(string n, Level lvl, ushort x, ushort y, ushort z, byte rotx, byte roty) {
            Pos = new Position(x, y, z);
            SetYawPitch(rotx, roty);
            Init(n, lvl);
        }
        
        void Init(string n, Level lvl) {
            name = n; DisplayName = n; SkinName = n;
            color = "&1";
            ModelBB = AABB.ModelAABB(Model, lvl);
            
            level = lvl;
            id = NextFreeId(this);
            hasExtPositions = true;
            BotsScheduler.Activate();
        }
        
        public override bool CanSeeEntity(Entity other) { return true; }
        public override byte EntityID { get { return id; } }
        public override Level Level { get { return level; } }
        
        public static void Add(PlayerBot bot, bool save = true) {
            bot.level.Bots.Add(bot);
            bot.GlobalSpawn();            
            if (save) BotsFile.UpdateBot(bot);
        }

        public static void Remove(PlayerBot bot, bool save = true) {
            bot.level.Bots.Remove(bot);
            bot.GlobalDespawn();
            bot.jumping = false;
            if (save) BotsFile.RemoveBot(bot);
        }
        
        public static void UnloadFromLevel(Level lvl) {
            BotsFile.UnloadBots(lvl);
            RemoveLoadedBots(lvl, false);
        }
        
        public static void RemoveAllFromLevel(Level lvl) {
            RemoveLoadedBots(lvl, true);
            BotsFile.DeleteBots(lvl.name);
        }
        
        static void RemoveLoadedBots(Level lvl, bool save) {
            PlayerBot[] bots = lvl.Bots.Items;
            for (int i = 0; i < bots.Length; i++) {
                Remove(bots[i], save);
            }
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
        
        public static void GlobalUpdatePosition() {
            Level[] levels = LevelInfo.Loaded.Items;
            for (int i = 0; i < levels.Length; i++) {
                PlayerBot[] bots = levels[i].Bots.Items;
                for (int j = 0; j < bots.Length; j++) { bots[j].UpdatePosition(); }
            }
        }
        
        void UpdatePosition() {
            if (movement) {
                double scale = Math.Ceiling(ServerConfig.PositionUpdateInterval / 25.0);
                int steps = movementSpeed * (int)scale;
                for (int i = 0; i < steps; i++)
                    DoMove();
            }
            
            Position pos = Pos; Orientation rot = Rot;
            if (pos == lastPos && rot.HeadX == lastRot.HeadX && rot.RotY == lastRot.RotY) return;
            lastPos = pos; lastRot = rot;
            
            // TODO: relative position updates, combine packets
            byte[] packet = Packet.Teleport(id, pos, rot, false);
            byte[] extPacket = Packet.Teleport(id, pos, rot, true);

            Player[] players = PlayerInfo.Online.Items;
            foreach (Player p in players) {
                if (p.level != level) continue;
                
                if (p.hasExtPositions) p.Send(extPacket);
                else p.Send(packet);
            }
        }

        
        unsafe static byte NextFreeId(PlayerBot bot) {
            byte* used = stackalloc byte[256];
            for (int i = 0; i < 256; i++)
                used[i] = 0;

            // Lock to ensure that no two bots can end up with the same playerid
            lock (bot.level.Bots.locker) {
                PlayerBot[] bots = bot.level.Bots.Items;
                for (int i = 0; i < bots.Length; i++) {
                    byte id = bots[i].id;
                    used[id] = 1;
                }
            }
            
            for (byte i = 127; i >= 64; i-- ) {
                if (used[i] == 0) return i;
            }
            // NOTE: For some clients these IDs mean self ID
            for (byte i = 254; i > 127; i-- ) {
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
        
        void DoMove() {
            Position pos = Pos;
            AABB bb = ModelBB.OffsetPosition(pos);
            // Advance the AABB to the bot's next position
            int dx = Math.Sign(TargetPos.X - Pos.X), dz = Math.Sign(TargetPos.Z - Pos.Z);
            bb = bb.Offset(dx, 0, dz);
            AABB bbCopy = bb;
            
            // Attempt to drop the bot down up to 1 block
            int hitY = -32;
            for (int dy = 0; dy >= -32; dy--) {
                if (AABB.IntersectsSolidBlocks(bb, level)) { hitY = dy + 1; break; }
                bb.Min.Y--; bb.Max.Y--;
            }          
            
            // Does the bot fall down a block
            if (hitY < 0) {
                pos.X += dx; pos.Y += hitY; pos.Z += dz;
                Pos = pos; return;
            }
            
            // Attempt to move the bot up to 1 block
            bb = bbCopy;
            for (int dy = 0; dy <= 32; dy++) {
                if (!AABB.IntersectsSolidBlocks(bb, level)) {
                    pos.X += dx; pos.Y += dy; pos.Z += dz;
                    Pos = pos; return;
                }
                bb.Min.Y++; bb.Max.Y++;
            }
            
            /*
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
}