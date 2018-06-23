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
        public static List<Command> allCmds  = new List<Command>();
        public static List<Command> coreCmds = new List<Command>();
        public static bool IsCore(Command cmd) { return coreCmds.Contains(cmd); }
        public static List<Command> CopyAll() { return new List<Command>(allCmds); }
        
        public static void InitAll() {
            Type[] types = Assembly.GetExecutingAssembly().GetTypes();
            allCmds.Clear();
            coreCmds.Clear();      
            foreach (Group grp in Group.GroupList) { grp.Commands.Clear(); }
            
            for (int i = 0; i < types.Length; i++) {
                Type type = types[i];
                if (!type.IsSubclassOf(typeof(Command)) || type.IsAbstract) continue;
                
                Command cmd = (Command)Activator.CreateInstance(type);
                Register(cmd);
            }
            
            coreCmds = new List<Command>(allCmds);
            IScripting.Autoload();
        }
        
        public static void Register(Command cmd) {
            allCmds.Add(cmd);
            
            CommandPerms perms = CommandPerms.Find(cmd.name);
            if (perms == null) {
                perms = new CommandPerms(cmd.name, cmd.defaultRank, null, null);
                CommandPerms.List.Add(perms);
            }
            foreach (Group grp in Group.GroupList) {
                if (perms.UsableBy(grp.Permission)) grp.Commands.Add(cmd);
            }
            
            CommandPerm[] extra = cmd.ExtraPerms;
            if (extra != null) {
                for (int i = 0; i < extra.Length; i++) {
                    CommandExtraPerms.Set(cmd.name, i + 1, extra[i].Description, 
                                          extra[i].Perm, null, null);
                }
            }
            
            CommandAlias[] aliases = cmd.Aliases;
            if (aliases == null) return;
            foreach (CommandAlias a in aliases) {
                Alias alias = new Alias(a.Trigger, cmd.name, a.Format);
                Alias.coreAliases.Add(alias);
            }
        }
        
        public static Command Find(string name) {
            foreach (Command cmd in allCmds) {
                if (cmd.name.CaselessEq(name)) return cmd;
            }
            return null;
        }
        
        public static bool Unregister(Command cmd) {
            bool removed = allCmds.Remove(cmd);
            foreach (Group grp in Group.GroupList) {
                grp.Commands.Remove(cmd);
            }
            return removed;
        }
        
        public static void Search(ref string cmdName, ref string cmdArgs) {
            Alias alias = Alias.Find(cmdName);
            
            // Aliases override built in command shortcuts
            if (alias == null && cmdName.Length > 0) {
                foreach (Command cmd in allCmds) {
                    if (!cmd.shortcut.CaselessEq(cmdName)) continue;
                    cmdName = cmd.name; return;
                }
                return;
            }
            
            cmdName = alias.Target;
            string format = alias.Format;
            if (format == null) return;
            
            if (format.Contains("{args}")) {
                cmdArgs = format.Replace("{args}", cmdArgs);
            } else {
                cmdArgs = format + " " + cmdArgs;
            }
            cmdArgs = cmdArgs.Trim();
        }
    }
    
    // Kept around for backwards compatibility
    public sealed class CommandList {
        [Obsolete("Use Command.Register() instead")]
        public void Add(Command cmd) { Command.Register(cmd); }
        [Obsolete("Use CommandUnregister() instead")]
        public bool Remove(Command cmd) { return Command.Unregister(cmd); }
        
        [Obsolete("Use Command.Find() instead")]
        public Command FindByName(string name) { return Command.Find(name); }        
        [Obsolete("Use Command.Find() instead")]
        public Command Find(string name) {
            foreach (Command cmd in Command.allCmds) {
                if (cmd.name.CaselessEq(name) || cmd.shortcut.CaselessEq(name)) return cmd;
            }
            return null;
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
        public string Trigger, Format;
        
        public CommandAlias(string cmd, string format = null) {
            Trigger = cmd; Format = format;
        }
    }
}
