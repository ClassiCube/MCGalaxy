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
using System.Collections.Generic;
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
            string[] args = message.SplitSpaces();
            if (args[0].CaselessEq("list")) {
                string modifier = args.Length > 1 ? args[1] : "";
                HandleList(p, modifier);
                return;
            }
            
            if (p == null) { MessageInGameOnly(p); return; }
            if (args.Length < 2) { Help(p); return; }
            string ai = args[1].ToLower();

            if (!Formatter.ValidName(p, ai, "bot AI")) return;
            if (ai == "hunt" || ai == "kill") { Player.Message(p, "Reserved for special AI."); return; }

            switch (args[0].ToLower()) {
                case "add": HandleAdd(p, ai, args); break;
                case "del": HandleDelete(p, ai, args); break;
                case "info": HandleInfo(p, ai); break;
                default: Help(p); break;
            }
        }
        
        void HandleDelete(Player p, string ai, string[] args) {
            if (!Directory.Exists("bots/deleted"))
                Directory.CreateDirectory("bots/deleted");
            if (!File.Exists("bots/" + ai)) {
                Player.Message(p, "Could not find specified AI."); return;
            }
            
            for (int attempt = 0; attempt < 10; attempt++) {
                try {
                    if (args.Length == 2) {
                        DeleteAI(p, ai, attempt); return;
                    } else if (args[2].ToLower() == "last") {
                        DeleteLast(p, ai); return;
                    } else {
                        Help(p); return;
                    }
                } catch (IOException) {
                }
            }
        }
        
        static void DeleteAI(Player p, string ai, int attempt) {
            if (attempt == 0) {
                File.Move("bots/" + ai, "bots/deleted/" + ai);
            } else {
                File.Move("bots/" + ai, "bots/deleted/" + ai + attempt);
            }
            Player.Message(p, "Deleted &b" + ai);
        }
        
        static void DeleteLast(Player p, string ai) {
            List<string> lines = Utils.ReadAllLinesList("bots/" + ai);
            if (lines.Count > 0) lines.RemoveAt(lines.Count - 1);

            File.WriteAllLines("bots/" + ai, lines.ToArray());
            Player.Message(p, "Deleted the last instruction from " + ai);
        }

        void HandleAdd(Player p, string ai, string[] args) {
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
                string action = args.Length > 2 ? args[2] : "";
                if (action != "reverse") {
                    ScriptFile.Append(p, ai, action, args); return;
                }
                
                using (StreamWriter w = new StreamWriter("bots/" + ai, true)) {
                    for (int i = allLines.Length - 1; i > 0; i--) {
                        if (allLines[i][0] != '#' && allLines[i] != "")
                            w.WriteLine(allLines[i]);
                    }
                }
            } catch {
                Player.Message(p, "Invalid parameter");
            }
        }
        
        void HandleList(Player p, string modifier) {
            string[] files = Directory.GetFiles("bots");
            for (int i = 0; i < files.Length; i++) {
                files[i] = Path.GetFileNameWithoutExtension(files[i]);
            }
            
            MultiPageOutput.Output(p, files, f => f, "botai list", "bot AIs", modifier, false);
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
            Player.Message(p, "%T/botai list %H- lists all current AIs");
            Player.Message(p, "%T/botai add [name] [instruction] <args>");
            
            Player.Message(p, "%HInstructions: %S{0}, reverse",
                           BotInstruction.Instructions.Join(ins => ins.Name.ToLower()));
            Player.Message(p, "%HTo see detailed help, type %T/help botai [instruction]");
        }
        
        public override void Help(Player p, string message) {
            BotInstruction ins = BotInstruction.Find(message);
            if (ins == null) {
                Player.Message(p, "%HInstructions: %S{0}, reverse",
                               BotInstruction.Instructions.Join(ins2 => ins2.Name.ToLower()));
            } else {
                Player.MessageLines(p, ins.Help);
            }
        }
    }
}
