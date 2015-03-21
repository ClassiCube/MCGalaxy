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
using System;
namespace MCGalaxy.Commands
{
    public sealed class CmdPause : Command
    {
        public override string name { get { return "pause"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return "mod"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdPause() { }

        public override void Use(Player p, string message)
        {
            if (message == "")
            {
                if (p != null)
                {
                    message = p.level.name + " 30";
                }
                else
                {
                    message = Server.mainLevel + " 30";
                }
            }
            int foundNum = 0; Level foundLevel;

            if (message.IndexOf(' ') == -1)
            {
                try
                {
                    foundNum = int.Parse(message);
                    if (p != null)
                    {
                        foundLevel = p.level;
                    }
                    else
                    {
                        foundLevel = Server.mainLevel;
                    }
                }
                catch
                {
                    foundNum = 30;
                    foundLevel = Level.Find(message);
                }
            }
            else
            {
                try
                {
                    foundNum = int.Parse(message.Split(' ')[1]);
                    foundLevel = Level.Find(message.Split(' ')[0]);
                }
                catch
                {
                    Player.SendMessage(p, "Invalid input");
                    return;
                }
            }

            if (foundLevel == null)
            {
                Player.SendMessage(p, "Could not find entered level.");
                return;
            }

            try
            {
                if (foundLevel.physPause)
                {
                    foundLevel.PhysicsEnabled = true;
                    foundLevel.physResume = DateTime.Now;
                    foundLevel.physPause = false;
                    Player.GlobalMessage("Physics on " + foundLevel.name + " were re-enabled.");
                }
                else
                {
                    foundLevel.PhysicsEnabled  = false;
                    foundLevel.physResume = DateTime.Now.AddSeconds(foundNum);
                    foundLevel.physPause = true;
                    Player.GlobalMessage("Physics on " + foundLevel.name + " were temporarily disabled.");

                    foundLevel.physTimer.Elapsed += delegate
                    {
                        if (DateTime.Now > foundLevel.physResume)
                        {
                            foundLevel.physPause = false;
                            try
                            {
                                foundLevel.PhysicsEnabled = true;
                            }
                            catch (Exception e) { Server.ErrorLog(e); }
                            Player.GlobalMessage("Physics on " + foundLevel.name + " were re-enabled.");
                            foundLevel.physTimer.Stop();
                            foundLevel.physTimer.Dispose();
                        }
                    };
                    foundLevel.physTimer.Start();
                }
            }
            catch (Exception e)
            {
                Server.ErrorLog(e);
            }
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/pause [map] [amount] - Pauses physics on [map] for 30 seconds");
        }
    }
}
