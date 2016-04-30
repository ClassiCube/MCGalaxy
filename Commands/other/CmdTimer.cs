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
namespace MCGalaxy.Commands
{
    public sealed class CmdTimer : Command
    {
        public override string name { get { return "timer"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdTimer() { }

        public override void Use(Player p, string message)
        {
            if (p.cmdTimer) { Player.SendMessage(p, "Can only have one timer at a time. Use /abort to cancel your previous timer."); return; }

            System.Timers.Timer messageTimer = new System.Timers.Timer(5000);
            if (message == "") { Help(p); return; }

            int TotalTime = 0;
            try
            {
                TotalTime = int.Parse(message.Split(' ')[0]);
                message = message.Substring(message.IndexOf(' ') + 1);
            }
            catch
            {
                TotalTime = 60;
            }

            if (TotalTime > 300) { Player.SendMessage(p, "Cannot have more than 5 minutes in a timer"); return; }

            Chat.GlobalChatLevel(p, "Timer lasting for " + TotalTime + " seconds has started.", false);
            TotalTime = (int)(TotalTime / 5);

            Chat.GlobalChatLevel(p, message, false);

            p.cmdTimer = true;
            messageTimer.Elapsed += delegate
            {
                TotalTime--;
                if (TotalTime < 1 || !p.cmdTimer)
                {
                    Player.SendMessage(p, "Timer ended.");
                    messageTimer.Stop();
                    messageTimer.Dispose();
                }
                else
                {
                    Chat.GlobalChatLevel(p, Server.DefaultColor + message, false);
                    Chat.GlobalChatLevel(p, "Timer has " + (TotalTime * 5) + " seconds remaining.", false);
                }
            };

            messageTimer.Start();
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/timer [time] [message] - Starts a timer which repeats [message] every 5 seconds.");
            Player.SendMessage(p, "Repeats constantly until [time] has passed");
        }
    }
}
