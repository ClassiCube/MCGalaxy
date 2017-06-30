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
using System.IO;
using MCGalaxy.Games;

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
                if (Server.ctf == null)  {
                    Player.Message(p, "Initialising CTF..");
                    Server.ctf = new CTFGame();
                }
                
                if (!Server.ctf.Start(p)) return;
                Chat.MessageGlobal("A CTF GAME IS STARTING AT CTF! TYPE /goto CTF to join!");
            } else if (message.CaselessEq("stop"))  {
                if (Server.ctf == null || !Server.ctf.started) {
                    Player.Message(p, "No CTF game is active."); return;
                }
                Server.ctf.Stop();
            } else if (message.CaselessEq("bluespawn")) {
                CTFConfig cfg = Retrieve(p);
                cfg.BlueSpawnX = p.Pos.X; cfg.BlueSpawnY = p.Pos.Y; cfg.BlueSpawnZ = p.Pos.Z;
                
                Update(p, cfg);
                Player.Message(p, "Set spawn of blue team to your position.");
            } else if (message.CaselessEq("redspawn")) {
                CTFConfig cfg = Retrieve(p);
                cfg.RedSpawnX = p.Pos.X; cfg.RedSpawnY = p.Pos.Y; cfg.RedSpawnZ = p.Pos.Z;
                
                Update(p, cfg);
                Player.Message(p, "Set spawn of red team to your position.");
            }
        }
        
        static CTFConfig Retrieve(Player p) {
            CTFConfig cfg = new CTFConfig();
            cfg.SetDefaults(p.level);
            cfg.Retrieve(p.level.name);
            return cfg;
        }
        
        static void Update(Player p, CTFConfig cfg) {
            if (!Directory.Exists("CTF")) Directory.CreateDirectory("CTF");
            cfg.Save(p.level.name);
            if (Server.ctf != null && p.level == Server.ctf.map) Server.ctf.UpdateConfig();
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/ctf start/stop");
            Player.Message(p, "%HStarts/stops the CTF game.");
            Player.Message(p, "%T/ctf redspawn/bluespawn");
            Player.Message(p, "%HSets spawn of red/blue team to your position.");
        }
    }
}
