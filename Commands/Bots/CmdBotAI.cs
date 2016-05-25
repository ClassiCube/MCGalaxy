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
using MCGalaxy.Bots;

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

            if (!Player.ValidName(ai)) { Player.Message(p, "Invalid AI name!"); return; }
            if (ai == "hunt" || ai == "kill") { Player.Message(p, "Reserved for special AI."); return; }

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
                Player.Message(p, "Could not find specified AI."); return;
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
                        Player.Message(p, "Deleted the last instruction from " + ai);
                        return;
                    } else {
                        Help(p); return;
                    }
                }
            }
            catch (IOException) { currentTry++; goto retry; }
            Player.Message(p, "Deleted &b" + ai);
        }

        void HandleAdd(Player p, string ai, string action = "", string extra = "10", string more = "2") {
            string[] allLines;
            try { allLines = File.ReadAllLines("bots/" + ai); }
            catch { allLines = new string[1]; }

            try {
                if (!File.Exists("bots/" + ai)) {
                    Player.Message(p, "Created new bot AI: &b" + ai);
                    using (StreamWriter w = new StreamWriter("bots/" + ai))
                        w.WriteLine("#Version 2");
                } else if (allLines[0] != "#Version 2") {
                    Player.Message(p, "File found is out-of-date. Overwriting");
                    File.Delete("bots/" + ai);
                    using (StreamWriter w = new StreamWriter("bots/" + ai))
                        w.WriteLine("#Version 2");
                } else {
                    Player.Message(p, "Appended to bot AI: &b" + ai);
                }
            } catch {
                Player.Message(p, "An error occurred when accessing the files. You may need to delete it."); return;
            }

            try {
            	if (action != "reverse") {
            		ScriptFile.Append(p, ai, action, extra, more); return;
            	}
            	
            	using (StreamWriter w = new StreamWriter("bots/" + ai, true)) {
            		for (int i = allLines.Length - 1; i > 0; i--) {
            			if (allLines[i][0] != '#' && allLines[i] != "")
            				w.WriteLine(allLines[i]);
            		}
            		return;
            	}
            } catch { 
                Player.Message(p, "Invalid parameter"); 
            }
        }
        
        void HandleInfo(Player p, string ai) {
            if (!File.Exists("bots/" + ai)) {
                Player.Message(p, "There is no bot AI with that name."); return;
            }
            string[] lines = File.ReadAllLines("bots/" + ai);
            foreach (string l in lines) {
                if (l.Length == 0 || l[0] == '#') continue;
                Player.Message(p, l);
            }
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/botai del [name] %H- deletes that AI");
            Player.Message(p, "%T/botai del [name] last%H- deletes last instruction of that AI");
            Player.Message(p, "%T/botai info [name] %H- prints list of instructions that AI has");
            Player.Message(p, "%T/botai add [name] [instruction] <args>");
            Player.Message(p, "%HInstructions: %Swalk, teleport, wait, nod, speed, " +
                           "spin, reset, remove, reverse, linkscript, jump");
            Player.Message(p, "%HTo see extended help, type %T/help botai [instruction]");
            Player.Message(p, "%Hwait, nod, spin %S- optional arg specifies '0.1 seconds'");
            Player.Message(p, "%Hnod, spin %S- optional second arg specifies 'speed'");
            Player.Message(p, "%Hlinkscript %S- arg specifies another AI name");
        }
        
        public override void Help(Player p, string message) {
            switch (message.ToLower()) {
                case "wait":
                    Player.Message(p, "%T/botai add [name] wait [interval]");
                    Player.Message(p, "%HCauses the bot to stay still for a period of time.");
                    Player.Message(p, "%HNote interval is in 10ths of a second, so an " +
                                   "interval of 20 means stay still for two seconds.");
                    Player.Message(p, "%HIf interval is not given, stays still for one second.");
                    break;
                case "speed":
                    Player.Message(p, "%T/botai add [name] speed [percentage]");
                    Player.Message(p, "%HSets how fast the bot moves, relative to its normal speed.");
                    Player.Message(p, "%H100 means it moves at normal speed, " +
                                   "50 means it moves half as fast as normal");
                    break;
                case "linkscript":
                    Player.Message(p, "%T/botai add [name] linkscript [AI name]");
                    Player.Message(p, "%HCauses the bot to execute all of " +
                                   "the instructions defined in the given AI.");
                    break;
                default:
                    Player.Message(p, "%HInstructions: %Swalk, teleport, wait, nod, speed, " +
                                   "spin, reset, remove, reverse, linkscript, jump");
                    break;
            }
        }
    }
}
