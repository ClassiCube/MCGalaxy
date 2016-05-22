/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
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
namespace MCGalaxy.Commands.Building {    
    public sealed class CmdClick : Command {        
        public override string name { get { return "click"; } }
        public override string shortcut { get { return "x"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public CmdClick() { }

        public override void Use(Player p, string message) {
            if (p == null) { MessageInGameOnly(p); return; }
            Vec3U16 click = p.lastClick;
            ushort value;

            if (message.IndexOf(' ') != -1) {
                string[] args = message.ToLower().Split(' ');
                if (args.Length != 3) { Help(p); return; }
                
                for (int i = 0; i < 3; i++) {
                    if (args[i] == "x" || args[i] == "y" || args[i] == "z") {
                       // use the last value
                	} else if (ushort.TryParse(args[i], out value)) {
                        if (i == 0) click.X = value;
                        else if (i == 1) click.Y = value;
                        else click.Z = value;
                    } else {
                        Player.Message(p, "\"" + args[i] + "\" was not valid"); return;
                    }
                }
            }
            
            click = Vec3U16.ClampToBounds(click.X, click.Y, click.Z, p.level);
            p.ManualChange(click.X, click.Y, click.Z, 0, Block.rock);
            Player.Message(p, "Clicked &b(" + click.X + ", " + click.Y + ", " + click.Z + ")");
        }
        
        public override void Help(Player p) {
            Player.Message(p, "/click [x y z] - Fakes a click");
            Player.Message(p, "If no xyz is given, it uses the last place clicked");
            Player.Message(p, "/click 200 y 200 will cause it to click at 200x, last y and 200z");
        }
    }
}
