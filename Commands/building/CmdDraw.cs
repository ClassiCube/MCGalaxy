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
    public sealed class CmdDraw : Command
    {
        public override string name { get { return "draw"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Builder; } }
        public CmdDraw() { }

        public override void Use(Player p, string message) {
            if (p == null) { MessageInGameOnly(p); return; }
            if (p.level.permissionbuild > p.group.Permission) {
                p.SendMessage("You can not edit this map."); return;
            }
            
            string[] parts = message.Split(' ');
            Player.BlockchangeEventHandler newHandler = null;
            bool help;

            switch (parts[0].ToLower()) {
                case "cone":
                    if (!CheckTwoArgs(p, 1, parts, out help)) { if (help) Help(p); return; }
                    newHandler = new Player.BlockchangeEventHandler(BlockchangeCone); break;
                case "hcone":
                    if (!CheckTwoArgs(p, 1, parts, out help)) { if (help) Help(p); return; }
                    newHandler = new Player.BlockchangeEventHandler(BlockchangeHCone); break;
                case "icone":
                    if (!CheckTwoArgs(p, 1, parts, out help)) { if (help) Help(p); return; }
                    newHandler = new Player.BlockchangeEventHandler(BlockchangeICone); break;
                case "hicone":
                    if (!CheckTwoArgs(p, 1, parts, out help)) { if (help) Help(p); return; }
                    newHandler = new Player.BlockchangeEventHandler(BlockchangeHICone); break;
                    
                case "pyramid":
                    if (!CheckTwoArgs(p, 2, parts, out help)) { if (help) Help(p); return; }
                    newHandler = new Player.BlockchangeEventHandler(BlockchangePyramid); break;
                case "hpyramid":
                    if (!CheckTwoArgs(p, 2, parts, out help)) { if (help) Help(p); return; }
                    newHandler = new Player.BlockchangeEventHandler(BlockchangeHPyramid); break;
                case "ipyramid":
                    if (!CheckTwoArgs(p, 2, parts, out help)) { if (help) Help(p); return; }
                    newHandler = new Player.BlockchangeEventHandler(BlockchangeIPyramid); break;
                case "hipyramid":
                    if (!CheckTwoArgs(p, 2, parts, out help)) { if (help) Help(p); return; }
                    newHandler = new Player.BlockchangeEventHandler(BlockchangeHIPyramid); break;

                case "sphere":
                    if (!CheckOneArg(p, 3, parts, out help)) { if (help) Help(p); return; }
                    newHandler = new Player.BlockchangeEventHandler(BlockchangeSphere); break;
                case "hsphere":
                    if (!CheckOneArg(p, 3, parts, out help)) { if (help) Help(p); return; }
                    newHandler = new Player.BlockchangeEventHandler(BlockchangeHSphere); break;
                case "volcano":
                    if (!CheckTwoArgs(p, 4, parts, out help)) { if (help) Help(p); return; }
                    newHandler = new Player.BlockchangeEventHandler(BlockchangeVolcano); break;
            }
            Player.SendMessage(p, "Place a block");
            p.ClearBlockchange();
            p.Blockchange += newHandler;        
        }
        
        bool CheckTwoArgs(Player p, int addition, string[] parts, out bool help) {
            if ((int)p.group.Permission < CommandOtherPerms.GetPerm(this, addition)) {
                Group group = Group.findPermInt(CommandOtherPerms.GetPerm(this, addition));
                Player.SendMessage(p, "That commands addition is for " + group.name + "+");
                help = false; return false;
            }
            
            help = true;
            if (parts.Length != 3)
                return false;
            ushort height, radius;
            if (!ushort.TryParse(parts[1], out height) || !ushort.TryParse(parts[2], out radius))
                return false;
            p.BcVar = new int[] { height, radius };
            return true;
        }
        
        bool CheckOneArg(Player p, int addition, string[] parts, out bool help) {
            if ((int)p.group.Permission < CommandOtherPerms.GetPerm(this, addition)) {
                Group group = Group.findPermInt(CommandOtherPerms.GetPerm(this, addition));
                Player.SendMessage(p, "That commands addition is for " + group.name + "+");
                help = false; return false;
            }
            
            help = true;
            if (parts.Length != 2)
                return false;
            ushort radius;
            if (!ushort.TryParse(parts[1], out radius))
                return false;
            p.BcVar = new int[] { 0, radius };
            return true;
        }

        public void BlockchangeCone(Player p, ushort x, ushort y, ushort z, byte type, byte extType) {
            RevertAndClearState(p, x, y, z);
            Util.SCOGenerator.Cone(p, x, y, z, p.BcVar[0], p.BcVar[1], type, extType, false);
        }
        
        public void BlockchangeHCone(Player p, ushort x, ushort y, ushort z, byte type, byte extType) {
            RevertAndClearState(p, x, y, z);
            Util.SCOGenerator.HCone(p, x, y, z, p.BcVar[0], p.BcVar[1], type, extType, false);
        }
        
        public void BlockchangeICone(Player p, ushort x, ushort y, ushort z, byte type, byte extType) {
            RevertAndClearState(p, x, y, z);
            Util.SCOGenerator.Cone(p, x, y, z, p.BcVar[0], p.BcVar[1], type, extType, true);
        }
        
        public void BlockchangeHICone(Player p, ushort x, ushort y, ushort z, byte type, byte extType) {
            RevertAndClearState(p, x, y, z);
            Util.SCOGenerator.HCone(p, x, y, z, p.BcVar[0], p.BcVar[1], type, extType, true);
        }

        public void BlockchangePyramid(Player p, ushort x, ushort y, ushort z, byte type, byte extType) {
            RevertAndClearState(p, x, y, z);
            Util.SCOGenerator.Pyramid(p, x, y, z, p.BcVar[0], p.BcVar[1], type, extType, false);
        }
        
        public void BlockchangeHPyramid(Player p, ushort x, ushort y, ushort z, byte type, byte extType) {
            RevertAndClearState(p, x, y, z);
            Util.SCOGenerator.HPyramid(p, x, y, z, p.BcVar[0], p.BcVar[1], type, extType, false);
        }
        
        public void BlockchangeIPyramid(Player p, ushort x, ushort y, ushort z, byte type, byte extType) {
            RevertAndClearState(p, x, y, z);
            Util.SCOGenerator.Pyramid(p, x, y, z, p.BcVar[0], p.BcVar[1], type, extType, true);
        }
        
        public void BlockchangeHIPyramid(Player p, ushort x, ushort y, ushort z, byte type, byte extType) {
            RevertAndClearState(p, x, y, z);
            Util.SCOGenerator.HPyramid(p, x, y, z, p.BcVar[0], p.BcVar[1], type, extType, true);
        }

        public void BlockchangeSphere(Player p, ushort x, ushort y, ushort z, byte type, byte extType) {
            RevertAndClearState(p, x, y, z);
            Util.SCOGenerator.Sphere(p, x, y, z, p.BcVar[1], type, extType);
        }
        
        public void BlockchangeHSphere(Player p, ushort x, ushort y, ushort z, byte type, byte extType) {
            RevertAndClearState(p, x, y, z);
            Util.SCOGenerator.HSphere(p, x, y, z, p.BcVar[1], type, extType);
        }

        public void BlockchangeVolcano(Player p, ushort x, ushort y, ushort z, byte type, byte extType) {
            RevertAndClearState(p, x, y, z);
            Util.SCOGenerator.Volcano(p, x, y, z, p.BcVar[0], p.BcVar[1]);
        }
        
        public override void Help(Player p) {
            p.SendMessage("/draw <shape> <height> <baseradius> - Draw an object in game- Valid Types cones, spheres, and pyramids, hspheres (hollow sphere), and hpyramids (hollow pyramid)");
        }
    }
}
