/*
    Copyright 2011 MCForge
        
    Dual-licensed under the Educational Community License, Version 2.0 and
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

namespace MCGalaxy.Commands.World {    
    public sealed class CmdImport : Command {       
        public override string name { get { return "import"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.World; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }

        public override void Use(Player p, string message) {
            if (message == "") { Help(p); return; }
            if (!Formatter.ValidName(p, message, "level")) return;
            
            string file = "extra/import/" + message;
            if (!Directory.Exists("extra/import"))
                Directory.CreateDirectory("extra/import");
            
            if (File.Exists(file + ".dat")) { Import(p, file + ".dat", message, FileType.Dat); return; }
            if (File.Exists(file + ".mcf")) { Import(p, file + ".mcf", message, FileType.Mcf); return; }
            if (File.Exists(file + ".fcm")) { Import(p, file + ".fcm", message, FileType.Fcm); return; }
            if (File.Exists(file + ".cw")) { Import(p, file + ".cw", message, FileType.Cw); return; }
            Player.Message(p, "No .dat, .mcf, .fcm or .cw file with that name was found in /extra/import folder.");
        }
        
        void Import(Player p, string fileName, string message, FileType type) {
            using (FileStream fs = File.OpenRead(fileName)) {
                try {
                    Level lvl = null;
                    if (type == FileType.Mcf) lvl = McfFile.Load(fs, message);
                    else if (type == FileType.Fcm) lvl = FcmFile.Load(fs, message);
                    else if (type == FileType.Cw) lvl = CwFile.Load(fs, message);
                    else lvl = DatFile.Load(fs, message);
                    
                    try {
                        lvl.Save(true);
                    } finally {
                        lvl.Dispose();
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                    }
                } catch (Exception ex) {
                    Server.ErrorLog(ex);
                    Player.Message(p, "The map conversion failed."); return;
                }
                Player.Message(p, "Converted map!");
                Command.all.Find("load").Use(p, message);
            }
        }
        
        enum FileType { Mcf, Fcm, Dat, Cw };
        
        public override void Help(Player p) {
            Player.Message(p, "%T/import [name]");
            Player.Message(p, "%HImports the .dat, .mcf, .fcm or .cw file with that name.");
            Player.Message(p, "%HNote this command only loads files from the /extra/import/ folder");
        }
    }
}
