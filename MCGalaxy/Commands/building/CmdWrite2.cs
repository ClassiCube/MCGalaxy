using System;
using System.Collections.Generic;
using MCGalaxy.Commands.Building;
using MCGalaxy.Drawing.Ops;
using MCGalaxy.Drawing.Brushes;
using MCGalaxy.Maths;
using BlockID = System.UInt16;

namespace MCGalaxy.Commands {
	
	public class Write2DrawOp : DrawOp {
		public override string Name { get { return "Write2"; } }
		public string Text;
		public override long BlocksAffected(Level lvl, Vec3S32[] marks) { return Text.Length; }
		
		Vec3S32 dir, pos;
		public override void Perform(Vec3S32[] marks, Brush brush, DrawOpOutput output) {
			Vec3S32 p1 = marks[0], p2 = marks[1];
			if (Math.Abs(p2.X - p1.X) > Math.Abs(p2.Z - p1.Z)) {
				dir.X = p2.X > p1.X ? 1 : -1;
			} else {
				dir.Z = p2.Z > p1.Z ? 1 : -1;
			}
			
			pos = p1;
			foreach (char c in Text) { DrawLetter(Player, c, output); }
		}
		
		void DrawLetter(Player p, char c, DrawOpOutput output) {
			if ((int)c >= 256 || letters[c] == 0) {
				if (c != ' ') Player.Message(p, "\"{0}\" is not currently supported, replacing with space", c);
			} else {
				BlockID block = Block.FromRaw(letters[c]);
				output(Place((ushort)pos.X, (ushort)pos.Y, (ushort)pos.Z, block));
			}
			pos += dir;
		}
		
		static BlockID[] letters;
		static Write2DrawOp() {
			letters = new BlockID[256];
			letters['.'] = 520;
			letters['!'] = 521;
			letters['/'] = 522;
			letters['?'] = 523;
			
			for (int i = '0'; i <= '9'; i++) {
				letters[i] = (BlockID)(484 + (i - '0'));
			}
			for (int i = 'A'; i <= 'Z'; i++) {
				letters[i] = (BlockID)(494 + (i - 'A'));
			}
		}
	}
	
	public class CmdWrite2 : DrawCmd {
		public override string name { get { return "Write2"; } }
		public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
		
		protected override string SelectionType { get { return "direction"; } }
		protected override string PlaceMessage { get { return "Place or break two blocks to determine direction."; } }
		
		protected override DrawOp GetDrawOp(DrawArgs dArgs) {
			Player p = dArgs.Player;
			if (dArgs.Message.Length == 0) { Help(p); return null; }
			
			Write2DrawOp op = new Write2DrawOp();
			op.Text = dArgs.Message.ToUpper();
			return op;
		}
		
		protected override void GetMarks(DrawArgs dArgs, ref Vec3S32[] m) {
			if (m[0].X != m[1].X || m[0].Z != m[1].Z) return;
			Player.Message(dArgs.Player, "No direction was selected");
			m = null;
		}

		public override void Help(Player p) {
			Player.Message(p, "%T/Write2 [message]");
			Player.Message(p, "%HWrites [message] in letter blocks");
		}
	}
}
