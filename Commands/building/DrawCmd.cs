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
using System;

namespace MCGalaxy.Commands {
    
    public abstract class DrawCmd : Command {
        
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Builder; } }

        public override void Use(Player p, string message) {
            string[] parts = message.Split(' ');
            for (int i = 0; i < parts.Length; i++)
                parts[i] = parts[i].ToLower();
            CatchPos cpos = default(CatchPos);
            
            if (parts.Length > MaxArgs) {
                Help(p); return;
            } else if (parts.Length >= 2) {                
                byte type = GetBlock(p, parts[0], out cpos.extType);
                if (type == Block.Zero) return;
                SolidType solid = GetType(parts[1]);
                if (solid == SolidType.Invalid) {
                    Help(p); return;
                }             
                cpos.solid = solid;
                cpos.type = type;
            } else if (message == "") {
                cpos.solid = SolidType.solid;
                cpos.type = 0xFF;
            } else if (parts.Length == 1) {
                byte type = 0xFF;
                SolidType solid = GetType(parts[0]);
                if (solid == SolidType.Invalid) {
                    solid = SolidType.solid;
                    type = GetBlock(p, parts[0], out cpos.extType);
                    if (type == 255) return;
                }             
                cpos.solid = solid;
                cpos.type = type;           
            }
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

        protected virtual int MaxArgs { get { return 2; } }
        
        protected virtual string PlaceMessage { get { return null; } }
        
        protected abstract void Blockchange2(Player p, ushort x, ushort y, ushort z, byte type, byte extType);
        
        protected abstract SolidType GetType(string msg);
        
        protected virtual void OnUse(Player p, string msg, string[] parts, ref CatchPos cpos) { }
        
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
        
        protected static void GetRealBlock(byte type, byte extType, Player p, ref CatchPos cpos) {
        	if (cpos.type != Block.Zero) return;
            cpos.type = type < 128 ? p.bindings[type] : type;
            cpos.extType = extType;
        }
        
        protected struct CatchPos {
            public SolidType solid;
            public byte type, extType;
            public ushort x, y, z;
            public object data;
        }

        protected enum SolidType {
            solid, hollow, walls,
            holes, wire, random,
            vertical, reverse, straight, 
            up, down, layer, verticalX, verticalZ,
            Invalid = -1,
        }
    }
}
