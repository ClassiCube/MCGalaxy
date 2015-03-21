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
    public sealed class CmdBlockSet : Command
    {
        public override string name { get { return "blockset"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return "mod"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdBlockSet() { }

        public override void Use(Player p, string message)
        {
            if (message == "" || message.IndexOf(' ') == -1) { Help(p); return; }

            byte foundBlock = Block.Byte(message.Split(' ')[0]);
            if (foundBlock == Block.Zero) { Player.SendMessage(p, "Could not find block entered"); return; }
            LevelPermission newPerm = Level.PermissionFromName(message.Split(' ')[1]);
            if (newPerm == LevelPermission.Null) { Player.SendMessage(p, "Could not find rank specified"); return; }
            if (p != null && newPerm > p.group.Permission) { Player.SendMessage(p, "Cannot set to a rank higher than yourself."); return; }

            if (p != null && !Block.canPlace(p, foundBlock)) { Player.SendMessage(p, "Cannot modify a block set for a higher rank"); return; }

            Block.Blocks newBlock = Block.BlockList.Find(bs => bs.type == foundBlock);
            newBlock.lowestRank = newPerm;

            Block.BlockList[Block.BlockList.FindIndex(bL => bL.type == foundBlock)] = newBlock;

            Block.SaveBlocks(Block.BlockList);

            Player.GlobalMessage("&d" + Block.Name(foundBlock) + Server.DefaultColor + "'s permission was changed to " + Level.PermissionToName(newPerm));
            if (p == null)
            {
                Player.SendMessage(p, Block.Name(foundBlock) + "'s permission was changed to " + Level.PermissionToName(newPerm));
                return;
            }
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/blockset [block] [rank] - Changes [block] rank to [rank]");
            Player.SendMessage(p, "Only blocks you can use can be modified");
            Player.SendMessage(p, "Available ranks: " + Group.concatList());
        }
    }
}