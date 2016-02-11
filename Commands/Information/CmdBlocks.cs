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
namespace MCGalaxy.Commands
{
    public sealed class CmdBlocks : Command
    {
        public override string name { get { return "blocks"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public CmdBlocks() { }

        public override void Use(Player p, string message)
        {
            if (message == "")
            {
                Player.SendMessage(p, "Basic blocks: ");
                for (byte i = 0; i < 50; i++)
                {
                    message += ", " + Block.Name(i);
                }
                Player.SendMessage(p, message.Remove(0, 2));
                Player.SendMessage(p, "&d/blocks all <0/1/2/3/4> " + Server.DefaultColor + "will show the rest.");
            }
            else if (message.ToLower() == "all")
            {
                Player.SendMessage(p, "Complex blocks: ");
                for (byte i = 50; i < 255; i++)
                {
                    if (Block.Name(i).ToLower() != "unknown") message += ", " + Block.Name(i);
                }
                Player.SendMessage(p, message.Remove(0, 2));
                Player.SendMessage(p, "Use &d/blocks all <0/1/2/3/4> " + Server.DefaultColor + "for a readable list.");
            }
            else if (message.ToLower().IndexOf(' ') != -1 && message.Split(' ')[0] == "all")
            {
                int foundRange = 0;
                try { foundRange = int.Parse(message.Split(' ')[1]); }
                catch { Player.SendMessage(p, "Incorrect syntax"); return; }

                if (foundRange >= 5 || foundRange < 0) { Player.SendMessage(p, "Number must be between 0 and 4"); return; }

                message = "";
                Player.SendMessage(p, "Blocks between " + foundRange * 51 + " and " + (foundRange + 1) * 51);
                for (byte i = (byte)(foundRange * 51); i < (byte)((foundRange + 1) * 51); i++)
                {
                    if (Block.Name(i).ToLower() != "unknown") message += ", " + Block.Name(i);
                }
                Player.SendMessage(p, message.Remove(0, 2));
            }
            else
            {
                string printMessage = ">>>&b";

                if (Block.Byte(message) != Block.Zero)
                {
                    byte b = Block.Byte(message);
                    if (b < 51)
                    {
                        for (byte i = 51; i < 255; i++)
                        {
                            if (Block.Convert(i) == b)
                                printMessage += Block.Name(i) + ", ";
                        }

                        if (printMessage != ">>>&b")
                        {
                            Player.SendMessage(p, "Blocks which look like \"" + message + "\":");
                            Player.SendMessage(p, printMessage.Remove(printMessage.Length - 2));
                        }
                        else Player.SendMessage(p, "No Complex Blocks look like \"" + message + "\"");
                    }
                    else
                    {
                        Player.SendMessage(p, "&bComplex information for \"" + message + "\":");
                        Player.SendMessage(p, "&cBlock will appear as a \"" + Block.Name(Block.Convert(b)) + "\" block");

                        if (Block.LightPass(b, 0, BlockDefinition.GlobalDefs))
                            Player.SendMessage(p, "Block will allow light through");
                        if (Block.Physics(b)) Player.SendMessage(p, "Block affects physics in some way"); //AFFECT!
                        else Player.SendMessage(p, "Block will not affect physics in any way"); //It's AFFECT!
                        if (Block.NeedRestart(b)) Player.SendMessage(p, "The block's physics will auto-start");

                        if (Block.OPBlocks(b)) Player.SendMessage(p, "Block is unaffected by explosions");

                        if (Block.AllowBreak(b)) Player.SendMessage(p, "Anybody can activate the block");
                        if (Block.Walkthrough(b)) Player.SendMessage(p, "Block can be walked through");
                        if (Block.Death(b)) Player.SendMessage(p, "Walking through block will kill you");

                        if (Block.DoorAirs(b) != (byte)0) Player.SendMessage(p, "Block is an ordinary door");
                        if (Block.tDoor(b)) Player.SendMessage(p, "Block is a tdoor, which allows other blocks through when open");
                        if (Block.odoor(b) != Block.Zero) Player.SendMessage(p, "Block is an odoor, which toggles (GLITCHY)");

                        if (Block.Mover(b)) Player.SendMessage(p, "Block can be activated by walking through it");
                    }
                }
                else if (Group.Find(message) != null)
                {
                    LevelPermission Perm = Group.Find(message).Permission;
                    foreach (Block.Blocks bL in Block.BlockList)
                    {
                        if (Block.canPlace(Perm, bL.type) && Block.Name(bL.type).ToLower() != "unknown") printMessage += Block.Name(bL.type) + ", ";
                    }

                    if (printMessage != ">>>&b")
                    {
                        Player.SendMessage(p, "Blocks which " + Group.Find(message).color + Group.Find(message).name + Server.DefaultColor + " can place: ");
                        Player.SendMessage(p, printMessage.Remove(printMessage.Length - 2));
                    }
                    else Player.SendMessage(p, "No blocks are specific to this rank");
                }
                else if (message.IndexOf(' ') == -1)
                {
                    if (message.ToLower() == "count") Player.SendMessage(p, "Blocks in this map: " + p.level.blocks.Length);
                    else Help(p);
                }
                else if (message.Split(' ')[0].ToLower() == "count")
                {
                    int foundNum = 0; byte foundBlock = Block.Byte(message.Split(' ')[1]);
                    if (foundBlock == Block.Zero) { Player.SendMessage(p, "Could not find block specified"); return; }

                    for (int i = 0; i < p.level.blocks.Length; i++)
                    {
                        if (foundBlock == p.level.blocks[i]) foundNum++;
                    }

                    if (foundNum == 0) Player.SendMessage(p, "No blocks were of type \"" + message.Split(' ')[1] + "\"");
                    else if (foundNum == 1) Player.SendMessage(p, "1 block was of type \"" + message.Split(' ')[1] + "\"");
                    else Player.SendMessage(p, foundNum.ToString() + " blocks were of type \"" + message.Split(' ')[1] + "\"");
                }
                else
                {
                    Player.SendMessage(p, "Unable to find block or rank");
                }
            }
        }
        
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/blocks - Lists all basic blocks");
            Player.SendMessage(p, "/blocks all - Lists all complex blocks");
            Player.SendMessage(p, "/blocks [basic block] - Lists all blocks which look the same");
            Player.SendMessage(p, "/blocks [complex block] - Lists specific information on block");
            Player.SendMessage(p, "/blocks <rank> - Lists all blocks <rank> can use");
            Player.SendMessage(p, ">> " + Group.concatList());
            Player.SendMessage(p, "/blocks count <block> - Finds total count for <block> in map");
        }
    }
}
