/*
    Copyright 2015 MCGalaxy
        
    Dual-licensed under the Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    https://opensource.org/license/ecl-2-0/
    https://www.gnu.org/licenses/gpl-3.0.html
    
    Unless required by applicable law or agreed to in writing,
    software distributed under the Licenses are distributed on an "AS IS"
    BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
    or implied. See the Licenses for the specific language governing
    permissions and limitations under the Licenses.
 */
using System;
using System.Collections.Generic;

namespace MCGalaxy.Commands {

    /// <summary>
    /// Represents the name, behavior, and help text for a subcommand. Used with SubCommandGroup to offer a variety of subcommands to run based on user input.
    /// </summary>
    public class SubCommand 
    {
        public delegate void Behavior(Player p, string arg);
        
        public readonly string Name;
        public readonly Behavior behavior;
        string[] Help;
        readonly bool MapOnly;
        string[] Aliases;

        /// <summary>
        /// When mapOnly is true, the subcommand can only be used when the player is the realm owner.
        /// </summary>
        public SubCommand(string name, Behavior behavior, string[] help, bool mapOnly = true, string[] aliases = null) {
            Name = name;
            this.behavior = behavior;
            Help = help;
            MapOnly = mapOnly;
            Aliases = aliases;
        }

        public bool Match(string cmd) {
            if (Aliases != null) {
                foreach (string alias in Aliases) 
                {
                    if (alias.CaselessEq(cmd)) return true;
                }
            }
            return Name.CaselessEq(cmd);
        }
        
        public bool AnyMatchingAlias(SubCommand other) {
            if (Aliases != null) {
                foreach (string alias in Aliases) 
                {
                    if (other.Match(alias)) return true;
                }
            }
            return other.Match(Name);
        }
        
        public bool Allowed(Player p, string parentCommandName) {
            if (MapOnly && !LevelInfo.IsRealmOwner(p.level, p.name)) {
                p.Message("You may only use &T/{0} {1}&S after you join your map.", parentCommandName, Name.ToLower());
                return false;
            }
            return true;
        }
        
        public void DisplayHelp(Player p) {
            if (Help == null || Help.Length == 0) {
                p.Message("No help is available for {0}", Name);
                return;
            }
            p.MessageLines(Help);
        }
    }

    /// <summary>
    /// Represents a group of SubCommands that can be called from a given parent command. SubCommands can be added or removed using Register and Unregister.
    /// </summary>
    public class SubCommandGroup 
    {
        public enum UsageResult { NoneFound, Success, Disallowed }

        public readonly string parentCommandName;
        List<SubCommand> subCommands;

        public SubCommandGroup(string parentCmd, List<SubCommand> initialCmds) {
            parentCommandName = parentCmd;
            subCommands = initialCmds;
        }

        public void Register(SubCommand subCmd) {
            foreach (SubCommand sub in subCommands) 
            {
                if (subCmd.AnyMatchingAlias(sub)) {
                    throw new ArgumentException(
                        String.Format("One or more aliases of the existing subcommand \"{0}\" conflicts with the subcommand \"{1}\" that is being registered.",
                        sub.Name, subCmd.Name));
                }
            }
            subCommands.Add(subCmd);
        }

        public void Unregister(SubCommand subCmd) {
            subCommands.Remove(subCmd);
        }

        public UsageResult Use(Player p, string message, bool alertNoneFound = true) {
            string[] args = message.SplitExact(2);
            string cmd    = args[0];

            foreach (SubCommand subCmd in subCommands) 
            {
                if (!subCmd.Match(cmd)) { continue; }
                if (!subCmd.Allowed(p, parentCommandName)) { return UsageResult.Disallowed; }

                subCmd.behavior(p, args[1]);
                return UsageResult.Success;
            }
            
            if (alertNoneFound) {
                p.Message("There is no {0} command \"{1}\".", parentCommandName, message);
                p.Message("See &T/help {0}&S for all {0} commands.", parentCommandName);
            }
            return UsageResult.NoneFound;
        }

        public void DisplayAvailable(Player p) {
            p.Message("&HCommands: &S{0}", subCommands.Join(grp => grp.Name));
            p.Message("&HUse &T/Help {0} [command] &Hfor more details", parentCommandName);
        }

        public void DisplayHelpFor(Player p, string subCmdName) {
            foreach (SubCommand subCmd in subCommands) 
            {
                if (!subCmd.Match(subCmdName)) { continue; }
                subCmd.DisplayHelp(p);
                return;
            }
            p.Message("There is no {0} command {1} to display help for.", parentCommandName, subCmdName);
        }
    }
}
