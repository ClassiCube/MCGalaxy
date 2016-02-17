/*
    Copyright 2011 MCGalaxy
        
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
using MCGalaxy.Levels.IO;

namespace MCGalaxy.Commands {
    
    public sealed class CmdImport : Command {
        
        public override string name { get { return "import"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.World; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }

        public override void Use(Player p, string message) {
            if (message == "") { Help(p); return; }
            string file = "extra/import/" + message;
            if (!Directory.Exists("extra/import"))
                Directory.CreateDirectory("extra/import");
            
            if (File.Exists(file + ".dat")) { Import(p, file + ".dat", message, false); return; }
            if (File.Exists(file + ".mcf")) { Import(p, file + ".mcf", message, true); return; }
            Player.SendMessage(p, "No .dat or .mcf file with the given name was found in the imports directory.");
        }
        
        void Import(Player p, string fileName, string message, bool mcf) {
            using (FileStream fs = File.OpenRead(fileName)) {
                try {
                    if (mcf) McfFile.Load(fs, message);
                    else DatFile.Load(fs, message);
                } catch (Exception ex) {
                    Server.ErrorLog(ex);
                    Player.SendMessage(p, "The map conversion failed."); return;
                }
                Player.SendMessage(p, "Converted map!");
                Command.all.Find("load").Use(p, message);
            }
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "%T/import [name]");
            Player.SendMessage(p, "%HImports the .dat or .mcf file with the given name.");
            Player.SendMessage(p, "%HNote this command only loads .dat/.mcf files from the /extra/import/ folder");
        }
    }
}
