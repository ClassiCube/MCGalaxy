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
            get { return new[] { new CommandPerm(LevelPermission.Operator, "+ can send the changelog to everybody") }; }
        }
        
        public override void Use(Player p, string message)
        {
            if (!File.Exists("changelog.txt"))
            {
                Player.Message(p, "Unable to find changelog");
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
                    Player.Message(p, strArray[j]);
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
                	if (!CheckExtraPerm(p)) { MessageNeedPerms(p, "can send the changelog to all players."); return; }
                    for (int k = 0; k < strArray.Length; k++)
                    {
                        Player.GlobalMessage(strArray[k]);
                    }                   
                    return;
                }
                else
                {
                    Player player = PlayerInfo.FindOrShowMatches(p, split[0]);
                    if (player == null) return;

                    Player.Message(player, "Changelog:");

                    for (int l = 0; l < strArray.Length; l++)
                    {
                        Player.Message(player, strArray[l]);
                    }
                    
                    Player.Message(p, "The Changelog was successfully sent to " + player.name + ".");                 
                    return;
                }
            }
        }
        public override void Help(Player p)
        {
            Player.Message(p, "/changelog - View the most recent changelog!!");
            Player.Message(p, "/changelog <player> - Sends the most recent changelog to <player>!!");
            Player.Message(p, "/changelog all - Sends the most recent changelog to everyone!!");
        }
    }
}
