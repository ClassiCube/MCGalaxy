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
using System.IO.Compression;
using System.Net;
using System.Threading;
using MCGalaxy.Levels.IO;

namespace MCGalaxy.Commands
{
    public sealed class CmdMuseum : Command
    {
        public override string name { get { return "museum"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.World; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public CmdMuseum() { }

        public override void Use(Player p, string message) {
            string[] args = message.Split(' ');
            string path = args.Length == 1 ? LevelInfo.LevelPath(args[0]) :
                LevelInfo.BackupPath(args[0], args[1]);
            Server.s.Log(path);
            
            if (!File.Exists(path)) {
                Player.Message(p, "Level or backup could not be found."); return;
            }
            
            Level lvl = LvlFile.Load(name, path);
            lvl.setPhysics(0);
            lvl.backedup = true;
            lvl.permissionbuild = LevelPermission.Admin;

            lvl.jailx = (ushort)(lvl.spawnx * 32);
            lvl.jaily = (ushort)(lvl.spawny * 32);
            lvl.jailz = (ushort)(lvl.spawnz * 32);
            lvl.jailrotx = lvl.rotx; lvl.jailroty = lvl.roty;

            p.Loading = true;
            Entities.DespawnEntities(p);
            Level oldLevel = p.level;
            p.level = lvl;
            p.SendMotd();
            if (!p.SendRawMap(oldLevel, lvl)) return;

            ushort x = (ushort)((0.5 + lvl.spawnx) * 32);
            ushort y = (ushort)((1 + lvl.spawny) * 32);
            ushort z = (ushort)((0.5 + lvl.spawnz) * 32);

            p.aiming = false;
            Entities.GlobalSpawn(p, x, y, z, lvl.rotx, lvl.roty, true);
            p.ClearBlockchange();
            p.Loading = false;

            if (args.Length == 1)
                lvl.name = "&cMuseum " + Server.DefaultColor + "(" + args[0] + ")";
            else
                lvl.name = "&cMuseum " + Server.DefaultColor + "(" + args[0] + " " + args[1] + ")";

            if (!p.hidden)
                Player.GlobalMessage(p.ColoredName + " %Swent to the " + lvl.name);
        }
        
        public override void Help(Player p) {
            Player.Message(p, "/museum <map> <restore> - Allows you to access a restore of the map entered.");
            Player.Message(p, "Works on offline maps");
        }
    }
}
