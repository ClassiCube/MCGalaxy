/*
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

namespace MCGalaxy.Commands.Info {
    public sealed class CmdView : Command2 {        
        public override string name { get { return "View"; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool UseableWhenFrozen { get { return true; } }
        
        public override void Use(Player p, string message, CommandData data) {
            if (!Directory.Exists("extra/text/")) 
                Directory.CreateDirectory("extra/text");
            
            if (message.Length == 0) {
                string[] files = Directory.GetFiles("extra/text", "*.txt");
                string all = files.Join(f => Path.GetFileNameWithoutExtension(f));

                if (all.Length == 0) {
                    p.Message("No files are viewable by you");
                } else {
                    p.Message("Available files:");
                    p.Message(all);
                }
            } else {
                message = Path.GetFileName(message);
                
                if (File.Exists("extra/text/" + message + ".txt")) {
                    string[] lines = File.ReadAllLines("extra/text/" + message + ".txt");
                    p.MessageLines(lines);
                } else {
                    p.Message("File specified doesn't exist");
                }
            }
        }
        
        public override void Help(Player p) {
            p.Message("&T/view &H- Lists all files you can view");
            p.Message("&T/view [file] &H- Views [file]'s contents");
        }
    }
}