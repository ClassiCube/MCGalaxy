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
using System.Collections.Generic;

namespace MCGalaxy.Commands {

    /// <summary>
    /// Represents the name, behavior, and help text for a subcommand that is called from under the umbrella of a broader parent command.
    /// Pass a List of subcommands to SubCommand.UseSubCommands in order to execute the appropriate subcommand based on user input.
    /// </summary>
    public class SubCommand {
        public delegate void SubCommandHandler(Player p, string cmd, string value);
        public enum UsageResult { NoneFound, Success, Disallowed }

        public readonly string Group;
        readonly SubCommandHandler Handler;
        string[] Help;
        readonly bool MapOnly;
        string[] Aliases;

        /// <summary>
        /// When mapOnly is true, the subcommand can only be used when the player is the realm owner.
        /// </summary>
        public SubCommand(string grpName, SubCommandHandler handler, string[] help, bool mapOnly = true, string[] aliases = null) {
            Group = grpName;
            Handler = handler;
            Help = help;
            MapOnly = mapOnly;
            Aliases = aliases;
        }
        bool Match(string cmd) {
            if (Aliases != null) {
                foreach (string alias in Aliases) {
                    if (alias.CaselessEq(cmd)) { return true; }
                }
            }
            return Group.CaselessEq(cmd);
        }
        public bool AnyMatchingAlias(SubCommand other) {
            if (Aliases != null) {
                foreach (string alias in Aliases) {
                    if (other.Match(alias)) { return true; }
                }
            }
            return other.Match(Group);
        }
        bool Allowed(Player p, string parentCommandName) {
            if (MapOnly && !LevelInfo.IsRealmOwner(p.level, p.name)) {
                p.Message("You may only use &T/{0} {1}&S after you join your map.", parentCommandName, Group.ToLower());
                return false;
            }
            return true;
        }
        void DisplayHelp(Player p) {
            if (Help == null || Help.Length == 0) {
                p.Message("No help is available for {0}", Group);
                return;
            }
            p.MessageLines(Help);
        }


        public static UsageResult UseSubCommands(Player p, string message, string parentCommandName, List<SubCommand> subCommands, bool alertNoneFound = true) {
            string[] args = message.SplitSpaces(3);
            string cmd = args[0];
            string arg1 = args.Length > 1 ? args[1] : "";
            string arg2 = args.Length > 2 ? args[2] : "";

            foreach (SubCommand subCmd in subCommands) {
                if (!subCmd.Match(cmd)) { continue; }
                if (!subCmd.Allowed(p, parentCommandName)) { return UsageResult.Disallowed; }
                subCmd.Handler(p, arg1, arg2);
                return UsageResult.Success;
            }
            if (alertNoneFound) {
                p.Message("There is no {0} command \"{1}\".", parentCommandName, message);
                p.Message("See &T/help {0}&S for all {0} commands.", parentCommandName);
            }
            return UsageResult.NoneFound;
        }
        public static void HelpSubCommands(Player p, string message, string parentCommandName, List<SubCommand> subCommands) {
            foreach (SubCommand subCmd in subCommands) {
                if (!subCmd.Match(message)) { continue; }
                subCmd.DisplayHelp(p);
                return;
            }
            p.Message("There is no {0} command \"{1}\" to display help for.", parentCommandName, message);
        }
        public static void HelpSubCommands(Player p, string parentCommandName, List<SubCommand> subCommands) {
            p.Message("&HCommands: &S{0}", subCommands.Join(grp => grp.Group));
            p.Message("&HUse &T/Help {0} [command] &Hfor more details", parentCommandName);
        }
    }
}
