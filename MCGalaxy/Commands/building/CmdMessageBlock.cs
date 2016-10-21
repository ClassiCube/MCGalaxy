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

            bool allMessage = false;
            MBData data;
            string[] args = message.SplitSpaces(2);
            string block = args[0].ToLower();
            data.Block = GetBlock(p, block, out data.ExtBlock, ref allMessage);
            
            if (allMessage) {
                data.Message = message;
            } else if (args.Length == 1) {
                Player.Message(p, "You need to provide text to put in the messageblock."); return;
            } else {
                data.Message = args[1];
            }
            
            string text;
            List<string> cmds = WalkthroughBehaviour.ParseMB(data.Message, out text);
            foreach (string cmd in cmds) {
                if (!CheckCommand(p, cmd)) return;
            }

            p.blockchangeObject = data;
            Player.Message(p, "Place where you wish the message block to go."); 
            p.ClearBlockchange();
            p.Blockchange += PlacedMark;
        }
        
        byte GetBlock(Player p, string name, out byte extBlock, 
                      ref bool allMessage) {
            extBlock = 0;
            byte id = Block.Byte(name);
            if (Block.Props[id].IsMessageBlock) return id;
            if (name == "show") { ShowMessageBlocks(p); return Block.Invalid; }
            
            id = BlockDefinition.GetBlock(name, p);
            if (p.level.CustomBlockProps[id].IsMessageBlock) {
                extBlock = id; return Block.custom_block;
            }
            
            // Hardcoded aliases for backwards compatibility
            id = Block.MsgWhite;
            if (name == "white") id = Block.MsgWhite;      
            if (name == "black") id = Block.MsgBlack;
            if (name == "air") id = Block.MsgAir;
            if (name == "water") id = Block.MsgWater;
            if (name == "lava") id = Block.MsgLava;
            
            allMessage = id == Block.MsgWhite && name != "white";
            if (!Block.Props[id].IsMessageBlock) { Help(p); return Block.Invalid; }
            return id;
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
            MBData data = (MBData)p.blockchangeObject;
            
            byte old = p.level.GetTile(x, y, z);
            if (p.level.CheckAffectPermissions(p, x, y, z, old, data.Block, data.ExtBlock)) {
                p.level.Blockchange(p, x, y, z, data.Block, data.ExtBlock);
                UpdateDatabase(p, data, x, y, z);
                Player.Message(p, "Message block created.");
            } else {                
                Player.Message(p, "Failed to create a message block.");
            }
            p.RevertBlock(x, y, z);

            if (p.staticCommands) p.Blockchange += PlacedMark;
        }
        
        void UpdateDatabase(Player p, MBData data, ushort x, ushort y, ushort z) {
            data.Message = data.Message.Replace("'", "\\'");
            data.Message = Colors.EscapeColors(data.Message);
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
                Database.Execute(syntax, x, y, z, data.Message);
            }
        }

        struct MBData { public string Message; public byte Block, ExtBlock; }

        
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

        
        static string Format(BlockProps props) {
            if (!props.IsMessageBlock) return null;
            
            // We want to use the simple aliases if possible
            if (Check(props, Block.MsgBlack, "black")) return "black";
            if (Check(props, Block.MsgWhite, "white")) return "white";
            if (Check(props, Block.MsgAir, "air")) return "air";
            if (Check(props, Block.MsgLava, "lava")) return "lava";
            if (Check(props, Block.MsgWater, "water")) return "water";
            return props.Name;
        }
        
        static bool Check(BlockProps props, byte id, string name) {
            if (props.BlockId != id) return false;
            if (props.Name == "unknown") return false;
            id = Block.Byte(name);
            return !Block.Props[id].IsMessageBlock;
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/mb [block] [message]");
            Player.Message(p, "%HPlaces a message in your next block.");
            
            var allProps = Block.Props;
            Player.Message(p, "%H  Supported blocks: %S{0}",
                           allProps.Join(props => Format(props)));
            Player.Message(p, "%H  Use | to separate commands, e.g. /say 1 |/say 2");
            Player.Message(p, "%H  Note: \"@p\" is a placeholder for player who clicked.");
            Player.Message(p, "%T/mb show %H- Shows or hides MBs");
        }
    }
}
