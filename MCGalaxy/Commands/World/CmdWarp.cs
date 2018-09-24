/*
    Copyright 2011 MCForge
        
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
using System.Threading;
using MCGalaxy.Maths;

namespace MCGalaxy.Commands.Misc {
    public class CmdWarp : Command2 {
        public override string name { get { return "Warp"; } }
        public override string type { get { return CommandTypes.World; } }
        public override bool museumUsable { get { return false; } }
        public override bool SuperUseable { get { return false; } }
        public override CommandPerm[] ExtraPerms {
            get { return new[] { new CommandPerm(LevelPermission.Operator, "can manage warps") }; }
        }
        
        public override void Use(Player p, string message, CommandData data) {
            UseCore(p, message, data, WarpList.Global, "Warp");
        }
        
        static string FormatWarp(Warp warp) {
            Vec3S32 pos = warp.Pos.BlockCoords;
            return warp.Name + " - (" + pos.X + ", " + pos.Y + ", " + pos.Z + ") on " + warp.Level;
        }
        
        protected void UseCore(Player p, string message, CommandData data,
                               WarpList warps, string group) {
            string[] args = message.SplitSpaces();
            string cmd = args[0];
            if (cmd.Length == 0) { Help(p); return; }
            bool checkExtraPerms = warps == WarpList.Global;
            
            if (IsListCommand(cmd)) {
                string modifier = args.Length > 1 ? args[1] : "";
                MultiPageOutput.Output(p, warps.Items, FormatWarp, group + " list", group + "s", modifier, true);
                return;
            } else if (args.Length == 1) {
                Warp warp = Matcher.FindWarps(p, warps, cmd);
                if (warp != null) warps.Goto(warp, p);
                return;
            }
            
            string name = args[1];
            if (IsCreateCommand(cmd)) {
                if (checkExtraPerms && !CheckExtraPerm(p, data, 1)) return;
                if (warps.Exists(name)) { p.Message("{0} already exists", group); return; }

                warps.Create(name, p);
                p.Message("{0} {1} created.", group, name);
            } else if (IsDeleteCommand(cmd)) {
                if (checkExtraPerms && !CheckExtraPerm(p, data, 1)) return;
                Warp warp = Matcher.FindWarps(p, warps, name);
                if (warp == null) return;
                
                warps.Remove(warp, p);
                p.Message("{0} {1} deleted.", group, warp.Name);
            } else if (IsEditCommand(cmd)) {
                if (checkExtraPerms && !CheckExtraPerm(p, data, 1)) return;
                Warp warp = Matcher.FindWarps(p, warps, name);
                if (warp == null) return;

                warps.Update(warp, p);
                p.Message("{0} {1} moved.", group, warp.Name);
            } else if (cmd.CaselessEq("goto")) {
                Warp warp = Matcher.FindWarps(p, warps, name);
                if (warp != null) warps.Goto(warp, p);
            } else {
                Help(p);
            }
        }
        
        public override void Help(Player p) {
            p.Message("%T/Warp [name] %H- Move to that warp");
            p.Message("%T/Warp list %H- List all the warps");
            p.Message("%T/Warp create [name] %H- Create a warp at your position");
            p.Message("%T/Warp delete [name] %H- Deletes a warp");
            p.Message("%T/Warp move [name] %H- Moves a warp to your position");
        }
    }
}
