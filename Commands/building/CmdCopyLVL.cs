/*
    Written by Jack1312

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
using System;
using System.IO;
namespace MCGalaxy.Commands
{
    public class CmdCopyLVL : Command
    {
        public override string name { get { return "copylvl"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdCopyLVL() { }

        public override void Use(Player p, string message)
        {
            string msg1 = String.Empty;
            string msg2 = String.Empty;
            try
            {
                msg1 = message.Split(' ')[0];
                msg2 = message.Split(' ')[1];
            }
            catch { }
              
            if (!p.group.CanExecute(Command.all.Find("newlvl")))
            {
                Player.SendMessage(p, "You cannot use this command, unless you can use /newlvl!");
                return;
            }
            int number = message.Split(' ').Length;
            if (message == "")
            {
                Help(p);
                return;
            }
            if (number < 2)
            {
                Player.SendMessage(p, "You did not specify the level it would be copied to as!");
                return;
            }
            try {
                File.Copy("levels/" + msg1 + ".lvl", "levels/" + msg2 + ".lvl");
                File.Copy("levels/level properties/" + msg1 + ".properties", "levels/level properties/" + msg1 + ".properties", false);

            } catch (System.IO.FileNotFoundException) {
                Player.SendMessage(p, msg2 + " does not exist!");
                return;

            } catch (System.IO.IOException) {
                Player.SendMessage(p, "The level, &c" + msg2 + " &e already exists!");
                return;

            } catch (System.ArgumentException) {
                Player.SendMessage(p, "One or both level names are either invalid, or corrupt.");
                return;

            }
            Player.SendMessage(p, "The level, &c" + msg1 + " &ehas been copied to &c" + msg2 + "!");
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/copylvl [Level] [Copied Level] - Makes a copy of [Level] called [Copied Level].");
        }
    }
}
