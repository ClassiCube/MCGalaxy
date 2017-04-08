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
            
            DrawArgs dArgs = default(DrawArgs);
            dArgs.Message = message;
            dArgs.Player = p;
            dArgs.Mode = GetMode(parts);
            dArgs.Op = GetDrawOp(dArgs);
            if (dArgs.Op == null) return;
            
            // Validate the brush syntax is correct
            int offset = 0;
            BrushFactory factory = BrushFactory.Find(GetBrush(p, dArgs, ref offset));
            BrushArgs bArgs = GetBrushArgs(dArgs, offset);
            if (!factory.Validate(bArgs)) return;
            
            Player.Message(p, PlaceMessage);
            p.MakeSelection(MarksCount, dArgs, DoDraw);
        }
        
        protected virtual bool DoDraw(Player p, Vec3S32[] marks,
                                      object state, byte block, byte extBlock) {
            DrawArgs dArgs = (DrawArgs)state;
            dArgs.Block = block; dArgs.ExtBlock = extBlock;
            GetMarks(dArgs, ref marks);
            if (marks == null) return false;
            
            int offset = 0;
            BrushFactory factory = BrushFactory.Find(GetBrush(p, dArgs, ref offset));
            BrushArgs bArgs = GetBrushArgs(dArgs, offset);
            Brush brush = factory.Construct(bArgs);
            return brush != null && DrawOpPerformer.Do(dArgs.Op, brush, p, marks);
        }
        
        protected virtual string PlaceMessage {
            get { return "Place or break two blocks to determine the edges."; }
        }
        
        
        protected virtual DrawMode GetMode(string[] parts) { return DrawMode.normal; }
        
        protected abstract DrawOp GetDrawOp(DrawArgs dArgs);
        
        protected virtual void GetMarks(DrawArgs dArgs, ref Vec3S32[] m) { }
        
        protected virtual string GetBrush(Player p, DrawArgs dArgs, ref int offset) {
            offset = dArgs.Mode == DrawMode.normal ? 0 : 1;
            return p.BrushName;
        }
        
        protected static BrushArgs GetBrushArgs(DrawArgs dArgs, int usedFromEnd) {
            int end = dArgs.Message.Length;
            string brushMsg = "";
            for (int i = 0; i < usedFromEnd; i++) {
                end = dArgs.Message.LastIndexOf(' ', end - 1);
                if (end == -1) break;
            }
            
            if (end >= 0) brushMsg = dArgs.Message.Substring(0, end);
            if (brushMsg == "") brushMsg = dArgs.Player.DefaultBrushArgs;
            return new BrushArgs(dArgs.Player, brushMsg, dArgs.Block, dArgs.ExtBlock);
        }
        
        protected struct DrawArgs {
            public DrawMode Mode;
            public byte Block, ExtBlock;
            public string Message;
            
            public DrawOp Op;
            public Player Player;
        }
    }
    
    public enum DrawMode {
        normal, solid, hollow, walls,
        holes, wire, random,
        vertical, reverse, straight,           // line
        up, down, layer, verticalX, verticalZ, // fill
        cone, hcone, icone, hicone, volcano,   // draw
        pyramid, hpyramid, ipyramid, hipyramid,// draw
        sphere, hsphere, circle,               // draw
    }
}
