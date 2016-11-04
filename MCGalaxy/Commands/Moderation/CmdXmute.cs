/*
    Copyright 2011 MCForge
    
    Written by GamezGalaxy (hypereddie10)
        
    Licensed under the
    Educational Community License, Version 2.0 (the "License"); you may
    not use this file except in compliance with the License. You may
    obtain a copy of the License at
    
    http://www.opensource.org/licenses/ecl2.php
    
    Unless required by applicable law or agreed to in writing,
    software distributed under the License is distributed on an "AS IS"
    BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
    or implied. See the License for the specific language governing
    permissions and limitations under the License.
*/
using System;
using System.Threading;
namespace MCGalaxy.Commands {
    
    public sealed class CmdXmute : Command {
        
        public override string name { get { return "xmute"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        
        public override void Use(Player p, string message) {
            if (message == "") { Help(p); return; }
            if (p == null) {
                Player.Message(p, "This command can only be used in-game. Use /mute [Player] instead."); return;
            }

            string[] args = message.Split(' ');
            Player muter = PlayerInfo.FindMatches(p, args[0]);
            if (muter == null) return;

            if (p != null && muter.group.Permission > p.Rank) {
                MessageTooHighRank(p, "xmute", true); return;
            }
            if (p == muter) {
                Player.Message(p, "You cannot use xmute on yourself!"); return;
            }
            Command.all.Find("mute").Use(p, muter.name);

            int time = 120;
            if (args.Length > 1 && !int.TryParse(args[1], out time)) { 
                Player.Message(p, "Invalid time given."); Help(p); return; 
            }
            if (time <= 0) {
                Player.Message(p, "Time must be positive and greater than zero."); return;
            }
            
            Chat.MessageAll("{0} %Shas been muted for {1} seconds", muter.ColoredName, time);
            Player.Message(muter, "You have been muted for " + time + " seconds");
            Thread.Sleep(time * 1000);
            Command.all.Find("mute").Use(p, muter.name);
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/xmute [player] [seconds]");
            Player.Message(p, "%HMutes [player] for [seconds] seconds");
        }
    }
}


