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
namespace MCGalaxy.Commands
{
    public sealed class CmdNewLvl : Command
    {
        public override string name { get { return "newlvl"; } }
        public override string shortcut { get { return ""; } }
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

            string name = args[0].ToLower();
            if (!UInt16.TryParse(args[1], out x) || !UInt16.TryParse(args[2], out y) || !UInt16.TryParse(args[3], out z)) {
                Player.SendMessage(p, "Invalid dimensions."); return;
            }
            
            int seed = 0; 
            bool useSeed = args.Length == 6;
            if (useSeed && !Int32.TryParse(args[5], out seed))
                seed = args[5].GetHashCode();
            if (!MapGen.OkayAxis(x)) { Player.SendMessage(p, "width must be a power of two >= to 16."); return; }
            if (!MapGen.OkayAxis(y)) { Player.SendMessage(p, "height must be a power of two >= to 16."); return; }
            if (!MapGen.OkayAxis(z)) { Player.SendMessage(p, "length must be a power of two >= to 16."); return; }

            if (!Player.ValidName(name)) { 
                Player.SendMessage(p, "Invalid name!"); return; 
            }
            if (File.Exists("levels/" + name + ".lvl")) { 
                Player.SendMessage(p, "Level \"" + name + "\" already exists!"); return;
            }

            if (p != null) {
                int limit = p.group.Permission < LevelPermission.Admin ? 30000000 : 225000000;
                if (x * y * z > limit ) {
                    string text = String.Format("Cannot create a map with over {0} million blocks",
                                                limit / 1000000);
                    Player.SendMessage(p, text); return;
                }
            }

            try {
                using (Level lvl = new Level(name, x, y, z, args[4], seed, useSeed)) {
                    lvl.Save(true);
                    lvl.Dispose();
                }
            } finally {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            
            string format = useSeed ? "Level \"{0}\" created with seed \"{1}\"" : "Level \"{0}\" created";
            if (useSeed)
                Player.GlobalMessage(String.Format(format, name));
            else
                Player.GlobalMessage(String.Format(format, name, args[5]));
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "/newlvl - creates a new level.");
            Player.SendMessage(p, "/newlvl mapname 128 64 128 type seed");
            Player.SendMessage(p, "Valid sizes: 16, 32, 64, 128, 256, 512, 1024"); //Update this to add more?
            MapGen.PrintValidFormats(p);
            Player.SendMessage(p, "The seed is optional, and controls how the level is generated.");
            Player.SendMessage(p, "If the seed is the same, the generated level will be the same.");
            Player.SendMessage(p, "The seed does not do anything on flat and pixel type maps.");
        }
    }
}
