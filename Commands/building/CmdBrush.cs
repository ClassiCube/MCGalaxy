/*
    Copyright 2015 MCGalaxy
        
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
using MCGalaxy;
using MCGalaxy.Drawing.Brushes;
 
namespace MCGalaxy.Commands {
    
    public sealed class CmdBrush : Command {
        public override string name { get { return "brush"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }

        public override void Use(Player p, string message) {
            if (message == "") { Help(p); return; }
            if (p == null) { MessageInGameOnly(p); return; }
            
            foreach (var brush in Brush.Brushes) {
            	if (brush.Key.Equals(message, StringComparison.OrdinalIgnoreCase)) {
                    Player.SendMessage(p, "Set your brush to: " + brush.Key);
                    p.BrushName = brush.Key;
                    return;
                }
            }
            Player.SendMessage(p, "No brush found with name \"" + message + "\".");
            Player.SendMessage(p, "Available brushes: " + AvailableBrushes);
        }
        
        static string AvailableBrushes {
            get { return string.Join( ", ", Brush.Brushes.Keys); }
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "/brush <name> - Sets the currently active brush to the given name.");
            Player.SendMessage(p, "Available brushes: " + AvailableBrushes);
        }
    }
}
