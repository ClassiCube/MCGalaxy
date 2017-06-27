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
using System.Collections.Generic;
using System.Data;
using MCGalaxy.SQL;

namespace MCGalaxy.Blocks.Extended {
    public static class MessageBlock {
        
        public static bool Handle(Player p, ushort x, ushort y, ushort z, bool alwaysRepeat) {
            if (!p.level.hasMessageBlocks) return false;
            
            try {
                DataTable Messages = Database.Backend.GetRows("Messages" + p.level.name, "*",
                                                              "WHERE X=@0 AND Y=@1 AND Z=@2", x, y, z);
                int last = Messages.Rows.Count - 1;
                if (last == -1) { Messages.Dispose(); return false; }
                
                string message = Messages.Rows[last]["Message"].ToString().Trim();
                message = message.Replace("\\'", "\'");
                message.Cp437ToUnicodeInPlace();
                message = message.Replace("@p", p.name);
                
                if (message != p.prevMsg || (alwaysRepeat || ServerConfig.RepeatMBs)) {
                    string text;
                    List<string> cmds = GetParts(message, out text);
                    if (text != null) Player.Message(p, text);
                    
                    if (cmds.Count == 1) {
                        string[] parts = cmds[0].SplitSpaces(2);
                        p.HandleCommand(parts[0], parts.Length > 1 ? parts[1] : "");
                    } else if (cmds.Count > 0) {
                        p.HandleCommands(cmds);
                    }
                    p.prevMsg = message;
                }
                return true;
            } catch {
                return false;
            }
        }
        
        static string[] sep = new string[] { " |/" };
        const StringSplitOptions opts = StringSplitOptions.RemoveEmptyEntries;
        static List<string> empty = new List<string>();
        internal static List<string> GetParts(string message, out string text) {
            if (message.IndexOf('|') == -1) return ParseSingle(message, out text);
            
            string[] parts = message.Split(sep, opts);
            List<string> cmds = ParseSingle(parts[0], out text);
            if (parts.Length == 1) return cmds;
            
            if (text != null) cmds = new List<string>();
            for (int i = 1; i < parts.Length; i++)
                cmds.Add(parts[i]);
            return cmds;
        }
        
        static List<string> ParseSingle(string message, out string text) {
            if (message[0] == '/') {
                text = null; return new List<string>(){ message.Substring(1) };
            } else {
                text = message; return empty;
            }
        }
    }
}