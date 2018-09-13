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

namespace MCGalaxy.Commands {

    /// <summary> Represents which ranks are allowed (and which are disallowed) to use a command. </summary>
    public sealed class CommandPerms : ItemPerms {
        public string CmdName;
        public override string ItemName { get { return CmdName; } }
        
        public CommandPerms(string cmd, LevelPermission min, List<LevelPermission> allowed, 
                            List<LevelPermission> disallowed) : base(min, allowed, disallowed) {
            CmdName = cmd;
        }
        
        public CommandPerms Copy() {
            CommandPerms copy = new CommandPerms(CmdName, 0, null, null);
            CopyTo(copy); return copy;
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
            CommandPerms perms = Find(cmd);            
            if (perms == null) {
                perms = new CommandPerms(cmd, min, allowed, disallowed);
                List.Add(perms);
            } else {
                perms.CmdName = cmd;
                perms.Init(min, allowed, disallowed);
            }
        }
        
        public void MessageCannotUse(Player p) {
            p.Message("Only {0} can use %T/{1}", Describe(), CmdName);
        }


        static readonly object saveLock = new object();
        public static void Save() {
            try {
                lock (saveLock) SaveCore();
            } catch (Exception ex) { 
                Logger.LogError("Error saving " + Paths.CmdPermsFile, ex); 
            }
        }
        
        static void SaveCore() {
            using (StreamWriter w = new StreamWriter(Paths.CmdPermsFile)) {
                WriteHeader(w, "each command", "CommandName", "gun");

                foreach (CommandPerms perms in List) {
                    w.WriteLine(perms.Serialise());
                }
            }
        }
        

        public static void Load() {
            foreach (Command cmd in Command.CopyAll()) {
                Set(cmd.name, cmd.defaultRank, null, null);
            }

            if (File.Exists(Paths.CmdPermsFile)) {
                using (StreamReader r = new StreamReader(Paths.CmdPermsFile)) {
                    ProcessLines(r);
                }
            } else {
                Save();
            }

            foreach (Group grp in Group.GroupList) {
                grp.SetUsableCommands();
            }
        }
        
        static void ProcessLines(StreamReader r) {
            string[] args = new string[4];
            string line;
            
            while ((line = r.ReadLine()) != null) {
                if (line.Length == 0 || line[0] == '#') continue;
                // Format - Name : Lowest : Disallow : Allow
                line.Replace(" ", "").FixedSplit(args, ':');
                
                try {
                    LevelPermission min;
                    List<LevelPermission> allowed, disallowed;
                    
                    Deserialise(args, 1, out min, out allowed, out disallowed);
                    Set(args[0], min, allowed, disallowed);
                } catch {
                    Logger.Log(LogType.Warning, "Hit an error on the command " + line); continue;
                }
            }
        }
    }
}
