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
using MCGalaxy.Drawing.Ops;
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
        
        protected virtual bool DoDraw(Player p, Vec3S32[] marks, 
                                      object state, byte block, byte extBlock) {
            DrawArgs dArgs = (DrawArgs)state;
            dArgs.block = block; dArgs.extBlock = extBlock;        
            marks = GetMarks(dArgs, marks);
            DrawOp op = GetDrawOp(dArgs);
            
            int offset = 0;
            BrushFactory factory = GetBrush(p, dArgs, ref offset);
            Brush brush = ParseBrush(p, dArgs, offset, factory);
            return brush != null && DrawOp.DoDrawOp(op, brush, p, marks);
        }
        
        protected virtual string PlaceMessage { 
            get { return "Place two blocks to determine the edges."; } 
        }
        
        protected virtual void OnUse(Player p, string msg, string[] parts, ref DrawArgs cpos) { }
        
        
        protected virtual DrawMode GetMode(string[] parts) { return DrawMode.Normal; }
        
        protected virtual void GetMarks(DrawArgs dArgs, Vec3S32[] m) { return m; }
        
        protected virtual BrushFactory GetBrush(Player p, DrawArgs dArgs, ref int brushOffset) {
            brushOffset = dArgs.mode == DrawMode.normal ? 0 : 1;
            return BrushFactory.Find(p.BrushName);
        }
        
        protected abstract DrawOp GetDrawOp(DrawArgs dArgs, Vec3S32[] m);
        
        
        internal static int GetBlock(Player p, string msg, out byte extBlock, bool checkPlacePerm = true) {
            byte block = Block.Byte(msg);
            extBlock = 0;
            if (msg.CaselessEq("skip") || msg.CaselessEq("none")) return Block.Zero;
            
            if (block == Block.Zero) {
                // try treat as a block definition id.
                block = BlockDefinition.GetBlock(msg, p);
                if (block == Block.Zero) {
                    Player.Message(p, "There is no block \"{0}\".", msg);
                    return -1;
                }
                extBlock = block;
                return Block.custom_block;
            }
            
            if (checkPlacePerm && !Block.canPlace(p, block)) {
                Formatter.MessageBlock(p, "draw with ", block); return -1;
            }
            return block;
        }
        
        protected static Brush ParseBrush(Player p, DrawArgs dArgs, 
                                        int usedFromEnd, BrushFactory factory = null) {
            int end = dArgs.message.Length;
            string brushMsg = "";
            for (int i = 0; i < usedFromEnd; i++) {
                end = dArgs.message.LastIndexOf(' ', end - 1);
                if (end == -1) break;  
            }
            
            if (end >= 0) brushMsg = dArgs.message.Substring(0, end);
            if (brushMsg == "") brushMsg = p.DefaultBrushArgs;
            if (factory == null) factory = BrushFactory.Find(p.BrushName);
            BrushArgs args = new BrushArgs(p, brushMsg, dArgs.block, dArgs.extBlock);
            return factory.Construct(args);
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
