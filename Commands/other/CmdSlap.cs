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
    public sealed class CmdSlap : Command
    {
        public override string name { get { return "slap"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public CmdSlap() { }

        public override void Use(Player p, string message)
        {
            if (message == "") { Help(p); return; }

            Player who = Player.Find(message);

            if (who == null)
            {
                Level which = Level.Find(message);

                if (which == null)
                {
                    Player.SendMessage(p, "Could not find player or map specified");
                    return;
                }
                else
                {
                    foreach (Player pl in Player.players)
                    {
                        if (pl.level == which && pl.group.Permission < p.group.Permission)
                        {
                               Command.all.Find("slap").Use(p, pl.name);
                        }
                    }
                    return;
                }
            }
            if (who.hidden)
            {
                Player.SendMessage(p, "Could not find player or map specified");
                return;
            }
            if (p != null)
            {
                if (who.group.Permission > p.group.Permission)
                {
                    Player.SendMessage(p, "You cannot slap someone ranked higher than you!");
                    return;
                }
            }

            ushort currentX = (ushort)(who.pos[0] / 32);
            ushort currentY = (ushort)(who.pos[1] / 32);
            ushort currentZ = (ushort)(who.pos[2] / 32);
            ushort foundHeight = 0;

            for (ushort yy = currentY; yy <= 1000; yy++)
            {
                if (p != null)
                {
                    if (!Block.Walkthrough(p.level.GetTile(currentX, yy, currentZ)) && p.level.GetTile(currentX, yy, currentZ) != Block.Zero)
                    {
                        foundHeight = (ushort)(yy - 1);
                        who.level.ChatLevel(who.color + who.DisplayName + Server.DefaultColor + " was slapped into the roof by " + p.color + p.DisplayName);
                        break;
                    }
                }
                else
                {
                    if (!Block.Walkthrough(who.level.GetTile(currentX, yy, currentZ)) && who.level.GetTile(currentX, yy, currentZ) != Block.Zero)
                    {
                        foundHeight = (ushort)(yy - 1);
                        who.level.ChatLevel(who.color + who.DisplayName + Server.DefaultColor + " was slapped into the roof by " + "the Console.");
                        break;
                    }
                }
            }
            if (foundHeight == 0)
            {
                if (p != null)
                {
                    who.level.ChatLevel(who.color + who.DisplayName + Server.DefaultColor + " was slapped sky high by " + p.color + p.DisplayName);
                }
                else
                {
                    who.level.ChatLevel(who.color + who.DisplayName + Server.DefaultColor + " was slapped sky high by " + "the Console.");
                }
                foundHeight = 1000;
            }
            
            unchecked { who.SendPos((byte)-1, who.pos[0], (ushort)(foundHeight * 32), who.pos[2], who.rot[0], who.rot[1]); }
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/slap <name> - Slaps <name>, knocking them into the air");
            Player.SendMessage(p, "/slap <level> - Slaps all players on <level> that are a lower rank than you, knocking them into the air");
        }
    }
}
