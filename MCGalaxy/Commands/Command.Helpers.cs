/*
    Copyright 2015 MCGalaxy

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
using System.Text;
using MCGalaxy.Commands;
using MCGalaxy.Eco;

namespace MCGalaxy {
    
    public abstract partial class Command {

        const CommandEnable bothFlags = CommandEnable.Lava | CommandEnable.Zombie;
        public static string GetDisabledReason(CommandEnable enable) {
            if (enable == CommandEnable.Always) return null;
            if (enable == CommandEnable.Economy && !Economy.Enabled)
                return "economy is disabled.";
            
            if (enable == bothFlags && !(Server.zombie.Running || Server.lava.Running))
                return "neither zombie nor lava survival is running.";
            if (enable == CommandEnable.Zombie && !Server.zombie.Running)
                return "zombie survival is not running.";
            if (enable == CommandEnable.Lava)
                return "lava survival is not running.";
            return null;
        }
        
        protected bool CheckSuper(Player p, string message, string type) {
            if (message.Length > 0 || !Player.IsSuper(p)) return false;
            SuperRequiresArgs(name, p, type);
            return true;
        }
        
        protected void SuperRequiresArgs(Player p, string type) { SuperRequiresArgs(name, p, type); }
        
        protected internal static void SuperRequiresArgs(string cmd, Player p, string type) {
            string src = p == null ? "console" : "IRC";
            Player.Message(p, "When using /{0} from {2}, you must provide a {1}.", cmd, type, src);
        }
        
        protected bool HasExtraPerm(Player p, int num) {
            return p == null || CommandExtraPerms.Find(name, num).UsableBy(p.Rank);
        }
        
        protected bool CheckExtraPerm(Player p, int num) {
            if (HasExtraPerm(p, num)) return true;
            
            CommandExtraPerms perms = CommandExtraPerms.Find(name, num);
            perms.MessageCannotUse(p);
            return false;
        }
        
        protected internal static void MessageTooHighRank(Player p, string action, bool canAffectOwnRank) {
            MessageTooHighRank(p, action, p.group, canAffectOwnRank);
        }
        
        protected static void MessageTooHighRank(Player p, string action, Group grp, bool canAffectGroup) {
            if (canAffectGroup)
                Player.Message(p, "Can only {0} players ranked {1} %Sor below", action, grp.ColoredName);
            else
                Player.Message(p, "Can only {0} players ranked below {1}", action, grp.ColoredName);
        }
        
        protected internal static bool IsCreateCommand(string str) {
            return str.CaselessEq("create") || str.CaselessEq("add") || str.CaselessEq("new");
        } 
        
        protected internal static bool IsDeleteCommand(string str) {
            return str.CaselessEq("del") || str.CaselessEq("delete") || str.CaselessEq("remove");
        }
        
        protected internal static bool IsEditCommand(string str) {
            return str.CaselessEq("edit") || str.CaselessEq("change") || str.CaselessEq("modify");
        }  

        protected internal static bool IsInfoCommand(string str) {
            return str.CaselessEq("info") || str.CaselessEq("status") || str.CaselessEq("about");
        }        
    }
    
    public sealed class CommandTypes {
        public const string Building = "build";
        public const string Chat = "chat";
        public const string Economy = "economy";
        public const string Games = "game";
        public const string Information = "information";
        public const string Moderation = "mod";
        public const string Other = "other";
        public const string World = "world";
    }
}
