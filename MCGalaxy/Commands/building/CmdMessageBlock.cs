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
using MCGalaxy.Blocks;
using MCGalaxy.SQL;

namespace MCGalaxy.Commands.Building {
    public sealed class CmdMessageBlock : Command {
        public override string name { get { return "mb"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public CmdMessageBlock() { }
        public override CommandPerm[] ExtraPerms {
            get { return new[] { new CommandPerm(LevelPermission.Nobody, "+ can use moderation commands in MBs") }; }
        }

        public override void Use(Player p, string message) {
            if (message == "") { Help(p); return; }

            MBData cpos;
            cpos.message = null;
            string[] args = message.SplitSpaces(2);
            switch (args[0].ToLower()) {
                    case "air": cpos.type = Block.MsgAir; break;
                    case "water": cpos.type = Block.MsgWater; break;
                    case "lava": cpos.type = Block.MsgLava; break;
                    case "black": cpos.type = Block.MsgBlack; break;
                    case "white": cpos.type = Block.MsgWhite; break;
                    case "show": ShowMessageBlocks(p); return;
                    default: cpos.type = Block.MsgWhite; cpos.message = message; break;
            }
            if (args.Length == 1) {
                Player.Message(p, "You need to provide text to put in the messageblock."); return;
            }
            if (cpos.message == null) cpos.message = args[1];
            
            string text;
            List<string> cmds = WalkthroughBehaviour.ParseMB(cpos.message, out text);
            foreach (string cmd in cmds) {
                if (!CheckCommand(p, cmd)) return;
            }

            p.blockchangeObject = cpos;
            Player.Message(p, "Place where you wish the message block to go."); p.ClearBlockchange();
            p.Blockchange += PlacedMark;
        }
        
        bool CheckCommand(Player p, string message) {
            bool allCmds = CheckExtraPerm(p);
            string[] parts = message.SplitSpaces(2);
            string alias = parts[0], cmdArgs = "";
            Command.Search(ref alias, ref cmdArgs);
            
            foreach (Command cmd in Command.all.commands) {
                if (cmd.defaultRank <= p.Rank && (allCmds || !cmd.type.Contains("mod"))) continue;
                
                if (IsCommand(message, cmd.name) || IsCommand(alias, cmd.name)) {
                    p.SendMessage("You cannot use that command in a messageblock."); return false;
                }
                if (cmd.shortcut != "" && IsCommand(message, cmd.shortcut)) {
                    p.SendMessage("You cannot use that command in a messageblock."); return false;
                }
            }
            return true;
        }
        
        bool IsCommand(string message, string cmd) {
            return message.CaselessEq(cmd) || message.CaselessStarts(cmd + " ");
        }

        void PlacedMark(Player p, ushort x, ushort y, ushort z, byte type, byte extType) {
            p.ClearBlockchange();
            MBData cpos = (MBData)p.blockchangeObject;
            
            byte old = p.level.GetTile(x, y, z);
            if (p.level.CheckAffectPermissions(p, x, y, z, old, cpos.type, 0)) {
                p.level.Blockchange(p, x, y, z, cpos.type, 0);
                p.SendBlockchange(x, y, z, cpos.type, 0); // for when same block type but different message
                UpdateDatabase(p, cpos, x, y, z);
                Player.Message(p, "Message block created.");
            } else {
                p.RevertBlock(x, y, z);
                Player.Message(p, "Failed to create a message block.");
            }

            if (p.staticCommands)
                p.Blockchange += PlacedMark;
        }
        
        void UpdateDatabase(Player p, MBData data, ushort x, ushort y, ushort z) {
            data.message = data.message.Replace("'", "\\'");
            data.message = Colors.EscapeColors(data.message);
            //safe against SQL injections because no user input is given here
            string lvlName = p.level.name;
            object locker = ThreadSafeCache.DBCache.Get(lvlName);
            
            lock (locker) {
                Database.Backend.CreateTable("Messages" + lvlName, LevelDB.createMessages);
                
                int count = 0;
                using (DataTable Messages = Database.Backend.GetRows("Messages" + lvlName, "*",
                                                                     "WHERE X=@0 AND Y=@1 AND Z=@2", x, y, z)) {
                    count = Messages.Rows.Count;
                }
                
                string syntax = count == 0 ?
                    "INSERT INTO `Messages" + lvlName + "` (X, Y, Z, Message) VALUES (@0, @1, @2, @3)"
                    : "UPDATE `Messages" + lvlName + "` SET Message=@3 WHERE X=@0 AND Y=@1 AND Z=@2";
                Database.Execute(syntax, x, y, z, data.message);
            }
        }

        struct MBData { public string message; public byte type; }

        void ShowMessageBlocks(Player p) {
            p.showMBs = !p.showMBs;
            using (DataTable table = Database.Backend.GetRows("Messages" + p.level.name, "*")) {
                if (p.showMBs) {
                    ShowMessageBlocks(p, table);
                } else {
                    HideMessageBlocks(p, table);
                }
            }
        }
        
        static void ShowMessageBlocks(Player p, DataTable table) {
            foreach (DataRow row in table.Rows) {
                p.SendBlockchange(U16(row["X"]), U16(row["Y"]), U16(row["Z"]), Block.green);
            }
            Player.Message(p, "Now showing &a" + table.Rows.Count + " %SMBs.");
        }
        
        static void HideMessageBlocks(Player p, DataTable table) {
            foreach (DataRow row in table.Rows) {
                p.RevertBlock(U16(row["X"]), U16(row["Y"]), U16(row["Z"]));
            }
            Player.Message(p, "Now hiding MBs.");
        }
        
        static ushort U16(object x) { return Convert.ToUInt16(x); }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/mb [block] [message]");
            Player.Message(p, "%HPlaces a message in your next block.");
            Player.Message(p, "%H  Supported blocks: %Swhite, black, air, water, lava");
            Player.Message(p, "%H  Use | to separate commands, e.g. /say 1 |/say 2");
            Player.Message(p, "%H  Note: \"@p\" is a placeholder for player who clicked.");
            Player.Message(p, "%T/mb show %H- Shows or hides MBs");
        }
    }
}
