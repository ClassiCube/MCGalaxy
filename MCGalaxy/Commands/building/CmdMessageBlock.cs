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
using MCGalaxy.Blocks.Extended;
using MCGalaxy.Maths;
using MCGalaxy.SQL;
using MCGalaxy.Util;

namespace MCGalaxy.Commands.Building {
    public sealed class CmdMessageBlock : Command {
        public override string name { get { return "MB"; } }
        public override string shortcut { get { return "MessageBlock"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public override bool SuperUseable { get { return false; } }
        public override CommandPerm[] ExtraPerms {
            get { return new[] { new CommandPerm(LevelPermission.Nobody, "+ can use moderation commands in MBs") }; }
        }

        public override void Use(Player p, string message) {
            if (message.Length == 0) { Help(p); return; }

            bool allMessage = false;
            MBArgs data = new MBArgs();
            string[] args = message.SplitSpaces(2);
            string block = args[0].ToLower();
            data.Block = GetBlock(p, block, ref allMessage);
            if (data.Block.IsInvalid) return;
            if (!CommandParser.IsBlockAllowed(p, "place a message block of", data.Block)) return;
            
            if (allMessage) {
                data.Message = message;
            } else if (args.Length == 1) {
                Player.Message(p, "You need to provide text to put in the messageblock."); return;
            } else {
                data.Message = args[1];
            }
            
            bool allCmds = HasExtraPerm(p, 1);
            if (!MessageBlock.Validate(p, data.Message, allCmds)) return;

            Player.Message(p, "Place where you wish the message block to go.");
            p.MakeSelection(1, data, PlacedMark);
        }
        
        ExtBlock GetBlock(Player p, string name, ref bool allMessage) {
            if (name == "show") { ShowMessageBlocks(p); return ExtBlock.Invalid; }
            ExtBlock block = CommandParser.RawGetBlock(p, name);
            if (!block.IsInvalid && p.level.Props[block.Index].IsMessageBlock)
                return block;
            
            // Hardcoded aliases for backwards compatibility
            block.BlockID = Block.MB_White; block.ExtID = 0;
            if (name == "white") block.BlockID = Block.MB_White;
            if (name == "black") block.BlockID = Block.MB_Black;
            if (name == "air") block.BlockID = Block.MB_Air;
            if (name == "water") block.BlockID = Block.MB_Water;
            if (name == "lava") block.BlockID = Block.MB_Lava;
            
            allMessage = block.BlockID == Block.MB_White && name != "white";
            if (p.level.Props[block.Index].IsMessageBlock) return block;
            
            Help(p); return ExtBlock.Invalid;
        }
        
        bool CheckCommand(Player p, string message) {
            bool allCmds = HasExtraPerm(p, 1);
            string[] parts = message.SplitSpaces(2);
            string alias = parts[0], cmdArgs = "";
            Command.Search(ref alias, ref cmdArgs);
            
            foreach (Command cmd in Command.all.commands) {
                if (p.group.CanExecute(cmd) && (allCmds || !cmd.type.Contains("mod"))) continue;
                
                if (IsCommand(message, cmd.name) || IsCommand(alias, cmd.name)) {
                    Player.Message(p, "You cannot use %T/{0} %Sin a messageblock.", cmd.name); return false;
                }
                if (cmd.shortcut.Length > 0 && IsCommand(message, cmd.shortcut)) {
                    Player.Message(p, "You cannot use %T/{0} %Sin a messageblock.", cmd.name); return false;
                }
            }
            return true;
        }
        
        bool IsCommand(string message, string cmd) {
            return message.CaselessEq(cmd) || message.CaselessStarts(cmd + " ");
        }

        bool PlacedMark(Player p, Vec3S32[] marks, object state, ExtBlock block) {
            ushort x = (ushort)marks[0].X, y = (ushort)marks[0].Y, z = (ushort)marks[0].Z;
            MBArgs args = (MBArgs)state;
            
            ExtBlock old = p.level.GetBlock(x, y, z);
            if (p.level.CheckAffectPermissions(p, x, y, z, old, args.Block)) {
                p.level.UpdateBlock(p, x, y, z, args.Block);
                UpdateDatabase(p, args, x, y, z);
                Player.Message(p, "Message block created.");
            } else {
                Player.Message(p, "Failed to create a message block.");
            }
            return true;
        }
        
        void UpdateDatabase(Player p, MBArgs args, ushort x, ushort y, ushort z) {
            args.Message = args.Message.Replace("'", "\\'");
            args.Message = Colors.Escape(args.Message);
            args.Message = args.Message.UnicodeToCp437();
            
            string lvlName = p.level.name;
            object locker = ThreadSafeCache.DBCache.GetLocker(lvlName);
            
            lock (locker) {
                Database.Backend.CreateTable("Messages" + lvlName, LevelDB.createMessages);
                p.level.hasMessageBlocks = true;
                
                int count = 0;
                using (DataTable Messages = Database.Backend.GetRows("Messages" + lvlName, "*",
                                                                     "WHERE X=@0 AND Y=@1 AND Z=@2", x, y, z)) {
                    count = Messages.Rows.Count;
                }
                
                if (count == 0) {
                    Database.Backend.AddRow("Messages" + lvlName, "X, Y, Z, Message", x, y, z, args.Message);
                } else {
                    Database.Backend.UpdateRows("Messages" + lvlName, "Message=@3",
                                                "WHERE X=@0 AND Y=@1 AND Z=@2", x, y, z, args.Message);
                }
            }
        }

        class MBArgs { public string Message; public ExtBlock Block; }

        
        void ShowMessageBlocks(Player p) {
            p.showMBs = !p.showMBs;
            int count = 0;
            
            if (p.level.hasMessageBlocks) {
                using (DataTable table = Database.Backend.GetRows("Messages" + p.level.name, "*")) {
                    count = table.Rows.Count;
                    if (p.showMBs) { ShowMessageBlocks(p, table); }
                    else { HideMessageBlocks(p, table); }
                }
            }
            Player.Message(p, "Now {0} %SMBs.", p.showMBs ? "showing &a" + count : "hiding");
        }
        
        static void ShowMessageBlocks(Player p, DataTable table) {
            foreach (DataRow row in table.Rows) {
                p.SendBlockchange(U16(row["X"]), U16(row["Y"]), U16(row["Z"]), (ExtBlock)Block.Green);
            }
        }
        
        static void HideMessageBlocks(Player p, DataTable table) {
            foreach (DataRow row in table.Rows) {
                p.RevertBlock(U16(row["X"]), U16(row["Y"]), U16(row["Z"]));
            }
        }
        
        static ushort U16(object x) { return Convert.ToUInt16(x); }

        
        static string Format(ExtBlock block, Level lvl, BlockProps[] props) {
            if (!props[block.Index].IsMessageBlock) return null;
            
            // We want to use the simple aliases if possible
            if (block.BlockID == Block.MB_Black) return "black";
            if (block.BlockID == Block.MB_White) return "white";
            if (block.BlockID == Block.MB_Air)   return "air";
            if (block.BlockID == Block.MB_Lava)  return "lava";
            if (block.BlockID == Block.MB_Water) return "water";
            
            return lvl == null ? Block.Name(block.BlockID) : lvl.BlockName(block);
        }
        
        static void GetAllNames(Player p, List<string> names) {
            GetCoreNames(names, p.level);
            for (int i = Block.CpeCount; i < Block.Count; i++) {
                ExtBlock block = ExtBlock.FromRaw((byte)i);
                string name = Format(block, p.level, p.level.Props);
                if (name != null) names.Add(name);
            }
        }
        
        static void GetCoreNames(List<string> names, Level lvl) {
            BlockProps[] props = lvl != null ? lvl.Props : Block.Props;
            for (int i = Block.Air; i < Block.Count; i++) {
                ExtBlock block = ExtBlock.FromIndex(i);
                if (block.BlockID == Block.custom_block) continue;
                
                string name = Format(block, lvl, props);
                if (name != null) names.Add(name);
            }
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/MB [block] [message]");
            Player.Message(p, "%HPlaces a message in your next block.");
            
            List<string> names = new List<string>();
            if (Player.IsSuper(p)) GetCoreNames(names, null);
            else GetAllNames(p, names);
            
            Player.Message(p, "%H  Supported blocks: %S{0}", names.Join());
            Player.Message(p, "%H  Use | to separate commands, e.g. /say 1 |/say 2");
            Player.Message(p, "%H  Note: \"@p\" is a placeholder for player who clicked.");
            Player.Message(p, "%T/MB show %H- Shows or hides message blocks");
        }
    }
}
