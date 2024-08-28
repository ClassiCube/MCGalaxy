﻿/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCForge)
    
    Dual-licensed under the Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    https://opensource.org/license/ecl-2-0/
    https://www.gnu.org/licenses/gpl-3.0.html
    
    Unless required by applicable law or agreed to in writing,
    software distributed under the Licenses are distributed on an "AS IS"
    BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
    or implied. See the Licenses for the specific language governing
    permissions and limitations under the Licenses.
 */
using System;
using System.Collections.Generic;
using System.IO;

namespace MCGalaxy.Commands 
{
    /// <summary> Represents which ranks are allowed (and which are disallowed) to use a command. </summary>
    public sealed class CommandPerms : ItemPerms 
    {
        public string CmdName;
        public override string ItemName { get { return CmdName; } }
        
        static List<CommandPerms> List = new List<CommandPerms>();
        
        
        public CommandPerms(string cmd, LevelPermission min) : base(min) {
            CmdName = cmd;
        }
        
        public CommandPerms Copy() {
            CommandPerms copy = new CommandPerms(CmdName, 0);
            CopyPermissionsTo(copy); return copy;
        }
        
        
        /// <summary> Find the permissions for the given command. (case insensitive) </summary>
        public static CommandPerms Find(string cmd) {
            foreach (CommandPerms perms in List) 
            {
                if (perms.CmdName.CaselessEq(cmd)) return perms;
            }
            return null;
        }

        
        /// <summary> Gets or adds permissions for the given command. </summary>
        public static CommandPerms GetOrAdd(string cmd, LevelPermission min) {
            CommandPerms perms = Find(cmd);
            if (perms != null) return perms;
            
            perms = new CommandPerms(cmd, min);
            List.Add(perms);
            return perms;
        }       
        
        public void MessageCannotUse(Player p) {
            /* p.Message("Only {0} can use &T/{1}", Describe(), CmdName); */
            p.Message("Unknown command \"{0}\".", CmdName); return;
        }


        static readonly object ioLock = new object();
        /// <summary> Saves list of command permissions to disc. </summary>
        public static void Save() {
            try {
                lock (ioLock) SaveCore();
            } catch (Exception ex) {
                Logger.LogError("Error saving " + Paths.CmdPermsFile, ex);
            }
        }
        
        static void SaveCore() {
            using (StreamWriter w = new StreamWriter(Paths.CmdPermsFile)) {
                WriteHeader(w, "command", "each command", "CommandName", "gun", "use");

                foreach (CommandPerms perms in List) 
                {
                    w.WriteLine(perms.Serialise());
                }
            }
        }
        

        /// <summary> Applies new command permissions to server state. </summary>
        public static void ApplyChanges() {
            // does nothing... for now anyways 
            //  (may be required if p.CanUse is changed to instead
            //   use a list of usable commands as a field instead)
        }
        

        /// <summary> Loads list of command permissions from disc. </summary>
        public static void Load() {
            lock (ioLock) LoadCore();
            ApplyChanges();
        }
        
        static void LoadCore() {
            if (!File.Exists(Paths.CmdPermsFile)) { Save(); return; }
            
            using (StreamReader r = new StreamReader(Paths.CmdPermsFile)) {
                 ProcessLines(r);
            }
        }
        
        static void ProcessLines(StreamReader r) {
            string[] args = new string[4];
            CommandPerms perms;
            string line;
            
            while ((line = r.ReadLine()) != null) {
                if (line.IsCommentLine()) continue;
                // Format - Name : Lowest : Disallow : Allow
                line.Replace(" ", "").FixedSplit(args, ':');
                
                try {
                    LevelPermission min;
                    List<LevelPermission> allowed, disallowed;
                    
                    Deserialise(args, 1, out min, out allowed, out disallowed);
                    perms = GetOrAdd(args[0], min);
                    perms.Init(min, allowed, disallowed);
                } catch {
                    Logger.Log(LogType.Warning, "Hit an error on the command " + line); continue;
                }
            }
        }
    }
}
