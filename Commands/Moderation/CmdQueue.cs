/*
	Copyright 2010 MCLawl Team - Written by Valek (Modified for use with MCGalaxy)
 
	Dual-licensed under the	Educational Community License, Version 2.0 and
	the GNU General Public License, Version 3 (the "Licenses"); you may
	not use this file except in compliance with the Licenses. You may
	obtain a copy of the Licenses at
	
	http://www.osedu.org/licenses/ECL-2.0
	http://www.gnu.org/licenses/gpl-3.0.html
	
	Unless required by applicable law or agreed to in writing,
	software distributed under the Licenses are distributed on an "AS IS"
	BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
	or implied. See the Licenses for the specific language governing
	permissions and limitations under the Licenses.
*/
using System.IO;
namespace MCGalaxy.Commands
{
    public sealed class CmdQueue : Command
    {
        public override string name { get { return "queue"; } }
        public override string shortcut { get { return "qz"; } }
       public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdQueue() { }

        public override void Use(Player p, string message)
        {
            int number = message.Split(' ').Length;
            if (number > 2) { Help(p); return; }
            if (number == 2)
            {
                Server.s.Log(message);
                string t = message.Split(' ')[0];
                string s = message.Split(' ')[1];
                if (t == "zombie")
                {
                    bool asdfasdf = false;
                    Player.players.ForEach(delegate(Player player)
                    {
                        if (player.name == s)
                        {
                            p.SendMessage(s + " was queued.");
                            Server.queZombie = true;
                            Server.nextZombie = s;
                            asdfasdf = true;
                            return;
                        }
                    });
                    if (!asdfasdf)
                    {
                        p.SendMessage(s + " is not online.");
                        return;
                    }
                }
                else if (t == "level")
                {
                    bool yes = false;
                    DirectoryInfo di = new DirectoryInfo("levels/");
                    FileInfo[] fi = di.GetFiles("*.lvl");
                    foreach (FileInfo file in fi)
                    {
                        if (file.Name.Replace(".lvl", "").ToLower().Equals(s.ToLower()))
                        {
                            yes = true;
                        }
                    }
                    if (yes)
                    {
                        p.SendMessage(s + " was queued.");
                        Server.queLevel = true;
                        Server.nextLevel = s.ToLower();
                        return;
                    }
                    else
                    {
                        p.SendMessage("Level does not exist.");
                        return;
                    }
                }
                else
                {
                    p.SendMessage("You did not enter a valid option.");
                }
            }
        }

        public override void Help(Player p)
        {
            Player.SendMessage(p, "/queue zombie [name] - Next round [name] will be infected");
            Player.SendMessage(p, "/queue level [name] - Next round [name] will be the round loaded");
        }
    }
}
