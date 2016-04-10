/*
    Written by Jack1312
  
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
using System.IO;
namespace MCGalaxy.Commands {
    
    public sealed class CmdIgnore : Command {
        
        public override string name { get { return "ignore"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Chat; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }

        public override void Use(Player p, string message) {
            if (p == null) { MessageInGameOnly(p); return; }
            string[] args = message.Split(' ');
            string action = args[0].ToLower();
            
            if (action == "all") {
                p.ignoreAll = !p.ignoreAll;
                Player.SendMessage(p, p.ignoreAll ? "&cAll chat is now ignored!" : "&aAll chat is no longer ignored!");
                CreateIgnoreFile(p); return;
            } else if (action == "irc") {
                p.ignoreIRC = !p.ignoreIRC;
                Player.SendMessage(p, p.ignoreIRC ? "&cIRC chat is now ignored!" : "&aIRC chat is no longer ignored!");
                CreateIgnoreFile(p); return;
            } else if (action == "global") {
                p.ignoreGlobal = !p.ignoreGlobal;
                Player.SendMessage(p, p.ignoreGlobal ? "&cGlobal Chat is now ignored!" : "&aGlobal Chat is no longer ignored!");
                CreateIgnoreFile(p); return;
            } else if (action == "list") {
                Player.SendMessage(p, "&cCurrently ignoring the following players:");
                string names = string.Join(", ", p.listignored);
                if (names != "") Player.SendMessage(p, names);
                if (p.ignoreAll) Player.SendMessage(p, "&cIgnoring all chat.");
                if (p.ignoreIRC) Player.SendMessage(p, "&cIgnoring IRC chat.");
                if (p.ignoreGlobal) Player.SendMessage(p, "&cIgnoring global chat.");                
                return;
            }
            
            Player who = PlayerInfo.Find(action);
            if (who == null) { Player.SendMessage(p, "Could not find player specified."); return; }
            if (who.name == p.name) { Player.SendMessage(p, "You cannot ignore yourself."); return; }
            CreateIgnoreFile(p);
            
            if (!p.listignored.Contains(who.name)) {
                p.listignored.Add(who.name);
                Player.SendMessage(p, "Player now ignored: &c" + who.DisplayName + "!");
            } else {
                p.listignored.Remove(who.name);
                Player.SendMessage(p, "Player is no longer ignored: &a" + who.DisplayName + "!");
            }
        }
        
        static void CreateIgnoreFile(Player p) {
            string path = "ranks/ignore/" + p.name + ".txt";
            if (!Directory.Exists("ranks/ignore")) Directory.CreateDirectory("ranks/ignore");
            if (!File.Exists(path)) File.Create(path).Dispose();
        }

        public override void Help(Player p) {
            Player.SendMessage(p, "%T/ignore [name]");
            Player.SendMessage(p, "%HUsing the same name again will unignore.");
            Player.SendMessage(p, "%H  If name is \"all\", all chat is ignored.");
            Player.SendMessage(p, "%H  If name is \"global\", MCGalaxy global chat is ignored.");
            Player.SendMessage(p, "%H  If name is \"irc\", IRC chat is ignored.");
            Player.SendMessage(p, "%H  Otherwise, the online player matching the name is ignored.");            
        }
    }
}
