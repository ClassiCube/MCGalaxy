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
namespace MCGalaxy.Commands
{
    public sealed class CmdLimit : Command
    {
        public override string name { get { return "limit"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return "mod"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        public CmdLimit() { }

        public override void Use(Player p, string message)
        {
            if (message.Split(' ').Length != 2) { Help(p); return; }
            int newLimit;
            try { newLimit = int.Parse(message.Split(' ')[1]); }
            catch { Player.SendMessage(p, "Invalid limit amount"); return; }
            if (newLimit < 1) { Player.SendMessage(p, "Cannot set below 1."); return; }

            Group foundGroup = Group.Find(message.Split(' ')[0]);
            if (foundGroup != null)
            {
                foundGroup.maxBlocks = newLimit;
                Player.GlobalMessage(foundGroup.color + foundGroup.name + Server.DefaultColor + "'s building limits were set to &b" + newLimit);
                Group.saveGroups(Group.GroupList);
            }
            else
            {
                switch (message.Split(' ')[0].ToLower())
                {
                    case "rp":
                    case "restartphysics":
                        Server.rpLimit = newLimit;
                        Player.GlobalMessage("Custom /rp's limit was changed to &b" + newLimit.ToString());
                        break;
                    case "rpnorm":
                    case "rpnormal":
                        Server.rpNormLimit = newLimit;
                        Player.GlobalMessage("Normal /rp's limit was changed to &b" + newLimit.ToString());
                        break;

                    default:
                        Player.SendMessage(p, "No supported /limit");
                        break;
                }
            }
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/limit <type> <amount> - Sets the limit for <type>");
            Player.SendMessage(p, "<types> - " + Group.concatList(true, true) + ", RP, RPNormal");
        }
    }
}