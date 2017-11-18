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
    public class CmdWarp : Command {
        public override string name { get { return "Warp"; } }
        public override string type { get { return CommandTypes.World; } }
        public override bool museumUsable { get { return false; } }
        public override bool SuperUseable { get { return false; } }
        public override CommandPerm[] ExtraPerms {
            get { return new[] {
                    new CommandPerm(LevelPermission.Operator, "+ can create warps"),
                    new CommandPerm(LevelPermission.Operator, "+ can delete warps"),
                    new CommandPerm(LevelPermission.Operator, "+ can move/edit warps"),
                }; }
        }
        
        public override void Use(Player p, string message) {
            UseCore(p, message, WarpList.Global, "Warp", true);
        }
        
        static string FormatWarp(Warp warp) {
            Vec3S32 pos = warp.Pos.BlockCoords;
            return warp.Name + " - (" + pos.X + ", " + pos.Y + ", " + pos.Z + ") on " + warp.Level;
        }
        
        protected void UseCore(Player p, string message, WarpList warps, 
                               string group, bool checkExtraPerms) {
            string[] args = message.SplitSpaces();
            string cmd = args[0];
            if (cmd.Length == 0) { Help(p); return; }
            
            if (cmd.CaselessEq("list")) {
                string modifier = args.Length > 1 ? args[1] : "";
                MultiPageOutput.Output(p, warps.Items, FormatWarp, group + " list", group + "s", modifier, true);
                return;
            } else if (args.Length == 1) {
                Warp warp = Matcher.FindWarps(p, warps, cmd);
                if (warp != null) warps.Goto(warp, p);
                return;
            }
            
            string name = args[1];
            if (cmd.CaselessEq("create") || cmd.CaselessEq("add")) {
                if (checkExtraPerms && !CheckExtraPerm(p, 1)) return;
                if (warps.Exists(name)) { Player.Message(p, "{0} already exists", group); return; }
                
                Player who = args.Length == 2 ? p : PlayerInfo.FindMatches(p, args[2]);
                if (who == null) return;

                warps.Create(name, who);
                Player.Message(p, "{0} {1} created.", group, name);
            } else if (cmd.CaselessEq("delete") || cmd.CaselessEq("remove")) {
                if (checkExtraPerms && !CheckExtraPerm(p, 2)) return;
                Warp warp = Matcher.FindWarps(p, warps, name);
                if (warp == null) return;
                
                warps.Remove(warp, p);
                Player.Message(p, "{0} {1} deleted.", group, warp.Name);
            } else if (cmd.CaselessEq("move") || cmd.CaselessEq("update")) {
                if (checkExtraPerms && !CheckExtraPerm(p, 3)) return;
                Warp warp = Matcher.FindWarps(p, warps, name);
                if (warp == null) return;
                
                Player who = args.Length == 2 ? p : PlayerInfo.FindMatches(p, args[2]);
                if (who == null) return;
                
                warps.Update(warp, who);
                Player.Message(p, "{0} {1} moved.", group, warp.Name);
            } else if (cmd.CaselessEq("goto")) {
                Warp warp = Matcher.FindWarps(p, warps, name);
                if (warp != null) warps.Goto(warp, p);
            } else {
                Help(p);
            }
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/Warp [name] %H- Move to that warp");
            Player.Message(p, "%T/Warp list %H- List all the warps");
            if (HasExtraPerm(p, 1))
                Player.Message(p, "%T/Warp create [name] <player> %H- Create a warp, if a <player> is given, it will be created where they are");
            if (HasExtraPerm(p, 2))
                Player.Message(p, "%T/Warp delete [name] %H- Deletes a warp");
            if (HasExtraPerm(p, 3))
                Player.Message(p, "%T/Warp move [name] <player> %H- Moves a warp, if a <player> is given, it will be created where they are");
        }
    }
}
