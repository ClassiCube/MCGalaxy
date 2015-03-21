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
namespace MCGalaxy.Commands
{
    public sealed class CmdMode : Command
    {
        public override string name { get { return "mode"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return "build"; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public CmdMode() { }

        public override void Use(Player p, string message)
        {
            if (message == "")
            {
                if (p.modeType != 0)
                {
                    Player.SendMessage(p, "&b" + Block.Name(p.modeType)[0].ToString().ToUpper() + Block.Name(p.modeType).Remove(0, 1).ToLower() + Server.DefaultColor + " mode: &cOFF");
                    p.modeType = 0;
                    p.BlockAction = 0;
                }
                else
                {
                    Help(p); return;
                }
            }
            else
            {
                byte b = Block.Byte(message);
                if (b == Block.Zero) { Player.SendMessage(p, "Could not find block given."); return; }
                if (b == Block.air) { Player.SendMessage(p, "Cannot use Air Mode."); return; }
                if (p.allowTnt == false)
                {
                    if (b == Block.tnt)
                    {
                        Player.SendMessage(p, "Tnt usage is not allowed at the moment");
                        return;
                    }
                }

                if (p.allowTnt == false)
                {
                    if (b == Block.bigtnt)
                    {
                        Player.SendMessage(p, "Tnt usage is not allowed at the moment");
                        return;
                    }
                }

                if (p.allowTnt == false)
                {
                    if (b == Block.nuketnt)
                    {
                        Player.SendMessage(p, "Tnt usage is not allowed at the moment");
                        return;
                    }
                }

                if (p.allowTnt == false)
                {
                    if (b == Block.fire)
                    {
                        Player.SendMessage(p, "Tnt usage is not allowed at the moment, fire is a lighter for tnt and is also disabled");
                        return;
                    }
                }

                if (p.allowTnt == false)
                {
                    if (b == Block.tntexplosion)
                    {
                        Player.SendMessage(p, "Tnt usage is not allowed at the moment");
                        return;
                    }
                }

                if (p.allowTnt == false)
                {
                    if (b == Block.smalltnt)
                    {
                        Player.SendMessage(p, "Tnt usage is not allowed at the moment");
                        return;
                    }
                }
                        
                if (!Block.canPlace(p, b)) { Player.SendMessage(p, "Cannot place this block at your rank."); return; }

                if (p.modeType == b)
                {
                    Player.SendMessage(p, "&b" + Block.Name(p.modeType)[0].ToString().ToUpper() + Block.Name(p.modeType).Remove(0, 1).ToLower() + Server.DefaultColor + " mode: &cOFF");
                    p.modeType = 0;
                    p.BlockAction = 0;
                }
                else
                {
                    p.BlockAction = 6;
                    p.modeType = b;
                    Player.SendMessage(p, "&b" + Block.Name(p.modeType)[0].ToString().ToUpper() + Block.Name(p.modeType).Remove(0, 1).ToLower() + Server.DefaultColor + " mode: &aON");
                }
            }
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/mode [block] - Makes every block placed into [block].");
            Player.SendMessage(p, "/[block] also works");
        }
    }
}