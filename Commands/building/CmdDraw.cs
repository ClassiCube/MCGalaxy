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
using MCGalaxy.Drawing;
using MCGalaxy.Drawing.Brushes;
using MCGalaxy.Drawing.Ops;

namespace MCGalaxy.Commands
{
    public sealed class CmdDraw : Command
    {
        public override string name { get { return "draw"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Builder; } }
        public override CommandPerm[] AdditionalPerms {
            get { return new[] {
                    new CommandPerm(defaultRank, "The lowest rank that can use cones with /draw", 1),
                    new CommandPerm(defaultRank, "The lowest rank that can use pyramids with /draw", 2),
                    new CommandPerm(defaultRank, "The lowest rank that can use spheres with /draw", 3),
                    new CommandPerm(defaultRank, "The lowest rank that can use volcanos with /draw", 4),
                }; }
        }

        public override void Use(Player p, string message) {
            if (p == null) { MessageInGameOnly(p); return; }
            if (p.level.permissionbuild > p.group.Permission) {
                p.SendMessage("You can not edit this map."); return;
            }
            
            string[] parts = message.Split(' ');
            Player.BlockchangeEventHandler newHandler = null;

            switch (parts[0].ToLower()) {
                case "cone":
                    if (!CheckTwoArgs(p, 1, parts)) return;
                    newHandler = new Player.BlockchangeEventHandler(BlockchangeCone); break;
                case "hcone":
                    if (!CheckTwoArgs(p, 1, parts)) return;
                    newHandler = new Player.BlockchangeEventHandler(BlockchangeHCone); break;
                case "icone":
                    if (!CheckTwoArgs(p, 1, parts)) return;
                    newHandler = new Player.BlockchangeEventHandler(BlockchangeICone); break;
                case "hicone":
                    if (!CheckTwoArgs(p, 1, parts)) return;
                    newHandler = new Player.BlockchangeEventHandler(BlockchangeHICone); break;
                    
                case "pyramid":
                    if (!CheckTwoArgs(p, 2, parts)) return;
                    newHandler = new Player.BlockchangeEventHandler(BlockchangePyramid); break;
                case "hpyramid":
                    if (!CheckTwoArgs(p, 2, parts)) return;
                    newHandler = new Player.BlockchangeEventHandler(BlockchangeHPyramid); break;
                case "ipyramid":
                    if (!CheckTwoArgs(p, 2, parts)) return;
                    newHandler = new Player.BlockchangeEventHandler(BlockchangeIPyramid); break;
                case "hipyramid":
                    if (!CheckTwoArgs(p, 2, parts)) return;
                    newHandler = new Player.BlockchangeEventHandler(BlockchangeHIPyramid); break;

                case "sphere":
                    if (!CheckOneArg(p, 3, parts)) return;
                    newHandler = new Player.BlockchangeEventHandler(BlockchangeSphere); break;
                case "hsphere":
                    if (!CheckOneArg(p, 3, parts)) return;
                    newHandler = new Player.BlockchangeEventHandler(BlockchangeHSphere); break;
                case "volcano":
                    if (!CheckTwoArgs(p, 4, parts)) return;
                    newHandler = new Player.BlockchangeEventHandler(BlockchangeVolcano); break;
            }
            Player.SendMessage(p, "Place a block");
            p.ClearBlockchange();
            p.Blockchange += newHandler;        
        }
        
        bool CheckTwoArgs(Player p, int addition, string[] parts) {
            if ((int)p.group.Permission < CommandOtherPerms.GetPerm(this, addition)) {
                Group group = Group.findPermInt(CommandOtherPerms.GetPerm(this, addition));
                Player.SendMessage(p, "That commands addition is for " + group.name + "+");
                return false;
            }
            
            if (parts.Length != 3) { Help(p); return false; }
            ushort height, radius;
            if (!ushort.TryParse(parts[1], out height) || height > 2000 ||
                !ushort.TryParse(parts[2], out radius) || radius > 2000) {
                Player.SendMessage(p, "Radius and height must be positive integers less than 2000."); return false;
            }
            p.BcVar = new int[] { height, radius };
            return true;
        }
        
        bool CheckOneArg(Player p, int addition, string[] parts) {
            if ((int)p.group.Permission < CommandOtherPerms.GetPerm(this, addition)) {
                Group group = Group.findPermInt(CommandOtherPerms.GetPerm(this, addition));
                Player.SendMessage(p, "That commands addition is for " + group.name + "+");
                return false;
            }
            
            if (parts.Length != 2) { Help(p); return false; }
            ushort radius;
            if (!ushort.TryParse(parts[1], out radius) || radius > 2000) { 
                Player.SendMessage(p, "Radius must be a positive integer less than 2000."); return false;
            }
            p.BcVar = new int[] { 0, radius };
            return true;
        }

        void BlockchangeCone(Player p, ushort x, ushort y, ushort z, byte type, byte extType) {
            RevertAndClearState(p, x, y, z);
            Cone(p, x, y, z, p.BcVar[0], p.BcVar[1], type, extType, false);
        }
        
