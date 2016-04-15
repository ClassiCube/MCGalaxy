/*
	Copyright 2011 MCForge
	
	Written by jordanneil23 with alot of help from TheMusiKid.
		
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
using System.Threading;
namespace MCGalaxy.Commands
{
    public sealed class CmdShutdown : Command
    {
        public override string name { get { return "shutdown"; } }
        public override string shortcut { get { return ""; } }
       public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        public override void Help(Player p) { Player.SendMessage(p, "/shutdown [time] [message] - Shuts the server down"); }
        public override void Use(Player p, string message)
        {
            int secTime = 10;
            bool shutdown = true;
            string file = "stopShutdown";
            if (File.Exists(file)) { File.Delete(file); }
            if (message == "") { message = "Server is going to shutdown in " + secTime + " seconds"; }
            else
            {
                if (message == "cancel") { File.Create(file).Close(); shutdown = false; message = "Shutdown cancelled"; }
                else
                {
                    if (!message.StartsWith("0"))
                    {
                        string[] split = message.Split(' ');
                        bool isNumber = false;
                        try { secTime = Convert.ToInt32(split[0]); isNumber = true; }
                        catch { secTime = 10; isNumber = false; }
                        if (split.Length > 1) { if (isNumber) { message = message.Substring(1 + split[0].Length); } }
                    }
                    else { Player.SendMessage(p, "Countdown time cannot be zero"); return; }
                }
            }
            if (shutdown)
            {
                Player.GlobalMessage("%4" + message);
                Server.s.Log(message);
                for (int t = secTime; t > 0; t = t - 1)
                {
                    if (!File.Exists(file)) { Player.GlobalMessage("%4Server shutdown in " + t + " seconds"); Server.s.Log("Server shutdown in " + t + " seconds"); Thread.Sleep(1000); }
                    else { File.Delete(file); Player.GlobalMessage("Shutdown cancelled"); Server.s.Log("Shutdown cancelled"); return; }
                }
                if (!File.Exists(file)) { MCGalaxy.Gui.App.ExitProgram(false); return; }
                else { File.Delete(file); Player.GlobalMessage("Shutdown cancelled"); Server.s.Log("Shutdown cancelled"); return; }
            }
            return;
        }
    }
}
