/*
	Copyright 2011 MCGalaxy
	
	Written by SebbiUltimate
		
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
    public sealed class CmdSendCmd : Command
    {
        public override string name { get { return "sendcmd"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Nobody; } }
        public override void Use(Player p, string message)
        {
            int length = message.Split().Length;
            Player player = null;
            if (length >= 1)
                player = Player.Find(message.Split(' ')[0]);
            else return;
            if (player == null)
            {
                Player.SendMessage(p, "Error: Player is not online.");
            }
            else
            {
                if (p == null) { }
                else { if (player.group.Permission >= p.group.Permission) { Player.SendMessage(p, "Cannot use this on someone of equal or greater rank."); return; } }
                string command;
                string cmdMsg = "";
                try
                {
                    command = message.Split(' ')[1];
                    for(int i = 2; i < length; i++)
                        cmdMsg += message.Split(' ')[i] + " ";
                    cmdMsg.Remove(cmdMsg.Length - 1); //removing the space " " at the end of the msg
                    Command.all.Find(command).Use(player, cmdMsg);
                }
                catch
                {
                    Player.SendMessage(p, "Error: No parameter found");
                    command = message.Split(' ')[1];
                    Command.all.Find(command).Use(player, "");
                }
            }
        }

        public override void Help(Player p)
        {
            Player.SendMessage(p, "/sendcmd - Make another user use a command, (/sendcmd player command parameter)");
            Player.SendMessage(p, "ex: /sendcmd bob tp bob2");
        }
    }
}



