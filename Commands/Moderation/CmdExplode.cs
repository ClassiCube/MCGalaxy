/*
    Written by Jack1312
  
    Copyright 2011 MCGalaxy
        
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
    public sealed class CmdExplode : Command
    {
        public override string name { get { return "explode"; } }
        public override string shortcut { get { return "ex"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdExplode() { }

        public override void Use(Player p, string message) {
            if (message == "") { Help(p); return; }
            string[] args = message.Split(' ');
            if (!(args.Length == 1 || args.Length == 3)) { Help(p); return; }
            if (message == "me" && p != null) args[0] = p.name;
            
            ushort x, y, z;
            if (args.Length == 1) {
                Player who = Player.Find(message);
                if (who == null || who.hidden) {
                    Player.SendMessage(p, "The specified player does not exist!"); return;
                }                
                if (who.level.physics < 3 || who.level.physics == 5) {
                    Player.SendMessage(p, "The physics on the player's level are not sufficient for exploding."); return;
                }
                
                x = (ushort)(who.pos[0] / 32);
                y = (ushort)(who.pos[1] / 32);
                z = (ushort)(who.pos[2] / 32);
                who.level.MakeExplosion(x, y, z, 1);
                Player.SendMessage(p, who.color + who.DisplayName + Server.DefaultColor + " has been exploded!");                
            } else if (args.Length == 3) {
                try {
                    x = Convert.ToUInt16(args[0]);
                    y = Convert.ToUInt16(args[1]);
                    z = Convert.ToUInt16(args[2]);
                } catch {
                    Player.SendMessage(p, "Invalid parameters"); return;
                }

                Level level = p.level;
                if (y >= p.level.Height) y = (ushort)(p.level.Height - 1);

                if (p.level.physics < 3 || p.level.physics == 5) {
                    Player.SendMessage(p, "The physics on this level are not sufficient for exploding!"); return;
                }
                p.level.MakeExplosion(x, y, z, 1);
                Player.SendMessage(p, "An explosion was made at (" + x + ", " + y + ", " + z + ").");
            }
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "/explode - Satisfying all your exploding needs :)");
            Player.SendMessage(p, "/explode me - Explodes at your location");
            Player.SendMessage(p, "/explode [Player] - Explode the specified player");
            Player.SendMessage(p, "/explode [X] [Y] [Z] - Explode at the specified co-ordinates");
        }
    }
}
