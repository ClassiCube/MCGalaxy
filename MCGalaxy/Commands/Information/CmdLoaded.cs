/*
    Copyright 2012 MCForge
    
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

namespace MCGalaxy.Commands.Info {   
    public sealed class CmdLoaded : Command2 {       
        public override string name { get { return "Loaded"; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool UseableWhenFrozen { get { return true; } }
        
        public override void Use(Player p, string message, CommandData data) {
            Level[] loaded = LevelInfo.Loaded.Items;
            p.Message("Loaded levels [physics level] (&c[no] %Sif not visitable): ");
            MultiPageOutput.Output(p, loaded, (lvl) => FormatMap(p, lvl),
                                   "Levels", "levels", message, false);
            p.Message("Use %T/Levels %Sfor all levels.");
        }
        
        static string FormatMap(Player p, Level lvl) {            
            bool canVisit = p.IsSuper || lvl.VisitAccess.CheckAllowed(p);
            string physics = " [" +  lvl.physics + "]";
            string visit = canVisit ? "" : " &c[no]";
            return lvl.ColoredName + physics + visit;
        }
        
        public override void Help(Player p) {
            p.Message("%T/Loaded");
            p.Message("%HLists loaded levels and their physics levels.");
        }
    }
}
