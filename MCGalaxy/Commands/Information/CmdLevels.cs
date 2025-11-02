/*
    Copyright 2011 MCForge
        
    Dual-licensed under the Educational Community License, Version 2.0 and
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

namespace MCGalaxy.Commands.Info 
{
    public sealed class CmdLevels : Command2 
    {
        public override string name { get { return "Levels"; } }
        public override string shortcut { get { return "Worlds"; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool UseableWhenFrozen { get { return true; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("Maps") }; }
        }
        
        public override void Use(Player p, string message, CommandData data) {
            string[] files = LevelInfo.AllMapNames();
            // Files list is not guaranteed to be in alphabetical order
            Array.Sort(files, new AlphanumComparator());
            LevelInfo.ListMaps(p, files, "Levels", "Levels", "levels", message);
        }

        public override void Help(Player p) {
            p.Message("&T/Levels");
            p.Message("&HLists levels and whether you can go to them.");
        }
    }
}
