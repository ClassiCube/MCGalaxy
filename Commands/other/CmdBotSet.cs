/*
	Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
	
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
    public sealed class CmdBotSet : Command
    {
        public override string name { get { return "botset"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public override CommandPerm[] AdditionalPerms {
            get { return new[] { new CommandPerm(LevelPermission.Operator, "The lowest rank that can set the bot to killer") }; }
        }

        public override void Use(Player p, string message)
        {
            if (message == "") { Help(p); return; }

            try
            {
                if (message.Split(' ').Length == 1)
                {
                    PlayerBot pB = PlayerBot.Find(message);
                    try { pB.Waypoints.Clear(); }
                    catch { }
                    pB.kill = false;
                    pB.hunt = false;
                    pB.AIName = "";
                    Player.SendMessage(p, pB.color + pB.name + Server.DefaultColor + "'s AI was turned off.");
                    Server.s.Log(pB.name + "'s AI was turned off.");
                    return;
                }
                else if (message.Split(' ').Length != 2)
                {
                    Help(p); return;
                }

                PlayerBot Pb = PlayerBot.Find(message.Split(' ')[0]);
                if (Pb == null) { Player.SendMessage(p, "Could not find specified Bot"); return; }
                string foundPath = message.Split(' ')[1].ToLower();

                if (foundPath == "hunt")
                {
                    Pb.hunt = !Pb.hunt;
                    try { Pb.Waypoints.Clear(); }
                    catch { }
                    Pb.AIName = "";
                    if (p != null) Chat.GlobalChatLevel(p, Pb.color + Pb.name + Server.DefaultColor + "'s hunt instinct: " + Pb.hunt, false);
                    Server.s.Log(Pb.name + "'s hunt instinct: " + Pb.hunt);
                    return;
                }
                else if (foundPath == "kill")
                {
                	if (CheckAdditionalPerm(p)) { MessageNeedPerms(p, "can toggle a bot's killer instinct."); return; }
                    Pb.kill = !Pb.kill;
                    if (p != null) Chat.GlobalChatLevel(p, Pb.color + Pb.name + Server.DefaultColor + "'s kill instinct: " + Pb.kill, false);
                    Server.s.Log(Pb.name + "'s kill instinct: " + Pb.kill);
                    return;
                }

                if (!File.Exists("bots/" + foundPath)) { Player.SendMessage(p, "Could not find specified AI."); return; }

                string[] foundWay = File.ReadAllLines("bots/" + foundPath);

                if (foundWay[0] != "#Version 2") { Player.SendMessage(p, "Invalid file version. Remake"); return; }

                PlayerBot.Pos newPos = new PlayerBot.Pos();
                try { Pb.Waypoints.Clear(); Pb.currentPoint = 0; Pb.countdown = 0; Pb.movementSpeed = 12; }
                catch { }

                try
                {
                    foreach (string s in foundWay)
                    {
                        if (s != "" && s[0] != '#')
                        {
                            bool skip = false;
                            newPos.type = s.Split(' ')[0];
                            switch (s.Split(' ')[0].ToLower())
                            {
                                case "walk":
                                case "teleport":
                                    newPos.x = Convert.ToUInt16(s.Split(' ')[1]);
                                    newPos.y = Convert.ToUInt16(s.Split(' ')[2]);
                                    newPos.z = Convert.ToUInt16(s.Split(' ')[3]);
                                    newPos.rotx = Convert.ToByte(s.Split(' ')[4]);
                                    newPos.roty = Convert.ToByte(s.Split(' ')[5]);
                                    break;
                                case "wait":
                                case "speed":
                                    newPos.seconds = Convert.ToInt16(s.Split(' ')[1]); break;
                                case "nod":
                                case "spin":
                                    newPos.seconds = Convert.ToInt16(s.Split(' ')[1]);
                                    newPos.rotspeed = Convert.ToInt16(s.Split(' ')[2]);
                                    break;
                                case "linkscript":
                                    newPos.newscript = s.Split(' ')[1]; break;
                                case "reset":
                                case "jump":
                                case "remove": break;
                                default: skip = true; break;
                            }
                            if (!skip) Pb.Waypoints.Add(newPos);
                        }
                    }
                }
                catch { Player.SendMessage(p, "AI file corrupt."); return; }

                Pb.AIName = foundPath;
                if (p != null) Chat.GlobalChatLevel(p, Pb.color + Pb.name + Server.DefaultColor + "'s AI is now set to " + foundPath, false);
                Server.s.Log(Pb.name + "'s AI was set to " + foundPath);
            }
            catch { Player.SendMessage(p, "Error"); return; }
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/botset <bot> <AI script> - Makes <bot> do <AI script>");
            Player.SendMessage(p, "Special AI scripts: Kill and Hunt");
        }
    }
}
