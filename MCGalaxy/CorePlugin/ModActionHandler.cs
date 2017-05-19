/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    Copyright 2011 MCForge
    
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
using MCGalaxy.Commands;
using MCGalaxy.Commands.Moderation;
using MCGalaxy.Events;

namespace MCGalaxy.Core {
    internal static class ModActionHandler {
        
        internal static void HandleModAction(ModAction action) {
            switch (action.Type) {
                    case ModActionType.Frozen: DoFreeze(action); break;
                    case ModActionType.Unfrozen: DoUnfreeze(action); break;
                    case ModActionType.Jailed: DoJail(action); break;
                    case ModActionType.Unjailed: DoUnjail(action); break;
                    case ModActionType.Muted: DoMute(action); break;
                    case ModActionType.Unmuted: DoUnmute(action); break;
                    case ModActionType.Ban: DoBan(action); break;
                    case ModActionType.BanIP: DoBanIP(action); break;
                    case ModActionType.UnbanIP: DoUnbanIP(action); break;
                    case ModActionType.Warned: DoWarn(action); break;
            }
        }
        
        static void LogAction(ModAction e, Player who, string action) {
            if (who != null) {
                Chat.MessageGlobal(who, e.TargetName + action + " %Sby " + e.ActorName + "%S." + e.ReasonSuffixed, false);
            } else {
                Chat.MessageGlobal(e.TargetName + action + " %Sby " + e.ActorName + "%S." + e.ReasonSuffixed);
            }
            
            action = Colors.StripColors(action);
            Server.s.Log(e.Target + action + " by " + e.ActorName);
        }

        
        static void DoFreeze(ModAction e) {
            Player who = PlayerInfo.FindExact(e.Target);
            if (who != null) who.frozen = true;
            LogAction(e, who, " %Swas &bfrozen");

            Server.frozen.AddIfNotExists(e.Target);
            Server.frozen.Save();
        }
        
        static void DoUnfreeze(ModAction e) {
            Player who = PlayerInfo.FindExact(e.Target);
            if (who != null) who.frozen = false;
            LogAction(e, who, " %Swas &adefrosted");
            
            Server.frozen.Remove(e.Target);
            Server.frozen.Save();
        }
        
        
        static void DoJail(ModAction e) {
            Player who = PlayerInfo.FindExact(e.Target);
            if (who == null) return;
            
            who.jailed = true;
            Server.jailed.AddOrReplace(who.name, who.level.name);
            LogAction(e, who, " %Swas &8jailed");
            
            Entities.GlobalDespawn(who, false);
            Position pos = new Position(who.level.jailx, who.level.jaily, who.level.jailz);
            Orientation rot = new Orientation(who.level.jailrotx, who.level.jailroty);
            Entities.GlobalSpawn(who, pos, rot, true);
            Server.jailed.Save(true);
        }
        
        static void DoUnjail(ModAction e) {
            Player who = PlayerInfo.FindExact(e.Target);
            if (who == null) return;
            
            who.jailed = false;
            Server.jailed.Remove(who.name);
            LogAction(e, who, " %Swas &afreed from jail");
            
            Command.all.Find("spawn").Use(who, "");
            Server.jailed.Save(true);
        }
        
        
        static void DoMute(ModAction e) {
            Player who = PlayerInfo.FindExact(e.Target);
            Server.muted.AddIfNotExists(e.Target);
            Server.muted.Save();
            
            if (who != null) who.muted = true;
            LogAction(e, who, " %Swas &8muted");
        }
        
        static void DoUnmute(ModAction e) {
            Player who = PlayerInfo.FindExact(e.Target);
            Server.muted.Remove(e.Target);
            Server.muted.Save();
            
            if (who != null) who.muted = false;
            LogAction(e, who, " %Swas &aun-muted");
        }
        
        
        static void DoBan(ModAction e) {
            bool banSealth = e.Metadata != null && (bool)e.Metadata;
            if (banSealth) {
                string msg = e.TargetName + " %Swas STEALTH &8banned %Sby " + e.ActorName + "%S." + e.ReasonSuffixed;
                Chat.MessageOps(msg);
            } else {
                string msg = e.TargetName + " %Swas &8banned %Sby " + e.ActorName + "%S." + e.ReasonSuffixed;
                Chat.MessageGlobal(msg);
            }
            
            Player who = PlayerInfo.FindExact(e.Target);
            if (who != null) who.color = "";
            Group group = who != null ? who.group : Group.findPlayerGroup(e.Target); // TODO: pass this in ??
            
            Ban.DeleteBan(e.Target);
            Ban.BanPlayer(e.Actor, e.Target, e.Reason, banSealth, e.TargetGroup.name);
            ModActionCmd.ChangeRank(e.Target, e.targetGroup, Group.BannedRank, who);
            Server.s.Log("BANNED: " + e.Target + " by " + e.ActorName);
        }
        
        
        static void DoBanIP(ModAction e) {
            LevelPermission seeIPperm = CommandExtraPerms.MinPerm("whois");
            Chat.MessageWhere("An IP was &8banned %Sby " + e.ActorName + "%S. " + e.ReasonSuffixed,
                              pl => pl.Rank < seeIPperm);
            Chat.MessageWhere(e.Target + " was &8IP banned %Sby " + e.ActorName + "%S. " + e.ReasonSuffixed,
                              pl  => pl.Rank >= seeIPperm);
            
            Server.s.Log("IP-BANNED: " + e.Target + " by " + e.ActorName + ".");
            Server.bannedIP.Add(e.Target);
            Server.bannedIP.Save();
        }
        
        static void DoUnbanIP(ModAction e) {
            LevelPermission seeIPperm = CommandExtraPerms.MinPerm("whois");
            Chat.MessageWhere("An IP was &8unbanned %Sby " + e.ActorName + "%S. " + e.ReasonSuffixed,
                              pl => pl.Rank < seeIPperm);
            Chat.MessageWhere(e.Target + " was &8IP unbanned %Sby " + e.ActorName + "%S. " + e.ReasonSuffixed,
                              pl  => pl.Rank >= seeIPperm);
            
            Server.s.Log("IP-UNBANNED: " + e.Target + " by " + e.ActorName + ".");
            Server.bannedIP.Remove(e.Target);
            Server.bannedIP.Save();
        }

        
        static void DoWarn(ModAction e) {
            Player who = PlayerInfo.FindExact(e.Target);
            if (who != null) {
                LogAction(e, who, " %Swas &ewarned");
                if (who.warn == 0) {
                    Player.Message(who, "Do it again twice and you will get kicked!");
                } else if (who.warn == 1) {
                    Player.Message(who, "Do it one more time and you will get kicked!");
                } else if (who.warn == 2) {
                    Chat.MessageGlobal("{0} %Swas warn-kicked by {1}", who.ColoredName, e.ActorName);
                    string chatMsg = "by " + e.ActorName + "%S: " + e.Reason;
                    string kickMsg = "Kicked by " + e.ActorName + "&f: " + e.Reason;
                    who.Kick(chatMsg, kickMsg);
                }
                who.warn++;
            } else {
                if (!Server.LogNotes) {
                    Player.Message(e.Actor, "Notes logging must be enabled to warn offline players."); return;
                }
                LogAction(e, who, " %Swas &ewarned");
            }
        }
    }
}
