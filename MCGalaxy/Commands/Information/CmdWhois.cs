/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCForge)
    
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
using MCGalaxy.DB;

namespace MCGalaxy.Commands.Info 
{
    public sealed class CmdWhois : Command2 
    {
        public override string name { get { return "WhoIs"; } }
        public override string shortcut { get { return "WhoWas"; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool UseableWhenFrozen { get { return true; } }
        public override CommandPerm[] ExtraPerms {
            get { return new[] { new CommandPerm(LevelPermission.Operator, "can see sensitive player information (e.g. IP)") }; }
        }
        public override CommandAlias[] Aliases {
            get { return new CommandAlias[] { new CommandAlias("Info"), new CommandAlias("i") }; }
        }
        
        public override void Use(Player p, string message, CommandData data) {
            if (message.Length == 0) {
                if (p.IsSuper) { SuperRequiresArgs(p, "player name"); return; }
                message = p.name;
            }
            if (!Formatter.ValidPlayerName(p, message)) return;
            
            int matches;
            Player who = PlayerInfo.FindMatches(p, message, out matches);
            if (matches > 1) return;
            
            if (matches == 0) {
                p.Message("Searching database for the player..");
                PlayerData target = PlayerDB.Match(p, message);
                if (target == null) return;
                
                foreach (OfflineStatPrinter printer in OfflineStat.Stats) {
                    printer(p, target);
                }
            } else {
                foreach (OnlineStatPrinter printer in OnlineStat.Stats) {
                    printer(p, who);
                }
            }
        }

        public override void Help(Player p) {
            p.Message("&T/WhoIs [player]");
            p.Message("&HDisplays information about that player.");
            p.Message("&HNote: Works for both online and offline players.");
        }
    }
}