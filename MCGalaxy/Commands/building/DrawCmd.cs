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
using MCGalaxy.Drawing.Brushes;
using MCGalaxy.Drawing.Ops;
using System;
using MCGalaxy.Maths;
using BlockID = System.UInt16;

namespace MCGalaxy.Commands.Building {
    public abstract class DrawCmd : Command2 {
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Builder; } }
        public override bool SuperUseable { get { return false; } }
        
        protected virtual int MarksCount { get { return 2; } }
        protected virtual string SelectionType { get { return "region"; } }
        protected virtual string PlaceMessage { get { return "Place or break two blocks to determine the edges."; } }
        protected const string BrushHelpLine = "   &HFor help about brushes, type &T/Help Brush";
        
        public override void Use(Player p, string message, CommandData data) {
            DrawArgs dArgs = MakeArgs(p, message);
            if (dArgs.Op == null) return;
            
            // Validate the brush syntax is correct
            BrushFactory factory = MakeBrush(dArgs);
            BrushArgs bArgs = new BrushArgs(p, dArgs.BrushArgs, dArgs.Block);
            if (!factory.Validate(bArgs)) return;
            
            p.Message(PlaceMessage);
            p.MakeSelection(MarksCount, "Selecting " + SelectionType + " for &S" + dArgs.Op.Name, dArgs, DoDraw);
        }
        
        protected virtual DrawArgs MakeArgs(Player p, string message) {
            DrawArgs dArgs = new DrawArgs();
            message = message.ToLower();
            string[] parts = message.SplitSpaces();
            
            dArgs.Message = message;
            dArgs.Player  = p;
            dArgs.Mode = GetMode(parts);
            dArgs.Op   = GetDrawOp(dArgs);
            return dArgs;
        }
        
        protected virtual bool DoDraw(Player p, Vec3S32[] marks, object state, BlockID block) {
            DrawArgs dArgs = (DrawArgs)state;
            dArgs.Block = block;
            GetMarks(dArgs, ref marks);
            if (marks == null) return false;
            
            BrushFactory factory = MakeBrush(dArgs);
            BrushArgs bArgs = new BrushArgs(p, dArgs.BrushArgs, dArgs.Block);
            Brush brush = factory.Construct(bArgs);
            if (brush == null) return false;
            
            DrawOpPerformer.Do(dArgs.Op, brush, p, marks);
            return true;
        }
        
        protected BrushFactory MakeBrush(DrawArgs args) {
            args.BrushName = args.Player.BrushName;
            args.BrushArgs = "";
            GetBrush(args);
            
            if (args.BrushArgs.Length == 0) args.BrushArgs = args.Player.DefaultBrushArgs;
            return BrushFactory.Find(args.BrushName);
        }

        
        protected virtual DrawMode GetMode(string[] parts) { return DrawMode.normal; }
        
        protected abstract DrawOp GetDrawOp(DrawArgs dArgs);
        
        protected virtual void GetMarks(DrawArgs dArgs, ref Vec3S32[] m) { }
        
        protected virtual void GetBrush(DrawArgs dArgs) {
            dArgs.BrushArgs = dArgs.Message.Splice(dArgs.ModeArgsCount, 0);
        }
        
        protected class DrawArgs {
            public DrawMode Mode;
            public BlockID Block;
            public string Message, BrushName, BrushArgs;
            public int ModeArgsCount { get { return Mode == DrawMode.normal ? 0 : 1; } }
            
            public DrawOp Op;
            public Player Player;
            public object Meta;
        }
    }
    
    public enum DrawMode {
        normal, solid, hollow, walls,
        holes, wire, random,
        vertical, reverse, straight,           // line
        up, down, layer, verticalX, verticalZ, // fill
        cone, hcone, icone, hicone, volcano,   // draw
        pyramid, hpyramid, ipyramid, hipyramid,// draw
        sphere, hsphere, circle, hcircle       // draw
    }
}
