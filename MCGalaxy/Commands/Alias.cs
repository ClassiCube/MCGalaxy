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

namespace MCGalaxy.Commands {
    
    public class Alias {
        
        public static List<Alias> coreAliases = new List<Alias>();
        public static List<Alias> aliases = new List<Alias>();
        public string Trigger, Target, Format;

        public Alias(string trigger, string target) {
            Trigger = trigger;
            target = target.Trim();
            int space = target.IndexOf(' ');
            
            if (space < 0) {
                Target = target;
            } else {
                Target = target.Substring(0, space);
                Format = target.Substring(space + 1);
            }
        }
        
        public Alias(string trigger, string target, string format) {
            Trigger = trigger; Target = target; Format = format;
        }

        public static void Load() {
            aliases.Clear();
            coreAliases.Clear();
            
            if (!File.Exists(Paths.AliasesFile)) { Save(); return; }
            PropertiesFile.Read(Paths.AliasesFile, LineProcessor, ':');
        }
        
        static void LineProcessor(string key, string value) {
            aliases.Add(new Alias(key, value));
        }

        public static void Save() {
            using (StreamWriter sw = new StreamWriter(Paths.AliasesFile)) {
                sw.WriteLine("# Aliases can be in one of three formats:");
                sw.WriteLine("# trigger : command");
                sw.WriteLine("#    e.g. \"xyz : help\" means /xyz is treated as /help <args given by user>");
                sw.WriteLine("# trigger : command [prefix]");
                sw.WriteLine("#    e.g. \"xyz : help me\" means /xyz is treated as /help me <args given by user>");
                sw.WriteLine("# trigger : command <prefix> {args} <suffix>");
                sw.WriteLine("#    e.g. \"mod : setrank {args} mod\" means /mod is treated as /setrank <args given by user> mod");
                
                foreach (Alias a in aliases) {
                    if (a.Format == null) {
                        sw.WriteLine(a.Trigger + " : " + a.Target);
                    } else {
                        sw.WriteLine(a.Trigger + " : " + a.Target + " " + a.Format);
                    }
                }
            }
        }

        public static Alias Find(string cmd) {
            foreach (Alias alias in aliases) {
                if (alias.Trigger.CaselessEq(cmd)) return alias;
            }
            foreach (Alias alias in coreAliases) {
                if (alias.Trigger.CaselessEq(cmd)) return alias;
            }
            return null;
        }
        
        /// <summary> Registers default aliases specified by a command. </summary>
        internal static void RegisterDefaults(Command cmd) {
            CommandAlias[] aliases = cmd.Aliases;
            if (aliases == null) return;
            
            foreach (CommandAlias a in aliases) {
                Alias alias = new Alias(a.Trigger, cmd.name, a.Format);
                coreAliases.Add(alias);
            }
        }
        
        internal static void UnregisterDefaults(Command cmd) {
            if (cmd.Aliases == null) return;
            coreAliases.RemoveAll(a => a.Target == cmd.name);
        }
    }
}
