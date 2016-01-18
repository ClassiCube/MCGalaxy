/*
	Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
	
	Dual-licensed under the	Educational Community License, Version 2.0 and
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
	
    public sealed class CmdBanEdit : Command {
		
        public override string name { get { return "banedit"; } }
        public override string shortcut { get { return "be"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdBanEdit() { }
        static char[] trimChars = { ' ' };

        public override void Use(Player p, string message) {
        	string[] args = message.Split(trimChars, 2);
            if (args.Length < 2) { Help(p); return; }

            string err = Ban.EditReason(args[0], args[1].Replace(" ", "%20"));
            if (err != "")
                Player.SendMessage(p, err);
            else
            	Player.SendMessage(p, "Succesfully edited baninfo about &0" + args[0] + " %Sto: &2" + args[1]);
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "/banedit <username> <reason> - Edits reason of ban for the user.");
        }
    }
}
