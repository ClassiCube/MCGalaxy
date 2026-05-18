/*
    Copyright 2010 MCLawl Team - Written by Valek (Modified for use with MCForge)
 
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
namespace MCGalaxy.Commands.Chatting 
{
    public class CmdTColor : EntityPropertyCmd 
    {
        public override string name { get { return "TColor"; } }
        public override string type { get { return CommandTypes.Chat; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        public override CommandPerm[] ExtraPerms {
            get { return new[] { new CommandPerm(LevelPermission.Operator, "can change the title color of others") }; }
        }
        public override CommandAlias[] Aliases {
            get { return new[] {
                new CommandAlias("XTColor"),
                new CommandAlias("OTColor", OTHER_FLAG)
            }; }
        }
        public override void Use(Player p, string message, CommandData data) { 
            UsePlayer(p, data, message, "title color"); 
        }
        
        protected override void SetPlayerData(Player p, string target, string colName) {
            PlayerOperations.SetTitleColor(p, target, colName);
        }

        public override void Help(Player p) {
            p.Message("&T/TColor <color>");
            p.Message("&H Sets your title color");
            p.Message("&T/OTColor [player] <color>");
            p.Message("&H Sets the title color of other player");
            p.Message("&H  Leave color blank to reset it.");
            p.Message("&H  To see a list of all colors, use &T/Help colors.");
        }
    }
}
