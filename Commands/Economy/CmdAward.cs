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
        
        public override void Use(Player p, string message) {
            if (message == "" || message.IndexOf(' ') == -1) { Help(p); return; }
            bool take = false;
            if (message.CaselessStarts("give ")) {
                message = message.Substring(5);
            } else if (message.CaselessStarts("take ")) {
                message = message.Substring(5); take = true;
            }
                    
            string[] args = message.SplitSpaces(2);
            string plName = args[0];
            Player who = PlayerInfo.Find(plName);
            if (who != null) plName = who.name;
            
            string award = args.Length > 1 ? args[1] : "";
            award = Awards.Find(award);
            if (award == null) {
                Player.Message(p, "The award you entered doesn't exist");
                Player.Message(p, "Use /awards for a list of awards");
                return;
            }

            if (!take) {
                if (Awards.GiveAward(plName, award)) {
                    Player.GlobalMessage(Server.FindColor(plName) + plName + " %Swas awarded: &b" + award);
                } else {
                    Player.Message(p, "The player already has that award."); return;
                }
            } else {
                if (Awards.TakeAward(plName, award)) {
                    Player.GlobalMessage(Server.FindColor(plName) + plName + " %Shad their &b" + award + " %Saward removed");
                } else {
                    Player.Message(p, "The player didn't have the award you tried to take"); return;
                }
            }
            Awards.Save();
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/award <give/take> [player] [award] %H- Gives/Takes [player] the [award]");
            Player.Message(p, "%HIf only [player] and [aware] are given, Give is used.");
            Player.Message(p, "%H[award] needs to be the full award's name. Not partial");
        }
    }
}
