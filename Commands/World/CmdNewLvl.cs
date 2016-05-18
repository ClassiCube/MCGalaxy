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

namespace MCGalaxy.Commands
{
    public sealed class CmdNewLvl : Command
    {
        public override string name { get { return "newlvl"; } }
        public override string shortcut { get { return "gen"; } }
        public override string type { get { return CommandTypes.World; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        public CmdNewLvl() { }

        public override void Use(Player p, string message) {
            string[] args = message.Split(' ');
            if (args.Length < 5 || args.Length > 6) {
                Help(p); return;
            }
            if (!MapGen.IsRecognisedFormat(args[4])) {
                MapGen.PrintValidFormats(p); return;
            }

            ushort x, y, z;
            string name = args[0].ToLower();
            if (!UInt16.TryParse(args[1], out x) || !UInt16.TryParse(args[2], out y) || !UInt16.TryParse(args[3], out z)) {
                Player.Message(p, "Invalid dimensions."); return;
            }
            
            int seed = 0; 
            bool useSeed = args.Length == 6;
            if (useSeed && !Int32.TryParse(args[5], out seed))
                seed = args[5].GetHashCode();
            if (!MapGen.OkayAxis(x)) { Player.Message(p, "width must divisible by 16, and >= 16"); return; }
            if (!MapGen.OkayAxis(y)) { Player.Message(p, "height must be divisible by 16, and >= 16"); return; }
            if (!MapGen.OkayAxis(z)) { Player.Message(p, "length must be divisible by 16, and >= to 16."); return; }

            if (!Player.ValidName(name)) { 
                Player.Message(p, "Invalid name!"); return; 
            }
            if (LevelInfo.ExistsOffline(name)) {
                Player.Message(p, "Level \"" + name + "\" already exists!"); return;
            }

            if (p != null) {
                int limit = p.group.Permission < LevelPermission.Admin ? 
                    Server.MapGenLimit : Server.MapGenLimitAdmin;
                if ((long)x * y * z > limit ) {
                    string text = "You cannot create a map with over ";
                    if (limit > 1000 * 1000) text += (limit / (1000 * 1000)) + " million blocks";
                    else if (limit > 1000) text += (limit / 1000) + " thousand blocks";
                    else text += limit + " blocks";
                    Player.Message(p, text); return;
                }
            }

            try {
                using (Level lvl = new Level(name, x, y, z, args[4], seed, useSeed)) {
                    Level.CreateLeveldb(name);
                    lvl.Save(true);
                    lvl.Dispose();
                }
            } finally {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            
            string format = useSeed ? "Level \"{0}\" created with seed \"{1}\"" : "Level \"{0}\" created";
            if (useSeed)
               Player.GlobalMessage(String.Format(format, name, args[5]));
            else
                Player.GlobalMessage(String.Format(format, name));
        }
        
        public override void Help(Player p) {
            Player.Message(p, "/newlvl - creates a new level.");
            Player.Message(p, "/newlvl mapname 128 64 128 type seed");
            Player.Message(p, "Valid sizes: Must be >= 16 and <= 8192, and divisible by 16.");
            Player.Message(p, "Note due to limitations, other players don't show past 1024.");
            MapGen.PrintValidFormats(p);
            Player.Message(p, "The seed is optional, and controls how the level is generated.");
            Player.Message(p, "If the seed is the same, the generated level will be the same.");
            Player.Message(p, "For flat maps the seed (if given) is used for the grass level.");
        }
    }
}
