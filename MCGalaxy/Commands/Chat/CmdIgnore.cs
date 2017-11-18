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
        public override bool SuperUseable { get { return false; } }
        public override CommandAlias[] Aliases {
            get { return new [] { new CommandAlias("Deafen", "all") }; }
        }

        public override void Use(Player p, string message) {
            string[] args = message.SplitSpaces();
            string action = args[0].ToLower();
            
            if (action == "all") {
                Toggle(p, ref p.Ignores.All, "{0} ignoring all chat"); return;
            } else if (action == "irc") {
                if (args.Length > 1) { IgnoreIRCNick(p, args[1]); } 
                else { Toggle(p, ref p.Ignores.IRC, "{0} ignoring IRC chat"); }
                return;
            } else if (action == "titles") {
                Toggle(p, ref p.Ignores.Titles, "{1}Player titles {0} show before names in chat"); return;
            } else if (action == "nicks") {
                Toggle(p, ref p.Ignores.Nicks, "{1}Custom player nicks {0} show in chat"); return;
            } else if (action == "8ball") {
                Toggle(p, ref p.Ignores.EightBall, "{0} ignoring %T/8ball"); return;
            } else if (action == "drawoutput") {
                Toggle(p, ref p.Ignores.DrawOutput, "{0} ignoring draw command output"); return;
            } else if (action == "list") {
                p.Ignores.Output(p); return;
            }
            
            string unignore = null;
            for (int i = 0; i < p.Ignores.Names.Count; i++) {
                if (!action.CaselessEq(p.Ignores.Names[i])) continue;
                unignore = p.Ignores.Names[i]; break;
            }
            
            if (unignore != null) {
                p.Ignores.Names.Remove(unignore);
                Player.Message(p, "&aNo longer ignoring {0}", unignore);
            } else {
                int matches = 0;
                Player who = PlayerInfo.FindMatches(p, action, out matches);
                if (who == null) {
                    if (matches == 0)
                        Player.Message(p, "You must use the full name when unignoring offline players.");
                    return;
                }
                
                if (p.Ignores.Names.CaselessRemove(who.name)) {
                    Player.Message(p, "&aNo longer ignoring {0}", who.ColoredName);
                } else {
                    p.Ignores.Names.Add(who.name);
                    Player.Message(p, "&cNow ignoring {0}", who.ColoredName);
                }
            }
            p.Ignores.Save(p);
        }
        
        static void Toggle(Player p, ref bool ignore, string format) {
            ignore = !ignore;
            if (format.StartsWith("{0}")) {
                Player.Message(p, format, ignore ? "&cNow" : "&aNo longer");
            } else {
                Player.Message(p, format, ignore ? "no longer" : "now", ignore ? "&c" : "&a");
            }
            p.Ignores.Save(p);
        }
        
        static void IgnoreIRCNick(Player p, string nick) {
            if (p.Ignores.IRCNicks.CaselessRemove(nick)) {
                Player.Message(p, "&aNo longer ignoring IRC nick: {0}", nick);
            } else {
                p.Ignores.IRCNicks.Add(nick);
                Player.Message(p, "&cNow ignoring IRC nick: {0}", nick);
            }
            p.Ignores.Save(p);
        }

        public override void Help(Player p) {
            Player.Message(p, "%T/Ignore [name]");
            Player.Message(p, "%HSee %T/Help ignore names %Hfor special names when ignoring.");
            Player.Message(p, "%HOtherwise, all chat from the player with [name] is ignored.");
            Player.Message(p, "%HUsing the same [name] again will unignore.");
        }
        
        public override void Help(Player p, string message) {
            if (!message.CaselessEq("names")) { Help(p); return; }
            Player.Message(p, "%HSpecial names for %T/Ignore [name]");
            Player.Message(p, "%H all - all chat is ignored.");
            Player.Message(p, "%H irc - IRC chat is ignored.");
            Player.Message(p, "%H irc [nick] - IRC chat by that IRC nick ignored.");            
            Player.Message(p, "%H 8ball - %T/8ball %His ignored.");
            Player.Message(p, "%H drawoutput - drawing command output is ignored.");
            Player.Message(p, "%H titles - player titles before names are ignored.");
            Player.Message(p, "%H nicks - custom player nicks do not show in chat.");         
        }
    }
}
