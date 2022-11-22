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

namespace MCGalaxy.Commands 
{    
    /// <summary> Represents additional permissions required to perform a special action in a command. </summary>
    /// <remarks> For example, /color command has an extra permission for changing the color of other players. </remarks>
    public class CommandExtraPerms : ItemPerms 
    {
        public string CmdName, Desc = "";
        public int Num;
        public override string ItemName { get { return CmdName + ":" + Num; } }
        
        static List<CommandExtraPerms> list = new List<CommandExtraPerms>();
        
        
        public CommandExtraPerms(string cmd, int num, string desc, LevelPermission min) : base(min) {
            CmdName = cmd; Num = num; Desc = desc;
        }
        
        public CommandExtraPerms Copy() {
            CommandExtraPerms copy = new CommandExtraPerms(CmdName, Num, Desc, 0);
            CopyPermissionsTo(copy); return copy;
        }
        
        
        public static CommandExtraPerms Find(string cmd, int num) {
            foreach (CommandExtraPerms perms in list) 
            {
                if (perms.CmdName.CaselessEq(cmd) && perms.Num == num) return perms;
            }
            return null;
        }
        
        public static List<CommandExtraPerms> FindAll(string cmd) {
            List<CommandExtraPerms> all = new List<CommandExtraPerms>();
            foreach (CommandExtraPerms perms in list) 
            {
                if (perms.CmdName.CaselessEq(cmd) && perms.Desc.Length > 0) all.Add(perms);
            }
            return all;
        }
        
        
        /// <summary> Gets or adds the nth extra permission for the given command. </summary>
        public static CommandExtraPerms GetOrAdd(string cmd, int num, LevelPermission min) {
            CommandExtraPerms perms = Find(cmd, num);
            if (perms != null) return perms;
            
            perms = new CommandExtraPerms(cmd, num, "", min);
            list.Add(perms);
            return perms;
        }
              
        public void MessageCannotUse(Player p) {
            p.Message("Only {0} {1}", Describe(), Desc);
        }
        

        static readonly object ioLock = new object();      
        /// <summary> Saves list of extra permissions to disc. </summary>
        public static void Save() {
            try {
                lock (ioLock) SaveCore();
            } catch (Exception ex) {
                Logger.LogError("Error saving " + Paths.CmdExtraPermsFile, ex);
            }
        }        
        
        static void SaveCore() {
            using (StreamWriter w = new StreamWriter(Paths.CmdExtraPermsFile)) {
                WriteHeader(w, "extra permissions in some commands",
                            "CommandName:ExtraPermissionNumber", "countdown:1");
                
                foreach (CommandExtraPerms perms in list) {
                    w.WriteLine(perms.Serialise());
                }
            }
        }
        

        /// <summary> Loads list of extra permissions to disc. </summary>
        public static void Load() {
            lock (ioLock) {
                if (!File.Exists(Paths.CmdExtraPermsFile)) Save();
                
                using (StreamReader r = new StreamReader(Paths.CmdExtraPermsFile)) {
                    ProcessLines(r);
                }
            }
        }
        
        static void ProcessLines(StreamReader r) {
            string[] args = new string[5];
            CommandExtraPerms perms;
            string line;
            
            while ((line = r.ReadLine()) != null) {
                if (line.IsCommentLine() || line.IndexOf(':') == -1) continue;
                // Format - Name:Num : Lowest : Disallow : Allow
                line.Replace(" ", "").FixedSplit(args, ':');
                
                try {
                    LevelPermission min;
                    List<LevelPermission> allowed, disallowed;
                    
                    // Old format - Name:Num : Lowest : Description
                    if (IsDescription(args[3])) {
                        min = (LevelPermission)int.Parse(args[2]);
                        allowed = null; disallowed = null;
                    } else {
                        Deserialise(args, 2, out min, out allowed, out disallowed);
                    }
                    
                    perms = GetOrAdd(args[0], int.Parse(args[1]), min);
                    perms.Init(min, allowed, disallowed);
                } catch (Exception ex) {
                    Logger.Log(LogType.Warning, "Hit an error on the extra command perms " + line);
                    Logger.LogError(ex);
                }
            }
        }
        
        static bool IsDescription(string arg) {
            foreach (char c in arg) 
            {
                if (c >= 'a' && c <= 'z') return true;
            }
            return false;
        }
    }
}
