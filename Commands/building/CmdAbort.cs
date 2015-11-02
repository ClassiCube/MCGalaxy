/*
	Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
	
	Dual-licensed under the	Educational Community License, Version 2.0 and
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
namespace MCGalaxy.Commands
{
    public sealed class CmdAbort : Command
    {
        public override string name { get { return "abort"; } }
        public override string shortcut { get { return "a"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Builder; } }
        public CmdAbort() { }

        public override void Use(Player p, string message)
        {
            if (p != null)
            {
                p.ClearBlockchange();
                p.painting = false;
                p.BlockAction = 0;
                p.megaBoid = false;
                p.cmdTimer = false;
                p.staticCommands = false;
                p.deleteMode = false;
                p.ZoneCheck = false;
                p.modeType = 0;
                p.aiming = false;
                p.onTrain = false;
                p.isFlying = false;
                try
                {
                    p.level.blockqueue.RemoveAll((BlockQueue.block b) => { if (b.p == p) return true; return false; });
                }
                finally { BlockQueue.resume(); }
                Player.SendMessage(p, "Every toggle or action was aborted.");
                return;
            }
            Player.SendMessage(p, "This command can only be used in-game!");
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/abort - Cancels an action.");
        }
    }
}
