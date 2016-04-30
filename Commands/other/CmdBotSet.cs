/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
    Dual-licensed under the    Educational Community License, Version 2.0 and
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
using System.IO;
using MCGalaxy.Bots;

namespace MCGalaxy.Commands {
    public sealed class CmdBotSet : Command {
        public override string name { get { return "botset"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public override CommandPerm[] AdditionalPerms {
            get { return new[] { new CommandPerm(LevelPermission.Operator, "Lowest rank that can set the bot to killer") }; }
        }

        public override void Use(Player p, string message) {
            if (message == "") { Help(p); return; }
            string[] args = message.Split(' ');

            if (args.Length == 1) {
                PlayerBot pB = PlayerBot.Find(message);
                try { pB.Waypoints.Clear(); } catch { }
                pB.kill = false;
                pB.hunt = false;
                pB.AIName = "";
                Player.SendMessage(p, pB.color + pB.name + "%S's AI was turned off.");
                Server.s.Log(pB.name + "'s AI was turned off.");
                return;
            } else if (args.Length != 2) {
                Help(p); return;
            }

            PlayerBot Pb = PlayerBot.Find(args[0]);
            if (Pb == null) { Player.SendMessage(p, "Could not find specified Bot"); return; }
            string ai = args[1].ToLower();

            if (ai == "hunt") {
                Pb.hunt = !Pb.hunt;
                try { Pb.Waypoints.Clear(); }
                catch { }
                Pb.AIName = "";
                if (p != null) Chat.GlobalChatLevel(p, Pb.color + Pb.name + "%S's hunt instinct: " + Pb.hunt, false);
                Server.s.Log(Pb.name + "'s hunt instinct: " + Pb.hunt);
                BotsFile.UpdateBot(Pb);
                return;
            } else if (ai == "kill") {
                if (!CheckAdditionalPerm(p)) { MessageNeedPerms(p, "can toggle a bot's killer instinct."); return; }
                Pb.kill = !Pb.kill;
                if (p != null) Chat.GlobalChatLevel(p, Pb.color + Pb.name + "%S's kill instinct: " + Pb.kill, false);
                Server.s.Log(Pb.name + "'s kill instinct: " + Pb.kill);
                BotsFile.UpdateBot(Pb);
                return;
            }
            
            if (!BotScript.Parse(p, Pb, "bots/" + ai)) return;
            Pb.AIName = ai;
            if (p != null) Chat.GlobalChatLevel(p, Pb.color + Pb.name + "%S's AI is now set to " + ai, false);
            Server.s.Log(Pb.name + "'s AI was set to " + ai);
            BotsFile.UpdateBot(Pb);
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "/botset <bot> <AI script> - Makes <bot> do <AI script>");
            Player.SendMessage(p, "Special AI scripts: Kill and Hunt");
        }
    }
}
