/*
    Copyright 2011 MCForge
        
    Dual-licensed under the    Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    https://opensource.org/license/ecl-2-0/
    https://www.gnu.org/licenses/gpl-3.0.html
    
    Unless required by applicable law or agreed to in writing,
    software distributed under the Licenses are distributed on an "AS IS"
    BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
    or implied. See the Licenses for the specific language governing
    permissions and limitations under the Licenses.
 */
using System;
using MCGalaxy.Commands;
using MCGalaxy.Maths;

namespace MCGalaxy.Modules.Warps
{
    abstract class WarpCommand : Command2 
    {
        public override string type { get { return CommandTypes.World; } }
        public override bool museumUsable { get { return false; } }
        public override bool SuperUseable { get { return false; } }
        
        static void PrintWarp(Player p, Warp warp) {
            Vec3S32 pos = warp.Pos.BlockCoords;
            p.Message("{0} - ({1}, {2}, {3}) on {4}",
                      warp.Name, pos.X, pos.Y, pos.Z, warp.Level);
        }
        
        protected void UseCore(Player p, string message, CommandData data,
                               WarpList warps, string group) {
            string[] args = message.SplitSpaces();
            string cmd = args[0];
            if (cmd.Length == 0) { Help(p); return; }
            bool checkExtraPerms = warps == WarpList.Global;
            
            if (IsListAction(cmd)) {
                string modifier = args.Length > 1 ? args[1] : "";
                Paginator.Output(p, warps.Items, PrintWarp, 
                                 group + " list", group + "s", modifier);
                return;
            } else if (args.Length == 1) {
                Warp warp = warps.FindMatch(p, cmd);
                if (warp != null) warps.Goto(warp, p);
                return;
            }
            
            string name = args[1];
            if (IsCreateAction(cmd)) {
                if (checkExtraPerms && !CheckExtraPerm(p, data, 1)) return;
                if (warps.Exists(name)) { p.Message("{0} already exists", group); return; }

                warps.Create(name, p);
                p.Message("{0} {1} created.", group, name);
            } else if (IsDeleteAction(cmd)) {
                if (checkExtraPerms && !CheckExtraPerm(p, data, 1)) return;
                Warp warp = warps.FindMatch(p, name);
                if (warp == null) return;
                
                warps.Remove(warp, p);
                p.Message("{0} {1} deleted.", group, warp.Name);
            } else if (IsEditAction(cmd)) {
                if (checkExtraPerms && !CheckExtraPerm(p, data, 1)) return;
                Warp warp = warps.FindMatch(p, name);
                if (warp == null) return;

                warps.Update(warp, p);
                p.Message("{0} {1} moved.", group, warp.Name);
            } else if (cmd.CaselessEq("goto")) {
                Warp warp = warps.FindMatch(p, name);
                if (warp != null) warps.Goto(warp, p);
            } else {
                Help(p);
            }
        }
        
        protected static WarpList LoadList(string path) {
            WarpList list = new WarpList();
            
            list.Filename = path;
            list.Load();
            return list;
        }
    }
}
