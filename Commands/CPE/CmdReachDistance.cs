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
 
namespace MCGalaxy.Commands.CPE {	
	public sealed class CmdReachDistance : Command {
		
		public override string name { get { return "reachdistance"; } }
		public override string shortcut { get { return "reach"; } }
		public override string type { get { return CommandTypes.Building; } }
		public override bool museumUsable { get { return false; } }
		public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }

		public override void Use(Player p, string message) {
			if (Player.IsSuper(p)) { MessageInGameOnly(p); return; }
			if (message == "") { Help(p); return; }
			
			float dist;
			if( !float.TryParse(message, out dist)) {
				Player.Message(p, "\"" + message + "\", is not a valid distance."); return;
			}
			int packedDist = (int)(dist * 32);
			if (packedDist < 0) packedDist = 160;
			
			if (packedDist > short.MaxValue) {
				Player.Message(p, "\"" + message + "\", is too long a reach distance. Max is 1023 blocks.");
			} else if (!p.HasCpeExt(CpeExt.ClickDistance)) {
				Player.Message(p, "Your client doesn't support changing your reach distance.");
			} else {		
				p.SendClickDistance((short)packedDist);
				p.ReachDistance = packedDist / 32f;
			}
		}
		
		public override void Help(Player p) {
			Player.Message(p, "%T/reach [distance]");
			Player.Message(p, "%HSets the reach distance for how far away you can modify blocks.");
			Player.Message(p, "%H  The default reach distance is 5.");
		}
	}
}
