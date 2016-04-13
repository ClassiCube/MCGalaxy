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
        
        protected static void RevertAndClearState(Player p, ushort x, ushort y, ushort z) {
            p.ClearBlockchange();
            p.RevertBlock(x, y, z);
        }
        
        protected void MessageInGameOnly(Player p) {
            Player.SendMessage(p, "/" + name + " can only be used in-game.");
        }
        
        protected bool CheckAdditionalPerm(Player p, int num = 1) {
            return p == null || (int)p.group.Permission >= CommandOtherPerms.GetPerm(this, num);
        }
        
        protected void MessageNeedPerms(Player p, string action, int num = 1) {
            int perm = CommandOtherPerms.GetPerm(this, num);
            MessageNeedMinPerm(p, action, perm);
        }
        
        protected void MessageNeedMinPerm(Player p, string action, int perm) {
            Group grp = Group.findPermInt(perm);
            if (grp == null)
                Player.SendMessage(p, "Onlys rank with a permission level greater than &a" + perm + "%Scan " + action);
            else
                Player.SendMessage(p, "Only " + grp.color + grp.name + "%s+ can " + action);
        }
        
        protected void MessageTooHighRank(Player p, string action, bool canAffectOwnRank) {
            MessageTooHighRank(p, action, p.group, canAffectOwnRank);
        }
        
        protected void MessageTooHighRank(Player p, string action, Group grp, bool canAffectGroup) {
            if (canAffectGroup)
                 Player.SendMessage(p, "Can only " + action + " players ranked " + grp.color + grp.name + " %Sor below");
            else
                 Player.SendMessage(p, "Can only " + action + " players ranked below " + grp.color + grp.name);
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
