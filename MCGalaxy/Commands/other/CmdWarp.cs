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

namespace MCGalaxy.Commands {
    public sealed class CmdWarp : Command {
        public override string name { get { return "warp"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public override CommandPerm[] ExtraPerms {
            get { return new[] {
                    new CommandPerm(LevelPermission.Operator, "+ can create warps"),
                    new CommandPerm(LevelPermission.Operator, "+ can delete warps"),
                    new CommandPerm(LevelPermission.Operator, "+ can move/edit warps"),
                }; }
        }
        
        public override void Use(Player p, string message) {
            if (Player.IsSuper(p)) { MessageInGameOnly(p); return; }
            WarpList warps = WarpList.Global;
            string[] args = message.ToLower().Split(' ');
            string cmd = args[0];
            if (cmd == "") { Help(p); return; }
            
            if (args.Length == 1 && (cmd == "list" || cmd == "view")) {
                Player.Message(p, "Warps:");
                foreach (Warp wr in warps.Items) {
                    if (LevelInfo.FindExact(wr.lvlname) != null)
                        Player.Message(p, wr.name + " : " + wr.lvlname);
                }
                return;
            } else if (args.Length == 1) {
                if (!warps.Exists(cmd)) { Player.Message(p, "That warp does not exist"); }
                warps.Goto(cmd, p);
                return;
            }
            
            string name = args[1];
            if (cmd == "create" || cmd == "add") {
                if (!CheckExtraPerm(p, 1)) { MessageNeedExtra(p, "create warps.", 1); return; }
                if (warps.Exists(name)) { Player.Message(p, "That warp already exists"); return; }                
                Player who = args.Length == 2 ? p : PlayerInfo.FindMatches(p, args[2]);
                if (who == null) return;

                warps.Create(name, who);
                Player.Message(p, "Warp created.");
            } else if (cmd == "delete" || cmd == "remove") {
                if (!CheckExtraPerm(p, 2)) { MessageNeedExtra(p, "delete warps.", 2); return; }
                if (!warps.Exists(name)) { Player.Message(p, "That warp does not exist"); return; }
                
                warps.Remove(name, p);
                Player.Message(p, "Warp deleted.");
            } else if (cmd == "move" || cmd == "change" || cmd == "edit") {
                if (!CheckExtraPerm(p, 3)) { MessageNeedExtra(p, "move warps.", 3); return; }
                if (!warps.Exists(name)) { Player.Message(p, "Warp doesn't exist!!"); return; }
                Player who = args.Length == 2 ? p : PlayerInfo.FindMatches(p, args[2]);
                if (who == null) return;
                
                warps.Update(name, who);
                Player.Message(p, "Warp moved.");
            } else {
                Help(p);
            }
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/warp [name] %H- Move to that warp");
            Player.Message(p, "%T/warp list %H- List all the warps");
            if (CheckExtraPerm(p, 1))
                Player.Message(p, "%T/warp create [name] <player> %H- Create a warp, if a <player> is given, it will be created where they are");
            if (CheckExtraPerm(p, 2))
                Player.Message(p, "%T/warp delete [name] %H- Deletes a warp");
            if (CheckExtraPerm(p, 3))
                Player.Message(p, "%T/warp move [name] <player> %H- Moves a warp, if a <player> is given, it will be created where they are");
        }
    }
}
