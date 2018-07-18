/*
    Copyright 2010 MCLawl Team - Written by Valek (Modified for use with MCGalaxy)
 
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
namespace MCGalaxy.Commands.Moderation {
    
    public sealed class CmdPossess : Command2 {
        public override string name { get { return "Possess"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        public override bool SuperUseable { get { return false; } }

        public override void Use(Player p, string message, CommandData data) {
            string[] args = message.SplitSpaces();
            if (args.Length > 2) { Help(p); return; }

            string skin = (args.Length == 2) ? args[1] : "";
            message = args[0];
            if (message.Length == 0 || message == p.possess) {
                if (message.Length == 0 && p.possess.Length == 0) { Help(p); return; }
                
                Player who = PlayerInfo.FindExact(p.possess);
                if (who == null) {
                    p.possess = "";
                    p.Message("Possession disabled."); return;
                }
                if (who == p) {
                    p.Message("Cannot possess yourself!"); return;
                }
                who.following = "";
                who.canBuild = true;
                p.possess = "";
                if (!who.MarkPossessed()) return;
                
                p.invincible = false;
                Command.Find("Hide").Use(p, "", data);
                p.Message("Stopped possessing " + who.ColoredName + "%S.");
            } else {
                Player who = PlayerInfo.FindMatches(p, message);
                if (who == null) return;
                if (!CheckRank(p, data, who, "teleport", false)) return;
                
                if (who.possess.Length > 0) {
                    p.Message("That player is currently possessing someone!"); return;
                }
                if (who.following.Length > 0) {
                    p.Message("That player is either following someone or already possessed."); return;
                }                
                if (p.possess.Length > 0) {
                    Player prev = PlayerInfo.FindExact(p.possess);
                    if (prev != null) {
                        prev.following = "";
                        prev.canBuild = true;
                        if (!prev.MarkPossessed()) return;
                    }
                }
                
                Command.Find("TP").Use(p, who.name, data);
                if (!p.hidden) Command.Find("Hide").Use(p, "", data);
                p.possess = who.name;
                who.following = p.name;
                if (!p.invincible) p.invincible = true;
                
                bool result = (skin == "#") ? who.MarkPossessed() : who.MarkPossessed(p.name);
                if (!result) return;
                
                Entities.Despawn(p, who);
                who.canBuild = false;
                p.Message("Successfully possessed {0}%S.", who.ColoredName);
            }
        }

        public override void Help(Player p) {
            p.Message("/possess [player] <skin as #> - DEMONIC POSSESSION HUE HUE");
            p.Message("Using # after player name makes possessed keep their custom skin during possession.");
            p.Message("Not using it makes them lose their skin, and makes their name show as \"Player (YourName)\".");
        }
    }
}
