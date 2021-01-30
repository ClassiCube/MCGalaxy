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
using MCGalaxy.Eco;

namespace MCGalaxy.Commands.Eco {   
    public sealed class CmdAwardMod : Command2 {        
        public override string name { get { return "AwardMod"; } }
        public override string type { get { return CommandTypes.Economy; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        static char[] awardArgs = new char[] { ':' };

        public override void Use(Player p, string message, CommandData data) {
            if (message.Length == 0 || message.IndexOf(' ') == -1) { Help(p); return; }
            string[] args = message.SplitSpaces(2);

            if (IsCreateCommand(args[0])) {
                args = args[1].Split(awardArgs, 2);
                if (args.Length == 1) { 
                    p.Message("&WUse a : to separate the award name from its description."); 
                    Help(p); return;
                }

                if (!Awards.Add(args[0], args[1])) {
                    p.Message("This award already exists."); return;
                } else {
                    Chat.MessageGlobal("Award added: &6{0} : {1}", args[0], args[1]);
                    Awards.SaveAwards();
                }
            } else if (IsDeleteCommand(args[0])) {
                if (!Awards.Remove(args[1])) {
                    p.Message("This award does not exist."); return;
                } else {
                    Chat.MessageGlobal("Award removed: &6{0}", args[1]);
                    Awards.SaveAwards();
                }
            } else {
                Help(p);
            }
        }
        
        public override void Help(Player p) {
            p.Message("&T/AwardMod add [name] : [description]");
            p.Message("&HAdds a new award");
            p.Message("&H  e.g. &T/AwardMod add Bomb voyage : Blow up a lot of TNT");
            p.Message("&T/AwardMod del [name]");
            p.Message("&HDeletes the given award");
        }
    }
}
