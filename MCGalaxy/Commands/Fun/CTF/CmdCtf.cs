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

namespace MCGalaxy.Commands.Fun {
    public sealed class CmdCTF : Command {
        public override string name { get { return "ctf"; } }
        public override string shortcut { get { return "ctfsetup"; } }
        public override string type { get { return CommandTypes.Games; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public override bool SuperUseable { get { return false; } }

        public override void Use(Player p, string message) {
            if (message.CaselessEq("start")) {
                HandleStart(p);
            } else if (message.CaselessEq("stop")) {
                HandleStop(p);
            } else if (message.CaselessEq("add")) {
                HandleAdd(p);
            } else if (message.CaselessEq("remove")) {
                HandleRemove(p);
            } else if (message.CaselessStarts("set ")) {
                string[] args = message.SplitSpaces(2);
                HandleSet(p, args[1]);
            } else {
                Help(p);
            }
        }
        
        static void HandleStart(Player p) {
            if (Server.ctf == null)  {
                Player.Message(p, "Initialising CTF..");
                Server.ctf = new CTFGame();
            }
            
            if (!Server.ctf.Start(p)) return;
            Chat.MessageGlobal("A CTF GAME IS STARTING AT CTF! TYPE %T/goto " + Server.ctf.Map.name + " %Sto join!");
        }
        
        static void HandleStop(Player p) {
            if (Server.ctf == null || !Server.ctf.started) {
                Player.Message(p, "No CTF game is active."); return;
            }
            Server.ctf.Stop();
        }
        
        
        static void HandleAdd(Player p) {
            if (!Directory.Exists("CTF")) Directory.CreateDirectory("CTF");
            List<string> maps = GetCtfMaps();
            
            if (maps.CaselessContains(p.level.name)) {
                Player.Message(p, "{0} %Sis already in the list of CTF maps.", p.level.ColoredName);
            } else {
                Player.Message(p, "Added {0} %Sto the list of CTF maps.", p.level.ColoredName);
                maps.Add(p.level.name);
                File.WriteAllLines("CTF/maps.config", maps.ToArray());
            }
        }
        
        static void HandleRemove(Player p) {
            if (!Directory.Exists("CTF")) Directory.CreateDirectory("CTF");
            List<string> maps = GetCtfMaps();
            
            if (!maps.CaselessRemove(p.level.name)) {
                Player.Message(p, "{0} %Swas not in the list of CTF maps.", p.level.ColoredName);
            } else {
                Player.Message(p, "Removed {0} %Sfrom the list of CTF maps.", p.level.ColoredName);
                File.WriteAllLines("CTF/maps.config", maps.ToArray());
            }
        }
        
        static List<string> GetCtfMaps() {
            if (!File.Exists("CTF/maps.config")) return new List<string>();
            string[] lines = File.ReadAllLines("CTF/maps.config");
            return new List<string>(lines);
        }

        
        void HandleSet(Player p, string property) {
            CTFConfig cfg = RetrieveConfig(p);
            
            if (property.CaselessEq("bluespawn")) {
                cfg.BlueSpawnX = p.Pos.X; cfg.BlueSpawnY = p.Pos.Y; cfg.BlueSpawnZ = p.Pos.Z;
                Player.Message(p, "Set spawn of blue team to your position.");
                UpdateConfig(p, cfg);
            } else if (property.CaselessEq("redspawn")) {
                cfg.RedSpawnX = p.Pos.X; cfg.RedSpawnY = p.Pos.Y; cfg.RedSpawnZ = p.Pos.Z;
                Player.Message(p, "Set spawn of red team to your position.");
                UpdateConfig(p, cfg);
            } else if (property.CaselessEq("blueflag")) {
                Player.Message(p, "Place or delete a block to set blue team's flag.");
                p.MakeSelection(1, null, BlueFlagCallback);
            } else if (property.CaselessEq("redflag")) {
                Player.Message(p, "Place or delete a block to set red team's flag.");
                p.MakeSelection(1, null, RedFlagCallback);
            } else {
                Help(p, "set");
            }
        }
        
        static bool BlueFlagCallback(Player p, Vec3S32[] marks, object state, ExtBlock block) {
            CTFConfig cfg = RetrieveConfig(p);
            cfg.BlueFlagX = marks[0].X; cfg.BlueFlagY = marks[0].Y; cfg.BlueFlagZ = marks[0].Z;
            UpdateConfig(p, cfg);
            return false;
        }
        
        static bool RedFlagCallback(Player p, Vec3S32[] marks, object state, ExtBlock block) {
            CTFConfig cfg = RetrieveConfig(p);
            cfg.RedFlagX = marks[0].X; cfg.RedFlagY = marks[0].Y; cfg.RedFlagZ = marks[0].Z;
            UpdateConfig(p, cfg);
            return false;
        }
        
        static CTFConfig RetrieveConfig(Player p) {
            CTFConfig cfg = new CTFConfig();
            cfg.SetDefaults(p.level);
            cfg.Retrieve(p.level.name);
            return cfg;
        }
        
        static void UpdateConfig(Player p, CTFConfig cfg) {
            if (!Directory.Exists("CTF")) Directory.CreateDirectory("CTF");
            cfg.Save(p.level.name);
            if (Server.ctf != null && p.level == Server.ctf.Map) Server.ctf.UpdateConfig();
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/ctf start/stop");
            Player.Message(p, "%HStarts/stops the CTF game.");
            Player.Message(p, "%T/ctf add/remove");
            Player.Message(p, "%HAdds or removes current map from list of CTF maps.");
            Player.Message(p, "%T/ctf set [property]");
            Player.Message(p, "%HSets a CTF game property, see %T/help ctf set");
        }
        
        public override void Help(Player p, string message) {
            if (message.CaselessEq("set")) {
                Player.Message(p, "%T/ctf set redspawn/bluespawn");
                Player.Message(p, "%HSets spawn of red/blue team to your position.");
            } else {
                Help(p);
            }
        }
    }
}
