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
    /// <summary> These are extra permissions for certain commands </summary>
    public static class CommandOtherPerms {
        
        /// <summary> Restore the permissions to their defaults </summary>
        public static void AddDefaultPerms() { }

        public static List<OtherPerms> list = new List<OtherPerms>();

        public class OtherPerms {
            public string cmd;
            public int Permission;
            public string Description = "";
            public int number;
        }

        public static int GetPerm(Command cmd, int number = 1) {
            OtherPerms otpe = Find(cmd, number);
            return otpe.Permission;
        }

        public static OtherPerms Find(Command cmd, int number = 1) { return Find(cmd.name, number); }
        
        public static OtherPerms Find(string cmd, int number = 1) {
            foreach (OtherPerms perms in list) {
                if (perms.cmd == cmd && perms.number == number) return perms;
            }
            return null;
        }
        
        public static LevelPermission FindPerm(string cmd, LevelPermission defPerm, int number = 1) {
            OtherPerms perms = Find(cmd, number);
            return perms == null ? defPerm : (LevelPermission)perms.Permission;
        }

        public static void Add(Command command, int Perm, string desc, int number = 1) {
            if (Perm > 120) return;
            OtherPerms otpe = new OtherPerms();
            otpe.cmd = command.name;
            otpe.Permission = Perm;
            otpe.Description = desc;
            otpe.number = number;
            list.Add(otpe);
        }

        [Obsolete("This method is completely unnecessary")]
        public static void Edit(OtherPerms op, int perm) { op.Permission = perm; }

        public static int GetMaxNumber(Command cmd) {
            for (int i = 1; ; i++) {
                if (Find(cmd, i) == null) return (i - 1);
            }
        }

        static readonly object saveLock = new object();
        public static void Save() {
            lock (saveLock)
                SaveCore();
        }
        
        const string file = "properties/ExtraCommandPermissions.properties";
        static void SaveCore() {
            using (StreamWriter w = new StreamWriter(file)) {
                w.WriteLine("#     This file is used for setting up additional permissions that are needed in commands!!");
                w.WriteLine("#");
                w.WriteLine("#     LAYOUT:");
                w.WriteLine("#     [commandname]:[additionalpermissionnumber]:[permissionlevel]:[description]");
                w.WriteLine("#     I.E:");
                w.WriteLine("#     countdown:2:80:Lowest rank that can setup countdown (download, start, restart, enable, disable, cancel)");
                w.WriteLine("#");
                w.WriteLine("#     Please also note that descriptions cannot contain ':' and permissions cannot be above 120");
                w.WriteLine("#");
                
                foreach (OtherPerms perms in list) {
                    try {
                        w.WriteLine(perms.cmd + ":" + perms.number + ":" + perms.Permission + ":" + perms.Description);
                    } catch (Exception ex) {
                        Server.s.Log("Saving an additional command permission failed.");
                        Server.ErrorLog(ex);
                    }
                }
            }
        }

        public static void Load() {
            if (list.Count == 0) { AddDefaultPerms(); }
            if (!File.Exists(file)) Save();
            
            using (StreamReader r = new StreamReader(file)) {
                string line;
                while ((line = r.ReadLine()) != null) {
                    if (line.Length == 0 || line[0] == '#' || line.IndexOf(':') == -1) continue;
                    try {
                        string[] parts = line.ToLower().Split(':');
                        OtherPerms perms = Find(parts[0], int.Parse(parts[1]));
                        if (perms == null) continue; // command has no additional perms, so skip
                        perms.Permission = int.Parse(parts[2]);
                    } catch (Exception ex) {
                        Server.s.Log("Loading an additional command permission failed!!");
                        Server.ErrorLog(ex);
                    }
                }
            }
        }
    }
}
