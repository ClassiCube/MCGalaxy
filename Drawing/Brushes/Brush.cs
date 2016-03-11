/*
    Copyright 2015 MCGalaxy
        
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
using MCGalaxy.Commands;
using MCGalaxy.Drawing.Ops;

namespace MCGalaxy.Drawing.Brushes {
    
    public abstract class Brush {
        
        /// <summary> Human friendly name of this brush. </summary>
        public abstract string Name { get; }
        
        /// <summary> Description of the brush, in addition to its syntax. </summary>
        public abstract string[] Help { get; }
        
        public abstract byte NextBlock(DrawOp op);
        
        public abstract byte NextExtBlock(DrawOp op);
        
        public static Dictionary<string, Func<BrushArgs, Brush>> Brushes 
            = new Dictionary<string, Func<BrushArgs, Brush>> {
            { "normal", SolidBrush.Process }, { "paste", PasteBrush.Process },
            { "checkered", CheckeredBrush.Process }, { "rainbow", RainbowBrush.Process },
            { "bwrainbow", BWRainbowBrush.Process }, { "striped", StripedBrush.Process },
            { "replace", ReplaceBrush.Process }, { "replacenot", ReplaceNotBrush.Process },
        };
        
        public static Dictionary<string, string[]> BrushesHelp = new Dictionary<string, string[]> {
            { "normal", SolidBrush.HelpString }, { "paste", PasteBrush.HelpString },
            { "checkered", CheckeredBrush.HelpString }, { "rainbow", RainbowBrush.HelpString },
            { "bwrainbow", BWRainbowBrush.HelpString }, { "striped", StripedBrush.HelpString },
            { "replace", ReplaceBrush.HelpString }, { "replacenot", ReplaceNotBrush.HelpString },
        };
    }
    
    public struct BrushArgs {
        public Player Player;
        public string Message;
        public byte Type, ExtType; // currently holding
        
        public BrushArgs(Player p, string message, byte type, byte extType) {
            Player = p; Message = message; Type = type; ExtType = extType;
        }
    }
    
    // TODO: Make this into a proper full-featured brush
    // Consider varying random based off 3D positon, like libnoise.
    public sealed class RandomBrush : Brush {
        readonly Random rnd = new Random();
        readonly byte type, extType;
        
        public RandomBrush(byte type, byte extType) {
            this.type = type;
            this.extType = extType;
        }
        
        public override string Name { get { return "Random"; } }
        
        public override string[] Help { get { return new string[0]; } }
        
        public static Brush Process(BrushArgs args) {
            if (args.Message == "")
                return new RandomBrush(args.Type, args.ExtType);
            byte extType;
            byte type = DrawCmd.GetBlock(args.Player, args.Message, out extType);
            if (type == Block.Zero) return null;
            return new RandomBrush(type, extType);
        }
        
        public override byte NextBlock(DrawOp op) {
            return (byte)rnd.Next(1, 11) <= 5 ? type : Block.Zero;
        }
        
        public override byte NextExtBlock(DrawOp op) { return extType; }
    }
}
