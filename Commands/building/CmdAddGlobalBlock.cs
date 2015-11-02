/*
	Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
	
	Dual-licensed under the	Educational Community License, Version 2.0 and
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
using System.Collections.Generic;
namespace MCGalaxy.Commands
{
    public sealed class CmdAddGlobalBlock : Command
    {
        public override string name { get { return "addglobalblock"; } }
        public override string shortcut { get { return "agb"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        public CmdAddGlobalBlock() { }

        public byte step = 0;

        public override void Use(Player p, string message)
        {
            if (p == null)
            {
                Player.SendMessage(p, "This command can only be used in-game!");
                return;
            }
            if (Server.Blocks != null)
            {
                if (Server.Blocks.Count > 10)
                {
                    Player.SendMessage(p, "You must remove a block before adding more. (Max is 10 global blocks)");
                    return;
                }
            }
            p.ClearChat();
            step = 1;
            p.OnChat += ChatEvent;
            p.SendMessage("Say 'continue' to proceed");
            p.SendMessage("Say 'abort' anytime to abort the processs");
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/addglobalblock - Adds a server-wide block");
        }
        BlockDefinitions bd;
        public void ChatEvent(Player p, string message)
        {
            p.SendMessage("&a==================================");
            p.cancelchat = true;
            message = message.ToLower();
            if (message == "abort")
            {
                p.ClearChat();
                p.SendMessage("Aborted.");
                return;
            }
            if (step == 1)
            {
                if (message == "continue")
                {
                    step = 2;
                    p.SendMessage("Say the name of this block");
                    bd = new BlockDefinitions();
                }
                else
                {
                    p.ClearChat();
                    p.SendMessage("Aborted. Use command again");
                    return;
                }
                return;
            }
            if (step == 2)
            {
                bd.Name = message.Capitalize();
                step = 3;
                p.SendMessage("Say '0' if this block is walk-through.");
                p.SendMessage("Say '1' if this block is swim-through.");
                p.SendMessage("Say '2' if this block is solid.");
                return;
            }
            if(step == 3)
            {
                if(message == "0" || message == "1" || message == "2")
                {
                    bd.Solidity = byte.Parse(message);
                    step = 4;
                    p.SendMessage("Say a number between 0 (0.25% speed) and 255 (396% speed).");
                    p.SendMessage("This speed is used inside or swimming in the block. Or when you are walking on it.");
                }
                else
                {
                    p.SendMessage("Say '0' if this block is walk-through.");
                    p.SendMessage("Say '1' if this block is swim-through.");
                    p.SendMessage("Say '2' if this block is solid.");
                }
                return;
            }
            if(step == 4)
            {
                bool result = byte.TryParse(message, out bd.MovementSpeed);
                if(result)
                {
                    step = 5;
                    p.SendMessage("Say a number between 0-255 to identify which texture tile to use for the top of the block.");
                    p.SendMessage("Textures tile numbers are left to right in terrain.png (The file the textures are located).");
                }
                else
                {
                    p.SendMessage("Say a number between 0 (0.25% speed) and 255 (396% speed).");
                    p.SendMessage("This speed is used inside or swimming in the block. Or when you are walking on it.");
                }
                return;
            }
            if(step == 5)
            {
                bool result = byte.TryParse(message, out bd.TopT);
                if (result)
                {
                    step = 40;
                    p.SendMessage("Say a number between 0-255 to identify which texture tile to use for the sides of the block.");
                    p.SendMessage("Textures tile numbers are left to right in terrain.png (The file the textures are located).");
                }
                else
                {
                    p.SendMessage("Say a number between 0-255 to identify which texture tile to use for the top of the block.");
                    p.SendMessage("Textures tile numbers are left to right in terrain.png (The file the textures are located).");
                }
                return;
            }
            //I forgot this step so I'm going to insert it here
            if (step == 40)
            {
                bool result = byte.TryParse(message, out bd.SideT);
                if (result)
                {
                    step = 6;
                    p.SendMessage("Say a number between 0-255 to identify which texture tile to use for the bottom of the block.");
                    p.SendMessage("Textures tile numbers are left to right in terrain.png (The file the textures are located).");
                }
                else
                {
                    p.SendMessage("Say a number between 0-255 to identify which texture tile to use for the sides of the block.");
                    p.SendMessage("Textures tile numbers are left to right in terrain.png (The file the textures are located).");
                }
                return;
            }
            if (step == 6)
            {
                bool result = byte.TryParse(message, out bd.BottomT);
                if (result)
                {
                    step = 7;
                    p.SendMessage("Say '0' if this block blocks light. Say '1' if it doesn't");
                }
                else
                {
                    p.SendMessage("Say a number between 0-255 to identify which texture tile to use for the bottom of the block.");
                    p.SendMessage("Textures tile numbers are left to right in terrain.png (The file the textures are located).");
                }
                return;
            }
            if (step == 7)
            {
                if (message == "0" || message == "1")
                {
                    bd.TransmitsLight = byte.Parse(message);
                    step = 8;
                    p.SendMessage("Say a number between 0 and 11 to represent the sound heard when walking on it.");
                    p.SendMessage("0 = None; 1 = Powder; 2 = Wood; 3 = Gravel; 4 = Grass; 5 = Stone; 6 = Metal;");
                    p.SendMessage("7 = Glass; 8 = Cloth; 9 = Sand; 10 = Snow; 11 = Ladder");
                }
                else
                {
                    p.SendMessage("Say '0' if this block blocks light. Say '1' if it doesn't");
                }
                return;
            }
            if (step == 8)
            {
                bool result = byte.TryParse(message, out bd.WalkSound);
                if(!result || bd.WalkSound > 11)
                {
                    p.SendMessage("Say a number between 0 and 11 to represent the sound heard when walking on it.");
                    p.SendMessage("0 = None; 1 = Powder; 2 = Wood; 3 = Gravel; 4 = Grass; 5 = Stone; 6 = Metal;");
                    p.SendMessage("7 = Glass; 8 = Cloth; 9 = Sand; 10 = Snow; 11 = Ladder");
                }
                else
                {
                    step = 9;
                    p.SendMessage("Say '0' if you want the block to be darkened when in shadows. Or 1 if not (Like lava).");
                }
                return;
            }
            if (step == 9)
            {
                if (message == "0" || message == "1")
                {
                    bd.TransmitsLight = byte.Parse(message);
                    step = 10;
                    p.SendMessage("What is the block's shape?");
                    p.SendMessage("1 = Cube; 2 = Slab; 3 = Upside-down Slab; 4 = Sprite");
                }
                else
                {
                    p.SendMessage("Say '0' if you want the block to be darkened when in shadows. Or 1 if not (Like lava).");
                }
                return;
            }
            if (step == 10)
            {
                bool result = byte.TryParse(message, out bd.Shape);
                if(!result || bd.Shape == 0 || bd.Shape > 4)
                {
                    p.SendMessage("What is the block's shape?");
                    p.SendMessage("1 = Cube; 2 = Slab; 3 = Upside-down Slab; 4 = Sprite");
                }
                else
                {
                    step = 11;
                    p.SendMessage("Define the block's transparency.");
                    p.SendMessage("0 = Opaque; 1 = Transparent (Like glass)");
                    p.SendMessage("2 = Transparent (Like leaves); 3 = Translucent (Like ice)");
                }
                return;
            }
            if (step == 11)
            {
                if (message == "0" || message == "1" || message == "2" || message == "3" || message == "4")
                {
                    bd.BlockDraw = byte.Parse(message);
                    step = 12;
                    p.SendMessage("Define the block's fog density (The density of it inside, i.e water, lava");
                    p.SendMessage("0 = No density; 1-255 = Less to greater density");
                }
                else
                {
                    p.SendMessage("Define the block's transparency.");
                    p.SendMessage("0 = Opaque; 1 = Transparent (Like glass)");
                    p.SendMessage("2 = Transparent (Like leaves); 3 = Translucent (Like ice)");
                }
                return;
            }
            if (step == 12)
            {
                bool result = byte.TryParse(message, out bd.FogD);
                if (result)
                {
                    step = 13;
                    if(bd.FogD == 0)
                    {
                        bd.FogR = 0;
                        bd.FogG = 0;
                        bd.FogB = 0;
                        step = 16;
                        p.SendMessage("Define a fallback for this block (Clients that can't see this block).");
                        p.SendMessage("You can use the block name or block ID");
                    }
                    else
                    {
                        p.SendMessage("Define the fog's red value of its RGB (0-255)");
                    }
                }
                else
                {
                    p.SendMessage("Define the block's fog density (The density of it inside, i.e water, lava");
                    p.SendMessage("0 = No density; 1-255 = Less to greater density");
                }
                return;
            }
            if (step == 13)
            {
                bool result = byte.TryParse(message, out bd.FogR);
                if (result)
                {
                    step = 14;
                    p.SendMessage("Define the fog's green value of its RGB (0-255)");
                }
                else
                {
                    p.SendMessage("Define the fog's red value of its RGB (0-255)");
                }
                return;
            }
            if (step == 14)
            {
                bool result = byte.TryParse(message, out bd.FogG);
                if (result)
                {
                    step = 15;
                    p.SendMessage("Define the fog's blue value of its RGB (0-255)");
                }
                else
                {
                    p.SendMessage("Define the fog's green value of its RGB (0-255)");
                }
                return;
            }
            if (step == 15)
            {
                bool result = byte.TryParse(message, out bd.FogB);
                if (result)
                {
                    step = 16;
                    p.SendMessage("Define a fallback for this block (Clients that can't see this block).");
                    p.SendMessage("You can use the block name or block ID");
                }
                else
                {
                    p.SendMessage("Define the fog's blue value of its RGB (0-255)");
                }
                return;
            }
            if (step == 16)
            {
                if(Block.Byte(message) != Block.Zero)
                {
                    bd.FallBack = Block.Byte(message);
                    p.SendMessage("Creating block " + bd.Name);
                    p.ClearChat();

                    byte id = 128;
                    //In case a hazardous plugin modifies this
                    if (Server.Blocks != null)
                    {
                        //Run 10 times to be secure that IDs don't intersect
                        for (int i = 0; i < 10; i++)
                        {
                            foreach (BlockDefinitions BD in Server.Blocks)
                            {
                                if (id == BD.ID)
                                    id += 1;
                            }
                        }
                    }
                    else
                    {
                        Server.Blocks = new List<BlockDefinitions>();
                    }
                    bd.ID = id;
                    Server.Blocks.Add(bd);
                    BlockDefinitionsJSON.Write(Server.Blocks, "blocks.json");
                    foreach(Player pl in Player.players)
                    {
                        if(pl.HasExtension("BlockDefinitions"))
                            pl.SendBlockDefinitions(bd);
                    }
                    return;
                }
                else
                {
                    p.SendMessage("Define a fallback for this block (Clients that can't see this block).");
                    p.SendMessage("You can use the block name or block ID");
                }
                return;
            }
        }
    }
}
