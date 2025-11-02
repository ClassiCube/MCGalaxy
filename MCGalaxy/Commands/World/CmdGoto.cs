/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCForge)
    
    Dual-licensed under the Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    https://opensource.org/license/ecl-2-0/
    https://www.gnu.org/licenses/gpl-3.0.html
    
    Unless required by applicable law or agreed to in writing,
    software distributed under the Licenses are distributed on an "AS IS"
    BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
    or implied. See the Licenses for the specific language governing
    permissions and limitations under the Licenses.
 */
using System;
using System.Collections.Generic;
using System.IO;
using MCGalaxy.Commands.Info;

namespace MCGalaxy.Commands.World {
    public sealed class CmdGoto : Command2 {
        public override string name { get { return "Goto"; } }
        public override string shortcut { get { return "g"; } }
        public override string type { get { return CommandTypes.World; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("j"), new CommandAlias("Join"), new CommandAlias("gr", "-random"),
                    new CommandAlias("GotoRandom", "-random"), new CommandAlias("JoinRandom", "-random") }; }
        }
        public override bool SuperUseable { get { return false; } }
        public override CommandParallelism Parallelism { get { return CommandParallelism.NoAndWarn; } }

        public override void Use(Player p, string message, CommandData data) {
            if (message.Length == 0) { Help(p); return; }
            
            if (message.CaselessStarts("-random")) {
                Random r = new Random();
                string[] files = LevelInfo.AllMapFiles();
                string[] args = message.SplitSpaces(2);

                int attempts = 0;
                GrResult res;
                do {
                    attempts++;
                    res = TryGotoRandom(p, r, files, args);
                } while (attempts < 5 && res == GrResult.NoPermission);

                if (res == GrResult.NoPermission) {
                    p.Message("&WTook too long to find a random map to go to. Giving up.");
                }

            } else if (Formatter.ValidMapName(p, message)) {
                PlayerActions.ChangeMap(p, message);
            }
        }

        enum GrResult { NoLevels, NoPermission, Success }
        static GrResult TryGotoRandom(Player p, Random r, string[] files, string[] args) {
            string map;

            // randomly visit a specified subset of all levels
            if (args.Length > 1) {
                List<string> maps = Wildcard.Filter(files, args[1],
                                                    mapFile => Path.GetFileNameWithoutExtension(mapFile));
                if (maps.Count == 0) {
                    p.Message("No maps found containing \"{0}\"", args[1]);
                    return GrResult.NoLevels;
                }
                map = maps[r.Next(maps.Count)];
            } else {
                map = files[r.Next(files.Length)];
                map = Path.GetFileNameWithoutExtension(map);
            }
            if (p.level.name == map) {
                // try again silently
                return GrResult.NoPermission;
            }

            bool changed = PlayerActions.ChangeMap(p, map);
            if (changed) return GrResult.Success;
            return GrResult.NoPermission;
        }
        
        public override void Help(Player p) {
            p.Message("&T/Goto [map name]");
            p.Message("&HTeleports yourself to a different level.");
            p.Message("&T/Goto -random");
            p.Message("&HTeleports yourself to a random level.");
        }
    }
}
