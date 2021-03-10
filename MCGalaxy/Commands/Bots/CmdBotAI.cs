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

namespace MCGalaxy.Commands.Bots{
    public sealed class CmdBotAI : Command2 {
        public override string name { get { return "BotAI"; } }
        public override string shortcut { get { return "bai"; } }
        public override string type { get { return CommandTypes.Other; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public override bool SuperUseable { get { return false; } }

        public override void Use(Player p, string message, CommandData data) {
            string[] args = message.SplitSpaces();
            string cmd = args[0];
            if (IsListCommand(cmd)) {
                string modifier = args.Length > 1 ? args[1] : "";
                HandleList(p, modifier);
                return;
            }
            
            if (args.Length < 2) { Help(p); return; }
            string ai = args[1].ToLower();

            if (!Formatter.ValidName(p, ai, "bot AI")) return;
            if (ai == "hunt" || ai == "kill") { p.Message("Reserved for special AI."); return; }

            if (IsCreateCommand(cmd)) {
                HandleAdd(p, ai, args);
            } else if (IsDeleteCommand(cmd)) {
                HandleDelete(p, ai, args);
            } else if (IsInfoCommand(cmd)) {
                HandleInfo(p, ai);
            } else {
                Help(p);
            }
        }
        
        void HandleDelete(Player p, string ai, string[] args) {
            if (!Directory.Exists("bots/deleted"))
                Directory.CreateDirectory("bots/deleted");
            if (!File.Exists("bots/" + ai)) {
                p.Message("Could not find specified bot AI."); return;
            }
            
            for (int attempt = 0; attempt < 10; attempt++) {
                try {
                    if (args.Length == 2) {
                        DeleteAI(p, ai, attempt); return;
                    } else if (args[2].CaselessEq("last")) {
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
            p.Message("Deleted bot AI &b" + ai);
        }
        
        static void DeleteLast(Player p, string ai) {
            List<string> lines = Utils.ReadAllLinesList("bots/" + ai);
            if (lines.Count > 0) lines.RemoveAt(lines.Count - 1);

            File.WriteAllLines("bots/" + ai, lines.ToArray());
            p.Message("Deleted last instruction from bot AI &b" + ai);
        }

        void HandleAdd(Player p, string ai, string[] args) {
            if (!File.Exists("bots/" + ai)) {
                p.Message("Created new bot AI: &b" + ai);
                using (StreamWriter w = new StreamWriter("bots/" + ai)) {
                    // For backwards compatibility
                    w.WriteLine("#Version 2");
                }
            }

            string action = args.Length > 2 ? args[2] : "";
            string instruction = ScriptFile.Append(p, ai, action, args);
            if (instruction != null) {
                p.Message("Appended " + instruction + " instruction to bot AI &b" + ai);
            }
        }
        
        void HandleList(Player p, string modifier) {
            string[] files = Directory.GetFiles("bots");
            MultiPageOutput.Output(p, files, f => Path.GetFileName(f),
                                   "BotAI list", "bot AIs", modifier, false);
        }
        
        void HandleInfo(Player p, string ai) {
            if (!File.Exists("bots/" + ai)) {
                p.Message("There is no bot AI with that name."); return;
            }
            string[] lines = File.ReadAllLines("bots/" + ai);
            foreach (string line in lines) {
                if (line.IsCommentLine()) continue;
                p.Message(line);
            }
        }
        
        public override void Help(Player p) {
            p.Message("&T/BotAI del [name] &H- deletes that AI");
            p.Message("&T/BotAI del [name] last&H- deletes last instruction of that AI");
            p.Message("&T/BotAI info [name] &H- prints list of instructions that AI has");
            p.Message("&T/BotAI list &H- lists all current AIs");
            p.Message("&T/BotAI add [name] [instruction] <args>");
            
            p.Message("&HInstructions: &S{0}",
                      BotInstruction.Instructions.Join(ins => ins.Name));
            p.Message("&HTo see detailed help, type &T/Help BotAI [instruction]");
        }
        
        public override void Help(Player p, string message) {
            BotInstruction ins = BotInstruction.Find(message);
            if (ins == null) {
                p.Message("&HInstructions: &S{0}, reverse",
                               BotInstruction.Instructions.Join(ins2 => ins2.Name));
            } else {
                p.MessageLines(ins.Help);
            }
        }
    }
}
