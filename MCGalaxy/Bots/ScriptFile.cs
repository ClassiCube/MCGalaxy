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
using MCGalaxy.Commands;

namespace MCGalaxy.Bots {
    public static class ScriptFile {
        
        public static bool Parse(Player p, PlayerBot bot, string ai) {
            string path = "bots/" + ai;
            if (!File.Exists(path)) {
                p.Message("Could not find specified AI."); return false;
            }

            string[] instructions = File.ReadAllLines(path);
            if (instructions.Length == 0) {
                p.Message("No instructions in the AI."); return false;
            }

            bot.AIName = ai;
            bot.Instructions.Clear();
            bot.cur = 0; bot.countdown = 0; bot.movementSpeed = 3;

            foreach (string line in instructions) {
                if (line.IsCommentLine()) continue;
                string[] args = line.SplitSpaces();

                try {
                    BotInstruction ins = BotInstruction.Find(args[0]);
                    if (ins == null) continue;
                    
                    InstructionData data = ins.Parse(args);
                    data.Name = args[0];
                    bot.Instructions.Add(data);
                } catch {
                    p.Message("AI file corrupt."); return false;
                }
            }
            return true;
        }
        
        public static string Append(Player p, string ai, string cmd, string[] args) {
            using (StreamWriter w = new StreamWriter("bots/" + ai, true)) {
                if (cmd.Length == 0)      cmd = "walk";
                if (cmd.CaselessEq("tp")) cmd = "teleport";

                BotInstruction ins = BotInstruction.Find(cmd);
                if (ins == null) {
                    p.Message("Could not find instruction \"" + cmd + "\""); return null;
                }
                
                CommandExtraPerms killPerms = CommandExtraPerms.Find("BotSet", 1);
                if (ins.Name.CaselessEq("kill") && !killPerms.UsableBy(p.Rank)) {
                    killPerms.MessageCannotUse(p); 
                    return null;
                }
                
                try {
                    ins.Output(p, args, w);
                } catch {
                    p.Message("Invalid arguments given for instruction " + ins.Name);
                    return null;
                }
                return ins.Name;
            }
        }
    }
}