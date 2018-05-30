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
using System.Text;
using MCGalaxy.Commands;

namespace MCGalaxy.Commands {

    /// <summary> Represents which ranks are allowed (and which are disallowed) to use a command. </summary>
    public class CommandPerms {
        public string CmdName;
        public LevelPermission MinRank;
        public List<LevelPermission> Allowed;
        public List<LevelPermission> Disallowed;
        
        public CommandPerms(string cmd, LevelPermission minRank, List<LevelPermission> allowed, 
                            List<LevelPermission> disallowed) {
            Init(cmd, minRank, allowed, disallowed);
        }
        
        void Init(string cmd, LevelPermission minRank, List<LevelPermission> allowed, 
                  List<LevelPermission> disallowed) {
            CmdName = cmd; MinRank = minRank;
            if (allowed == null) allowed = new List<LevelPermission>();
            if (disallowed == null) disallowed = new List<LevelPermission>();
            Allowed = allowed;
            Disallowed = disallowed;
        }
        
        public CommandPerms Copy() {
            List<LevelPermission> allowed = new List<LevelPermission>(Allowed);
            List<LevelPermission> disallowed = new List<LevelPermission>(Disallowed);
            return new CommandPerms(CmdName, MinRank, allowed, disallowed);
        }
        
        public bool UsableBy(LevelPermission perm) {
             return (perm >= MinRank || Allowed.Contains(perm)) && !Disallowed.Contains(perm);
        }
        
        public static List<CommandPerms> List = new List<CommandPerms>();
        
        
        public static CommandPerms Find(string cmd) {
            foreach (CommandPerms perms in List) {
                if (perms.CmdName.CaselessEq(cmd)) return perms;
            }
            return null;
        }

        public static LevelPermission MinPerm(Command cmd) {
            CommandPerms perms = Find(cmd.name);
            return perms == null ? cmd.defaultRank : perms.MinRank;
        }


        public static void Set(string cmd, LevelPermission min,
                               List<LevelPermission> allowed, List<LevelPermission> disallowed) {
            if (min > LevelPermission.Nobody) return;
            
            // add or replace existing
            CommandPerms perms = Find(cmd);
            if (perms == null) {
                perms = new CommandPerms(cmd, min, allowed, disallowed);
                List.Add(perms);
            } else {
                perms.Init(cmd, min, allowed, disallowed);
            }
        }
        

        public static string JoinPerms(List<LevelPermission> list) {
            if (list == null || list.Count == 0) return "";
            return list.Join(p => ((int)p).ToString(), ",");
        }
 
        public static List<LevelPermission> ExpandPerms(string input) {
            List<LevelPermission> perms = new List<LevelPermission>();
            if (input == null || input.Length == 0) return perms;
            
            foreach (string perm in input.Split(',')) {
                perms.Add((LevelPermission)int.Parse(perm));
            }
            return perms;
        }        
        
        public void MessageCannotUse(Player p) {
            StringBuilder builder = new StringBuilder("Only ");
            Formatter.PrintRanks(MinRank, Allowed, Disallowed, builder);
            builder.Append(" can use %T/" + CmdName);
            Player.Message(p, builder.ToString());
        }


        static readonly object saveLock = new object();
        public static void Save() {
            try {
                lock (saveLock) SaveCore();
            } catch (Exception e) {
                Logger.Log(LogType.Warning, "SAVE FAILED! command.properties");
                Logger.LogError(e);
            }
        }
        
        static void SaveCore() {
            using (StreamWriter w = new StreamWriter(Paths.CmdPermsFile)) {
                w.WriteLine("#Version 2");
                w.WriteLine("#   This file list the ranks that can use each command.");
                w.WriteLine("#   Disallow and allow can be left empty.");
                w.WriteLine("#   Works entirely on rank permission values, not rank names.");
                w.WriteLine("#");
                w.WriteLine("#   Layout: CommandName : LowestRank : Disallow : Allow");
                w.WriteLine("#   gun : 60 : 80,67 : 40,41,55");
                w.WriteLine("");

                foreach (CommandPerms perms in List) {
                    w.WriteLine(perms.CmdName + " : " + (int)perms.MinRank + " : " + JoinPerms(perms.Disallowed) + " : " + JoinPerms(perms.Allowed));
                }
            }
        }
        

        public static void Load() {
            foreach (Command cmd in Command.CopyAll()) {
                Set(cmd.name, cmd.defaultRank, null, null);
            }

            if (File.Exists(Paths.CmdPermsFile)) {
                string[] lines = File.ReadAllLines(Paths.CmdPermsFile);
                ProcessLines(lines);
            } else {
                Save();
            }

            foreach (Group grp in Group.GroupList) {
                grp.SetUsableCommands();
            }
        }
        
        static void ProcessLines(string[] lines) {
            string[] args = new string[4];
            foreach (string line in lines) {
                if (line.Length == 0 || line[0] == '#') continue;
                //Name : Lowest : Disallow : Allow
                line.Replace(" ", "").FixedSplit(args, ':');
                
                try {
                    LevelPermission min = (LevelPermission)int.Parse(args[1]);
                    string disallowRaw = args[2], allowRaw = args[3];
                    
                    List<LevelPermission> allowed = ExpandPerms(allowRaw);
                    List<LevelPermission> disallowed = ExpandPerms(disallowRaw);
                    Set(args[0], min, allowed, disallowed);
                } catch {
                    Logger.Log(LogType.Warning, "Hit an error on the command " + line); continue;
                }
            }
        }
    }
}
