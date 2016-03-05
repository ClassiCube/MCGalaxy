/*
    Copyright 2011 MCGalaxy
        
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
using System.Linq;

namespace MCGalaxy {
    
    /// <summary> These are extra permissions for certain commands </summary>
    public static class CommandOtherPerms {
        
        /// <summary> Restore the permissions to their defaults </summary>
        public static void AddDefaultPerms() { }

        public static List<OtherPerms> list = new List<OtherPerms>();

        public class OtherPerms {
            public Command cmd;
            public int Permission;
            public string Description = "";
            public int number;
        }

        public static int GetPerm(Command cmd, int number = 1) {
            OtherPerms otpe = Find(cmd, number);
            return otpe.Permission;
        }

        public static OtherPerms Find(Command cmd, int number = 1) {
            return list.FirstOrDefault(OtPe => OtPe.cmd == cmd && OtPe.number == number);
        }

        public static void Add(Command command, int Perm, string desc, int number = 1) {
            if (Perm > 120) return;
            OtherPerms otpe = new OtherPerms();
            otpe.cmd = command;
            otpe.Permission = Perm;
            otpe.Description = desc;
            otpe.number = number;
            list.Add(otpe);
        }

        public static void Edit(OtherPerms op, int perm) {
            if (perm > 120) return;
            OtherPerms otpe = op;
            list.Remove(op);
            otpe.Permission = perm;
            list.Add(otpe);
        }

        public static int GetMaxNumber(Command cmd) {
            for (int i = 1; ; i++) {
                if (Find(cmd, i) == null) return (i - 1);
            }
        }

        public static void Save() {
            using (StreamWriter w = new StreamWriter("properties/ExtraCommandPermissions.properties")) {
                w.WriteLine("#     This file is used for setting up additional permissions that are needed in commands!!");
                w.WriteLine("#");
                w.WriteLine("#     LAYOUT:");
                w.WriteLine("#     [commandname]:[additionalpermissionnumber]:[permissionlevel]:[description]");
                w.WriteLine("#     I.E:");
                w.WriteLine("#     countdown:2:80:The lowest rank that can setup countdown (download, start, restart, enable, disable, cancel)");
                w.WriteLine("#");
                w.WriteLine("#     Please also note that descriptions cannot contain ':' and permissions cannot be above 120");
                w.WriteLine("#");
                
                foreach (OtherPerms otpe in list) {
                    try {
                        w.WriteLine(otpe.cmd.name + ":" + otpe.number + ":" + otpe.Permission + ":" + otpe.Description);
                    } catch (Exception ex) {
                        Server.s.Log("Saving an additional command permission failed.");
                        Server.ErrorLog(ex);
                    }
                }
            }
        }

        public static void Load() {
            if (list.Count == 0) { AddDefaultPerms(); }
            if (!File.Exists("properties/ExtraCommandPermissions.properties"))
                Save();
            
            using (StreamReader r = new StreamReader("properties/ExtraCommandPermissions.properties")) {
                string line;
                while ((line = r.ReadLine()) != null) {
                    try {
                        if (!line.StartsWith("#") && line.Contains(':')) {
                            string[] LINE = line.ToLower().Split(':');
                            OtherPerms OTPE = Find(Command.all.Find(LINE[0]), int.Parse(LINE[1]));
                            Edit(OTPE, int.Parse(LINE[2]));
                        }
                    } catch (Exception ex) {
                        Server.s.Log("Loading an additional command permission failed!!");
                        Server.ErrorLog(ex);
                    }
                }
            }
        }
    }
}
