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
using MCGalaxy.Games;
using MCGalaxy.Maths;
using BlockID = System.UInt16;

namespace MCGalaxy.Commands.Fun {
    public sealed class CmdCTF : RoundsGameCmd {
        public override string name { get { return "CTF"; } }
        public override string shortcut { get { return "CTFSetup"; } }
        protected override RoundsGame Game { get { return CTFGame.Instance; } }
        public override CommandPerm[] ExtraPerms {
            get { return new[] { new CommandPerm(LevelPermission.Operator, "can manage CTF") }; }
        }
        
        protected override void HandleSet(Player p, RoundsGame game, string[] args) {
            string prop = args[1];
            CTFMapConfig cfg = new CTFMapConfig();
            LoadMapConfig(p, cfg);
            
            if (prop.CaselessEq("bluespawn")) {
                cfg.BlueSpawn = (Vec3U16)p.Pos.FeetBlockCoords;
                p.Message("Set spawn of blue team to &b" + cfg.BlueSpawn);
                SaveMapConfig(p, cfg);
            } else if (prop.CaselessEq("redspawn")) {
                cfg.RedSpawn = (Vec3U16)p.Pos.FeetBlockCoords;
                p.Message("Set spawn of red team to &b" + cfg.RedSpawn);
                SaveMapConfig(p, cfg);
            } else if (prop.CaselessEq("blueflag")) {
                p.Message("Place or delete a block to set blue team's flag.");
                p.MakeSelection(1, cfg, BlueFlagCallback);
            } else if (prop.CaselessEq("redflag")) {
                p.Message("Place or delete a block to set red team's flag.");
                p.MakeSelection(1, cfg, RedFlagCallback);
            } else if (prop.CaselessEq("divider")) {
                cfg.ZDivider = p.Pos.BlockZ;
                p.Message("Set Z line divider to {0}.", cfg.ZDivider);
                SaveMapConfig(p, cfg);
            } else {
                Help(p, "set");
            }
        }
        
        bool BlueFlagCallback(Player p, Vec3S32[] marks, object state, BlockID block) {
            CTFMapConfig cfg = (CTFMapConfig)state;
            Vec3U16 P = (Vec3U16)marks[0];
            cfg.BlueFlagPos = P;
            p.Message("Set flag position of blue team to ({0})", P);
            
            block = p.level.GetBlock(P.X, P.Y, P.Z);
            if (block == Block.Air) block = Block.Blue;
            cfg.BlueFlagBlock = block;
            p.Message("Set flag block of blue team to {0}", Block.GetName(p, block));
            
            SaveMapConfig(p, cfg);
            return false;
        }
        
        bool RedFlagCallback(Player p, Vec3S32[] marks, object state, BlockID block) {
            CTFMapConfig cfg = (CTFMapConfig)state;
            Vec3U16 P = (Vec3U16)marks[0];         
            cfg.RedFlagPos = P;
            p.Message("Set flag position of red team to ({0})", P);
            
            block = p.level.GetBlock(P.X, P.Y, P.Z);
            if (block == Block.Air) block = Block.Red;
            cfg.RedFlagBlock = block;
            p.Message("Set flag block of red team to {0}", Block.GetName(p, block));
            
            SaveMapConfig(p, cfg);
            return false;
        }
        
        static void UpdateConfig(Player p, CTFMapConfig cfg) {
            cfg.Save(p.level.name);
            if (p.level == CTFGame.Instance.Map) CTFGame.Instance.UpdateMapConfig();
        }
        
        public override void Help(Player p, string message) {
            if (message.CaselessEq("set")) {
                p.Message("&T/CTF set redspawn/bluespawn");
                p.Message("&HSets spawn of red/blue team to your position.");
                p.Message("&T/CTF set redflag/blueflag");
                p.Message("&HSets flag position and block of red/blue team to the next block you place or delete.");
                p.Message("&T/CTF set divider");
                p.Message("&HSets the divider line to your current Z position.");
                p.Message("   &HRed team tags blue team when the Z position is less than the divider, " +
                               "blue teams tags when Z position is more.");
            } else {
                Help(p);
            }
        }
                
        public override void Help(Player p) {
            p.Message("&T/CTF start <map> &H- Starts CTF game");
            p.Message("&T/CTF stop &H- Stops CTF game");
            p.Message("&T/CTF end &H- Ends current round of CTF");
            p.Message("&T/CTF add/remove &H- Adds/removes current map from map list");
            p.Message("&T/CTF set [property] &H- Sets a property. See &T/Help CTF set");
            p.Message("&T/CTF status &H- View stats of both teams");
            p.Message("&T/CTF go &H- Moves you to the current CTF map");
        }
    }
}
