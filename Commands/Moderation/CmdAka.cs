/*
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
    public sealed class CmdAka : Command
    {
        public override string name { get { return "aka"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public CmdAka() { }
        
        public override void Use(Player p, string message) {
            bool showInfected = p.aka;
            p.aka = !p.aka;
            ushort x = p.pos[0], y = p.pos[1], z = p.pos[2];

            p.Loading = true;
            foreach (Player pl in Player.players)
                if (p.level == pl.level && p != pl) p.SendDespawn(pl.id);
            foreach (PlayerBot b in PlayerBot.playerbots)
                if (p.level == b.level) p.SendDespawn(b.id);

            Player.GlobalDespawn(p, true);
            p.SendUserMOTD();
            p.SendMap();

            if (!p.hidden) {
                Player.GlobalDespawn(p, false);
                Player.GlobalSpawn(p, x, y, z, p.level.rotx, p.level.roty, true);
            } else {
                p.SendPos(0xFF, x, y, z, p.level.rotx, p.level.roty);
            }
            
            foreach (Player pl in Player.players) {
                if (pl.level != p.level || p == pl || pl.hidden || pl.referee)
                    continue;
                
                string name = null;
                if (pl.infected && showInfected) {
                    name = Server.ZombieName != "" ? c.red + Server.ZombieName : c.red + pl.name;
                } else {
                    name = pl.color + pl.name;
                }
                p.SendSpawn(pl.id, name,
                            pl.pos[0], pl.pos[1], pl.pos[2], pl.rot[0], pl.rot[1]);
            }
            
            foreach (PlayerBot b in PlayerBot.playerbots)
                if (b.level == p.level)
                    p.SendSpawn(b.id, b.color + b.name, b.pos[0], b.pos[1], b.pos[2], b.rot[0], b.rot[1]);

            p.Loading = false;
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "%T/aka");
            Player.SendMessage(p, "%HToggles whether infected players show their actual names.");
        }
    }
}
