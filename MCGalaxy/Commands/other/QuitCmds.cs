/*
    Copyright 2010 MCLawl Team - Written by Valek (Modified for use with MCGalaxy)
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
using System.Threading;

namespace MCGalaxy.Commands.Misc {    
    public sealed class CmdRagequit : Command {        
        public override string name { get { return "ragequit"; } }
        public override string shortcut { get { return "rq"; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        
        public override void Use(Player p, string message)  {
            p.Leave("RAGEQUIT!!");
        }

        public override void Help(Player p) {
            Player.Message(p, "%T/ragequit");
            Player.Message(p, "%HMakes you ragequit");
        }
    }
    
    public sealed class CmdQuit : Command {        
        public override string name { get { return "quit"; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }

        public override void Use(Player p, string message) {
            string msg = message != "" ? "Left the game: " + message : "Left the game.";
            if (p.muted) msg = "Left the game.";
            p.Leave(msg);
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/quit <reason>");
            Player.Message(p, "%HLeave the server.");
        }
    }
    
    public sealed class CmdCrashServer : Command {        
        public override string name { get { return "crashserver"; } }
        public override string shortcut { get { return "crash"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }

        public override void Use(Player p, string message) {
            if (message != "") { Help(p); return; }
            int code = p.random.Next(int.MinValue, int.MaxValue);
            p.Leave("Server crash! Error code 0x" + Convert.ToString(code, 16).ToUpper());
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/crashserver");
            Player.Message(p, "%HCrash the server with a generic error");
        }
    }
    
    public sealed class CmdHacks : Command {
        public override string name { get { return "hacks"; } }
        public override string shortcut { get { return "hax"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }

        public override void Use(Player p, string message) {
            if (message != "") {
                Player.Message(p, "&cIncorrect syntax. Abuse detected.");
                Thread.Sleep(3000);
            }
            
            const string msg = "Your IP has been backtraced + reported to FBI Cyber Crimes Unit.";
            p.Leave("kicked (" + msg + ")", msg, false);
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/hacks");
            Player.Message(p, "%HPerforms various server hacks. OPERATORS ONLY!!!");
        }
    }
}
