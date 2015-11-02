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
using System;
using System.IO;
namespace MCGalaxy.Commands
{
    public sealed class CmdCmdCreate : Command
    {
        public override string name { get { return "cmdcreate"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        public CmdCmdCreate() { }

        public override void Use(Player p, string message)
        {

            if (message == "")
            {
                Help(p);
                return;
            }
            string[] name = message.Split(' ');


            if (name.Length == 1)
            {
                if (File.Exists("extra/commands/source/Cmd" + message + ".cs")) { p.SendMessage("File Cmd" + message + ".cs already exists.  Choose another name."); return; }
                try
                {
                    Scripting.CreateNew(message);
                }
                catch (Exception e)
                {
                    Server.ErrorLog(e);
                    Player.SendMessage(p, "An error occurred creating the class file.");
                    return;
                }
                Player.SendMessage(p, "Successfully created a new command class.");
                return;
            }

            if (name[1] == "vb")
            {
                if (File.Exists("extra/commands/source/Cmd" + name[0] + ".vb")) { p.SendMessage("File Cmd" + name[0] + ".vb already exists.  Choose another name."); return; }
                try
                {
                    ScriptingVB.CreateNew(name[0]);
                }
                catch (Exception e)
                {
                    Server.ErrorLog(e);
                    Player.SendMessage(p, "An error occurred creating the class file.");
                    return;
                }
                Player.SendMessage(p, "Successfully created a new vb command class.");
                return;
            }
            // else if (name.Length > 2) { Help(p); return; }

        }

        public override void Help(Player p)
        {
            Player.SendMessage(p, "/cmdcreate <message> - Creates a dummy command class named Cmd<Message> from which you can make a new command.");
            Player.SendMessage(p, "Or use \"/cmdcreate <name of command> vb\" to create a dummy class in visual basic");
        }
    }
}
