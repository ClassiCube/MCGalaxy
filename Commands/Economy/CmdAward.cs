/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
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
namespace MCGalaxy.Commands {
    
    public sealed class CmdAward : Command {
        
        public override string name { get { return "award"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Economy; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdAward() { }
        static char[] trimChars = { ' ' };
        
        public override void Use(Player p, string message) {
            if (message == "" || message.IndexOf(' ') == -1) { Help(p); return; }
            string[] args = message.Split(trimChars, 3);           
            string pl = args[args.Length - 2];
            Player who = PlayerInfo.Find(pl);
            if (who != null) pl = who.name;
            
            string award = args[args.Length - 1];
            if (!Awards.ExistsAward(award)) {
                Player.SendMessage(p, "The award you entered doesn't exist");
                Player.SendMessage(p, "Use /awards for a list of awards");
                return;
            }

            if (args.Length == 2 || !args[0].CaselessEq("take")) {
                if (Awards.GiveAward(pl, award)) {
                    Player.GlobalMessage(Server.FindColor(pl) + pl + " %Swas awarded: &b" + award);
                } else {
                    Player.SendMessage(p, "The player already has that award."); return;
                }
            } else {
                if (Awards.TakeAward(pl, award)) {
                    Player.GlobalMessage(Server.FindColor(pl) + pl + " %Shad their &b" + award + " %Saward removed");
                } else {
                    Player.SendMessage(p, "The player didn't have the award you tried to take"); return;
                }
            }
            Awards.Save();
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "/award <give/take> [player] [award] - Gives [player] the [award]");
            Player.SendMessage(p, "If no Give or Take is given, Give is used");
            Player.SendMessage(p, "[award] needs to be the full award's name. Not partial");
        }
    }
}
