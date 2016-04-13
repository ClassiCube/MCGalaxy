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

namespace MCGalaxy {
    
    public abstract partial class Command {
        
        public abstract string name { get; }
        public abstract string shortcut { get; }
        public abstract string type { get; }
        public abstract bool museumUsable { get; }
        public abstract LevelPermission defaultRank { get; }
        public abstract void Use(Player p, string message);
        public abstract void Help(Player p);
        public virtual CommandPerm[] AdditionalPerms { get { return null; } }
        public virtual CommandEnable Enabled { get { return CommandEnable.Always; } }
        public virtual CommandAlias[] Aliases { get { return null; } }

        public static CommandList all = new CommandList();
        public static CommandList core = new CommandList();
        
        public static void InitAll() {
            all.AddOtherPerms = true;
            Type[] types = Assembly.GetExecutingAssembly().GetTypes();
            for (int i = 0; i < types.Length; i++) {
                Type type = types[i];
                if (!type.IsSubclassOf(typeof(Command)) || type.IsAbstract) continue;                
                Command cmd = (Command)Activator.CreateInstance(type);
                all.Add(cmd);
                
                CommandAlias[] aliases = cmd.Aliases;
                if (aliases == null) continue;                
                foreach (CommandAlias alias in aliases) {
                    string target = cmd.name + " " + alias.Args;
                    Alias.coreAliases.Add(new Alias(alias.Trigger, target));
                }
            }            
            core.commands = new List<Command>(all.commands);
            Scripting.Autoload();
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
        public string Trigger, Args;
        
        public CommandAlias(string cmd) {
            Trigger = cmd; Args = "";
        }
        
        public CommandAlias(string cmd, string args) {
            Trigger = cmd; Args = args;
        }
    }
    
    [Flags]
    public enum CommandEnable {
        Always = 0, Economy = 1, Zombie = 2, Lava = 4,
    }
}
