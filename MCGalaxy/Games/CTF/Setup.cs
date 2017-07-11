/*
    Copyright 2011 MCForge
        
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
using System.Collections.Generic;
using System.IO;
using MCGalaxy.Events.PlayerEvents;

namespace MCGalaxy.Games {
    /// <summary> This plugin creates CTF Config files for you by using /ctfsetup in-game </summary>
    public sealed class CtfSetup : Plugin_Simple {
        static Dictionary<Player, SetupData> cache = new Dictionary<Player, SetupData>();
        public override string creator { get { return "GamezGalaxy"; } }
        public override string MCGalaxy_Version { get { return ""; } }
        public override string name { get { return "/ctfsetup"; }  }

        public override void Load(bool startup) {
            OnPlayerCommandEvent.Register(OnCommand, Priority.Critical);
            OnPlayerChatEvent.Register(OnChat, Priority.Critical);
            OnBlockChangeEvent.Register(OnBlock, Priority.Critical);
            OnPlayerDisconnectEvent.Register(OnDisconnect, Priority.Critical);
        }
        
        public override void Unload(bool shutdown) {
            OnPlayerCommandEvent.Unregister(OnCommand);
            OnPlayerChatEvent.Unregister(OnChat);
            OnBlockChangeEvent.Unregister(OnBlock);
            OnPlayerDisconnectEvent.Unregister(OnDisconnect);
        }
        
        
        void OnDisconnect(Player p, string reason) { cache.Remove(p); }
        
        void OnBlock(Player p, ushort x, ushort y, ushort z, ExtBlock block) {
            if (!cache.ContainsKey(p)) return;
            
            switch (cache[p].s) {
                case Step.GetBlueFlag:
                    cache[p].bx = x;
                    cache[p].by = y;
                    cache[p].bz = z;
                    cache[p].blue = p.level.GetBlock(x, y, z);
                    Player.Message(p, "Ok! I got the blue flag, now can you show me the red flag?");
                    Player.Message(p, "Just hit it");
                    cache[p].s = Step.GetRedFlag;
                    break;
                case Step.GetRedFlag:
                    cache[p].rx = x;
                    cache[p].ry = y;
                    cache[p].rz = z;
                    cache[p].red = p.level.GetBlock(x, y, z);
                    Player.Message(p, "Got it!");
                    Player.Message(p, "Now I can do random spawns, or do you have a spawn in mind?");
                    Player.Message(p, "Say - (Random/Set)");
                    cache[p].s = Step.RandomorSet;
                    break;
            }
        }
        
        void Finish(Player p, int bx, int by, int bz, int rx, int ry, int rz) {
            Player.Message(p, "I'll set the tag points and capture points to their defaults");
            Player.Message(p, "You can change them by going into CTF/<mapname>.config :)");
            WriteMapConfig(p, bx, by, bz, rx, ry, rz);
            
            using (StreamWriter w = new StreamWriter("CTF/maps.config", true))
                w.WriteLine(cache[p].current.name);
            if (!Directory.Exists("CTF/maps")) Directory.CreateDirectory("CTF/maps");
            File.Copy("levels/" + cache[p].current.name + ".lvl", "CTF/maps/" + cache[p].current.name + ".lvl", true);
        }
        
        static void WriteMapConfig(Player p, int bx, int by, int bz, int rx, int ry, int rz) {
            List<string> config = new List<string>();
            config.Add("base.red.x=" + cache[p].rx);
            config.Add("base.red.y=" + cache[p].ry);
            config.Add("base.red.z=" + cache[p].rz);
            config.Add("base.blue.x=" + cache[p].bx);
            config.Add("base.blue.y=" + cache[p].by);
            config.Add("base.blue.z=" + cache[p].bz);
            config.Add("map.line.z=" + cache[p].middle);
            config.Add("base.red.block=" + cache[p].red.RawID);
            config.Add("base.blue.block=" + cache[p].blue.RawID);
            config.Add("game.maxpoints=3");
            config.Add("game.tag.points-gain=5");
            config.Add("game.tag.points-lose=5");
            config.Add("game.capture.points-gain=10");
            config.Add("game.capture.points-lose=10");
            
            if (bx != 0 && by != 0 && bz != 0 && rx != 0 && ry != 0 && rz != 0) {
                config.Add("base.blue.spawnx=" + bx);
                config.Add("base.blue.spawny=" + by);
                config.Add("base.blue.spawnz=" + bz);
                config.Add("base.red.spawnx=" + rx);
                config.Add("base.red.spawny=" + ry);
                config.Add("base.red.spawnz=" + rz);
            }
            File.WriteAllLines("CTF/" + cache[p].current.name + ".config", config.ToArray());
        }
        
        void OnChat(Player p, string message) {
            if (!cache.ContainsKey(p)) return;
            
            if (message.CaselessEq("random")) {
                if (cache[p].s == Step.RandomorSet) {
                    Player.Message(p, "Ok random spawns it is!");
                    Finish(p, 0, 0, 0, 0, 0, 0);
                    cache.Remove(p);
                    Player.Message(p, "Setup Complete!");
                }
            } else if (message.CaselessEq("set")) {
                if (cache[p].s == Step.RandomorSet) {
                    Player.Message(p, "Ok, can you stand in the blue spawn and say \"continue\" (without the \" \")");
                    cache[p].s = Step.BlueSetSpawn;
                }
            } else if (message.CaselessEq("continue")) {
                switch (cache[p].s)
                {
                    case Step.GetCenter:
                        cache[p].middle = p.Pos.BlockZ;
                        Player.Message(p, "I got " + cache[p].middle);
                        Player.Message(p, "Ok, now I need to know where the blue flag is. Can you point me to it?");
                        Player.Message(p, "Simply hit the block..");
                        cache[p].s = Step.GetBlueFlag;
                        break;
                    case Step.BlueSetSpawn:
                        cache[p].bluex = p.Pos.X;
                        cache[p].bluey = p.Pos.Y;
                        cache[p].bluez = p.Pos.Z;
                        Player.Message(p, "Ok, now can you stand in the red spawn and say \"conintue\"");
                        cache[p].s = Step.RedSetSpawn;
                        break;
                    case Step.RedSetSpawn:
                        Player.Message(p, "ALMOST DONE!");
                        Finish(p, cache[p].bluex, cache[p].bluey, cache[p].bluez, p.Pos.X, p.Pos.Y, p.Pos.Z);
                        cache.Remove(p);
                        Player.Message(p, "Setup Complete!");
                        break;
                }
            }
        }
        
        void OnCommand(Player p, string cmd, string args) {
            if (!cmd.CaselessEq("ctfsetup")) return;

            Player.Message(p, "%2Hello and welcome to the noob friendly CTF setup :D");
            if (!Directory.Exists("CTF")) Directory.CreateDirectory("CTF");
            Player.Message(p, "I'll setup this map, but first can you stand in the middle of the map?");
            Player.Message(p, "Once you get to the middle type \"continue\" in chat (without \" \")");
            
            SetupData d = new SetupData();
            d.s = Step.GetCenter;
            d.current = p.level;
            d.middle = p.level.Height / 2;
            cache.Add(p, d);
        }
    }
    
    class SetupData {
        public Step s;
        public Level current;
        public int middle = 0;
        public int bx, by, bz;
        public int rx, ry, rz;
        public ExtBlock blue, red;
        public int bluex, bluey, bluez;
    }
    
    enum Step {
        GetCenter,
        GetBlueFlag,
        GetRedFlag,
        RandomorSet,
        BlueSetSpawn,
        RedSetSpawn
    }
}
