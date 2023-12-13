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
    class CmdWarp : WarpCommand 
    {
        public override string name { get { return "Warp"; } }
        public override CommandPerm[] ExtraPerms {
            get { return new[] { new CommandPerm(LevelPermission.Operator, "can manage warps") }; }
        }
        
        public override void Use(Player p, string message, CommandData data) {
            if (WarpList.Global == null)
                WarpList.Global = LoadList("extra/warps.save");

            UseCore(p, message, data, WarpList.Global, "Warp");
        }
        
        public override void Help(Player p) {
            p.Message("&T/Warp [name] &H- Move to that warp");
            p.Message("&T/Warp list &H- List all the warps");
            p.Message("&T/Warp create [name] &H- Create a warp at your position");
            p.Message("&T/Warp delete [name] &H- Deletes a warp");
            p.Message("&T/Warp move [name] &H- Moves a warp to your position");
        }
    }
}
