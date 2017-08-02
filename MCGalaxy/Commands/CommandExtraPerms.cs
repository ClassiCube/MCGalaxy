/*
    Copyright 2011 MCForge
        
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
    
    /// <summary> Represents additional permissions required to perform a special action in a command. </summary>
    /// <remarks> For example, /color command has an extra permission for changing the color of other players. </remarks>
    public class CommandExtraPerms {

        /// <summary> Name of the command this extra permission is for. </summary>
        public string CmdName;
        
        /// <summary> Minimum rank required by a player to have this extra permission. </summary>
        public LevelPermission MinRank;
        
        /// <summary> Describes the action that having this extra permission allows a player to perform. </summary>
        public string Description = "";
        
        /// <summary> The number / identifier of this extra permission. </summary>
        public int Number;
        
        
        /// <summary> Creates a copy of this instance. </summary>
        public CommandExtraPerms Copy() {
            CommandExtraPerms perms = new CommandExtraPerms();
            perms.CmdName = CmdName;
            perms.MinRank = MinRank;
            perms.Description = Description;
            perms.Number = Number;
            return perms;
        }
        
        static List<CommandExtraPerms> list = new List<CommandExtraPerms>();
        
        
        /// <summary> Finds the nth extra permission for a given command. </summary>
        /// <returns> null if the nth extra permission does not exist. </returns>
        public static CommandExtraPerms Find(string cmd, int num = 1) {
            foreach (CommandExtraPerms perms in list) {
                if (perms.CmdName.CaselessEq(cmd) && perms.Number == num) return perms;
            }
            return null;
        }

        /// <summary> Returns the lowest rank that has the nth extra permission for a given command. </summary>
        /// <returns> defPerm if the nth extra permission does not exist, otherwise the lowest rank. </returns>
        public static LevelPermission MinPerm(string cmd, LevelPermission defPerm, int num = 1) {
            CommandExtraPerms perms = Find(cmd, num);
            return perms == null ? defPerm : perms.MinRank;
        }

        /// <summary> Returns the lowest rank that has the nth extra permission for a given command. </summary>
        public static LevelPermission MinPerm(string cmd, int num = 1) { return Find(cmd, num).MinRank; }
        
        /// <summary> Finds all extra permissions for a given command. </summary>
        /// <remarks> list is empty when no extra permissions are found, not null. </remarks>
        public static List<CommandExtraPerms> FindAll(string cmd) {
            List<CommandExtraPerms> allPerms = new List<CommandExtraPerms>();
            foreach (CommandExtraPerms perms in list) {
                if (perms.CmdName.CaselessEq(cmd)) allPerms.Add(perms);
            }
            return allPerms;
        }
        

        /// <summary> Sets the nth extra permission for a given command. </summary>
        public static void Set(string cmd, LevelPermission perm, string desc, int num = 1) {
            if (perm > LevelPermission.Nobody) return;
            
            // add or replace existing
            CommandExtraPerms perms = Find(cmd, num);
            if (perms == null) {
                perms = new CommandExtraPerms(); list.Add(perms);
            }
            
            perms.CmdName = cmd;
            perms.MinRank = perm;
            perms.Description = desc;
            perms.Number = num;
        }
        

        static readonly object ioLock = new object();
        
        /// <summary> Saves the list of all extra permissions. </summary>
        public static void Save() {
            lock (ioLock) {
                try {
                    SaveCore();
                } catch (Exception ex) {
                    Logger.Log(LogType.Warning, "Saving {0} failed.", Paths.CmdExtraPermsFile);
                    Logger.LogError(ex);
                }
            }
        }
        
        
        static void SaveCore() {
            using (StreamWriter w = new StreamWriter(Paths.CmdExtraPermsFile)) {
                w.WriteLine("#   This file sets extra permissions used in some commands.");
                w.WriteLine("#   Note descriptions cannot contain ':', and permissions cannot be above 120");
                w.WriteLine("#");
                w.WriteLine("#   LAYOUT: [commandname]:[additionalpermissionnumber]:[permissionlevel]:[description]");
                w.WriteLine("#   e.g.  : countdown:2:80:Lowest rank that can setup countdown (download, start, restart, enable, disable, cancel)");
                w.WriteLine("");
                
                foreach (CommandExtraPerms perms in list) {
                    w.WriteLine(perms.CmdName + ":" + perms.Number + ":" + (int)perms.MinRank + ":" + perms.Description);
                }
            }
        }
        

        /// <summary> Loads the list of all extra permissions. </summary>
        public static void Load() {
            lock (ioLock) {
                if (!File.Exists(Paths.CmdExtraPermsFile)) Save();
                
                LoadCore();
            }
        }
        
        static void LoadCore() {
            string[] args = new string[4];
            using (StreamReader r = new StreamReader(Paths.CmdExtraPermsFile)) {
                string line;
                while ((line = r.ReadLine()) != null) {
                    if (line.Length == 0 || line[0] == '#' || line.IndexOf(':') == -1) continue;
                    
                    try {
                        line.FixedSplit(args, ':');
                        LoadExtraPerm(args);
                    } catch (Exception ex) {
                        Logger.Log(LogType.Warning, "Loading an additional command permission failed!!");
                        Logger.LogError(ex);
                    }
                }
            }
        }
        
        static void LoadExtraPerm(string[] args) {
            string cmdName = args[0];
            int number = int.Parse(args[1]), minPerm = int.Parse(args[2]);
            string desc = args[3] == null ? "" : args[3];
            
            CommandExtraPerms existing = Find(cmdName, number);
            if (existing != null) desc = existing.Description;
            Set(cmdName, (LevelPermission)minPerm, desc, number);
        }
    }
}
