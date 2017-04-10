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
        public override string type { get { return CommandTypes.World; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }

        public override void Use(Player p, string message) {
            if (message == "") { Help(p); return; }
            if (!Formatter.ValidName(p, message, "level")) return;
            
            string file = "extra/import/" + message;
            if (!Directory.Exists("extra/import"))
                Directory.CreateDirectory("extra/import");
            
            foreach (IMapImporter importer in IMapImporter.Formats) {
                if (!File.Exists(file + importer.Extension)) continue;
                Import(p, file, message, importer);
                return;
            }

            string formats = IMapImporter.Formats.Join(imp => imp.Extension);
            Player.Message(p, "&cNo {0} file with that name was found in /extra/import folder.", formats);
        }
        
        void Import(Player p, string path, string name, IMapImporter importer) {
            try {
                Level lvl = importer.Read(path + importer.Extension, name, true);
                try {
                    lvl.Save(true);
                } finally {
                    lvl.Dispose();
                    Server.DoGC();
                }
            } catch (Exception ex) {
                Server.ErrorLog(ex);
                Player.Message(p, "The map conversion failed."); 
                return;
            }
            Player.Message(p, "Converted map!");
        }
        
        enum FileType { Mcf, Fcm, Dat, Cw };
        
        public override void Help(Player p) {
            Player.Message(p, "%T/import [name]");
            Player.Message(p, "%HImports a map file with that name.");
            Player.Message(p, "%HSupported formats: %S{0}",
                           IMapImporter.Formats.Join(imp => imp.Extension));
            Player.Message(p, "  %HNote: Only loads maps from the /extra/import/ folder");
        }
    }
}
