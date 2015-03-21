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
using System.IO;
namespace MCGalaxy.Commands
{
    /// <summary>
    /// This is the command /pload
    /// </summary>
    public sealed class CmdPLoad : Command
    {
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public override bool museumUsable { get { return true; } }
        public override string name { get { return "pload"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return "mod"; } }
        public override void Use(Player p, string message)
        {
            if (File.Exists("plugins/" + message + ".dll"))
                Plugin.Load(message, false);
            else
                Player.SendMessage(p, "Plugin not found!");
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/pload <filename> - Load a plugin in your plugins folder!");
        }
        public CmdPLoad() { }
    }
}
