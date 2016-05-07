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
    
    /// <summary> Describes an alias so that when the user types /trigger,
    /// it is treated as /target &lt;args given by user&gt; </summary>
    /// <remarks>Note \"target\" can be in the form of either "cmdname" or "cmdname args".</remarks>
    public class Alias {
        
        public static List<Alias> coreAliases = new List<Alias>();
        public static List<Alias> aliases = new List<Alias>();
        public const string file = "text/aliases.txt";
        public string Trigger, Target, Prefix, Suffix;

        public Alias(string trigger, string target) {
            Trigger = trigger;
            target = target.Trim();
            int space = target.IndexOf(' ');
            
            if (space < 0) {
                Target = target;
            } else {
                Target = target.Substring(0, space);
                Prefix = target.Substring(space + 1);
            }
        }
        
        public Alias(string trigger, string target, string prefix, string suffix) {
            Trigger = trigger; Target = target;
            Prefix = prefix; Suffix = suffix;
        }

        public static void Load(){
            aliases = new List<Alias>();
            if (!File.Exists(file)) { Save(); return; }
            PropertiesFile.Read(file, LineProcessor, ':');
        }
        
        static void LineProcessor(string key, string value) {
            aliases.Add(new Alias(key, value));
        }

        public static void Save() {
            using (StreamWriter sw = new StreamWriter(file)) {
                sw.WriteLine("# The format goes trigger : command");
                sw.WriteLine("# For example, \"y : help me\" means that when /y is typed,");
                sw.WriteLine("# it is treated as /help me <args given by user>.");
                
                foreach (Alias a in aliases) {
                    if (a.Prefix == null)
                        sw.WriteLine(a.Trigger + " : " + a.Target);
                    else
                        sw.WriteLine(a.Trigger + " : " + a.Target + " " + a.Prefix);
                }                   
            }
        }

        public static Alias Find(string cmd) {
            foreach (Alias alias in coreAliases) {
                if (alias.Trigger == cmd) return alias;
            }
            foreach (Alias alias in aliases) {
                if (alias.Trigger == cmd) return alias;
            }
            return null;
        }
    }
}
