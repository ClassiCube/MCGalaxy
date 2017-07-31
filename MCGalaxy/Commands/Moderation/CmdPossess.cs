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
    
    public sealed class CmdPossess : Command {
        public override string name { get { return "Possess"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        public override bool SuperUseable { get { return false; } }

        public override void Use(Player p, string message) {
            string[] args = message.SplitSpaces();
            if (args.Length > 2) { Help(p); return; }

            string skin = (args.Length == 2) ? args[1] : "";
            message = args[0];
            if (message.Length == 0 || message == p.possess) {
                if (message.Length == 0 && p.possess.Length == 0) { Help(p); return; }
                
                Player who = PlayerInfo.FindExact(p.possess);
                if (who == null) {
                    p.possess = "";
                    Player.Message(p, "Possession disabled."); return;
                }
                if (who == p) {
                    Player.Message(p, "Cannot possess yourself!"); return;
                }
                who.following = "";
                who.canBuild = true;
                p.possess = "";
                if (!who.MarkPossessed()) return;
                
                p.invincible = false;
                Command.all.FindByName("Hide").Use(p, "");
                Player.Message(p, "Stopped possessing " + who.ColoredName + "%S.");
            } else {
                Player who = PlayerInfo.FindMatches(p, message);
                if (who == null) return;
                if (who.Rank >= p.Rank) {
                    MessageTooHighRank(p, "possess", false); return;
                }
                
                if (who.possess != "") {
                    Player.Message(p, "That player is currently possessing someone!"); return;
                }
                if (who.following != "") {
                    Player.Message(p, "That player is either following someone or already possessed."); return;
                }                
                if (p.possess != "") {
                    Player prev = PlayerInfo.FindExact(p.possess);
                    if (prev != null) {
                        prev.following = "";
                        prev.canBuild = true;
                        if (!prev.MarkPossessed()) return;
                    }
                }
                
                Command.all.FindByName("TP").Use(p, who.name);
                if (!p.hidden) Command.all.FindByName("Hide").Use(p, "");
                p.possess = who.name;
                who.following = p.name;
                if (!p.invincible) p.invincible = true;
                
                bool result = (skin == "#") ? who.MarkPossessed() : who.MarkPossessed(p.name);
                if (!result) return;
                
                Entities.Despawn(p, who);
                who.canBuild = false;
                Player.Message(p, "Successfully possessed {0}%S.", who.ColoredName);
            }
        }

        public override void Help(Player p) {
            Player.Message(p, "/possess [player] <skin as #> - DEMONIC POSSESSION HUE HUE");
            Player.Message(p, "Using # after player name makes possessed keep their custom skin during possession.");
            Player.Message(p, "Not using it makes them lose their skin, and makes their name show as \"Player (YourName)\".");
        }
    }
}
