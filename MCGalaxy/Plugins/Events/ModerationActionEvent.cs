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
    public sealed class ModerationAction {
        
        /// <summary> Target player name or IP. </summary>
        public string Target;
        
        /// <summary> Player performing the action (e.g. person who is banning). </summary>
        public Player Actor;

        /// <summary> Type of this action. </summary>
        public ModerationActionType Type;
        
        /// <summary> Reason provided for the action, can be null. </summary>
        public string Reason;
        
        /// <summary> How long the action lasts for (e.g. duration of a tempban), before reverting. </summary>
        /// <remarks> Duration of 0 means the action is permanent. (e.g. regular /ban) </remarks>
        public TimeSpan Duration;
        
        /// <summary> Action-specific metadata, see remarks in ModerationActionType for what is in this. </summary>
        public object Metadata;
        
        public ModerationAction(string target, Player actor, ModerationActionType type,
                                string reason = null, TimeSpan duration = default(TimeSpan)) {
            Target = target;
            Actor = actor;
            Type = type;
            
            if (reason != null && reason == "") reason = null;
            Reason = reason;
            Duration = duration;
        }
    }
    
    public delegate void OnModerationAction(ModerationAction action);
    
    /// <summary> Types of moderation actions that can occur. </summary>
    public enum ModerationActionType {
        
        /// <summary> Player is banned. </summary>
        /// <remarks> Metadata is a boolean, true if the ban is a stealth ban. </remarks>
        Ban,
        /// <summary> Player is unbanned. </summary>
        Unban,
        /// <summary> IP address is banned. </summary>
        BanIP,
        /// <summary> IP address is unbanned. </summary>
        UnbanIP,
        /// <summary> Player is muted. </summary>
        Muted,
        /// <summary> Player is unmuted. </summary>
        Unmuted,
        /// <summary> Player is frozen. </summary>
        Frozen,
        /// <summary> Player is unfrozen. </summary>
        Unfrozen,
        /// <summary> Player is jailed. </summary>
        Jailed,
        /// <summary> Player is unjailed. </summary>
        Unjailed,
        /// <summary> Player is given a warning. </summary>
        Warned,
        /// <summary> Player has their block changes undone. </summary>
        Undone,
        /// <summary> Player is kicked from the server. </summary>
        Kicked,
    }
    
    /// <summary> Raised when a moderation action occurs. </summary>
    public sealed class OnModerationActionEvent : IPluginEvent<OnModerationAction> {
        internal OnModerationActionEvent(OnModerationAction method, Priority priority, Plugin plugin)
            : base(method, priority, plugin) { }
        
        public static void Call(ModerationAction e) {
            if (handlers.Count == 0) return;
            CallImpl(pl => pl(e));
        }
    }
}
