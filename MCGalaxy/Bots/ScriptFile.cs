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
        
        public static bool Parse(Player p, PlayerBot bot, string file) {
            if (!File.Exists(file)) { 
                Player.Message(p, "Could not find specified AI."); return false; 
            }

            string[] instructions = File.ReadAllLines(file);
            if (instructions.Length == 0 || !instructions[0].CaselessEq("#Version 2")) {
                Player.Message(p, "Invalid file version. Remake"); return false; 
            }

            bot.Instructions.Clear();
            bot.cur = 0; bot.countdown = 0; bot.movementSpeed = 3;

            foreach (string line in instructions) {
                if (line.Length == 0 || line[0] == '#') continue;
                string[] args = line.SplitSpaces();

                try {
                    BotInstruction ins = BotInstruction.Find(args[0]);
                    if (ins == null) continue;
                    
                    InstructionData data = ins.Parse(args);
                    data.Name = args[0];
                    bot.Instructions.Add(data);
                } catch {
                    Player.Message(p, "AI file corrupt."); return false;
                }
            }
            return true;
        }
        
        public static void Append(Player p, string ai, string action, string[] args) {
            using (StreamWriter w = new StreamWriter("bots/" + ai, true)) {
                if (action.Length == 0) action = "walk";
                if (action.CaselessEq("tp")) action = "teleport";

                BotInstruction ins = BotInstruction.Find(action);
                if (ins == null) {
                    Player.Message(p, "Could not find instruction \"" + action + "\""); return;
                }
                
                LevelPermission killPerm = CommandExtraPerms.MinPerm("botset");
                if (ins.Name.CaselessEq("kill") && p.Rank < killPerm) {
                    Formatter.MessageNeedMinPerm(p, "+ can toggle a bot's killer instinct.", killPerm);
                    return;
                }
                ins.Output(p, args, w);
            }
        }
    }
}