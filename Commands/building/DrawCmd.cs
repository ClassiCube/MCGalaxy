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

namespace MCGalaxy.Commands {
    
    public abstract class DrawCmd : Command {
        
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Builder; } }

        public override void Use(Player p, string message) {
        	message = message.ToLower();
            string[] parts = message.Split(' ');
            CatchPos cpos = default(CatchPos);
            cpos.message = message;
            cpos.mode = GetMode(message, parts);
            OnUse(p, message, parts, ref cpos);
            p.blockchangeObject = cpos;
            
            if (PlaceMessage == null)
            	Player.SendMessage(p, "Place two blocks to determine the edges.");
            else
            	Player.SendMessage(p, PlaceMessage);
            
            p.ClearBlockchange();
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }
        
        // most draw commands use two coordinates, so implement this here to simplify implementation.
        protected virtual void Blockchange1(Player p, ushort x, ushort y, ushort z, byte type, byte extType) {
            RevertAndClearState(p, x, y, z);
            CatchPos bp = (CatchPos)p.blockchangeObject;
            bp.x = x; bp.y = y; bp.z = z;
            p.blockchangeObject = bp;
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange2);
        }
        
        protected virtual string PlaceMessage { get { return null; } }
        
        protected abstract void Blockchange2(Player p, ushort x, ushort y, ushort z, byte type, byte extType);
        
        protected abstract DrawMode ParseMode(string mode);
        
        protected virtual void OnUse(Player p, string msg, string[] parts, ref CatchPos cpos) { }
        
        protected virtual DrawMode GetMode(string message, string[] parts) {
        	return message == "" ? DrawMode.normal : ParseMode(parts[parts.Length - 1]);
        }
        
        internal static byte GetBlock(Player p, string msg, out byte extType, bool checkPlacePerm = true) {
            byte type = Block.Byte(msg);
            extType = 0;
            if (type == Block.Zero) {
            	// try treat as a block definition id.
            	type = BlockDefinition.GetBlock(msg, p);
            	if (type == Block.Zero) {
            		Player.SendMessage(p, "There is no block \"" + msg + "\".");
            		return Block.Zero;
            	}
            	extType = type;
            	return Block.custom_block;
            }
            
            if (checkPlacePerm && !Block.canPlace(p, type)) {
                Player.SendMessage(p, "Cannot place the block \"" + msg + "\".");
                return Block.Zero;
            }
            return type;
        }
        
        protected static Brush GetBrush(Player p, CatchPos cpos, 
                                        int usedFromEnd, Func<BrushArgs, Brush> constructor = null) {
        	int end = cpos.message.Length;
        	string brushMsg = "";
        	for (int i = 0; i < usedFromEnd; i++) {
        		end = cpos.message.LastIndexOf(' ', end - 1);
        		if (end == -1) break;  
        	}
        	
        	if (end >= 0) brushMsg = cpos.message.Substring(0, end);
        	if (constructor == null) constructor = Brush.Brushes[p.BrushName];
        	BrushArgs args = new BrushArgs(p, brushMsg, cpos.type, cpos.extType);
        	return constructor(args);
        }
        
        protected static void GetRealBlock(byte type, byte extType, Player p, ref CatchPos cpos) {
            cpos.type = type < 128 ? p.bindings[type] : type;
            cpos.extType = extType;
        }
        
        protected struct CatchPos {
            public DrawMode mode;
            public byte type, extType;
            public ushort x, y, z;
            public object data;
            public string message;
        }

        protected enum DrawMode {
            normal, solid, hollow, walls,
            holes, wire, random,
            vertical, reverse, straight, 
            up, down, layer, verticalX, verticalZ,
        }
    }
}
