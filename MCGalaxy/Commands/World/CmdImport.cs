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
    public sealed class CmdImport : Command2 {
        public override string name { get { return "Import"; } }
        public override string type { get { return CommandTypes.World; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }

        public override void Use(Player p, string message, CommandData data) {
            if (message.Length == 0) { Help(p); return; }
            if (!Formatter.ValidMapName(p, message)) return;
            
            if (!Directory.Exists(Paths.ImportsDir)) {
                Directory.CreateDirectory(Paths.ImportsDir);
            }
            
            if (message.CaselessEq("all")) {
                string[] maps = Directory.GetFiles(Paths.ImportsDir);
                foreach (string map in maps) {
                    Import(p, Path.GetFileName(map));
                }
            } else {
                Import(p, message);
            }
        }
        
        static void Import(Player p, string map) {
            string path = Paths.ImportsDir + map;
            IMapImporter importer = IMapImporter.Find(ref path);
            
            if (importer == null) {
                string formats = IMapImporter.Formats.Join(imp => imp.Extension);
                p.Message("%WNo {0} file with that name was found in /extra/import folder.", formats);
            }
            
            if (LevelInfo.MapExists(map)) {
                p.Message("%WMap {0} already exists. Rename the file to something else before importing", 
            	          Path.GetFileNameWithoutExtension(map));
                return;
            }
            try {
                Level lvl = importer.Read(path, map, true);
                try {
                    lvl.Save(true);
                } finally {
                    lvl.Dispose();
                    Server.DoGC();
                }
            } catch (Exception ex) {
                Logger.LogError("Error importing map", ex);
                p.Message("%WImporting map {0} failed. See error logs.", map);
                return;
            }
            p.Message("Successfully imported map {0}!", map);
        }
        
        public override void Help(Player p) {
            p.Message("%T/Import all");
            p.Message("%HImports every map in /extra/import/ folder");
            p.Message("%T/Import [name]");
            p.Message("%HImports a map file with that name.");
            p.Message("  %HNote: Only loads maps from the /extra/import/ folder");
            p.Message("%HSee %T/Help Import formats %Hfor supported formats");
            
        }
        
        public override void Help(Player p, string message) {
            if (message.CaselessEq("formats")) {
                p.Message("%HSupported formats:");
                foreach (IMapImporter format in IMapImporter.Formats) {
                    p.Message("  {0} ({1})", format.Extension, format.Description);
                }
            } else {
                base.Help(p, message);
            }
        }
    }
}