        void BlockchangeHCone(Player p, ushort x, ushort y, ushort z, byte type, byte extType) {
            RevertAndClearState(p, x, y, z);
            HCone(p, x, y, z, p.BcVar[0], p.BcVar[1], type, extType, false);
        }
        
        void BlockchangeICone(Player p, ushort x, ushort y, ushort z, byte type, byte extType) {
            RevertAndClearState(p, x, y, z);
            Cone(p, x, y, z, p.BcVar[0], p.BcVar[1], type, extType, true);
        }
        
        void BlockchangeHICone(Player p, ushort x, ushort y, ushort z, byte type, byte extType) {
            RevertAndClearState(p, x, y, z);
            HCone(p, x, y, z, p.BcVar[0], p.BcVar[1], type, extType, true);
        }

        void BlockchangePyramid(Player p, ushort x, ushort y, ushort z, byte type, byte extType) {
            RevertAndClearState(p, x, y, z);
            Pyramid(p, x, y, z, p.BcVar[0], p.BcVar[1], type, extType, false);
        }
        
        void BlockchangeHPyramid(Player p, ushort x, ushort y, ushort z, byte type, byte extType) {
            RevertAndClearState(p, x, y, z);
            HPyramid(p, x, y, z, p.BcVar[0], p.BcVar[1], type, extType, false);
        }
        
        void BlockchangeIPyramid(Player p, ushort x, ushort y, ushort z, byte type, byte extType) {
            RevertAndClearState(p, x, y, z);
            Pyramid(p, x, y, z, p.BcVar[0], p.BcVar[1], type, extType, true);
        }
        
        void BlockchangeHIPyramid(Player p, ushort x, ushort y, ushort z, byte type, byte extType) {
            RevertAndClearState(p, x, y, z);
            HPyramid(p, x, y, z, p.BcVar[0], p.BcVar[1], type, extType, true);
        }

        void BlockchangeSphere(Player p, ushort x, ushort y, ushort z, byte type, byte extType) {
            RevertAndClearState(p, x, y, z);
            AdvSphereDrawOp op = new AdvSphereDrawOp();
            op.Radius = p.BcVar[1];
            Brush brush = new SolidBrush(type, extType);
            DrawOp.DoDrawOp(op, brush, p, new [] { new Vec3U16(x, y, z) });
        }
        
        void BlockchangeHSphere(Player p, ushort x, ushort y, ushort z, byte type, byte extType) {
            RevertAndClearState(p, x, y, z);
            AdvHollowSphereDrawOp op = new AdvHollowSphereDrawOp();
            op.Radius = p.BcVar[1];
            Brush brush = new SolidBrush(type, extType);
            DrawOp.DoDrawOp(op, brush, p, new [] { new Vec3U16(x, y, z) });
        }

        void BlockchangeVolcano(Player p, ushort x, ushort y, ushort z, byte type, byte extType) {
            RevertAndClearState(p, x, y, z);
            AdvVolcanoDrawOp op = new AdvVolcanoDrawOp();
            op.Radius = p.BcVar[1]; op.Height = p.BcVar[1];
            DrawOp.DoDrawOp(op, null, p, new [] { new Vec3U16(x, y, z) });
        }
        
        static void Cone(Player p, ushort x, ushort y, ushort z, int height, int radius, byte type, byte extType, bool invert) {
            AdvConeDrawOp op = new AdvConeDrawOp();
            op.Radius = radius; op.Height = height; op.Invert = invert;
            Brush brush = new SolidBrush(type, extType);
            DrawOp.DoDrawOp(op, brush, p, new [] { new Vec3U16(x, y, z) });
        }
        
        static void HCone(Player p, ushort x, ushort y, ushort z, int height, int radius, byte type, byte extType, bool invert) {
            AdvHollowConeDrawOp op = new AdvHollowConeDrawOp();
            op.Radius = radius; op.Height = height; op.Invert = invert;
            Brush brush = new SolidBrush(type, extType);
            DrawOp.DoDrawOp(op, brush, p, new [] { new Vec3U16(x, y, z) });
        }

        static void Pyramid(Player p, ushort x, ushort y, ushort z, int height, int radius, byte type, byte extType, bool invert) {
            AdvPyramidDrawOp op = new AdvPyramidDrawOp();
            op.Radius = radius; op.Height = height; op.Invert = invert;
            Brush brush = new SolidBrush(type, extType);
            DrawOp.DoDrawOp(op, brush, p, new [] { new Vec3U16(x, y, z) });
        }
        
        static void HPyramid(Player p, ushort x, ushort y, ushort z, int height, int radius, byte type, byte extType, bool invert) {
            AdvHollowPyramidDrawOp op = new AdvHollowPyramidDrawOp();
            op.Radius = radius; op.Height = height; op.Invert = invert;
            Brush brush = new SolidBrush(type, extType);
            DrawOp.DoDrawOp(op, brush, p, new [] { new Vec3U16(x, y, z) });
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "/draw <shape> <height> <baseradius> - Draw an object in game- " +
                               "Valid Types cones, spheres, and pyramids, hspheres (hollow sphere), and hpyramids (hollow pyramid)");
        }
    }
}
