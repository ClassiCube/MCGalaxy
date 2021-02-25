/*
    Copyright 2015 MCGalaxy
 
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
using System.Collections.Generic;
using System.IO;

namespace MCGalaxy {
    
    public class PlayerIgnores {
        public List<string> Names = new List<string>(), IRCNicks = new List<string>();
        public bool All, IRC, Titles, Nicks, EightBall, DrawOutput, WorldChanges;
        
        public void Load(Player p) {
            string path = "ranks/ignore/" + p.name + ".txt";
            if (!File.Exists(path)) return;
            
            try {
                string[] lines = File.ReadAllLines(path);
                foreach (string line in lines) {
                    if (line == "&global") continue; // deprecated /ignore global
                    if (line == "&all") { All = true; continue; }
                    if (line == "&irc") { IRC = true; continue; }
                    
                    if (line == "&titles") { Titles = true; continue; }
                    if (line == "&nicks") { Nicks = true; continue; }
                    
                    if (line == "&8ball") { EightBall = true; continue; }
                    if (line == "&drawoutput") { DrawOutput = true; continue; }
                    if (line == "&worldchanges") { WorldChanges = true; continue; }
                    
                    if (line.StartsWith("&irc_")) {
                        IRCNicks.Add(line.Substring("&irc_".Length));
                    } else {
                        Names.Add(line);
                    }
                }
            } catch (IOException ex) {
                Logger.LogError("Error loading ignores for " + p.name, ex);
            }
            
            bool special = All || IRC || Titles || Nicks || EightBall || DrawOutput || WorldChanges;
            if (special || Names.Count > 0 || IRCNicks.Count > 0) {
                p.Message("&cType &a/ignore list &cto see who you are still ignoring");
            }
        }
        
        public void Save(Player p) {
            string path = "ranks/ignore/" + p.name + ".txt";
            if (!Directory.Exists("ranks/ignore"))
                Directory.CreateDirectory("ranks/ignore");
            
            try {
                using (StreamWriter w = new StreamWriter(path)) {
                    if (All) w.WriteLine("&all");
                    if (IRC) w.WriteLine("&irc");
                    
                    if (Titles) w.WriteLine("&titles");
                    if (Nicks) w.WriteLine("&nicks");
                    
                    if (EightBall) w.WriteLine("&8ball");                    
                    if (DrawOutput) w.WriteLine("&drawoutput");
                    if (WorldChanges) w.WriteLine("&worldchanges");
                    
                    foreach (string nick in IRCNicks) { w.WriteLine("&irc_" + nick); }
                    foreach (string name in Names) { w.WriteLine(name); }
                }
            } catch (IOException ex) {
                Logger.LogError("Error saving ignores for " + p.name, ex);
            }
        }
        
        public void Output(Player p) {
            if (Names.Count > 0) {
                p.Message("&cCurrently ignoring the following players:");
                p.Message(Names.Join(n => p.FormatNick(n)));
            }
            if (IRCNicks.Count > 0) {
                p.Message("&cCurrently ignoring the following IRC nicks:");
                p.Message(IRCNicks.Join());
            }
            
            if (All) p.Message("&cIgnoring all chat");
            if (IRC) p.Message("&cIgnoring IRC chat");
            
            if (Titles) p.Message("&cPlayer titles do not show before names in chat");
            if (Nicks) p.Message("&cCustom player nicks do not show in chat");
            
            if (EightBall) p.Message("&cIgnoring &T/8ball");            
            if (DrawOutput) p.Message("&cIgnoring draw command output");           
            if (WorldChanges) p.Message("&cIgnoring world change messages");
        }
    }
}