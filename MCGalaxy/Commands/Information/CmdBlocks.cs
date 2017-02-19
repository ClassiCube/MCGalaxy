/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
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
using MCGalaxy.Blocks;

namespace MCGalaxy.Commands
{
    public sealed class CmdBlocks : Command
    {
        public override string name { get { return "blocks"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("materials") }; }
        }
        public CmdBlocks() { }

        public override void Use(Player p, string message) {
            string[] args = message.Split(' ');
            string modifier = args.Length > 1 ? args[1] : "";
            
            if (args[0] == "" || args[0].CaselessEq("basic")) {
                Player.Message(p, "Basic blocks: ");
                MultiPageOutput.Output(p, BasicBlocks(), FormatBlockName,
                                       "blocks basic", "blocks", modifier, false);
            } else if (args[0].CaselessEq("all") || args[0].CaselessEq("complex")) {
                Player.Message(p, "Complex blocks: ");
                MultiPageOutput.Output(p, ComplexBlocks(), FormatBlockName,
                                       "blocks complex", "blocks", modifier, false);
            } else if (Block.Byte(args[0]) != Block.Invalid) {
                OutputBlockData(p, args[0]);
            } else if (Group.Find(args[0]) != null) {
                Group grp = Group.Find(args[0]);
                Player.Message(p, "Blocks which {0} %Scan place: ", grp.ColoredName);
                MultiPageOutput.Output(p, RankBlocks(grp.Permission), FormatBlockName,
                                       "blocks " + args[0], "blocks", modifier, false);
            } else if (args.Length > 1) {
                Help(p);
            } else {
                Player.Message(p, "Unable to find block or rank");
            }
        }
        
        static List<string> BasicBlocks() {
            List<string> items = new List<string>(Block.CpeCount);
            for (int i = 0; i < Block.CpeCount; i++)
                items.Add(Block.Props[i].Name);
            return items;
        }
        
        static List<string> ComplexBlocks() {
            List<string> items = new List<string>(256);
            for (int i = Block.CpeCount; i < Block.Invalid; i++) {
                if (Block.Props[i].Name.CaselessEq("unknown")) continue;
                items.Add(Block.Props[i].Name);
            }
            return items;
        }
        
        static List<string> RankBlocks(LevelPermission perm) {
            List<string> items = new List<string>(256);
            foreach (Block.Blocks bl in Block.BlockList) {
                if (!Block.canPlace(perm, bl.type)) continue;
                if (Block.Name(bl.type).CaselessEq("unknown")) continue;
                items.Add(Block.Name(bl.type));
            }
            return items;
        }
        
        
        internal static string FormatBlockName(string block) {
            Block.Blocks perms = Block.BlockList[Block.Byte(block)];
            return Group.GetColor(perms.lowestRank) + block;
        }
        
        static void OutputBlockData(Player p, string block) {
            byte b = Block.Byte(block);
            if (b >= Block.CpeCount) {
                Player.Message(p, "&bComplex information for \"{0}\":", block);
                Player.Message(p, "&cBlock will appear as a \"{0}\" block", Block.Name(Block.Convert(b)));
                OutputBlockProps(p, b);
                return;
            }
            
            string msg = "";
            for (byte i = Block.CpeCount; i < Block.Invalid; i++) {
                if (Block.Convert(i) == b)
                    msg += Block.Name(i) + ", ";
            }

            if (msg != "") {
                Player.Message(p, "Blocks which look like \"{0}\":", block);
                Player.Message(p, msg.Remove(msg.Length - 2));
            } else {
                Player.Message(p, "No Complex Blocks look like \"{0}\"", block);
            }
        }
        
        static void OutputBlockProps(Player p, byte b) {
            BlockProps props = Block.Props[b];

            if (Block.LightPass(b, 0, BlockDefinition.GlobalDefs))
                Player.Message(p, "Block will allow light through");
            if (Block.Physics(b))
                Player.Message(p, "Block affects physics in some way"); //AFFECT!
            else
                Player.Message(p, "Block will not affect physics in any way"); //It's AFFECT!
            if (Block.NeedRestart(b)) Player.Message(p, "The block's physics will auto-start");

            if (props.OPBlock) Player.Message(p, "Block is unaffected by explosions");

            if (Block.AllowBreak(b)) Player.Message(p, "Anybody can activate the block");
            if (Block.Walkthrough(b)) Player.Message(p, "Block can be walked through");
            if (props.KillerBlock) Player.Message(p, "Walking through block will kill you");

            if (props.IsDoor) Player.Message(p, "Block is an ordinary door");
            if (props.IsTDoor) Player.Message(p, "Block is a tdoor, which allows other blocks through when open");
            if (props.ODoorId != Block.Invalid) Player.Message(p, "Block is an odoor, which toggles (GLITCHY)");

            if (Block.Mover(b)) Player.Message(p, "Block can be activated by walking through it");
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/blocks %H- Lists all basic blocks");
            Player.Message(p, "%T/blocks complex %H- Lists all complex blocks");
            Player.Message(p, "%T/blocks [basic block] %H- Lists all blocks which look the same");
            Player.Message(p, "%T/blocks [complex block] %H- Lists specific info on that block");
            Player.Message(p, "%T/blocks [rank] %H- Lists all blocks [rank] can use");
            Player.Message(p, "  %HSee %T/viewranks %Hfor a list of ranks");
        }
    }
}
