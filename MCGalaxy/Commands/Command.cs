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
using MCGalaxy.Scripting;

namespace MCGalaxy {
    
    public abstract partial class Command {
        
        public abstract string name { get; }
        public virtual string shortcut { get { return ""; } }
        /// <summary> The type/category/group this command falls under.</summary>
        public abstract string type { get; }
        /// <summary> Whether this command can be used in museum maps. </summary>
        public virtual bool museumUsable { get { return true; } }
        /// <summary> The default minimum rank that is able to use this command. </summary>
        public virtual LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        /// <summary> Executes this command. </summary>
        public abstract void Use(Player p, string message);
        /// <summary> Outputs usage information about this command, for when a user does /Help [command]. </summary>
        public abstract void Help(Player p);
        
        /// <summary> Outputs further usage information about this command, for when a user does /Help [command] [message]. </summary>
        /// <remarks> Defaults to just calling Help(p). </remarks>
        public virtual void Help(Player p, string message) { Help(p); Formatter.PrintCommandInfo(p, this); }
        /// <summary> Extra permissions required to use certain aspects of this command. </summary>
        public virtual CommandPerm[] ExtraPerms { get { return null; } }
        public virtual CommandEnable Enabled { get { return CommandEnable.Always; } }
        /// <summary>  Aliases for this command. </summary>
        public virtual CommandAlias[] Aliases { get { return null; } }
        /// <summary> Whether this command can be used by 'super' players. (Console and IRC controllers). </summary>
        public virtual bool SuperUseable { get { return true; } }
        /// <summary> Whether this command is restricted in usage in message blocks. 
        /// Restricted commands require the player to have the extra permission for /mb to be able to be placed in message blocks. </summary>
        public virtual bool MessageBlockRestricted { get { return false; } }
        /// <summary> Whether this command can be used by players who are frozen. </summary>
        public virtual bool UseableWhenFrozen { get { return false; } }
        
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
            IScripting.Autoload();
        }
        
        /// <summary> Modifies the parameters if they match any command shortcut or command alias. </summary>
        public static void Search(ref string cmd, ref string cmdArgs) {
            Alias alias = Alias.Find(cmd);
            // Aliases should be able to override built in shortcuts
            if (alias == null) {
                Command shortcut = all.FindByShortcut(cmd);
                if (shortcut != null) cmd = shortcut.name;
                return;
            }
            
            cmd = alias.Target;
            if (alias.Prefix != null)
                cmdArgs = cmdArgs.Length == 0 ? alias.Prefix : alias.Prefix + " " + cmdArgs;
            if (alias.Suffix != null)
                cmdArgs = cmdArgs.Length == 0 ? alias.Suffix : cmdArgs + " " + alias.Suffix;
        }
    }
    
    [Flags]
    public enum CommandEnable {
        Always = 0, Economy = 1, Zombie = 2, Lava = 4,
    }
}

namespace MCGalaxy.Commands {
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
}
