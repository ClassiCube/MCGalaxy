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
using System.Collections.Generic;
using MCGalaxy.Commands;

namespace MCGalaxy {
    public sealed class CommandList {
        public List<Command> commands = new List<Command>();
        public bool AddOtherPerms = false;

        public void Add(Command cmd) { 
            commands.Add(cmd);
            CommandPerm[] perms = cmd.ExtraPerms;
            if (!AddOtherPerms || perms == null) return;
            
            for (int i = 0; i < perms.Length; i++)
                CommandExtraPerms.Set(cmd.name, perms[i].Perm, perms[i].Description, i + 1);
        }

        public bool Remove(Command cmd) { return commands.Remove(cmd); }
        public bool Contains(Command cmd) { return commands.Contains(cmd); }
        public bool Contains(string name) { return FindByName(name) != null; }
        
        /// <summary> Finds the command which has the given name or shortcut, or null if not found. </summary>        
        public Command Find(string name) {
            name = name.ToLower();
            foreach (Command cmd in commands) {
                if (cmd.name == name || cmd.shortcut == name) return cmd;
            }
            return null;
        }
        
        /// <summary> Finds the command which has the given name, or null if not found. </summary>
        public Command FindByName(string name) {
            foreach (Command cmd in commands) {
                if (cmd.name.CaselessEq(name)) return cmd;
            }
            return null;
        }

        /// <summary> Finds the command which has the given shortcut, or null if not found. </summary>
        public Command FindByShortcut(string shortcut) {
            if (shortcut == "") return null;
            foreach (Command cmd in commands) {
                if (cmd.shortcut.CaselessEq(shortcut)) return cmd;
            }
            return null;
        }

        public List<Command> All() { return new List<Command>(commands); }
    }
}