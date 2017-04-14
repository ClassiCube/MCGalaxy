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

namespace MCGalaxy {
    
    /// <summary> Holds extra permissions for some commands. </summary>
    public static class CommandExtraPerms {

        static List<ExtraPerms> list = new List<ExtraPerms>();
        
        /// <summary> Represents additional permissions required to perform a special action in a command. </summary>
        /// <remarks> For example, /color command has an extra permission for changing the color of other players. </remarks>
        public class ExtraPerms {
            
            /// <summary> Name of the command this extra permission is for. </summary>
            public string Command;
            
            /// <summary> Minimum rank required by a player to have this extra permission. </summary>
            public LevelPermission MinRank;
            
            /// <summary> Describes the action that having this extra permission allows a player to perform. </summary>
            public string Description = "";
            
            /// <summary> The number / identifier of this extra permission. </summary>
            public int Number;
        }

        
        /// <summary> Finds the nth extra permission for a given command. </summary>
        /// <returns> null if the nth extra permission does not exist. </returns>
        public static ExtraPerms Find(string cmd, int num = 1) {
            foreach (ExtraPerms perms in list) {
                if (perms.Command.CaselessEq(cmd) && perms.Number == num) return perms;
            }
            return null;
        }

        /// <summary> Returns the lowest rank that has the nth extra permission for a given command. </summary>
        /// <returns> defPerm if the nth extra permission does not exist, otherwise the lowest rank. </returns>
        public static LevelPermission MinPerm(string cmd, LevelPermission defPerm, int num = 1) {
            ExtraPerms perms = Find(cmd, num);
            return perms == null ? defPerm : perms.MinRank;
        }

        /// <summary> Returns the lowest rank that has the nth extra permission for a given command. </summary>
        public static LevelPermission MinPerm(string cmd, int num = 1) { return Find(cmd, num).MinRank; }
        

        /// <summary> Sets the nth extra permission for a given command. </summary>
        public static void Set(string cmd, LevelPermission perm, string desc, int num = 1) {
            if (perm > LevelPermission.Nobody) return;
            
            // add or replace existing
            ExtraPerms perms = Find(cmd, num);
            if (perms == null) {
                perms = new ExtraPerms();
                list.Add(perms);
            }
            
            perms.Command = cmd;
            perms.MinRank = perm;
            perms.Description = desc;
            perms.Number = num;
        }

        /// <summary> Returns the highest number/identifier of an extra permission for the given command. </summary>
        public static int GetMaxNumber(Command cmd) {
            for (int i = 1; ; i++) {
                if (Find(cmd.name, i) == null) return (i - 1);
            }
        }
        

        static readonly object saveLock = new object();
        
        /// <summary> Saves the list of all extra permissions. </summary>
        public static void Save() {
            lock (saveLock) {
                try {
                    SaveCore();
                } catch (Exception ex) {
                    Server.s.Log("Saving properties/ExtraCommandPermissions.properties failed.");
                    Server.ErrorLog(ex);
                }
            }
        }
        
        const string file = "properties/ExtraCommandPermissions.properties";
        static void SaveCore() {
            using (StreamWriter w = new StreamWriter(file)) {
                w.WriteLine("#     This file sets extra permissions used in some commands.");
                w.WriteLine("#");
                w.WriteLine("#     LAYOUT: [commandname]:[additionalpermissionnumber]:[permissionlevel]:[description]");
                w.WriteLine("#     e.g.  : countdown:2:80:Lowest rank that can setup countdown (download, start, restart, enable, disable, cancel)");
                w.WriteLine("#");
                w.WriteLine("#     Note descriptions cannot contain ':', and permissions cannot be above 120");
                w.WriteLine("#");
                
                foreach (ExtraPerms perms in list) {
                	w.WriteLine(perms.Command + ":" + perms.Number + ":" + (int)perms.MinRank + ":" + perms.Description);
                }
            }
        }

        /// <summary> Loads the list of all extra permissions. </summary>
        public static void Load() {
            if (!File.Exists(file)) Save();
            
            using (StreamReader r = new StreamReader(file)) {
                string line;
                while ((line = r.ReadLine()) != null) {
                    if (line.Length == 0 || line[0] == '#' || line.IndexOf(':') == -1) continue;
                    
                    try {
                        string[] parts = line.Split(':');
                        LoadExtraPerm(parts);
                    } catch (Exception ex) {
                        Server.s.Log("Loading an additional command permission failed!!");
                        Server.ErrorLog(ex);
                    }
                }
            }
        }
        
        static void LoadExtraPerm(string[] parts) {
            string cmdName = parts[0];
            int number = int.Parse(parts[1]), minPerm = int.Parse(parts[2]);
            string desc = parts.Length > 3 ? parts[3] : "";
            
            ExtraPerms existing = Find(cmdName, number);
            if (existing != null) desc = existing.Description;
            Set(cmdName, (LevelPermission)minPerm, desc, number);
        }
    }
}
