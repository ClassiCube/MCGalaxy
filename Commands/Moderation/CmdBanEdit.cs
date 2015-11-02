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
namespace MCGalaxy.Commands
{
    public sealed class CmdBanEdit : Command
    {
        public override string name { get { return "banedit"; } }
        public override string shortcut { get { return "be"; } }
       public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdBanEdit() { }

        public override void Use(Player p, string message)
        {
            if (message.Split(' ').Length < 2) { Help(p); return; }
            if (message.Split(' ')[1].Length == 1)
            {
                Help(p); return;
            }
            string username = message.Split(' ')[0];
            string creason = message;
            string reason = creason.Remove(0, username.Length + 1).Replace(" ", "%20");
            string errormessage = Ban.Editreason(username, reason);
            if (errormessage != "")
            {
                Player.SendMessage(p, errormessage);
            }
            else
            {
                Player.SendMessage(p, "Succesfully edited baninfo about &0" + username + Server.DefaultColor + " to: &2" + reason.Replace("%20", " "));
            }
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/banedit <username> <reason> - Edits reason of ban for the user.");
        }
    }
}
