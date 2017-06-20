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
using System.Threading;
using System.Timers;
using MCGalaxy.Bots;
using MCGalaxy.Maths;

namespace MCGalaxy {
    
    public sealed class PlayerBot : Entity {
        
        [Obsolete("Use PlayerBot.Bots.Items instead")]
        public static List<PlayerBot> playerbots;
        public static VolatileArray<PlayerBot> Bots = new VolatileArray<PlayerBot>(true);

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
        public ushort[] pos = new ushort[3];
        public byte[] rot = new byte[2];
        public bool movement = false;
        public int movementSpeed = 3;
        internal bool jumping = false;
        internal int currentjump = 0;

        System.Timers.Timer botTimer = new System.Timers.Timer(100);

        
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

            botTimer.Elapsed += BotTimerFunc;
            botTimer.Start();
        }
        
        public override bool CanSeeEntity(Entity other) { return true; }
        public override byte EntityID { get { return id; } }
        public override Level Level { get { return level; } }
        
        protected override void OnSetPos() {
            Position p = Pos;
            pos[0] = (ushort)p.X; pos[1] = (ushort)p.Y; pos[2] = (ushort)p.Z;
        }
        
        protected override void OnSetRot() {
            Orientation r = Rot;
            rot[0] = r.RotY; rot[1] = r.HeadX;
        }
        
        public static void Add(PlayerBot bot, bool save = true) {
            Bots.Add(bot);
            bot.GlobalSpawn();
            
            if (save)
                BotsFile.UpdateBot(bot);
        }

        public static void Remove(PlayerBot bot, bool save = true) {
            Bots.Remove(bot);
            bot.GlobalDespawn();
            
            bot.botTimer.Stop();
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
            PlayerBot[] bots = Bots.Items;
            for (int i = 0; i < bots.Length; i++) {
                PlayerBot bot = bots[i];
                if (bots[i].level == lvl) Remove(bot, save);
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
                if (p.level == level) Entities.Despawn(p, id);
            }
        }
        
        public static void GlobalUpdatePosition() {
            PlayerBot[] bots = Bots.Items;
            foreach (PlayerBot b in bots) b.UpdatePosition();
        }
        
        
        public static PlayerBot Find(string name) {
            PlayerBot match = null; int matches = 0;
            PlayerBot[] bots = Bots.Items;

            foreach (PlayerBot bot in bots) {
                if (bot.name.CaselessEq(name)) return bot;
                if (bot.name.CaselessContains(name)) {
                    match = bot; matches++;
                }
            }
            return matches == 1 ? match : null;
        }
        
        void UpdatePosition() {
            if (movement) {
                double scale = Math.Ceiling(Server.PositionInterval / 25.0);
                int steps = movementSpeed * (int)scale;
                for (int i = 0; i < steps; i++)
                    DoMove();
            }
            
            // TODO: check if external code modified pos/rot
            byte[] packet = Entities.GetPositionPacket(this, false);
            lastPos = Pos; lastRot = Rot;
            if (packet == null) return;
            byte[] extPacket = Entities.GetPositionPacket(this, true);

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
            lock (Bots.locker) {
                PlayerBot[] bots = Bots.Items;
                for (int i = 0; i < bots.Length; i++) {
                    if (bots[i].level != bot.level) continue;
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
        
        
        // Script handling
        void BotTimerFunc(object sender, ElapsedEventArgs e) {
            if (kill) {
                InstructionData data = default(InstructionData);
                BotInstruction.Find("kill").Execute(this, data);
            }
            movement = false;

            if (Instructions.Count == 0) {
                if (hunt) {
                    InstructionData data = default(InstructionData);
                    BotInstruction.Find("hunt").Execute(this, data);
                }
            } else {
                bool doNextInstruction = !DoInstruction();
                if (cur == Instructions.Count) cur = 0;
                if (doNextInstruction) {
                    DoInstruction();
                    if (cur == Instructions.Count) cur = 0;
                }
            }
            
            if (jumping) DoJump();
        }
        
        bool DoInstruction() {
            BotInstruction ins = BotInstruction.Find(Instructions[cur].Name);
            if (ins == null) return false;
            return ins.Execute(this, Instructions[cur]);
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
        
        void DoJump() {
            currentjump++;
            Position pos = Pos;
            switch (currentjump) {
                 case 1: pos.Y += 24; break;
                 case 2: pos.Y += 12; break;
                 case 3: break;
                 case 4: pos.Y -= 12; break;
                 case 5: pos.Y -= 24; jumping = false; currentjump = 0; break;
            }
            Pos = pos;
        }
    }
}