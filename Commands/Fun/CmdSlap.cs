/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
    Dual-licensed under the    Educational Community License, Version 2.0 and
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
            Player who = PlayerInfo.Find(message);

            if (who == null)
            {
                Level which = LevelInfo.Find(message);

                if (which == null)
                {
                    Player.SendMessage(p, "Could not find player or map specified");
                    return;
                }
                else
                {
                	Player[] players = PlayerInfo.Online;
                    foreach (Player pl in players) {
                        if (pl.level == which && pl.group.Permission < p.group.Permission)
                            Command.all.Find("slap").Use(p, pl.name);
                    }
                    return;
                }
            }
            if (!Player.CanSee(p, who)) {
                Player.SendMessage(p, "Could not find player or map specified"); return;
            }
            if (p != null && who.group.Permission > p.group.Permission) {
                Player.SendMessage(p, "You cannot slap someone ranked higher than you!"); return;
            }

            ushort curX = (ushort)(who.pos[0] / 32), curY = (ushort)(who.pos[1] / 32), curZ = (ushort)(who.pos[2] / 32);
            ushort foundHeight = ushort.MaxValue;

            string src = p == null ? "the Console" : p.color + p.DisplayName;
            for (ushort yy = curY; yy <= 1000; yy++) {
                if (!Block.Walkthrough(p.level.GetTile(curX, yy, curZ)) && p.level.GetTile(curX, yy, curZ) != Block.Zero) {
                    foundHeight = (ushort)(yy - 1);
                    who.level.ChatLevel(who.color + who.DisplayName + " %Swas slapped into the roof by " + src);
                    break;
                }
            }
            
            if (foundHeight == ushort.MaxValue) {
                who.level.ChatLevel(who.color + who.DisplayName + " %Swas slapped sky high by " + src);
                foundHeight = 1000;
            }         
            who.SendPos(0xFF, who.pos[0], (ushort)(foundHeight * 32), who.pos[2], who.rot[0], who.rot[1]);
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "/slap <name> - Slaps <name>, knocking them into the air");
            Player.SendMessage(p, "/slap <level> - Slaps all players on <level> that are a lower rank than you, knocking them into the air");
        }
    }
}
