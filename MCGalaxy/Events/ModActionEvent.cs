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

namespace MCGalaxy.Events {
    
    /// <summary> Represents a moderation action. </summary>
    public sealed class ModAction {
        
        /// <summary> Target player name or IP. </summary>
        public string Target;

        /// <summary> Gets the colored name of the target. (Not nickname) </summary>
        public string TargetName { get { return Actor.FormatNick(Target); } }
        
        internal Group targetGroup;
        /// <summary> Gets the rank/group target is in. </summary>
        public Group TargetGroup {
            get {
                if (targetGroup != null) return targetGroup;
                targetGroup = PlayerInfo.GetGroup(Target);
                return targetGroup;
            }
        }
        
        
        /// <summary> Player performing the action (e.g. person who is banning). </summary>
        public Player Actor;
        
        /// <summary> Type of this action. </summary>
        public ModActionType Type;
        
        /// <summary> Reason provided for the action, can be empty string. Never null. </summary>
        public string Reason;
        
        /// <summary> Whether the moderation action should be announced publicly to chat, IRC, etc. </summary>
        public bool Announce;
        
        /// <summary> Returns " (reason)" if reason is given, "" if not. </summary>
        public string ReasonSuffixed {
            get { return Reason.Length == 0 ? "" : " (" + Reason + "&S)"; }
        }
        
        /// <summary> Returns a formatted moderation action message. </summary>
        public string FormatMessage(string target, string action) {
            string suffix = "&S";
            if (Duration.Ticks != 0) suffix += " for " + Duration.Shorten();
            
            suffix += "." + ReasonSuffixed;
            return target + " &Swas " + action + " &Sby " + Actor.ColoredName + suffix;
        }
        
        
        /// <summary> How long the action lasts for (e.g. duration of a tempban), before reverting. </summary>
        /// <remarks> Duration of 0 means the action is permanent. (e.g. regular /ban) </remarks>
        public TimeSpan Duration;
        
        /// <summary> Action-specific metadata, see remarks in ModerationActionType for what is in this. </summary>
        public object Metadata;
        
        public ModAction(string target, Player actor, ModActionType type,
                                string reason = null, TimeSpan duration = default(TimeSpan)) {
            Target = target;
            Actor = actor;
            Type = type;
            
            if (reason == null) reason = "";
            Reason = reason;
            Duration = duration;
            Announce = true;
        }
    }
    
    public delegate void OnModAction(ModAction action);
    
    /// <summary> Types of moderation actions that can occur. </summary>
    public enum ModActionType {
        
        /// <summary> Player was banned. </summary>
        Ban,
        /// <summary> Player was unbanned. </summary>
        Unban,
        /// <summary> IP address was banned. </summary>
        BanIP,
        /// <summary> IP address was unbanned. </summary>
        UnbanIP,
        /// <summary> Player was muted. </summary>
        Muted,
        /// <summary> Player was unmuted. </summary>
        Unmuted,
        /// <summary> Player was frozen. </summary>
        Frozen,
        /// <summary> Player was unfrozen. </summary>
        Unfrozen,
        /// <summary> Player was jailed. </summary>
        [Obsolete]
        Jailed,
        /// <summary> Player was unjailed. </summary>
        [Obsolete]
        Unjailed,
        /// <summary> Player was given a warning. </summary>
        Warned,
        /// <summary> Player has their rank changed. </summary>
        /// <remarks> Metadata is Group of new rank. </remarks>
        Rank,
        /// <summary> Player was kicked from the server. </summary>
        Kicked,
    }
    
    /// <summary> Raised when a moderation action occurs. </summary>
    public sealed class OnModActionEvent : IEvent<OnModAction> {
        public static void Call(ModAction e) {
            if (handlers.Count == 0) return;
            CallCommon(pl => pl(e));
        }
    }
}
