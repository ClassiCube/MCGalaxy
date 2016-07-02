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
using System.Reflection;
using MCGalaxy.Commands;

namespace MCGalaxy {
    
    public abstract partial class Command {
        
        public abstract string name { get; }
        public abstract string shortcut { get; }
        public abstract string type { get; }
        public abstract bool museumUsable { get; }
        public abstract LevelPermission defaultRank { get; }
        public abstract void Use(Player p, string message);
        public abstract void Help(Player p);
        public virtual void Help(Player p, string message) { Help(p); CmdHelp.PrintCommandInfo(p, this); }
        public virtual CommandPerm[] AdditionalPerms { get { return null; } }
        public virtual CommandEnable Enabled { get { return CommandEnable.Always; } }
        public virtual CommandAlias[] Aliases { get { return null; } }

        public static CommandList all = new CommandList();
        public static CommandList core = new CommandList();
        
        public static void InitAll() {
            all.commands.Clear();
            core.commands.Clear();
            all.AddOtherPerms = true;
            Type[] types = Assembly.GetExecutingAssembly().GetTypes();
            
            for (int i = 0; i < types.Length; i++) {
                Type type = types[i];
                if (!type.IsSubclassOf(typeof(Command)) || type.IsAbstract) continue;                
                Command cmd = (Command)Activator.CreateInstance(type);
                all.Add(cmd);
                
                CommandAlias[] aliases = cmd.Aliases;
                if (aliases == null) continue;                
                foreach (CommandAlias a in aliases) {
                    Alias alias = new Alias(a.Trigger, cmd.name, a.Prefix, a.Suffix);
                    Alias.coreAliases.Add(alias);
                }
            }            
            core.commands = new List<Command>(all.commands);
            Scripting.Autoload();
        }
        
        /// <summary> Modifies the parameters if they match any command shortcut or command alias. </summary>
        public static void Search(ref string cmd, ref string cmdArgs) {
            string shortcut = all.FindShort(cmd);
            if (shortcut != "") cmd = shortcut;
            Alias alias = Alias.Find(cmd);
            if (alias == null) return;
            
            cmd = alias.Target;
            if (alias.Prefix != null)
                cmdArgs = cmdArgs == "" ? alias.Prefix : alias.Prefix + " " + cmdArgs;
            if (alias.Suffix != null)
                cmdArgs = cmdArgs == "" ? alias.Suffix : cmdArgs + " " + alias.Suffix;
        }
    }
    
    public struct CommandPerm {
        public LevelPermission Perm;
        public string Description;
        
        public CommandPerm(LevelPermission perm, string desc) {
            Perm = perm; Description = desc;
        }
    }
    
    public struct CommandAlias {
        public string Trigger, Prefix, Suffix;
        
        public CommandAlias(string cmd, string prefix = null, string suffix = null) {
            Trigger = cmd; Prefix = prefix; Suffix = suffix;
        }
    }
    
    [Flags]
    public enum CommandEnable {
        Always = 0, Economy = 1, Zombie = 2, Lava = 4,
    }
}
