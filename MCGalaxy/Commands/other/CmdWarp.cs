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

namespace MCGalaxy.Commands.Misc {
    public class CmdWarp : Command {
        public override string name { get { return "Warp"; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public override bool SuperUseable { get { return false; } }
        public override CommandPerm[] ExtraPerms {
            get { return new[] {
                    new CommandPerm(LevelPermission.Operator, "+ can create warps"),
                    new CommandPerm(LevelPermission.Operator, "+ can delete warps"),
                    new CommandPerm(LevelPermission.Operator, "+ can move/edit warps"),
                }; }
        }
        protected virtual bool CheckExtraPerms { get { return true; } }
        
        public override void Use(Player p, string message) {
            UseCore(p, message, WarpList.Global, "Warp");
        }
        
        protected void UseCore(Player p, string message, WarpList warps, string group) {
            string[] args = message.ToLower().SplitSpaces();
            string cmd = args[0];
            if (cmd.Length == 0) { Help(p); return; }
            
            if (args.Length == 1 && (cmd == "list" || cmd == "view")) {
                Player.Message(p, "{0}s:", group);
                foreach (Warp wr in warps.Items) {
                    if (LevelInfo.FindExact(wr.Level) != null)
                        Player.Message(p, wr.Name + " : " + wr.Level);
                }
                return;
            } else if (args.Length == 1) {
                Warp warp = Matcher.FindWarps(p, warps, cmd);
                if (warp != null) warps.Goto(warp, p);
                return;
            }
            
            string name = args[1];
            if (cmd == "create" || cmd == "add") {
                if (CheckExtraPerms && !CheckExtraPerm(p, 1)) { MessageNeedExtra(p, 1); return; }
                if (warps.Exists(name)) { Player.Message(p, "{0} already exists", group); return; }
                
                Player who = args.Length == 2 ? p : PlayerInfo.FindMatches(p, args[2]);
                if (who == null) return;

                warps.Create(name, who);
                Player.Message(p, "{0} {1} created.", group, name);
            } else if (cmd == "delete" || cmd == "remove") {
                if (CheckExtraPerms && !CheckExtraPerm(p, 2)) { MessageNeedExtra(p, 2); return; }
                Warp warp = Matcher.FindWarps(p, warps, name);
                if (warp == null) return;
                
                warps.Remove(warp, p);
                Player.Message(p, "{0} {1} deleted.", group, warp.Name);
            } else if (cmd == "move" || cmd == "update") {
                if (CheckExtraPerms && !CheckExtraPerm(p, 3)) { MessageNeedExtra(p, 3); return; }
                Warp warp = Matcher.FindWarps(p, warps, name);
                if (warp == null) return;
                
                Player who = args.Length == 2 ? p : PlayerInfo.FindMatches(p, args[2]);
                if (who == null) return;
                
                warps.Update(warp, who);
                Player.Message(p, "{0} {1} moved.", group, warp.Name);
            } else if (cmd == "goto") {
                Warp warp = Matcher.FindWarps(p, warps, name);
                if (warp != null) warps.Goto(warp, p);
            } else {
                Help(p);
            }
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/Warp [name] %H- Move to that warp");
            Player.Message(p, "%T/Warp list %H- List all the warps");
            if (CheckExtraPerm(p, 1))
                Player.Message(p, "%T/Warp create [name] <player> %H- Create a warp, if a <player> is given, it will be created where they are");
            if (CheckExtraPerm(p, 2))
                Player.Message(p, "%T/Warp delete [name] %H- Deletes a warp");
            if (CheckExtraPerm(p, 3))
                Player.Message(p, "%T/Warp move [name] <player> %H- Moves a warp, if a <player> is given, it will be created where they are");
        }
    }
}
