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
    public sealed class CmdAwardMod : Command
    {
        public override string name { get { return "awardmod"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Economy; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        public CmdAwardMod() { }

        public override void Use(Player p, string message)
        {
            if (message == "" || message.IndexOf(' ') == -1) { Help(p); return; }

            bool add = true;
            if (message.Split(' ')[0].ToLower() == "add")
            {
                message = message.Substring(message.IndexOf(' ') + 1);
            }
            else if (message.Split(' ')[0].ToLower() == "del")
            {
                add = false;
                message = message.Substring(message.IndexOf(' ') + 1);
            }

            if (add)
            {
                if (message.IndexOf(":") == -1) { Player.SendMessage(p, "&cMissing a colon!"); Help(p); return; }
                string awardName = message.Split(':')[0].Trim();
                string description = message.Split(':')[1].Trim();

                if (!Awards.AddAward(awardName, description))
                    Player.SendMessage(p, "This award already exists!");
                else
                    Player.GlobalMessage("Award added: &6" + awardName + " : " + description);
            }
            else
            {
                if (!Awards.RemoveAward(message))
                    Player.SendMessage(p, "This award doesn't exist!");
                else
                    Player.GlobalMessage("Award removed: &6" + message);
            }

            Awards.Save();
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/awardmod <add/del> [award name] : [description]");
            Player.SendMessage(p, "Adds or deletes a reward with the name [award name]");
            Player.SendMessage(p, "&b/awardmod add Bomb joy : Bomb lots of people" + Server.DefaultColor + " is an example");
        }
    }
}
