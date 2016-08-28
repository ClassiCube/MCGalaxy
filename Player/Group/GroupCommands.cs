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

namespace MCGalaxy {
    public class GrpCommands {
        
        public class rankAllowance {
            public string commandName;
            public LevelPermission lowestRank;
            public List<LevelPermission> disallow = new List<LevelPermission>();
            public List<LevelPermission> allow = new List<LevelPermission>();
        }
        public static List<rankAllowance> allowedCommands;

        public static LevelPermission defaultRanks(string command) {
            Command cmd = Command.all.Find(command);
            return cmd != null ? cmd.defaultRank : LevelPermission.Null;
        }
        
        public static LevelPermission MinPerm(Command cmd) {
            var perms = GrpCommands.allowedCommands.Find(C => C.commandName == cmd.name);
            return perms == null ? cmd.defaultRank : perms.lowestRank;
        }
        
        public static bool Remove(string cmdName) {
            for (int i = 0; i < allowedCommands.Count; i++) {
                if (allowedCommands[i].commandName != cmdName) continue;
                allowedCommands.RemoveAt(i); return true;
            }
            return false;
        }

        public static void fillRanks() {
            List<string> cmdNames = Command.all.commandNames();
            allowedCommands = new List<rankAllowance>();

            rankAllowance allowVar;
            foreach (Command cmd in Command.all.All()) {
                allowVar = new rankAllowance();
                allowVar.commandName = cmd.name;
                allowVar.lowestRank = cmd.defaultRank;
                allowedCommands.Add(allowVar);
            }

            if (File.Exists("properties/command.properties")) {
                string[] lines = File.ReadAllLines("properties/command.properties");
                //if (lines.Length == 0) ; // this is useless?
                if (lines[0] == "#Version 2") ReadVersion2(lines, cmdNames);
                else ReadVersion1(lines, cmdNames);
            } else {
                Save(allowedCommands);
            }

            foreach (Group grp in Group.GroupList)
                grp.fillCommands();
        }
        
        static void ReadVersion2(string[] lines, List<string> cmdNames) {
            string[] colon = new[] { " : " };
            foreach (string line in lines) {
                if (line == "" || line[0] == '#') continue;
                rankAllowance perms = new rankAllowance();
                //Name : Lowest : Disallow : Allow
                string[] args = line.Split(colon, StringSplitOptions.None);

                if (!cmdNames.Contains(args[0])) {
                    Server.s.Log("Incorrect command name: " + args[0]); continue;
                }
                perms.commandName = args[0];

                string[] disallow = new string[0];
                if (args[2] != "") disallow = args[2].Split(',');
                string[] allow = new string[0];
                if (args[3] != "") allow = args[3].Split(',');

                try {
                    perms.lowestRank = (LevelPermission)int.Parse(args[1]);
                    foreach (string s in disallow) { perms.disallow.Add((LevelPermission)int.Parse(s)); }
                    foreach (string s in allow) { perms.allow.Add((LevelPermission)int.Parse(s)); }
                } catch {
                    Server.s.Log("Hit an error on the command " + line); continue;
                }

                for (int i = 0; i < allowedCommands.Count; i++) {
                    if (args[0] == allowedCommands[i].commandName) {
                        allowedCommands[i] = perms; break;
                    }
                }
            }
        }
        
        static void ReadVersion1(string[] lines, List<string> cmdNames) {
            foreach (string line in lines) {
                if (line == "" || line[0] == '#') continue;
                rankAllowance perms = new rankAllowance();
                string key = line.Split('=')[0].Trim().ToLower();
                string value = line.Split('=')[1].Trim().ToLower();

                if (!cmdNames.Contains(key)) {
                    Server.s.Log("Incorrect command name: " + key);
                } else if (Level.PermissionFromName(value) == LevelPermission.Null) {
                    Server.s.Log("Incorrect value given for " + key + ", using default value.");
                } else{
                    perms.commandName = key;
                    perms.lowestRank = Level.PermissionFromName(value);

                    for (int i = 0; i < allowedCommands.Count; i++) {
                        if (allowedCommands[i].commandName == key) {
                            allowedCommands[i] = perms; break;
                        }
                    }
                }
            }
        }

        static readonly object saveLock = new object();
        public static void Save(List<rankAllowance> givenList) {
            lock (saveLock)
                SaveCore(givenList);
        }
        
        static void SaveCore(List<rankAllowance> givenList) {
            try {
                using (StreamWriter w = new StreamWriter("properties/command.properties")) {
                    w.WriteLine("#Version 2");
                    w.WriteLine("#   This file contains a reference to every command found in the server software");
                    w.WriteLine("#   Use this file to specify which ranks get which commands");
                    w.WriteLine("#   Current ranks: " + Group.concatList(false, false, true));
                    w.WriteLine("#   Disallow and allow can be left empty, just make sure there's 2 spaces between the colons");
                    w.WriteLine("#   This works entirely on permission values, not names. Do not enter a rank name. Use its permission value");
                    w.WriteLine("#   CommandName : LowestRank : Disallow : Allow");
                    w.WriteLine("#   gun : 60 : 80,67 : 40,41,55");
                    w.WriteLine("");

                    foreach (rankAllowance aV in givenList) {
                        w.WriteLine(aV.commandName + " : " + (int)aV.lowestRank + " : " + getInts(aV.disallow) + " : " + getInts(aV.allow));
                    }
                }
            } catch {
                Server.s.Log("SAVE FAILED! command.properties");
            }
        }
        
        public static string getInts(List<LevelPermission> givenList) {
            if (givenList == null) return "";
            return givenList.Join(p => ((int)p).ToString(), ",");
        }
        
        public static void AddCommands(out CommandList commands, LevelPermission perm) {
            commands = new CommandList();
            foreach (rankAllowance perms in allowedCommands) {
                bool canUse = perms.lowestRank <= perm && !perms.disallow.Contains(perm);
                if (canUse || perms.allow.Contains(perm))                
                    commands.Add(Command.all.Find(perms.commandName));
            }
        }
    }
}
