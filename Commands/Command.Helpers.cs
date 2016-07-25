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

namespace MCGalaxy {
    
    public abstract partial class Command {

        const CommandEnable bothFlags = CommandEnable.Lava | CommandEnable.Zombie;
        public static string GetDisabledReason(CommandEnable enable) {
            if (enable == CommandEnable.Always) return null;
            if (enable == CommandEnable.Economy && !Economy.Enabled)
                return "economy is disabled.";
            
            if (enable == bothFlags && !(Server.zombie.Running || Server.lava.active))
                return "neither zombie nor lava survival is running.";
            if (enable == CommandEnable.Zombie && !Server.zombie.Running)
                return "zombie survival is not running.";
            if (enable == CommandEnable.Lava)
                return "lava survival is not running.";
            return null;
        }
        
        /// <summary> Clears the block change handler, and reverts the block back 
        /// to the existing one in the world. </summary>
        protected static void RevertAndClearState(Player p, ushort x, ushort y, ushort z) {
            p.ClearBlockchange();
            p.RevertBlock(x, y, z);
        }
        
        protected void MessageInGameOnly(Player p) {
            Player.Message(p, "/{0} can only be used in-game.", name);
        }
        
        protected bool CheckSuper(Player p, string message, string type) {
            if (message != "" || !Player.IsSuper(p)) return false;
            SuperRequiresArgs(name, p, type); 
            return true;
        }
        
        protected void SuperRequiresArgs(Player p, string type) { SuperRequiresArgs(name, p, type); }
        
        protected static void SuperRequiresArgs(string cmd, Player p, string type) {
            string src = p == null ? "console" : "IRC";
            Player.Message(p, "When using /{0} from {2}, you must provide a {1}.", cmd, type, src);
        }
        
        protected bool CheckExtraPerm(Player p, int num = 1) {
            return p == null || (int)p.Rank >= CommandOtherPerms.GetPerm(this, num);
        }
        
        protected void MessageNeedExtra(Player p, string action, int num = 1) {
            int perm = CommandOtherPerms.GetPerm(this, num);
            MessageNeedMinPerm(p, action, perm);
        }
        
        protected static void MessageNeedMinPerm(Player p, string action, int perm) {
            Group grp = Group.findPermInt(perm);
            if (grp == null)
                Player.Message(p, "Only {0}%S+ can {1}", perm, action);
            else
                Player.Message(p, "Only {0}%S+ can {1}", grp.ColoredName, action);
        }
        
        protected static void MessageTooHighRank(Player p, string action, bool canAffectOwnRank) {
            MessageTooHighRank(p, action, p.group, canAffectOwnRank);
        }
        
        protected static void MessageTooHighRank(Player p, string action, Group grp, bool canAffectGroup) {
            if (canAffectGroup)
                Player.Message(p, "Can only {0} players ranked {1} %Sor below", action, grp.ColoredName);
            else
                Player.Message(p, "Can only {0} players ranked below {1}", action, grp.ColoredName);
        }
        
        protected static bool ValidName(Player p, string name, string type) {
            if (Player.ValidName(name)) return true;
            Player.Message(p, "\"{0}\" is not a valid {1} name.", name, type);
            return false;
        }
        
        internal void MessageCannotUse(Player p) {
            var perms = GrpCommands.allowedCommands.Find(C => C.commandName == name);
            StringBuilder builder = new StringBuilder("Only ");
            Formatter.PrintRanks(perms.lowestRank, perms.allow, perms.disallow, builder);
            builder.Append(" can use %T/" + name);
            Player.Message(p, builder.ToString());
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
