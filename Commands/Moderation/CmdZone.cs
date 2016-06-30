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
            string[] args = message.Split(' ');
            if (message == "") {
                Player.Message(p, "Place a block where you would like to check for zones.");
                p.MakeSelection(1, null, CheckZone);
            } else if (args[0].CaselessEq("add")) {
                if (!CheckExtraPerm(p, 3)) { MessageNeedPerms(p, "can create zones.", 3); return; }
                if (args.Length == 1) { Help(p); return; }
                
                if (Group.Find(args[1]) != null)
                    args[1] = "grp" + Group.Find(args[1]).name;
                if (!ValidName(p,  args[1], "player or rank")) return;

                Player.Message(p, "Place two blocks to determine the edges.");
                Player.Message(p, "Zone for: &b" + args[1] + ".");
                p.MakeSelection(2, args[1], AddZone);
            } else if (args[0].CaselessEq("del") && args.Length > 1 && args[1].CaselessEq("all")) {
                if (!CheckExtraPerm(p, 2)) { MessageNeedPerms(p, "can delete all zones.", 2); return; }
                
                for (int i = 0; i < p.level.ZoneList.Count; i++) {
                    Level.Zone Zn = p.level.ZoneList[i];
                    LevelDB.DeleteZone(p.level.name, Zn);
                    Player.Message(p, "Zone deleted for &b" + Zn.Owner);
                    p.level.ZoneList.Remove(p.level.ZoneList[i]);
                    if (i == p.level.ZoneList.Count) { Player.Message(p, "Finished removing all zones"); return; }
                    i--;
                }
            } else if (args[0].CaselessEq("del")) {
                if (!CheckExtraPerm(p, 1)) { MessageNeedPerms(p, "can delete zones.", 1); return; }
                
                if (p.canBuild) { //Checks if player can build there
                    Player.Message(p, "Place a block where you would like to delete a zone.");
                    p.MakeSelection(1, null, DeleteZone);
                } else { //if they cant, it warns them, the ops and logs it on the server!
                    Player.Message(p, "You can't delete a zone which is above your rank!");
                    Chat.GlobalMessageOps(p.name + " tried to delete a zone that is above their rank!");
                    Server.s.Log(p.name + " tried to delete a zone that is above their rank!");
                }
            } else {
                Help(p);
            }
        }
        
        bool CheckZone(Player p, Vec3S32[] marks, object state, byte type, byte extType) {
            Level lvl = p.level;
            Vec3S32 P = marks[0];
            string owners = "";
            
            for (int i = 0; i < lvl.ZoneList.Count; i++) {
                Level.Zone zn = lvl.ZoneList[i];
                if (P.X < zn.smallX || P.X > zn.bigX || P.Y < zn.smallY || P.Y > zn.bigY || P.Z < zn.smallZ || P.Z > zn.bigZ)
                    continue;
                
                if (zn.Owner.Length >= 3 && zn.Owner.StartsWith("grp"))
                    owners += ", " + zn.Owner.Substring(3);
                else if (zn.Owner != "")
                    owners += ", " + zn.Owner;
            }
            
            if (owners.Length == 0) 
                Player.Message(p, "No zones affect this block.");
            else
                Player.Message(p, "This zone belongs to &b{0}.", owners.Remove(0, 2));
            return true;
        }
        

        bool DeleteZone(Player p, Vec3S32[] marks, object state, byte type, byte extType) {
            Level lvl = p.level;
            bool foundDel = false;
            Vec3S32 P = marks[0];
            
            for (int i = 0; i < lvl.ZoneList.Count; i++) {
                Level.Zone zn = lvl.ZoneList[i];
                if (P.X < zn.smallX || P.X > zn.bigX || P.Y < zn.smallY || P.Y > zn.bigY || P.Z < zn.smallZ || P.Z > zn.bigZ)
                    continue;
                
                if (zn.Owner.Length >= 3 && zn.Owner.StartsWith("grp")) {
                    string grpName = zn.Owner.Substring(3);
                    if (p.group.Permission < Group.Find(grpName).Permission) continue;
                } else if (zn.Owner != "" && !zn.Owner.CaselessEq(p.name)) {
                    Group group = Group.findPlayerGroup(zn.Owner);
                    if (p.group.Permission < group.Permission) continue;
                }
                
                LevelDB.DeleteZone(lvl.name, zn);
                lvl.ZoneList.RemoveAt(i); i--;
                Player.Message(p, "Zone deleted for &b" + zn.Owner);
                foundDel = true;
            }
            
            if (!foundDel) Player.Message(p, "No zones found to delete.");
            return false;
        }
        
        bool AddZone(Player p, Vec3S32[] marks, object state, byte type, byte extType) {
            Level.Zone Zn;
            Zn.smallX = (ushort)Math.Min(marks[0].X, marks[1].X);
            Zn.smallY = (ushort)Math.Min(marks[0].Y, marks[1].Y);
            Zn.smallZ = (ushort)Math.Min(marks[0].Z, marks[1].Z);
            Zn.bigX = (ushort)Math.Max(marks[0].X, marks[1].X);
            Zn.bigY = (ushort)Math.Max(marks[0].Y, marks[1].Y);
            Zn.bigZ = (ushort)Math.Max(marks[0].Z, marks[1].Z);
            Zn.Owner = (string)state;

            p.level.ZoneList.Add(Zn);
            LevelDB.CreateZone(p.level.name, Zn);
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
