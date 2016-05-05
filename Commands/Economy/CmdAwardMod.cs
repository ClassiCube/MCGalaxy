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
	
    public sealed class CmdAwardMod : Command {
		
        public override string name { get { return "awardmod"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Economy; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        public CmdAwardMod() { }
        static char[] trimChars = { ' ' }, awardArgs = { ':' };

        public override void Use(Player p, string message) {
            if (message == "" || message.IndexOf(' ') == -1) { Help(p); return; }            
            string[] args = message.Split(trimChars, 2);
            if (!(args[0].CaselessEq("add") || args[0].CaselessEq("del"))) { Help(p); return; }

            if (args[0].CaselessEq("add")) {
                args = args[1].Split(awardArgs, 2);
                if (args.Length == 1) { 
                    Player.Message(p, "&cUse a : to separate the award name from its description."); 
                    Help(p); return; 
                }

                if (!Awards.Add(args[0], args[1])) {
                    Player.Message(p, "This award already exists."); return;
                } else {
                    Player.GlobalMessage("Award added: &6" + args[0] + " : " + args[1]);
                }
            } else {
                if (!Awards.Remove(args[1])) {
                    Player.Message(p, "This award does not exist."); return;
                } else {
                    Player.GlobalMessage("Award removed: &6" + args[1]);
                }
            }
            Awards.Save();
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/awardmod <add/del> [award name] : [description]");
            Player.Message(p, "%HAdds or deletes a reward with the name [award name]");
            Player.Message(p, "%T/awardmod add Bomb joy : Bomb lots of people %His an example");
        }
    }
}
