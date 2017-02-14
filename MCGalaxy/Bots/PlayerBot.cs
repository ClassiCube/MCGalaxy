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

namespace MCGalaxy {
    
    public sealed class PlayerBot {
        
        [Obsolete("Use PlayerBot.Bots.Items instead")]
        public static List<PlayerBot> playerbots;
        public static VolatileArray<PlayerBot> Bots = new VolatileArray<PlayerBot>(true);

        public bool hunt = false, kill = false;

        public string AIName = "", color, model = "humanoid";
        public AABB ModelBB;
        public string name, SkinName, DisplayName;
        public string ColoredName { get { return color + DisplayName; } }
        
        public byte id;
        public Level level;
        public int cur = 0;
        public int countdown = 0;
        public bool nodUp = false;
        public List<InstructionData> Instructions = new List<InstructionData>();
        
        public ushort[] pos = new ushort[3], oldpos = new ushort[3], foundPos = new ushort[3];
        public byte[] rot = new byte[2], oldrot = new byte[2];
        public bool movement = false;
        public int movementSpeed = 3;
        internal bool jumping = false;
        internal int currentjump = 0;

        System.Timers.Timer botTimer = new System.Timers.Timer(100);

        public PlayerBot(string n, Level lvl, ushort x, ushort y, ushort z, byte rotx, byte roty) {
            name = n;
            DisplayName = n;
            SkinName = n;
            color = "&1";
            ModelBB = AABB.ModelAABB(model, lvl);
            
            level = lvl;
            id = NextFreeId(this);
            SetPos(x, y, z, rotx, roty);

            botTimer.Elapsed += BotTimerFunc;
            botTimer.Start();
        }
        
        public void SetPos(ushort x, ushort y, ushort z, byte rotx, byte roty) {
            pos = new ushort[3] { x, y, z };
            rot = new byte[2] { rotx, roty };
        }
        
        public static void Add(PlayerBot bot, bool save = true) {
            Bots.Add(bot);
            bot.GlobalSpawn();
            
            Chat.MessageLevel(bot.level, bot.ColoredName + "%S, the bot, has been added.");
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
            RemoveAll(lvl, false);
        }
        
        public static void RemoveAllFromLevel(Level lvl) {
            RemoveAll(lvl, true);
            BotsFile.RemoveLevelBots(lvl.name);
        }
        
        static void RemoveAll(Level lvl, bool save) {
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
        
        public static PlayerBot FindMatchesPreferLevel(Player pl, string name) {
            if (Player.IsSuper(pl)) return Matcher.FindBots(pl, name);
            
            // Try for exact match in current level, then partial match against all bots
            PlayerBot[] bots = Bots.Items;
            for (int i = 0; i < bots.Length; i++) {
                if (pl.level == bots[i].level && name.CaselessEq(bots[i].name))
                    return bots[i];
            }
            return Matcher.FindBots(pl, name);
        }
        
        void UpdatePosition() {
            if (movement) {
                double scale = Math.Ceiling(Server.updateTimer.Interval / 25.0);
                int steps = movementSpeed * (int)scale;
                for (int i = 0; i < steps; i++)
                    DoMove();
            }
            
            byte[] packet = Entities.GetPositionPacket(this);
            oldpos = pos; oldrot = rot;
            if (packet == null) return;

            Player[] players = PlayerInfo.Online.Items;
            foreach (Player p in players)
                if (p.level == level) p.Send(packet);
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
                if (rot[0] < 245) rot[0] += 8;
                else rot[0] = 0;

                if (rot[1] > 32 && rot[1] < 64) rot[1] = 224;
                else if (rot[1] > 250) rot[1] = 0;
                else rot[1] += 4;
            }
        }
        
        public void NextInstruction() {
            cur++;
            if (cur == Instructions.Count) cur = 0;
        }
        
        void DoMove() {
            AABB bb = ModelBB.OffsetPosition(pos);
            // Advance the AABB to the bot's next position
            int dx = Math.Sign(foundPos[0] - pos[0]), dz = Math.Sign(foundPos[2] - pos[2]);
            bb = bb.Offset(dx, 0, dz);
            AABB bbCopy = bb;
            
            // Attempt to drop the bot down up to 1 block
            int fallY = -32;
            for (int dy = 0; dy >= -32; dy--) {
                if (AABB.IntersectsSolidBlocks(bb, level)) { fallY = dy + 1; break; }
                bb.Min.Y--; bb.Max.Y--;
            }
            
            // Does the bot fall down a block
            if (fallY < 0) {
                pos[0] += (ushort)dx; pos[1] += (ushort)fallY; pos[2] += (ushort)dz;
                return;
            }
            
            // Attempt to move the bot up to 1 block
            bb = bbCopy;
            for (int dy = 0; dy <= 32; dy++) {
                if (!AABB.IntersectsSolidBlocks(bb, level)) {
                    pos[0] += (ushort)dx; pos[1] += (ushort)dy; pos[2] += (ushort)dz;
                    return;
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
            switch (currentjump) {
                 case 1: pos[1] += 24; break;
                 case 2: pos[1] += 12; break;
                 case 3: break;
                 case 4: pos[1] -= 12; break;
                 case 5: pos[1] -= 24; jumping = false; currentjump = 0; break;
            }
        }
    }
}