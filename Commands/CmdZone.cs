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
using System.Collections.Generic;
using MCGalaxy.SQL;
namespace MCGalaxy.Commands
{
    public sealed class CmdZone : Command
    {
        public override string name { get { return "zone"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return "mod"; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdZone() { }

        public override void Use(Player p, string message)
        {
            CatchPos cpos;

            if (message == "")
            {
                p.ZoneCheck = true;
                Player.SendMessage(p, "Place a block where you would like to check for zones.");
                return;
            }
            else if ((int)p.group.Permission < CommandOtherPerms.GetPerm(this, 1))
            {
                Player.SendMessage(p, "Reserved for " + Group.findPermInt(CommandOtherPerms.GetPerm(this, 1)).name + "+");
                return;
            }

            if (message.IndexOf(' ') == -1)
            {
                if (p.canBuild) //Checks if player can build there
                {
                    switch (message.ToLower()) //If true - they can delete the zone
                    {
                        case "del":
                            p.zoneDel = true;
                            Player.SendMessage(p, "Place a block where you would like to delete a zone.");
                            return;
                        default:
                            Help(p);
                            return;
                    }
                }
                else //if they cant, it warns them, the ops and logs it on the server!
                {
                    Player.SendMessage(p, "You can't delete a zone which is above your rank!");
                    Player.GlobalMessageOps(p.name + " tried to delete a zone that is above their rank!");
                    Server.s.Log(p.name + " tried to delete a zone that is above their rank!");
                    return;
                }
            }


            if (message.ToLower() == "del all")
            {
                if ((int)p.group.Permission < CommandOtherPerms.GetPerm(this, 2))
                {
                    Player.SendMessage(p, "Only a " + Group.findPermInt(CommandOtherPerms.GetPerm(this, 2)).name + "+ may delete all zones at once");
                    return;
                }
                else
                {
                    for (int i = 0; i < p.level.ZoneList.Count; i++)
                    {
                        Level.Zone Zn = p.level.ZoneList[i];
                        Database.AddParams("@Owner", Zn.Owner);
                        Database.executeQuery("DELETE FROM `Zone" + p.level.name + "` WHERE Owner=@Owner AND SmallX='" + Zn.smallX + "' AND SMALLY='" + Zn.smallY + "' AND SMALLZ='" + Zn.smallZ + "' AND BIGX='" + Zn.bigX + "' AND BIGY='" + Zn.bigY + "' AND BIGZ='" + Zn.bigZ + "'");

                        Player.SendMessage(p, "Zone deleted for &b" + Zn.Owner);
                        p.level.ZoneList.Remove(p.level.ZoneList[i]);
                        if (i == p.level.ZoneList.Count) { Player.SendMessage(p, "Finished removing all zones"); return; }
                        i--;
                    }
                }
            }


            if ((int)p.group.Permission < CommandOtherPerms.GetPerm(this, 3))
            {
                Player.SendMessage(p, "Setting zones is reserved for " + Group.findPermInt(CommandOtherPerms.GetPerm(this, 3)).name); return;
            }

            if (Group.Find(message.Split(' ')[1]) != null)
            {
                message = message.Split(' ')[0] + " grp" + Group.Find(message.Split(' ')[1]).name;
            }

            if (message.Split(' ')[0].ToLower() == "add")
            {
                Player foundPlayer = Player.Find(message.Split(' ')[1]);
                if (foundPlayer == null)
                    cpos.Owner = message.Split(' ')[1].ToString();
                else
                    cpos.Owner = foundPlayer.name;
            }
            else { Help(p); return; }

            if (!Player.ValidName(cpos.Owner)) { Player.SendMessage(p, "INVALID NAME."); return; }

            cpos.x = 0; cpos.y = 0; cpos.z = 0; p.blockchangeObject = cpos;

            Player.SendMessage(p, "Place two blocks to determine the edges.");
            Player.SendMessage(p, "Zone for: &b" + cpos.Owner + ".");
            p.ClearBlockchange();
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/zone [add] [name] - Creates a zone only [name] can build in");
            Player.SendMessage(p, "/zone [add] [rank] - Creates a zone only [rank]+ can build in");
            Player.SendMessage(p, "/zone del - Deletes the zone clicked");
        }

        public void Blockchange1(Player p, ushort x, ushort y, ushort z, byte type)
        {
            p.ClearBlockchange();
            byte b = p.level.GetTile(x, y, z);
            p.SendBlockchange(x, y, z, b);
            CatchPos bp = (CatchPos)p.blockchangeObject;
            bp.x = x; bp.y = y; bp.z = z; p.blockchangeObject = bp;
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange2);
        }

        public void Blockchange2(Player p, ushort x, ushort y, ushort z, byte type)
        {
            p.ClearBlockchange();
            byte b = p.level.GetTile(x, y, z);
            p.SendBlockchange(x, y, z, b);
            CatchPos cpos = (CatchPos)p.blockchangeObject;

            Level.Zone Zn;

            Zn.smallX = Math.Min(cpos.x, x);
            Zn.smallY = Math.Min(cpos.y, y);
            Zn.smallZ = Math.Min(cpos.z, z);
            Zn.bigX = Math.Max(cpos.x, x);
            Zn.bigY = Math.Max(cpos.y, y);
            Zn.bigZ = Math.Max(cpos.z, z);
            Zn.Owner = cpos.Owner;

            p.level.ZoneList.Add(Zn);

            //DB
            Database.AddParams("@Owner", Zn.Owner);
            Database.executeQuery("INSERT INTO `Zone" + p.level.name + "` (SmallX, SmallY, SmallZ, BigX, BigY, BigZ, Owner) VALUES (" + Zn.smallX + ", " + Zn.smallY + ", " + Zn.smallZ + ", " + Zn.bigX + ", " + Zn.bigY + ", " + Zn.bigZ + ", @Owner)");
            //DB

            Player.SendMessage(p, "Added zone for &b" + cpos.Owner);
        }

        struct CatchPos { public ushort x, y, z; public string Owner; }
    }
}