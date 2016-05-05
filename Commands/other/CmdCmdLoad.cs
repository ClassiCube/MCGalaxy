/*
	Copyright 2010 MCLawl Team - Written by Valek (Modified for use with MCGalaxy)
 
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
    public sealed class CmdCmdLoad : Command
    {
        public override string name { get { return "cmdload"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Nobody; } }

        public override void Use(Player p, string message)
        {

            if (Command.all.Contains(message.Split(' ')[0]))
            {
                Player.Message(p, "That command is already loaded!");
                return;
            }

            string[] param = message.Split(' ');
            string name = "Cmd" + param[0];


            if (param.Length == 1)
            {
                string error = Scripting.Load(name);
                if (error != null)
                {
                    Player.Message(p, error);
                    return;
                }
                GrpCommands.fillRanks();
                Player.Message(p, "Command was successfully loaded.");
                return;
            } else if (param[1] == "vb") {

                string error = ScriptingVB.Load(name);
                if (error != null)
                {
                    Player.Message(p, error);
                    return;
                }
                GrpCommands.fillRanks();
                Player.Message(p, "Command was successfully loaded.");
                return;
            }
        }


        public override void Help(Player p)
        {
            Player.Message(p, "/cmdload <command name> - Loads a C# command into the server for use.");
            Player.Message(p, "/cmdload <command name> vb - Loads a Visual basic command into the server for use.");
        }
    }
}
