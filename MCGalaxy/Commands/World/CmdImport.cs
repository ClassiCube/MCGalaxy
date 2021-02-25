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
using MCGalaxy.Network;

namespace MCGalaxy.Commands.World {
    public sealed class CmdImport : Command2 {
        public override string name { get { return "Import"; } }
        public override string type { get { return CommandTypes.World; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }

        public override void Use(Player p, string message, CommandData data) {
            if (message.Length == 0) { Help(p); return; }
            if (!Directory.Exists(Paths.ImportsDir)) {
                Directory.CreateDirectory(Paths.ImportsDir);
            }
            
            if (message.CaselessEq("all")) {
                string[] maps = Directory.GetFiles(Paths.ImportsDir);
                foreach (string map in maps) { ImportFile(p, map); }
            } else if (message.IndexOf('/') >= 0) {
                ImportWeb(p, message);
            } else {
                if (!Formatter.ValidMapName(p, message)) return;
                ImportFile(p, message);
            }
        }
        
        static void ImportWeb(Player p, string url) {
            HttpUtil.FilterURL(ref url);
            byte[] data = HttpUtil.DownloadData(url, p);
            if (data == null) return;
            
            // if data is not NULL, URL must be valid
            string path = new Uri(url).AbsolutePath;
            string map  = Path.GetFileNameWithoutExtension(path);
            if (!Formatter.ValidMapName(p, map)) return;
            
            foreach (IMapImporter imp in IMapImporter.Formats) {
                if (!path.CaselessEnds(imp.Extension)) continue;
                
                using (Stream src = new MemoryStream(data)) {
                    Import(p, imp, src, map); return;
                }
            }
            
            string formats = IMapImporter.Formats.Join(imp => imp.Extension);
            p.Message("&WOnly {0} formats are supported.", formats);
        }

        static void ImportFile(Player p, string map) {
            map = Path.GetFileNameWithoutExtension(map);
            string path = Paths.ImportsDir + map;
            
            foreach (IMapImporter imp in IMapImporter.Formats) {
                path = Path.ChangeExtension(path, imp.Extension);
                if (!File.Exists(path)) continue;
                
                using (Stream src = File.OpenRead(path)) {
                    Import(p, imp, src, map); return;
                }
            }
            
            string formats = IMapImporter.Formats.Join(imp => imp.Extension);
            p.Message("&WNo {0} file with that name was found in /extra/import folder.", formats);
        }
        
        static void Import(Player p, IMapImporter importer, Stream src, string map) {
            if (LevelInfo.MapExists(map)) {
                p.Message("&WMap {0} already exists. Rename the file to something else before importing",
                          Path.GetFileNameWithoutExtension(map));
                return;
            }
            
            try {
                Level lvl = importer.Read(src, map, true);
                try {
                    lvl.Save(true);
                } finally {
                    lvl.Dispose();
                    Server.DoGC();
                }
            } catch (Exception ex) {
                Logger.LogError("Error importing map", ex);
                p.Message("&WImporting map {0} failed. See error logs.", map);
                return;
            }
            p.Message("Successfully imported map {0}!", map);
        }
        
        public override void Help(Player p) {
            p.Message("&T/Import all");
            p.Message("&HImports every map in /extra/import/ folder");
            p.Message("&T/Import [url/filename]");
            p.Message("&HImports a map from a webpage or the /extra/import/ folder");
            p.Message("&HSee &T/Help Import formats &Hfor supported formats");
            
        }
        
        public override void Help(Player p, string message) {
            if (message.CaselessEq("formats")) {
                p.Message("&HSupported formats:");
                foreach (IMapImporter format in IMapImporter.Formats) {
                    p.Message("  {0} ({1})", format.Extension, format.Description);
                }
            } else {
                base.Help(p, message);
            }
        }
    }
}
