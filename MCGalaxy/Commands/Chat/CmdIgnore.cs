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
using System;
using System.IO;

namespace MCGalaxy.Commands.Chatting {
    public sealed class CmdIgnore : Command {
        public override string name { get { return "Ignore"; } }
        public override string type { get { return CommandTypes.Chat; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public override bool SuperUseable { get { return false; } }
        public override CommandAlias[] Aliases {
            get { return new [] { new CommandAlias("Deafen", "all") }; }
        }

        public override void Use(Player p, string message) {
            string[] args = message.SplitSpaces();
            string action = args[0].ToLower();
            
            if (action == "all") {
                Toggle(p, ref p.ignoreAll, "{0} ignoring all chat"); return;
            } else if (action == "irc") {
               Toggle(p, ref p.ignoreIRC, "{0} ignoring IRC chat"); return;
            } else if (action == "titles") {
                Toggle(p, ref p.ignoreTitles, "{1}Player titles {0} show before names in chat"); return;
            } else if (action == "nicks") {
                Toggle(p, ref p.ignoreNicks, "{1}Custom player nicks {0} show in chat"); return;
            } else if (action == "8ball") {
                Toggle(p, ref p.ignore8ball, "{0} ignoring %T/8ball"); return;
            } else if (action == "drawoutput") {
                Toggle(p, ref p.ignoreDrawOutput, "{0} ignoring draw command output"); return;
            } else if (action == "list") {                
                string names = p.listignored.Join();
                if (names.Length > 0) {
                    Player.Message(p, "&cCurrently ignoring the following players:");
                    Player.Message(p, names);
                }
                
                if (p.ignoreAll) Player.Message(p, "&cIgnoring all chat");
                if (p.ignoreIRC) Player.Message(p, "&cIgnoring IRC chat");
                if (p.ignore8ball) Player.Message(p, "&cIgnoring %T/8ball");
                
                if (p.ignoreDrawOutput) Player.Message(p, "&cIgnoring draw command output.");
                if (p.ignoreTitles) Player.Message(p, "&cPlayer titles do not show before names in chat.");
                if (p.ignoreNicks) Player.Message(p, "&cCustom player nicks do not show in chat.");
                return;
            }
            
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
                        Player.Message(p, "You must use the full name when unignoring offline players.");
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
            SaveIgnores(p);
        }
        
        static void Toggle(Player p, ref bool ignore, string format) {
            ignore = !ignore;
            if (format.StartsWith("{0}")) {
                Player.Message(p, format, ignore ? "&cNow" : "&aNo longer");
            } else {
                Player.Message(p, format, ignore ? "now" : "no longer", ignore ? "&c" : "&a");
            }
            SaveIgnores(p);
        }
        static void SaveIgnores(Player p) {
            string path = "ranks/ignore/" + p.name + ".txt";
            if (!Directory.Exists("ranks/ignore"))
                Directory.CreateDirectory("ranks/ignore");
            
            try {
                using (StreamWriter w = new StreamWriter(path)) {
                    if (p.ignoreAll) w.WriteLine("&all");
                    if (p.ignoreIRC) w.WriteLine("&irc");
                    if (p.ignore8ball) w.WriteLine("&8ball");
                    
                    if (p.ignoreDrawOutput) w.WriteLine("&drawoutput");
                    if (p.ignoreTitles) w.WriteLine("&titles");
                    if (p.ignoreNicks) w.WriteLine("&nicks");
                    
                    foreach (string line in p.listignored)
                        w.WriteLine(line);
                }
            } catch (IOException ex) {
                Logger.LogError(ex);
                Logger.Log(LogType.Warning, "Failed to save ignored list for player: " + p.name);
            }
        }

        public override void Help(Player p) {
            Player.Message(p, "%T/Ignore [name]");
            Player.Message(p, "%HUsing the same name again will unignore.");
            Player.Message(p, "%H If name is \"all\", all chat is ignored.");
            Player.Message(p, "%H If name is \"irc\", IRC chat is ignored.");
            Player.Message(p, "%H If name is \"8ball\", %T/8ball %His ignored.");
            Player.Message(p, "%H If name is \"drawoutput\", drawing command output is ignored.");
            Player.Message(p, "%H If name is \"titles\", player titles before names are ignored.");
            Player.Message(p, "%H If name is \"nicks\", custom player nicks do not show in chat.");
            Player.Message(p, "%HOtherwise, all chat from the player with [name] is ignored.");
        }
    }
}
