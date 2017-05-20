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
    public sealed class CmdLevels : Command {       
        public override string name { get { return "levels"; } }
        public override string shortcut { get { return "maps"; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("worlds") }; }
        }

        public override void Use(Player p, string message) {
            Level[] loaded = LevelInfo.Loaded.Items;
            Player.Message(p, "Loaded maps [physics level] (&c[no] %Sif not visitable): ");
            MultiPageOutput.Output(p, loaded, (lvl) => FormatMap(p, lvl),
                                   "levels", "maps", message, false);
            Player.Message(p, "Use %T/unloaded %Sfor unloaded levels.");
        }
        
        static string FormatMap(Player p, Level lvl) {            
            bool canVisit = Player.IsSuper(p);
            if (!canVisit) {
                AccessResult access = lvl.VisitAccess.Check(p);
                canVisit = access == AccessResult.Allowed || access == AccessResult.Whitelisted;
            }
            
            string physics = " [" +  lvl.physics + "]";
            string visit = canVisit ? "" : " &c[no]";
            return lvl.ColoredName + physics + visit;
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/levels");
            Player.Message(p, "%HLists all loaded levels and their physics levels.");
        }
    }
}
