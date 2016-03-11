/*
    Copyright 2010 MCLawl Team - Written by Valek (Modified for use with MCGalaxy)
 
    Dual-licensed under the    Educational Community License, Version 2.0 and
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
        public override string type { get { return CommandTypes.Games; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdQueue() { }

        public override void Use(Player p, string message) {
            string[] args = message.Split(' ');
            if (args.Length != 2) { Help(p); return; }
            string value = args[1];
            
            if (args[0] == "zombie") {
                Player who = PlayerInfo.Find(value);
                if (who == null) {
                    p.SendMessage(value + " is not online.");
                } else {
                    p.SendMessage(value + " was queued.");
                    Server.zombie.queZombie = true;
                    Server.zombie.nextZombie = value;
                }
            } else if (args[0] == "level") {
                bool match = false;
                DirectoryInfo di = new DirectoryInfo("levels/");
                FileInfo[] fi = di.GetFiles("*.lvl");
                foreach (FileInfo file in fi) {
                    if (file.Name.Replace(".lvl", "").ToLower() == value.ToLower())
                        match = true;
                }
                
                if (match) {
                    p.SendMessage(value + " was queued.");
                    Server.zombie.queLevel = true;
                    Server.zombie.nextLevel = value.ToLower();
                } else {
                    p.SendMessage("Level does not exist.");
                }
            } else {
                p.SendMessage("You did not enter a valid option.");
            }
        }

        public override void Help(Player p) {
            Player.SendMessage(p, "/queue zombie [name] - Next round [name] will be infected");
            Player.SendMessage(p, "/queue level [name] - Next round [name] will be the round loaded");
        }
    }
}
