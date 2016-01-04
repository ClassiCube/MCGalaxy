/*
    Copyright 2015 MCGalaxy
        
    Dual-licensed under the    Educational Community License, Version 2.0 and
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

        int step = 0;
        BlockDefinition bd;

        public override void Use(Player p, string message) {
            if (p == null) {
                Player.SendMessage(p, "This command can only be used in-game!");
                return;
            }

            p.ClearChat();
            step = 1;
            p.OnChat += ChatEvent;
            p.SendMessage("Say 'continue' to proceed");
            p.SendMessage("Say 'abort' anytime to abort the processs");
        }
        
        void ChatEvent(Player p, string value) {
            p.SendMessage("&a==================================");
            p.cancelchat = true;
            value = value.ToLower();
            
            if (value == "abort") {
                p.ClearChat();
                p.SendMessage("Aborted.");
                bd = null;
                return;
            }
            if (step == 1) {
                if (value == "continue") {
                    step++;
                    SendStepHelp(p, step);
                    bd = new BlockDefinition();
                } else {
                    p.ClearChat();
                    p.SendMessage("Aborted. Use command again");
                    bd = null;
                }
                return;
            }
            
            if (step == 2) {
                bd.Name = value.Capitalize();
                step++;
            } else if (step == 3) {
                if (value == "0" || value == "1" || value == "2") {
                    bd.Solidity = byte.Parse(value);
                    step++;
                }
            } else if (step == 4) {
                if (byte.TryParse(value, out bd.MovementSpeed))
                    step++;
            } else if (step == 5) {
                if (byte.TryParse(value, out bd.TopT))
                    step++;
            } else if (step == 6) {
                if (byte.TryParse(value, out bd.SideT))
                    step++;
            } else if (step == 7) {
                if (byte.TryParse(value, out bd.BottomT))
                    step++;
            } else if (step == 8) {
                if (value == "0" || value == "1") {
                    bd.TransmitsLight = byte.Parse(value);
                    step++;
                }
            } else if (step == 9) {
                bool result = byte.TryParse(value, out bd.WalkSound);
                if (result && bd.WalkSound <= 11)
                    step++;
            } else if (step == 10) {
                if (value == "0" || value == "1") {
                    bd.FullBright = byte.Parse(value);
                    step++;
                }
            } else if (step == 11) {
                bool result = byte.TryParse(value, out bd.Shape);
                if (result && bd.Shape >= 1 && bd.Shape <= 4)
                    step++;
            } else if (step == 12) {
                bool result = byte.TryParse(value, out bd.BlockDraw);
                if (result && bd.Shape >= 0 && bd.Shape <= 3)
                    step = 13;
            } else if (step == 13) {
                if (byte.TryParse(value, out bd.FogD))
                    step = bd.FogD == 0 ? 17 : 14;
            } else if (step == 14) {
                if (byte.TryParse(value, out bd.FogR))
                    step++;
            } else if (step == 15) {
                if (byte.TryParse(value, out bd.FogG))
                    step++;
            } else if (step == 16) {
                if (byte.TryParse(value, out bd.FogB))
                    step++;
            } else if (step == 17) {
                if (Block.Byte(value) == Block.Zero) {
                    SendStepHelp(p, step); return;
                }
                
                bd.FallBack = Block.Byte(value);
                p.SendMessage("Creating block " + bd.Name);
                p.ClearChat();

                byte id = 128;
                for (int i = 128; i < 255; i++) {
                	if ( BlockDefinition.GlobalDefinitions[i] == null) {
                		id = (byte)i; break;
                	}
                }
                
                bd.ID = id;
                BlockDefinition.AddGlobal(bd);
                BlockDefinition.SaveGlobal("blocks.json");
                return;
            }
            SendStepHelp(p, step);
        }
        
        static string[][] stepsHelp = new string[][] {
            null, // step 0
            null, // step 1
            new[] { "Type the name of this block." },
            new[] { "Type \"0\" if this block is walk-through.", "Type \"1\" if this block is swim-through.",
                "Type \"2\" if this block is solid.",
            },
            new[] { "Type a number between \"0\" (0.25% speed) and \"255\" (396% speed).",
                "This speed is used inside or swimming in the block. Or when you are walking on it.",
            },
            new[] { "Type a number between \"0\" and \"255\" to identify which texture tile to use for the top of the block.",
                "Textures tile numbers are left to right in terrain.png (The file the textures are located).",
            },
            new[] { "Say a number between \"0\" and \"255\" to identify which texture tile to use for the sides of the block.",
                "Textures tile numbers are left to right in terrain.png (The file the textures are located).",
            },
            new[] { "Say a number between \"0\" and \"255\" to identify which texture tile to use for the bottom of the block.",
                "Textures tile numbers are left to right in terrain.png (The file the textures are located).",
            },
            new[] { "Type '0' if this block blocks light, otherwise '1' if it doesn't" },
            new[] { "Say a number between 0 and 11 to represent the sound heard when walking on it.",
                "0 = None; 1 = Powder; 2 = Wood; 3 = Gravel; 4 = Grass; 5 = Stone; 6 = Metal;",
                "7 = Glass; 8 = Cloth; 9 = Sand; 10 = Snow; 11 = Ladder",
            },
            new[] { "Type '0' if the block should be darkened when in shadow, 1 if not (Like lava)." },
            new[] { "What is the block's shape?", "1 = Cube; 2 = Slab; 3 = Snow; 4 = Sprite", },
            new[] { "Define the block's transparency.", "0 = Opaque; 1 = Transparent (Like glass)",
                "2 = Transparent (Like leaves, 3 = Translucent (Like ice)",
            },
            new[] { "Define the block's fog density (The density of it inside, i.e water, lava",
                "0 = No fog at all; 1-255 = Less to greater density",
            },
            new[] { "Define the fog's red value of its RGB (0-255)", },
            new[] { "Define the fog's green value of its RGB (0-255)", },
            new[] { "Define the fog's blue value of its RGB (0-255)", },
            new[] { "Define a fallback for this block (Clients that can't see this block).",
                "You can use the block name or block ID",
            },
        };
        
        static void SendStepHelp(Player p, int step) {
            string[] help = stepsHelp[step];
            for (int i = 0; i < help.Length; i++)
                Player.SendMessage(p, help[i]);
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "/addglobalblock - Adds a server-wide block");
        }
    }
}