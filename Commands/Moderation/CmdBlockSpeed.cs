/*
	Copyright 2011 MCGalaxy
		
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
using System;
namespace MCGalaxy.Commands
{
    public sealed class CmdBlockSpeed : Command
    {
        public override string name { get { return "blockspeed"; } }
        public override string shortcut { get { return "bs"; } }
       public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdBlockSpeed() { }

        public override void Use(Player p, string text)
        {
            if (text == "")
            {
                SendEstimation(p);
                return;
            }
            if (text == "clear")
            {
                Server.levels.ForEach((l) => { l.blockqueue.Clear(); });
                return;
            }
            if (text.StartsWith("bs"))
            {
                try { BlockQueue.blockupdates = int.Parse(text.Split(' ')[1]); }
                catch { Player.SendMessage(p, "Invalid number specified."); return; }
                Player.SendMessage(p, String.Format("Blocks per interval is now {0}.", BlockQueue.blockupdates));
                return;
            }
            if (text.StartsWith("ts"))
            {
                try { BlockQueue.time = int.Parse(text.Split(' ')[1]); }
                catch { Player.SendMessage(p, "Invalid number specified."); return; }
                Player.SendMessage(p, String.Format("Block interval is now {0}.", BlockQueue.time));
                return;
            }
            if (text.StartsWith("buf"))
            {
                if (p.level.bufferblocks)
                {
                    p.level.bufferblocks = false;
                    Player.SendMessage(p, String.Format("Block buffering on {0} disabled.", p.level.name));
                }
                else
                {
                    p.level.bufferblocks = true;
                    Player.SendMessage(p, String.Format("Block buffering on {0} enabled.", p.level.name));
                }
                return;
            }
            if (text.StartsWith("net"))
            {
                switch (int.Parse(text.Split(' ')[1]))
                {
                    case 2:
                        BlockQueue.blockupdates = 25;
                        BlockQueue.time = 100;
                        break;
                    case 4:
                        BlockQueue.blockupdates = 50;
                        BlockQueue.time = 100;
                        break;
                    case 8:
                        BlockQueue.blockupdates = 100;
                        BlockQueue.time = 100;
                        break;
                    case 12:
                        BlockQueue.blockupdates = 200;
                        BlockQueue.time = 100;
                        break;
                    case 16:
                        BlockQueue.blockupdates = 200;
                        BlockQueue.time = 100;
                        break;
                    case 161:
                        BlockQueue.blockupdates = 100;
                        BlockQueue.time = 50;
                        break;
                    case 20:
                        BlockQueue.blockupdates = 125;
                        BlockQueue.time = 50;
                        break;
                    case 24:
                        BlockQueue.blockupdates = 150;
                        BlockQueue.time = 50;
                        break;
                    default:
                        BlockQueue.blockupdates = 200;
                        BlockQueue.time = 100;
                        break;
                }
                SendEstimation(p);
                return;
            }
        }
        private static void SendEstimation(Player p)
        {
            Player.SendMessage(p, String.Format("{0} blocks every {1} milliseconds = {2} blocks per second.", BlockQueue.blockupdates, BlockQueue.time, BlockQueue.blockupdates * (1000 / BlockQueue.time)));
            Player.SendMessage(p, String.Format("Using ~{0}KB/s times {1} player(s) = ~{2}KB/s", (BlockQueue.blockupdates * (1000 / BlockQueue.time) * 8) / 1000, PlayerInfo.players.Count, PlayerInfo.players.Count * ((BlockQueue.blockupdates * (1000 / BlockQueue.time) * 8) / 1000)));
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/bs [option] [option value] - Options for block speeds.");
            Player.SendMessage(p, "Options are: bs (blocks per interval), ts (interval in milliseconds), buf (toggles buffering), clear, net.");
            Player.SendMessage(p, "/bs net [2,4,8,12,16,20,24] - Presets, divide by 8 and times by 1000 to get blocks per second.");
        }
    }
}
