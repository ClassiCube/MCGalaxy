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
namespace MCGalaxy.Commands
{
    public sealed class CmdPaint : Command
    {
        public override string name { get { return "paint"; } }
        public override string shortcut { get { return "p"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Builder; } }
        public CmdPaint() { }

        public override void Use(Player p, string message) {
            if (message != "") { Help(p); return; }
            p.painting = !p.painting;
            
            string type = p.painting ? "&aON" : "&cOFF";
            Player.Message(p, "Painting mode: " + type + "%S.");
            p.modeType = 0;
        }
        
        public override void Help(Player p) {
            Player.Message(p, "/paint - Turns painting mode on/off.");
        }
    }
}
