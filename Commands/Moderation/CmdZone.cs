/*
    Copyright 2011 MCForge
        
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
using System;

namespace MCGalaxy.Commands {
    public sealed class CmdZone : Command {
        public override string name { get { return "zone"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public override CommandPerm[] AdditionalPerms {
            get { return new[] {
                    new CommandPerm(LevelPermission.Operator, "+ can delete zones"),
                    new CommandPerm(LevelPermission.Operator, "+ can delete all zones"),
                    new CommandPerm(LevelPermission.Operator, "+ can create zones"),
                }; }
        }

        public override void Use(Player p, string message) {
            string owner = null;
            if (message == "") {
                p.ZoneCheck = true;
                Player.Message(p, "Place a block where you would like to check for zones.");
                return;
            }

            if (message.IndexOf(' ') == -1) {
                if (!CheckExtraPerm(p, 1)) { MessageNeedPerms(p, "can delete zones.", 1); return; }
                if (p.canBuild) //Checks if player can build there
                {
                    switch (message.ToLower()) //If true - they can delete the zone
                    {
                        case "del":
                            p.zoneDel = true;
                            Player.Message(p, "Place a block where you would like to delete a zone.");
                            return;
                        default:
                            Help(p);
                            return;
                    }
                }
                else //if they cant, it warns them, the ops and logs it on the server!
                {
                    Player.Message(p, "You can't delete a zone which is above your rank!");
                    Chat.GlobalMessageOps(p.name + " tried to delete a zone that is above their rank!");
                    Server.s.Log(p.name + " tried to delete a zone that is above their rank!");
                    return;
                }
            }

            if (message.ToLower() == "del all") {
                if (!CheckExtraPerm(p, 2)) { MessageNeedPerms(p, "can delete all zones.", 2); return; }
                
                for (int i = 0; i < p.level.ZoneList.Count; i++) {
                    Level.Zone Zn = p.level.ZoneList[i];
                    Zones.Delete(p.level.name, Zn);
                    Player.Message(p, "Zone deleted for &b" + Zn.Owner);
                    p.level.ZoneList.Remove(p.level.ZoneList[i]);
                    if (i == p.level.ZoneList.Count) { Player.Message(p, "Finished removing all zones"); return; }
                    i--;
                }
            }


            if (!CheckExtraPerm(p, 3)) { MessageNeedPerms(p, "can create zones.", 3); return; }

            if (Group.Find(message.Split(' ')[1]) != null)
            {
                message = message.Split(' ')[0] + " grp" + Group.Find(message.Split(' ')[1]).name;
            }

            if (message.Split(' ')[0].ToLower() == "add")
            {
                Player foundPlayer = PlayerInfo.Find(message.Split(' ')[1]);
                if (foundPlayer == null)
                    owner = message.Split(' ')[1].ToString();
                else
                    owner = foundPlayer.name;
            }
            else { Help(p); return; }

            if (!ValidName(p, owner, "player or rank")) return;

            Player.Message(p, "Place two blocks to determine the edges.");
            Player.Message(p, "Zone for: &b" + owner + ".");
            p.MakeSelection(2, owner, DoZone);
        }
        
        bool DoZone(Player p, Vec3S32[] m, object state, byte type, byte extType) {
            Level.Zone Zn;
            Zn.smallX = (ushort)Math.Min(m[0].X, m[1].X);
            Zn.smallY = (ushort)Math.Min(m[0].Y, m[1].Y);
            Zn.smallZ = (ushort)Math.Min(m[0].Z, m[1].Z);
            Zn.bigX = (ushort)Math.Max(m[0].X, m[1].X);
            Zn.bigY = (ushort)Math.Max(m[0].Y, m[1].Y);
            Zn.bigZ = (ushort)Math.Max(m[0].Z, m[1].Z);
            Zn.Owner = (string)state;

            p.level.ZoneList.Add(Zn);
            Zones.Create(p.level.name, Zn);
            Player.Message(p, "Added zone for &b" + (string)state);
            return false;
        }
        
        public override void Help(Player p) {
            Player.Message(p, "/zone [add] [name] - Creates a zone only [name] can build in");
            Player.Message(p, "/zone [add] [rank] - Creates a zone only [rank]+ can build in");
            Player.Message(p, "/zone del - Deletes the zone clicked");
        }
    }
}
