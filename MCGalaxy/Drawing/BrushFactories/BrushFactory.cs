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
using BlockID = System.UInt16;

namespace MCGalaxy.Drawing.Brushes {
    public abstract class BrushFactory {
        public abstract string Name { get; }
        public abstract string[] Help { get; }
        
        /// <summary> Creates a brush from the given arguments, 
        /// returning null if invalid arguments are specified. </summary>
        public abstract Brush Construct(BrushArgs args);
        
        /// <summary> Validates the given arguments, returning false if they are invalid. </summary>
        public virtual bool Validate(BrushArgs args) { return Construct(args) != null; }
        
        public static List<BrushFactory> Brushes = new List<BrushFactory>() {
            new SolidBrushFactory(), new CheckeredBrushFactory(),
            new StripedBrushFactory(), new PasteBrushFactory(),
            new ReplaceBrushFactory(), new ReplaceNotBrushFactory(),
            new RainbowBrushFactory(), new BWRainbowBrushFactory(),
            new RandomBrushFactory(), new CloudyBrushFactory(),
        };
        
        public static BrushFactory Find(string name) {
            foreach (BrushFactory entry in Brushes) {
                if (entry.Name.CaselessEq(name)) return entry;
            }
            return null;
        }
    }
    
    public struct BrushArgs {
        /// <summary> Player that is providing arguments. </summary>
        public Player Player;
        
        /// <summary> Raw message provided for arguments, including spaces. </summary>
        public string Message;
        
        /// <summary> Block the player is currently holding. </summary>
        public BlockID Block;
        
        public BrushArgs(Player p, string message, BlockID block) {
            Player = p; Message = message; Block = block;
        }
    }
}
