/*
	Copyright 2011 MCForge
	
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
using System.Linq;
namespace MCGalaxy.Commands
{
    public sealed class CmdChangeLog : Command
    {
        public override string name { get { return "changelog"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public override CommandPerm[] AdditionalPerms {
            get { return new[] { new CommandPerm(LevelPermission.Operator, "The lowest rank that can send the changelog to everybody") }; }
        }
        
        public override void Use(Player p, string message)
        {
            if (!File.Exists("changelog.txt"))
            {
                Player.SendMessage(p, "Unable to find changelog");
                return;
            }

            // Read the changelog but stop reading if it encounters a blank line
            // This is done so that a player will only see the latest changes even if multiple version info exists in the changelog
            // Because of this, its really important that blank lines are ONLY used to separate different versions
            string[] strArray = File.ReadAllLines("changelog.txt").TakeWhile(s => !String.IsNullOrEmpty(s.Trim())).ToArray();
            if (message == "")
            {
                for (int j = 0; j < strArray.Length; j++)
                {
                    Player.SendMessage(p, strArray[j]);
                }
            }
            else
            {
                string[] split = message.Split(' ');
                if(split.Length != 1)
                {
                    Help(p);
                    return;
                }

                if (split[0] == "all")
                {
                    if ((int)p.group.Permission < CommandOtherPerms.GetPerm(this))
                    {
                        Player.SendMessage(p, "You must be at least " + Group.findPermInt(CommandOtherPerms.GetPerm(this)).name + " to send the changelog to all players.");
                        return;
                    }
                    for (int k = 0; k < strArray.Length; k++)
                    {
                        Player.GlobalMessage(strArray[k]);
                    }
                    
                    return;
                }
                else
                {
                    Player player = PlayerInfo.Find(split[0]);
                    if (player == null)
                    {
                        Player.SendMessage(p, "Could not find player \"" + split[0] + "\"!");
                        return;
                    }

                    Player.SendMessage(player, "Changelog:");

                    for (int l = 0; l < strArray.Length; l++)
                    {
                        Player.SendMessage(player, strArray[l]);
                    }
                    
                    Player.SendMessage(p, "The Changelog was successfully sent to " + player.name + ".");
                    
                    return;
                }
            }
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/changelog - View the most recent changelog!!");
            Player.SendMessage(p, "/changelog <player> - Sends the most recent changelog to <player>!!");
            Player.SendMessage(p, "/changelog all - Sends the most recent changelog to everyone!!");
        }
    }
}
