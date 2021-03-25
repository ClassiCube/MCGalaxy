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
using MCGalaxy.Maths;
using MCGalaxy.Scripting;

namespace MCGalaxy {
    
    public abstract partial class Command {
        
        public abstract string name { get; }
        public virtual string shortcut { get { return ""; } }
        public abstract string type { get; }
        public virtual bool museumUsable { get { return true; } }
        public virtual LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        
        public abstract void Use(Player p, string message);
        public virtual void Use(Player p, string message, CommandData data) { Use(p, message); }
        public abstract void Help(Player p);
        public virtual void Help(Player p, string message) { Help(p); Formatter.PrintCommandInfo(p, this); }
        
        public virtual CommandPerm[] ExtraPerms { get { return null; } }
        public virtual CommandEnable Enabled { get { return CommandEnable.Always; } }
        public virtual CommandAlias[] Aliases { get { return null; } }
        
        public virtual bool SuperUseable { get { return true; } }
        public virtual bool MessageBlockRestricted { get { return false; } }
        public virtual bool UseableWhenFrozen { get { return false; } }
        public virtual bool LogUsage { get { return true; } }
        public virtual bool UpdatesLastCmd { get { return true; } }
        
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
            IScripting.AutoloadCommands();
        }
        
        public static void Register(Command cmd) {
            allCmds.Add(cmd);
            
            CommandPerms perms = CommandPerms.GetOrAdd(cmd.name, cmd.defaultRank);
            foreach (Group grp in Group.GroupList) {
                if (perms.UsableBy(grp.Permission)) grp.Commands.Add(cmd);
            }
            
            CommandPerm[] extra = cmd.ExtraPerms;
            if (extra != null) {
                for (int i = 0; i < extra.Length; i++) {
                    CommandExtraPerms exPerms = CommandExtraPerms.GetOrAdd(cmd.name, i + 1, extra[i].Perm);
                    exPerms.Desc = extra[i].Description;
                }
            }           
            Alias.RegisterDefaults(cmd);
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
            
            // typical usage: Command.Unregister(Command.Find("xyz"))
            // So don't throw exception if Command.Find returned null
            if (cmd != null) Alias.UnregisterDefaults(cmd);
            return removed;
        }
        
        public static void Search(ref string cmdName, ref string cmdArgs) {
            if (cmdName.Length == 0) return;
            Alias alias = Alias.Find(cmdName);
            
            // Aliases override built in command shortcuts
            if (alias == null) {
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
    
    public enum CommandContext : byte {
        Normal, Static, SendCmd, Purchase, MessageBlock
    }
    
    public struct CommandData {
        public LevelPermission Rank;
        public CommandContext Context;
        public Vec3S32 MBCoords;
    }
    
    // Clunky design, but needed to stay backwards compatible with custom commands
    public abstract class Command2 : Command {
        public override void Use(Player p, string message) {
            if (p == null) p = Player.Console;
            Use(p, message, p.DefaultCmdData);
        }
    }
    
    // Kept around for backwards compatibility
    public sealed class CommandList {
        [Obsolete("Use Command.Register() instead", true)]
        public void Add(Command cmd) { Command.Register(cmd); }
        [Obsolete("Use Command.Unregister() instead", true)]
        public bool Remove(Command cmd) { return Command.Unregister(cmd); }
        
        [Obsolete("Use Command.Find() instead", true)]
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
