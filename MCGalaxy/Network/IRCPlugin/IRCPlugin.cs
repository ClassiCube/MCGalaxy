﻿/*
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
using MCGalaxy.Events;

namespace MCGalaxy.Network {

    public sealed class IRCPlugin : Plugin_Simple {
        public override string creator { get { return Server.SoftwareName + " team"; } }
        public override string MCGalaxy_Version { get { return Server.VersionString; } }
        public override string name { get { return "Core_IRCPlugin"; } }
        public IRCBot Bot;

        public override void Load(bool startup) {
            OnPlayerConnectEvent.Register(HandleConnect, Priority.Low, this);
            OnPlayerDisconnectEvent.Register(HandleDisconnect, Priority.Low, this);
            OnPlayerChatEvent.Register(HandleChat, Priority.Low, this);
            Player.DoPlayerAction += Player_PlayerAction;
            OnModActionEvent.Register(HandleModerationAction, Priority.Low, this);
        }
        
        public override void Unload(bool shutdown) {
            OnPlayerConnectEvent.UnRegister(this);
            OnPlayerDisconnectEvent.UnRegister(this);
            OnPlayerChatEvent.UnRegister(this);
            Player.DoPlayerAction -= Player_PlayerAction;
            OnModActionEvent.UnRegister(this);
        }
        
        
        void HandleModerationAction(ModAction e) {
            if (!Server.IRC.Enabled || !e.Announce) return;
            
            switch (e.Type) {
                case ModActionType.Warned:
                    Bot.Say(e.FormatMessage(e.TargetName, "&ewarned")); break;
                case ModActionType.Ban:
                    Bot.Say(e.FormatMessage(e.TargetName, "&8banned")); break;
                case ModActionType.Unban:
                    Bot.Say(e.FormatMessage(e.TargetName, "&8unbanned")); break;
                case ModActionType.BanIP:
                    Bot.Say(e.FormatMessage(e.TargetName, "&8IP banned"), true);
                    Bot.Say(e.FormatMessage("An IP", "&8IP banned")); break;
                case ModActionType.UnbanIP:
                    Bot.Say(e.FormatMessage(e.TargetName, "&8IP unbanned"), true);
                    Bot.Say(e.FormatMessage("An IP", "&8IP unbanned")); break;
                case ModActionType.Rank:
                    Bot.Say(e.FormatMessage(e.TargetName, GetRankAction(e))); break;
            }
        }
        
        static string GetRankAction(ModAction action) {
            Group newRank = (Group)action.Metadata;
            string prefix = newRank.Permission >= action.TargetGroup.Permission ? "promoted to " : "demoted to ";
            return prefix + newRank.ColoredName;
        }
        
        void Player_PlayerAction(Player p, PlayerAction action,
                                 string message, bool stealth) {
            if (!Server.IRC.Enabled) return;
            string msg = null;
            
            if (action == PlayerAction.AFK && !p.hidden)
                msg = p.ColoredName + " %Sis AFK " + message;
            else if (action == PlayerAction.UnAFK && !p.hidden)
                msg = p.ColoredName + " %Sis no longer AFK";
            else if (action == PlayerAction.Joker)
                msg = p.ColoredName + " %Sis now a &aJ&bo&ck&5e&9r%S";
            else if (action == PlayerAction.Unjoker)
                msg = p.ColoredName + " %Sis no longer a &aJ&bo&ck&5e&9r%S";
            else if (action == PlayerAction.Me)
                msg = "*" + p.DisplayName + " " + message;
            else if (action == PlayerAction.Review)
                msg = p.ColoredName + " %Sis requesting a review.";
            else if (action == PlayerAction.JoinWorld && Server.ircShowWorldChanges && !p.hidden)
                msg = p.ColoredName + " %Swent to &8" + message;
            
            if (msg != null) Bot.Say(msg, stealth);
        }
        
        
        void HandleDisconnect(Player p, string reason) {
            if (!Server.IRC.Enabled || p.hidden) return;
            if (!Server.guestLeaveNotify && p.Rank <= LevelPermission.Guest) return;
            
            Bot.Say(p.ColoredName + " %Sleft the game (" + reason + "%S)", false);
        }

        void HandleConnect(Player p) {
            if (!Server.IRC.Enabled || p.hidden) return;
            if (!Server.guestJoinNotify && p.Rank <= LevelPermission.Guest) return;
            if (Plugin.IsPlayerEventCanceled(PlayerEvents.PlayerLogin, p)) return;
            
            Bot.Say(p.ColoredName + " %Sjoined the game", false);
        }

        static char[] trimChars = new char[] { ' ' };
        void HandleChat(Player p, string message) {
            if (!Server.IRC.Enabled) return;
            if (message.Trim(trimChars) == "") return;
            if (Plugin.IsPlayerEventCanceled(PlayerEvents.PlayerChat, p)) return;
            
            string name = Server.ircPlayerTitles ? p.FullName : p.group.prefix + p.ColoredName;
            Bot.Say(name + "%S: " + message, p.opchat);
        }
    }
}
