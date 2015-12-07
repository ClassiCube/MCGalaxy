using System;

namespace MCGalaxy {

	public abstract class Brush {
		
		public abstract byte NextBlock();
	}
	
	public sealed class SolidBrush : Brush {
		
	}
}
