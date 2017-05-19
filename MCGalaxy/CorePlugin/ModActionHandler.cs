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
                    case ModActionType.Ban: DoBan(action); break;
                    case ModActionType.Warned: DoWarn(action); break;
            }
        }
        
        static void DoFreeze(ModAction e) {
            Player who = PlayerInfo.FindExact(e.Target);
            if (who != null) {
                Chat.MessageGlobal(who, who.ColoredName + " %Swas &bfrozen %Sby " + e.ActorName + "%S.", false);
                who.frozen = true;
            }
            
            Server.s.Log(e.Target + " was frozen by " + e.ActorName);
            Server.frozen.AddIfNotExists(e.Target);
            Server.frozen.Save();
        }
        
        static void DoUnfreeze(ModAction e) {
            Player who = PlayerInfo.FindExact(e.Target);
            if (who != null) {
                Chat.MessageGlobal(who, who.ColoredName + " %Swas &adefrosted %Sby " + e.ActorName + "%S.", false);
                who.frozen = false;
            }
            
            Server.s.Log(e.Target + "  was defrosted by " + e.ActorName);
            Server.frozen.Remove(e.Target);
            Server.frozen.Save();
        }
        
        
        static void DoJail(ModAction e) {
            Player who = PlayerInfo.FindExact(e.Target);
            if (who == null) return;
            
            Player.Message(e.Actor, "You jailed " + who.ColoredName);
            who.jailed = true;
            Server.jailed.AddOrReplace(who.name, who.level.name);
            Chat.MessageGlobal(who, who.ColoredName + " %Swas &8jailed", false);
            
            Entities.GlobalDespawn(who, false);
            Position pos = new Position(who.level.jailx, who.level.jaily, who.level.jailz);
            Orientation rot = new Orientation(who.level.jailrotx, who.level.jailroty);
            Entities.GlobalSpawn(who, pos, rot, true);
            Server.jailed.Save(true);
        }
        
        static void DoUnjail(ModAction e) {
            Player who = PlayerInfo.FindExact(e.Target);
            if (who == null) return;
            
            Player.Message(e.Actor, "You freed " + who.ColoredName + " %Sfrom jail");
            who.jailed = false;
            Server.jailed.Remove(who.name);
            Chat.MessageGlobal(who, who.ColoredName + " %Swas &afreed %Sfrom jail", false);
            
            Command.all.Find("spawn").Use(who, "");
            Server.jailed.Save(true);
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
            Ban.BanPlayer(e.Actor, e.Target, e.Reason, banSealth, group.name);
            ModActionCmd.ChangeRank(e.Target, group, Group.BannedRank, who);
            Server.s.Log("BANNED: " + e.Target + " by " + e.ActorName);
        }
        
        
        static void DoWarn(ModAction e) {
            Player who = PlayerInfo.FindExact(e.Target);
            if (who != null) {
                Chat.MessageGlobal("{0} &ewarned {1} &efor: &c{2}", e.ActorName, who.ColoredName, e.Reason);
                Server.s.Log(e.ActorName + " warned " + who.name);

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
                
                string reason = e.Reason != null ? " for: " + e.Reason : "";
                Player.Message(e.Actor, "Warned {0}{1}.", e.TargetName, reason);
            }
        }
    }
}
