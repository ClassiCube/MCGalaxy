/*
    Copyright 2015 MCGalaxy team
    
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
using MCGalaxy.Drawing.Brushes;
using System;

namespace MCGalaxy.Commands.Building {    
    public abstract class DrawCmd : Command {       
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Builder; } }
        public virtual int MarksCount { get { return 2; } }

        public override void Use(Player p, string message) {
            if (Player.IsSuper(p)) { MessageInGameOnly(p); return; }
        	message = message.ToLower();
            string[] parts = message.Split(' ');
            DrawArgs cpos = default(DrawArgs);
            cpos.message = message;
            cpos.mode = GetMode(message, parts);
            OnUse(p, message, parts, ref cpos);
            
            Player.Message(p, PlaceMessage);
            p.MakeSelection(MarksCount, cpos, DoDraw);
        }
        
        protected abstract bool DoDraw(Player p, Vec3S32[] marks, object state, byte type, byte extType);
        
        protected virtual string PlaceMessage { get { return "Place two blocks to determine the edges."; } }
        
        protected abstract DrawMode ParseMode(string mode);
        
        protected virtual void OnUse(Player p, string msg, string[] parts, ref DrawArgs cpos) { }
        
        protected virtual DrawMode GetMode(string message, string[] parts) {
        	return message == "" ? DrawMode.normal : ParseMode(parts[parts.Length - 1]);
        }
        
        internal static int GetBlock(Player p, string msg, out byte extType, bool checkPlacePerm = true) {
            byte type = Block.Byte(msg);
            extType = 0;
            if (msg.CaselessEq("skip") || msg.CaselessEq("none")) return Block.Zero;
            
            if (type == Block.Zero) {
            	// try treat as a block definition id.
            	type = BlockDefinition.GetBlock(msg, p);
            	if (type == Block.Zero) {
            		Player.Message(p, "There is no block \"{0}\".", msg);
            		return -1;
            	}
            	extType = type;
            	return Block.custom_block;
            }
            
            if (checkPlacePerm && !Block.canPlace(p, type)) {
                Player.Message(p, "Cannot place the block \"{0}\".", msg);
                return -1;
            }
            return type;
        }
        
        protected static Brush GetBrush(Player p, DrawArgs dArgs, 
                                        int usedFromEnd, Func<BrushArgs, Brush> constructor = null) {
        	int end = dArgs.message.Length;
        	string brushMsg = "";
        	for (int i = 0; i < usedFromEnd; i++) {
        		end = dArgs.message.LastIndexOf(' ', end - 1);
        		if (end == -1) break;  
        	}
        	
        	if (end >= 0) brushMsg = dArgs.message.Substring(0, end);
        	if (brushMsg == "") brushMsg = p.DefaultBrushArgs;
        	if (constructor == null) constructor = Brush.Brushes[p.BrushName];
        	BrushArgs args = new BrushArgs(p, brushMsg, dArgs.block, dArgs.extBlock);
        	return constructor(args);
        }
        
        protected struct DrawArgs {
            public DrawMode mode;
            public byte block, extBlock;
            public object data;
            public string message;
        }

        protected enum DrawMode {
            normal, solid, hollow, walls,
            holes, wire, random,
            vertical, reverse, straight,           // line           
            up, down, layer, verticalX, verticalZ, // fill
            cone, hcone, icone, hicone, volcano,   // draw
            pyramid, hpyramid, ipyramid, hipyramid,// draw
            sphere, hsphere, circle,               // draw    
        }
    }
}
