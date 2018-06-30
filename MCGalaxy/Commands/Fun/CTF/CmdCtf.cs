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
using MCGalaxy.Games;
using MCGalaxy.Maths;
using BlockID = System.UInt16;

namespace MCGalaxy.Commands.Fun {
    public sealed class CmdCTF : RoundsGameCmd {
        public override string name { get { return "CTF"; } }
        public override string shortcut { get { return "CTFSetup"; } }
        protected override RoundsGame Game { get { return Server.ctf; } }
        public override CommandPerm[] ExtraPerms {
            get { return new[] { new CommandPerm(LevelPermission.Operator, "can manage CTF") }; }
        }

        // TODO: avoid code duplication with CTFLevelPicker
        static List<string> GetCtfMaps() {
            if (!File.Exists("CTF/maps.config")) return new List<string>();
            string[] lines = File.ReadAllLines("CTF/maps.config");
            return new List<string>(lines);
        }
        
        protected override void HandleSetCore(Player p, RoundsGame game, string[] args) {
            string prop = args[1];
            CTFMapConfig cfg = RetrieveConfig(p);
            
            if (prop.CaselessEq("bluespawn")) {
                cfg.BlueSpawn = (Vec3U16)p.Pos.FeetBlockCoords;
                Player.Message(p, "Set spawn of blue team to &b" + cfg.BlueSpawn);
                UpdateConfig(p, cfg);
            } else if (prop.CaselessEq("redspawn")) {
                cfg.RedSpawn = (Vec3U16)p.Pos.FeetBlockCoords;
                Player.Message(p, "Set spawn of red team to &b" + cfg.RedSpawn);
                UpdateConfig(p, cfg);
            } else if (prop.CaselessEq("blueflag")) {
                Player.Message(p, "Place or delete a block to set blue team's flag.");
                p.MakeSelection(1, null, BlueFlagCallback);
            } else if (prop.CaselessEq("redflag")) {
                Player.Message(p, "Place or delete a block to set red team's flag.");
                p.MakeSelection(1, null, RedFlagCallback);
            } else if (prop.CaselessEq("divider")) {
                cfg.ZDivider = p.Pos.BlockZ;
                Player.Message(p, "Set Z line divider to {0}.", cfg.ZDivider);
                UpdateConfig(p, cfg);
            } else {
                Help(p, "set");
            }
        }

        static void HandleAdd(Player p, Level lvl) {
            if (!Directory.Exists("CTF")) Directory.CreateDirectory("CTF");
            List<string> maps = GetCtfMaps();
            
            if (maps.CaselessContains(lvl.name)) {
                Player.Message(p, "{0} %Sis already in the list of CTF maps", lvl.ColoredName);
            } else {
                Player.Message(p, "Added {0} %Sto the list of CTF maps", lvl.ColoredName);
                maps.Add(p.level.name);
                File.WriteAllLines("CTF/maps.config", maps.ToArray());
            }
        }
        
        static void HandleRemove(Player p, Level lvl) {
            if (!Directory.Exists("CTF")) Directory.CreateDirectory("CTF");
            List<string> maps = GetCtfMaps();
            
            if (!maps.CaselessRemove(lvl.name)) {
                Player.Message(p, "{0} %Swas not in the list of CTF maps", lvl.ColoredName);
            } else {
                Player.Message(p, "Removed {0} %Sfrom the list of CTF maps", lvl.ColoredName);
                File.WriteAllLines("CTF/maps.config", maps.ToArray());
            }
        }
        
        static bool BlueFlagCallback(Player p, Vec3S32[] marks, object state, BlockID block) {
            CTFMapConfig cfg = RetrieveConfig(p);
            Vec3U16 P = (Vec3U16)marks[0];
            cfg.BlueFlagPos = P;
            Player.Message(p, "Set flag position of blue team to ({0})", P);
            
            block = p.level.GetBlock(P.X, P.Y, P.Z);
            if (block == Block.Air) block = Block.Blue;
            cfg.BlueFlagBlock = block;
            Player.Message(p, "Set flag block of blue team to {0}", Block.GetName(p, block));
            
            UpdateConfig(p, cfg);
            return false;
        }
        
        static bool RedFlagCallback(Player p, Vec3S32[] marks, object state, BlockID block) {
            CTFMapConfig cfg = RetrieveConfig(p);
            Vec3U16 P = (Vec3U16)marks[0];         
            cfg.RedFlagPos = P;
            Player.Message(p, "Set flag position of red team to ({0})", P);
            
            block = p.level.GetBlock(P.X, P.Y, P.Z);
            if (block == Block.Air) block = Block.Red;
            cfg.RedFlagBlock = block;
            Player.Message(p, "Set flag block of red team to {0}", Block.GetName(p, block));
            
            UpdateConfig(p, cfg);
            return false;
        }
        
        static CTFMapConfig RetrieveConfig(Player p) {
            CTFMapConfig cfg = new CTFMapConfig();
            cfg.SetDefaults(p.level);
            cfg.Load(p.level.name);
            return cfg;
        }
        
        static void UpdateConfig(Player p, CTFMapConfig cfg) {
            if (!Directory.Exists("CTF")) Directory.CreateDirectory("CTF");
            cfg.Save(p.level.name);
            if (p.level == Server.ctf.Map) Server.ctf.UpdateMapConfig();
        }
        
        public override void Help(Player p, string message) {
            if (message.CaselessEq("set")) {
                Player.Message(p, "%T/CTF set redspawn/bluespawn");
                Player.Message(p, "%HSets spawn of red/blue team to your position.");
                Player.Message(p, "%T/CTF set redflag/blueflag");
                Player.Message(p, "%HSets flag position and block of red/blue team to the next block you place or delete.");
                Player.Message(p, "%T/CTF set divider");
                Player.Message(p, "%HSets the divider line to your current Z position.");
                Player.Message(p, "   %HRed team tags blue team when the Z position is less than the divider, " +
                               "blue teams tags when Z position is more.");
            } else {
                Help(p);
            }
        }
                
        public override void Help(Player p) {
            Player.Message(p, "%T/CTF start <map> %H- Starts CTF game");
            Player.Message(p, "%T/CTF stop %H- Stops CTF game");
            Player.Message(p, "%T/CTF end %H- Ends current round of CTF");
            Player.Message(p, "%T/CTF set add/remove %H- Adds/removes current map from map list");
            Player.Message(p, "%T/CTF set [property] %H- Sets a property. See %T/Help CTF set");
            Player.Message(p, "%T/CTF status %H- View stats of both teams");
            Player.Message(p, "%T/CTF go %H- Moves you to the current CTF map");
        }
    }
}
