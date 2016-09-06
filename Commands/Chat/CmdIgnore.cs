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
        public override CommandAlias[] Aliases {
            get { return new [] { new CommandAlias("deafen", "all") }; }
        }

        public override void Use(Player p, string message) {
            if (p == null) { MessageInGameOnly(p); return; }
            string[] args = message.Split(' ');
            string action = args[0].ToLower();
            
            if (action == "all") {
                p.ignoreAll = !p.ignoreAll;
                Player.Message(p, "{0} ignoring all chat", p.ignoreAll ? "&cNow " : "&aNo longer ");
                CreateIgnoreFile(p); return;
            } else if (action == "irc") {
                p.ignoreIRC = !p.ignoreIRC;
                Player.Message(p, "{0} ignoring IRC chat", p.ignoreIRC ? "&cNow " : "&aNo longer ");
                CreateIgnoreFile(p); return;
            } else if (action == "global") {
                p.ignoreGlobal = !p.ignoreGlobal;
                Player.Message(p, "{0} ignoring global chat", p.ignoreGlobal ? "&cNow " : "&aNo longer ");
                CreateIgnoreFile(p); return;
            } else if (action == "titles") {
                p.ignoreTitles = !p.ignoreTitles;
                Player.Message(p, "{0} show before names in chat", 
                               p.ignoreTitles ? "&cPlayer titles no longer" : "&aPlayer titles now");
                CreateIgnoreFile(p); return;
            } else if (action == "nicks") {
                p.ignoreNicks = !p.ignoreNicks;
                Player.Message(p, "{0} show in chat", 
                               p.ignoreNicks ? "&cCustom player nicks no longer" : "&aCustom player nicks");
                CreateIgnoreFile(p); return;
            } else if (action == "list") {
                Player.Message(p, "&cCurrently ignoring the following players:");
                string names = p.listignored.Join();
                if (names != "") Player.Message(p, names);
                if (p.ignoreAll) Player.Message(p, "&cIgnoring all chat");
                if (p.ignoreIRC) Player.Message(p, "&cIgnoring IRC chat");
                if (p.ignoreGlobal) Player.Message(p, "&cIgnoring global chat");
                if (p.ignoreTitles) Player.Message(p, "&cPlayer titles do not show before names in chat.");
                if (p.ignoreNicks) Player.Message(p, "&cCustom player nicks do not show in chat.");
                return;
            }
            
            CreateIgnoreFile(p);
            string unignore = null;
            for (int i = 0; i < p.listignored.Count; i++) {
                if (!action.CaselessEq(p.listignored[i])) continue;
                unignore = p.listignored[i]; break;
            }
            
            if (unignore != null) {
                p.listignored.Remove(unignore);
                Player.Message(p, "&aNo longer ignoring {0}", unignore);
            } else {
                int matches = 0;
                Player who = PlayerInfo.FindMatches(p, action);
                if (who == null) {
                    if (matches == 0)
                        Player.SendMessage(p, "You must use the full name when unignoring offline players.");
                    return;
                }
                if (p.name == who.name) { Player.Message(p, "You cannot ignore yourself."); return; }
                
                if (p.listignored.Contains(who.name)) {
                    p.listignored.Remove(who.name);
                    Player.Message(p, "&aNo longer ignoring {0}", who.ColoredName);
                } else {
                    p.listignored.Add(who.name);
                    Player.Message(p, "&cNow ignoring {0}", who.ColoredName);
                }
            }
        }
        
        static void CreateIgnoreFile(Player p) {
            string path = "ranks/ignore/" + p.name + ".txt";
            if (!Directory.Exists("ranks/ignore")) Directory.CreateDirectory("ranks/ignore");
            if (!File.Exists(path)) File.Create(path).Dispose();
        }

        public override void Help(Player p) {
            Player.Message(p, "%T/ignore [name]");
            Player.Message(p, "%HUsing the same name again will unignore.");
            Player.Message(p, "%H If name is \"all\", all chat is ignored.");
            Player.Message(p, "%H If name is \"global\", MCGalaxy global chat is ignored.");
            Player.Message(p, "%H If name is \"irc\", IRC chat is ignored.");
            Player.Message(p, "%H If name is \"titles\", player titles before names are ignored.");
            Player.Message(p, "%H If name is \"nicks\", custom player nicks do not show in chat.");
            Player.Message(p, "%HOtherwise, all chat from the player with [name] is ignored.");
        }
    }
}
