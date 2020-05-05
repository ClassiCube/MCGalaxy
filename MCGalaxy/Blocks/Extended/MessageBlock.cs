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
using MCGalaxy.Maths;
using MCGalaxy.SQL;

namespace MCGalaxy.Blocks.Extended {
    public static class MessageBlock {
        
        public static bool Handle(Player p, ushort x, ushort y, ushort z, bool alwaysRepeat) {
            if (!p.level.hasMessageBlocks) return false;
            
            string message = Get(p.level.MapName, x, y, z);
            if (message == null) return false;
            message = message.Replace("@p", p.name);
            
            if (message != p.prevMsg || (alwaysRepeat || Server.Config.RepeatMBs)) {
                Execute(p, message, new Vec3S32(x, y, z));
            }
            return true;
        }
        
        public static void Execute(Player p, string message, Vec3S32 mbCoords) {
            string text;
            List<string> cmds = GetParts(message, out text);
            if (text != null) p.Message(text);
            
            CommandData data = p.DefaultCmdData;
            data.Context  = CommandContext.MessageBlock;
            data.MBCoords = mbCoords;
            
            if (cmds.Count == 1) {
                string[] parts = cmds[0].SplitSpaces(2);
                string args = parts.Length > 1 ? parts[1] : "";
                p.HandleCommand(parts[0], args, data);
            } else if (cmds.Count > 0) {
                p.HandleCommands(cmds, data);
            }
            p.prevMsg = message;
        }
        
        public static bool Validate(Player p, string message, bool allCmds) {
            string text;
            List<string> cmds = MessageBlock.GetParts(message, out text);
            foreach (string cmd in cmds) {
                if (!CheckCommand(p, cmd, allCmds)) return false;
            }
            return true;
        }
        
        static bool CheckCommand(Player p, string message, bool allCmds) {
            string[] parts = message.SplitSpaces(2);
            string cmdName = parts[0], cmdArgs = "";
            Command.Search(ref cmdName, ref cmdArgs);
            
            Command cmd = Command.Find(cmdName);
            if (cmd == null) return true;
            bool mbUseable = !cmd.MessageBlockRestricted && !cmd.type.CaselessContains("mod");
            
            if (p.CanUse(cmd) && (allCmds || mbUseable)) return true;
            p.Message("You cannot use %T/{0} %Sin a messageblock.", cmd.name);
            return false;
        }
        
        static string[] sep = new string[] { " |/" };
        const StringSplitOptions opts = StringSplitOptions.RemoveEmptyEntries;
        static List<string> empty = new List<string>();
        static List<string> GetParts(string message, out string text) {
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


        public static List<Vec3U16> GetAllCoords(string map) {
            List<Vec3U16> coords = new List<Vec3U16>();
            Database.Backend.ReadRows("Messages" + map, "X,Y,Z", coords, Portal.ReadCoords);
            return coords;
        }
        
        public static string Get(string map, ushort x, ushort y, ushort z) {
            string msg = Database.ReadString("Messages" + map, "Message",
                                             "WHERE X=@0 AND Y=@1 AND Z=@2", x, y, z);
            if (msg == null) return null;

            msg = msg.Trim().Replace("\\'", "\'");
            msg = msg.Cp437ToUnicode();
            return msg;
        }
        
        public static void Delete(string map, ushort x, ushort y, ushort z) {
            Database.Backend.DeleteRows("Messages" + map,
                                        "WHERE X=@0 AND Y=@1 AND Z=@2", x, y, z);
        }
        
        public static void Set(string map, ushort x, ushort y, ushort z, string contents) {
            contents = contents.Replace("'", "\\'");
            contents = Colors.Escape(contents);
            contents = contents.UnicodeToCp437();
            
            Database.Backend.CreateTable("Messages" + map, LevelDB.createMessages);            
            int count = Database.CountRows("Messages" + map,
                                           "WHERE X=@0 AND Y=@1 AND Z=@2", x, y, z);
            
            if (count == 0) {
                Database.Backend.AddRow("Messages" + map, "X, Y, Z, Message", x, y, z, contents);
            } else {
                Database.Backend.UpdateRows("Messages" + map, "Message=@3",
                                            "WHERE X=@0 AND Y=@1 AND Z=@2", x, y, z, contents);
            }
            
            Level lvl = LevelInfo.FindExact(map);
            if (lvl != null) lvl.hasMessageBlocks = true;
        }
    }
}