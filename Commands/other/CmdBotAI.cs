/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
    Dual-licensed under the Educational Community License, Version 2.0 and
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
    public sealed class CmdBotAI : Command
    {
        public override string name { get { return "botai"; } }
        public override string shortcut { get { return "bai"; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public CmdBotAI() { }

        public override void Use(Player p, string message) {
            string[] args = message.Split(' ');
            if (p == null) { MessageInGameOnly(p); return; }
            if (args.Length < 2) { Help(p); return; }
            string ai = args[1].ToLower();

            if (!Player.ValidName(ai)) { Player.SendMessage(p, "Invalid AI name!"); return; }
            if (ai == "hunt" || ai == "kill") { Player.SendMessage(p, "Reserved for special AI."); return; }

            switch (args[0].ToLower()) {
                case "add":
                    if (args.Length == 2) HandleAdd(p, ai);
                    else if (args.Length == 3) HandleAdd(p, ai, args[2]);
                    else if (args.Length == 4) HandleAdd(p, ai, args[2], args[3]);
                    else HandleAdd(p, ai, args[2], args[3], args[4]);
                    break;
                case "del":
                    HandleDelete(p, ai, args);
                    break;
                case "info":
                    HandleInfo(p, ai);
                    break;
                default:
                    Help(p);
                    return;
            }
        }
        
        void HandleDelete(Player p, string ai, string[] args) {
            if (!Directory.Exists("bots/deleted")) Directory.CreateDirectory("bots/deleted");

            int currentTry = 0;
            if (!File.Exists("bots/" + ai)) {
                Player.SendMessage(p, "Could not find specified AI."); return;
            }
            
            retry:
            try {
                if (args.Length == 2) {
                    if (currentTry == 0)
                        File.Move("bots/" + ai, "bots/deleted/" + ai);
                    else
                        File.Move("bots/" + ai, "bots/deleted/" + ai + currentTry);
                } else {
                    if (args[2].ToLower() == "last") {
                        string[] Lines = File.ReadAllLines("bots/" + ai);
                        string[] outLines = new string[Lines.Length - 1];
                        for (int i = 0; i < Lines.Length - 1; i++) {
                            outLines[i] = Lines[i];
                        }

                        File.WriteAllLines("bots/" + ai, outLines);
                        Player.SendMessage(p, "Deleted the last instruction from " + ai);
                        return;
                    } else {
                        Help(p); return;
                    }
                }
            }
            catch (IOException) { currentTry++; goto retry; }
            Player.SendMessage(p, "Deleted &b" + ai);
        }

        void HandleAdd(Player p, string ai, string action = "", string extra = "10", string more = "2") {
            string[] allLines;
            try { allLines = File.ReadAllLines("bots/" + ai); }
            catch { allLines = new string[1]; }

            StreamWriter SW;
            try {
                if (!File.Exists("bots/" + ai)) {
                    Player.SendMessage(p, "Created new bot AI: &b" + ai);
                    using (SW = File.CreateText("bots/" + ai))
                    {
                        SW.WriteLine("#Version 2");
                    }
                } else if (allLines[0] != "#Version 2") {
                    Player.SendMessage(p, "File found is out-of-date. Overwriting");
                    File.Delete("bots/" + ai);
                    using (SW = File.CreateText("bots/" + ai))
                    {
                        SW.WriteLine("#Version 2");
                    }
                } else {
                    Player.SendMessage(p, "Appended to bot AI: &b" + ai);
                }
            } catch { 
                Player.SendMessage(p, "An error occurred when accessing the files. You may need to delete it."); return; 
            }

            try {
                using (SW = File.AppendText("bots/" + ai))
                {
                    switch (action.ToLower())
                    {
                        case "":
                        case "walk":
                            SW.WriteLine("walk " + p.pos[0] + " " + p.pos[1] + " " + p.pos[2] + " " + p.rot[0] + " " + p.rot[1]);
                            break;
                        case "teleport":
                        case "tp":
                            SW.WriteLine("teleport " + p.pos[0] + " " + p.pos[1] + " " + p.pos[2] + " " + p.rot[0] + " " + p.rot[1]);
                            break;
                        case "wait":
                            SW.WriteLine("wait " + int.Parse(extra)); break;
                        case "nod":
                            SW.WriteLine("nod " + int.Parse(extra) + " " + int.Parse(more)); break;
                        case "speed":
                            SW.WriteLine("speed " + int.Parse(extra)); break;
                        case "remove":
                            SW.WriteLine("remove"); break;
                        case "reset":
                            SW.WriteLine("reset"); break;
                        case "spin":
                            SW.WriteLine("spin " + int.Parse(extra) + " " + int.Parse(more)); break;
                        case "reverse":
                            for (int i = allLines.Length - 1; i > 0; i--) if (allLines[i][0] != '#' && allLines[i] != "") SW.WriteLine(allLines[i]);
                            break;
                        case "linkscript":
                            if (extra != "10") SW.WriteLine("linkscript " + extra); else Player.SendMessage(p, "Linkscript requires a script as a parameter");
                            break;
                        case "jump":
                            SW.WriteLine("jump"); break;
                        default:
                            Player.SendMessage(p, "Could not find \"" + action + "\""); break;
                    }
                }
            }
            catch { Player.SendMessage(p, "Invalid parameter"); }
        }
        
        void HandleInfo(Player p, string ai) {
            if (!File.Exists("bots/" + ai)) {
                Player.SendMessage(p, "There is no bot AI with that name."); return;
            }
            string[] lines = File.ReadAllLines("bots/" + ai);
            foreach (string l in lines) {
                if (l.Length == 0 || l[0] == '#') continue;
                Player.SendMessage(p, l);
            }
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "%T/botai del [name] %H- deletes that AI");
            Player.SendMessage(p, "%T/botai del [name] last%H- deletes last instruction of that AI");
            Player.SendMessage(p, "%T/botai info [name] %H- prints list of instructions that AI has");
            Player.SendMessage(p, "%T/botai add [name] [instruction] <args>");
            Player.SendMessage(p, "%HInstructions: %Swalk, teleport, wait, nod, speed, " +
                               "spin, reset, remove, reverse, linkscript, jump");
            Player.SendMessage(p, "%Hwait, nod, spin %S- optional arg specifies '0.1 seconds'");
            Player.SendMessage(p, "%Hnod, spin %S- optional second arg specifies 'speed'");
            Player.SendMessage(p, "%Hspeed %S- arg specifies percentage of normal speed");
            Player.SendMessage(p, "%Hlinkscript %S- arg specifies another AI name");
        }
    }
}
