/* 
	Copyright 2011 MCGalaxy
	
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
using System.Collections.Generic;
using System.IO;
namespace MCGalaxy.Commands
{
    public sealed class CmdFaq : Command
    {
        public override string name { get { return "faq"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return "information"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public CmdFaq() { }

        public override void Use(Player p, string message)
        {
            List<string> faq = new List<string>();
            if (!File.Exists("text/faq.txt"))
            {
                File.WriteAllText("text/faq.txt", "Example: What does this server run on? This server runs on &bMCGalaxy");
            }
			using (StreamReader r = File.OpenText("text/faq.txt"))
			{
				while (!r.EndOfStream)
					faq.Add(r.ReadLine());

			}

            Player who = null;
            if (message != "")
            {
                if ((int)p.group.Permission < CommandOtherPerms.GetPerm(this))
                { Player.SendMessage(p, "You can't send the FAQ to another player!"); return; }
                who = Player.Find(message);
            }
            else
            {
                who = p;
            }

            if (who != null)
            {
                who.SendMessage("&cFAQ&f:");
                foreach (string s in faq)
                    who.SendMessage("&f" + s);
            }
            else
            {
                Player.SendMessage(p, "There is no player \"" + message + "\"!");
            }
        }

        public override void Help(Player p)
        {
            Player.SendMessage(p, "/faq [player]- Displays frequently asked questions");
        }
    }
}
