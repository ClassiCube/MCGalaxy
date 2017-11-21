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

namespace MCGalaxy.Util {
    
    /// <summary> Represents a text file and associated data for it. </summary>
    public sealed class TextFile {
        public readonly string Filename;
        public readonly string[] DefaultText;        
        public Action OnTextChanged;
        
        public TextFile(string filename, params string[] defaultText) {
            Filename = filename;
            DefaultText = defaultText;
        }
        
        public void EnsureExists() {
            if (File.Exists(Filename)) return;
            
            Logger.Log(LogType.SystemActivity, Filename + " does not exist, creating");
            using (StreamWriter w = new StreamWriter(Filename)) {
                if (DefaultText == null) return;

                for (int i = 0; i < DefaultText.Length; i++) {
                    w.WriteLine(DefaultText[i]);
                }
            }
        }
        
        public string[] GetText() {
            return File.ReadAllLines(Filename);
        }
        
        public void SetText(string[] text) {
            File.WriteAllLines(Filename, text);
            if (OnTextChanged != null) OnTextChanged();
        }
        

        public static Dictionary<string, TextFile> Files = new Dictionary<string, TextFile>() {
            { "News", new TextFile(Paths.NewsFile, "News have not been created. Put News in '" + Paths.NewsFile + "'.") },
            { "FAQ", new TextFile(Paths.FaqFile, 
                                  "Example: What does this server run on? This server runs on &b" + Server.SoftwareName) },
            { "Rules", new TextFile(Paths.RulesFile, "No rules entered yet!") },
            { "OpRules", new TextFile(Paths.OprulesFile, "No oprules entered yet!") },
            { "Custom $s", new TextFile(Paths.CustomTokensFile,
                                        "// This is used to create custom chat tokens",
                                        "// Lines starting with // are ignored",
                                        "// Lines should be formatted like this:",
                                        "// $website:http://example.org",
                                        "// That would replace '$website' in any message to 'http://example.org'") },
            { "Welcome", new TextFile(Paths.WelcomeFile, "Welcome to my server!") },
            { "Eat", new TextFile(Paths.EatMessagesFile, "guzzled a grape", "chewed a cherry", "ate an avocado") },
            { "Profanity filter", new TextFile(Paths.BadWordsFile,
                                               "# This file is a list of words to remove via the profanity filter",
                                               "# Each word to remove must be on an individual line") },
            { "Announcements", new TextFile(Paths.AnnouncementsFile, null) },
            { "Joker", new TextFile(Paths.JokerFile, null) },
            { "8ball", new TextFile(Paths.EightBallFile, 
                                    "Not likely.", "Very likely.", "Impossible!", "No.", 
                                    "Yes.", "Definitely!", "Do some more thinking.") },
        };
    }
}
